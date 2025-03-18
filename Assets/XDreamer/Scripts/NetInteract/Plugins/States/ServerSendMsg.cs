using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Net;
using XCSJ.Net.Tcp;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.CNScripts;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 服务器发送消息:服务器发送消息到客户端
    /// </summary>
    [Name(Title)]
    [Tip("服务器发送消息到客户端", "The server sends a message to the client")]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    [Owner(typeof(NetInteractManager))]
    public class ServerSendMsg : LifecycleExecutor<ServerSendMsg>, IGetCustomFunction
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "服务器发送消息";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Server, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ServerSendMsg))]
        [Tip("服务器发送消息到客户端", "The server sends a message to the client")]
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
        /// 网络消息类型
        /// </summary>
        [Name("网络消息类型")]
        [EnumPopup]
        public ENetMsgType _netMsgType = ENetMsgType.Msg;

        /// <summary>
        /// 网络消息
        /// </summary>
        [Name("网络消息")]
        [HideInSuperInspector(nameof(_netMsgType), EValidityCheckType.NotEqual, ENetMsgType.Msg)]
        public NetMsg _netMsg = new NetMsg();

        /// <summary>
        /// 网络中文脚本
        /// </summary>
        [Name("网络中文脚本")]
        [HideInSuperInspector(nameof(_netMsgType), EValidityCheckType.NotEqual, ENetMsgType.CNScript)]
        public NetCNScript _netCNScript = new NetCNScript();

        /// <summary>
        /// 发送规则
        /// </summary>
        [Name("发送规则")]
        public enum ESendRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 广播:将网络消息广播到所有已连接到服务器的客户端
            /// </summary>
            [Name("广播")]
            [Tip("将网络消息广播到所有已连接到服务器的客户端", "Broadcast network messages to all clients connected to the server")]
            Broadcast,

            /// <summary>
            /// 广播列表:将网络消息广播到广播列表中客户端唯一标识对应的客户端
            /// </summary>
            [Name("广播列表")]
            [Tip("将网络消息广播到广播列表中客户端唯一标识对应的客户端", "Broadcast the network message to the broadcast list. The client uniquely identifies the corresponding client")]
            BroadcastList,

            /// <summary>
            /// 广播忽略列表:将网络消息广播到除广播忽略列表中客户端唯一标识对应客户端以外的所有已连接到服务器的客户端
            /// </summary>
            [Name("广播列表")]
            [Tip("将网络消息广播到除广播忽略列表中客户端唯一标识对应客户端以外的所有已连接到服务器的客户端", "Broadcast the network message to all the clients connected to the server except the client that uniquely identifies the corresponding client in the broadcast ignore list")]
            BroadcastIgnoreList,
        }

        /// <summary>
        /// 发送规则
        /// </summary>
        [Name("发送规则")]
        [EnumPopup]
        public ESendRule _sendRule = ESendRule.Broadcast;

        /// <summary>
        /// 广播列表
        /// </summary>
        [Name("广播列表")]
        [HideInSuperInspector(nameof(_sendRule), EValidityCheckType.NotEqual, ESendRule.BroadcastList)]
        public List<StringPropertyValue> _broadcastList = new List<StringPropertyValue>();

        /// <summary>
        /// 广播忽略列表
        /// </summary>
        [Name("广播忽略列表")]
        [HideInSuperInspector(nameof(_sendRule), EValidityCheckType.NotEqual, ESendRule.BroadcastIgnoreList)]
        public List<StringPropertyValue> _broadcastIgnoreList = new List<StringPropertyValue>();

        /// <summary>
        /// 获取网络问题
        /// </summary>
        /// <returns></returns>
        public NetAnswer GetNetAnswer()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.CNScript: return _netCNScript;
                case ENetMsgType.Msg: return _netMsg;
                default: return NetAnswer.HeatBeatAnswer;
            }
        }

        private IDataValidity GetDataValidityObject()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.CNScript: return _netCNScript;
                case ENetMsgType.Msg: return _netMsg;
                default: return NetAnswer.HeatBeatAnswer;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var server = this.server;
            if (!server) return;
            switch(_sendRule)
            {
                case ESendRule.Broadcast:
                    {
                        server.Broadcast(GetNetAnswer());
                        break;
                    }
                case ESendRule.BroadcastList:
                    {
                        var guids = _broadcastList.ToList(spv => spv.GetValue(""));
                        if (server._server.clients.TryGetItems(c => guids.Contains(c.guid), out var clients))
                        {
                            var a = GetNetAnswer();
                            foreach (var c in clients)
                            {
                                c.Send(a);
                            }
                        }
                        break;
                    }
                case ESendRule.BroadcastIgnoreList:
                    {
                        var guids = _broadcastIgnoreList.ToList(spv => spv.GetValue(""));
                        if (server._server.clients.TryGetItems(c => !guids.Contains(c.guid), out var clients))
                        {
                            var a = GetNetAnswer();
                            foreach (var c in clients)
                            {
                                c.Send(a);
                            }
                        }
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
            if (!_server || !GetDataValidityObject().DataValidity()) return false;
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

        CustomFunction IGetCustomFunction.GetCustomFunction(string propertyPath) => _netCNScript._scriptSet._value;

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.Msg:
                    {
                        return _netMsg.GetTip();
                    }
                case ENetMsgType.CNScript:
                    {
                        return CommonFun.Name(_netMsgType);
                    }
            }
            return "";
        }
    }
}
