using System;
using System.Text;
using XCSJ.Net;
using System.Net;
using XCSJ.Algorithms;

#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;
#else
using System.Collections.Generic;
using System.Security.Authentication;
#endif

namespace XCSJ.Extension.Base.Net
{
    public class WebSocket
    {
        private Uri mUrl;

        private string protocols = "json";

        public WebSocket(Uri url, string serialization = null)
        {
            //Log.Debug(url);
            this.mUrl = url;
            if (serialization != null)
            {
                this.protocols = serialization;
            }

            string protocol = mUrl.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
            {
                throw new ArgumentException("无效协议: " + protocol);
            }
        }

        public WebSocket(string url, string serialization = null) : this(new Uri(url), serialization) { }

        public static string ToIP(string url)
        {
            if (IPAddress.TryParse(url, out IPAddress address))
            {
                return address.ToString(); ;
            }
#if UNITY_WEBGL && !UNITY_EDITOR
            return url;
#else
            //Dns无法在WebGL环境中工作
            var hostInfo = Dns.GetHostEntry(url);
            return hostInfo?.AddressList?[0].ToString();
#endif
        }

        public static string ToUrl(string ip, int port = 0, bool safe = false, string relativeUri = "")
        {
            if (string.IsNullOrEmpty(ip)) return "";
            ip = ToIP(ip);
            var scheme = (ip.StartsWith("ws://", StringComparison.CurrentCultureIgnoreCase) || ip.StartsWith("wss://", StringComparison.CurrentCultureIgnoreCase)) ? "" : (safe ? "wss://" : "ws://");
            if (NetHelper.IsValidPort(port))
            {
                return string.Format("{0}{1}:{2}{3}", scheme, ip, port, relativeUri);
            }
            else
            {
                return string.Format("{0}{1}{2}", scheme, ip, relativeUri);
            }
        }

        public static Uri ToUri(string ip, int port = 0, bool safe = false, string relativeUri = "") => new Uri(ToUrl(ip, port, safe, relativeUri));

        public int SendString(string str)
        {
            return Send(NetHelper.ToBytes(str));
        }

        public int RecvString(out string str)
        {
            byte[] retval = Recv();
            if (retval == null)
            {
                str = null;
                return 0;
            }
            str = NetHelper.ToString(retval);
            //UnityEngine.Debug.Log("RecvString:" + str);
            return retval.Length;
        }

        public string RecvString()
        {
            RecvString(out string str);
            return str;
        }

#if UNITY_WEBGL//WebGL环境

#if !UNITY_EDITOR//WebGL环境-运行时

        [DllImport("__Internal")]
        private static extern int SocketCreate(string url, string protocols);

        [DllImport("__Internal")]
        private static extern int SocketState(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketSend(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern void SocketRecv(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern int SocketRecvLength(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketClose(int socketInstance);

        [DllImport("__Internal")]
        private static extern int SocketError(int socketInstance, byte[] ptr, int length);

        int m_NativeRef = 0;

        public int Send(byte[] buffer)
        {
            SocketSend(m_NativeRef, buffer, buffer.Length);
            return buffer.Length;
        }

        public byte[] Recv()
        {
            int length = SocketRecvLength(m_NativeRef);
            if (length == 0)
            {
                return null;
            }
            byte[] buffer = new byte[length];
            SocketRecv(m_NativeRef, buffer, length);
            return buffer;
        }

        public void Connect()
        {
            m_NativeRef = SocketCreate(mUrl.ToString(), this.protocols);
        }

        public void Close()
        {
            if (m_NativeRef != 0)
            {
                SocketClose(m_NativeRef);
                m_NativeRef = 0;
            }
        }

        public bool Connected
        {
            get { return SocketState(m_NativeRef) != 0; }
        }

        public string Error
        {
            get
            {
                const int bufsize = 1024;
                byte[] buffer = new byte[bufsize];
                int result = SocketError(m_NativeRef, buffer, bufsize);

                if (result == 0)
                    return null;

                return Encoding.UTF8.GetString(buffer);
            }
        }
#else//WebGL环境-编辑器

        private WebSocketSharp.WebSocket m_Socket;
        private Queue<byte[]> m_Messages = new Queue<byte[]>();

        private string m_Error = null;
        public string Error => m_Error;

        private bool m_IsConnected = false;
        public bool Connected => m_IsConnected;

        public void Connect()
        {
            m_Socket = new WebSocketSharp.WebSocket(mUrl.ToString(), string.IsNullOrEmpty(protocols) ? Empty<string>.Array : new string[] { protocols });
            m_Socket.SslConfiguration.EnabledSslProtocols = m_Socket.SslConfiguration.EnabledSslProtocols | SslProtocols.Tls12 | SslProtocols.Tls11;
            m_Socket.OnMessage += (sender, e) => m_Messages.Enqueue(e.RawData);
            m_Socket.OnOpen += (sender, e) =>
            {
                m_IsConnected = true;
            };
            m_Socket.OnClose += (sender, e) =>
            {
                m_IsConnected = false;
            };
            m_Socket.OnError += (sender, e) =>
            {
                m_IsConnected = false;
                m_Error = e.Message + (e.Exception == null ? "" : " / " + e.Exception);
            };
            m_Socket.ConnectAsync();
        }

        public int Send(byte[] buffer)
        {
            m_Socket.Send(buffer);
            return buffer.Length;
        }

        public byte[] Recv()
        {
            if (m_Messages.Count == 0) return null;
            return m_Messages.Dequeue();
        }

        public void Close()
        {
            if (m_Socket != null)
            {
                m_Socket.Close();
                m_Socket = null;
            }
            m_IsConnected = false;
        }
#endif

#else//非WebGL环境

        public bool Connected => false;

        public string Error => "不支持当前平台对应的编辑器或运行时环境！";

        public void Connect() { }

        public int Send(byte[] buffer) => 0;

        public byte[] Recv() => null;

        public void Close() { }

#endif
    }
}