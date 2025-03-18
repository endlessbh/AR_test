using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXAR.Foundation.Base.Tools;

namespace XCSJ.PluginXAR.Foundation.Base.States
{
    /// <summary>
    /// 跟踪器事件
    /// </summary>
    [Name(Title)]
    [Owner(typeof(XARFoundationManager))]
    public class TrackerEvent : Trigger<TrackerEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "跟踪器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(TrackerEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(XARFoundationHelper.Title, typeof(XARFoundationManager))]
        [StateComponentMenu(XARFoundationHelper.Title + "/" + Title, typeof(XARFoundationManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 跟踪器
        /// </summary>
        [Name("跟踪器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseTracker _tracker;

        /// <summary>
        /// 跟踪事件
        /// </summary>
        [Name("跟踪事件")]
        [EnumPopup]
        public ETrackEvent _trackEvent;

        /// <summary>
        /// 进入时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            BaseTracker.onTrackerChanged += OnTrackerChanged;
        }

        /// <summary>
        /// 退出时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            BaseTracker.onTrackerChanged -= OnTrackerChanged;
        }

        private void OnTrackerChanged(BaseTracker tracker, ETrackEvent trackEvent)
        {
            if (finished) return;
            if (tracker != this._tracker) return;
            if (trackEvent != this._trackEvent) return;

            finished = true;
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (_tracker ? _tracker.name : "") + "." + CommonFun.Name(_trackEvent);
        }
    }
}
