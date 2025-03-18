using System.Net.Sockets;
using XCSJ.Net.Tcp.Threading;

namespace XCSJ.PluginNetInteract.Base
{
    /// <summary>
    /// 客户端实体：也可称实体客户端；用于客户端记录与服务器连接对象信息的实体对象
    /// </summary>
    public class ClientEntity : TcpClientEntityThread<ClientEntity, NetQuestion, NetAnswer>
    {
        /// <summary>
        /// 构造
        /// </summary>
        public ClientEntity() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="tcpClient"></param>
        public ClientEntity(TcpClient tcpClient) : base(tcpClient) { }
    }
}
