using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.ComponentModel;
using XCSJ.Extension.GenericStandards;
using XCSJ.Extension.GenericStandards.Managers;
using XCSJ.Helper;
using XCSJ.Message;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.Scripts;
using XCSJ.Extension.CNScripts.Base;
using XCSJ.Extension.CNScripts;

#if XDREAMER_ZXING
using XCSJ.PluginZXing;
using XCSJ.PluginCommonUtils.CNScripts;
#endif

#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace XCSJ.Extension
{
    /// <summary>
    /// 通用标准脚本：管理维护与Unity直接相关的各种中文脚本命令；包括：游戏对象、组件、资源对象、时间、IMGUI、UGUI、声音、动画、粒子、物理系统、二维向量、三维向量、文件等；
    /// </summary>
    [ExecuteInEditMode, DisallowMultipleComponent]
    [ComponentKit(EKit.Base)]
    [ComponentOption(EComponentOption.Recommended)]
    [Name("通用标准脚本")]
    [Tip("管理维护与Unity直接相关的各种中文脚本命令；包括：游戏对象、组件、资源对象、时间、IMGUI、UGUI、声音、动画、粒子、物理系统、二维向量、三维向量、文件等；", "Manage and maintain various Chinese script commands directly related to unity; Including: game object, component, resource object, time, imgui, ugui, sound, animation, particle, physical system, two-dimensional vector, three-dimensional vector, file, etc;")]
    [Guid("16C6D275-0AFC-49D0-8FE7-512ED7CAC45C")]
    [Serializable]
    [Version("23.428")]
    public sealed class GenericStandardScriptManager : BaseManager<GenericStandardScriptManager, EGenericStandardScriptID>
    {
        private HierarchyVarWindow hierarchyVarWindow = new HierarchyVarWindow();

        /// <summary>
        /// 通用材质：不指明具体材质情况下，底层程序代码处理时使用的通用材质;
        /// </summary>
        [Name("通用材质")]
        [Tip("不指明具体材质情况下，底层程序代码处理时使用的通用材质;", "The general material used in the processing of the underlying program code without specifying the specific material;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _commonMaterial;

        /// <summary>
        /// Unity资产对象列表
        /// </summary>
        [Name("Unity资产对象列表")]
        [Tip("用于记录中文脚本中可能存在引用的Unity资产对象", "Used to record unity asset objects that may be referenced in Chinese scripts")]
        [Array]
        public List<UnityEngine.Object> _objects = new List<UnityEngine.Object>();

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="obj"></param>
        public void Add(UnityEngine.Object obj)
        {
            if (!obj) return;
            if (!_objects.Contains(obj))
            {
                this.XModifyProperty(() =>
                {
                    _objects.Add(obj);
                });
                AddToUnityAssetObjectManager(obj);
            }
        }

        private void AddToUnityAssetObjectManager(UnityEngine.Object obj)
        {
            if (obj)
            {
                UnityAssetObjectManager.instance.AddBuffer(obj.GetType());
                UnityAssetObjectManager.instance.Add(obj);
            }
        }

        /// <summary>
        /// 添加到Unity资产对象管理器
        /// </summary>
        public void AddToUnityAssetObjectManager()
        {
            foreach (var obj in _objects)
            {
                AddToUnityAssetObjectManager(obj);
            }
        }

        #region 管理器接口

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue Init(ParamList param)
        {
            GUIManager.Init();
            WebCamManager.Init();
            TimerManager.Init();
            return base.Init(param);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public override void Release()
        {
            base.Release();
            GUIManager.Release();
            WebCamManager.Release();
            TimerManager.Release();
        }

        /// <summary>
        /// 消息回调
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override ReturnValue OnMsg(Msg msg)
        {
            switch ((GenericStandards.EMsgID)msg.ID.ID)
            {
                case GenericStandards.EMsgID.WebResponseOfWebRequestTask:
                    {
                        var webDataInfo = msg.Params["data"] as WebDataInfo;
                        if (webDataInfo == null) break;

                        var tagParamList = msg.Params["tag"] as ParamList;
                        if (tagParamList == null) break;

                        string fun = tagParamList["fun"] as string;
                        string var = tagParamList["var"] as string;
                        string tag = tagParamList["tag"] as string;

                        EDataType type = (EDataType)msg.Params["type"];
                        bool ret = (bool)msg.Params["ret"];

                        switch (type)
                        {
                            case EDataType.Text:
                                {
                                    ScriptManager.TrySetGlobalVariableValue(var, webDataInfo.dataInfo.text);
                                    ScriptManager.CallUDF(fun, ret ? tag : ReturnValue.FalseString);
                                    break;
                                }
                            case EDataType.File:
                                {
                                    string fileName = webDataInfo.dataInfo.url.Substring(webDataInfo.dataInfo.url.LastIndexOf("/") + 1);
                                    string filePath = WebDataManager.TmpFloder() + fileName;

                                    ScriptManager.TrySetGlobalVariableValue(var, filePath);
                                    ScriptManager.CallUDF(fun, (ret && WebDataManager.SaveFile(filePath, webDataInfo.dataInfo.bytes)) ? tag : ReturnValue.FalseString);
                                    break;
                                }
                        }
                        break;
                    }
            }
            return base.OnMsg(msg);
        }

        #endregion

        #region Mono方法

        private ScriptManager scriptManager;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            scriptManager = ScriptManager.instance;
            scriptManager.XGetOrAddComponent<StaticVarCollection>();
            scriptManager.XGetOrAddComponent<AppVarCollection>();
            scriptManager.XGetOrAddComponent<ReferenceVarCollection>();

            MsgManager.instance.AddListener(GenericStandards.EMsgID.WebResponseOfWebRequestTask, this);

            AddToUnityAssetObjectManager();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            MsgManager.instance.RemoveListener(GenericStandards.EMsgID.WebResponseOfWebRequestTask, this);
            foreach (var obj in _objects)
            {
                UnityAssetObjectManager.instance.Remove(obj);
            }
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        private void OnGUI()
        {
            GUIManager.OnGUI();
            hierarchyVarWindow.OnGUI();
        }

        #endregion

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EGenericStandardScriptID id, ScriptParamList param)
        {
            switch (id)
            {
                #region 通用标准

                case EGenericStandardScriptID.SetAnimatorSpeed:
                    {
                        var animator = param[1] as Animator;
                        if (!animator) break;
                        animator.speed = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetShader:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        var newShader = Shader.Find(param[2] as string);
                        if (!newShader) break;

                        var meshRenders = go.GetComponentsInChildren<Renderer>();
                        foreach (var mr in meshRenders)
                        {
                            Material[] materials = mr.materials;
                            for (int i = 0; i < materials.Length; i++)
                            {
                                materials[i].shader = newShader;
                            }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetModelLayerID:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        return ReturnValue.True(go.layer.ToString());
                    }
                case EGenericStandardScriptID.GetModelLayerName:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        return ReturnValue.True(LayerMask.LayerToName(go.layer));
                    }
                case EGenericStandardScriptID.SetModelLayerID:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        int layer = (int)param[2];
                        bool isIncludeChildren = ((EBool2)param[3] == EBool2.Yes);
                        if (isIncludeChildren)
                        {
                            Transform[] children = go.GetComponentsInChildren<Transform>();
                            foreach (Transform item in children)
                            {
                                item.gameObject.layer = layer;
                            }
                        }
                        else
                        {
                            go.layer = layer;
                        }

                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetModelLayerName:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        int layer = LayerMask.NameToLayer(param[2] as string);
                        if (layer >= 0 && layer < 32)
                        {
                            bool isIncludeChildren = ((EBool2)param[3] == EBool2.Yes);
                            if (isIncludeChildren)
                            {
                                Transform[] children = go.GetComponentsInChildren<Transform>();
                                foreach (Transform item in children)
                                {
                                    item.gameObject.layer = layer;
                                }
                            }
                            else
                            {
                                go.layer = layer;
                            }

                            return ReturnValue.Yes;
                        }
                        else
                        {
                            Debug.LogError(param[2] + "层不存在！");
                        }

                        break;
                    }
                case EGenericStandardScriptID.GetProfilerInfo:
                    {
                        switch (param[1] as string)
                        {
                            case "MonoHeapSize":
                                {
#if UNITY_2018_1_OR_NEWER
                                    return ReturnValue.True(Profiler.GetMonoHeapSizeLong().ToString());
#else

                                    return ReturnValue.True(Profiler.GetMonoHeapSize().ToString());
#endif
                                }
                            case "MonoUsedSize":
                                {
#if UNITY_2018_1_OR_NEWER
                                    return ReturnValue.True(Profiler.GetMonoUsedSizeLong().ToString());
#else

                                    return ReturnValue.True(Profiler.GetMonoUsedSize().ToString());
#endif
                                }
                            case "TotalAllocatedMemory":
                                {
#if UNITY_2018_1_OR_NEWER
                                    return ReturnValue.True(Profiler.GetTotalAllocatedMemoryLong().ToString());
#else
                                    return ReturnValue.True(Profiler.GetTotalAllocatedMemory().ToString());
#endif
                                }
                            case "TotalReservedMemory":
                                {
#if UNITY_2018_1_OR_NEWER
                                    return ReturnValue.True(Profiler.GetTotalReservedMemoryLong().ToString());
#else
                                    return ReturnValue.True(Profiler.GetTotalReservedMemory().ToString());
#endif
                                }
                            case "TotalUnusedReservedMemory":
                                {
#if UNITY_2018_1_OR_NEWER
                                    return ReturnValue.True(Profiler.GetTotalUnusedReservedMemoryLong().ToString());
#else
                                    return ReturnValue.True(Profiler.GetTotalUnusedReservedMemory().ToString());
#endif
                                }
                            case "usedHeapSize":
                                {
#if UNITY_2018_1_OR_NEWER                                    
                                    return ReturnValue.True(Profiler.usedHeapSizeLong.ToString());
#else
                                    return ReturnValue.True(Profiler.usedHeapSize.ToString());
#endif
                                }
                            case "supported":
                                {
                                    return ReturnValue.Create(Profiler.supported);
                                }
                            case "enabled":
                                {
                                    return ReturnValue.Create(Profiler.enabled);
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetProfilerEnabled:
                    {
                        Profiler.enabled = CommonFun.BoolChange(Profiler.enabled, (EBool)(param[1]));
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetGraphicColor:
                    {
                        Graphic graphic = param[1] as Graphic;
                        if (graphic)
                        {
                            var color = (Color)param[2];
                            graphic.color = color;
                            if ((EBool2)param[3] == EBool2.Yes)
                            {
                                graphic.gameObject.GetComponentsInChildren<Graphic>().ForeachLite(g =>
                                { if (g != graphic) g.color = color; });
                            }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetAngleWithTwoGameObjectForward:
                    {
                        var go1 = param[1] as GameObject;
                        var go2 = param[2] as GameObject;
                        if (go1 && go2)
                        {
                            return ReturnValue.True(Vector3.Angle(go1.transform.forward, go2.transform.forward));
                        }
                        return ReturnValue.No;
                    }
                #endregion

                #region 应用程序
                case EGenericStandardScriptID.FullWindowChange:
                    {
                        string oper = param[1] as string;
                        switch (oper)
                        {
                            case "开启":
                                {
                                    SystemRuntimeInfo.height = Screen.height;
                                    SystemRuntimeInfo.width = Screen.width;
                                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                                    break;
                                }
                            case "关闭":
                                {
                                    Screen.SetResolution(SystemRuntimeInfo.width, SystemRuntimeInfo.height, true);
                                    break;
                                }
                            case "切换":
                                {
                                    if (Screen.fullScreen)
                                    {
                                        Screen.SetResolution(SystemRuntimeInfo.width, SystemRuntimeInfo.height, false);
                                    }
                                    else
                                    {
                                        SystemRuntimeInfo.height = Screen.height;
                                        SystemRuntimeInfo.width = Screen.width;
                                        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                                    }
                                    break;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.CloseApplication:
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
#if UNITY_2018_1_OR_NEWER
                        wantsToQuit = true;
                        Application.wantsToQuit -= WantsToQuit;
                        Application.wantsToQuit += WantsToQuit;
#endif
                        Application.Quit();
#endif
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.CancelQuitApplication:
                    {
#if UNITY_2018_1_OR_NEWER
                        wantsToQuit = false;
#else
                        Application.CancelQuit();
#endif
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetApplicationInfo:
                    {
                        string oper = param[1] as string;
                        switch (oper)
                        {
                            case "绝对网址":
                                {
                                    return new ReturnValue(true, Application.absoluteURL);
                                }
                            case "公司名":
                                {
                                    return new ReturnValue(true, Application.companyName);
                                }
                            case "数据路径":
                                {
                                    return new ReturnValue(true, Application.dataPath);
                                }
#if UNITY_5_3_OR_NEWER
                            case "关卡数目":
                                {
                                    return new ReturnValue(true, SceneManager.sceneCountInBuildSettings);
                                }
                            case "关卡索引":
                                {
                                    return new ReturnValue(true, SceneManager.GetActiveScene().buildIndex);
                                }
                            case "关卡名称":
                                {
                                    return new ReturnValue(true, SceneManager.GetActiveScene().name);
                                }
#else
                            case "关卡数目":
                                {
                                    return new ReturnValue(true, Application.levelCount);
                                }
                            case "关卡索引":
                                {
                                    return new ReturnValue(true, Application.loadedLevel);
                                }
                            case "关卡名称":
                                {
                                    return new ReturnValue(true, Application.loadedLevelName);
                                }
#endif
                            case "持续数据路径":
                                {
                                    return new ReturnValue(true, Application.persistentDataPath);
                                }
                            case "运行平台":
                                {
                                    return new ReturnValue(true, Application.platform.ToString());
                                }
                            case "产品名称":
                                {
                                    return new ReturnValue(true, Application.productName);
                                }
                            case "流资源路径":
                                {
                                    return new ReturnValue(true, Application.streamingAssetsPath);
                                }
                            case "系统语言":
                                {
                                    return new ReturnValue(true, Application.systemLanguage);
                                }
                            case "临时缓存路径":
                                {
                                    return new ReturnValue(true, Application.temporaryCachePath);
                                }
                            case "unity版本":
                                {
                                    return new ReturnValue(true, Application.unityVersion);
                                }
                            case "版本":
                                {
                                    return new ReturnValue(true, Application.version);
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetApplicationInfo:
                    {
                        switch (param[1] as string)
                        {
                            case "目标帧速率":
                                {
                                    if (int.TryParse(param[2] as string, out var value))
                                    {
                                        Application.targetFrameRate = value;
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                }
                            case "后台运行":
                                {
                                    switch (param[2] as string)
                                    {
                                        case "1":
                                        case "True":
                                        case "#True":
                                        case "是":
                                            {
                                                Application.runInBackground = true;
                                                return ReturnValue.Yes;
                                            }
                                        case "Switch":
                                        case "切换":
                                            {
                                                Application.runInBackground = !Application.runInBackground;
                                                return ReturnValue.Yes;
                                            }
                                        case "":
                                            {
                                                break;
                                            }
                                        default:
                                            {
                                                Application.runInBackground = false;
                                                return ReturnValue.Yes;
                                            }
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.ApplicationExternalCall:
                    {
#if UNITY_WEBGL
                        string func = param[1] as string;
                        if (string.IsNullOrEmpty(func)) break;

                        try
                        {
#if UNITY_5_6_OR_NEWER

#else
                            Application.ExternalCall(func, param[2] as string);
#endif
                            return ReturnValue.Yes;
                        }
                        catch { }
#endif
                        break;
                    }
                case EGenericStandardScriptID.ApplicationExternalEval:
                    {
#if UNITY_WEBGL
                        string script = param[1] as string;
                        if (string.IsNullOrEmpty(script)) break;

                        try
                        {
#if UNITY_5_6_OR_NEWER
#else
                            Application.ExternalEval(script);
#endif
                            return ReturnValue.Yes;
                        }
                        catch { }
#endif
                        break;
                    }
                case EGenericStandardScriptID.RecoverWindow:
                    {
                        Screen.SetResolution(SystemRuntimeInfo.initWidth, SystemRuntimeInfo.initHeight, SystemRuntimeInfo.initFullScreen);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetWindowSize:
                    {
                        Screen.SetResolution((int)param[1], (int)param[2], false);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetWindowSize:
                    {
                        string oper = param[1] as string;
                        switch (oper)
                        {
                            case "窗口宽度":
                                {
                                    return new ReturnValue(true, Screen.width);
                                }
                            case "窗口高度":
                                {
                                    return new ReturnValue(true, Screen.height);
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.OpenURL:
                    {
                        Application.OpenURL(param[1] as string);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.StartExe:
                    {
                        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(param[1] as string, param[2] as string);
                        System.Diagnostics.Process.Start(psi);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.StartFile:
                    {
                        System.Diagnostics.Process.Start(param[1] as string);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ScreenshotExportToImage:
                    {
                        var namePath = param[1] as string;
                        switch (param[3] as string)
                        {
                            case "文件名称与系统时间":
                                {
                                    if (TryGetFilePathInfo(namePath, out string dir, out string fileNameWioutExt, out string extWithDot))
                                    {
                                        var time = DateTime.Now.ToString(param[4] as string);
                                        namePath = dir + fileNameWioutExt + time + extWithDot;
                                    }
                                    break;
                                }
                            case "系统时间":
                                {
                                    if (TryGetFilePathInfo(namePath, out string dir, out string fileNameWioutExt, out string extWithDot))
                                    {
                                        var time = DateTime.Now.ToString(param[4] as string);
                                        namePath = dir + time + extWithDot;
                                    }
                                    break;
                                }
                            case "文件名称与递增编号":
                                {
                                    if (TryGetFilePathInfo(namePath, out string dir, out string fileNameWioutExt, out string extWithDot))
                                    {
                                        var index = (screenCaptureIndex++).ToString();
                                        namePath = dir + fileNameWioutExt + index + extWithDot;
                                    }
                                    break;
                                }
                            case "递增编号":
                                {
                                    if (TryGetFilePathInfo(namePath, out string dir, out string fileNameWioutExt, out string extWithDot))
                                    {
                                        var index = (screenCaptureIndex++).ToString();
                                        namePath = dir + index + extWithDot;
                                    }
                                    break;
                                }
                        }
                        ScreenCapture.CaptureScreenshot(namePath, (int)param[2]);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.IfCurrentRuntimePlatform:
                case EGenericStandardScriptID.ElseIfCurrentRuntimePlatform:
                    {
                        string oper = param[1] as string;
                        RuntimePlatform rp = (RuntimePlatform)param[2];
                        bool ret = false;
                        switch (oper)
                        {
                            case "是":
                                {
                                    ret = (Application.platform == rp);
                                    break;
                                }
                            case "不是":
                                {
                                    ret = (Application.platform != rp);
                                    break;
                                }
                        }
                        return new ReturnValue(ret);
                    }
                case EGenericStandardScriptID.RequestUserAuthorization:
                    {
                        string deviceType = param[1] as string;
                        UserAuthorization ua = UserAuthorization.WebCam;
                        switch (deviceType)
                        {
                            case "相机":
                                {
                                    ua = UserAuthorization.WebCam;
                                    break;
                                }
                            case "麦克风":
                                {
                                    ua = UserAuthorization.Microphone;
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Create(StartCoroutine(RequestUserAuthorization(ua, param[2] as string, param.localVarCollectionHost)) != null);
                    }
                case EGenericStandardScriptID.CheckUserAuthorization:
                    {
                        string deviceType = param[1] as string;
                        UserAuthorization ua = UserAuthorization.WebCam;
                        switch (deviceType)
                        {
                            case "相机":
                                {
                                    ua = UserAuthorization.WebCam;
                                    break;
                                }
                            case "麦克风":
                                {
                                    ua = UserAuthorization.Microphone;
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Create(Application.HasUserAuthorization(ua));
                    }
                case EGenericStandardScriptID.GetCommandLineArgs:
                    {
                        if (!ScriptManager.instance.varCollection.TryGetOrAddVar(param[1] as string, EVarType.Array, out var hierarchyVar)) break;
                        hierarchyVar.TryClearChildren();
                        hierarchyVar.TryAddChildren(Environment.GetCommandLineArgs());
                        return ReturnValue.True(Environment.CommandLine);
                    }
                #endregion

                #region 系统

                case EGenericStandardScriptID.GetEnvironmentInfo:
                    {
                        switch (param[1] as string)
                        {
                            case "退出码":
                                {
                                    return ReturnValue.True(Environment.ExitCode.ToString());
                                }
                            case "是64为进程":
                                {
                                    return ReturnValue.Create(Environment.Is64BitProcess);
                                }
                            case "当前管理线程ID":
                                {
                                    return ReturnValue.True(Environment.CurrentManagedThreadId.ToString());
                                }
                            case "当前目录":
                                {
                                    return ReturnValue.True(Environment.CurrentDirectory);
                                }
                            case "命令行":
                                {
                                    return ReturnValue.True(Environment.CommandLine);
                                }
                            case "机器名":
                                {
                                    return ReturnValue.True(Environment.MachineName);
                                }
                            case "新行":
                                {
                                    return ReturnValue.True(Environment.NewLine);
                                }
                            case "OS版本":
                                {
                                    return ReturnValue.True(Environment.OSVersion.ToString());
                                }
                            case "处理器数":
                                {
                                    return ReturnValue.True(Environment.ProcessorCount.ToString());
                                }
                            case "堆栈跟踪":
                                {
                                    return ReturnValue.True(Environment.StackTrace);
                                }
                            case "系统目录":
                                {
                                    return ReturnValue.True(Environment.SystemDirectory);
                                }
                            case "系统页尺寸":
                                {
                                    return ReturnValue.True(Environment.SystemPageSize.ToString());
                                }
                            case "滴答数":
                                {
                                    return ReturnValue.True(Environment.TickCount.ToString());
                                }
                            case "用户域名":
                                {
                                    return ReturnValue.True(Environment.UserDomainName);
                                }
                            case "用户交互":
                                {
                                    return ReturnValue.Create(Environment.UserInteractive);
                                }
                            case "用户名":
                                {
                                    return ReturnValue.True(Environment.UserName);
                                }
                            case "版本":
                                {
                                    return ReturnValue.True(Environment.Version.ToString());
                                }
                            case "工作集":
                                {
                                    return ReturnValue.True(Environment.WorkingSet.ToString());
                                }
                            case "关机已启动":
                                {
                                    return ReturnValue.Create(Environment.HasShutdownStarted);
                                }
                            case "是64位操作系统":
                                {
                                    return ReturnValue.Create(Environment.Is64BitOperatingSystem);
                                }
                        }
                        break;
                    }

                case EGenericStandardScriptID.GetDNSInfo:
                    {
                        switch (param[1] as string)
                        {
                            case "主机名":
                                {
                                    return ReturnValue.True(Dns.GetHostName());
                                }
                            case "主机IP地址":
                                {
                                    var hostName = Dns.GetHostName();
                                    return ReturnValue.True(Dns.GetHostAddresses(hostName).ToStringDirect());
                                }
                            case "主机IPV4地址":
                                {
                                    var hostName = Dns.GetHostName();
                                    return ReturnValue.True(Dns.GetHostAddresses(hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToStringDirect());
                                }
                            case "主机IPV6地址":
                                {
                                    var hostName = Dns.GetHostName();
                                    return ReturnValue.True(Dns.GetHostAddresses(hostName).Where(ip => ip.AddressFamily == AddressFamily.InterNetworkV6).ToStringDirect());
                                }
                        }
                        break;
                    }

                #endregion

                #region 时间
                case EGenericStandardScriptID.GetCurrentSystemTime:
                    {
                        string fmt = param[2] as string;
                        if (string.IsNullOrEmpty(fmt)) fmt = param[1] as string;
                        return new ReturnValue(true, DateTime.Now.ToString(fmt));
                    }
                case EGenericStandardScriptID.GetStartTime:
                    {
                        string fmt = param[2] as string;
                        if (string.IsNullOrEmpty(fmt)) fmt = param[1] as string;
                        return new ReturnValue(true, SystemRuntimeInfo.startTime.ToString(fmt));
                    }
                case EGenericStandardScriptID.GetUnityTimeInfo:
                    {
                        #region GetUnityTimeInfo
                        switch (param[1] as string)
                        {
                            case nameof(Time.captureFramerate):
                                {
                                    return new ReturnValue(true, Time.captureFramerate.ToString());
                                }
                            case nameof(Time.deltaTime):
                                {
                                    return new ReturnValue(true, Time.deltaTime.ToString());
                                }
                            case nameof(Time.fixedDeltaTime):
                                {
                                    return new ReturnValue(true, Time.fixedDeltaTime.ToString());
                                }
                            case nameof(Time.fixedTime):
                                {
                                    return new ReturnValue(true, Time.fixedTime.ToString());
                                }
                            case nameof(Time.frameCount):
                                {
                                    return new ReturnValue(true, Time.frameCount.ToString());
                                }
                            case nameof(Time.maximumDeltaTime):
                                {
                                    return new ReturnValue(true, Time.maximumDeltaTime.ToString());
                                }
                            case nameof(Time.realtimeSinceStartup):
                                {
                                    return new ReturnValue(true, Time.realtimeSinceStartup.ToString());
                                }
                            case nameof(Time.renderedFrameCount):
                                {
                                    return new ReturnValue(true, Time.renderedFrameCount.ToString());
                                }
                            case nameof(Time.smoothDeltaTime):
                                {
                                    return new ReturnValue(true, Time.smoothDeltaTime.ToString());
                                }
                            case nameof(Time.time):
                                {
                                    return new ReturnValue(true, Time.time.ToString());
                                }
                            case nameof(Time.timeScale):
                                {
                                    return new ReturnValue(true, Time.timeScale.ToString());
                                }
                            case nameof(Time.timeSinceLevelLoad):
                                {
                                    return new ReturnValue(true, Time.timeSinceLevelLoad.ToString());
                                }
                            case nameof(Time.unscaledDeltaTime):
                                {
                                    return new ReturnValue(true, Time.unscaledDeltaTime.ToString());
                                }
                            case nameof(Time.unscaledTime):
                                {
                                    return new ReturnValue(true, Time.unscaledTime.ToString());
                                }
                            case "即时FPS":
                                {
                                    return ReturnValue.True((1f / Time.deltaTime).ToString());
                                }
                            case "平均FPS(全局)":
                                {
                                    return ReturnValue.True((Time.frameCount / Time.realtimeSinceStartup).ToString());
                                }
                            case "平均FPS(当前)":
                                {
                                    return ReturnValue.True((Time.frameCount / Time.timeSinceLevelLoad).ToString());
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.ConvertSecondToHMS:
                    {
                        int seconds = (int)param[1];

                        if (seconds >= 0)
                        {
                            switch (param[2] as string)
                            {
                                case "时:分:秒":
                                    {
                                        if (seconds < (3600 * 24))
                                        {
                                            return ReturnValue.True(CommonFun.ConvertIntToHMS(seconds, (EBool2)param[3] == EBool2.Yes));
                                        }
                                        break;
                                    }
                                case "分:秒":
                                    {
                                        if (seconds < 3600)
                                        {
                                            return ReturnValue.True(CommonFun.ConvertIntToMS(seconds, (EBool2)param[3] == EBool2.Yes));
                                        }
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.ConvertSecondsToTimeFormat:
                    {
                        string fmt = param[3] as string;
                        if (string.IsNullOrEmpty(fmt)) fmt = param[2] as string;
                        return ReturnValue.True(TimeSpan.FromSeconds((double)param[1]).ToString(fmt));
                    }
                case EGenericStandardScriptID.ConvertMillisecondsToTimeFormat:
                    {
                        string fmt = param[3] as string;
                        if (string.IsNullOrEmpty(fmt)) fmt = param[2] as string;
                        return ReturnValue.True(TimeSpan.FromMilliseconds((double)param[1]).ToString(fmt));
                    }

                case EGenericStandardScriptID.CreateTimer:
                    {
                        #region CreateTimer
                        string funName = param[1] as string;
                        if (ScriptOption.IsDefaultCustomFunction(funName)) break;
                        string p = param[2] as string;
                        int delayTime = (int)param[3];
                        int count = (int)param[4];
                        if (count == 0) break;
                        if (count < 0) count = -1;
                        return new ReturnValue(TimerManager.CreateTimer(param[0] as string, this, funName, p, delayTime, param.localVarCollectionHost, count));
                        #endregion
                    }
                case EGenericStandardScriptID.ControlTimer:
                    {
                        #region ControlTimer
                        bool ret = false;
                        switch (param[1] as string)
                        {
                            case "删除":
                                {
                                    ret = TimerManager.Kill(param[0] as string);
                                    break;
                                }
                            case "暂停":
                                {
                                    ret = TimerManager.Pause(param[0] as string);
                                    break;
                                }
                            case "继续":
                                {
                                    ret = TimerManager.Resume(param[0] as string);
                                    break;
                                }
                            case "全部删除":
                                {
                                    TimerManager.Kill();
                                    ret = true;
                                    break;
                                }
                            case "全部暂停":
                                {
                                    TimerManager.Pause();
                                    ret = true;
                                    break;
                                }
                            case "全部继续":
                                {
                                    TimerManager.Resume();
                                    ret = true;
                                    break;
                                }
                        }
                        return ReturnValue.Create(ret);
                        #endregion
                    }
                #endregion

                #region 选择集扩展

                case EGenericStandardScriptID.SetSelectionLimitedMaxCount:
                    {
                        LimitedSelection.maxCount = (int)param[1];
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetSelectionCount:
                    {
                        return ReturnValue.True(Selection.countValid);
                    }
                case EGenericStandardScriptID.SelectionOperation:
                    {
                        var goPath = string.Empty;
                        switch (param[1] as string)
                        {
                            case "克隆":
                                {
                                    Selection.selections.Foreach(go =>
                                    {
                                        if (go)
                                        {
                                            var newGO = go.XCloneObject();
                                            newGO.XSetParent(go.transform.parent);
                                            newGO.XSetUniqueName(go.name);

                                            if (string.IsNullOrEmpty(goPath)) goPath = CommonFun.GameObjectToString(newGO);
                                        }
                                    });
                                    break;
                                }
                            case "删除":
                                {
                                    Selection.selections.Foreach(go =>
                                    {
                                        if (go)
                                        {
                                            if (string.IsNullOrEmpty(goPath)) goPath = CommonFun.GameObjectToString(go);
                                            DestroyImmediate(go);
                                        }
                                    });
                                    break;
                                }
                        }

                        return string.IsNullOrEmpty(goPath) ? ReturnValue.No : new ReturnValue(true, goPath);
                    }

                #endregion

                #region 其他操作


                case EGenericStandardScriptID.DecodeWebCamTextureQRCode:
                    {
#if XDREAMER_ZXING
                        string text;
                        if (QRCode.Decode(WebCamManager.GetWebCamTexture(param[1] as string), out text))
                        {
                            ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, text);
                            return ReturnValue.Yes;
                        }
#endif
                        break;
                    }
                case EGenericStandardScriptID.DecodeTexture2DQRCode:
                    {
#if XDREAMER_ZXING
                        string text;
                        if (QRCode.Decode(param[1] as Texture2D, out text))
                        {
                            ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, text);
                            return ReturnValue.Yes;
                        }
#endif
                        break;
                    }
                case EGenericStandardScriptID.SetCursorTexture:
                    {
                        Cursor.SetCursor(param[1] as Texture2D, (Vector2)param[2], CursorMode.Auto);
                        break;
                    }
                case EGenericStandardScriptID.GetWindZoneProperty:
                    {
                        var windZone = param[1] as WindZone;
                        if (windZone)
                        {
                            switch (param[2] as string)
                            {
                                case "模式":
                                    {
                                        return ReturnValue.True(windZone.mode.ToString());
                                    }
                                case "半径":
                                    {
                                        return ReturnValue.True(windZone.radius);
                                    }
                                case "主风力":
                                    {
                                        return ReturnValue.True(windZone.windMain);
                                    }
                                case "湍流":
                                    {
                                        return ReturnValue.True(windZone.windTurbulence);
                                    }
                                case "脉冲强度":
                                    {
                                        return ReturnValue.True(windZone.windPulseMagnitude);
                                    }
                                case "脉冲频率":
                                    {
                                        return ReturnValue.True(windZone.windPulseFrequency);
                                    }
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetWindZoneProperty:
                    {
                        var windZone = param[1] as WindZone;
                        if (windZone)
                        {
                            switch (param[2] as string)
                            {
                                case "模式":
                                    {
                                        if (Enum.TryParse<WindZoneMode>(param[3] as string, out var mode))
                                        {
                                            windZone.mode = mode;
                                            return ReturnValue.True();
                                        }
                                        break;
                                    }
                                case "半径":
                                    {
                                        windZone.radius = (float)param[4];
                                        return ReturnValue.True();
                                    }
                                case "主风力":
                                    {
                                        windZone.windMain = (float)param[5];
                                        return ReturnValue.True();
                                    }
                                case "湍流":
                                    {
                                        windZone.windTurbulence = (float)param[6];
                                        return ReturnValue.True();
                                    }
                                case "脉冲强度":
                                    {
                                        windZone.windPulseMagnitude = (float)param[7];
                                        return ReturnValue.True();
                                    }
                                case "脉冲频率":
                                    {
                                        windZone.windPulseFrequency = (float)param[8];
                                        return ReturnValue.True();
                                    }
                            }
                        }
                        break;
                    }

                #endregion

                #region 粒子系统

                case EGenericStandardScriptID.ControlParticleSystem:
                    {
                        var particleSystem = param[1] as ParticleSystem;
                        if (!particleSystem) break;

                        switch (param[2] as string)
                        {
                            case "播放":
                                {
                                    particleSystem.Play();
                                    return ReturnValue.Yes;
                                }
                            case "暂停":
                                {
                                    particleSystem.Pause();
                                    return ReturnValue.Yes;
                                }
                            case "停止":
                                {
                                    particleSystem.Stop();
                                    return ReturnValue.Yes;
                                }
                            case "清空":
                                {
                                    particleSystem.Clear();
                                    return ReturnValue.Yes;
                                }
                            case "重置":
                                {
                                    particleSystem.Stop();
                                    particleSystem.Clear();
                                    particleSystem.Play();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }

                case EGenericStandardScriptID.ControlParticleSystemBatch:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        var particleSystems = go.GetComponentsInChildren<ParticleSystem>();
                        if (particleSystems.Length == 0) break;

                        switch (param[2] as string)
                        {
                            case "播放":
                                {
                                    particleSystems.Foreach(p => p.Play());
                                    return ReturnValue.Yes;
                                }
                            case "暂停":
                                {
                                    particleSystems.Foreach(p => p.Pause());
                                    return ReturnValue.Yes;
                                }
                            case "停止":
                                {
                                    particleSystems.Foreach(p => p.Stop());
                                    return ReturnValue.Yes;
                                }
                            case "清空":
                                {
                                    particleSystems.Foreach(p => p.Clear());
                                    return ReturnValue.Yes;
                                }
                            case "重置":
                                {
                                    particleSystems.Foreach(p =>
                                    {
                                        p.Stop();
                                        p.Clear();
                                        p.Play();
                                    });
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetParticleSystemInfo:
                    {
                        var particleSystem = param[1] as ParticleSystem;
                        if (!particleSystem) break;

                        switch (param[2] as string)
                        {
                            case "是否循环":
                                {
                                    return ReturnValue.True(particleSystem.main.loop);
                                }
                            case "是否发射":
                                {
                                    return ReturnValue.True(particleSystem.isEmitting);
                                }
                            case "是否播放":
                                {
                                    return ReturnValue.True(particleSystem.isPlaying);
                                }
                            case "是否暂停":
                                {
                                    return ReturnValue.True(particleSystem.isPaused);
                                }
                            case "是否停止":
                                {
                                    return ReturnValue.True(particleSystem.isStopped);
                                }
                        }

                        break;
                    }

                #endregion

                #region UGUI

                case EGenericStandardScriptID.UGUIGetGameObjectPosition:
                    {
                        RectTransform rt = param[1] as RectTransform;
                        if (rt)
                        {
                            switch (param[2] as string)
                            {
                                case "世界坐标":
                                    {
                                        return ReturnValue.True(CommonFun.Vector3ToString(rt.position));
                                    }
                                case "本地坐标":
                                default:
                                    {
                                        return ReturnValue.True(CommonFun.Vector3ToString(rt.localPosition));
                                    }
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetGameObjectPosition:
                    {
                        RectTransform rt = param[1] as RectTransform;
                        if (rt)
                        {
                            switch (param[3] as string)
                            {
                                case "世界坐标":
                                    {
                                        rt.position = (Vector3)param[2];
                                        return ReturnValue.Yes;
                                    }
                                case "本地坐标":
                                default:
                                    {
                                        rt.localPosition = (Vector3)param[2];
                                        return ReturnValue.Yes;
                                    }
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetScrollbarValue:
                    {
                        Scrollbar scrollbar = param[1] as Scrollbar;
                        if (scrollbar != null)
                        {
                            scrollbar.value = (float)param[2];
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIGetScrollbarValue:
                    {
                        Scrollbar scrollbar = param[1] as Scrollbar;
                        if (scrollbar != null)
                        {
                            return new ReturnValue(true, scrollbar.value);
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetSliderValue:
                    {
                        Slider slider = param[1] as Slider;
                        if (slider != null)
                        {
                            slider.value = (float)param[2];
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIGetSliderValue:
                    {
                        Slider slider = param[1] as Slider;
                        if (slider != null)
                        {
                            return new ReturnValue(true, slider.value);
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetTextValue:
                    {
                        var text = param[1] as UnityEngine.UI.Text;
                        if (text != null)
                        {
                            text.text = param[2] as string;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIGetTextValue:
                    {
                        var text = param[1] as UnityEngine.UI.Text;
                        if (text != null)
                        {
                            return new ReturnValue(true, text.text);
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetInputFieldText:
                    {
                        var inputField = param[1] as InputField;
                        if (inputField)
                        {
                            inputField.text = param[2] as string;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIGetInputFieldText:
                    {
                        var inputField = param[1] as InputField;
                        if (inputField)
                        {
                            return ReturnValue.True(inputField.text);
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetToggleOn:
                    {
                        var toggle = param[1] as Toggle;
                        if (toggle)
                        {
                            toggle.isOn = CommonFun.BoolChange(toggle.isOn, (EBool)param[2]);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIGetToggleOn:
                    {
                        var toggle = param[1] as Toggle;
                        if (toggle)
                        {
                            return ReturnValue.Create(toggle.isOn);
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetControlInteractable:
                    {
                        var selectable = param[1] as Selectable;
                        if (selectable)
                        {
                            selectable.interactable = CommonFun.BoolChange(selectable.interactable, (EBool)param[2]);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIGetControlInteractable:
                    {
                        var selectable = param[1] as Selectable;
                        if (selectable)
                        {
                            return ReturnValue.Create(selectable.interactable);
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUIClearDropdownOptions:
                    {
                        var dropdown = param[1] as Dropdown;
                        if (dropdown)
                        {
                            dropdown.ClearOptions();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetDropdownValue:
                    {
                        Dropdown dropdown = param[1] as Dropdown;
                        if (dropdown)
                        {
                            int selectedIndex = (int)param[2] - 1;
                            if (selectedIndex >= 0 && selectedIndex < dropdown.options.Count)
                            {
                                dropdown.value = selectedIndex;
                                return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.UGUISetInteractableIncludeChildren:
                    {
                        var rectTransform = param[1] as RectTransform;
                        if (!rectTransform) break;

                        var ebool = (EBool)param[3];
                        Type type = null;
                        switch (param[2] as string)
                        {
                            case "Selectable":
                                type = typeof(Selectable);
                                break;
                            case "Button":
                                type = typeof(Button);
                                break;
                            case "Toggle":
                                type = typeof(Toggle);
                                break;
                            case "Scrollbar":
                                type = typeof(Scrollbar);
                                break;
                            case "Slider":
                                type = typeof(Slider);
                                break;
                            default: break;
                        }
                        if (type == null) break;

                        foreach (var c in rectTransform.GetComponentsInChildren(type))
                        {
                            var i = c as Selectable;
                            if (i) i.interactable = CommonFun.BoolChange(i.interactable, ebool);
                        }

                        break;
                    }
                case EGenericStandardScriptID.UGUIIfIsPointerOverGameObject:
                case EGenericStandardScriptID.UGUIElseIfIsPointerOverGameObject:
                case EGenericStandardScriptID.UGUIGetIsPointerOverGameObject:
                    {
                        return ReturnValue.Create(EventSystem.current && EventSystem.current.IsPointerOverGameObject());
                    }
                case EGenericStandardScriptID.UGUISetClickButton:
                    {
                        var button = param[1] as Button;
                        if (button)
                        {
                            button.onClick.Invoke();
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                #endregion

                #region Unity游戏对象操作
                case EGenericStandardScriptID.GameObjectFind:
                    {
                        GameObject go = param[1] as GameObject;
                        if (go) return new ReturnValue(true, CommonFun.GameObjectToString(go));
                        break;
                    }
                case EGenericStandardScriptID.GameObjectRecursionCallUserDefineFun:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        string funName = param[2] as string;
                        //先对根节点进行操作
                        ScriptManager.instance.ExecuteFunction(funName, CommonFun.GameObjectToString(go), param.localVarCollectionHost);
                        //再对所有具有该组件的子级节点进行操作
                        foreach (Component c in go.GetComponentsInChildren(typeof(Transform), ((EBool)param[3] == EBool.Yes)))
                        {
                            ScriptManager.instance.ExecuteFunction(funName, CommonFun.GameObjectToString(c.gameObject), param.localVarCollectionHost);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectChildrenCallUserDefineFun:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        string funName = param[2] as string;
                        foreach (Transform child in go.transform)
                        {
                            //Debug.Log("------->" + child.gameObject.name);
                            ScriptManager.instance.ExecuteFunction(funName, CommonFun.GameObjectToString(child.gameObject), param.localVarCollectionHost);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetGameObjectChildrenCount:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        return ReturnValue.True(go.transform.childCount);
                    }
                case EGenericStandardScriptID.GetGameObjectChildrenWithIndex:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        Transform tran = go.transform.GetChild((int)param[2] - 1);
                        if (!tran) break;
                        return ReturnValue.True(CommonFun.GameObjectToString(tran.gameObject));
                    }
                #endregion

                #region Unity组件操作
                case EGenericStandardScriptID.ComponentAdd:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        Type type = param[2] as Type;
                        if (type == null) break;
                        if (go.GetComponent(type) != null) break;
                        Component c = go.AddComponent(type);
                        if (c == null) break;
                        return new ReturnValue(true, CommonFun.GameObjectComponentToString(c));
                    }
                case EGenericStandardScriptID.ComponentRemove:
                    {
                        Component c = param[1] as Component;
                        if (c == null) break;
                        if (Application.isEditor && !Application.isPlaying)
                        {
                            DestroyImmediate(c);
                        }
                        else
                        {
                            Destroy(c);
                        }
                        ScriptManager.DelayCallUDF(param[2] as string, param[3] as string);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ComponentFind:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        Type type = param[2] as Type;
                        if (type == null) break;
                        Component c = go.GetComponent(type);
                        if (c == null) break;
                        return new ReturnValue(true, CommonFun.GameObjectComponentToString(c));
                    }
                case EGenericStandardScriptID.ComponentActive:
                    {
                        var component = param[1] as Component;
                        if (!component) break;
                        component.XSetEnable((EBool)param[2]);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ComponentRecursionActive:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        Type t = param[2] as Type;
                        if (t == null) break;
                        //先对根节点进行操作
                        Component component = go.GetComponent(t);
                        EBool ebool = (EBool)param[3];
                        component.XSetEnable(ebool);
                        //再对所有具有该组件的子级节点进行操作
                        foreach (Component c in go.GetComponentsInChildren(t, ((EBool)param[4] == EBool.Yes)))
                        {
                            c.XSetEnable(ebool);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ComponentRecursionCallUserDefineFun:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        Type t = param[2] as Type;
                        if (t == null) break;
                        string funName = param[3] as string;
                        //先对根节点进行操作
                        Component component = go.GetComponent(t);
                        if (component) ScriptManager.instance.ExecuteFunction(funName, CommonFun.GameObjectComponentToString(component), param.localVarCollectionHost);
                        //再对所有具有该组件的子级节点进行操作
                        foreach (Component c in go.GetComponentsInChildren(t, ((EBool)param[4] == EBool.Yes)))
                        {
                            ScriptManager.instance.ExecuteFunction(funName, CommonFun.GameObjectComponentToString(c), param.localVarCollectionHost);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ComponentMemberGetValue:
                    {
                        Component c = param[1] as Component;
                        if (c == null) break;
                        string memberName = param[2] as string;
                        FieldInfo fi = c.GetType().GetField(memberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance);
                        if (fi != null)
                        {
                            return new ReturnValue(true, CommonFun.ObjectToString(fi.GetValue(c)));
                        }
                        PropertyInfo pi = c.GetType().GetProperty(memberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
                        if (pi != null)
                        {
                            return new ReturnValue(true, CommonFun.ObjectToString(pi.GetValue(c, null)));
                        }
                        break;
                    }
                case EGenericStandardScriptID.ComponentMemberSetValue:
                    {
                        Component c = param[1] as Component;
                        if (c == null) break;

                        string memberName = param[2] as string;
                        FieldInfo fi = c.GetType().GetField(memberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.SetField | BindingFlags.Instance);
                        PropertyInfo pi = null;
                        if (fi == null)
                        {
                            pi = c.GetType().GetProperty(memberName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
                            if (pi == null) break;
                        }

                        object newValue = null;
                        string newValueString = param[3] as string;
                        switch (param[4] as string)
                        {
                            case "string":
                                {
                                    newValue = newValueString;
                                    break;
                                }
                            case "bool":
                                {
                                    newValue = Converter.instance.ConvertTo<bool>(newValueString);
                                    break;
                                }
                            case "int":
                                {
                                    newValue = CommonFun.StringToInt(newValueString);
                                    break;
                                }
                            case "long":
                                {
                                    newValue = CommonFun.StringToLong(newValueString);
                                    break;
                                }
                            case "float":
                                {
                                    newValue = CommonFun.StringToFloat(newValueString);
                                    break;
                                }
                            case "double":
                                {
                                    newValue = CommonFun.StringToDouble(newValueString);
                                    break;
                                }
                            case nameof(Vector2Int):
                                {
                                    newValue = CommonFun.StringToVector2Int(newValueString);
                                    break;
                                }
                            case nameof(Vector3Int):
                                {
                                    newValue = CommonFun.StringToVector3Int(newValueString);
                                    break;
                                }
                            case nameof(Vector2):
                                {
                                    newValue = CommonFun.StringToVector2(newValueString);
                                    break;
                                }
                            case nameof(Vector3):
                                {
                                    newValue = CommonFun.StringToVector3(newValueString);
                                    break;
                                }
                            case nameof(Vector4):
                                {
                                    newValue = CommonFun.StringToVector4(newValueString);
                                    break;
                                }
                            case nameof(Rect):
                                {
                                    newValue = CommonFun.StringToRect(newValueString);
                                    break;
                                }
                            case nameof(Bounds):
                                {
                                    newValue = CommonFun.StringToBounds(newValueString);
                                    break;
                                }
                            case "Color":
                                {
                                    newValue = CommonFun.StringToColor(newValueString);
                                    break;
                                }
                            case "GameObject":
                                {
                                    newValue = CommonFun.StringToGameObject(newValueString);
                                    break;
                                }
                            case "GameObjectComponent":
                                {
                                    newValue = CommonFun.StringToGameObjectComponent(newValueString);
                                    break;
                                }
                            default:
                                {
                                    if (!Converter.instance.TryConvertTo(newValueString, fi != null ? fi.FieldType : pi.PropertyType, out newValue))
                                    {
                                        return ReturnValue.No;
                                    }
                                    break;
                                }
                        }
                        if (fi != null)
                        {
                            fi.SetValue(c, newValue);
                            return ReturnValue.Yes;
                        }
                        else if (pi != null)
                        {
                            pi.SetValue(c, newValue, null);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetComponentInfo:
                    {
                        var component = param[1] as Component;
                        if (!component) break;
                        switch (param[2] as string)
                        {
                            case "组件路径名称":
                                {
                                    return ReturnValue.True(CommonFun.GameObjectComponentToString(component));
                                }
                            case "组件类型":
                                {
                                    return ReturnValue.True(ComponentManager.TypeToKey(component.GetType()));
                                }
                            case "游戏对象路径名称":
                                {
                                    return ReturnValue.True(CommonFun.GameObjectToString(component.gameObject));
                                }
                            case "游戏对象名称":
                                {
                                    return ReturnValue.True(component.name);
                                }
                            case "父级游戏对象路径名称":
                                {
                                    var parent = component.transform.parent;
                                    if (!parent) break;
                                    return ReturnValue.True(CommonFun.GameObjectToString(parent.gameObject));
                                }
                            case "父级游戏对象名称":
                                {
                                    var parent = component.transform.parent;
                                    if (!parent) break;
                                    return ReturnValue.True(parent.name);
                                }
                            case "可用性":
                                {
                                    return ReturnValue.Create(CommonFun.GetComponentEnabled(component));
                                }
                        }
                        break;
                    }
                #endregion

                #region Unity资源对象处理脚本

                case EGenericStandardScriptID.LoadUnityAssetObject:
                    {
                        #region LoadUnityAssetObject
                        EUnityAssetObjectType type = (EUnityAssetObjectType)param[0];
                        UnityEngine.Object obj = UnityAssetObjectManager.instance.GetUnityAssetObject(type.ToString(), param[1] as string);

                        string varName = param[5] as string;
                        string funName = param[6] as string;
                        string tag = param[7] as string;
                        if (obj)
                        {
                            ScriptManager.TrySetGlobalVariableValue(varName, CommonFun.UnityAssetObjectToString(obj));
                            ScriptManager.DelayCallUDF(funName, tag, param.localVarCollectionHost);
                            return ReturnValue.Yes;
                        }
                        string path = param[2] as string;
                        if (string.IsNullOrEmpty(path)) break;
                        //使用的参数信息
                        ParamList pl = new ParamList("var", varName, "funName", funName, "tag", tag, "varCollectionHost", param.localVarCollectionHost, "type", type)
                        {
                            ["name"] = param[4] as string
                        };
                        //网络路径
                        if (PathHandler.IsHttpPath(path))
                        {
                            //Debug.Log("IsHttpPath: " + path);
                            return ReturnValue.Create(FileHandler.LoadHttpFile(path, UnityAssetObjectManager.UnityAssetObjectTypeToDataType(type), this.LoadUnityAssetObject, pl));
                        }
                        //本地路径
                        switch (param[8] as string)
                        {
                            case "绝对路径加载":
                                {
                                    if (!System.IO.File.Exists(path)) return ReturnValue.No;
                                    break;
                                }
                            case "智能加载":
                            default:
                                {
                                    string existPath;
                                    if (!PathHandler.LocalFileSmartSearch(path, out existPath))
                                    {
                                        return ReturnValue.No;
                                    }
                                    path = existPath;
                                    break;
                                }
                        }
                        //Debug.Log("File.Exists :" + path);
                        return ReturnValue.Create(FileHandler.LoadLocalFile(path, UnityAssetObjectManager.UnityAssetObjectTypeToDataType(type), this.LoadUnityAssetObject, pl));
                        #endregion
                    }
                case EGenericStandardScriptID.DestroyUnityAssetObject:
                    {
                        #region DestroyUnityAssetObject
                        UnityEngine.Object obj = param[1] as UnityEngine.Object;
                        if (!obj) break;
                        switch (param[2] as string)
                        {
                            case "智能销毁":
                                {
                                    UnityEngine.Object.Destroy(obj);
                                    return ReturnValue.Yes;
                                }
                            case "立即销毁":
                                {
                                    UnityEngine.Object.DestroyImmediate(obj);
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.GetTextAssetObjectTextInfo:
                    {
                        TextAsset ta = param[1] as TextAsset;
                        if (ta)
                        {
                            return ReturnValue.Create(ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, ta.text));
                        }
                        break;
                    }
                case EGenericStandardScriptID.ReplaceGameObjectMainTexture:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;
                        Texture texutre = param[2] as Texture;
                        if (!texutre) break;
                        MeshRenderer mr = go.GetComponent<MeshRenderer>();
                        if (!mr) break;
                        Material mat = mr.material;
                        if (!mat) break;
                        mat.mainTexture = texutre;
                        break;
                    }
                case EGenericStandardScriptID.ModifyGameObjectMaterial:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var material = param[2] as Material;
                        if (!material) break;
                        var ren = go.GetComponent<Renderer>();
                        if (!ren) break;

                        var length = ren.materials.Length;

                        var index = (int)param[3];
                        if (index < 0)
                        {
                            var materials = new Material[length];
                            for (int i = 0; i < length; ++i)
                            {
                                materials[i] = material;
                            }
                            ren.materials = materials;

                            return ReturnValue.Yes;
                        }
                        else if (index >= 0 && index < length)
                        {
                            var materials = new Material[length];
                            for (int i = 0; i < length; ++i)
                            {
                                materials[i] = ren.materials[i];
                            }
                            materials[index] = material;
                            ren.materials = materials;

                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.ModifyGameObjectMaterialMainTexture:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var texture = param[2] as Texture;
                        if (!texture) break;
                        var ren = go.GetComponent<Renderer>();
                        if (!ren) break;

                        var length = ren.materials.Length;

                        var index = (int)param[3];
                        if (index < 0)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                ren.materials[i].mainTexture = texture;
                            }

                            return ReturnValue.Yes;
                        }
                        else if (index >= 0 && index < length)
                        {
                            ren.materials[index].mainTexture = texture;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.ModifyGameObjectMaterialColor:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var ren = go.GetComponent<Renderer>();
                        if (!ren) break;

                        var color = (Color)param[2];
                        var length = ren.materials.Length;

                        var index = (int)param[3];
                        if (index < 0)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                ren.materials[i].color = color;
                            }

                            return ReturnValue.Yes;
                        }
                        else if (index >= 0 && index < length)
                        {
                            ren.materials[index].color = color;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.ModifyGameObjectSharedMaterial:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var material = param[2] as Material;
                        if (!material) break;
                        var ren = go.GetComponent<Renderer>();
                        if (!ren) break;

                        var length = ren.sharedMaterials.Length;

                        var index = (int)param[3];
                        if (index < 0)
                        {
                            var materials = new Material[length];
                            for (int i = 0; i < length; ++i)
                            {
                                materials[i] = material;
                            }
                            ren.sharedMaterials = materials;

                            return ReturnValue.Yes;
                        }
                        else if (index >= 0 && index < length)
                        {
                            var materials = new Material[length];
                            for (int i = 0; i < length; ++i)
                            {
                                materials[i] = ren.sharedMaterials[i];
                            }
                            materials[index] = material;
                            ren.sharedMaterials = materials;

                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.ModifyGameObjectSharedMaterialMainTexture:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var texture = param[2] as Texture;
                        if (!texture) break;
                        var ren = go.GetComponent<Renderer>();
                        if (!ren) break;

                        var length = ren.sharedMaterials.Length;

                        var index = (int)param[3];
                        if (index < 0)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                ren.sharedMaterials[i].mainTexture = texture;
                            }

                            return ReturnValue.Yes;
                        }
                        else if (index >= 0 && index < length)
                        {
                            ren.sharedMaterials[index].mainTexture = texture;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.ModifyGameObjectSharedMaterialColor:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        var ren = go.GetComponent<Renderer>();
                        if (!ren) break;

                        var color = (Color)param[2];
                        var length = ren.sharedMaterials.Length;

                        var index = (int)param[3];
                        if (index < 0)
                        {
                            for (int i = 0; i < length; ++i)
                            {
                                ren.sharedMaterials[i].color = color;
                            }

                            return ReturnValue.Yes;
                        }
                        else if (index >= 0 && index < length)
                        {
                            ren.sharedMaterials[index].color = color;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                #endregion

                #region UI处理脚本
                case EGenericStandardScriptID.GUILabel:
                    {
                        GUI.Label((Rect)param[2], param[1] as string);
                        break;
                    }
                case EGenericStandardScriptID.GUIButton:
                    {
                        int fontSize = (int)param[5];
                        var buttonStyle = GUI.skin.button;
                        var fontSizeBK = buttonStyle.fontSize;
                        try
                        {
                            if (fontSize > 0) buttonStyle.fontSize = fontSize;
                            if (GUI.Button((Rect)param[2], param[1] as string, buttonStyle))
                            {
                                return ScriptManager.Instance().ExecuteFunction(param[3] as string, param[4] as string, param.localVarCollectionHost);
                            }
                        }
                        finally
                        {
                            buttonStyle.fontSize = fontSizeBK;
                        }
                        break;
                    }
                case EGenericStandardScriptID.GUIDrawTexture:
                    {
                        Texture texture = param[1] as Texture;
                        if (texture)
                        {
                            ScaleMode sm;
                            switch (param[3] as string)
                            {
                                case "ScaleAndCrop":
                                    {
                                        sm = ScaleMode.ScaleAndCrop;
                                        break;
                                    }
                                case "ScaleToFit":
                                    {
                                        sm = ScaleMode.ScaleToFit;
                                        break;
                                    }
                                case "StretchToFill":
                                default:
                                    {
                                        sm = ScaleMode.StretchToFill;
                                        break;
                                    }
                            }
                            GUI.DrawTexture((Rect)param[2], texture, sm, (param[4] as string) != "否", (float)param[5]);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.GUIDrawTextureWithTextureSize:
                    {
                        Texture texture = param[1] as Texture;
                        if (texture)
                        {
                            ScaleMode sm;
                            switch (param[3] as string)
                            {
                                case "ScaleAndCrop":
                                    {
                                        sm = ScaleMode.ScaleAndCrop;
                                        break;
                                    }
                                case "ScaleToFit":
                                    {
                                        sm = ScaleMode.ScaleToFit;
                                        break;
                                    }
                                case "StretchToFill":
                                default:
                                    {
                                        sm = ScaleMode.StretchToFill;
                                        break;
                                    }
                            }
                            Rect rect = new Rect((Vector2)param[2], new Vector2(texture.width, texture.height));
                            GUI.DrawTexture(rect, texture, sm, (param[4] as string) != "否", (float)param[5]);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.GUIWindow:
                    {
                        string funName = param[3] as string;
                        if (string.IsNullOrEmpty(funName)) break;
                        int windowID = (int)param[1];
                        GUISkin skin = param[7] as GUISkin;
                        GUIManager.CreateOrUpdateWindow(windowID, funName, ((EBool)param[8] == EBool.Yes), param.localVarCollectionHost);
                        Rect rect;
                        if (skin != null && skin.window != null)
                        {
                            rect = GUI.Window(windowID, (Rect)param[2], GUIManager.GUIWindowFunction, new GUIContent(param[4] as string, param[6] as Texture, param[5] as string), skin.window);
                        }
                        else
                        {
                            rect = GUI.Window(windowID, (Rect)param[2], GUIManager.GUIWindowFunction, new GUIContent(param[4] as string, param[6] as Texture, param[5] as string));
                        }
                        return ReturnValue.True(CommonFun.RectToString(rect));
                    }
                case EGenericStandardScriptID.GUITextBuddle:
                    {
                        TextBuddle tb = GUIManager.CreateTextBuddle(param[1] as string, (Vector3)param[2], (ECoordinateType)param[3], (float)param[4], (TextAnchor)param[5]);
                        return ReturnValue.Create(tb != null);
                    }
                #endregion

                #region Unity游戏对象
                case EGenericStandardScriptID.GameObjectShowHide:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        Renderer render = go.GetComponent<Renderer>();
                        if (!render) break;
                        switch (param[2] as string)
                        {
                            case "隐藏":
                                {
                                    render.enabled = false;
                                    break;
                                }
                            case "显示":
                                {
                                    render.enabled = true;
                                    break;
                                }
                            case "切换":
                                {
                                    render.enabled = !render.enabled;
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectActive:
                case EGenericStandardScriptID.SetGameObjectActive:
                    {
                        //Debug.Log(this.GetType().Name + ".RunScript ModelActive 0: ");
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        //Debug.Log(this.GetType().Name + ".RunScript ModelActive 1: ");
                        switch (param[2] as string)
                        {
                            case "是":
                                {
                                    go.SetActive(true);
                                    break;
                                }
                            case "否":
                                {
                                    go.SetActive(false);
                                    break;
                                }
                            case "切换":
                                {
                                    go.SetActive(!go.activeSelf);
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectActiveFlash:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        if (gameObjects_GameObjectActiveFlash.Contains(go))
                        {
                            gameObjects_GameObjectActiveFlash.RemoveWhere(i => !i);
                            break;
                        }
                        var hz = 2 * (float)param[2];
                        if (hz <= 0) break;
                        var tl = (float)param[3];
                        if (tl <= 0) break;
                        var count = (int)(tl * hz);
                        if (count <= 1) break;

                        var t = 1 / hz;
                        var active = go.activeSelf;
                        gameObjects_GameObjectActiveFlash.Add(go);
                        DelayCall(count, t, i =>
                        {
                            if (i + 1 == count)
                            {
                                go.SetActive(active);
                                gameObjects_GameObjectActiveFlash.Remove(go);
                            }
                            else
                            {
                                go.SetActive(!go.activeSelf);
                            }
                        });
                        break;
                    }
                case EGenericStandardScriptID.SubLevelGameObjectActive:
                case EGenericStandardScriptID.SetSubLevelGameObjectActive:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;

                        go = CommonFun.GetChildGameObject(go.transform, param[2] as string);
                        if (!go) break;

                        switch (param[3] as string)
                        {
                            case "是":
                                {
                                    go.SetActive(true);
                                    break;
                                }
                            case "否":
                                {
                                    go.SetActive(false);
                                    break;
                                }
                            case "切换":
                                {
                                    go.SetActive(!go.activeSelf);
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ParentLevelGameObjectActive:
                case EGenericStandardScriptID.SetParentLevelGameObjectActive:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        if (!go.transform.parent)
                        {
                            //说明go已经是根对象
                            break;
                        }

                        string parentLevelName = param[2] as string;
                        if (string.IsNullOrEmpty(parentLevelName) || parentLevelName == go.transform.parent.name)
                        {
                            //如果为空或者当前模型直系父节点的名称，那么直接指向当前游戏对象的直系父节点模型(游戏对象)
                            go = go.transform.parent.gameObject;
                        }
                        else
                        {
                            //说明当前游戏对象的父节点是根模型
                            if (!go.transform.parent.parent)
                            {
                                go = null;
                                foreach (var g in gameObject.scene.GetRootGameObjects())
                                {
                                    if (g.name == parentLevelName)
                                    {
                                        go = g;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                go = CommonFun.GetChildGameObject(go.transform.parent.parent, parentLevelName);
                            }
                        }
                        //父级节点游戏对象无效
                        if (!go) break;

                        go.SetActive(CommonFun.BoolChange(go.activeInHierarchy, (EBool)param[3]));
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SameLevelGameObjectActive:
                case EGenericStandardScriptID.SetSameLevelGameObjectActive:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;

                        string sameLevelName = param[2] as string;
                        if (!string.IsNullOrEmpty(sameLevelName) && sameLevelName != go.name)
                        {
                            //如果为空，直接指向当前游戏对象 --> 保持不变
                            //不为空且不是当前游戏对象，从当前游戏对象的父节点中查找指定名称的游戏对象
                            go = CommonFun.GetChildGameObject(go.transform.parent, sameLevelName);
                            if (!go) break;
                        }
                        go.SetActive(CommonFun.BoolChange(go.activeInHierarchy, (EBool)param[3]));
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetGameObjectName:
                    {
                        GameObject go = param[1] as GameObject;
                        if (go != null)
                        {
                            return new ReturnValue(true, go.name);
                        }
                        break;
                    }
                case EGenericStandardScriptID.GameObjectRename:
                case EGenericStandardScriptID.SetGameObjectName:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        string newName = param[2] as string;
                        if (string.IsNullOrEmpty(newName)) break;
                        go.name = newName;
                        return new ReturnValue(true, CommonFun.GameObjectToString(go));
                    }
                case EGenericStandardScriptID.GameObjectClone:
                    {
                        string goName = param[2] as string;
                        if (string.IsNullOrEmpty(goName)) break;
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        GameObject newGo = (GameObject)GameObject.Instantiate(go, go.transform.position, go.transform.rotation);
                        if (newGo == null) break;
                        newGo.name = goName;
                        GameObject parentObj = param[3] as GameObject;
                        if (!parentObj) parentObj = go.transform.gameObject;
                        if (parentObj)
                        {
                            newGo.transform.parent = parentObj.transform;
                            newGo.transform.localScale = parentObj.transform.localScale;
                        }
                        return new ReturnValue(true, CommonFun.GameObjectToString(newGo));
                    }
                case EGenericStandardScriptID.GameObjectRemove:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        GameObject.Destroy(go);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectRemoveChild:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        foreach (GameObject obj in CommonFun.GetChildGameObjects(go.transform))
                        {
                            GameObject.Destroy(obj);
                        }
                        ScriptManager.DelayCallUDF(param[2] as string, param[3] as string);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetGameObjectParentName:
                    {
                        GameObject go = param[1] as GameObject;
                        if (go != null)
                        {
                            Transform parent = go.transform.parent;
                            if (parent)
                                return new ReturnValue(true, parent.name);
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetGameObjectPath:
                    {
                        GameObject go = param[1] as GameObject;
                        if (go != null)
                        {
                            return new ReturnValue(true, CommonFun.GameObjectToString(go));
                        }
                        break;
                    }
                case EGenericStandardScriptID.GameObjectExistChild:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        if (go.transform.childCount != 0)
                        {
                            return ReturnValue.Yes;
                        }
                        return ReturnValue.No;
                    }
                case EGenericStandardScriptID.GameObjectOffset:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        switch (param[2] as string)
                        {
                            case "世界坐标系":
                                {
                                    go.transform.Translate((Vector3)param[3], Space.World);
                                    break;
                                }
                            case "自身坐标系":
                                {
                                    go.transform.Translate((Vector3)param[3], Space.Self);
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectRotate:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        switch (param[2] as string)
                        {
                            case "世界坐标系":
                                {
                                    go.transform.Rotate((Vector3)param[3], Space.World);
                                    break;
                                }
                            case "自身坐标系":
                                {
                                    go.transform.Rotate((Vector3)param[3], Space.Self);
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectPosition:
                case EGenericStandardScriptID.SetGameObjectPosition:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        switch (param[2] as string)
                        {
                            case "世界坐标系":
                                {
                                    go.transform.position = (Vector3)param[3];
                                    break;
                                }
                            case "自身坐标系":
                                {
                                    go.transform.localPosition = (Vector3)param[3];
                                    break;
                                }
                            default: return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetGameObjectPosition:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        switch (param[2] as string)
                        {
                            case "自身坐标系": return ReturnValue.True(CommonFun.Vector3ToString(go.transform.localPosition));
                            default: return ReturnValue.True(CommonFun.Vector3ToString(go.transform.position));
                        }
                    }
                case EGenericStandardScriptID.GetGameObjectEularAngle:
                    {
                        GameObject gameObj = param[1] as GameObject;
                        if (gameObj)
                        {
                            switch (param[3] as string)
                            {
                                case "自身坐标系":
                                    {
                                        return new ReturnValue(true, CommonFun.Vector3ToString(gameObj.transform.localEulerAngles));
                                    }
                                default:
                                    {
                                        return new ReturnValue(true, CommonFun.Vector3ToString(gameObj.transform.eulerAngles));
                                    }
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetGameObjectEularAngle:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        switch (param[3] as string)
                        {
                            case "自身坐标系":
                                {
                                    go.transform.localEulerAngles = (Vector3)param[2];
                                    break;
                                }
                            default:
                                {
                                    go.transform.eulerAngles = (Vector3)param[2];
                                    break;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectLookAt:
                case EGenericStandardScriptID.SetGameObjectLookAt:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        go.transform.LookAt((Vector3)param[2], (Vector3)param[3]);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetGameObjectScale:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        return ReturnValue.True(go.transform.localScale.ToString());
                    }
                case EGenericStandardScriptID.SetGameObjectScale:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        go.transform.localScale = (Vector3)param[2];
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectLookAtGameObject:
                    {
                        GameObject go = param[1] as GameObject;
                        GameObject targetGO = param[2] as GameObject;
                        if (!go || !targetGO) break;
                        go.transform.LookAt(targetGO.transform, (Vector3)param[3]);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GameObjectOverlap:
                case EGenericStandardScriptID.SetGameObjectOverlap:
                    {
                        GameObject go = param[1] as GameObject;
                        if (!go) break;
                        GameObject target = param[2] as GameObject;
                        if (target == null) break;
                        go.transform.position = target.transform.position;
                        go.transform.rotation = target.transform.rotation;
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetGameObjectColor:
                    {
                        GameObject model = param[1] as GameObject;
                        if (model)
                        {
                            Color color = (Color)param[2];
                            EBool2 isContainChildren = (EBool2)param[3];
                            var meshRendererList = new List<Renderer>();

                            if (isContainChildren == EBool2.Yes)
                            {
                                // 包含子物体
                                var meshRenderers = model.GetComponentsInChildren<Renderer>();
                                foreach (var mr in meshRenderers)
                                {
                                    meshRendererList.Add(mr);
                                }
                            }
                            else
                            {
                                // 不包含子物体
                                var meshRender = model.GetComponent<Renderer>();
                                if (meshRender) meshRendererList.Add(meshRender);
                            }

                            // 设置材质颜色
                            meshRendererList.ForEach(mr =>
                            {
                                foreach (var mat in mr.materials)
                                {
                                    if (mat) mat.color = color;
                                }
                            });
                            return ReturnValue.Create(meshRendererList.Count > 0);
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetGameObjectParent:
                    {
                        var cgo = param[1] as GameObject;
                        if (!cgo) break;
                        var pgo = param[2] as GameObject;
                        if (pgo)
                        {
                            cgo.transform.parent = pgo.transform;
                        }
                        else
                        {
                            cgo.transform.parent = null;
                        }
                        return new ReturnValue(true, CommonFun.GameObjectToString(cgo));
                    }
                case EGenericStandardScriptID.GetGameObjectSiblingIndex:
                    {
                        var go = param[1] as GameObject;
                        if (go)
                        {
                            return ReturnValue.Create(true, go.transform.GetSiblingIndex());
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetGameObjectSiblingIndex:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        switch (param[2] as string)
                        {
                            case "编号":
                                {
                                    var index = (int)param[3];
                                    if (index < 0) index = 0;
                                    go.transform.SetSiblingIndex(index);
                                    return ReturnValue.Yes;
                                }
                            case "第一个":
                                {
                                    go.transform.SetAsFirstSibling();
                                    return ReturnValue.Yes;
                                }
                            case "最后一个":
                                {
                                    go.transform.SetAsLastSibling();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetGameObjectLayerID:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        return ReturnValue.True(go.layer.ToString());
                    }
                case EGenericStandardScriptID.SetGameObjectLayerID:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        int layer = (int)param[2];
                        bool isIncludeChildren = ((EBool2)param[3] == EBool2.Yes);
                        if (isIncludeChildren)
                        {
                            Transform[] children = go.GetComponentsInChildren<Transform>();
                            foreach (Transform item in children)
                            {
                                item.gameObject.layer = layer;
                            }
                        }
                        else
                        {
                            go.layer = layer;
                        }

                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetGameObjectLayerName:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        return ReturnValue.True(LayerMask.LayerToName(go.layer));
                    }
                case EGenericStandardScriptID.SetGameObjectLayerName:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        int layer = LayerMask.NameToLayer(param[2] as string);
                        if (layer >= 0 && layer < 32)
                        {
                            bool isIncludeChildren = ((EBool2)param[3] == EBool2.Yes);
                            if (isIncludeChildren)
                            {
                                Transform[] children = go.GetComponentsInChildren<Transform>();
                                foreach (Transform item in children)
                                {
                                    item.gameObject.layer = layer;
                                }
                            }
                            else
                            {
                                go.layer = layer;
                            }

                            return ReturnValue.Yes;
                        }
                        else
                        {
                            Debug.LogError(param[2] + "层不存在！");
                        }

                        break;
                    }
                case EGenericStandardScriptID.GetGameObjectTagName:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        return ReturnValue.True(go.tag);
                    }
                case EGenericStandardScriptID.SetGameObjectTagName:
                    {
                        var go = param[1] as GameObject;
                        if (!go) break;

                        var tag = param[2] as string;
                        if (string.IsNullOrEmpty(tag)) break;

                        bool isIncludeChildren = ((EBool2)param[3] == EBool2.Yes);
                        if (isIncludeChildren)
                        {
                            Transform[] children = go.GetComponentsInChildren<Transform>();
                            foreach (Transform item in children)
                            {
                                item.gameObject.tag = tag;
                            }
                        }
                        else
                        {
                            go.tag = tag;
                        }

                        return ReturnValue.Yes;
                    }
                #endregion

                #region 场景资源处理脚本

                case EGenericStandardScriptID.LoadMainScene:
                    {
                        #region LoadMainScene
                        switch (param[1] as string)
                        {
                            case "同步加载":
                                {
                                    return new ReturnValue(SceneAssetsManager.LoadMainScene(false));
                                }
                            case "异步加载":
                                {
                                    return new ReturnValue(SceneAssetsManager.LoadMainScene(true));
                                }
                            default: break;
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.LoadSceneByBuildIndex:
                    {
                        #region LoadSceneByBuildIndex
                        int sceneBuildIndex = (int)param[1] - 1;
                        string operAsync = param[2] as string;
                        string operAdd = param[3] as string;
                        bool ret = false;
                        switch (operAsync)
                        {
                            case "同步加载":
                                {
                                    switch (operAdd)
                                    {
                                        case "清空加载":
                                            {
                                                ret = SceneAssetsManager.LoadScene(sceneBuildIndex);
                                                break;
                                            }
                                        case "累加加载":
                                            {
                                                ret = SceneAssetsManager.LoadScene(sceneBuildIndex, LoadSceneMode.Additive);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case "异步加载":
                                {
                                    string fun = param[4] as string;
                                    string tag = param[5] as string;
                                    switch (operAdd)
                                    {
                                        case "清空加载":
                                            {
                                                ret = SceneAssetsManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Single, fun, tag);
                                                break;
                                            }
                                        case "累加加载":
                                            {
                                                ret = SceneAssetsManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive, fun, tag);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        return new ReturnValue(ret);
                        #endregion
                    }
                case EGenericStandardScriptID.LoadSceneByName:
                    {
                        #region LoadSceneByName
                        string sceneName = param[1] as string;
                        string operAsync = param[2] as string;
                        string operAdd = param[3] as string;
                        bool checkSceneValid = ((EBool2)param[6]) == EBool2.Yes;
                        bool ret = false;
                        switch (operAsync)
                        {
                            case "同步加载":
                                {
                                    switch (operAdd)
                                    {
                                        case "清空加载":
                                            {
                                                ret = SceneAssetsManager.LoadScene(sceneName, LoadSceneMode.Single, checkSceneValid);
                                                break;
                                            }
                                        case "累加加载":
                                            {
                                                ret = SceneAssetsManager.LoadScene(sceneName, LoadSceneMode.Additive, checkSceneValid);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case "异步加载":
                                {
                                    string fun = param[4] as string;
                                    string tag = param[5] as string;
                                    switch (operAdd)
                                    {
                                        case "清空加载":
                                            {
                                                ret = SceneAssetsManager.LoadSceneAsync(sceneName, LoadSceneMode.Single, fun, tag, checkSceneValid);
                                                break;
                                            }
                                        case "累加加载":
                                            {
                                                ret = SceneAssetsManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive, fun, tag, checkSceneValid);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        return new ReturnValue(ret);
                        #endregion
                    }
                case EGenericStandardScriptID.ImprotAndLoadSceneByName:
                    {
                        #region ImprotAndLoadSceneByName
                        string path = param[1] as string;
                        string sceneName = param[2] as string;
                        string operAsync = param[3] as string;
                        string operAdd = param[4] as string;
                        bool ret = false;
                        switch (operAsync)
                        {
                            case "同步加载":
                                {
                                    switch (operAdd)
                                    {
                                        case "清空加载":
                                            {
                                                ret = SceneAssetsManager.ImportAndLoadScene(path, sceneName);
                                                break;
                                            }
                                        case "累加加载":
                                            {
                                                ret = SceneAssetsManager.ImportAndLoadScene(path, sceneName, LoadSceneMode.Additive);
                                                break;
                                            }
                                    }
                                    break;
                                }
                            case "异步加载":
                                {
                                    string fun = param[5] as string;
                                    string tag = param[6] as string;
                                    switch (operAdd)
                                    {
                                        case "清空加载":
                                            {
                                                //Debug.Log("ImprotAndLoadSceneAsync xxx: " + sceneName);
                                                ret = SceneAssetsManager.ImportAndLoadSceneAsync(path, sceneName, LoadSceneMode.Single, fun, tag);
                                                break;
                                            }
                                        case "累加加载":
                                            {
                                                ret = SceneAssetsManager.ImportAndLoadSceneAsync(path, sceneName, LoadSceneMode.Additive, fun, tag);
                                                break;
                                            }
                                    }
                                    break;
                                }
                        }
                        return new ReturnValue(ret);
                        #endregion
                    }
                case EGenericStandardScriptID.ImportSceneToMemory:
                    {
                        #region ImportSceneToMemory
                        string path = param[1] as string;
                        string sceneName = param[2] as string;
                        string operAsync = param[3] as string;
                        bool ret = false;
                        switch (operAsync)
                        {
                            case "同步导入":
                                {
                                    ret = SceneAssetsManager.ImportScene(path, sceneName);
                                    break;
                                }
                            case "异步导入":
                                {
                                    string fun = param[4] as string;
                                    string tag = param[5] as string;
                                    ret = SceneAssetsManager.ImportSceneAsync(path, sceneName, fun, tag);
                                    break;
                                }
                        }
                        return new ReturnValue(ret);
                        #endregion
                    }
                case EGenericStandardScriptID.GetAsyncImportAndLoadSceneProgress:
                    {
                        #region GetSceneAsyncLoadProgress
                        float progress = 0;
                        if (SceneAssetsManager.TryGetAsyncImportAndLoadSceneProgress(out progress))
                        {
                            return new ReturnValue(true, progress.ToString());
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.GetAsyncImportSceneProgress:
                    {
                        #region GetAsyncImportSceneProgress
                        float progress = 0;
                        if (SceneAssetsManager.TryGetAsyncImportSceneProgress(out progress))
                        {
                            return new ReturnValue(true, progress.ToString());
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.GetAsyncLoadSceneProgress:
                    {
                        #region GetAsyncLoadSceneProgress
                        float progress = 0;
                        if (SceneAssetsManager.TryGetAsyncLoadSceneProgress(out progress))
                        {
                            return new ReturnValue(true, progress.ToString());
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.GetSceneInfo:
                    {
                        #region GetSceneInfo
                        string oper = param[1] as string;
                        switch (oper)
                        {
                            case "场景数目":
                                {
                                    return new ReturnValue(true, SceneManager.sceneCount.ToString());
                                }
                            case "编译设置中场景数目":
                                {
                                    return new ReturnValue(true, SceneManager.sceneCountInBuildSettings.ToString());
                                }
                            case "当前场景打包索引":
                                {
                                    return new ReturnValue(true, (SceneManager.GetActiveScene().buildIndex + 1).ToString());
                                }
                            case "当前场景资源名":
                                {
                                    return new ReturnValue(true, SceneManager.GetActiveScene().name);
                                }
                            case "当前场景是否被加载":
                                {
                                    return new ReturnValue(SceneManager.GetActiveScene().isLoaded);
                                }
                            case "当前场景是否被修改":
                                {
                                    return new ReturnValue(SceneManager.GetActiveScene().isDirty);
                                }
                            case "当前场景路径":
                                {
                                    return new ReturnValue(true, SceneManager.GetActiveScene().path);
                                }
                            case "当前场景根游戏对象数目":
                                {
                                    return new ReturnValue(true, SceneManager.GetActiveScene().rootCount.ToString());
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.GetSceneAssetsInfo:
                    {
                        #region GetSceneAssetsInfo
                        string oper = param[1] as string;
                        switch (oper)
                        {
                            case "场景数目":
                                {
                                    return ReturnValue.True(SceneAssetsManager.sceneNameAssetsDictionary.Count);
                                }
                            case "场景资源名(通过索引)":
                                {
                                    string sceneName;
                                    if (SceneAssetsManager.TryGetSceneName((int)param[2] - 1, out sceneName))
                                    {
                                        return ReturnValue.True(sceneName);
                                    }
                                    break;
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.UnloadAllSubScene:
                    {
                        #region UnloadAllScene
                        switch (param[1] as string)
                        {
                            case "除主场景与当前场景外全部卸载":
                                {
                                    return ReturnValue.Create(SceneAssetsManager.UnloadAllSubSceneWithoutActiveScene());
                                }
                            case "除主场景外全部卸载":
                                {
                                    return ReturnValue.Create(SceneAssetsManager.UnloadAllSubScene());
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.UnloadSubScene:
                    {
                        return ReturnValue.Create(SceneAssetsManager.UnloadScene(param[1] as string));
                    }
                case EGenericStandardScriptID.UnloadUnusedAssetAndGCCollect:
                    {
                        SceneAssetsManager.UnloadUnusedAssetAndGCCollect();
                        return ReturnValue.Yes;
                    }
                #endregion

                #region Web请求操作
                case EGenericStandardScriptID.SendWebRequest:
                    {
                        string url = param[1] as string;
                        string fun = param[2] as string;
                        string var = param[3] as string;
                        string tag = param[4] as string;
                        return ReturnValue.Create(WebDataManager.Request(url, fun, var, tag));
                    }
                case EGenericStandardScriptID.GetSendWebRequestCount:
                    {
                        return ReturnValue.True(WebDataManager.GetRequestCount(EDataType.Text).ToString());
                    }
                case EGenericStandardScriptID.GetWebRequestProgress:
                    {
                        return ReturnValue.True(WebDataManager.GetRequestProgressByURL(param[1] as string).ToString());
                    }
                case EGenericStandardScriptID.WebDownloadFile:
                    {
                        string url = param[1] as string;
                        string fun = param[2] as string;
                        string var = param[3] as string;
                        string tag = param[4] as string;
                        return ReturnValue.Create(WebDataManager.DownloadFile(url, fun, var, tag));
                    }
                case EGenericStandardScriptID.GetWebDownloadFileCount:
                    {
                        return ReturnValue.True(WebDataManager.GetRequestCount(EDataType.File).ToString());
                    }
                case EGenericStandardScriptID.GetWebDownloadFileProgress:
                    {
                        return ReturnValue.True(WebDataManager.GetRequestProgressByURL(param[1] as string).ToString());
                    }
                case EGenericStandardScriptID.CheckNetworkConnection:
                    {
                        return ReturnValue.Create(CommonFun.NetIsOK());
                    }
                case EGenericStandardScriptID.GetInternetReachabilityInfo:
                    {
                        return ReturnValue.True(Application.internetReachability.ToString());
                    }
                #endregion

                #region 声音处理
                // 声音播放控制
                case EGenericStandardScriptID.AudioSoundPlay:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (audioSrc == null) break;
                        string type = param[2] as string;
                        switch (type)
                        {
                            case "播放":
                                audioSrc.Play();
                                break;
                            case "暂停":
                                audioSrc.Pause();
                                break;
                            case "停止":
                                audioSrc.Stop();
                                break;
                        }

                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetAudioSoundProgress:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (!audioSrc || audioSrc.clip == null) break;
                        audioSrc.time = audioSrc.clip.length * (float)param[2];
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetAudioSoundTime:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (!audioSrc) break;
                        audioSrc.time = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetAudioSound:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (audioSrc == null) break;
                        AudioClip clip = param[2] as AudioClip;
                        audioSrc.clip = clip;
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetAudioSoundLoop:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (audioSrc == null) break;
                        bool loop = (param[2] as string) == "是" ? true : false;
                        audioSrc.loop = loop;
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.SetAudioSourceVolume:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (audioSrc == null) break;
                        audioSrc.volume = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetAudioSourceInfo:
                    {
                        AudioSource audioSrc = param[1] as AudioSource;
                        if (!audioSrc) break;
                        switch (param[2] as string)
                        {
                            case "播放状态":
                                {
                                    return ReturnValue.Create(audioSrc.isPlaying);
                                }
                            case "时长":
                                {
                                    if (!audioSrc.clip) break;
                                    return ReturnValue.True(audioSrc.clip.length.ToString());
                                }
                            case "时间":
                                {
                                    return ReturnValue.True(audioSrc.time.ToString());
                                }
                            case "进度":
                                {
                                    if (!audioSrc.clip) break;
                                    return ReturnValue.True((audioSrc.time / audioSrc.clip.length).ToString());
                                }
                            case "游戏对象":
                                {
                                    return ReturnValue.True(CommonFun.GameObjectToString(audioSrc.gameObject));
                                }
                            case "循环":
                                {
                                    return ReturnValue.Create(audioSrc.loop);
                                }
                            case "音量":
                                {
                                    return ReturnValue.True(audioSrc.volume.ToString());
                                }
                        }
                        break;
                    }
                #endregion

                #region 动画操作
                case EGenericStandardScriptID.SetAnimationWarpMode:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string oper = param[2] as string;
                        switch (oper)
                        {
                            case "Default":
                                {
                                    animation.wrapMode = WrapMode.Default;
                                    break;
                                }
                            case "Once":
                            case "Clamp":
                                {
                                    animation.wrapMode = WrapMode.Once;
                                    break;
                                }
                            case "Loop":
                                {
                                    animation.wrapMode = WrapMode.Loop;
                                    break;
                                }
                            case "PingPong":
                                {
                                    animation.wrapMode = WrapMode.PingPong;
                                    break;
                                }
                            case "ClampForever":
                                {
                                    animation.wrapMode = WrapMode.ClampForever;
                                    break;
                                }
                            default:
                                {
                                    animation.wrapMode = WrapMode.Default;
                                    break;
                                }
                        }
                        return ReturnValue.Yes;
                    }

                case EGenericStandardScriptID.GetAnimationWarpMode:
                    {
                        var animation = param[1] as Animation;
                        if (!animation) break;
                        return ReturnValue.True(animation.wrapMode.ToString());
                    }
                case EGenericStandardScriptID.AnimationPlay:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        PlayMode playMode;
                        switch (param[3] as string)
                        {
                            case "停止全部动画":
                                {
                                    playMode = PlayMode.StopAll;
                                    break;
                                }
                            case "停止同层动画":
                            default:
                                {
                                    playMode = PlayMode.StopSameLayer;
                                    break;
                                }
                        }
                        if (string.IsNullOrEmpty(aniName))
                        {
                            animation.Play(playMode);
                        }
                        else
                        {
                            animation.Play(aniName, playMode);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimationPlayQueued:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        PlayMode playMode;
                        switch (param[3] as string)
                        {
                            case "停止全部动画":
                                {
                                    playMode = PlayMode.StopAll;
                                    break;
                                }
                            case "停止同层动画":
                            default:
                                {
                                    playMode = PlayMode.StopSameLayer;
                                    break;
                                }
                        }
                        QueueMode queueMode;
                        switch (param[4] as string)
                        {
                            case "立即开始播放":
                                {
                                    queueMode = QueueMode.PlayNow;
                                    break;
                                }
                            case "完成其他动画后开始播放":
                            default:
                                {
                                    queueMode = QueueMode.CompleteOthers;
                                    break;
                                }
                        }
                        return ReturnValue.Create(animation.PlayQueued(aniName, queueMode, playMode) != null);
                    }
                case EGenericStandardScriptID.AnimationStop:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        if (string.IsNullOrEmpty(aniName))
                        {
                            animation.Stop();
                        }
                        else
                        {
                            animation.Stop(aniName);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimationRewind:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        if (string.IsNullOrEmpty(aniName))
                        {
                            animation.Rewind();
                        }
                        else
                        {
                            animation.Rewind(aniName);
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimationIsPlaying:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        if (string.IsNullOrEmpty(aniName))
                        {
                            return ReturnValue.Create(animation.isPlaying);
                        }
                        else
                        {
                            return ReturnValue.Create(animation.IsPlaying(aniName));
                        }
                    }
                case EGenericStandardScriptID.AnimationBlend:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        if (animation.GetClip(aniName) == null) break;
                        float targetWeight = (float)param[3];
                        float fadeLength = (float)param[4];
                        animation.Blend(aniName, targetWeight, fadeLength);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimationCrossFade:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        if (animation.GetClip(aniName) == null) break;
                        float fadeLength = (float)param[3];
                        PlayMode playMode;
                        switch (param[4] as string)
                        {
                            case "停止全部动画":
                                {
                                    playMode = PlayMode.StopAll;
                                    break;
                                }
                            case "停止同层动画":
                            default:
                                {
                                    playMode = PlayMode.StopSameLayer;
                                    break;
                                }
                        }
                        animation.CrossFade(aniName, fadeLength, playMode);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimationCrossFadeQueued:
                    {
                        Animation animation = param[1] as Animation;
                        if (animation == null) break;
                        string aniName = param[2] as string;
                        if (animation.GetClip(aniName) == null) break;
                        float fadeLength = (float)param[3];
                        PlayMode playMode;
                        switch (param[4] as string)
                        {
                            case "停止全部动画":
                                {
                                    playMode = PlayMode.StopAll;
                                    break;
                                }
                            case "停止同层动画":
                            default:
                                {
                                    playMode = PlayMode.StopSameLayer;
                                    break;
                                }
                        }
                        QueueMode queueMode;
                        switch (param[4] as string)
                        {
                            case "立即开始播放":
                                {
                                    queueMode = QueueMode.PlayNow;
                                    break;
                                }
                            case "完成其他动画后开始播放":
                            default:
                                {
                                    queueMode = QueueMode.CompleteOthers;
                                    break;
                                }
                        }
                        return ReturnValue.Create(animation.CrossFadeQueued(aniName, fadeLength, queueMode, playMode) != null);
                    }
                // 设置Animator动画参数值
                case EGenericStandardScriptID.SetAnimatorParamValue:
                    {
                        Animator anim = param[1] as Animator;
                        if (anim == null) break;
                        string setParamName = param[2] as string;
                        if (string.IsNullOrEmpty(setParamName)) break;
                        float paramValue = (float)param[4];
                        string type = param[3] as string;
                        switch (type)
                        {
                            case "布尔型":
                                anim.SetBool(setParamName, paramValue > 0f);
                                break;
                            case "浮点数":
                                anim.SetFloat(setParamName, paramValue);
                                break;
                            case "整数":
                                anim.SetInteger(setParamName, (int)paramValue);
                                break;
                            case "触发器":
                                if (paramValue > 0.0f) anim.SetTrigger(setParamName);
                                else anim.ResetTrigger(setParamName);
                                break;
                            default:
                                return ReturnValue.No;
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetAnimatorParamValue:
                    {
                        Animator animator = param[1] as Animator;
                        if (animator == null) break;
                        string setParamName = param[2] as string;
                        if (string.IsNullOrEmpty(setParamName)) break;
                        string type = param[3] as string;
                        switch (type)
                        {
                            case "布尔型":
                                return ReturnValue.Create(animator.GetBool(setParamName));
                            case "浮点数":
                                return ReturnValue.True(animator.GetFloat(setParamName).ToString());
                            case "整数":
                                return ReturnValue.True(animator.GetInteger(setParamName).ToString());
                        }
                        break;
                    }
                case EGenericStandardScriptID.AnimatorPlay:
                    {
                        Animator animator = param[1] as Animator;
                        if (animator == null) break;
                        string stateName = param[2] as string;
                        if (string.IsNullOrEmpty(stateName)) break;
                        int layer = (int)param[3];
                        float time = (float)param[4];
                        if (time < 0.0f) time = float.NegativeInfinity;
                        animator.Play(stateName, layer, time);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimatorPlayInFixedTime:
                    {
                        Animator animator = param[1] as Animator;
                        if (animator == null) break;
                        string stateName = param[2] as string;
                        if (string.IsNullOrEmpty(stateName)) break;
                        int layer = (int)param[3];
                        float time = (float)param[4];
                        if (time < 0.0f) time = float.NegativeInfinity;
                        animator.PlayInFixedTime(stateName, layer, time);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimatorCrossFade:
                    {
                        Animator animator = param[1] as Animator;
                        if (animator == null) break;
                        string stateName = param[2] as string;
                        if (string.IsNullOrEmpty(stateName)) break;
                        int layer = (int)param[3];
                        float time = (float)param[4];
                        if (time < 0.0f) time = float.NegativeInfinity;
                        float durTime = (float)param[5];
                        animator.CrossFade(stateName, durTime, layer, time);
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.AnimatorCrossFadeInFixedTime:
                    {
                        Animator animator = param[1] as Animator;
                        if (animator == null) break;
                        string stateName = param[2] as string;
                        if (string.IsNullOrEmpty(stateName)) break;
                        int layer = (int)param[3];
                        float time = (float)param[4];
                        if (time < 0.0f) time = 0.0f;
                        float durTime = (float)param[5];
                        animator.CrossFadeInFixedTime(stateName, durTime, layer, time);
                        return ReturnValue.Yes;
                    }
                #endregion

                #region 二维向量操作
                case EGenericStandardScriptID.Vector2Calculate:
                    {
                        Vector2 a = (Vector2)param[1];
                        string oper = param[2] as string;
                        Vector2 b = (Vector2)param[3];
                        switch (oper)
                        {
                            case "加":
                                {
                                    return new ReturnValue(true, a + b);
                                }
                            case "减":
                                {
                                    return new ReturnValue(true, a - b);
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.Vector2Create:
                    {
                        return new ReturnValue(true, CommonFun.Vector2ToString((Vector2)param[1]));
                    }
                case EGenericStandardScriptID.Vector2Modify:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        switch (param[2] as string)
                        {
                            case "x":
                                {
                                    v2.x = (float)param[3];
                                    break;
                                }
                            case "y":
                                {
                                    v2.y = (float)param[3];
                                    break;
                                }
                        }
                        return new ReturnValue(true, CommonFun.Vector2ToString(v2));
                    }
                case EGenericStandardScriptID.Vector2Merge:
                    {
                        Vector2 v2 = new Vector2((float)param[1], (float)param[2]);
                        return new ReturnValue(true, CommonFun.Vector2ToString(v2));
                    }
                case EGenericStandardScriptID.Vector2Split:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, v2.x.ToString());
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[3] as string, v2.y.ToString());
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.Vector2Magnitude:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        return new ReturnValue(true, v2.magnitude.ToString());
                    }
                case EGenericStandardScriptID.Vector2SqrMagnitude:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        return new ReturnValue(true, v2.sqrMagnitude.ToString());
                    }
                case EGenericStandardScriptID.Vector2Normalized:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        return new ReturnValue(true, CommonFun.Vector2ToString(v2.normalized));
                    }
                case EGenericStandardScriptID.Vector2Angle:
                    {
                        Vector2 v2From = (Vector2)param[1];
                        Vector2 v2To = (Vector2)param[2];
                        return new ReturnValue(true, Vector2.Angle(v2From, v2To).ToString());
                    }
                case EGenericStandardScriptID.Vector2ClampMagnitude:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        float maxLength = (float)param[2];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.ClampMagnitude(v2, maxLength)));
                    }
                case EGenericStandardScriptID.Vector2Distance:
                    {
                        Vector2 v2a = (Vector2)param[1];
                        Vector2 v2b = (Vector2)param[2];
                        return new ReturnValue(true, Vector2.Distance(v2a, v2b).ToString());
                    }
                case EGenericStandardScriptID.Vector2Dot:
                    {
                        Vector2 v2a = (Vector2)param[1];
                        Vector2 v2b = (Vector2)param[2];
                        return new ReturnValue(true, Vector2.Dot(v2a, v2b).ToString());
                    }
                case EGenericStandardScriptID.Vector2Lerp:
                    {
                        Vector2 v2a = (Vector2)param[1];
                        Vector2 v2b = (Vector2)param[2];
                        float t = (float)param[3];
                        if (t <= 0) t = 0;
                        else if (t > 1) t = 1;
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.Lerp(v2a, v2b, t)));
                    }
                case EGenericStandardScriptID.Vector2LerpUnclamped:
                    {
                        Vector2 v2a = (Vector2)param[1];
                        Vector2 v2b = (Vector2)param[2];
                        float t = (float)param[3];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.LerpUnclamped(v2a, v2b, t)));
                    }
                case EGenericStandardScriptID.Vector2Max:
                    {
                        Vector2 v2a = (Vector2)param[1];
                        Vector2 v2b = (Vector2)param[2];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.Max(v2a, v2b)));
                    }
                case EGenericStandardScriptID.Vector2Min:
                    {
                        Vector2 v2a = (Vector2)param[1];
                        Vector2 v2b = (Vector2)param[2];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.Min(v2a, v2b)));
                    }
                case EGenericStandardScriptID.Vector2MoveTowards:
                    {
                        Vector2 v2c = (Vector2)param[1];
                        Vector2 v2t = (Vector2)param[2];
                        float mdd = (float)param[3];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.MoveTowards(v2c, v2t, mdd)));
                    }
                case EGenericStandardScriptID.Vector2Reflect:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        Vector2 v2n = (Vector2)param[2];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.Reflect(v2, v2n.normalized)));
                    }
                case EGenericStandardScriptID.Vector2Scale:
                    {
                        Vector2 v2 = (Vector2)param[1];
                        Vector2 v2s = (Vector2)param[2];
                        return new ReturnValue(true, CommonFun.Vector2ToString(Vector2.Scale(v2, v2s)));
                    }
                case EGenericStandardScriptID.Vector2SmoothDamp:
                    {
                        Vector2 v2c = (Vector2)param[1];
                        Vector2 v2t = (Vector2)param[2];
                        Vector2 currentVelocity = (Vector2)param[3];
                        float smoothTime = (float)param[4];
                        float maxSpeed = (float)param[5];
                        if (maxSpeed < Vector2.kEpsilon) maxSpeed = Mathf.Infinity;
                        float deltaTime = (float)param[6];
                        string speedVarName = param[7] as string;
                        if (deltaTime < Vector2.kEpsilon) deltaTime = Time.deltaTime;
                        Vector2 nv2 = Vector2.SmoothDamp(v2c, v2t, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(speedVarName, CommonFun.Vector2ToString(currentVelocity));
                        return new ReturnValue(true, CommonFun.Vector2ToString(nv2));
                    }
                #endregion

                #region 三维向量操作
                case EGenericStandardScriptID.Vector3Calculate:
                    {
                        Vector3 a = (Vector3)param[1];
                        string oper = param[2] as string;
                        Vector3 b = (Vector3)param[3];
                        switch (oper)
                        {
                            case "加":
                                {
                                    return new ReturnValue(true, CommonFun.Vector3ToString(a + b));
                                }
                            case "减":
                                {
                                    return new ReturnValue(true, CommonFun.Vector3ToString(a - b));
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.Vector3Create:
                    {
                        return new ReturnValue(true, CommonFun.Vector3ToString((Vector3)param[1]));
                    }
                case EGenericStandardScriptID.Vector3Modify:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        switch (param[2] as string)
                        {
                            case "x":
                                {
                                    v3.x = (float)param[3];
                                    break;
                                }
                            case "y":
                                {
                                    v3.y = (float)param[3];
                                    break;
                                }
                            case "z":
                                {
                                    v3.z = (float)param[3];
                                    break;
                                }
                        }
                        return new ReturnValue(true, CommonFun.Vector3ToString(v3));
                    }
                case EGenericStandardScriptID.Vector3Merge:
                    {
                        Vector3 v3 = new Vector3((float)param[1], (float)param[2], (float)param[3]);
                        return new ReturnValue(true, CommonFun.Vector3ToString(v3));
                    }
                case EGenericStandardScriptID.Vector3Split:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, v3.x.ToString());
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[3] as string, v3.y.ToString());
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[4] as string, v3.z.ToString());
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.Vector3Magnitude:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        return new ReturnValue(true, v3.magnitude.ToString());
                    }
                case EGenericStandardScriptID.Vector3SqrMagnitude:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        return new ReturnValue(true, v3.sqrMagnitude.ToString());
                    }
                case EGenericStandardScriptID.Vector3Normalized:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        return new ReturnValue(true, CommonFun.Vector3ToString(v3.normalized));
                    }
                case EGenericStandardScriptID.Vector3Angle:
                    {
                        Vector3 v3From = (Vector3)param[1];
                        Vector3 v3To = (Vector3)param[2];
                        return new ReturnValue(true, Vector3.Angle(v3From, v3To).ToString());
                    }
                case EGenericStandardScriptID.Vector3ClampMagnitude:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        float maxLength = (float)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.ClampMagnitude(v3, maxLength)));
                    }
                case EGenericStandardScriptID.Vector3Cross:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Cross(v3a, v3b)));
                    }
                case EGenericStandardScriptID.Vector3Distance:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        return new ReturnValue(true, Vector3.Distance(v3a, v3b).ToString());
                    }
                case EGenericStandardScriptID.Vector3Dot:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        return new ReturnValue(true, Vector3.Dot(v3a, v3b).ToString());
                    }
                case EGenericStandardScriptID.Vector3Lerp:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        float t = (float)param[3];
                        if (t <= 0) t = 0;
                        else if (t > 1) t = 1;
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Lerp(v3a, v3b, t)));
                    }
                case EGenericStandardScriptID.Vector3LerpUnclamped:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        float t = (float)param[3];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.LerpUnclamped(v3a, v3b, t)));
                    }
                case EGenericStandardScriptID.Vector3Max:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Max(v3a, v3b)));
                    }
                case EGenericStandardScriptID.Vector3Min:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Min(v3a, v3b)));
                    }
                case EGenericStandardScriptID.Vector3MoveTowards:
                    {
                        Vector3 v3c = (Vector3)param[1];
                        Vector3 v3t = (Vector3)param[2];
                        float mdd = (float)param[3];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.MoveTowards(v3c, v3t, mdd)));
                    }
                case EGenericStandardScriptID.Vector3Project:
                    {
                        Vector3 v3c = (Vector3)param[1];
                        Vector3 v3n = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Project(v3c, v3n.normalized)));
                    }
                case EGenericStandardScriptID.Vector3ProjectOnPlane:
                    {
                        Vector3 v3c = (Vector3)param[1];
                        Vector3 v3n = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.ProjectOnPlane(v3c, v3n.normalized)));
                    }
                case EGenericStandardScriptID.Vector3Reflect:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        Vector3 v3n = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Reflect(v3, v3n.normalized)));
                    }
                case EGenericStandardScriptID.Vector3RotateTowards:
                    {
                        Vector3 v3c = (Vector3)param[1];
                        Vector3 v3t = (Vector3)param[2];
                        float mrd = (float)param[3];
                        float mmd = (float)param[4];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.RotateTowards(v3c, v3t, mrd, mmd)));
                    }
                case EGenericStandardScriptID.Vector3Scale:
                    {
                        Vector3 v3 = (Vector3)param[1];
                        Vector3 v3s = (Vector3)param[2];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Scale(v3, v3s)));
                    }
                case EGenericStandardScriptID.Vector3Slerp:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        float t = (float)param[3];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.Slerp(v3a, v3b, t)));
                    }
                case EGenericStandardScriptID.Vector3SlerpUnclamped:
                    {
                        Vector3 v3a = (Vector3)param[1];
                        Vector3 v3b = (Vector3)param[2];
                        float t = (float)param[3];
                        return new ReturnValue(true, CommonFun.Vector3ToString(Vector3.SlerpUnclamped(v3a, v3b, t)));
                    }
                case EGenericStandardScriptID.Vector3SmoothDamp:
                    {
                        Vector3 v3c = (Vector3)param[1];
                        Vector3 v3t = (Vector3)param[2];
                        Vector3 currentVelocity = (Vector3)param[3];
                        float smoothTime = (float)param[4];
                        float maxSpeed = (float)param[5];
                        if (maxSpeed < Vector3.kEpsilon) maxSpeed = Mathf.Infinity;
                        float deltaTime = (float)param[6];
                        string speedVarName = param[7] as string;
                        if (deltaTime < Vector3.kEpsilon) deltaTime = Time.deltaTime;
                        Vector3 nv3 = Vector3.SmoothDamp(v3c, v3t, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(speedVarName, CommonFun.Vector3ToString(currentVelocity));
                        return new ReturnValue(true, CommonFun.Vector3ToString(nv3));
                    }
                #endregion

                #region 输入处理脚本

                case EGenericStandardScriptID.GetMousePos:
                    {
                        switch (Input.touchCount)
                        {
                            case 0: return new ReturnValue(true, CommonFun.Vector3ToString(Input.mousePosition));
                            default: return new ReturnValue(true, CommonFun.Vector2ToString(Input.GetTouch(0).position) + "/0");
                        }
                    }
                case EGenericStandardScriptID.ScreenToWorldPoint:
                    {
                        Vector3 pos = (Vector3)param[1];
                        if (Camera.main != null)
                        {
                            return new ReturnValue(true, CommonFun.Vector3ToString(Camera.main.ScreenToWorldPoint(pos)));
                        }
                        break;
                    }
                case EGenericStandardScriptID.InputGetKey:
                    {
                        KeyCode kc = (KeyCode)param[1];
                        string status = param[2] as string;
                        bool ret = false;
                        switch (status)
                        {
                            case "按键按下中":
                                {
                                    ret = Input.GetKey(kc);
                                    break;
                                }
                            case "按键按下":
                                {
                                    ret = Input.GetKeyDown(kc);
                                    break;
                                }
                            case "按键弹起":
                                {
                                    ret = Input.GetKeyUp(kc);
                                    break;
                                }
                        }
                        return ReturnValue.Create(ret);
                    }
                case EGenericStandardScriptID.IfInputGetKey:
                case EGenericStandardScriptID.ElseIfInputGetKey:
                    {
                        #region IfInputGetKey
                        KeyCode kc = (KeyCode)param[1];
                        string status = param[2] as string;
                        bool ret = false;
                        switch (status)
                        {
                            case "按键按下中":
                                {
                                    ret = Input.GetKey(kc);
                                    break;
                                }
                            case "按键按下":
                                {
                                    ret = Input.GetKeyDown(kc);
                                    break;
                                }
                            case "按键弹起":
                                {
                                    ret = Input.GetKeyUp(kc);
                                    break;
                                }
                        }
                        string oper = param[3] as string;
                        switch (oper)
                        {
                            case "有效":
                                {
                                    break;
                                }
                            case "无效":
                                {
                                    ret = !ret;
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Create(ret);
                        #endregion
                    }
                case EGenericStandardScriptID.InputGetMouseButton:
                    {
                        int button = (int)(EMouseButtonType)param[1];
                        string status = param[2] as string;
                        bool ret = false;
                        switch (status)
                        {
                            case "按钮按下中":
                                {
                                    ret = Input.GetMouseButton(button);
                                    break;
                                }
                            case "按钮按下":
                                {
                                    ret = Input.GetMouseButtonDown(button);
                                    break;
                                }
                            case "按钮弹起":
                                {
                                    ret = Input.GetMouseButtonUp(button);
                                    break;
                                }
                        }
                        return ReturnValue.Create(ret);
                    }
                case EGenericStandardScriptID.IfInputGetMouseButton:
                case EGenericStandardScriptID.ElseIfInputGetMouseButton:
                    {
                        #region IfInputGetMouseButton
                        int button = (int)(EMouseButtonType)param[1];
                        string status = param[2] as string;
                        bool ret = false;
                        switch (status)
                        {
                            case "按钮按下中":
                                {
                                    ret = Input.GetMouseButton(button);
                                    break;
                                }
                            case "按钮按下":
                                {
                                    ret = Input.GetMouseButtonDown(button);
                                    break;
                                }
                            case "按钮弹起":
                                {
                                    ret = Input.GetMouseButtonUp(button);
                                    break;
                                }
                        }
                        string oper = param[3] as string;
                        switch (oper)
                        {
                            case "有效":
                                {
                                    break;
                                }
                            case "无效":
                                {
                                    ret = !ret;
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Create(ret);
                        #endregion
                    }
                case EGenericStandardScriptID.GetClickPointGameObject:
                    {
                        Vector3 screenPos;
                        if (!Camera.main) break;
                        if (!TryGetClickScreenPos(out screenPos)) break;
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh))
                        {
                            return new ReturnValue(true, CommonFun.GameObjectToString(rh.collider.gameObject));
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetClickPoint3DPos:
                    {
                        Vector3 screenPos;
                        if (!Camera.main) break;
                        if (!TryGetClickScreenPos(out screenPos)) break;
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh))
                        {
                            return new ReturnValue(true, CommonFun.Vector3ToString(rh.point));
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetClickInfo:
                    {
                        #region GetClickInfo
                        if (!Camera.main) break;
                        Vector3 screenPos;
                        if (!TryGetClickScreenPos(out screenPos)) break;
                        string infoType = param[1] as string;
                        float dis = (float)param[2];
                        return GetInfoByRay(infoType, screenPos, dis);
                        #endregion
                    }
                case EGenericStandardScriptID.GetScreenPointInfo:
                    {
                        #region GetClickInfo
                        Vector3 screenPos = (Vector3)param[1];
                        string infoType = param[2] as string;
                        float dis = (float)param[3];
                        return GetInfoByRay(infoType, screenPos, dis);
                        #endregion
                    }
                #endregion

                #region 文件处理脚本
                case EGenericStandardScriptID.FileDelete:
                    {
                        string path = param[1] as string;
                        try
                        {
                            FileHelper.Delete(path);
                            return ReturnValue.Yes;
                        }
                        catch { }
                        break;
                    }
                case EGenericStandardScriptID.FolderDelete:
                    {
                        string path = param[1] as string;
                        try
                        {
                            return ReturnValue.Create(DirectoryHelper.Delete(path, ((param[2] as string) == "删除文件夹以及文件夹内的全部子文件夹与文件")));
                        }
                        catch { }
                        break;
                    }
                case EGenericStandardScriptID.GetFilePathInfo:
                    {
                        try
                        {
                            string path = param[1] as string;
                            if (string.IsNullOrEmpty(path)) break;
                            switch (param[2] as string)
                            {
                                case "路径":
                                    {
                                        string str = System.IO.Path.GetDirectoryName(path);
                                        //if (string.IsNullOrEmpty(str)) break;
                                        return ReturnValue.True(str);
                                    }
                                case "文件名":
                                    {
                                        string str = System.IO.Path.GetFileNameWithoutExtension(path);
                                        //if (string.IsNullOrEmpty(str)) break;
                                        return ReturnValue.True(str);
                                    }
                                case "文件扩展名":
                                    {
                                        string str = System.IO.Path.GetExtension(path);
                                        //if (string.IsNullOrEmpty(str)) break;
                                        return ReturnValue.True(str);
                                    }
                                case "文件名(含扩展名)":
                                    {
                                        string str = System.IO.Path.GetFileName(path);
                                        //if (string.IsNullOrEmpty(str)) break;
                                        return ReturnValue.True(str);
                                    }
                                case "绝对路径":
                                    {
                                        string str = System.IO.Path.GetFullPath(path);
                                        //if (string.IsNullOrEmpty(str)) break;
                                        return ReturnValue.True(str);
                                    }
                                case "是否存在":
                                    {
                                        return ReturnValue.Create(System.IO.File.Exists(path));
                                    }
                            }
                        }
                        catch// (Exception ex)
                        {
                            //
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetFileText:
                    {
                        string path = param[1] as string;
                        //if (File.Exists(path))
                        {
                            //Debug.Log("File.Exists :" + path);
                            string var = param[2] as string;
                            string funName = param[3] as string;
                            string tag = param[4] as string;
                            return ReturnValue.Create(FileHandler.LoadFile(path, EDataType.Text, this.GetFileText, new ParamList("var", var, "funName", funName, "tag", tag, "varCollectionHost", param.localVarCollectionHost)));
                        }
                        //break;
                    }
                case EGenericStandardScriptID.SetFileText:
                    {
                        var path = param[1] as string;
                        switch (param[3] as string)
                        {
                            case "新建或覆盖":
                                {
                                    if (FileHelper.OutputFile(path, param[2] as string, false, ((EBool2)param[4]) == EBool2.Yes))
                                    {
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                }
                            case "新建":
                                {
                                    if (!FileHelper.Exists(path) && FileHelper.OutputFile(path, param[2] as string, false, ((EBool2)param[4]) == EBool2.Yes))
                                    {
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                }
                            case "覆盖":
                                {
                                    if (FileHelper.Exists(path) && FileHelper.OutputFile(path, param[2] as string, false, ((EBool2)param[4]) == EBool2.Yes))
                                    {
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.FileSearch:
                    {
                        string path = param[1] as string;
                        if (string.IsNullOrEmpty(path)) break;
                        switch (param[3] as string)
                        {
                            case "绝对路径":
                                {
                                    if (System.IO.File.Exists(path))
                                    {
                                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, path);
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                }
                            case "根据文件名智能查找":
                                {
                                    string existPath;
                                    if (PathHandler.LocalFileSmartSearch(path, out existPath))
                                    {
                                        ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, existPath);
                                        return ReturnValue.Yes;
                                    }
                                    break;
                                }
                            case "通配符查找":
                                {
                                    try
                                    {
                                        //获取文件名
                                        string fileName = System.IO.Path.GetFileName(path);
                                        if (string.IsNullOrEmpty(fileName)) break;

                                        //获取文件当前的文件夹路径信息
                                        string fileCurrentDirectoryPath = System.IO.Path.GetDirectoryName(path);
                                        if (!string.IsNullOrEmpty(fileCurrentDirectoryPath))
                                        {
                                            //优先从当前目录的子级目录中查找！
                                            string[] result = DirectoryHelper.GetFiles(fileCurrentDirectoryPath, fileName, System.IO.SearchOption.AllDirectories);
                                            if (result != null && result.Length > 0)
                                            {
                                                ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, result[0]);
                                                return ReturnValue.Yes;
                                            }
                                        }
                                        {
                                            //从沙盒的数据路径中查找
                                            string[] result = DirectoryHelper.GetFiles(PathHandler.persistentDataPath, fileName, System.IO.SearchOption.AllDirectories);
                                            if (result != null && result.Length > 0)
                                            {
                                                ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(param[2] as string, result[0]);
                                                return ReturnValue.Yes;
                                            }
                                        }
                                    }
                                    catch// (Exception ex)
                                    {
                                        //不处理异常
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetPathBaseInfo:
                    {
                        switch (param[1] as string)
                        {
                            case "ALT目录分隔符":
                                {
                                    return ReturnValue.True(System.IO.Path.AltDirectorySeparatorChar.ToString());
                                }
                            case "目录分隔符":
                                {
                                    return ReturnValue.True(System.IO.Path.DirectorySeparatorChar.ToString());
                                }
                            case "路径分隔符":
                                {
                                    return ReturnValue.True(System.IO.Path.PathSeparator.ToString());
                                }
                            case "卷分隔符":
                                {
                                    return ReturnValue.True(System.IO.Path.VolumeSeparatorChar.ToString());
                                }
                        }
                        break;
                    }
                #endregion

                #region 网络摄像设备操作
                case EGenericStandardScriptID.GetWebCamDeviceCount:
                    {
                        if (Application.HasUserAuthorization(UserAuthorization.WebCam) && WebCamTexture.devices != null)
                        {
                            return ReturnValue.True(WebCamTexture.devices.Length.ToString());
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetWebCamDeviceName:
                    {
                        int index = (int)param[1] - 1;
                        if (index >= 0 && Application.HasUserAuthorization(UserAuthorization.WebCam) && WebCamTexture.devices != null && index < WebCamTexture.devices.Length)
                        {
                            return ReturnValue.True(WebCamTexture.devices[index].name);
                        }
                        break;
                    }
                case EGenericStandardScriptID.CreateWebCamTexture:
                    {
                        WebCamTexture webCamTexture = WebCamManager.CreateOrGetWebCamTexture(param[1] as string);
                        if (webCamTexture != null)
                        {
                            webCamTexture.requestedWidth = (int)param[2];
                            webCamTexture.requestedHeight = (int)param[3];
                            webCamTexture.requestedFPS = (int)param[4];
                        }
                        break;
                    }
                case EGenericStandardScriptID.GetWebCamTextureInfo:
                    {
                        WebCamTexture webCamTexture = WebCamManager.GetWebCamTexture(param[1] as string);
                        if (webCamTexture != null)
                        {
                            string infoType = param[2] as string;
                            switch (infoType)
                            {
                                case "名称":
                                    {
                                        return ReturnValue.True(webCamTexture.name);
                                    }
                                case "设备名":
                                    {
                                        return ReturnValue.True(webCamTexture.deviceName);
                                    }
                                case "本帧画面缓冲区是否更新":
                                    {
                                        return ReturnValue.Create(webCamTexture.didUpdateThisFrame);
                                    }
                                case "是否播放中":
                                    {
                                        return ReturnValue.Create(webCamTexture.isPlaying);
                                    }
                                case "帧速率FPS":
                                    {
                                        return ReturnValue.True(webCamTexture.requestedFPS);
                                    }
                                case "画面宽度":
                                    {
                                        return ReturnValue.True(webCamTexture.requestedWidth);
                                    }
                                case "画面高度":
                                    {
                                        return ReturnValue.True(webCamTexture.requestedHeight);
                                    }
                                case "视频旋转角度":
                                    {
                                        return ReturnValue.True(webCamTexture.videoRotationAngle);
                                    }
                                case "视频垂直镜像":
                                    {
                                        return ReturnValue.Create(webCamTexture.videoVerticallyMirrored);
                                    }
                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case EGenericStandardScriptID.ControlWebCamTexture:
                    {
                        string oper = param[2] as string;
                        switch (oper)
                        {
                            case "播放":
                                {
                                    return ReturnValue.Create(WebCamManager.Play(param[1] as string));
                                }
                            case "暂停":
                                {
                                    return ReturnValue.Create(WebCamManager.Pause(param[1] as string));
                                }
                            case "停止":
                                {
                                    return ReturnValue.Create(WebCamManager.Stop(param[1] as string));
                                }
                            case "销毁":
                                {
                                    return ReturnValue.Create(WebCamManager.Release(param[1] as string));
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.ControlAllWebCamTexture:
                    {
                        string oper = param[1] as string;
                        switch (oper)
                        {
                            case "播放":
                                {
                                    WebCamManager.Play();
                                    break;
                                }
                            case "暂停":
                                {
                                    WebCamManager.Pause();
                                    break;
                                }
                            case "停止":
                                {
                                    WebCamManager.Stop();
                                    break;
                                }
                            case "销毁":
                                {
                                    WebCamManager.Release();
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.DrawWebCamTexture:
                    {
                        WebCamTexture webCamTexture = WebCamManager.GetWebCamTexture(param[1] as string);
                        if (webCamTexture != null)
                        {
                            ScaleMode sm;
                            switch (param[3] as string)
                            {
                                case "ScaleAndCrop":
                                    {
                                        sm = ScaleMode.ScaleAndCrop;
                                        break;
                                    }
                                case "ScaleToFit":
                                    {
                                        sm = ScaleMode.ScaleToFit;
                                        break;
                                    }
                                case "StretchToFill":
                                default:
                                    {
                                        sm = ScaleMode.StretchToFill;
                                        break;
                                    }
                            }
                            GUI.DrawTexture((Rect)param[2], webCamTexture, sm, (param[4] as string) != "否", (float)param[5]);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.DrawWebCamTextureWithAutoSize:
                    {
                        WebCamTexture webCamTexture = WebCamManager.GetWebCamTexture(param[1] as string);
                        if (webCamTexture != null)
                        {
                            ScaleMode sm;
                            switch (param[3] as string)
                            {
                                case "ScaleAndCrop":
                                    {
                                        sm = ScaleMode.ScaleAndCrop;
                                        break;
                                    }
                                case "ScaleToFit":
                                    {
                                        sm = ScaleMode.ScaleToFit;
                                        break;
                                    }
                                case "StretchToFill":
                                default:
                                    {
                                        sm = ScaleMode.StretchToFill;
                                        break;
                                    }
                            }
                            Rect rect = new Rect((Vector2)param[2], new Vector2(webCamTexture.requestedWidth, webCamTexture.requestedHeight));
                            GUI.DrawTexture(rect, webCamTexture, sm, (param[4] as string) != "否", (float)param[5]);
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                #endregion

                #region 数组

                case EGenericStandardScriptID.AddArray:
                    {
                        return ReturnValue.Create(scriptManager.varCollection.TryAddVar(param[1] as string, null, EVarType.Array, out _));
                    }
                case EGenericStandardScriptID.RemoveArray:
                    {
                        return ReturnValue.Create(scriptManager.varCollection.TryRemoveVar(param[1] as string));
                    }
                case EGenericStandardScriptID.GetArrayItemCount:
                    {
                        if (scriptManager.varCollection.TryGetVar(param[1] as string, out var array) && array.varType == EVarType.Array && array.TryGetChildCount(out var count))
                        {
                            return new ReturnValue(true, count);
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetArrayItemCount:
                    {
                        var dstCount = (int)param[2];
                        if (scriptManager.varCollection.TryGetVar(param[1] as string, out var array) && array.varType == EVarType.Array && array.TrySetChildCount(dstCount))
                        {
                            return new ReturnValue(true, dstCount);
                        }
                        break;
                    }
                case EGenericStandardScriptID.AddArrayItem:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        switch (param[3] as string)
                        {
                            case "数组末尾添加":
                                {
                                    array.TryAddChild(param[2] as string, out _);
                                    return ReturnValue.Yes;
                                }
                            case "数据存在则不添加":
                                {
                                    string value = param[2] as string;
                                    if (array.TryGetChild((i, hv) => hv.stringValue == value, out _, out int _)) break;

                                    array.TryAddChild(value, out _);
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.RemoveArrayItemByValue:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        var list = (IList)array;
                        switch (param[3] as string)
                        {
                            case "根据数据值删除所有重复数据":
                                {
                                    string value = param[2] as string;
                                    list.RemoveAll(o => ((IHierarchyVar)o).stringValue == value);
                                    return ReturnValue.Yes;
                                }
                            case "根据数据值删除正向第一个查找到的数据":
                                {
                                    string value = param[2] as string;
                                    list.RemoveFirst(o => ((IHierarchyVar)o).stringValue == value);
                                    return ReturnValue.Yes;
                                }
                            case "根据数据值删除反向第一个查找到的数据":
                                {
                                    string value = param[2] as string;
                                    list.RemoveLast(o => ((IHierarchyVar)o).stringValue == value);
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.RemoveArrayItemByIndex:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;

                        switch (param[3] as string)
                        {
                            case "根据索引删除数组中的值":
                                {
                                    return ReturnValue.Create(array.TryRemoveChild((int)param[2], true, out _));
                                }
                            case "删除数组中第一个数据(忽略索引值)":
                                {
                                    return ReturnValue.Create(array.TryRemoveChild(1, true, out _));
                                }
                            case "删除数组中最后一个数据(忽略索引值)":
                                {
                                    return ReturnValue.Create(array.TryRemoveChild(-1, true, out _));
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.ClearArrayItem:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        var list = (IList)array;
                        list.Clear();
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.GetArrayItemByIndex:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        if (array.TryGetChild((int)param[2], true, out var item))
                        {
                            return new ReturnValue(true, item.stringValue);
                        }
                        break;
                    }
                case EGenericStandardScriptID.SetArrayItem:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        if (array.TryGetChild((int)param[2], true, out var item) && item.TrySetValue(param[3] as string, EVarType.String))
                        {
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EGenericStandardScriptID.CheckArrayItem:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;

                        var vluae = param[2] as string;
                        var list = (IList)array;
                        return ReturnValue.Create(list.AnyMatch(o => ((IHierarchyVar)o).stringValue == vluae));
                    }
                case EGenericStandardScriptID.ArrayBaseOperation:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        var list = ((HierarchyVar)array).arrayValue;
                        switch (param[2] as string)
                        {
                            case "去除重复值":
                                {
                                    //必须做一次深拷贝~不然之后的clear操作会将迭代器清空
                                    List<string> newArray = new List<string>(list.Cast(hv => hv.stringValue).Distinct());
                                    array.TryClearChildren();
                                    array.TryAddChildren(newArray);
                                    break;
                                }
                            case "排序(升序)":
                                {
                                    list.Sort((x, y) => string.Compare(x.stringValue, y.stringValue));
                                    array.UpdateHierarchy();
                                    break;
                                }
                            case "排序(降序)":
                                {
                                    list.Sort((x, y) => string.Compare(x.stringValue, y.stringValue));
                                    list.Reverse();
                                    array.UpdateHierarchy();
                                    break;
                                }
                            case "转置(前后顺序反转)":
                                {
                                    list.Reverse();
                                    array.UpdateHierarchy();
                                    break;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ConvertStringToArray:
                    {
                        var arrayName = param[2] as string;
                        if (string.IsNullOrEmpty(arrayName)) break;
                        if (!scriptManager.varCollection.varDictionary.TryGetOrAddVar(arrayName, EVarType.Array, out var array)) break;
                        array.TrySetVarType(EVarType.Array, null);
                        var list = ((HierarchyVar)array).arrayValue;

                        string str = param[1] as string;
                        string spe = param[3] as string;
                        string oper = param[4] as string;
                        switch (oper)
                        {
                            case "直接追加":
                                {
                                    list.AddRange(str.Split(new string[] { spe }, StringSplitOptions.None).Cast(v => new HierarchyVar(v)));
                                    array.UpdateHierarchy();
                                    break;
                                }
                            case "去除空字符后追加":
                                {
                                    list.AddRange(str.Split(new string[] { spe }, StringSplitOptions.RemoveEmptyEntries).Cast(v => new HierarchyVar(v)));
                                    array.UpdateHierarchy();
                                    break;
                                }
                            case "清空数组数据后直接追加":
                                {
                                    array.TryClearChildren();
                                    list.AddRange(str.Split(new string[] { spe }, StringSplitOptions.None).Cast(v => new HierarchyVar(v)));
                                    array.UpdateHierarchy();
                                    break;
                                }
                            case "请空数组数据并去除空字符后追加":
                                {
                                    array.TryClearChildren();
                                    list.AddRange(str.Split(new string[] { spe }, StringSplitOptions.RemoveEmptyEntries).Cast(v => new HierarchyVar(v)));
                                    array.UpdateHierarchy();
                                    break;
                                }
                            default:
                                {
                                    return ReturnValue.No;
                                }
                        }
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ConvertArrayToString:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var array) || array.varType != EVarType.Array) break;
                        var list = ((HierarchyVar)array).arrayValue;
                        string spe = param[2] as string; ;
                        return new ReturnValue(true, list.ToString(i => i.stringValue, spe));
                    }
                case EGenericStandardScriptID.ArrayMerge:
                    {
                        #region ArrayMerge

                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var arrayA) || arrayA.varType != EVarType.Array) break;
                        if (!scriptManager.varCollection.TryGetVar(param[2] as string, out var arrayB) || arrayB.varType != EVarType.Array) break;

                        if (!scriptManager.varCollection.varDictionary.TryGetOrAddVar(param[3] as string, EVarType.Array, out var arrayC)) break;
                        arrayC.TrySetVarType(EVarType.Array, null);

                        var dstArray = (arrayC as HierarchyVar).arrayValue;
                        var tmpA = (arrayA as HierarchyVar).arrayValue.ToList(hv => new HierarchyVar(hv.stringValue));
                        var tmpB = (arrayB as HierarchyVar).arrayValue.ToList(hv => new HierarchyVar(hv.stringValue));

                        if (arrayC == arrayA)
                        {
                            dstArray.AddRange(tmpB);
                        }
                        else if (arrayC == arrayB)
                        {
                            dstArray.AddRange(tmpB);
                        }
                        else
                        {
                            dstArray.AddRange(tmpA);
                            dstArray.AddRange(tmpB);
                        }
                        arrayC.UpdateHierarchy();
                        return ReturnValue.Yes;
                        #endregion
                    }
                case EGenericStandardScriptID.ArrayClone:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var arrayA) || arrayA.varType != EVarType.Array) break;
                        if (!scriptManager.varCollection.varDictionary.TryGetOrAddVar(param[2] as string, EVarType.Array, out var arrayB)) break;

                        if (arrayA == arrayB) break;

                        arrayB.TrySetVarType(EVarType.Array, null);
                        arrayB.TryClearChildren();
                        arrayB.TryAddChildren((arrayA as HierarchyVar).arrayValue.Cast(hv => hv.stringValue));
                        return ReturnValue.Yes;
                    }
                case EGenericStandardScriptID.ArraySort:
                    {
                        #region ArraySort
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var arrayA) || arrayA.varType != EVarType.Array) break;
                        var array = ((HierarchyVar)arrayA).arrayValue;

                        bool asc = ((param[3] as string).CompareTo("升序") == 0);
                        bool strFirst = ((param[4] as string).CompareTo("数值前字符串后") == 0);
                        string oper = param[2] as string;
                        switch (oper)
                        {
                            case "字符串比较":
                                {
                                    array.Sort((x, y) => string.Compare(x.stringValue, y.stringValue));
                                    if (!asc) array.Reverse();
                                    arrayA.UpdateHierarchy();
                                    break;
                                }
                            case "自然比较":
                                {
                                    array.Sort((x, y) => StringHelper.NaturalCompare(x.stringValue, y.stringValue));
                                    if (!asc) array.Reverse();
                                    arrayA.UpdateHierarchy();
                                    break;
                                }
                            case "转化整形数值后比较":
                                {
                                    array.Sort((x, y) =>
                                    {
                                        var xText = x.stringValue;
                                        var yText = y.stringValue;
                                        if (int.TryParse(xText, out var xi))
                                        {
                                            if (int.TryParse(yText, out var yi))
                                            {
                                                return xi - yi;
                                            }
                                            else
                                            {
                                                return strFirst ? 1 : -1;
                                            }
                                        }
                                        else
                                        {
                                            if (int.TryParse(yText, out var yi))
                                            {
                                                return strFirst ? -1 : 1;
                                            }
                                            else
                                            {
                                                return string.Compare(xText, yText);
                                            }
                                        }
                                    });
                                    if (!asc) array.Reverse();
                                    arrayA.UpdateHierarchy();
                                    break;
                                }
                            case "转化浮点数值后比较":
                                {
                                    array.Sort((x, y) =>
                                    {
                                        var xText = x.stringValue;
                                        var yText = y.stringValue;
                                        if (double.TryParse(xText, out var xi))
                                        {
                                            if (double.TryParse(yText, out var yi))
                                            {
                                                return Comparer<double>.Default.Compare(xi, yi);
                                            }
                                            else
                                            {
                                                return strFirst ? 1 : -1;
                                            }
                                        }
                                        else
                                        {
                                            if (double.TryParse(yText, out var yi))
                                            {
                                                return strFirst ? -1 : 1;
                                            }
                                            else
                                            {
                                                return string.Compare(xText, yText);
                                            }
                                        }
                                    });
                                    if (!asc) array.Reverse();
                                    arrayA.UpdateHierarchy();
                                    break;
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.WhileArray:
                    {
                        #region WhileArray
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out var arrayA) || arrayA.varType != EVarType.Array) break;
                        var array = ((HierarchyVar)arrayA).arrayValue;
                        var state = param.state;
                        if (state == null) break;
                        if (array.Count <= state.count) break;

                        if (scriptManager.varCollection.TrySetOrAddSetVarValue(param[2] as string, (state.count + 1).ToString()) && scriptManager.varCollection.TrySetOrAddSetVarValue(param[3] as string, array[state.count].stringValue))
                        {
                            return ReturnValue.Yes;
                        }
                        break;
                        #endregion
                    }
                #endregion

                #region 字典

                case EGenericStandardScriptID.AddDictionary:
                    {
                        return ReturnValue.Create(scriptManager.varCollection.TryAddVar(param[1] as string, null, EVarType.Dictionary, out _));
                    }
                case EGenericStandardScriptID.RemoveDictionary:
                    {
                        return ReturnValue.Create(scriptManager.varCollection.TryRemoveVar(param[1] as string));
                    }
                case EGenericStandardScriptID.UpdateDictionaryItem:
                    {
                        #region UpdateDictionaryItem
                        var varString = "$" + (param[1] as string) + "/" + (param[2] as string);
                        if (!VarStringAnalysisResult.TryParse(varString, out var result)) break;

                        IHierarchyVar hierarchyVar;
                        switch (param[5] as string)
                        {
                            case "键名中间层缺少自动补齐":
                                {
                                    if (!scriptManager.TrySetOrAddSetHierarchyVarValue(result, "", out hierarchyVar, out HierarchyKey extensionHierarchyKey)) return ReturnValue.No;
                                    break;
                                }
                            case "键名中间层缺少不补齐":
                                {
                                    if (!scriptManager.TrySetHierarchyVarValue(result, "", out hierarchyVar, out HierarchyKey extensionHierarchyKey)) return ReturnValue.No;
                                    break;
                                }
                            default: return ReturnValue.No;
                        }

                        string value = param[3] as string;
                        switch (param[4] as string)
                        {
                            case "字符":
                                {
                                    hierarchyVar.TrySetValue(value, EVarType.String);
                                    return ReturnValue.Yes;
                                }
                            case "子级字典对象":
                                {
                                    if (!scriptManager.varCollection.TryGetVar(value, out var tmpDic) || tmpDic.varType != EVarType.Dictionary) break;
                                    hierarchyVar.TrySetValue(value, EVarType.Dictionary);
                                    break;
                                }
                            case "子级数组对象":
                                {
                                    if (!scriptManager.varCollection.TryGetVar(value, out var tmpArray) || tmpArray.varType != EVarType.Array) break;
                                    hierarchyVar.TrySetValue(value, EVarType.Array);
                                    break;
                                }
                            case "字典字符串":
                                {
                                    hierarchyVar.TrySetValue(value, EVarType.Dictionary);
                                    break;
                                }
                            case "数组字符串":
                                {
                                    hierarchyVar.TrySetValue(value, EVarType.Array);
                                    break;
                                }
                        }
                        break;
                        #endregion
                    }
                case EGenericStandardScriptID.GetDictionaryItem:
                    {
                        var varString = "$" + (param[1] as string) + "/" + (param[2] as string);
                        if (!scriptManager.TryGetHierarchyVarValue(varString, out var varValue)) break;

                        return new ReturnValue(true, varValue);
                    }
                case EGenericStandardScriptID.GetDictionaryItemCountWhenArray:
                    {
                        var varString = "$" + (param[1] as string) + "/" + (param[2] as string);

                        if (!scriptManager.TryGetHierarchyVar(varString, out IHierarchyVar hierarchyVar, out HierarchyKey extensionHierarchyKey) || hierarchyVar.varType != EVarType.Array || !hierarchyVar.TryGetChildCount(out var count)) break;

                        return new ReturnValue(true, count.ToString());
                    }
                case EGenericStandardScriptID.GetDictionaryItemCountWhenObject:
                    {
                        var varString = "$" + (param[1] as string) + "/" + (param[2] as string);

                        if (!scriptManager.TryGetHierarchyVar(varString, out IHierarchyVar hierarchyVar, out HierarchyKey extensionHierarchyKey) || hierarchyVar.varType != EVarType.Dictionary || !hierarchyVar.TryGetChildCount(out var count)) break;

                        return new ReturnValue(true, count.ToString());
                    }
                case EGenericStandardScriptID.GetDictionaryItemKeyWhenObjectByIndex:
                    {
                        var varString = "$" + (param[1] as string) + "/" + (param[2] as string);

                        if (!scriptManager.TryGetHierarchyVar(varString, out IHierarchyVar hierarchyVar, out HierarchyKey extensionHierarchyKey) || hierarchyVar.varType != EVarType.Dictionary) break;

                        if (!hierarchyVar.TryGetChild((int)param[3], true, out var child)) break;

                        return new ReturnValue(true, child.name);
                    }
                case EGenericStandardScriptID.ConvertStringToDictionary:
                    {
                        if (!scriptManager.varCollection.TryGetOrAddVar(param[1] as string, EVarType.Dictionary, out IHierarchyVar hierarchyVar)) break;

                        return ReturnValue.Create(hierarchyVar.TrySetValue(param[2] as string, EVarType.Invalid));
                    }
                case EGenericStandardScriptID.ConvertDictionaryToString:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out IHierarchyVar hierarchyVar)) break;
                        return new ReturnValue(true, hierarchyVar.stringValue);
                    }
                case EGenericStandardScriptID.ShowDictionaryWindow:
                    {
                        if (!scriptManager.varCollection.TryGetVar(param[1] as string, out IHierarchyVar hierarchyVar)) break;

                        hierarchyVarWindow.dictionary = (HierarchyVar)hierarchyVar;
                        switch ((EBool)param[3])
                        {
                            case EBool.Yes:
                                {
                                    hierarchyVarWindow.rect = (Rect)param[2];
                                    break;
                                }
                        }
                        hierarchyVarWindow._visable = CommonFun.BoolChange(hierarchyVarWindow._visable, (EBool)param[4]);
                        return ReturnValue.Create(hierarchyVarWindow.dictionary != null);
                    }
                case EGenericStandardScriptID.GetDictionaryWindowInfo:
                    {
                        switch (param[1] as string)
                        {
                            case "可见性":
                                {
                                    return ReturnValue.Create(hierarchyVarWindow._visable);
                                }
                            case "位置尺寸":
                                {
                                    return ReturnValue.True(CommonFun.RectToString(hierarchyVarWindow.rect));
                                }
                        }
                        break;
                    }
                case EGenericStandardScriptID.WhileDictionary:
                    {
                        #region WhileDictionary

                        var state = param.state;
                        if (state == null) break;

                        var varString = "$" + (param[1] as string) + "/" + (param[2] as string);

                        if (!scriptManager.TryGetHierarchyVar(varString, out IHierarchyVar hierarchyVar, out HierarchyKey extensionHierarchyKey)) break;

                        if (!hierarchyVar.TryGetChild(state.count, false, out var child)) break;

                        scriptManager.varCollection.TrySetOrAddSetVarValue(param[3] as string, child.name);
                        scriptManager.varCollection.TrySetOrAddSetVarValue(param[4] as string, child.stringValue);
                        return ReturnValue.Yes;

                        #endregion
                    }
                    #endregion
            }

            return ReturnValue.No;
        }

        private int screenCaptureIndex = 0;

        private bool TryGetFilePathInfo(string path, out string dir, out string fileNameWioutExt, out string extWithDot)
        {
            try
            {
                var namePath = PathHelper.Format(path);
                dir = Path.GetDirectoryName(namePath);
                if (!string.IsNullOrEmpty(dir))
                {
                    dir = dir + "/";
                }
                fileNameWioutExt = Path.GetFileNameWithoutExtension(namePath);
                extWithDot = Path.GetExtension(namePath);
                return true;
            }
            catch
            {
                dir = null;
                fileNameWioutExt = null;
                extWithDot = null;
                return false;
            }
        }

        private bool wantsToQuit = true;

        private bool WantsToQuit() => wantsToQuit;

        private IEnumerator RequestUserAuthorization(UserAuthorization ua, string funName, IVarCollectionHost varCollectionHost)
        {
            yield return Application.RequestUserAuthorization(ua);
            ScriptManager.CallUDF(funName, ScriptOption.ReturnValueFlag + Application.HasUserAuthorization(ua), varCollectionHost);
        }

        private bool TryGetClickScreenPos(out Vector3 screenPos)
        {
            screenPos = new Vector3();
            switch (Input.touchCount)
            {
                case 0:
                    {
                        if (Input.GetMouseButtonDown((int)EMouseButtonType.Left))
                        {
                            screenPos = Input.mousePosition;
                            return true;
                        }
                        break;
                    }
                default:
                    {
                        Vector2 v2 = Input.GetTouch(0).position;
                        screenPos = new Vector3(v2.x, v2.y, 0);
                        return true;
                    }
            }
            return false;
        }

        private ReturnValue GetInfoByRay(string infoType, Vector3 screenPos, float dis)
        {
            switch (infoType)
            {
                case "屏幕坐标":
                    {
                        return new ReturnValue(true, CommonFun.Vector3ToString(screenPos));
                    }
                case "世界坐标":
                    {
                        return new ReturnValue(true, CommonFun.Vector3ToString(Camera.main.ScreenToWorldPoint(screenPos)));
                    }
                case "射线起点坐标":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        return new ReturnValue(true, CommonFun.Vector3ToString(ray.origin));
                    }
                case "射线方向":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        return new ReturnValue(true, CommonFun.Vector3ToString(ray.direction));
                    }
                case "三维坐标(限定距离)":
                    {
                        Vector3 v3 = Camera.main.ScreenPointToRay(screenPos).GetPoint(dis);
                        return new ReturnValue(true, CommonFun.Vector3ToString(v3));
                    }
                case "游戏对象":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.GameObjectToString(rh.collider.gameObject));
                        }
                        break;
                    }
                case "三维坐标(与游戏对象的碰撞点)":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.Vector3ToString(rh.point));
                        }
                        break;
                    }
                case "交点三角面法向量":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.Vector3ToString(rh.normal));
                        }
                        break;
                    }
                case "交点三角面城重心坐标":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.Vector3ToString(rh.barycentricCoordinate));
                        }
                        break;
                    }
                case "起点到碰撞点的距离":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, rh.distance.ToString());
                        }
                        break;
                    }
                case "碰撞点的UV纹理坐标":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.Vector2ToString(rh.textureCoord));
                        }
                        break;
                    }
                case "碰撞点的第二个UV纹理坐标":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.Vector2ToString(rh.textureCoord2));
                        }
                        break;
                    }
                case "碰撞器组件":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.GameObjectComponentToString(rh.collider));
                        }
                        break;
                    }
                case "碰撞的刚体组件":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            string retString = CommonFun.GameObjectComponentToString(rh.rigidbody);
                            if (!string.IsNullOrEmpty(retString)) return new ReturnValue(true, retString);
                        }
                        break;
                    }
                case "碰撞的Transform组件":
                    {
                        Ray ray = Camera.main.ScreenPointToRay(screenPos);
                        RaycastHit rh;
                        if (Physics.Raycast(ray, out rh, dis))
                        {
                            return new ReturnValue(true, CommonFun.GameObjectComponentToString(rh.transform));
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }

        private void GetFileText(IDataInfo dataInfo, object tag)
        {
            //Debug.Log("GetFileText :" + tag);
            ParamList pl = tag as ParamList;
            if (pl == null) return;
            string var = pl["var"] as string;
            string funName = pl["funName"] as string;

            if (dataInfo != null)
            {
                ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(var, dataInfo.text);
                ScriptManager.instance.ExecuteFunction(funName, pl["tag"] as string, pl["varCollectionHost"] as IVarCollectionHost);
            }
            else
            {
                ScriptManager.instance.ExecuteFunction(funName, "#False", pl["varCollectionHost"] as IVarCollectionHost);
            }
        }

        private void LoadUnityAssetObject(IDataInfo dataInfo, object tag)
        {
            ParamList pl = tag as ParamList;
            if (pl == null) return;
            UnityAssetObjectManager.LoadUnityAssetObject(dataInfo, pl["type"].ToString(), pl["name"].ToString(), obj => {

                //回调函数名
                var funName = pl["funName"] as string;
                if (obj)
                {
                    if (string.IsNullOrEmpty(obj.name))
                    {
                        //Debug.Log("修改名称！");
                        obj.name = System.IO.Path.GetFileNameWithoutExtension(dataInfo.fullPath);
                    }
                    //Debug.Log("obj: " + obj.name);
                    ScriptManager.instance.varCollection.TrySetOrAddSetVarValue(pl["var"] as string, CommonFun.UnityAssetObjectToString(obj));
                    ScriptManager.instance.ExecuteFunction(funName, pl["tag"] as string, pl["varCollectionHost"] as IVarCollectionHost);
                }
                else
                {
                    ScriptManager.instance.ExecuteFunction(funName, "#False", pl["varCollectionHost"] as IVarCollectionHost);
                }
            });
        }

        private HashSet<GameObject> gameObjects_GameObjectActiveFlash = new HashSet<GameObject>();

        public static void DelayCall(int count, float delayTime, Action<int> onCall)
        {
            if (count > 0 && delayTime > 0 && onCall != null)
            {
                DelayCall(0, count, delayTime, onCall);
            }
        }

        private static void DelayCall(int index, int count, float delayTime, Action<int> onCall)
        {
            if (index >= count) return;
            CommonFun.DelayCall(() =>
            {
                onCall(index);
                DelayCall(index + 1, count, delayTime, onCall);
            }, delayTime);
        }
    }
}