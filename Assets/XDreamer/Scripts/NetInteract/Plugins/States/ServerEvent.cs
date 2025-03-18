using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Net.Tcp.Threading;
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
    /// 服务器事件
    /// </summary>
    [Name(Title)]
    [Tip("服务器事件", "Server events")]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Owner(typeof(NetInteractManager))]
    public class ServerEvent : Trigger<ServerEvent>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "服务器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Server, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ServerEvent))]
        [Tip("服务器事件", "Server events")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 服务器
        /// </summary>
        [Name("服务器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Server _server;

        /// <summary>
        /// 服务器
        /// </summary>
        public Server server => this.XGetComponentInGlobal(ref _server);

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
            /// 已启动
            /// </summary>
            [Name("已启动")]
            Started,

            /// <summary>
            /// 已停止
            /// </summary>
            [Name("已停止")]
            Stopped,

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
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.ClientConnected;

        /// <summary>
        /// 客户端唯一编号:用于将客户端唯一编号存储的变量
        /// </summary>
        [Name("客户端唯一编号变量")]
        [Tip("用于将客户端唯一编号存储的变量", "The variable used to store the client unique number")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _clientGuidVariable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _clientGuidVariable);
        }

        #endregion

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            Server.onClientConnected += OnClientConnected;
            Server.onClientClosed += OnClientClosed;
            Server.onReceived += OnReceivedMsg;
            ServerEntity.onChanged += OnServerChanged;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            Server.onClientConnected -= OnClientConnected;
            Server.onClientClosed -= OnClientClosed;
            Server.onReceived -= OnReceivedMsg;
            ServerEntity.onChanged -= OnServerChanged;
            base.OnExit(stateData);
        }

        private void OnClientConnected(Server server, ClientVirtual client)
        {
            if (finished) return;
            if (server != this.server) return;
            if (_eventType == EEventType.ClientConnected)
            {
                finished = true;
                _clientGuidVariable.TrySetOrAddSetHierarchyVarValue(client.guid);
            }
        }

        private void OnClientClosed(Server server, ClientVirtual client)
        {
            if (finished) return;
            if (server != this.server) return;
            if (_eventType == EEventType.ClientClosed)
            {
                finished = true;
                _clientGuidVariable.TrySetOrAddSetHierarchyVarValue(client.guid);
            }
        }

        private void OnReceivedMsg(Server server, NetQuestion netQuestion)
        {
            if (finished) return;
            if (server != this.server) return;
            if (_eventType == EEventType.ReceivedMsg)
            {
                finished = true;
                _clientGuidVariable.TrySetOrAddSetHierarchyVarValue(netQuestion.client.guid);
            }
        }

        private void OnServerChanged(ServerEntity serverEntity, EServerChangedEvent serverChangedEvent)
        {
            if (finished) return;
            var server = this.server;
            if (!server || serverEntity != server._server) return;
            switch (_eventType)
            {
                case EEventType.Started:
                    {
                        finished = serverChangedEvent == EServerChangedEvent.Started;
                        break;
                    }
                case EEventType.Stopped:
                    {
                        finished = serverChangedEvent == EServerChangedEvent.Stopped;
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_server) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!server) { }
        }
    }
}
