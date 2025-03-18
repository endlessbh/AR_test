using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 网络组件中被特性SyncVarAttribute修饰的同步变量变发生变化时触发；网络到本地；如果没有同步变量不执行回调；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Variable)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(SyncVarChanged))]
    [Tip("同步组件中被特性SyncVarAttribute修饰的同步变量变发生变化时触发；网络到本地；如果没有同步变量不执行回调；", "Triggered when the synchronization variable modified by the attribute syncvarattribute in the synchronization component changes; Network to local; If there is no synchronization variable, the callback will not be executed;")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class SyncVarChanged : Trigger<SyncVarChanged>
    {
        public const string Title = "同步变量变化";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(SyncVarChanged))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("同步组件中被特性SyncVarAttribute修饰的同步变量变发生变化时触发；网络到本地；如果没有同步变量不执行回调；", "Triggered when the synchronization variable modified by the attribute syncvarattribute in the synchronization component changes; Network to local; If there is no synchronization variable, the callback will not be executed;")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("网络组件")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetMB netMB;

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            NetMB.onSyncVarChanged += OnSyncVarChanged;
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            NetMB.onSyncVarChanged -= OnSyncVarChanged;
        }

        private void OnSyncVarChanged(NetMB netMB)
        {
            if (netMB && this.netMB == netMB)
            {
                finished = true;
            }
        }

        public override string ToFriendlyString()
        {
            return netMB ? netMB.name : "";// base.ToFriendlyString();
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && netMB;
        }
    }
}
