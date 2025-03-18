using XCSJ.Net.Tcp.Threading;

namespace XCSJ.PluginNetInteract.Base
{
    /// <summary>
    /// 客户端虚体：也可称虚拟客户端；用于服务器记录客户端连接对象信息的虚体对象
    /// </summary>
    public class ClientVirtual : TcpClientVirtualThread<ClientVirtual, ServerEntity, NetQuestion, NetAnswer>
    {
    }    
}
