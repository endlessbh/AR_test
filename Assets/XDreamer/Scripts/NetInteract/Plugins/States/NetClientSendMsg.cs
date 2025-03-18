using System;
using System.Net.Sockets;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 网络客户端发送消息:模拟网络客户端，发送消息到指定的IP与端口；
    /// </summary>
    [Name(Title)]
    [Tip("模拟网络客户端，发送消息到指定远程的IP与端口", "Simulate the network client and send messages to the specified remote IP and port")]
    [ComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.Export)]
    [Owner(typeof(NetInteractManager))]
    public class NetClientSendMsg : LifecycleExecutor<NetClientSendMsg>
    {
        /// <summary>
        /// 客户端发送消息
        /// </summary>
        public const string Title = "网络客户端发送消息";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Client, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(NetClientSendMsg))]
        [Tip("模拟网络客户端，发送消息到指定的IP与端口；", "Simulate the network client and send messages to the specified IP and port;")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 远程IP
        /// </summary>
        [Name("远程IP")]
        [OnlyMemberElements]
        public StringPropertyValue _remoteIP = new StringPropertyValue();

        /// <summary>
        /// 远程IP
        /// </summary>
        public string remoteIP => _remoteIP.GetValue();

        /// <summary>
        /// 远程端口
        /// </summary>
        [Name("远程端口")]
        [OnlyMemberElements]
        public IntPropertyValue _remotePort = new IntPropertyValue();

        /// <summary>
        /// 远程端口
        /// </summary>
        public int remotePort => _remotePort.GetValue();

        /// <summary>
        /// 协议类型
        /// </summary>
        [Name("协议类型")]
        public enum EProtocolType
        {
            /// <summary>
            /// UDP
            /// </summary>
            [Name("UDP")]
            UDP,

            /// <summary>
            /// UDP
            /// </summary>
            [Name("TCP")]
            TCP,
        }

        /// <summary>
        /// 协议类型属性值
        /// </summary>
        [Serializable]
        [Name("协议类型属性值")]
        public class EProtocolTypePropertyValue : EnumPropertyValue<EProtocolType> { }

        /// <summary>
        /// 协议类型
        /// </summary>
        [Name("协议类型")]
        [OnlyMemberElements]
        public EProtocolTypePropertyValue _protocolType = new EProtocolTypePropertyValue();

        /// <summary>
        /// 消息
        /// </summary>
        [Name("消息")]
        [OnlyMemberElements]
        public StringPropertyValue_TextArea _msg = new StringPropertyValue_TextArea();

        /// <summary>
        /// 字符串类型
        /// </summary>
        [Name("字符串类型")]
        [OnlyMemberElements]
        public EStringTypePropertyValue _stringType = new EStringTypePropertyValue();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_protocolType.GetValue())
            {
                case EProtocolType.UDP:
                    {
                        UdpSend();
                        break;
                    }
                case EProtocolType.TCP:
                    {
                        TcpSend();
                        break;
                    }
            }
        }

        private byte[] GetMsgBytes() => StringHelper.GetBytes(_msg.GetValue(), _stringType.GetValue());

        private void UdpSend()
        {
            try
            {
                var msg = GetMsgBytes();
                if (msg == null || msg.Length == 0) return;
                var client = new UdpClient();
                client.Connect(remoteIP, remotePort);
                client.Send(msg, msg.Length);
                client.Close();
            }
            catch (Exception ex)
            {
                Log.Exception(ex.ToString());
            }

        }

        private void TcpSend()
        {
            try
            {
                var msg = GetMsgBytes();
                if (msg == null || msg.Length == 0) return;
                var client = new TcpClient();
                client.Connect(remoteIP, remotePort);
                client.GetStream().Write(msg, 0, msg.Length);
                client.Close();
            }
            catch (Exception ex)
            {
                Log.Exception(ex.ToString());
            }
        }
    }
}
