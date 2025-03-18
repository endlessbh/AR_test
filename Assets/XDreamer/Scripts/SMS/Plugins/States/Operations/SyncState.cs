using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Operations
{
    [Name("同步状态类型")]
    public enum ESyncStateType
    {
        [Name("启用入状态")]
        EnableInState,

        [Name("所有入状态")]
        AllInState,

        [Name("启用入跳转")]
        ActiveInTransition,

        [Name("所有入跳转")]
        AllInTransition
    }

    /// <summary>
    /// 同步状态:同步状态组件是等待连入的多个状态完成的执行体。当连入的多个状态都切换为完成态，组件本身才切换为完成态。
    /// </summary>
    [ComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(SyncState))]
    [Tip("同步状态组件是等待连入的多个状态完成的执行体。当连入的多个状态都切换为完成态，组件本身才切换为完成态。", "The synchronization state component is the execution body waiting for the completion of multiple connected states. When multiple connected states are switched to the completed state, the component itself is switched to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Merge)]
    public class SyncState : StateComponent<SyncState>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "同步状态";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(SyncState))]
        [Tip("同步状态组件是等待连入的多个状态完成的执行体。当连入的多个状态都切换为完成态，组件本身才切换为完成态。", "The synchronization state component is the execution body waiting for the completion of multiple connected states. When multiple connected states are switched to the completed state, the component itself is switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("同步状态")]
        [Tip("勾选时, 等待所有输入状态退出;不勾选时,等待所有输入跳转退出", "When checked, wait for all input states to exit; When unchecked, wait for all inputs to jump out")]
        public bool syncState = true;

        [Name("仅等待启用")]
        [Tip("勾选时, 仅等待处于启用的所有输入状态或跳转退出;不勾选时,等待所有输入状态或跳转(启用或未启用的均等待)退出", "When checked, only wait for all input states that are enabled or jump out; When unchecked, wait for all input status or jump (wait for those enabled or not enabled) to exit")]
        public bool onlyWaitEnable = true;

        protected State state => this.parent;

        protected HashSet<State> entryStates = new HashSet<State>();

        protected HashSet<Transition> entryTransitions = new HashSet<Transition>();

        public override void OnStateEntry(StateData data)
        {
            base.OnStateEntry(data);

            foreach (var inTransition in data.GetInTransitions(state))
            {
                entryTransitions.Add(inTransition);
                entryStates.Add(inTransition.inState);
            }
        }

        public override void OnExit(StateData data)
        {
            entryStates.Clear();
            entryTransitions.Clear();

            base.OnExit(data);
        }

        public override bool Finished()
        {
            if (syncState)
            {
                if (onlyWaitEnable)
                {
                    return state.inStates.All(s => s.enable ? entryStates.Contains(s) : true);
                }
                else
                {
                    return state.inStates.All(s => entryStates.Contains(s));
                }
            }
            else
            {
                if (onlyWaitEnable)
                {
                    return state.inTransitions.All(t => t.enable ? entryTransitions.Contains(t) : true);
                }
                else
                {
                    return state.inTransitions.All(t => entryTransitions.Contains(t));
                }
            }
        }
    }
}
