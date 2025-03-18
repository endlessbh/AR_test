using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Net;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 网络数据库:可跨平台使用，需要与[XDreamer数据库服务]应用程序配合;
    /// </summary>
    [Name("网络数据库")]
    [Tip("可跨平台使用，需要与[XDreamer数据库服务]应用程序配合;", "It can be used across platforms and needs to cooperate with [xdreamer database service] application;")]
    public sealed class NetDBMB : BaseNetDBMB
    {
        private DBClient _client = new DBClient();

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
            serverPort= DBHelper.DefaultPort;
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

            //连接数据库
            netDB.Connect();

            //配置设置
            _client.noDelay = _handleClientConfig._noDealy;
            _client.readTimeout = _handleClientConfig.readTimeout;
            _client.writeTimeout = _handleClientConfig.writeTimeout;

            //连接成功开始读取答案
            _client.StartReadAnswer(OnReadAnswer, OnExit, OnError);
        }

        private void OnError(object e)
        {
            if(_outputErrorInfo)
            {
                //errorLogs.Add(e);
                Debug.LogError(e);
            }
        }

        private void OnExit()
        {
            //禁用组件
            CommonFun.DelayCall(() => enabled = false);
        }

        private void OnReadAnswer(AnswerNetPackage answerNetPackage)
        {
            //if (answerNetPackage != null)
            //{
            //    Debug.Log("NetDBMB OnReadAnswer " + answerNetPackage.GetType() + " : " + answerNetPackage.IsFromJsonString() + ", Json: " + answerNetPackage.GetFromJsonString());
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
                if (_client.Connect(_connectServerConfig, DBHelperExtension.WSPath))
                {
                    //开始检查客户端的连接性
                    _client.StartCheckConnection(OnConnected);
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

    /// <summary>
    /// DB客户端
    /// </summary>
    public class DBClient : CrossPlatformTcpClient { }

    /// <summary>
    /// 网络客户端类型
    /// </summary>
    [Name("网络客户端类型")]
    public enum ENetClientType
    {
        /// <summary>
        /// XTcp:使用XDreamer Tcp客户端方式请求网络数据；需要启动XDreamer网络数据服务；仅支持部分平台；
        /// </summary>
        [Name("XTcp")]
        [Tip("使用XDreamer Tcp客户端方式请求网络数据；需要启动XDreamer网络数据服务；仅支持部分平台；", "Use XDreamer TCP client mode to request network data; XDreamer network data service needs to be started; Only some platforms are supported;")]
        XTcp = 0,

        /// <summary>
        /// XTcpS:使用XDreamer Tcp安全客户端方式请求网络数据；需要启动XDreamer网络数据安全服务；仅支持部分平台；
        /// </summary>
        [Name("XTcpS")]
        [Tip("使用XDreamer Tcp安全客户端方式请求网络数据；需要启动XDreamer网络数据安全服务；仅支持部分平台；", "Use XDreamer TCP security client mode to request network data; XDreamer network data security service needs to be started; Only some platforms are supported;")]
        XTcpS,
    }
}
