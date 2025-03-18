using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.TimeLine;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 时间轴可播放内容
    /// </summary>
    [Name("时间轴可播放内容")]
    [Tool(TimelineManager.ToolLib, rootType = typeof(TimelineManager), purposes = new string[] {  TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class TimeLinePlayableContent : PlayableContentHostPlayer
    {
        /// <summary>
        /// 时间轴播放内容
        /// </summary>
        [Name("时间轴播放内容")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup]
        public TimeLinePlayContent _timeLinePlayContent;

        /// <summary>
        /// 时间轴播放内容
        /// </summary>
        public TimeLinePlayContent timeLinePlayContent
        {
            get
            {
                if (!_timeLinePlayContent)
                {
                    // 查找当前活跃的播放器
                    _timeLinePlayContent = SMSHelper.GetStateComponents<TimeLinePlayContent>().Find(p => p.parent.active);
                }
                return _timeLinePlayContent;
            }
        }

        /// <summary>
        /// 时间长度
        /// </summary>
        public override double timeLength { get => _timeLinePlayContent.timeLength; }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            timeLinePlayContent.progress = percent.percent01OfWorkCurve;
        }

        /// <summary>
        /// 缺省加载内容
        /// </summary>
        protected override IPlayableContent loadContentOnEnable => _timeLinePlayContent; 
    }
}
