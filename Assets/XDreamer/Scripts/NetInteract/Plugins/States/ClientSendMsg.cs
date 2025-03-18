using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
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
    /// 客户端发送消息:客户端发送消息到服务器；
    /// </summary>
    [Name(Title)]
    [Tip("客户端发送消息到服务器", "The client sends a message to the server")]
    [ComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    [Owner(typeof(NetInteractManager))]
    public class ClientSendMsg : LifecycleExecutor<ClientSendMsg>, IGetCustomFunction
    {
        /// <summary>
        /// 客户端发送消息
        /// </summary>
        public const string Title = "客户端发送消息";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Client, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ClientSendMsg))]
        [Tip("客户端发送消息到服务器", "The client sends a message to the server")]
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
            /// 尝试连接并发送:如果客户端已连接到服务器，则直接发送；否者客户端尝试连接服务器后成功后发送；
            /// </summary>
            [Name("尝试连接并发送")]
            [Tip("如果客户端已连接到服务器，则直接发送；否者客户端尝试连接服务器后成功后发送；", "If the client has connected to the server, send it directly; If not, the client will send it after successfully trying to connect to the server;")]
            TryConnectAndSend,

            /// <summary>
            /// 发送:不管客户端是否已连接到服务器，均将网络消息存储到待发送缓存区；
            /// </summary>
            [Name("发送")]
            [Tip("不管客户端是否已连接到服务器，均将网络消息存储到待发送缓存区；", "Whether the client has connected to the server or not, store the network message in the buffer to be sent;")]
            Send,
        }

        /// <summary>
        /// 发送规则
        /// </summary>
        [Name("发送规则")]
        [EnumPopup]
        public ESendRule _sendRule = ESendRule.TryConnectAndSend;

        /// <summary>
        /// 获取网络问题
        /// </summary>
        /// <returns></returns>
        public NetQuestion GetNetQuestion()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.CNScript: return _netCNScript;
                case ENetMsgType.Msg: return _netMsg;
                default: return NetQuestion.HeartBeatQuestion;
            }
        }

        private IDataValidity GetDataValidityObject()
        {
            switch (_netMsgType)
            {
                case ENetMsgType.CNScript: return _netCNScript;
                case ENetMsgType.Msg: return _netMsg;
                default: return NetQuestion.HeartBeatQuestion;
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var client = this.client;
            if (!client) return;
            switch (_sendRule)
            {
                case ESendRule.TryConnectAndSend:
                    {
                        if (!client.isConnected)
                        {
                            client.ConnectAndTrySyncObject();
                        }
                        client.Send(GetNetQuestion());
                        break;
                    }
                case ESendRule.Send:
                    {
                        client.Send(GetNetQuestion());
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
            if (!_client || !GetDataValidityObject().DataValidity()) return false;
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
