using UnityEditor;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.EditorCommonUtils;
using UnityEngine;
using XCSJ.EditorCameras;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using System.Linq;
using XCSJ.PluginsCameras.Base;
using XCSJ.Languages;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// XR原点拥有者检查器
    /// </summary>
    [CustomEditor(typeof(XROriginOwner))]
    [Name("XR原点拥有者检查器")]
    public class XROriginOwnerInspector : MBInspector<XROriginOwner>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawSetPreset();

            EditorCameraHelperExtension.DrawSelectCameraManager();
            EditorXRITHelper.DrawSelectManager();
            EditorXRITHelper.DrawOpenXRInteractionDebugger();

            DrawHMDDetailInfos();
            DrawXRControllerDetailInfos();
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT
        private bool setLeftHandResult = true;
        private bool setRightHandResult = true;
        private bool hasInputActionAsset = true;
#endif

        [LanguageTuple("Set [LeftHandController] Preset", "设置[左手控制器]预设动作")]
        [LanguageTuple("Set [RightHandController] Preset", "设置[右手控制器]预设动作")]
        [LanguageTuple("Set [InputActionManager] Preset", "设置[输入动作管理器]预设动作")]
        [LanguageTuple("No preset action resources were found in the project for the [LeftHandController] type, Standard assets can be imported from the [XR Interaction Toolkit]", "项目中未找到适用于[左手控制器]类型的预设动作资源,可从[XR Interaction Toolkit]中导入标准资产")]
        [LanguageTuple("No preset action resources were found in the project for the [RightHandController] type, Standard assets can be imported from the [XR Interaction Toolkit]", "项目中未找到适用于[右手控制器]类型的预设动作资源,可从[XR Interaction Toolkit]中导入标准资产")]
        [LanguageTuple("No preset action resources were found in the project for the [InputActionManager] type, Standard assets can be imported from the [XR Interaction Toolkit]", "项目中未找到适用于[输入动作管理器]类型的预设动作资源,可从[XR Interaction Toolkit]中导入标准资产")]
        private void DrawSetPreset()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

            // 设置[左手控制器]预设动作
            {
                var leftController = targetObject.leftController.GetComponent<XRBaseController>();
                EditorGUI.BeginDisabledGroup(!leftController);
                {
                    if (GUILayout.Button("Set [LeftHandController] Preset".Tr(typeof(XROriginOwnerInspector))))
                    {
                        setLeftHandResult = EditorXRITHelper.SetPresetTo(leftController, p => p.name.Contains("Left"));
                        if (setLeftHandResult)
                        {
                            Selection.activeObject = leftController;
                        }
                    }
                    if (leftController && !setLeftHandResult)
                    {
                        UICommonFun.RichHelpBox(@"<size=12>" + "No preset action resources were found in the project for the [LeftHandController] type, Standard assets can be imported from the [XR Interaction Toolkit]".Tr(typeof(XROriginOwnerInspector)) + "</size>", MessageType.Warning);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }

            // 设置[右手控制器]预设动作
            {
                var rightController = targetObject.rightController.GetComponent<XRBaseController>();
                EditorGUI.BeginDisabledGroup(!rightController);
                {
                    if (GUILayout.Button("Set [RightHandController] Preset".Tr(typeof(XROriginOwnerInspector))))
                    {
                        setRightHandResult = EditorXRITHelper.SetPresetTo(rightController, p => p.name.Contains("Right"));
                        if (setRightHandResult)
                        {
                            Selection.activeObject = rightController;
                        }
                    }
                    if (rightController && !setRightHandResult)
                    {
                        UICommonFun.RichHelpBox(@"<size=12>" + "No preset action resources were found in the project for the [RightHandController] type, Standard assets can be imported from the [XR Interaction Toolkit]".Tr(typeof(XROriginOwnerInspector)) + "</size>", MessageType.Warning);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }

            // 设置[输入动作管理]预设动作
            {
                var inputActionManager = targetObject.GetComponent<InputActionManager>();
                EditorGUI.BeginDisabledGroup(!inputActionManager);
                {
                    if (GUILayout.Button("Set [InputActionManager] Preset".Tr(typeof(XROriginOwnerInspector))))
                    {
                        var rs = EditorXRITHelper.SetInputActionManagerPreset(inputActionManager, out hasInputActionAsset);
                        if (rs) 
                        {
                            Selection.activeObject = inputActionManager;
                        }
                    }
                    if (inputActionManager && !hasInputActionAsset)
                    {
                        UICommonFun.RichHelpBox(@"<size=12>" + "No preset action resources were found in the project for the [InputActionManager] type, Standard assets can be imported from the [XR Interaction Toolkit]".Tr(typeof(XROriginOwnerInspector)) + "</size>", MessageType.Warning);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
#endif
        }

        /// <summary>
        /// HMD列表
        /// </summary>
        [Name("HMD列表")]
        [Tip("当前XR装备的所有HMD的相机控制器组件对象或相机组件对象", "Camera controller component object or camera component object of all HMDS currently equipped with XR")]
        private static bool _displayHMDs = true;

        private void DrawHMDDetailInfos()
        {
            _displayHMDs = UICommonFun.Foldout(_displayHMDs, CommonFun.NameTip(GetType(), nameof(_displayHMDs)));
            if (!_displayHMDs) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("HMD对象", "HMD型组件对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "HMD型组件对象是否激活并启用；本项只读；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var hasCameraController = false;
            var owner = targetObject;

            //相机控制器
            {
                var cache = ComponentCache.Get(typeof(BaseCameraMainController), true);
                for (int i = 0; i < cache.components.Length; i++)
                {
                    var component = cache.components[i] as BaseCameraMainController;
                    if (!component.GetComponentsInParent<XROriginOwner>().Any(o => o == owner)) continue;
                    hasCameraController = true;

                    UICommonFun.BeginHorizontal(i);

                    //编号
                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                    //相机控制器
                    EditorGUILayout.ObjectField(component, component.GetType(), true);

                    //激活启用
                    EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                    UICommonFun.EndHorizontal();
                }
            }

            if (!hasCameraController)//相机
            {
                var cache = ComponentCache.Get(typeof(Camera), true);
                for (int i = 0; i < cache.components.Length; i++)
                {
                    var component = cache.components[i] as Camera;
                    if (!component.GetComponentsInParent<XROriginOwner>().Any(o => o == owner)) continue;

                    UICommonFun.BeginHorizontal(i);

                    //编号
                    EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                    //相机
                    EditorGUILayout.ObjectField(component, component.GetType(), true);

                    //激活启用
                    EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                    UICommonFun.EndHorizontal();
                }
            }

            CommonFun.EndLayout();
        }

        /// <summary>
        /// XR控制器列表
        /// </summary>
        [Name("XR控制器列表")]
        [Tip("当前XR装备的所有XR控制器对象", "XR of all current equipped controllers")]
        private static bool _displayXRRigs = true;

        private void DrawXRControllerDetailInfos()
        {
            _displayXRRigs = UICommonFun.Foldout(_displayXRRigs, CommonFun.NameTip(GetType(), nameof(_displayXRRigs)));
            if (!_displayXRRigs) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("XR控制器", "XR控制器组件对象；本项只读；"));
            GUILayout.Label(CommonFun.TempContent("激活启用", "XR控制器组件对象是否激活并启用；本项只读；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

#if XDREAMER_XR_INTERACTION_TOOLKIT

            var owner = targetObject;
            var cache = ComponentCache.Get(typeof(XRBaseController), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as XRBaseController;
                if (!component.GetComponentsInParent<XROriginOwner>().Any(o => o == owner)) continue;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //XR装备对象
                EditorGUILayout.ObjectField(component, component.GetType(), true);

                //激活启用
                EditorGUILayout.Toggle(component.isActiveAndEnabled, UICommonOption.Width60);

                UICommonFun.EndHorizontal();
            }
#endif

            CommonFun.EndLayout();
        }
    }
}
