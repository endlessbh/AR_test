using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XCSJ.Attributes;

#if XDREAMER_STEAMVR
using Valve.VR;
#endif

using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginHTCVive;

namespace XCSJ.EditorHTCVive
{
    /// <summary>
    /// HTC Vive检查器
    /// </summary>
    [Name("HTC Vive检查器")]
    [CustomEditor(typeof(HTCViveManager))]
    public class HTCViveManagerInspector : BaseManagerInspector<HTCViveManager>
    {
        #region 编译宏

        public static readonly Macro XDREAMER_STEAMVR = new Macro(nameof(XDREAMER_STEAMVR), BuildTargetGroup.Standalone);
        public static readonly Macro XDREAMER_STEAMVR_INPUT = new Macro(nameof(XDREAMER_STEAMVR_INPUT), BuildTargetGroup.Standalone);

        [Macro]
        public static void InitMacro()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

#if UNITY_EDITOR || UNITY_STANDALONE 
            var type = TypeHelper.GetType("Valve.VR.SteamVR_Action");
            if (type != null && FileHelper.Exists(Application.dataPath + @"/SteamVR/Input/SteamVR_Action.cs"))
            {
                try
                {
                    XDREAMER_STEAMVR.DefineIfNoDefined();

                    if (TypeHelper.GetType("Valve.VR.SteamVR_Actions") != null)
                    {
                        XDREAMER_STEAMVR_INPUT.DefineIfNoDefined();
                    }
                }
                catch
                {
                    XDREAMER_STEAMVR.UndefineWithSelectedBuildTargetGroup();
                    XDREAMER_STEAMVR_INPUT.UndefineWithSelectedBuildTargetGroup();
                }
            }
            else
#endif
            {
                XDREAMER_STEAMVR.UndefineWithSelectedBuildTargetGroup();
                XDREAMER_STEAMVR_INPUT.UndefineWithSelectedBuildTargetGroup();
            }
        }

        #endregion

        public const string UnityPackagePath = "Assets/XDreamer-ThirdPartyUnityPackage/SteamVR Plugin.unitypackage";

        [InitializeOnLoadMethod]
        public static void Init()
        {
            //编辑器运行时不处理
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            InitMacro();

            XDreamerInspector.onCreatedManager += (t) =>
            {
                if (t == typeof(HTCViveManager))
                {
                    EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_STEAMVR, UnityPackagePath, true);
                }
            };

            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                UICommonFun.DelayCall(() =>
                {
                    if (HTCViveManager.instance)
                    {
                        EditorHelper.ImportPackageIfNeedWithDialog(XDREAMER_STEAMVR, UnityPackagePath, false);
                    }
                });
            };
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("No valid StreamVR input system was found. Please confirm to execute the menu [Window/StreamVR Input] command and agree to use the sample file!", "未找到有效的StreamVR输入系统，请确认执行菜单[Window/StreamVR Input]命令并同意使用示例文件！")]
        public override void OnInspectorGUI()
        {
            #region 检测是否需要导入UnityPackage

            EditorHelper.ImportPackageIfNeedWithButton(XDREAMER_STEAMVR, UnityPackagePath);

            #endregion

#if XDREAMER_STEAMVR && !XDREAMER_STEAMVR_INPUT
            UICommonFun.RichHelpBox(Tr("No valid StreamVR input system was found. Please confirm to execute the menu [Window/StreamVR Input] command and agree to use the sample file!"), MessageType.Error);
#endif
            base.OnInspectorGUI();
        }
    }
}
