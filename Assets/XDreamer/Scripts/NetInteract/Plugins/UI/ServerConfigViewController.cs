using System.Linq;
using System.Net;
using System.Net.Sockets;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginNetInteract.UI
{
    /// <summary>
    /// 服务器配置视图控制器
    /// </summary>
    [Name("服务器配置视图控制器")]
    [RequireManager(typeof(NetInteractManager))]
    public class ServerConfigViewController : BaseViewController
    {
        /// <summary>
        /// 服务器
        /// </summary>
        [Name("服务器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Server _server;

        /// <summary>
        /// 服务器
        /// </summary>
        public Server server => this.XGetComponentInChildrenOrGlobal(ref _server);

        /// <summary>
        /// 服务器状态
        /// </summary>
        public string serverState
        {
            get
            {
                return server.isOnLine ? "在线" : "离线";
            }
        }

        /// <summary>
        /// 连入客户端数量
        /// </summary>
        public string clientCount
        {
            get
            {
                return server._server.clients.count.ToString();
            }
        }

        /// <summary>
        /// IP类型
        /// </summary>
        [Name("IP类型")]
        public EIPType _ipType = EIPType.HostAddresseIPV4;

        /// <summary>
        /// IP类型
        /// </summary>
        [Name("IP类型")]
        public enum EIPType
        {
            [Name("主机IP地址")]
            HostAddresse,

            [Name("主机IPV4地址")]
            HostAddresseIPV4,

            [Name("主机IPV6地址")]
            HostAddresseIPV6
        }

        /// <summary>
        /// IP
        /// </summary>
        public string IP
        {
            get
            {
                return GetHostIP(_ipType);
            }
        }

        /// <summary>
        /// 端口
        /// </summary>
        public int port
        {
            get
            {
                return server.port;
            }
            set
            {
                server.port = value;
            }
        }

        /// <summary>
        /// 启用服务器
        /// </summary>
        public void StartServer() => server.StartServerAndTrySyncObject();

        /// <summary>
        /// 停止服务器
        /// </summary>
        public void StopServer() => server.StopServerAndSyncObject();

        private string GetHostIP(EIPType ipType)
        {
            var hostName = Dns.GetHostName();
            switch (ipType)
            {
                case EIPType.HostAddresse:
                    {
                        return Dns.GetHostAddresses(hostName).ToStringDirect();
                    }
                case EIPType.HostAddresseIPV4:
                    {
                        return Dns.GetHostAddresses(hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToStringDirect();
                    }
                case EIPType.HostAddresseIPV6:
                    {
                        return Dns.GetHostAddresses(hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetworkV6).ToStringDirect();
                    }
                default: return "";
            }
        }
    }
}
