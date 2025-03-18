using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Views.Inputs;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 基础处理器
    /// </summary>
    [Name("基础处理器")]
    [RequireManager(typeof(NetInteractManager))]
    public abstract class BaseProcessor : InteractProvider
    {
    }

    /// <summary>
    /// 服务器处理器
    /// </summary>
    [Name("客户端处理器")]
    [RequireManager(typeof(NetInteractManager))]
    [Owner(typeof(NetInteractManager))]
    public abstract class ClientProcessor : InteractProvider
    {
        /// <summary>
        /// 客户端信息
        /// </summary>
        [Name("客户端信息")]
        public ClientInfo _clientInfo = new ClientInfo();

        /// <summary>
        /// 有效客户端
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool ValidClient(Client client) => _clientInfo.ValidClient(client);

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Client.onReceived += OnReceived;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Client.onReceived -= OnReceived;
        }

        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="netAnswer"></param>
        protected abstract void OnReceived(Client client, NetAnswer netAnswer);

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() { }
    }

    /// <summary>
    /// 服务器处理器
    /// </summary>
    [Name("服务器处理器")]
    [RequireManager(typeof(NetInteractManager))]
    [Owner(typeof(NetInteractManager))]
    public abstract class ServerProcessor : InteractProvider
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        [Name("服务器信息")]
        [FormerlySerializedAs(nameof(serverInfo))]
        public ServerInfo _serverInfo = new ServerInfo();

        public ServerInfo serverInfo => _serverInfo;

        /// <summary>
        /// 有效服务器
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public bool ValidServer(Server server) => serverInfo.ValidServer(server);

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Server.onReceived += OnReceived;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Server.onReceived -= OnReceived;
        }

        /// <summary>
        /// 当收到数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="netQuestion"></param>
        protected abstract void OnReceived(Server server, NetQuestion netQuestion);

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset() { }
    }
}
