using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Net;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginTools;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 服务器：用于TCP监听的服务器端组件对象
    /// </summary>
    [Name("服务器")]
    [Tip("用于TCP监听的服务器端组件对象", "Server side component object for TCP listening")]
    [XCSJ.Attributes.Icon(EIcon.Server)]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), rootType = typeof(NetInteractManager), groupRule = EToolGroupRule.Create)]
    [RequireManager(typeof(NetInteractManager))]
    [Owner(typeof(NetInteractManager))]
    [DisallowMultipleComponent]
#if UNITY_WEBGL
    //[Obsolete("不支持在WebGL环境中使用本功能组件")]
#endif
    public sealed class Server : InteractProvider
    {
        /// <summary>
        /// 服务器是否在线
        /// </summary>
        public bool isOnLine => _server.IsRunning();

        /// <summary>
        /// 服务器对象
        /// </summary>
        public ServerEntity _server = new ServerEntity();

        /// <summary>
        /// 服务器监听配置
        /// </summary>
        [Name("服务器监听配置")]
        public ServerListenConfig _serverListenConfig = new ServerListenConfig();

        /// <summary>
        /// 是否在运行中
        /// </summary>
        public bool isRunning => _server.IsRunning();

        /// <summary>
        /// 端口
        /// </summary>
        public int port
        {
            get => _serverListenConfig._port;
            set
            {
                if (_serverListenConfig._port == value) return;
                this.XModifyProperty(ref _serverListenConfig._port, value);
            }
        }

        /// <summary>
        /// 监听端口
        /// </summary>
        public int listenPort => _server.listenPort;

        /// <summary>
        /// 有效监听端口
        /// </summary>
        public int validListenPort => isRunning ? listenPort : port;

        /// <summary>
        /// 客户端最大数目
        /// </summary>
        public int clientMaxCount => _serverListenConfig._clientMaxCount;

        /// <summary>
        /// 处理客户端配置
        /// </summary>
        [Name("处理客户端配置")]
        public HandleClientConfig _handleClientConfig = new HandleClientConfig();

        /// <summary>
        /// 每帧数据处理数量
        /// </summary>
        [Name("每帧数据处理数量")]
        [Tip("每帧的数据处理数量；值越小，对帧速率波动影响越小，处理时间会较长；值越大，对帧速率波动影响越大，处理时间会较短；", "Number of data processing per frame; The smaller the value, the smaller the impact on the fluctuation of frame rate and the longer the processing time; The larger the value, the greater the influence on the fluctuation of frame rate and the shorter the processing time;")]
        [Range(1, 100)]
        public int _dataHandleCountPerFrame = 1;

        /// <summary>
        /// 每帧数据处理数量
        /// </summary>
        public int dataHandleCountPerFrame => Mathf.Max(1, _dataHandleCountPerFrame);

        /// <summary>
        /// 当有客户端连入后回调；仅在主线程中回调；
        /// </summary>
        public static event Action<Server, ClientVirtual> onClientConnected;

        /// <summary>
        /// 当有客户端将要关闭前回调；仅在主线程中回调；
        /// </summary>
        public static event Action<Server, ClientVirtual> onClientClosed;

        /// <summary>
        /// 当接收到网络问题时回调；仅在主线程中回调；
        /// </summary>
        public static event Action<Server, NetQuestion> onReceived;

        /// <summary>
        /// 启动服务并尝试同步对象(将组件设置为启用)
        /// </summary>
        /// <returns></returns>
        public bool StartServerAndTrySyncObject() => enabled = StartServer();

        /// <summary>
        /// 停止服务并尝试同步对象(将组件设置为禁用)
        /// </summary>
        public void StopServerAndSyncObject() => enabled = false;

        /// <summary>
        /// 重新启动服务并尝试同步对象(将组件设置为启用)
        /// </summary>
        /// <param name="delayTime"></param>
        public void RestartServerAndTrySyncObject(float delayTime = 0.01F)
        {
            StopServerAndSyncObject();
            CommonFun.DelayCall(() => StartServerAndTrySyncObject(), delayTime);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public bool StartServer()
        {
            var result = _server.Startup(port);
            if (result)
            {
                ServerEntity.onClientConnected -= OnClientConnected;
                ServerEntity.onClientConnected += OnClientConnected;

                ServerEntity.onClientClosed -= OnClientClosed;
                ServerEntity.onClientClosed += OnClientClosed;

                ClientVirtual.onReceived -= OnReceived;
                ClientVirtual.onReceived += OnReceived;
            }
            return result;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopServer()
        {
            _server.Stop();

            ClientVirtual.onReceived -= OnReceived;
            ServerEntity.onClientClosed -= OnClientClosed;
            ServerEntity.onClientConnected -= OnClientConnected;
        }

        /// <summary>
        /// 仅用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            enabled = StartServer();
            if(enabled)
            {
                NetInteractManager.activeServers.Add(this);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StopServer();
            NetInteractManager.activeServers.Remove(this);
        }

        /// <summary>
        /// 新连入的客户端列表：主分线程使用
        /// </summary>
        ListS<ClientVirtual> connectedClients = new ListS<ClientVirtual>();

        /// <summary>
        /// 已关闭的客户端列表：主分线程使用
        /// </summary>
        ListS<ClientVirtual> closedClients = new ListS<ClientVirtual>();

        /// <summary>
        /// 问题列表：主分线程使用，所有客户端的问题均会在分线程时被添加到本列表中；并在主线程中将列表元素移除；
        /// </summary>
        ListS<NetQuestion> questionsS = new ListS<NetQuestion>();

        /// <summary>
        /// 待处理的问题列表：主线程使用；在主线程中将<see cref="questionsS"/>问题拷贝到本列表中，并逐个处理；
        /// </summary>
        List<NetQuestion> questions = new List<NetQuestion>();

        /// <summary>
        /// 是否有问题的弱标记量：标识服务器是否收到问题需要处理
        /// </summary>
        private bool hasQuestion = false;

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!isOnLine) return;

            while (connectedClients.TryGetAndRemove(0, out var client))
            {
                onClientConnected?.Invoke(this, client);
            }

            //拷贝问题
            if (hasQuestion && questionsS.CopyListAndClear(ref questions, false))
            {
                hasQuestion = questionsS.count > 0;
            }

            //处理问题
            var count = dataHandleCountPerFrame;
            while (questions.Count > 0 && count-- > 0)
            {
                var q = questions[0];
                questions.RemoveAt(0);

                //Debug.Log("服务器:" + count.ToString() + "/" + questions.Count + ":" + q.questionCode);

                onReceived?.Invoke(this, q);
            }

            while (closedClients.TryGetAndRemove(0, out var client))
            {
                onClientClosed?.Invoke(this, client);
            }
        }

        /// <summary>
        /// 广播
        /// </summary>
        /// <param name="netAnswer"></param>
        public void Broadcast(NetAnswer netAnswer) => _server.Broadcast(netAnswer);

        private void OnClientConnected(ServerEntity server, ClientVirtual client)
        {
            if (server == this._server)
            {
                client.noDelay = _handleClientConfig._noDealy;
                client.writeTimeout = _handleClientConfig.writeTimeout;
                client.readTimeout = _handleClientConfig.readTimeout;
                client.netQAMode = _serverListenConfig._netQAMode; 
                connectedClients.Add(client);
            }
        }

        private void OnClientClosed(ServerEntity server, ClientVirtual client)
        {
            if (server == this._server)
            {
                closedClients.Add(client);
            }
        }

        private void OnReceived(ClientVirtual client, NetQuestion q)
        {
            switch (q.questionCode)
            {
                case EQuestionCode.HeartBeat:
                    {
                        switch (client.answers.count)
                        {
                            case 0:
                                {
                                    client.Send(NetAnswer.HeatBeatAnswer);
                                    break;
                                }
                            default:
                                {
                                    //有答案要发送，不处理了！
                                    break;
                                }
                        }
                        break;
                    }
                case EQuestionCode.Valid:
                    {
                        q.client = client;
                        questionsS.Add(q);
                        hasQuestion = true;
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 服务器监听配置
    /// </summary>
    [Name("服务器监听配置")]
    [Serializable]
    public class ServerListenConfig
    {
        /// <summary>
        /// 端口
        /// </summary>
        [Name("端口")]
        public int _port = NetInteractHelper.DefaultPort;

        /// <summary>
        /// 客户端最大数目:允许连入的客户端最大数目；-1表示不限制数量，0不允许任何客户端连入，默认允许2个；
        /// </summary>
        [Name("客户端最大数目")]
        [Tip("允许连入的客户端最大数目；-1表示不限制数量，0不允许任何客户端连入，默认允许2个；", "The maximum number of clients allowed to connect- 1 means unlimited quantity, 0 does not allow any client to connect, and 2 are allowed by default;")]
        [Range(-1, 64)]
        public int _clientMaxCount = 2;

        /// <summary>
        /// 网络问答模式
        /// </summary>
        [Name("网络问答模式")]
        [EnumPopup]
        [Readonly]
        public ENetQAMode _netQAMode = ENetQAMode.T1QT2A;
    }



    /// <summary>
    /// 服务器信息
    /// </summary>
    [Serializable]
    [Name("服务器信息")]
    public class ServerInfo
    {
        /// <summary>
        /// 服务器规则
        /// </summary>
        [Name("服务器规则")]
        public enum EServerRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 任意
            /// </summary>
            [Name("任意")]
            Any,

            /// <summary>
            /// 列表中
            /// </summary>
            [Name("列表中")]
            InList,

            /// <summary>
            /// 不在列表中
            /// </summary>
            [Name("不在列表中")]
            NotInList,
        }

        /// <summary>
        /// 服务器规则
        /// </summary>
        [Name("服务器规则")]
        [EnumPopup]
        public EServerRule _serverRule = EServerRule.Any;

        /// <summary>
        /// 服务器列表
        /// </summary>
        [Name("服务器列表")]
        public List<Server> _servers = new List<Server>();

        /// <summary>
        /// 有效服务器
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public bool ValidServer(Server server)
        {
            if (!server) return false;
            switch (_serverRule)
            {
                case EServerRule.Any: return true;
                case EServerRule.InList: return _servers.Contains(server);
                case EServerRule.NotInList: return !_servers.Contains(server);
                default: return false;
            }
        }
    }
}
