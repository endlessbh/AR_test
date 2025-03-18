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
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.States
{
    /// <summary>
    /// 网络组件中有同步数据发生变化时触发，即反序列数据完成时的回调事件（包括同步变量、自定义的数据）；网络到本地；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Data)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(SyncDataChanged))]
    [Tip("网络组件中有同步数据发生变化时触发，即反序列数据完成时的回调事件（包括同步变量、自定义的数据）；网络到本地；", "Triggered when the synchronization data in the network component changes, that is, the callback event when the inverse sequence data is completed (including synchronization variables and user-defined data); Network to local;")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class SyncDataChanged : Trigger<SyncDataChanged>
    {
        public const string Title = "同步数据变化";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(SyncDataChanged))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("网络组件中有同步数据发生变化时触发，即反序列数据完成时的回调事件（包括同步变量、自定义的数据）；网络到本地；", "Triggered when the synchronization data in the network component changes, that is, the callback event when the inverse sequence data is completed (including synchronization variables and user-defined data); Network to local;")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("网络组件")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetMB netMB;

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            NetMB.onSyncDataChanged += OnSyncDataChanged;
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            NetMB.onSyncDataChanged -= OnSyncDataChanged;
        }

        private void OnSyncDataChanged(NetMB netMB)
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
