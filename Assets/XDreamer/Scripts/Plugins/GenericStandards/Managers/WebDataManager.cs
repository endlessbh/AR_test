using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;
using XCSJ.Message;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.GenericStandards.Managers
{
    /// <summary>
    /// 网络数据管理器
    /// </summary>
    public static class WebDataManager
    {
        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, WebDataInfo> webDataInfos { get; } = new Dictionary<string, WebDataInfo>();

        private static MonoBehaviour asyncMono => GlobalMB.instance;

        /// <summary>
        /// 请求数据，将结果回调到各种事件中
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="dataType"></param>
        /// <param name="readyCallback"></param>
        /// <param name="completedCallback"></param>
        /// <param name="failedCallback"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool Request(string uri, EDataType dataType, Action<WebDataInfo, object> readyCallback, Action<WebDataInfo, object> completedCallback, Action<WebDataInfo, object, object> failedCallback, object tag)
        {
            return TryStart(uri, dataType, out WebDataInfo webDataInfo, WebRequestAsync(uri, readyCallback, completedCallback, failedCallback, tag));
        }

        /// <summary>
        /// 请求数据，将结果回调到中文脚本的变量与函数中
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="callbackFun"></param>
        /// <param name="varOfResponseText"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool Request(string uri, string callbackFun, string varOfResponseText, string tag)
        {
            return TryStart(uri, EDataType.Text, out WebDataInfo webDataInfo, WebRequestAsync(uri, new ParamList("fun", callbackFun, "var", varOfResponseText, "tag", tag)));
        }

        /// <summary>
        /// 下载文件，将文件下载到本地文件夹中，并回调中文脚本函数
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="callbackFun"></param>
        /// <param name="varOfFileLocalPath"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool DownloadFile(string uri, string callbackFun, string varOfFileLocalPath, string tag)
        {
            return TryStart(uri, EDataType.File, out WebDataInfo webDataInfo, WebRequestAsync(uri, new ParamList("fun", callbackFun, "var", varOfFileLocalPath, "tag", tag)));
        }

        private static bool TryStart(string uri, EDataType dataType, out WebDataInfo webDataInfo, IEnumerator routine)
        {
            if (string.IsNullOrEmpty(uri))
            {
                webDataInfo = null;
                return false;
            }
            if (webDataInfos.TryGetValue(uri, out webDataInfo)) return false;

            webDataInfo = new WebDataInfo(uri, dataType);
            webDataInfos.Add(uri, webDataInfo);
            if (asyncMono.StartCoroutine(routine) is Coroutine coroutine)
            {
                webDataInfo.coroutine = coroutine;
                return true;
            }
            webDataInfos.Remove(uri);
            return false;
        }

        private static IEnumerator WebRequestAsync(string uri, Action<WebDataInfo, object> readyCallback, Action<WebDataInfo, object> completedCallback, Action<WebDataInfo, object, object> failedCallback, object tag)
        {
            //获取网络数据信息对象
            if (!webDataInfos.TryGetValue(uri, out WebDataInfo webDataInfo) || webDataInfo == null)
            {
                yield break;
            }

            //异步加载数据中...
            yield return DataHandler.LoadDataAsync(uri, webDataInfo.dataType, (di, o) =>
            {
                webDataInfo.dataInfo = di;
                readyCallback?.Invoke(webDataInfo, o);
            }, (di, o) =>
            {
                completedCallback?.Invoke(webDataInfo, o);
            }, (di, o, e) =>
            {
                failedCallback?.Invoke(webDataInfo, o, e);
            }, tag);

            //移除网络数据信息对象
            webDataInfos.Remove(uri);
        }

        private static IEnumerator WebRequestAsync(string uri, object tag)
        {
            yield return WebRequestAsync(uri, null, (wdi, o) =>
            {
                var di = wdi.dataInfo;

                //没有错误情况下，并且完成的情况下；
                bool ret = !di.isError && di.isDone;

                // 将消息回调到消息管理器-直接回调
                MsgManager.instance.SendMsg(EMsgID.WebResponseOfWebRequestTask, "url", uri, "ret", ret, "type", wdi.dataType, "tag", tag, "data", wdi, true);
            }, (wdi, o, e) =>
            {
                // 将消息回调到消息管理器-直接回调
                MsgManager.instance.SendMsg(EMsgID.WebResponseOfWebRequestTask, "url", uri, "ret", false, "type", wdi.dataType, "tag", tag, "data", wdi, true);
            }, tag);
        }

        /// <summary>
        /// 停止请求
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool StopRequest(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                return false;
            }
            if (webDataInfos.TryGetValue(uri, out WebDataInfo webDataInfo))
            {
                webDataInfos.Remove(uri);
                asyncMono.StopCoroutine(webDataInfo.coroutine);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通过URL获取请求的进度
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static float GetRequestProgressByURL(string uri)
        {
            if (webDataInfos.TryGetValue(uri, out WebDataInfo webDataInfo) && webDataInfo != null)
            {
                return webDataInfo.dataInfo.progress;
            }
            return -1;
        }

        /// <summary>
        /// 获取请求的数目
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static int GetRequestCount(EDataType dataType)
        {
            return webDataInfos.Count(kv => kv.Value.dataType == dataType);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static bool SaveFile(string filePath, byte[] bytes)
        {
            DirectoryHelper.Create(filePath);
            return FileHelper.SaveFile(filePath, bytes);
        }

        /// <summary>
        /// 临时文件夹目录；所有下载的文件都默认下载到本文件夹下;
        /// </summary>
        /// <returns></returns>
        public static string TmpFloder() => Product.temporaryCachePath;

        /// <summary>
        /// 清空临时文件夹
        /// </summary>
        public static bool ClearTmpFloder() => DirectoryHelper.Delete(TmpFloder(), true);

        /// <summary>
        /// 确保零时文件夹存在
        /// </summary>
        public static void EnsureTmpFloder()
        {
            DirectoryHelper.Create(TmpFloder());
        }
    }

    /// <summary>
    /// 网络数据信息类
    /// </summary>
    public class WebDataInfo
    {
        /// <summary>
        /// URI
        /// </summary>
        public string uri { get; private set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public EDataType dataType { get; private set; }

        /// <summary>
        /// 数据信息
        /// </summary>
        public IDataInfo dataInfo { get; internal set; }

        internal Coroutine coroutine { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="dataType"></param>
        internal WebDataInfo(string uri, EDataType dataType)
        {
            this.uri = uri;
            this.dataType = dataType;
        }
    }
}
