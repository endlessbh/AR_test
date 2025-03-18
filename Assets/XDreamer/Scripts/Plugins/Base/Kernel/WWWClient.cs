using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.Net;
using XCSJ.Net.Http;

namespace XCSJ.Extension.Base.Kernel
{
#if UNITY_2018_3_OR_NEWER
    [Obsolete("因Unity的WWW类被标记过时,所以WWW客户端类不再推荐使用!可使用UnityWebRequest客户端类替代!")]
#endif
    public class WWWClient : BaseClient
    {
        public override Address remoteAddress => new Address(url, port);

        public override Address localAddress => new Address();

        public string url = "";

        public int port = 0;

        public WWW www { get; private set; }

        public override void Close()
        {
            www = null;
            clientState = EClientState.Closed;
        }

        public override bool Connect()
        {
            if (IsConnected()) return true;
            clientState = string.IsNullOrEmpty(url) ? EClientState.ConnectFail : EClientState.Connected;
            return IsConnected();
        }

        public override bool Connect(IAddress serverAddress)
        {
            if (serverAddress == null) return false;
            if (IsConnected()) return true;
            this.url = serverAddress.address;
            this.port = serverAddress.port;
            return Connect();
        }

        public override int Read(out string data)
        {
            if (!IsConnected() || www == null)
            {
                data = null;
                return 0;
            }

            while (!www.isDone)
            {
                if (!string.IsNullOrEmpty(www.error))
                {
                    data = null;
                    return 0;
                }
            }

            var ret = www.bytesDownloaded;
            data = www.text;
            www = null;
            return ret;
        }

        public override int Write(string data)
        {
            if (!IsConnected()) return 0;
            var form = new WWWForm();
            form.AddField(HttpHelper.DefaultFormFieldName, data);
            www = new WWW(url, form);
            return form.data.Length;
        }
    }
}
