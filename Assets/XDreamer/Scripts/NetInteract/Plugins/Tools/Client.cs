using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Net;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Net;
using XCSJ.Net.Tcp;
using XCSJ.Net.Tcp.Threading;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginTools;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 客户端：用于通过TCP连接服务器的客户端组件对象
    /// </summary>
    [Name("客户端")]
    [Tip("用于通过TCP连接服务器的客户端组件对象", "Client component object for connecting to the server via TCP")]
    [XCSJ.Attributes.Icon(EIcon.Client)]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager), rootType = typeof(NetInteractManager), groupRule = EToolGroupRule.Create)]
    [RequireManager(typeof(NetInteractManager))]
    [Owner(typeof(NetInteractManager))]
    [DisallowMultipleComponent]
#if UNITY_WEBGL
    //[Obsolete("不支持在WebGL环境中使用本功能组件")]
#endif
    public sealed class Client : InteractProvider
    {
        /// <summary>
        /// 客户端对象
        /// </summary>
        public ClientEntity _client = new ClientEntity();

        /// <summary>
        /// 连接服务器配置
        /// </summary>
        [Name("连接服务器配置")]
        public ConnectServerConfigWithMode _connectServerConfig = new ConnectServerConfigWithMode();

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string serverAddress
        {
            get => _connectServerConfig.address;
            set
            {
                if (_connectServerConfig.address == value) return;
                this.XModifyProperty(ref _connectServerConfig._address, value);
            }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int serverPort
        {
            get => _connectServerConfig.port;
            set
            {
                if (_connectServerConfig.port == value) return;
                this.XModifyProperty(ref _connectServerConfig._port, value);
            }
        }

        /// <summary>
        /// 连接模式
        /// </summary>
        public EConnectMode connectMode
        {
            get => _connectServerConfig._connectMode;
            set
            {
                if (_connectServerConfig._connectMode == value) return;
                this.XModifyProperty(ref _connectServerConfig._connectMode, value);
            }
        }

        /// <summary>
        /// 网络问答模式
        /// </summary>
        public ENetQAMode netQAMode
        {
            get => _connectServerConfig._netQAMode;
            set
            {
                if (_connectServerConfig._netQAMode == value) return;
                this.XModifyProperty(ref _connectServerConfig._netQAMode, value);
            }
        }

        /// <summary>
        /// 处理客户端配置
        /// </summary>
        [Name("处理客户端配置")]
        public HandleClientConfig _handleClientConfig = new HandleClientConfig();

        /// <summary>
        /// 心跳设置
        /// </summary>
        [Name("心跳设置")]
        public TimeSetting _heartBeatSetting = new TimeSetting();

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
        /// 已连接后回调；仅在主线程中回调
        /// </summary>
        public static event Action<Client, bool> onConnected;

        /// <summary>
        /// 已关闭后回调；仅在主线程中回调
        /// </summary>
        public static event Action<Client> onClosed;

        /// <summary>
        /// 当接收到网络答案时回调；仅在主线程中回调
        /// </summary>
        public static event Action<Client, NetAnswer> onReceived;

        /// <summary>
        /// 是否已连接
        /// </summary>
        public bool isConnected => _client.IsConnected();

        /// <summary>
        /// 是否正在异步连接中
        /// </summary>
        public bool inConnectAsync => _client.inConnectAsync;

        /// <summary>
        /// 调用
        /// </summary>
        private void CallConnected()
        {
            var isConnected = this.isConnected;
            SetEnabled(isConnected);
            onConnected?.Invoke(this, isConnected);
        }

        private void CallClosed()
        {
            onClosed?.Invoke(this);
        }

        private void SetClientInfo()
        {
            _client.noDelay = _handleClientConfig._noDealy;
            _client.writeTimeout = _handleClientConfig.writeTimeout;
            _client.readTimeout = _handleClientConfig.readTimeout;
        }

        /// <summary>
        /// 连接回调：可在多线程中回调
        /// </summary>
        private void ConnectCallback(XTcpClient tcpClient)
        {
            if (tcpClient != _client) return;
            SetClientInfo();
            CommonFun.DelayCall(CallConnected);
        }

        /// <summary>
        /// 关闭回调：可在多线程中回调
        /// </summary>
        private void CloseCallback(XTcpClient tcpClient)
        {
            if (tcpClient != _client) return;
            CommonFun.DelayCall(CallClosed);
        }

        /// <summary>
        /// 连接并尝试同步对象(将组件设置为启用)
        /// </summary>
        /// <returns></returns>
        public void ConnectAndTrySyncObject()
        {
            if (isConnected)
            {
                SetEnabled(true);
                return;
            }
            switch (_connectServerConfig._connectMode)
            {
                case EConnectMode.Async:
                    {
                        SetEnabled(ConnectAsync());
                        break;
                    }
                case EConnectMode.Sync:
                    {
                        SetEnabled(Connect());
                        break;
                    }
            }
        }

        /// <summary>
        /// 关闭并尝试同步对象(将组件设置为禁用)
        /// </summary>
        public void CloseAndSyncObject() => enabled = false;

        /// <summary>
        /// 重新连接并尝试同步对象(将组件设置为启用)
        /// </summary>
        /// <param name="delayTime"></param>
        public void ReconnectAndTrySyncObject(float delayTime = 0.01F)
        {
            CloseAndSyncObject();
            CommonFun.DelayCall(() => ConnectAndTrySyncObject(), delayTime);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        private bool Connect()
        {
            //Debug.Log("连接");
            if (isConnected || _client.inConnectAsync) return true;
            _client.netQAMode = netQAMode;
            return _client.Connect(_connectServerConfig);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        private bool ConnectAsync()
        {
            if (isConnected || _client.inConnectAsync) return true;
            _client.netQAMode = netQAMode;
            return _client.ConnectAsync(_connectServerConfig) != null;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void Close()
        {
            //Debug.Log("关闭");
            _client.Close();
        }

        private void SetEnabled(bool enabled)
        {
            if (enabled || isConnected || _client.inConnectAsync)
            {
                this.enabled = true;
            }
            else
            {
                this.enabled = false;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Awake()
        {
            SetClientInfo();
            ClientEntity.onConnect += ConnectCallback;
            ClientEntity.onClose += CloseCallback;
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestroy()
        {
            ClientEntity.onConnect -= ConnectCallback;
            ClientEntity.onClose -= CloseCallback;
        }

        /// <summary>
        /// 仅用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ClientEntity.onReceived += OnReceived;
            ConnectAndTrySyncObject();
            if (enabled)
            {
                NetInteractManager.activeClients.Add(this);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            Close();
            ClientEntity.onReceived -= OnReceived;
            NetInteractManager.activeClients.Remove(this);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            //_heartBeatSettings._intervalTime = 1;
            serverPort = NetInteractHelper.DefaultPort;
        }

        /// <summary>
        /// 发送:加入待发送缓存区，等待后续线程处理；
        /// </summary>
        /// <param name="netQuestion"></param>
        public void Send(NetQuestion netQuestion) => _client.Send(netQuestion);

        /// <summary>
        /// 发送心跳问题
        /// </summary>
        public void SendHeartBeatQuestion()
        {
            //Debug.Log("发送心跳问题");
            Send(NetQuestion.HeartBeatQuestion);
            _heartBeatSetting.ResetWaitedTime();
        }

        /// <summary>
        /// 待处理答案列表：主线程使用
        /// </summary>
        List<NetAnswer> answers = new List<NetAnswer>();

        /// <summary>
        /// 是否有答案的弱标记量：标识服务器是否收到答案需要处理
        /// </summary>
        private bool hasAnswer = false;

        private void OnReceived(ClientEntity clientEntity, NetAnswer netAnswer)
        {
            if (clientEntity != _client) return;
            hasAnswer = true;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!isConnected)
            {
                //Debug.Log("已关闭");
                if (!_client.inConnectAsync)//不是在异步连接状态时，将组件禁用
                {
                    CloseAndSyncObject();
                }
                return;
            }

            //拷贝答案
            if (hasAnswer && _client.answers.CopyListAndClear(ref answers, false))
            {
                //Debug.Log(answers.Count);
                hasAnswer = _client.answers.count > 0;
            }

            //处理答案
            var count = dataHandleCountPerFrame;
            while (answers.Count > 0 && count-- > 0)
            {
                var a = answers[0];
                answers.RemoveAt(0);

                //Debug.Log("客户端:" + count.ToString() + "/" + answers.Count + ":" + a.answerCode);
                switch (a.answerCode)
                {
                    case EAnswerCode.Valid:
                        {
                            onReceived?.Invoke(this, a);
                            break;
                        }
                    case EAnswerCode.ValidAgain:
                        {
                            SendHeartBeatQuestion();
                            onReceived?.Invoke(this, a);
                            break;
                        }
                    case EAnswerCode.InvalidAgain:
                        {
                            SendHeartBeatQuestion();
                            break;
                        }
                }
            }

            //处理心跳
            if (_heartBeatSetting.NeedTry())
            {
                SendHeartBeatQuestion();
            }
        }
    }
}
