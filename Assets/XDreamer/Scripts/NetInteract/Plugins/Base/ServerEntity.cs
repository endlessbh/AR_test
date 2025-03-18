using XCSJ.Net.Tcp.Threading;

namespace XCSJ.PluginNetInteract.Base
{
    /// <summary>
    /// 服务器实体：也可称实体服务器；
    /// </summary>
    public class ServerEntity : TcpServerThread<ServerEntity, ClientVirtual, NetQuestion, NetAnswer> { }
}
