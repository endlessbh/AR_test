using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.TimeLine
{
    /// <summary>
    /// 状态工作剪辑集合：用于管理子状态机内的其他具有工作剪辑接口状态
    /// </summary>
    [ComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StateWorkClipSet))]
    [Tip("用于管理子状态机内的其他具有工作剪辑接口状态", "It is used to manage other interface states with working clips in the sub state machine")]
    [XCSJ.Attributes.Icon(index = 33657)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    public class StateWorkClipSet : WorkClip<StateWorkClipSet>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态工作剪辑集合";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("时间轴", typeof(SMSManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu("时间轴/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateWorkClipSet))]
        [Tip("用于管理子状态机内的其他具有工作剪辑接口状态", "It is used to manage other interface states with working clips in the sub state machine")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(StateWorkClipSet)), null, typeof(StateWorkClipSet));
        }

        [Name("状态机连线顺序")]
        [Tip("以状态机'进入'节点为起始点，查找生成一条有工作剪辑型状态组件的状态列表", "Take the 'entry' node of the state machine as the starting point to find and generate a state list with working clip state components")]
        public bool SMSequence = true;

        [HideInInspector]
        [SerializeField]
        public List<StateWorkClip> stateWorkClips = new List<StateWorkClip>();

        public StateCollection stateCollection => parent as StateCollection;

        /// <summary>
        /// 是否模拟
        /// </summary>
        private bool needSimulateRunning = true;

        /// <summary>
        /// 模拟运行，对移动，旋转等数据进行初始化
        /// </summary>
        private void SimulateRunWithPercent(StateData stateData)
        {
            // 禁用工作剪辑触发器，防止在预演中触发动作
            stateWorkClips.ForEach(swc => swc.ValidTriggerPoint(false));

            // 试用动画片段的时间节点作为关键点，对动画进行预演
            List<double> keyPoints = new List<double>();
            foreach (var clip in stateWorkClips)
            {
                var percentRange = clip.workRange.percentRange.percentRange;
                if (!keyPoints.Exists(p => MathX.Approximately(p, percentRange.x)))
                {
                    keyPoints.Add(percentRange.x);
                }
                if (!keyPoints.Exists(p => MathX.Approximately(p, percentRange.y)))
                {
                    keyPoints.Add(percentRange.y);
                }
            }
            keyPoints.Sort((a, b) => a.CompareTo(b));

            // 开始配置
            lastPercent = 0;
            foreach (var percent in keyPoints)
            {
                foreach (var info in stateWorkClips)
                {
                    if (info.workRange.percentRange.In(percent))
                    {
                        if (MathX.Approximately(percent, info.beginPercent))
                        {
                            //Debug.Log("info.OnEntry:" + info.name);
                            info.OnEntrySetPercent(percent, stateData);
                        }
                        else if (MathX.Approximately(percent, info.endPercent))
                        {
                            //Debug.Log("info.OnExit:" + info.name);
                            info.OnExitSetPercent(percent, stateData);
                        }
                        else
                        {
                            //Debug.Log("info.Update:" + info.name);
                            info.SetPercentOfState(percent, stateData);
                        }
                    }
                }
            }

            // 重置动画到初始位置
            lastPercent = endPercent;
            SetPercent(0, stateData);

            // 重新启用触发器
            stateWorkClips.ForEach(swc => swc.ValidTriggerPoint(true));
        }

        public void ResetTimeLength(double timeLength)
        {
            this.timeLength = timeLength;
            stateWorkClips.ForEach(info =>
            {
                info.ResetTimeLength(timeLength);
            });
        }

        #region ILifcyle

        public override bool Init(StateData data)
        {
            UpdateData();
            return base.Init(data);
        }

        /// <summary>
        /// 屏蔽基类更新
        /// </summary>
        public override void OnUpdate(StateData data) { }

        public override bool Finished() => true;

        #endregion

        #region IPercentClipHandle

        /// <summary>
        /// 当工作剪辑播放器的播放状态发生变化时回调
        /// </summary>
        /// <param name="player">工作剪辑播放器对象</param>
        /// <param name="lastPlayerState">上次的工作剪辑播放器的播放状态</param>
        public override void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState)
        {
            base.OnPlayerStateChanged(player, lastPlayerState);

            foreach (var clip in stateWorkClips)
            {
                clip.OnPlayerStateChanged(player, lastPlayerState);
            }
        }

        public bool SetPercent(State state)
        {
            if (TryGetPercentOfState(state, out double percent))
            {
                return SetPercentOfState(percent);
            }
            return false;
        }

        private double lastPercent = 0;

        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (needSimulateRunning)
            {
                needSimulateRunning = false;
                var workMode = parent.workMode;
                try
                {
                    SimulateRunWithPercent(StateData.Clone(stateData, parent.workMode = EWorkMode.Simulate));
                }
                finally
                {
                    parent.workMode = workMode;
                }
            }

            var p = percent.percentOfWorkCurve;
            double percentOffset = p - lastPercent;

            List<StateWorkClip> beforeRanges = new List<StateWorkClip>();
            List<StateWorkClip> curRanges = new List<StateWorkClip>();
            List<StateWorkClip> afterRanges = new List<StateWorkClip>();

            // 比较百分比区间
            foreach (var clip in stateWorkClips)
            {
                if (clip.workRange.percentRange.In(p))
                {
                    curRanges.Add(clip);
                }
                else if (clip.workRange.percentRange.Right(p))
                {
                    beforeRanges.Add(clip);
                }
                else
                {
                    afterRanges.Insert(0, clip);
                }
            }

            // 从前到后
            if (percentOffset >= 0)
            {
                // 使用区间与lastTime比较
                // lastTime  |-----------| time
                // before  |---|
                // in         |---|
                // in             |---|
                // after             |---|
                foreach (var clip in beforeRanges)
                {
                    if (clip.workRange.percentRange.Right(lastPercent))
                    {
                        continue;
                    }
                    else
                    {
                        // 使用大值来设置
                        clip.SetPercentOfState(p, stateData);
                        // 回调右越界事件
                        clip.OnOutOfBounds(workClipPlayer, EOutOfBoundsMode.Right, p, stateData, lastPercent, clip);
                    }
                }
            }
            // 从后到前
            else
            {
                // 使用区间与lastTime比较
                // time  |-----------| lastTime
                // before  |---|
                // in            |---|
                // in                |---|
                // after                |---|
                foreach (var clip in afterRanges)
                {
                    if (clip.workRange.percentRange.Left(lastPercent))
                    {
                        continue;
                    }
                    else
                    {
                        // 使用小值来设置
                        clip.SetPercentOfState(p, stateData);
                        // 回调左越界事件
                        clip.OnOutOfBounds(workClipPlayer, EOutOfBoundsMode.Left, p, stateData, lastPercent, clip);
                    }
                }
            }

            // 设置当前片段动画
            foreach (var clip in curRanges)
            {
                if (clip.workRange.percentRange.Left(lastPercent))
                {
                    clip.OnOutOfBounds(workClipPlayer, EOutOfBoundsMode.LeftToIn, p, stateData, lastPercent, clip);
                }
                else if (clip.workRange.percentRange.Right(lastPercent))
                {
                    clip.OnOutOfBounds(workClipPlayer, EOutOfBoundsMode.RightToIn, p, stateData, lastPercent, clip);
                }
                clip.SetPercentOfState(p, stateData);
            }

            lastPercent = p;
        }

        #endregion

        public void UpdateData()
        {
            if (SMSequence)
            {
                stateWorkClips = WorkClipHelper.CreateStateWorkClips(stateCollection.entryState as State);

                timeLength = 0;
                stateWorkClips.ForEach(clip =>
                {
                    if (clip.endTime > timeLength)
                    {
                        timeLength = clip.endTime;
                    }
                });
            }

            // 排除空对象
            stateWorkClips = stateWorkClips.Where(clip => clip.state).ToList();
        }

        #region 状态与百分比互相定位

        public bool TryGetPercentOfState(State state, out double percent)
        {
            if (TryGetPercent(state, out percent))
            {
                percent = percent * oncePercentLength + beginPercent;
                return true;
            }
            percent = 0;
            return false;
        }

        public bool TryGetPercent(State state, out double percent)
        {
            if (!state || this.parent == state)
            {
                percent = 0;
                return false;
            }

            double tmpPercent = 0;
            if (StateToPercent(state, ref tmpPercent))
            {
                percent = tmpPercent;
                return true;
            }
            percent = 0;
            return false;
        }

        public List<State> GetStates(double percent)
        {
            List<State> activeState = new List<State>();
            PercentToStates(percent, activeState);
            return activeState;
        }

        /// <summary>
        /// 通过工作剪辑所在状态，查找对应百分比
        /// 因为工作剪辑是进度，而工作剪辑又依附于状态，所以可以转换为百分比
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>是否成功返回</returns>
        public virtual bool StateToPercent(State state, ref double percent)
        {
            if (this.parent == state)
            {
                percent = beginPercent;
                return true;
            }
            else
            {
                foreach (var clip in stateWorkClips)
                {
                    if (clip.StateToPercent(state, ref percent))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void PercentToStates(double percent, List<State> outStates)
        {
            if (workRange.percentRange.In(percent))
            {
                outStates.Add(this.parent);

                foreach (var clip in stateWorkClips)
                {
                    clip.PercentToState(percent, outStates);
                }
            }
        }

        #endregion
    }
}