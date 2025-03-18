using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using XCSJ.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    public class DefaultDataHandler : InstanceClass<DefaultDataHandler>, IDataHandler
    {
        public byte[] GetDataFromStreamingAssetsPath(string fromRelativePath, EDataPlatform dataPlatform)
        {
            switch (dataPlatform)
            {
                case EDataPlatform.Android:
                    {
                        return LoadData(new DataRequest("jar:file://" + Application.dataPath + "!/assets/" + fromRelativePath, EDataType.Bytes))?.bytes;
                    }
                case EDataPlatform.iOS:
                    {
                        return LoadData(new DataRequest("file://" + Application.dataPath + "/Raw/" + fromRelativePath, EDataType.Bytes))?.bytes;
                    }
                case EDataPlatform.OSX:
                case EDataPlatform.Windows:
                    {
                        try
                        {
                            return File.ReadAllBytes(Application.streamingAssetsPath + "\\" + fromRelativePath);
                        }
                        catch
                        {
                            return null;
                        }
                    }
                case EDataPlatform.RuntimePlatform:
                    {
                        return GetDataFromStreamingAssetsPath(fromRelativePath, DataHandler.GetDataPlatform(Application.platform));
                    }
                default:
                    {
                        throw new InvalidOperationException(string.Format("运行时平台[{0}]不支持从 StreamingAssetsPath:[{1}] 获取数据!", Application.platform.ToString(), Application.streamingAssetsPath));
                    }
            }
        }

        public IEnumerator LoadDataAsync(DataRequest dataRequest, Action<IDataInfo, object> readyCallback, Action<IDataInfo, object> completedCallback, Action<IDataInfo, object, object> failedCallback, object tag)
        {
            if (dataRequest == null)
            {
                failedCallback?.Invoke(null, tag, string.Format("输入参数[{0}]无效!", nameof(dataRequest)));
                yield break;
            }
            if (string.IsNullOrEmpty(dataRequest.uri))
            {
                failedCallback?.Invoke(null, tag, string.Format("输入参数[{0}].[{1}]无效!", nameof(dataRequest), nameof(dataRequest.uri)));
                yield break;
            }

            yield return new WaitForEndOfFrame();

#if UNITY_2018_3_OR_NEWER

            if (GetUnityWebRequest(dataRequest.uri, dataRequest.dataType) is UnityWebRequest request)
            {
                request.timeout = dataRequest.timeout;

                var dataInfo = new UnityWebRequestDataInfo(request, dataRequest);
                readyCallback?.Invoke(dataInfo, tag);

                yield return request.SendWebRequest();

                completedCallback?.Invoke(dataInfo, tag);
            }
            else
            {
                failedCallback?.Invoke(null, tag, string.Format("输入参数[{0}]:[{1}]无效!", nameof(dataRequest.dataType), dataRequest.dataType.ToString()));
            }
#else
            using (var www = new WWW(dataRequest.uri))
            {
                var dataInfo = new WWWDataInfo(www, dataRequest);
                readyCallback?.Invoke(dataInfo, tag);

                yield return www;

                completedCallback?.Invoke(dataInfo, tag);
            }
#endif
        }

        public bool LoadData(DataRequest dataRequest, Action<IDataInfo, object> completedCallback, Action<IDataInfo, object, object> failedCallback, object tag)
        {
            try
            {
                if (dataRequest == null)
                {
                    failedCallback?.Invoke(null, tag, string.Format("输入参数[{0}]无效!", nameof(dataRequest)));
                    return false;
                }
                if (string.IsNullOrEmpty(dataRequest.uri))
                {
                    failedCallback?.Invoke(null, tag, string.Format("输入参数[{0}]:[{1}]无效!", nameof(dataRequest.uri), dataRequest.uri));
                    return false;
                }

#if UNITY_2018_3_OR_NEWER
                if (GetUnityWebRequest(dataRequest.uri, dataRequest.dataType) is UnityWebRequest request)
                {
                    request.SendWebRequest();
                    while (!request.isDone && request.downloadProgress < 1)
                    {
#if UNITY_2020_2_OR_NEWER
                        if (request.result == UnityWebRequest.Result.ConnectionError)
#else
                        if (request.isNetworkError)
#endif
                        {
                            failedCallback?.Invoke(null, tag, string.Format("从[{0}]路径获取数据时发生错误:{1}", dataRequest.uri, request.error));
                            return false;
                        }
                    }
                    completedCallback?.Invoke(new UnityWebRequestDataInfo(request, dataRequest), tag);
                }
                else
                {
                    failedCallback?.Invoke(null, tag, string.Format("输入参数[{0}]:[{1}]无效!", nameof(dataRequest.dataType), dataRequest.dataType.ToString()));
                    return false;
                }
#else
                var www = new WWW(dataRequest.uri);
                while (!www.isDone)
                {
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        //下载失败抛出异常
                        failedCallback?.Invoke(null, tag, string.Format("从[{0}]路径获取数据时发生错误:{1}", dataRequest.uri, www.error));
                        return false;
                    }
                }
                completedCallback?.Invoke(new WWWDataInfo(www, dataRequest), tag);
#endif
                        return true;
            }
            catch (Exception ex)
            {
                failedCallback?.Invoke(null, tag, ex);
            }
            return false;
        }

        public IDataInfo LoadData(DataRequest dataRequest)
        {
            IDataInfo dataInfo = null;
            LoadData(dataRequest, (di, o) => dataInfo = di, null, null);
            return dataInfo;
        }

        public UnityWebRequest GetUnityWebRequest(string uri, EDataType dataType)
        {
            switch (dataType)
            {
                case EDataType.AssetBundle:
                    {
                        return UnityWebRequestAssetBundle.GetAssetBundle(uri);
                    }
                case EDataType.Texture:
                    {
                        return UnityWebRequestTexture.GetTexture(uri);
                    }
                case EDataType.AudioClip:
                    {
                        return UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.UNKNOWN);
                    }
                case EDataType.Text:
                case EDataType.Bytes:
                case EDataType.File:
                case EDataType.Unknow:
                    {
                        return UnityWebRequest.Get(uri);
                    }
                default:
                    {
                        return null;
                    }
            }
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoadMethod]
        public static void InitInEditor()
        {
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static bool inChangePlayModeState = false;

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange playModeStateChange)
        {
            switch(playModeStateChange)
            {
                case UnityEditor.PlayModeStateChange.EnteredEditMode:
                case UnityEditor.PlayModeStateChange.EnteredPlayMode:
                    {
                        inChangePlayModeState = false;
                        break;
                    }
                default:
                    {
                        inChangePlayModeState = true;
                        break;
                    }
            }
        }

#endif

        public GameObject FindGameObject(string name)
        {
#if UNITY_EDITOR
            //Debug.Log(inChangePlayModeState);
            if (inChangePlayModeState) return null;
#endif
            //1、必须保证首字母开头是 "/", 即游戏对面名称路径的Unity输出规则
            //if (!name.StartsWith("/"))
            //{
            //    //不符合规则时，尝试使用别名系统进行查找
            //    return Alias.GetGameObject(name);
            //}
            var go = GameObject.Find(name);
            if (go) return go;
            //2、拆分字符串
            string[] nameArray = name.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (nameArray == null || nameArray.Length == 0) return null;
#if UNITY_5_4_OR_NEWER
            //3、使用Unity的场景管理类进行查找~~
            //注意：Unity5.3.4f1（版本以下）的使用以下代码会出错，没有对应Unity API 实现
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene != null)
            {
                foreach (GameObject tempGO in scene.GetRootGameObjects())
                {
                    if (tempGO.name == nameArray[0])
                    {
                        go = tempGO;
                        break;
                    }
                }
                if (go)
                {
                    for (int i = 1; i < nameArray.Length; ++i)
                    {
                        go = CommonFun.GetChildGameObject(go.transform, nameArray[i]);
                        if (!go) break;
                    }
                }
                if (go) return go;
            }
#endif
            //4、获取最后的模型名，并重新生成标准化的连接字符串 
            string goName = nameArray[nameArray.Length - 1];
            string names = "";
            foreach (string tempName in nameArray)
            {
                names = names + "/" + tempName;
            }
            //5、开始递归查找：开启变态查找方法 -- 全局所有的GameObject      
            foreach (GameObject tempGO in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (tempGO == null || tempGO.name != goName) continue;
                if (CommonFun.GameObjectToString(tempGO) == names) return tempGO;
            }
            return null;
        }

        public static event Func<MemberInfo, Texture2D> onGetIconInLibByMemberInfo;

        public Texture2D GetIconInLib(MemberInfo memberInfo, Texture2D defaultTexture = null)
        {
            if (memberInfo == null) return defaultTexture;
            return onGetIconInLibByMemberInfo.FirstOrDefault(t => t, memberInfo, defaultTexture);
        }

        public static event Func<string, Texture2D> onGetIconInLibByPath;

        public Texture2D GetIconInLib(string path, Texture2D defaultTexture = null)
        {
            if (string.IsNullOrEmpty(path)) return defaultTexture;
            return onGetIconInLibByPath.FirstOrDefault(t => t, path, defaultTexture);
        }

        public static event Func<string, XGUIStyle> onGetStyle;

        public XGUIStyle GetStyle(string name)
        {
            XGUIStyle style = null;
            if (!string.IsNullOrEmpty(name))
            {
                style = onGetStyle.FirstOrDefault(t => t != null, name);
            }
            if (style == null)
            {
                style = new XGUIStyle("", s => s = GUIStyle.none);
            }
            return style;
        }
    }
}
