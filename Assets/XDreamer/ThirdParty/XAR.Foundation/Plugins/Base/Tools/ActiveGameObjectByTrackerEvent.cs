using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.PluginCommonUtils.Tools;

#if XDREAMER_AR_FOUNDATION
#endif

namespace XCSJ.PluginXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 激活游戏对象通过跟踪器事件
    /// </summary>
    [Name("激活游戏对象通过跟踪器事件")]
    [Tool(XARFoundationHelper.Title, nameof(BaseTracker))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    public class ActiveGameObjectByTrackerEvent : BaseARMB
    {
        /// <summary>
        /// 跟踪器
        /// </summary>
        [Name("跟踪器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseTracker _tracker;

        /// <summary>
        /// 跟踪器
        /// </summary>
        public BaseTracker tracker => this.XGetComponentInParent(ref _tracker);

        /// <summary>
        /// 激活游戏对象信息列表
        /// </summary>
        [Name("激活游戏对象信息列表")]
        public List<ActiveGameObjectInfoList> _infoLists = new List<ActiveGameObjectInfoList>();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            BaseTracker.onTrackerChanged += OnTrackerChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            BaseTracker.onTrackerChanged -= OnTrackerChanged;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (!tracker) { }
        }

        private void OnTrackerChanged(BaseTracker tracker, ETrackEvent trackEvent)
        {
            if (tracker != this.tracker) return;

            Handle(trackEvent);
        }

        private void Handle(ETrackEvent trackEvent)
        {
            foreach (var infos in _infoLists)
            {
                infos.Active(trackEvent);
            }
        }

        /// <summary>
        /// 激活游戏对象信息列表
        /// </summary>
        [Name("激活游戏对象信息列表")]
        [Serializable]
        public class ActiveGameObjectInfoList : ActiveGameObjectInfoList<ETrackEvent>
        {
        }
    }
}
