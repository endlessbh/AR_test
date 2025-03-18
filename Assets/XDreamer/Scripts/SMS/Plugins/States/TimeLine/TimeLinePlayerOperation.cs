using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放器操作:时间轴播放器操作组件是控制播放器的播放、暂停和停止的对象。控制操作完成之后，切换为完成态。
    /// </summary>
    [ComponentMenu("时间轴/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(TimeLinePlayerOperation))]
    [Tip("时间轴播放器操作组件是控制播放器的播放、暂停和停止的对象。控制操作完成之后，切换为完成态。", "The timeline player operation component is an object that controls the playback, pause and stop of the player. After the control operation is completed, switch to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33659)]
    [DisallowMultipleComponent]
    public class TimeLinePlayerOperation : LifecycleExecutor<TimeLinePlayerOperation>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "时间轴播放器操作";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(SMSManager))]
        [StateComponentMenu("时间轴/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(TimeLinePlayerOperation))]
        [Tip("时间轴播放器操作组件是控制播放器的播放、暂停和停止的对象。控制操作完成之后，切换为完成态。", "The timeline player operation component is an object that controls the playback, pause and stop of the player. After the control operation is completed, switch to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateTimeLinePlayerController(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("播放器")]
        [StateComponentPopup(typeof(TimeLinePlayer))]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public TimeLinePlayer timeLinePlayer = null;

        [Name("进入播放控制")]
        [EnumPopup]
        public EPlayControl playControl = EPlayControl.Play;

        [Name("播放内容")]
        [HideInSuperInspector(nameof(playControl), EValidityCheckType.NotEqual, EPlayControl.SetContent,
            nameof(playControl), EValidityCheckType.NotEqual, EPlayControl.SetContentAndPlay)]
        public TimeLinePlayContent playContent;

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            SetPlayer(playControl);
        }

        private void SetPlayer(EPlayControl playControl)
        {
            if (!timeLinePlayer) return;

            switch (playControl)
            {
                case EPlayControl.Play:
                    {
                        timeLinePlayer.Play();
                        break;
                    }
                case EPlayControl.Pause:
                    {
                        timeLinePlayer.Pause();
                        break;
                    }
                case EPlayControl.Replay:
                    {
                        timeLinePlayer.Replay();
                        break;
                    }
                case EPlayControl.SetContent:
                    {
                        timeLinePlayer.SetPlayContent(playContent);
                        break;
                    }
                case EPlayControl.SetContentAndPlay:
                    {
                        timeLinePlayer.SetPlayContent(playContent);
                        timeLinePlayer.Play();
                        break;
                    }
                case EPlayControl.SwitchLoop:
                    {
                        timeLinePlayer.isLoop = !timeLinePlayer.isLoop;
                        break;
                    }
            }
        }

        public override bool DataValidity()
        {
            return timeLinePlayer;
        }

        public override string ToFriendlyString()
        {
            return (timeLinePlayer ? timeLinePlayer.parent.name : "") + "." + CommonFun.Name(playControl);
        }
    }

    [Name("播放操作")]
    public enum EPlayControl
    {
        [Name("无")]
        None,

        [Name("播放")]
        Play,

        [Name("暂停")]
        Pause,

        [Name("重播")]
        Replay,

        [Name("设置播放内容")]
        SetContent,

        [Name("设置播放内容并播放")]
        SetContentAndPlay,

        [Name("切换循环")]
        SwitchLoop,

        [Name("循环开启")]
        LoopOn,

        [Name("循环关闭")]
        LoopOff,
    }
}
