using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using System.Collections.Generic;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Base.States
{
    /// <summary>
    /// 基础检测器
    /// </summary>
    /// <typeparam name="TDetector"></typeparam>
    [Owner(typeof(XARFoundationManager))]
    public class BaseDetector<TDetector> : Trigger<TDetector>, ITrackEntity
        where TDetector : BaseDetector<TDetector>
    {
        /// <summary>
        /// 跟踪事件
        /// </summary>
        [Name("跟踪事件")]
        [EnumPopup]
        public ETrackEvent _trackEvent = ETrackEvent.OnAdded;

        /// <summary>
        /// 当跟踪事件发生时回调
        /// </summary>
        /// <param name="trackEvent"></param>
        public virtual void OnTrackEvent(ETrackEvent trackEvent)
        {
            if (_trackEvent == trackEvent)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_trackEvent);
    }

    /// <summary>
    /// 基础检测器
    /// </summary>
    /// <typeparam name="TDetector"></typeparam>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseDetector<TDetector, TTrackData> : BaseDetector<TDetector>
        where TDetector : BaseDetector<TDetector>
        where TTrackData : TrackData
    {
        /// <summary>
        /// 跟踪数据
        /// </summary>
        public abstract TTrackData trackData { get; }

        /// <summary>
        /// 数据有效性判断
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => trackData.DataValidity();
    }

#if XDREAMER_AR_FOUNDATION

    /// <summary>
    /// 基础检测器
    /// </summary>
    /// <typeparam name="TDetector"></typeparam>
    /// <typeparam name="TSessionRelativeData"></typeparam>
    /// <typeparam name="TTrackable"></typeparam>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseDetector<TDetector, TSessionRelativeData, TTrackable, TTrackData> : BaseDetector<TDetector, TTrackData>, ITrackEntity<TSessionRelativeData, TTrackable>
        where TDetector : BaseDetector<TDetector>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        where TTrackData : TrackData<TSessionRelativeData, TTrackable>
    {
        /// <summary>
        /// 跟踪的对象
        /// </summary>
        public TTrackable trackable => trackData.trackable;

        /// <summary>
        /// 可跟踪对象集
        /// </summary>
        public abstract TrackableCollection<TTrackable> trackables { get; }

        /// <summary>
        /// 当可跟踪对象发生变换时回调
        /// </summary>
        /// <param name="added"></param>
        /// <param name="updated"></param>
        /// <param name="removed"></param>
        protected virtual void OnTrackablesChanged(List<TTrackable> added, List<TTrackable> updated, List<TTrackable> removed) => trackData.OnTrackablesChanged(added, updated, removed, this);

        /// <summary>
        /// 当跟踪关联时回调
        /// </summary>
        /// <param name="trackable">将要新关联的对象</param>
        public virtual void OnTrackLink(TTrackable trackable) { }

        /// <summary>
        /// 当跟踪取消关联时回调
        /// </summary>
        public virtual void OnTrackUnlink() { }
    }

#endif
}
