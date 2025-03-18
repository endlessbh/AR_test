using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.Transitions.Base;
using XCSJ.Scripts;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.Transitions
{
    /// <summary>
    /// 将网络状态的值与待比较值进行比较判断；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetStateCompare))]
    [Tip("当网络状态变化时的回调，并根据参数情况跳过入状态", "Callback when the network state changes, and skip into the state according to the parameters")]
    [RequireManager(typeof(MMOManager))]
    public class NetStateChangedSkip : SkipState
    {
        public const string Title = "网络状态变化跳过";

        [Name("旧状态")]
        [Tip("如果选择[未知],则表示不限定旧状态；否者表示必须由旧状态必须满足时，才执行触发；", "If [unknown] is selected, it means that the old state is not limited; No means that the trigger can only be executed when the old status must be met;")]
        [EnumPopup]
        public ENetState oldState = ENetState.Unknow;

        [Name("状态")]
        [Tip("如果选择[未知],则表示不限定状态；否者表示必须由状态必须满足时，才执行触发；", "If [unknown] is selected, it indicates unlimited status; No means that the trigger can only be executed when the status must be met;")]
        [EnumPopup]
        public ENetState state = ENetState.Unknow;

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            //注册状态变更事件
            MMOManager.onNetStateChanged += OnNetStateChanged;
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            //取消注册状态变更事件
            MMOManager.onNetStateChanged -= OnNetStateChanged;
        }

        private void OnNetStateChanged(ENetState oldState, ENetState state)
        {
            if (this.oldState == ENetState.Unknow)
            {
                //任意旧状态都处理
            }
            else if (this.oldState != oldState)
            {
                //旧状态不符合
                return;
            }

            if (this.state == ENetState.Unknow || this.state == state)
            {
                //任意状态都处理或状态符合限定，完成标记量修改
                finished = true;
            }
        }

        public override string ToFriendlyString()
        {
            return string.Format("{0}->{1}", CommonFun.Name(oldState), CommonFun.Name(state));
        }

        protected override bool CanSkip(StateData stateData) => finished;
    }
}
