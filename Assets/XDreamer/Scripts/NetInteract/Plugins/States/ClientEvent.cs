using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 客户端事件
    /// </summary>
    [Name(Title)]
    [Tip("客户端事件", "Client events")]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Owner(typeof(NetInteractManager))]
    public class ClientEvent : Trigger<ClientEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "客户端事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Client, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ClientEvent))]
        [Tip("客户端事件", "Client events")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 客户端
        /// </summary>
        [Name("客户端")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Client _client;

        /// <summary>
        /// 客户端
        /// </summary>
        public Client client => this.XGetComponentInGlobal(ref _client);

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        public enum EEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 客户端已连接
            /// </summary>
            [Name("客户端已连接")]
            ClientConnected,

            /// <summary>
            /// 客户端已关闭
            /// </summary>
            [Name("客户端已关闭")]
            ClientClosed,

            /// <summary>
            /// 收到消息
            /// </summary>
            [Name("收到消息")]
            ReceivedMsg,

            /// <summary>
            /// 客户端连接失败
            /// </summary>
            [Name("客户端连接失败")]
            ClientConnectFalse,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.ClientConnected;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            Client.onConnected += OnClientConnected;
            Client.onClosed += OnClientClosed;
            Client.onReceived += OnReceivedMsg;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            Client.onConnected -= OnClientConnected;
            Client.onClosed -= OnClientClosed;
            Client.onReceived -= OnReceivedMsg;
            base.OnExit(stateData);
        }

        private void OnClientConnected(Client client, bool isConnected)
        {
            if (finished) return;
            if (client != this.client) return;

            switch (_eventType)
            {
                case EEventType.ClientConnected:
                    {
                        finished = isConnected;
                        break;
                    }
                case EEventType.ClientConnectFalse:
                    {
                        finished = !isConnected;
                        break;
                    }
            }
        }

        private void OnClientClosed(Client client)
        {
            if (finished) return;
            if (client != this.client) return;
            if (_eventType == EEventType.ClientClosed)
            {
                finished = true;
            }
        }

        private void OnReceivedMsg(Client client, NetAnswer netAnswer)
        {
            if (finished) return;
            if (client != this.client) return;
            if (_eventType == EEventType.ReceivedMsg)
            {
                finished = true;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_client) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!client) { }
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_eventType);
        }
    }
}
