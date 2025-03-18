using System;
using System.Collections.Generic;
using UnityEngine;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Base.Tools
{
    /// <summary>
    /// 跟踪实体接口
    /// </summary>
    public interface ITrackEntity
    {
        /// <summary>
        /// 当跟踪事件发生时回调
        /// </summary>
        /// <param name="trackEvent"></param>
        void OnTrackEvent(ETrackEvent trackEvent);
    }

    /// <summary>
    /// 跟踪实体接口泛型
    /// </summary>
    /// <typeparam name="TSessionRelativeData"></typeparam>
    /// <typeparam name="TTrackable"></typeparam>
    public interface ITrackEntity<TSessionRelativeData, TTrackable> : ITrackEntity
#if XDREAMER_AR_FOUNDATION
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
#endif
    {
#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        TrackableCollection<TTrackable> trackables { get; }


        /// <summary>
        /// 当跟踪关联时回调
        /// </summary>
        /// <param name="trackable">将要新关联的对象</param>
        void OnTrackLink(TTrackable trackable);

        /// <summary>
        /// 当跟踪取消关联时回调
        /// </summary>
        void OnTrackUnlink();

#endif
    }


    /// <summary>
    /// 跟踪数据
    /// </summary>
    public abstract class TrackData
    {
        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        public virtual bool DataValidity() => true;
    }

    /// <summary>
    /// 跟踪数据泛型
    /// </summary>
    /// <typeparam name="TSessionRelativeData"></typeparam>
    /// <typeparam name="TTrackable"></typeparam>
    public abstract class TrackData<TSessionRelativeData, TTrackable> : TrackData
#if XDREAMER_AR_FOUNDATION
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
#endif
    {
#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 上次的跟踪状态
        /// </summary>
        public TrackingState lastTrackingState { get; private set; } = TrackingState.None;

        /// <summary>
        /// 当前的跟踪状态
        /// </summary>
        public TrackingState trackingState { get; private set; } = TrackingState.None;

        /// <summary>
        /// 可跟踪对象
        /// </summary>
        public TTrackable trackable { get; private set; } = null;

        /// <summary>
        /// 是相同的可跟踪对象
        /// </summary>
        /// <param name="trackable"></param>
        /// <returns></returns>
        public virtual bool IsSameTrackable(TTrackable trackable)
        {
            return trackable && this.trackable && (this.trackable == trackable || this.trackable.trackableId == trackable.trackableId);
        }

        /// <summary>
        /// 尝试更新
        /// </summary>
        /// <param name="trackable"></param>
        /// <param name="trackEvent"></param>
        /// <param name="trackEntity"></param>
        /// <returns></returns>
        public virtual bool TryUpdate(TTrackable trackable, ETrackEvent trackEvent, ITrackEntity<TSessionRelativeData, TTrackable> trackEntity)
        {
            if (IsSameTrackable(trackable))
            {
                Update(trackable);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="trackable"></param>
        public virtual void Update(TTrackable trackable)
        {
            lastTrackingState = trackingState;
            this.trackable = trackable;
            trackingState = trackable ? trackable.trackingState : TrackingState.None;
        }

        /// <summary>
        /// 清空跟踪状态
        /// </summary>
        public virtual void ClearTrackingState()
        {
            Update(null);
        }

        /// <summary>
        /// 当跟踪对象变更时调用
        /// </summary>
        /// <param name="added"></param>
        /// <param name="updated"></param>
        /// <param name="removed"></param>
        public virtual void OnTrackablesChanged(List<TTrackable> added, List<TTrackable> updated, List<TTrackable> removed, ITrackEntity<TSessionRelativeData, TTrackable> trackEntity)
        {
#if XDREAMER_AR_FOUNDATION
            foreach (var trackable in added)
            {
                if (TryUpdate(trackable, ETrackEvent.OnAdded, trackEntity))
                {
                    trackEntity.OnTrackLink(trackable);
                    trackEntity.OnTrackEvent(ETrackEvent.OnAdded);
                    break;
                }
            }

            foreach (var trackable in updated)
            {
                if (TryUpdate(trackable, ETrackEvent.OnUpdated, trackEntity))
                {
                    trackEntity.OnTrackEvent(ETrackEvent.OnUpdated);
                    XARFoundationHelper.HandleTrackerEvent(lastTrackingState, trackingState, trackEntity.OnTrackEvent);
                    break;
                }
            }

            foreach (var trackable in removed)
            {
                if (TryUpdate(trackable, ETrackEvent.OnRemoved, trackEntity))
                {
                    trackEntity.OnTrackUnlink();
                    trackEntity.OnTrackEvent(ETrackEvent.OnRemoved);
                    break;
                }
            }
#endif
        }
#endif
    }
}
