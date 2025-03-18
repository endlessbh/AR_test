using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Collections.Generic;
using XCSJ.PluginSMS.Base;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Interactions.Tools;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Base.Tools
{
    /// <summary>
    /// 基础跟踪器
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class BaseTracker : BaseARMB, ITrackEntity
    {
        #region 变换处理

        /// <summary>
        /// 当前变换
        /// </summary>
        public Transform thisTransform { get; private set; }

        /// <summary>
        /// 变换同步规则
        /// </summary>
        [Name("变换同步规则")]
        public enum ETransformSyncRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 位置：基于世界坐标执行同步
            /// </summary>
            [Name("位置")]
            [Tip("基于世界坐标执行同步", "Perform synchronization based on world coordinates")]
            Position,

            /// <summary>
            /// 旋转：基于世界坐标执行同步
            /// </summary>
            [Name("旋转")]
            [Tip("基于世界坐标执行同步", "Perform synchronization based on world coordinates")]
            Rotation,

            /// <summary>
            /// 位置与旋转：基于世界坐标执行同步
            /// </summary>
            [Name("位置与旋转")]
            [Tip("基于世界坐标执行同步", "Perform synchronization based on world coordinates")]
            PositionAndRotation,

            /// <summary>
            /// 作为子级
            /// </summary>
            [Name("作为子级")]
            AsChild,
        }

        /// <summary>
        /// 变换同步规则
        /// </summary>
        [Name("变换同步规则")]
        [EnumPopup]
        public ETransformSyncRule _transformSyncRule = ETransformSyncRule.AsChild;

        /// <summary>
        /// 变换同步规则
        /// </summary>
        public ETransformSyncRule transformSyncRule
        {
            get => _transformSyncRule;
            set => this.XModifyProperty(ref _transformSyncRule, value);
        }

        /// <summary>
        /// 同步变换
        /// </summary>
        /// <param name="dstTransform"></param>
        protected void SyncTransform(Transform dstTransform)
        {
#if XDREAMER_AR_FOUNDATION
            switch (_transformSyncRule)
            {
                case ETransformSyncRule.Position:
                    {
                        thisTransform.position = dstTransform.position;
                        break;
                    }
                case ETransformSyncRule.Rotation:
                    {
                        thisTransform.rotation = dstTransform.rotation;
                        break;
                    }
                case ETransformSyncRule.PositionAndRotation:
                    {
                        thisTransform.position = dstTransform.position;
                        thisTransform.rotation = dstTransform.rotation;
                        break;
                    }
            }
#endif
        }

        private HierarchyTransformRecorder hierarchyTransformRecorder = new HierarchyTransformRecorder();

        /// <summary>
        /// 记录变换
        /// </summary>
        /// <param name="dstParentTransform"></param>
        public void RecordTransform(Transform dstParentTransform)
        {
            if(!hierarchyTransformRecorder.HasRecord())
            {
                hierarchyTransformRecorder.Record(thisTransform);
            }

            if (_transformSyncRule == ETransformSyncRule.AsChild)
            {
                thisTransform.parent = dstParentTransform;
                thisTransform.localPosition = Vector3.zero;
                thisTransform.localRotation = Quaternion.identity;
                thisTransform.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// 恢复变换
        /// </summary>
        public void RecoverTransform() => hierarchyTransformRecorder.Recover();

        #endregion

        #region MonoBehaviour函数

        /// <summary>
        /// 唤醒
        /// </summary>
        public virtual void Awake()
        {
            thisTransform = transform;
        }

        /// <summary>
        /// 本帧需要更新的标记量
        /// </summary>
        protected bool needUpdateThisFrame = false;

        /// <summary>
        /// 当更新时
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (needUpdateThisFrame)
            {
                OnUpdate();
                needUpdateThisFrame = false;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() { }

        #endregion

        #region 事件

        /// <summary>
        /// 当跟踪事件发生时回调
        /// </summary>
        /// <param name="trackEvent"></param>
        public virtual void OnTrackEvent(ETrackEvent trackEvent)
        {
            needUpdateThisFrame = true;
            onTrackerChanged?.Invoke(this, trackEvent);
        }

        /// <summary>
        /// 跟踪器变更事件
        /// </summary>
        public static Action<BaseTracker, ETrackEvent> onTrackerChanged;

        #endregion
    }

    /// <summary>
    /// 基础跟踪器
    /// </summary>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseTracker<TTrackData> : BaseTracker
        where TTrackData : TrackData
    {
        /// <summary>
        /// 跟踪数据
        /// </summary>
        public abstract TTrackData trackData { get; }

        /// <summary>
        /// 当更新时
        /// </summary>
        protected override void OnUpdate() { }
    }

    /// <summary>
    /// 基础主跟踪器
    /// </summary>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseMainTracker<TTrackData> : BaseTracker<TTrackData>
        where TTrackData : TrackData
    {

    }

    /// <summary>
    /// 基础子跟踪器
    /// </summary>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseSubTracker<TMainTracker, TTrackData> : BaseTracker<TTrackData>
        where TMainTracker : BaseMainTracker<TTrackData>
        where TTrackData : TrackData
    {
        /// <summary>
        /// 主跟踪器
        /// </summary>
        public abstract TMainTracker mainTracker { get; }
    }

#if XDREAMER_AR_FOUNDATION

    /// <summary>
    /// 基础主跟踪器
    /// </summary>
    /// <typeparam name="TSessionRelativeData"></typeparam>
    /// <typeparam name="TTrackable"></typeparam>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseMainTracker<TSessionRelativeData, TTrackable, TTrackData> : BaseMainTracker<TTrackData>, ITrackEntity<TSessionRelativeData, TTrackable>
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
        public virtual void OnTrackLink(TTrackable trackable) => RecordTransform(trackable.transform);

        /// <summary>
        /// 当跟踪取消关联时回调
        /// </summary>
        public virtual void OnTrackUnlink() => RecoverTransform();

        /// <summary>
        /// 当更新时
        /// </summary>
        protected override void OnUpdate()
        {
            //base.OnUpdate();
            SyncTransform(trackable.transform);
        }
    }

    /// <summary>
    /// 基础子跟踪器
    /// </summary>
    /// <typeparam name="TMainTracker"></typeparam>
    /// <typeparam name="TSessionRelativeData"></typeparam>
    /// <typeparam name="TTrackable"></typeparam>
    /// <typeparam name="TTrackData"></typeparam>
    public abstract class BaseSubTracker<TMainTracker, TSessionRelativeData, TTrackable, TTrackData> : BaseSubTracker<TMainTracker, TTrackData>
        where TMainTracker : BaseMainTracker<TSessionRelativeData, TTrackable, TTrackData>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        where TTrackData : TrackData<TSessionRelativeData, TTrackable>
    {
        /// <summary>
        /// 跟踪的对象
        /// </summary>
        public TTrackable trackable => trackData.trackable;
    }

#endif

    /// <summary>
    /// 基础跟踪器可视化工具
    /// </summary>
    public abstract class BaseTrackerVisualizer : BaseARMB { }

    /// <summary>
    /// 基础AR组件
    /// </summary>
    [RequireManager(typeof(XARFoundationManager))]
    [Owner(typeof(XARFoundationManager))]
    public abstract class BaseARMB : Interactor { }
}
