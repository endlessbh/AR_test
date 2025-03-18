using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorXRSpaceSolution;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView
{
    /// <summary>
    /// 编辑器立体显示组手
    /// </summary>
    [LanguageFileOutput]
    public class EditorStereoViewHelper
    {
        /// <summary>
        /// 绘制选中管理器
        /// </summary>
        [LanguageTuple("Select [{0}] Manager", "选中[{0}]管理器")]
        public static void DrawSelectManager()
        {
            if (GUILayout.Button(string.Format("Select [{0}] Manager".Tr(typeof(EditorStereoViewHelper)), typeof(StereoViewManager).Tr())) && StereoViewManager.instance)
            {
                Selection.activeObject = StereoViewManager.instance;
            }
        }

        static string warnLog = "";

        /// <summary>
        /// 绘制设置主动立体配置
        /// </summary>
        [LanguageTuple("One Click Setting Active Stereo Configuration", "一键设置主动立体配置")]
        public static void DrawSettingActiveStereoConfig()
        {
            if (GUILayout.Button("One Click Setting Active Stereo Configuration".Tr(typeof(EditorStereoViewHelper))))
            {
                SetActiveStereoConfig();
            }
            if (!string.IsNullOrEmpty(warnLog))
            {
                EditorGUILayout.HelpBox(warnLog, MessageType.Warning);
            }
            EditorXRITHelper.DrawOpenXRPluginManagement();
        }

#if UNITY_2020_1_OR_NEWER
        
        private const string XRManagementPackageId = "com.unity.xr.management";

#else
        /// <summary>
        /// 对应界面显示"Stereo Display (non head-mounted)"
        /// </summary>
        private const string VRSDK_Key = "stereo";
#endif

        [LanguageTuple("Active stereo can only be used on [{0}] platform!", "主动立体仅可在[{0}]平台使用!")]
        private static void SetActiveStereoConfig()
        {
            warnLog = "";

            if (EditorUserBuildSettings.selectedBuildTargetGroup != BuildTargetGroup.Standalone)
            {
                warnLog = string.Format("Active stereo can only be used on [{0}] platform!".Tr(typeof(EditorStereoViewHelper)), BuildTargetGroup.Standalone);
                return;
            }

            //独占全屏模式
            PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

            //强制单例
            PlayerSettings.forceSingleInstance = true;

#if UNITY_2020_1_OR_NEWER //Unity 2020 （含）以后的版本设置

            //直接安装XR插件管理器
            Client.Add(XRManagementPackageId);

#else //Unity 2019 （含）以前的版本设置

            //多通道立体绘制模式
            PlayerSettings.stereoRenderingPath = StereoRenderingPath.MultiPass;

#pragma warning disable CS0618 // 类型或成员已过时

            //设置VR支持且启用VR-SDK
            PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Standalone, true);
            {
                var sdks = PlayerSettings.GetVirtualRealitySDKs(BuildTargetGroup.Standalone);
                if (sdks.IndexOf(VRSDK_Key) < 0)
                {
                    var list = sdks.ToList();
                    list.Insert(0, VRSDK_Key);
                    PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Standalone, list.ToArray());
                }
            }
#pragma warning restore CS0618 // 类型或成员已过时

#endif

            //{//设置首选图形API为OpenGLCore
            //    var apis64 = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64);
            //    if (apis64.IndexOf(GraphicsDeviceType.OpenGLCore) < 0)
            //    {
            //        var list = apis64.ToList();
            //        list.Insert(0, GraphicsDeviceType.OpenGLCore);
            //        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, list.ToArray());
            //    }
            //}

            //{//设置首选图形API为OpenGLCore
            //    var apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
            //    if (apis.IndexOf(GraphicsDeviceType.OpenGLCore) < 0)
            //    {
            //        var list = apis.ToList();
            //        list.Insert(0, GraphicsDeviceType.OpenGLCore);
            //        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows, list.ToArray());
            //    }
            //}

            //{//设置相机属性
            //    foreach (var camera in ComponentCache.GetComponents<Camera>())
            //    {
            //        camera.SetCameraPropertyForActiveStereo();
            //    }
            //}
        }
    }
}
