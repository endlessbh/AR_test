using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.Base.Net;
using XCSJ.Net;
using XCSJ.Net.Http;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// Web网络数据库:Web网络数据库通过Http协议提交网络数据;可跨平台使用,同时需要WebServer支持对应的处理,并遵循特殊的格式;
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [Name("Web网络数据库")]
    [Tip("Web网络数据库通过Http协议提交网络数据;可跨平台使用,同时需要WebServer支持对应的处理,并遵循特殊的格式;", "Web network database submits network data through HTTP protocol; It can be used across platforms. At the same time, webserver needs to support corresponding processing and follow special formats;")]
    public sealed class WebNetDBMB : BaseNetDBMB
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private UnityWebRequestClient _client = new UnityWebRequestClient();

        /// <summary>
        /// 客户端网络包
        /// </summary>
        public override IClientNetPackage client => _client;

        /// <summary>
        /// 在连接中
        /// </summary>
        public override bool inConnecting => _client.clientState == EClientState.Connecting;

        private NetDB _netDB = null;

        /// <summary>
        /// 网络数据库
        /// </summary>
        public override NetDB netDB
        {
            get
            {
                if (_netDB == null)
                {
                    _netDB = new NetDB(_client, _userInfo);
                }
                return _netDB;
            }
            protected set => _netDB = value;
        }

        private void OnValidate()
        {
            connectMode = EConnectMode.Async;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            serverPort = HttpHelper.DefaultPort;
            connectMode = EConnectMode.Async;
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() => _client.IsConnected();

        //private ListS<object> errorLogs = new ListS<object>();

        private void OnConnected()
        {
            //Debug.Log("Connected");
            //Debug.Log("Connected 1:"+ netDB.validAsyncMode);

            //连接成功开始读取答案
            _client.StartReadAnswer(OnReadAnswer, OnExit, OnError);
        }

        private void OnError(object e)
        {
            //errorLogs.Add(e);
            Debug.LogError(e);
        }

        private void OnExit()
        {
            //禁用组件
            CommonFun.DelayCall(() => enabled = false);

            //停止读取答案
            _client.StopReadAnswer(OnReadAnswer, OnExit, OnError);
        }

        private void OnReadAnswer(AnswerNetPackage answerNetPackage)
        {
            //if (answerNetPackage != null)
            //{
            //    Debug.Log("WebNetDBMB OnReadAnswer " + answerNetPackage.GetType() + " : " + answerNetPackage.IsFromJsonString() + ", Json: " + answerNetPackage.GetFromJsonString());
            //}
            //else
            //{
            //    Debug.LogError("无效答案！");
            //}
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        protected override bool ConnectDB()
        {
            if (IsConnected()) return true;
            if (inConnecting) return true;

            try
            {
                if (netDB.Connect(serverAddress, serverPort))
                {
                    OnConnected();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
            }
            return false;
        }
    }
}
