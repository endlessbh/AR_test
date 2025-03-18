using System;
using System.Collections;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Net;

namespace XCSJ.Extension.Base.Net
{
    /// <summary>
    /// WebSocket客户端
    /// </summary>
    public class WebSocketClient : BaseClient
    {
        /// <summary>
        /// 远程地址
        /// </summary>
        public override Address remoteAddress => new Address(url, port);

        /// <summary>
        /// 本地地址
        /// </summary>
        public override Address localAddress => new Address();

        /// <summary>
        /// URL
        /// </summary>
        public string url = "";

        /// <summary>
        /// 端口
        /// </summary>
        public int port = 0;

        /// <summary>
        /// WebSocket对象
        /// </summary>
        public WebSocket sock { get; private set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; protected set; } = "";

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            try
            {
                clientState = EClientState.Closing;
                if (sock != null)
                {
                    sock.Close();
                    sock = null;
                }
            }
            finally
            {
                clientState = EClientState.Closed;
            }            
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            if (IsConnected()) return true;

            clientState = EClientState.WillConnect;
            sock = new WebSocket(WebSocket.ToUri(url, port, false, path));
            sock.Connect();
            clientState = EClientState.Connecting;
            return true;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <returns></returns>
        public override bool Connect(IAddress serverAddress)
        {
            if (serverAddress == null) return false;
            if (IsConnected()) return true;
            this.url = serverAddress.address;
            this.port = serverAddress.port;
            return Connect();
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public override bool IsConnected()
        {
            if (sock == null) return false;
            if (sock.Connected)
            {
                clientState = EClientState.Connected;
                return true;
            }
            else
            {
                if (!string.IsNullOrEmpty(sock.Error))
                {
                    Close();
                    return false;
                }
                if (clientState == EClientState.Connecting) return true;
            }
            return false;
        }        

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Write(string data)
        {
            if (!IsConnected())
            {
                return 0;
            }
            if (clientState == EClientState.Connecting)
            {
                //Debug.Log("Write 1");
                return 0;
            }
            return sock.SendString(data);
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Read(out string data)
        {
            if (!IsConnected() || clientState == EClientState.Connecting)
            {
                data = null;
                return 0;
            }
            return sock.RecvString(out data);
        }
    }
}