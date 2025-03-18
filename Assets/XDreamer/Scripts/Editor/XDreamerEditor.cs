using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.ProjectSettings;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension
{
    /// <summary>
    /// XDreamer编辑器类
    /// </summary>
    [InitializeOnLoad]
    public class XDreamerEditor : AssetPostprocessor, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        static XDreamerEditor()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            InitMacro();

            AssemblyReloadEvents.beforeAssemblyReload += XDreamerEvents.CallBeforeAssemblyReloadInEditor;
            AssemblyReloadEvents.afterAssemblyReload += XDreamerEvents.CallAfterAssemblyReloadInEditor;

            // 如果输入系统为新版，则设置为新旧版兼容模式
            var inputHandler = XProjectSetting.GetActiveInputHandler();
            switch (inputHandler)
            {
                case XProjectSetting.EInputHandler.InputSystem:
                    {
                        XProjectSetting.SetActiveInputHandler(XProjectSetting.EInputHandler.Both, true, "XDreamer需将输入系统设置为新旧版兼容模式");
                        break;
                    }
            }
        }

        /// <summary>
        /// 版本
        /// </summary>
        public static string version => Product.fullVersion;

        /// <summary>
        /// 编辑窗口菜单
        /// </summary>
        public const string EditorWindowMenu = XDreamerMenu.EditorWindow;

        /// <summary>
        /// 组件菜单
        /// </summary>
        public const string ComponentMenu = XDreamerMenu.Component;

        /// <summary>
        /// 最低主版本号
        /// </summary>
        public const int MinMajorVersion = 18;

        /// <summary>
        /// 或最新的编译宏后缀
        /// </summary>
        public const string OrNewer = "OR_NEWER";

        /// <summary>
        /// 当编译所有资产之前回调
        /// </summary>
        public static event Action onBeforeCompileAllAssets;

        /// <summary>
        /// 默认的编译目标组
        /// </summary>
        public static readonly BuildTargetGroup[] DefaultBuildTargetGroups = new BuildTargetGroup[]
           {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.iOS,
                BuildTargetGroup.Android,
                BuildTargetGroup.WebGL,
                BuildTargetGroup.WSA
           };

        /// <summary>
        /// 编译宏前缀
        /// </summary>
        public const string CompileMacroPrefix = nameof(XDREAMER);

        /// <summary>
        /// XDreamer宏对象
        /// </summary>
        public static readonly Macro XDREAMER = new Macro(CompileMacroPrefix, DefaultBuildTargetGroups);

        /// <summary>
        /// 检测编译宏
        /// </summary>
        //[MenuItem(ToolsMenu + "检测编译宏")]
        public static void CheckCompileMacro()
        {
            //Debug.Log("检测XDreamer编译宏");
            MacroAttribute.InvokeMacroMethod();
        }

        /// <summary>
        /// 移除所有XDreamer前缀编译宏
        /// </summary>
        //[MenuItem(ToolsMenu + "移除所有XDreamer前缀编译宏")]
        public static void RemoveAllCompileMacroOfXDreamerPrefix()
        {
            Macro.RemoveMacro((group, name) => name.StartsWith(CompileMacroPrefix), DefaultBuildTargetGroups);
        }

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            try
            {
                //编辑器运行时不处理编译宏
                if (EditorApplication.isPlayingOrWillChangePlaymode) return;

                XDREAMER.DefineIfNoDefined();
                var version = Version.Parse(Product.Version);

                //当前主版本号的版本宏字符串 生成
                var currentMajorVersionMacroString = Macro.CreateDefineName(CompileMacroPrefix, version.Major.ToString());

                //当前主版本号的版本宏
                var currentMajorVersion = new Macro(currentMajorVersionMacroString, DefaultBuildTargetGroups);

                //当前主版本号的后续版本宏
                var currentMajorVersionOrNewer = new Macro(Macro.CreateDefineName(currentMajorVersionMacroString, OrNewer), DefaultBuildTargetGroups);

                //当前版本号的版本宏
                var currentVersion = new Macro(Macro.CreateDefineName(currentMajorVersionMacroString, version.Minor.ToString(), version.Build == -1 ? "" : version.Build.ToString()), DefaultBuildTargetGroups);

                //先对可能存在的 <=当前主版本号 的宏做处理
                for (int i = MinMajorVersion; i <= version.Major; ++i)
                {
                    var majorVersionMacroString = Macro.CreateDefineName(CompileMacroPrefix, i.ToString());

                    //<=当前主版本号的后续版本宏 添加
                    var majorVersionMacroOrNewer = new Macro(Macro.CreateDefineName(majorVersionMacroString, OrNewer), DefaultBuildTargetGroups);
                    majorVersionMacroOrNewer.DefineIfNoDefined();

                    //<=当前主版本号的版本宏 移除
                    foreach (var targetGroup in DefaultBuildTargetGroups)
                    {
                        foreach (var defineName in Macro.GetScriptingDefineSymbols(targetGroup))
                        {
                            if (defineName.StartsWith(majorVersionMacroString))
                            {
                                if (defineName == majorVersionMacroOrNewer.defineName
                                    || defineName == currentMajorVersion.defineName
                                    || defineName == currentMajorVersionOrNewer.defineName
                                    || defineName == currentVersion.defineName)
                                {
                                    continue;
                                }
                                new Macro(defineName, DefaultBuildTargetGroups).UndefineAll();
                            }
                        }
                    }

                    //<当前主版本号的版本宏 移除
                    if (i < version.Major) new Macro(majorVersionMacroString, DefaultBuildTargetGroups).UndefineAll();
                }

                //当前主版本号的版本宏 添加
                currentMajorVersion.DefineIfNoDefined();

                //当前主版本号的后续版本宏 添加
                currentMajorVersionOrNewer.DefineIfNoDefined();

                //当前版本号的版本宏 添加
                currentVersion.DefineIfNoDefined();

                InitPorjectMacro();
            }
            catch { }
        }

        /// <summary>
        /// 当处理所有资产后回调
        /// </summary>
        public static event Action<string[], string[], string[], string[]> onPostprocessAllAssets;

        /// <summary>
        /// 当处理所有资产后回调
        /// </summary>
        /// <param name="importedAssets">导入的资产</param>
        /// <param name="deletedAssets">删除的资产</param>
        /// <param name="movedAssets">移动的资产</param>
        /// <param name="movedFromAssetPaths">被移动的资产</param>
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            XDreamerEvents.CallBeforeCompileAllAssetsInEditor();
            onBeforeCompileAllAssets?.Invoke();
            onPostprocessAllAssets?.Invoke(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
        }

        /// <summary>
        /// 清除当前场景游戏对象的无效脚本组件
        /// </summary>
        //[MenuItem(ToolsMenu + "清除当前场景游戏对象的无效脚本组件")]
        public static void ClearMissingScriptsInCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid()) return;
            int missingCount = 0;
            foreach (var go in scene.GetRootGameObjects())
            {
                ClearMissingScripts(go, ref missingCount);
            }
            Debug.Log(string.Format("统计移除无效脚本的数量:{0}", missingCount));

        }

        private static void ClearMissingScripts(GameObject go, ref int count)
        {
            SerializedObject so = new SerializedObject(go);
            var soProperties = so.FindProperty("m_Component");
            var cs = go.GetComponents<Component>();
            for (int i = cs.Length - 1; i >= 0; --i)
            {
                if (!cs[i])
                {
                    Debug.Log(string.Format("{0:000}.移除游戏对象[{1}] 的无效脚本[{2}]", ++count, CommonFun.GameObjectToString(go), i), go);
                    //DestroyImmediate(cs[i]);
                    soProperties.DeleteArrayElementAtIndex(i);
                }
            }
            so.ApplyModifiedProperties();

            //递归移除子级游戏对象的无效脚本
            foreach (Transform child in go.transform)
            {
                ClearMissingScripts(child.gameObject, ref count);
            }
        }

        #region 工程宏处理

        private static readonly Macro XDREAMER_PROJECT_NORMAL = new Macro(nameof(XDREAMER_PROJECT_NORMAL), DefaultBuildTargetGroups);

        private static readonly Macro XDREAMER_PROJECT_SRP = new Macro(nameof(XDREAMER_PROJECT_SRP), DefaultBuildTargetGroups);

        private static readonly Macro XDREAMER_PROJECT_URP = new Macro(nameof(XDREAMER_PROJECT_URP), DefaultBuildTargetGroups);

        private static readonly Macro XDREAMER_PROJECT_HDRP = new Macro(nameof(XDREAMER_PROJECT_HDRP), DefaultBuildTargetGroups);

        private static void InitPorjectMacro()
        {
            bool isURP = false;
            if (TypeHelper.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData") != null)//URP-Unity URP工程
            {
                isURP = true;
                XDREAMER_PROJECT_URP.DefineIfNoDefined();
            }
            else
            {
                XDREAMER_PROJECT_URP.UndefineWithSelectedBuildTargetGroup();
            }

            bool isHDRP = false;
            if (TypeHelper.GetType("UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData") != null)//HDRP-Unity HDRP工程
            {
                isHDRP = true;
                XDREAMER_PROJECT_HDRP.DefineIfNoDefined();
            }
            else
            {
                XDREAMER_PROJECT_HDRP.UndefineWithSelectedBuildTargetGroup();
            }

            if (isURP || isHDRP || TypeHelper.GetType("UnityEngine.Rendering.CoreUtils") != null)//SRP-Unity SRP工程
            {
                XDREAMER_PROJECT_SRP.DefineIfNoDefined();
                XDREAMER_PROJECT_NORMAL.UndefineWithSelectedBuildTargetGroup();
            }
            else//内置渲染管线-Unity普通工程
            {
                XDREAMER_PROJECT_SRP.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_PROJECT_NORMAL.DefineIfNoDefined();
            }
        }

        #endregion

        #region 编译事件

        /// <summary>
        /// 接口<see cref="IOrderedCallback"/>的实现
        /// </summary>
        public int callbackOrder => 0;

        private DateTime beginTime;

        /// <summary>
        /// 编译开始之前的回调事件，接口<see cref="IPreprocessBuildWithReport"/>的实现
        /// </summary>
        /// <param name="report"></param>
        public void OnPreprocessBuild(BuildReport report)
        {
            beginTime = DateTime.Now;
            Debug.Log("编译开始: " + beginTime.ToDefaultFormat());
            XDreamerInspector.isBuilding = true;
        }

        /// <summary>
        /// 编译完成之后的回调事件，接口<see cref="IPostprocessBuildWithReport"/>的实现
        /// </summary>
        /// <param name="report"></param>
        public void OnPostprocessBuild(BuildReport report)
        {
            var endTime = DateTime.Now;
            Debug.Log("编译结束: " + endTime.ToDefaultFormat());
            Debug.Log("编译总时长: " + (endTime - beginTime).TotalSeconds + " 秒");
            Debug.LogFormat("Unity事件通知的编译结果: {0}, 总耗时 {1} 秒", report.summary.result, report.summary.totalTime.TotalSeconds);
            XDreamerInspector.isBuilding = false;

        }

        #endregion
    }
}

