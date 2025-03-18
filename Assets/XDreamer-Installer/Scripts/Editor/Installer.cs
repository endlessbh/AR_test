using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

namespace XCSJ.EditorInstallers
{
    /// <summary>
    /// 安装器
    /// </summary>
    public static class Installer
    {
#if UNITY_EDITOR_WIN

        /// <summary>
        /// Hub主机
        /// </summary>
        public const string HubHost = "http://127.0.0.1:18520/";

        /// <summary>
        /// Hub端口
        /// </summary>
        public const int HubPort = 18520;

        /// <summary>
        /// 主机前缀
        /// </summary>
        public const string HostPrefix = "http://127.0.0.1:";

        /// <summary>
        /// 端口
        /// </summary>
        public static int Port { get; private set; } = 0;

        /// <summary>
        /// 获取主机
        /// </summary>
        /// <returns></returns>
        static string GetHost() => HostPrefix + Port + "/";

        /// <summary>
        /// 遍历端口
        /// </summary>
        /// <param name="func"></param>
        public static void ForeachPort(Func<int, bool> func)
        {
            if (func == null) return;

            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            var tcpListenInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            var udpConnInfoArray = ipGlobalProperties.GetActiveUdpListeners();

            foreach (var tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint is IPEndPoint point)
                {
                    if (!func(point.Port)) return;
                }
            }

            foreach (var tcpi in tcpListenInfoArray)
            {
                if (tcpi is IPEndPoint point)
                {
                    if (!func(point.Port)) return;
                }
            }

            foreach (var udpI in udpConnInfoArray)
            {
                if (udpI is IPEndPoint point)
                {
                    if (!func(point.Port)) return;
                }
            }
        }

        /// <summary>
        /// 检查端口有效性
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CheckAvailablePort(int port)
        {
            if (port < 1024 || port >= 65535) return false;

            bool valid = true;
            ForeachPort(p =>
            {
                if (p == port)
                {
                    valid = false;
                    return false;
                }
                return true;
            });

            return valid;
        }

        /// <summary>
        /// 获取有效端口
        /// </summary>
        /// <param name="startPort"></param>
        /// <returns></returns>
        public static int GetAvailablePort(int startPort)
        {
            if (startPort < 1024 || startPort >= 65535) return 0;

            var usedPorts = new HashSet<int>();
            ForeachPort(p =>
            {
                usedPorts.Add(p);
                return true;
            });
            for (int i = startPort; i < 65535; i++)
            {
                if (!usedPorts.Contains(i)) return i;
            }
            return 0;
        }

        static HttpListener httpListener;

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            if (Application.isBatchMode) return;

            EditorApplication.update += Invoke;

            _searchParams = GetSearchParamsString(("projectpath", System.IO.Path.GetDirectoryName(Application.dataPath)), ("unityversion", Application.unityVersion));

            EditorApplication.playModeStateChanged += (mode =>
            {
                switch (mode)
                {
                    case PlayModeStateChange.EnteredEditMode:
                        {
                            state = "edit";
                            break;
                        }
                    default:
                        {
                            state = "play";
                            break;
                        }
                }
            });

            ServerHandle();

            EditorApplication.quitting += () =>
            {
                httpListener?.Stop();
            };
        }

        static string state = "edit";

        static string GetState() => GetSearchParamsString(("state", state));

        static void ServerHandle()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                Port = GetAvailablePort(HubPort + 1);
                if (Port == 0)
                {
                    Debug.LogWarning("未找到有效的本地服务端口");
                    return;
                }
                httpListener = new HttpListener();
                httpListener.Prefixes.Add(GetHost());
                httpListener.Start();

                ThreadPool.QueueUserWorkItem(state1 =>
                {
                    while (httpListener.IsListening)
                    {
                        Thread.Sleep(5000);
                        ReportToHub();
                    }
                });

                while (httpListener.IsListening)
                {
                    var context = httpListener.GetContext();
                    var request = context.Request;
                    var response = context.Response;
                    var result = OnServerHandle(request, response);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
                    response.ContentLength64 = buffer.Length;
                    using (System.IO.Stream output = response.OutputStream)
                    {
                        output.Write(buffer, 0, buffer.Length);
                    }
                }
            });
        }

        static Type productType = null;
        static bool productTypeFlag = false;
        const string ProductTypeFullName = "XCSJ.PluginCommonUtils.Product";

        static Type GetProductType()
        {
            try
            {
                if (productTypeFlag) return productType;
                productTypeFlag = true;
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!assembly.Location.Contains("XCSJ")) continue;
                        foreach (var type in assembly.GetTypes())
                        {
                            try
                            {
                                if (type?.FullName == ProductTypeFullName)
                                {
                                    productType = type;
                                    return productType;
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return default;
        }

        static string OnServerHandle(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                if (state != "edit")
                {
                    return "error";
                }
                var cmd = request.RawUrl.Substring(1, request.RawUrl.IndexOf("?") - 1);
                switch (cmd)
                {
                    case "install":
                        {
                            var version = request.QueryString["version"];
                            var filepath = request.QueryString["filepath"];
                            Call(() =>
                            {
                                try
                                {
                                    var type = GetProductType();
                                    if (type == null && !EditorApplication.isCompiling && !EditorApplication.isUpdating && !EditorApplication.isPlayingOrWillChangePlaymode)
                                    {
                                        AssetDatabase.ImportPackage(filepath, false);
                                    }
                                }
                                catch { }
                            });
                            return "success";
                        }
                    case "uninstall":
                        {
                            var version = request.QueryString["version"];
                            Call(() =>
                            {
                                if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && !EditorApplication.isPlayingOrWillChangePlaymode)
                                {
                                    ClearAllMacro();
                                    AssetDatabase.Refresh();
                                }
                            });
                            return "success";
                        }
                    case "refresh":
                        {
                            Call(() =>
                            {
                                if (!EditorApplication.isCompiling && !EditorApplication.isUpdating && !EditorApplication.isPlayingOrWillChangePlaymode)
                                {
                                    AssetDatabase.Refresh();
                                }
                            });
                            return "success";
                        }
                }
            }
            catch { }
            return "error";
        }

        static void ClearAllMacro()
        {
            var oldDefinesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (string.IsNullOrEmpty(oldDefinesString)) return;
            var list = oldDefinesString.Split(';').ToList();
            if (list.Count == 0) return;
            var defines = list.Where(defineName => !defineName.StartsWith("XDREAMER"));
            string definesString = string.Join(";", defines.ToArray());
            if (oldDefinesString == definesString) return;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definesString);
        }

        static void Invoke()
        {
            if (taskDatas.Count == 0) return;
            TaskData taskData = null;
            lock (taskDatas)
            {
                if (taskDatas.Count > 0)
                {
                    taskData = taskDatas.Dequeue();
                }
            }
            taskData?.action?.Invoke();
        }

        static void Call(Action callback)
        {
            if (callback == null) return;
            lock (taskDatas)
            {
                taskDatas.Enqueue(new TaskData() { action = callback });
            }
        }

        static Queue<TaskData> taskDatas = new Queue<TaskData>();

        class TaskData
        {
            public Action action;
        }

#if XDREAMER
        static string _version = null;
#endif

        /// <summary>
        /// 报告到Hub
        /// </summary>
        /// <param name="state"></param>
        public static void ReportToHub()
        {
            var version = "";
#if XDREAMER
            try
            {
                if (_version == null)
                {
                    var type = GetProductType();
                    if (type == null)
                    {
                        _version = "";
                        ClearAllMacro();
                    }
                    else
                    {
                        _version = (type.GetField("Version").GetValue(null) as string) ?? "";
                    }
                }
                version = _version;
            }
            catch { }
#endif
            CmdGet("unity", null, ("port", Port.ToString()), ("version", version));
        }

        static string _searchParams = "";

        static string GetURL(string cmd) => HubHost + cmd + "?" + _searchParams + GetState();

        static string GetSearchParamsString(params (string key, string value)[] searchParams)
        {
            var result = "";
            foreach (var kv in searchParams)
            {
                result += kv.key + "=" + UnityWebRequest.EscapeURL(kv.value) + "&";
            }
            return result;
        }

        /// <summary>
        /// 命令获取
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="callback"></param>
        /// <param name="searchParams"></param>
        public static void CmdGet(string cmd, Action<string, string> callback, params (string key, string value)[] searchParams)
        {
            Get(GetURL(cmd) + GetSearchParamsString(searchParams), callback);
        }

        /// <summary>
        /// 获取：多线程回调
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        public static void Get(string url, Action<string, string> callback)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    var request = WebRequest.CreateHttp(url);
                    var response = request.GetResponse();
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        //Debug.Log(url + "=>" + streamReader.ReadToEnd());
                        callback?.Invoke(url, streamReader.ReadToEnd());
                    }
                }
                catch// (Exception ex)
                {
                    callback?.Invoke(url, null);
                }
            });
        }

#endif
    }
}

