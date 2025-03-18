using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.Net;
using XCSJ.Net.Http;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Kernel
{
    /// <summary>
    /// Unity Web请求客户端
    /// </summary>
    public class UnityWebRequestClient : BaseClient, INetPackageAsync
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
        /// Web请求
        /// </summary>
        public UnityWebRequest webRequest { get; private set; }

        /// <summary>
        /// 关闭
        /// </summary>
        public override void Close()
        {
            clientState = EClientState.Closing;
            webRequest = null;
            url = "";
            StopAsync();
            clientState = EClientState.Closed;
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            if (IsConnected()) return true;
            webRequest = null;
            clientState = string.IsNullOrEmpty(url) ? EClientState.ConnectFail : EClientState.Connected;
            StartAsync();
            return IsConnected();
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
        /// 读取
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Read(out string data)
        {
            if (!IsConnected() || webRequest == null)
            {
                data = null;
                return 0;
            }

            webRequest.SendWebRequest();
            while (!webRequest.isDone)
            {

#if UNITY_2020_2_OR_NEWER

                if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError || !string.IsNullOrEmpty(webRequest.error))
                {
                    data = null;
                    return 0;
                }

#else

                if (webRequest.isHttpError || webRequest.isNetworkError || !string.IsNullOrEmpty(webRequest.error))
                {
                    data = null;
                    return 0;
                }

#endif
            }

            var ret = webRequest.downloadedBytes;
            data = webRequest.downloadHandler.text;
            webRequest = null;
            return (int)ret;
        }

        private object sendLocker = new object();

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override int Write(string data)
        {
            if (!IsConnected()) return 0;
            lock (sendLocker)
            {
                if (webRequest != null) return 0;

                var form = new WWWForm();
                form.AddField(HttpHelper.DefaultFormFieldName, data);
                webRequest = UnityWebRequest.Post(url, form);
                return form.data.Length;
            }
        }

        /// <summary>
        /// 有效异步模式
        /// </summary>
        public bool validAsyncMode => true;

        /// <summary>
        /// 当收到数据时
        /// </summary>

        public event Action<NetPackage> onReceived;

        private ListS<NetPackage> questions = new ListS<NetPackage>();

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public bool Send(NetPackage package)
        {
            if (package == null) return false;
            questions.Add(package);
            return true;
        }

        private Coroutine asyncCoroutine;

        private void StartAsync()
        {
            CommonFun.DelayCall(() =>
            {
                if (asyncCoroutine != null) return;
                asyncCoroutine = GlobalMB.instance.StartCoroutine(HandleAsync());
            });          
        }

        private void StopAsync()
        {
            CommonFun.DelayCall(() =>
            {
                if (asyncCoroutine == null) return;
                GlobalMB.instance.StopCoroutine(asyncCoroutine);
                asyncCoroutine = null;
            });           
        }

        private IEnumerator HandleAsync()
        {
            List<NetPackage> list = new List<NetPackage>();
            while (IsConnected())
            {
                if (!questions.CopyListAndClear(ref list, true))
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                foreach (var q in list)
                {
                    //Log.Debug("写入问题" + q.ToJson());
                    if (Write(q))
                    {
                        //等待发送
                        yield return webRequest.SendWebRequest();

                        //等待完成
                        yield return new WaitUntil(() => webRequest.isDone);

                        //错误处理
                        {

#if UNITY_2020_2_OR_NEWER

                            if (webRequest.result == UnityWebRequest.Result.ProtocolError || webRequest.result == UnityWebRequest.Result.ConnectionError || !string.IsNullOrEmpty(webRequest.error))//发生错误
                            {
                                onError?.Invoke(string.Format("执行Web请求[{0}]发生错误:{1}", url, webRequest.error));
                            }

#else

                            if (webRequest.isHttpError || webRequest.isNetworkError || !string.IsNullOrEmpty(webRequest.error))//发生错误
                            {
                                onError?.Invoke(string.Format("执行Web请求[{0}]发生错误:{1}", url, webRequest.error));
                            }
#endif
                        }
                        try
                        {
                            //var ret = webRequest.downloadedBytes;
                            var data = webRequest.downloadHandler.text;
                            var answerNetPackage = NetPackage.FromJson<AnswerNetPackage>(data);

                            onReceived?.Invoke(answerNetPackage);
                            if(onReceived==null)
                            {
                                Debug.Log("空接收");
                            }
                            OnRead(answerNetPackage);
                        }
                        catch (Exception ex)
                        {
                            onError?.Invoke(string.Format("执行Web请求[{0}]发生异常:{1}", url, ex));
                        }
                        finally
                        {
                            webRequest = null;
                        }
                    }
                }
            }
            onExit?.Invoke();
        }

        private void OnRead(AnswerNetPackage answerNetPackage)
        {
            //Debug.Log("UnityWebRequestClient OnRead:" + answerNetPackage.ToJson());
            onRead?.Invoke(answerNetPackage);
        }

        private event Action<AnswerNetPackage> onRead;
        private event Action onExit;
        private event Action<object> onError;

        /// <summary>
        /// 开始读取答案
        /// </summary>
        /// <param name="onRead"></param>
        /// <param name="onExit"></param>
        /// <param name="onError"></param>
        public void StartReadAnswer(Action<AnswerNetPackage> onRead, Action onExit, Action<object> onError)
        {
            StopReadAnswer(onRead, onExit, onError);

            this.onRead += onRead;
            this.onExit += onExit;
            this.onError += onError;
        }

        /// <summary>
        /// 停止读取答案
        /// </summary>
        /// <param name="onRead"></param>
        /// <param name="onExit"></param>
        /// <param name="onError"></param>
        public void StopReadAnswer(Action<AnswerNetPackage> onRead, Action onExit, Action<object> onError)
        {
            this.onRead -= onRead;
            this.onExit -= onExit;
            this.onError -= onError;
        }
    }
}
