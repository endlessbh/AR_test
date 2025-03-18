using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginNetInteract.UI
{
    /// <summary>
    /// 客户端配置视图控制器
    /// </summary>
    [Name("客户端配置视图控制器")]
    [RequireManager(typeof(NetInteractManager))]
    public class ClientConfigViewController : BaseViewController
    {
        /// <summary>
        /// 客户端
        /// </summary>
        [Name("客户端")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Client _client;

        /// <summary>
        /// 客户端
        /// </summary>
        public Client client => this.XGetComponentInChildrenOrGlobal(ref _client);

        /// <summary>
        /// 连接状态
        /// </summary>
        public string connectionState
        {
            get
            {
                if (client.inConnectAsync)
                {
                    return "连接中";
                }
                else
                {
                    return client.isConnected ? "已连接" : "未连接";
                }
            }
        }

        /// <summary>
        /// 服务器IP
        /// </summary>
        public string serverIP
        {
            get
            {
                return client._connectServerConfig._address;
            }
            set
            {
                client._connectServerConfig._address = value;
            }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int serverPort
        {
            get
            {
                return client._connectServerConfig._port;
            }
            set
            {
                client._connectServerConfig._port = value;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void ConnectServer()
        {
            if (client.inConnectAsync || client.isConnected)
            {
                client.CloseAndSyncObject();
            }

            client.ConnectAndTrySyncObject();
        }

        /// <summary>
        /// 断开服务器
        /// </summary>
        public void DisconnectServer()
        {
            client.CloseAndSyncObject();
        }    
    }
}