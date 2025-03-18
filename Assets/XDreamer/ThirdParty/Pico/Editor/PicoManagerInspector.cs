using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorTools;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginPeripheralDevice;
using XCSJ.PluginPico;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorPico
{
    /// <summary>
    /// PICO管理器检查器
    /// </summary>
    [CustomEditor(typeof(PicoManager))]
    public class PicoManagerInspector : BaseManagerInspector<PicoManager>
    {
        #region 编译宏

        /// <summary>
        /// 宏
        /// </summary>
        public static readonly Macro XDREAMER_PICO = new Macro(nameof(XDREAMER_PICO), BuildTargetGroup.Android);

        /// <summary>
        /// 初始化宏
        /// </summary>
        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_ANDROID
            if (TypeHelper.Exists("Unity.XR.PXR.PXR_Manager")
                && UICommonFun.NaturalCompare(PicoHelper.VersionWeak, "2.1.2") >= 0)
            {
                XDREAMER_PICO.DefineIfNoDefined();
            }
            else
#endif
            {
                XDREAMER_PICO.UndefineWithSelectedBuildTargetGroup();
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            //编辑器运行时不处理
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            InitMacro();

            XDreamerInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(PicoManager))
                {
                    EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_PICO, PackageName, true);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (PicoManager.instance)
                    {
                        EditorHelper.OpenPackageManagerIfNeedWithDialog(XDREAMER_PICO, PackageName, false);
                    }
                });
            };
        }

        /// <summary>
        /// 需要的所有依赖包;需要调用包管理器
        /// </summary>
        private const string PackageName = "com.unity.xr.picoxr";

        #endregion

        private static CategoryList categoryList = null;

        bool allStarted = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(PicoManager));

            if (manager && manager.hasAccess)
            {
                allStarted = XDreamer.StartManager(typeof(PeripheralDeviceInputManager), typeof(XXRInteractionToolkitManager));
            }
            else
            {
                allStarted = false;
            }
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("One Click Setting PICO Configuration", "一键设置PICO配置")]
        [LanguageTuple("Open [XR Plugin Management]", "打开[XR插件管理器]")]
        [LanguageTuple("Please install (or update) to Unity's [{0}] package [2.1.2] (included) or later!", "请安装(或更新)到Unity的[{0}]包[2.1.2](含)或更高版本！")]
        [LanguageTuple("Enable Required Plug-ins", "一键启用所需插件")]
        public override void OnInspectorGUI()
        {

#if !XDREAMER_PICO

            EditorHelper.OpenPackageManagerIfNeedWithButton(XDREAMER_PICO, PackageName);

            UICommonFun.RichHelpBox("<color=red>" + string.Format(Tr("Please install (or update) to Unity's [{0}] package [2.1.2] (included) or later!"), PicoHelper.Title) + "</color>", MessageType.Warning);
#endif

            base.OnInspectorGUI();

            if (!allStarted && manager.hasAccess)
            {
                if (GUILayout.Button(Tr("Enable Required Plug-ins")))
                {
                    XDreamer.StartManager(typeof(PeripheralDeviceInputManager), typeof(XXRInteractionToolkitManager));
                }
            }

#if XDREAMER_PICO

            if (GUILayout.Button(Tr("One Click Setting PICO Configuration")))
            {
                if(PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel26)
                {
                    PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
                }
                if(PlayerSettings.Android.targetSdkVersion != AndroidSdkVersions.AndroidApiLevelAuto)
                {
                    PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
                }
                PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            }

#endif
            EditorXRITHelper.DrawOpenXRPluginManagement();

            categoryList.DrawVertical();
        }
    }
}

