using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Net;

namespace XCSJ.PluginMMO.Base
{
    /// <summary>
    /// MMO客户端
    /// </summary>
    public class MMOClient : CrossPlatformTcpClient, IMMOClient
    {
        /// <summary>
        /// MMO管理器对象
        /// </summary>
        public MMOManager manager { get; private set; } = null;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="manager"></param>
        public MMOClient(MMOManager manager)
        {
            if (!manager) throw new ArgumentNullException(nameof(manager));
            this.manager = manager;
        }
    }


    /// <summary>
    /// 网络状态属性值
    /// </summary>
    [Serializable]
    [Name("网络状态属性值")]
    public class ENetStatePropertyValue : EnumPropertyValue<ENetState> { }
}
