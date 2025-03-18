using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 时间轴播放内容:被播放器所播放，用于管理子状态机内的其他具有工作剪辑接口状态
    /// </summary>
    [ComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(TimeLinePlayContent))]
    [Tip("被播放器所播放，用于管理子状态机内的其他具有工作剪辑接口状态", "It is played by the player and used to manage other states with working clip interface in the sub state machine")]
    [XCSJ.Attributes.Icon(index = 33658)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    public class TimeLinePlayContent : StateWorkClipSet, IPlayableContent
    {
        /// <summary>
        /// 标题
        /// </summary>
        public new const string Title = "时间轴播放内容";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(SMSManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(TimeLinePlayContent))]
        [Tip("时间轴播放内容组件是组织和管理状态机内的工作剪辑的对象。只在状态机上使用，可被播放器播放。状态机退出之后，切换为完成态。", "The timeline playback content component is the object of organizing and managing working clips in the state machine. It is only used on the state machine and can be played by the player. After the state machine exits, it switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateTimeLinePlayContent(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(TimeLinePlayContent)), null, typeof(TimeLinePlayContent));
        }

        public double GetTimeLength()
        {
            UpdateData();
            return timeLength;
        }

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            OnSetPercent(Percent.Zero, data);
        }

        /// <summary>
        /// 当有的新的播放内容元素时回调
        /// </summary>
        public event Action<TimeLinePlayContent,List<State>, State, double> onNewPlayContentElement;

        /// <summary>
        /// 当播放内容元素发生变化时回调
        /// </summary>
        public event Action<TimeLinePlayContent, List<State>, List<State>, double> onPlayContentElementChanged;

        public event Action<TimeLinePlayContent> onPlay;

        public event Action<TimeLinePlayContent> onStop;

        public void OnPlay()
        {
            lastPlayState = new List<State>();

            onPlay?.Invoke(this);
        }

        public void OnStop()
        {
            lastPlayState.ForEach(s => s.workMode = EWorkMode.Default);

            onStop?.Invoke(this);
        }

        public void PlayContentElements(params State[] states)
        {
            if (states == null || states.Length == 0) return;

            List<KeyValuePair<State, double>> list = new List<KeyValuePair<State, double>>();
            foreach (var state in states)
            {
                if(TryGetPercentOfState(state, out double p))
                {
                    list.Add(new KeyValuePair<State, double>(state, p));
                }
            }

            if (list.Count == 0) return;
            list.Sort((x, y) => x.Value.CompareTo(y.Value));

            // 百分比值最高的对象
            var last = list.Last();
            PlayContent(last.Value);

            onNewPlayContentElement?.Invoke(this, lastPlayState, last.Key, last.Value);
        }

        public List<State> lastPlayState { get; private set; } = new List<State>();

        public bool PlayContent(double percent, StateData stateData = null)
        {
            try
            {
                return SetPercentOfState(percent, stateData);
            }
            finally
            {
                List<State> newPlayStates = GetStates(percent);

                // 简单比较新旧元素是否发生变化
                if (lastPlayState.Count != newPlayStates.Count || !lastPlayState.All(newPlayStates.Contains))
                {
                    onPlayContentElementChanged?.Invoke(this, lastPlayState, newPlayStates, percent);

                    lastPlayState.ForEach(s => s.workMode = EWorkMode.Default);
                    lastPlayState = newPlayStates;
                }
                newPlayStates.ForEach(s => s.workMode = EWorkMode.Play);
            }
        }

        #region IPlayableContent

        public IPlayableContentHost host { get; set; }

        double IPercent.percent { get => progress; set => progress = value; }

        public IContentPlayer player { get; set; }

        /// <summary>
        /// 当内容加载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnLoad(PlayableData playableData)
        {
            host = playableData.playableContentHost;
            player = playableData.player;

            return EInteractResult.Finished;
        }

        /// <summary>
        /// 当内容卸载
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnUnload(PlayableData playableData)
        {
            if (host == playableData.playableContentHost)
            {
                host = null;
            }
            if (player == playableData.player)
            {
                player = null;
            }
            return EInteractResult.Finished;
        }

        /// <summary>
        /// 当播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnPlay(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当暂停
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnPause(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当恢复播放
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnResume(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当停止
        /// </summary>
        /// <param name="playableData"></param>
        /// <returns></returns>
        public virtual EInteractResult OnStop(PlayableData playableData) => EInteractResult.Finished;

        /// <summary>
        /// 当设置百分比
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="playableData"></param>
        public void OnSetPercent(Percent percent, PlayableData playableData) { }

        #endregion
    }
}
