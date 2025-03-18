using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.GenericStandards.Managers
{
    /// <summary>
    /// 场景资源管理类;用于负责场景调度
    /// </summary>
    public static class SceneAssetsManager
    {
        /// <summary>
        /// 用于存储所有子场景的场景名;主场景即索引为0的场景，不在本字典中！
        /// </summary>
        public static readonly Dictionary<string, AssetBundle> sceneNameAssetsDictionary = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 正在从本地导入的文件
        /// </summary>
        public static IDataInfo dataInfo { get; private set; }

        private enum ESceneState
        {
            Free = 0,
            ImportBegin,//仅有导入处理时，在完成后会重置为 Free 
            ImportEnd,//只有在导入并加载处理，并正常导入完成后才会设置为本状态
            LoadBegin,
            LoadEnd,//理论上不会出现的状态，因为场景已经切换了!但是如果出现，说明导入场景场景失败了!
            ImportAndLoadBegin,
            ImportEndAndWaitLoadBegin,
        }

        /// <summary>
        /// 场景的导入/加载状态
        /// </summary>
        private static ESceneState sceneState = ESceneState.Free;

        /// <summary>
        /// 异步加载场景操作对象;用于获取当前正在加载的场景信息
        /// </summary>
        public static AsyncOperation asyncOperation { get; private set; }

        private static MonoBehaviour asyncMono => GlobalMB.instance;

        /// <summary>
        /// 检查场景有效性，目前仅检查场景名不为空
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private static bool _CheckSceneValid(string sceneName)
        {
            return !string.IsNullOrEmpty(sceneName);
        }

        /// <summary>
        /// 检查场景有效性, 通过在场景管理类中检查对应名称的场景是否存在并且有效；
        /// **问题: 在BuildSettings中添加打包在一起的场景，使用场景名称获取不到场景对象！后续完善检测机制时，再做对应的细节处理！**
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private static bool CheckSceneValid(string sceneName)
        {
            //Debug.Log("SceneExist 1: " + sceneName);
            if (string.IsNullOrEmpty(sceneName)) return false;

            if (sceneNameAssetsDictionary.ContainsKey(sceneName)) return true;

            {
                var scene = SceneManager.GetSceneByName(sceneName);
                //Debug.Log("SceneExist 1: " + sceneName + ", " + scene.buildIndex);
                if (scene != null && scene.IsValid()) return true;
                //                 {
                //                     Log.Info(scene.buildIndex);
                //                     Log.Info(scene.isDirty);
                //                     Log.Info(scene.isLoaded);
                //                     Log.Info(scene.name);
                //                     Log.Info(scene.path);
                //                     Log.Info(scene.rootCount);
                //                     Log.Info();
                //                     return true;
                //                 }
            }

            try
            {
                for (int i = 0; i < SceneManager.sceneCount; ++i)
                {
                    var scene = SceneManager.GetSceneAt(i);

                    //Debug.Log("SceneExist 2: " + sceneName + ", scene: " + scene.name);
                    if (scene != null && scene.name == sceneName && scene.IsValid())
                    {
                        //Debug.Log("SceneExist 2: " + sceneName + ", " + scene.buildIndex);
                        return true;
                    }
                }
            }
            catch { }

            //             try
            //             {
            //                 for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            //                 {
            //                     var scene = SceneManager.GetSceneAt(i);
            // 
            //                     //Debug.Log("SceneExist 2: " + sceneName + ", scene: " + scene.name);
            //                     if (scene != null && scene.name == sceneName && scene.IsValid())
            //                     {
            //                         return true;
            //                     }
            //                 }
            //             }
            //             catch { }
            //Debug.Log("SceneExist x: " + sceneName);
            return false;
        }

        /// <summary>
        /// 尝试获取场景名称
        /// </summary>
        /// <param name="index"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool TryGetSceneName(int index, out string sceneName)
        {
            if (index >= 0 && index < sceneNameAssetsDictionary.Count)
            {
                foreach (var kv in sceneNameAssetsDictionary)
                {
                    if (index == 0)
                    {
                        sceneName = kv.Key;
                        return true;
                    }
                    --index;
                }
            }
            sceneName = "";
            return false;
        }

        private static bool AddScene(string sceneName, AssetBundle ab, bool unloadIfHasSameSceneName = true)
        {
            if (string.IsNullOrEmpty(sceneName) || !ab) return false;

            AssetBundle tmpAB = null;
            if (sceneNameAssetsDictionary.TryGetValue(sceneName, out tmpAB) && tmpAB)
            {
                //卸载
                if (unloadIfHasSameSceneName) ab.Unload(true);
                return false;
            }
            sceneNameAssetsDictionary[sceneName] = ab;
            return true;
        }

        /// <summary>
        /// 加载主场景；在当前场景中只允许调用一次！！<br />
        /// 如果当前场景已经是主场景，则不做任何操作
        /// </summary>
        /// <param name="async"></param>
        /// <returns></returns>
        public static bool LoadMainScene(bool async = true)
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.buildIndex == 0) return true;
            return async ? LoadSceneAsync(0) : LoadScene(0);
        }

        /// <summary>
        /// 卸载指定的子场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool UnloadScene(string sceneName)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == sceneName) return false;
            AssetBundle ab;
            if (sceneNameAssetsDictionary.TryGetValue(sceneName, out ab))
            {
                //SceneManager.UnloadScene(kv.Key);
                //卸载
                ab.Unload(true);
                //从字典中删除
                sceneNameAssetsDictionary.Remove(sceneName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 卸载当前场景以外的全部子场景
        /// </summary>
        /// <returns></returns>
        public static bool UnloadAllSubSceneWithoutActiveScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            AssetBundle ab = null;
            foreach (var kv in sceneNameAssetsDictionary)
            {
                //Debug.Log("准备卸载场景: " + kv.Key);
                if (kv.Key == scene.name)
                {
                    ab = kv.Value;
                    //Debug.Log("当前激活的场景不执行卸载操作: " + kv.Key);
                }
                else kv.Value.Unload(true);
                //下面这个方法不能卸载动态加载的场景！只能卸载打包时就在的
                //SceneManager.UnloadScene(kv.Key);
            }
            sceneNameAssetsDictionary.Clear();
            if (ab != null)
            {
                sceneNameAssetsDictionary.Add(scene.name, ab);
            }
            UnloadUnusedAssetAndGCCollect();
            return true;
        }

        /// <summary>
        /// 卸载全部子场景；当前场景是主场景（即索引为0的场景）时，才会执行卸载操作；
        /// </summary>
        /// <returns></returns>
        public static bool UnloadAllSubScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.buildIndex != 0) return false;
            foreach (var kv in sceneNameAssetsDictionary)
            {
                //Debug.Log("准备卸载场景: " + kv.Key);
                kv.Value.Unload(true);
                //下面这个方法不能卸载动态加载的场景！只能卸载打包时就在的
                //UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneName);
            }
            sceneNameAssetsDictionary.Clear();
            UnloadUnusedAssetAndGCCollect();
            return true;
        }

        /// <summary>
        /// 卸载所有的未使用的资产并调用GC回收
        /// </summary>
        public static void UnloadUnusedAssetAndGCCollect()
        {
            asyncMono.StartCoroutine(__UnloadUnusedAssetAndGCCollect(() => { }));
        }

        private static IEnumerator __UnloadUnusedAssetAndGCCollect(Action action)
        {
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            action?.Invoke();
        }

        /// <summary>
        /// 是否繁忙中；即有资产正在导入或加载中；
        /// </summary>
        /// <returns></returns>
        public static bool IsBusy()
        {
            return (sceneState != ESceneState.Free || dataInfo != null || asyncOperation != null);
        }

        /// <summary>
        /// 尝试获取异步导入与加载场景的进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static bool TryGetAsyncImportAndLoadSceneProgress(out float progress)
        {
            if (dataInfo != null)
            {
                progress = dataInfo.progress / 2f;
                return true;
            }
            if (asyncOperation != null)
            {
                progress = (1.0f + asyncOperation.progress) / 2f;
                return true;
            }
            switch (sceneState)
            {
                case ESceneState.ImportBegin:
                case ESceneState.LoadBegin:
                case ESceneState.ImportAndLoadBegin:
                    {
                        progress = 0;
                        return true;
                    }
                case ESceneState.ImportEnd:
                case ESceneState.LoadEnd://不应该出现的状态
                case ESceneState.ImportEndAndWaitLoadBegin:
                    {
                        progress = 0.5f;
                        return true;
                    }
            }
            progress = 0;
            return false;
        }

        /// <summary>
        /// 尝试获取异步导入场景的进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static bool TryGetAsyncImportSceneProgress(out float progress)
        {
            if (dataInfo != null)
            {
                progress = dataInfo.progress;
                return true;
            }
            switch (sceneState)
            {
                case ESceneState.ImportBegin:
                case ESceneState.ImportAndLoadBegin:
                    {
                        progress = 0;
                        return true;
                    }
                case ESceneState.ImportEnd:
                case ESceneState.ImportEndAndWaitLoadBegin:
                    {
                        progress = 1;
                        return true;
                    }
            }
            progress = 0;
            return false;
        }

        /// <summary>
        /// 尝试获取异步加载场景的进度
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static bool TryGetAsyncLoadSceneProgress(out float progress)
        {
            if (asyncOperation != null)
            {
                progress = asyncOperation.progress;
                return true;
            }
            switch (sceneState)
            {
                case ESceneState.LoadBegin:
                case ESceneState.ImportAndLoadBegin:
                case ESceneState.ImportEndAndWaitLoadBegin:
                    {
                        progress = 0;
                        return true;
                    }
            }
            progress = 0;
            return false;
        }

        /// <summary>
        /// 导入并加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool ImportAndLoadScene(string scenePath, string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (ImportScene(scenePath, sceneName))
            {
                return LoadScene(sceneName, mode);
            }
            return false;
        }

        /// <summary>
        /// 异步导入并加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="failFun"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool ImportAndLoadSceneAsync(string scenePath, string sceneName, LoadSceneMode mode = LoadSceneMode.Single, string failFun = "", string tag = "")
        {
            return ImportAndLoadSceneAsync(scenePath, sceneName, mode, (ok, error) => ScriptManager.CallUDF(failFun, ok ? tag : error));
        }

        /// <summary>
        /// 异步导入并加载场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool ImportAndLoadSceneAsync(string scenePath, string sceneName, LoadSceneMode mode, Action<bool, string> action)
        {
            //Debug.Log("ImprotAndLoadSceneAsync 0: " + sceneName + ",  路径: " + scenePath);
            //已经有异步导入或加载操作在进行中
            if (IsBusy()) return false;

            //Debug.Log("ImprotAndLoadSceneAsync 00: " + sceneName + ",  路径: " + scenePath);
            //已经存在一个同名的场景
            if (CheckSceneValid(sceneName)) return false;

            //Debug.Log("ImprotAndLoadSceneAsync 1: " + sceneName + ",  路径: " + scenePath);
            return asyncMono.StartCoroutine(__ImprotAndLoadSceneAsync(scenePath, sceneName, mode, action)) != null;
        }

        private static IEnumerator __ImprotAndLoadSceneAsync(string scenePath, string sceneName, LoadSceneMode mode, Action<bool, string> action)
        {
            //Debug.Log("__ImprotAndLoadSceneAsync 0: " + sceneName + ",  路径: " + scenePath);
            sceneState = ESceneState.ImportAndLoadBegin;

            yield return new WaitForEndOfFrame();
            //Debug.Log("__ImprotAndLoadSceneAsync 1: " + sceneName);
            yield return __ImprotSceneAsync(scenePath, sceneName, action);

            //Debug.Log("__ImprotAndLoadSceneAsync 2: " + sceneName);
            if (sceneState == ESceneState.Free) yield break;//导入发生错误了！！
            sceneState = ESceneState.ImportEndAndWaitLoadBegin;
            yield return new WaitForEndOfFrame();

            //Debug.Log("__ImprotAndLoadSceneAsync 3: " + sceneName);
            yield return __LoadSceneAsync(sceneName, mode, action);
            //Debug.Log("__ImprotAndLoadSceneAsync 4: " + sceneName);
        }

        /// <summary>
        /// 导入场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static bool ImportScene(string scenePath, string sceneName)
        {
            try
            {
                //已经有异步导入或加载操作在进行中
                if (IsBusy()) return false;

                //Debug.Log("ImportScene 0: " + sceneName + ",  路径: " + scenePath);
                //已经存在一个同名的场景
                if (CheckSceneValid(sceneName)) return false;

                //Debug.Log("ImportScene 1: " + sceneName);

                return DataHandler.LoadData(PathHandler.GetFilePath(scenePath), EDataType.AssetBundle, (di, o) =>
                {
                    AssetBundle ab = di.assetBundle;

                    //Debug.Log("ImportScene 2: " + sceneName + ",  loadfile: " + string.IsNullOrEmpty(loadfile.error) + ", ab: " + (ab != null));
                    // ab为false原因，可能相同的资源已经被加载导致
                    if ((di.isDone && !di.isError && ab))
                    {
                        AddScene(sceneName, ab);
                    }
                }, null, null);
            }
            catch// (Exception ex)
            {
                //理论上不会执行到本处
            }
            return false;
        }

        /// <summary>
        /// 异步导入场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="sceneName"></param>
        /// <param name="fun"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool ImportSceneAsync(string scenePath, string sceneName, string fun = "", string tag = "")
        {
            return ImportSceneAsync(scenePath, sceneName, (ok, error) => ScriptManager.CallUDF(fun, ok ? tag : error));
        }

        /// <summary>
        /// 异步导入场景
        /// </summary>
        /// <param name="scenePath"></param>
        /// <param name="sceneName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool ImportSceneAsync(string scenePath, string sceneName, Action<bool, string> action)
        {
            //Debug.Log("ImportSceneAsync 1: " + sceneName + ",  路径: " + scenePath);
            //已经有异步导入或加载操作在进行中
            if (IsBusy()) return false;

            //Debug.Log("ImportSceneAsync 2: " + sceneName + ",  路径: " + scenePath);
            //已经存在一个同名的场景
            if (CheckSceneValid(sceneName)) return false;

            //Debug.Log("ImportSceneAsync 3: " + sceneName + ",  路径: " + scenePath);
            //开启异步导入操作
            return asyncMono.StartCoroutine(__ImprotSceneAsync(scenePath, sceneName, action)) != null;
        }

        private static IEnumerator __ImprotSceneAsync(string scenePath, string sceneName, Action<bool, string> action)
        {
            bool onlyImport = (sceneState == ESceneState.Free);
            sceneState = ESceneState.ImportBegin;

            //Debug.Log("__ImprotSceneAsync 0: " + sceneName + ",  sceneState: " + sceneState);
            yield return DataHandler.LoadDataAsync(PathHandler.GetFilePath(scenePath), EDataType.AssetBundle, (di, o) =>
            {
                dataInfo = di;
            }, (di, o) =>
            {
                //Debug.Log("__ImprotSceneAsync 1: " + sceneName);
                var ab = di.assetBundle;
                //Debug.Log("__ImprotSceneAsync 2:" + ab + ", name: " + (ab ? ab.name : "nullxx"));
                //Debug.Log("__ImprotSceneAsync 2.1:" + ab);

                //获取执行结果
                bool ok = dataInfo.isDone && !dataInfo.isError && AddScene(sceneName, ab);
                string error = "";

                //判断执行结果
                if (ok)
                {
                    sceneState = onlyImport ? ESceneState.Free : ESceneState.ImportEnd;
                }
                else
                {
                    sceneState = ESceneState.Free;
                    error = dataInfo.isError ? dataInfo.error : "导入时场景同名冲突或场景资源无效!";
                }

                //将 数据信息 对象设置为空
                dataInfo = null;

                //执行回调
                if (sceneState == ESceneState.Free && action != null) action(ok, error);
                //Debug.Log("__ImprotSceneAsync 3:" + sceneName + ", sceneState: " + sceneState + ", ok: " + ok + ", error: " + error);
            }, (di, o, e) =>
            {
                sceneState = ESceneState.Free;
                //将 数据信息 对象设置为空
                dataInfo = null;
                action?.Invoke(false, e != null ? e.ToString() : "导入场景时发生未知错误!");
            }, null);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneBuildIndex"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single)
        {
            //检查索引范围
            if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings) return false;

            //已经有异步导入或加载操作在进行中
            if (IsBusy()) return false;

            //开始同步加载
            SceneManager.LoadScene(sceneBuildIndex, mode);
            return true;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneBuildIndex"></param>
        /// <param name="mode"></param>
        /// <param name="failFun"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static bool LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single, string failFun = "", string tag = "")
        {
            return LoadSceneAsync(sceneBuildIndex, mode, (ok, error) => ScriptManager.CallUDF(failFun, ok ? tag : error));
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneBuildIndex"></param>
        /// <param name="mode"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode, Action<bool, string> action)
        {
            //检查索引范围
            if (sceneBuildIndex < 0 || sceneBuildIndex >= SceneManager.sceneCountInBuildSettings) return false;

            //已经有异步导入或加载操作在进行中
            if (IsBusy()) return false;

            //开启异步加载
            return asyncMono.StartCoroutine(__LoadSceneAsync(sceneBuildIndex, mode, action)) != null;
        }

        private static IEnumerator __LoadSceneAsync(int sceneBuildIndex, LoadSceneMode mode, Action<bool, string> action)
        {
            __LoadSceneAsyncBegin();

            yield return new WaitForEndOfFrame();
            asyncOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, mode);
            yield return asyncOperation;

            __LoadSceneAsyncEnd(action);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="checkSceneValid"></param>
        /// <returns></returns>
        public static bool LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, bool checkSceneValid = false)
        {
            //已经有异步导入或加载操作在进行中
            if (IsBusy()) return false;

            //未找到对应名称的场景
            if (checkSceneValid && !CheckSceneValid(sceneName)) return false;
            else if (!_CheckSceneValid(sceneName)) return false;

            //开始同步加载
            SceneManager.LoadScene(sceneName, mode);
            return true;
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="failFun"></param>
        /// <param name="tag"></param>
        /// <param name="checkSceneValid"></param>
        /// <returns></returns>
        public static bool LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, string failFun = "", string tag = "", bool checkSceneValid = false)
        {
            return LoadSceneAsync(sceneName, mode, (ok, error) => ScriptManager.CallUDF(failFun, ok ? tag : error), checkSceneValid);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="mode"></param>
        /// <param name="action"></param>
        /// <param name="checkSceneValid"></param>
        /// <returns></returns>
        public static bool LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<bool, string> action, bool checkSceneValid = false)
        {
            //已经有异步导入或加载操作在进行中
            if (IsBusy()) return false;

            //未找到对应名称的场景
            if (checkSceneValid && !CheckSceneValid(sceneName)) return false;
            else if (!_CheckSceneValid(sceneName)) return false;

            //开启异步加载
            return asyncMono.StartCoroutine(__LoadSceneAsync(sceneName, mode, action)) != null;
        }

        private static IEnumerator __LoadSceneAsync(string sceneName, LoadSceneMode mode, Action<bool, string> action)
        {
            __LoadSceneAsyncBegin();

            yield return new WaitForEndOfFrame();
            asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            yield return asyncOperation;

            __LoadSceneAsyncEnd(action);
        }


        private static void __LoadSceneAsyncBegin()
        {
            if (sceneState == ESceneState.Free) sceneState = ESceneState.LoadBegin;
        }

        /// <summary>
        /// 使用 XDreamer.Root ，在场景切换后,游戏对象销毁，导致本函数不执行！
        /// 使用 GlobalComponent.instance ，场景切换本游戏对象唯一，所以依然会回调本函数!
        /// </summary>
        /// <param name="action"></param>
        private static void __LoadSceneAsyncEnd(Action<bool, string> action)
        {
            try
            {
                //Log.Debug("__LoadSceneAsyncEnd");
                sceneState = ESceneState.LoadEnd;

                if (asyncOperation == null)
                {
                    action?.Invoke(false, "异步加载场景失败");
                }
                else
                {
                    //Log.DebugFormat("asyncOperation: {0}, {1}, {2}, {3}", asyncOperation.isDone, asyncOperation.progress, asyncOperation.priority, asyncOperation.allowSceneActivation);
                    action?.Invoke(true, "异步加载场景成功");
                }
            }
            catch { }
            finally
            {
                asyncOperation = null;
                sceneState = ESceneState.Free;
            }
        }
    }

    /// <summary>
    /// 场景信息
    /// </summary>
    public class SceneInfo
    {
        /// <summary>
        /// 场景对象
        /// </summary>
        public Scene scene { get; internal set; }

        /// <summary>
        /// 资产包
        /// </summary>
        public AssetBundle assetBundle { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="assetBundle"></param>
        public SceneInfo(Scene scene, AssetBundle assetBundle)
        {
            this.scene = scene;
            this.assetBundle = assetBundle;
        }
    }
}
