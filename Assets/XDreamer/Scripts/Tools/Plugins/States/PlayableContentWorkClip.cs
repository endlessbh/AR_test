using System;
using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTimelines;

namespace XCSJ.PluginTools.States
{
    /// <summary>
    /// 可播放内容工作剪辑：通过状态中的工作剪辑来控制可播放内容Mono组件
    /// </summary>
    [Name(Title, nameof(PlayableContentWorkClip))]
    [XCSJ.Attributes.Icon(EIcon.Play)]
    [ComponentMenu(TimelineManager.Title + "/" + Title, typeof(ToolsManager))]
    public class PlayableContentWorkClip : WorkClip<PlayableContentWorkClip>, IPlayableContentHost
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "可播放内容工作剪辑";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(PlayableContentWorkClip))]
        [XCSJ.Attributes.Icon(EIcon.Play)]
        [StateLib(TimelineManager.Title, typeof(ToolsManager))]
        [StateComponentMenu(TimelineManager.Title + "/" + Title, typeof(ToolsManager))]
        public static State CreateInteractEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 可播放内容
        /// </summary>
        [Name("可播放内容")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public PlayableContent _playableContent;

        /// <summary>
        /// 宿主播放器
        /// </summary>
        public IContentPlayer hostPlayer => workClipPlayer as IContentPlayer;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if(_playableContent) _playableContent.Load(_playableContent, this);
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (_playableContent) _playableContent.Unload(_playableContent, this);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _playableContent;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _playableContent? _playableContent.name:"";

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (_playableContent && _playableContent.isLoaded)
            {
                _playableContent.percent = percent.percent01OfWorkCurve;
            }
        }
    }
}

