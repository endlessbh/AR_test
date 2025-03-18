using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.XUnityEditor.PackageManager;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools;
using System;
using System.Linq;
using System.Collections.Generic;
using XCSJ.PluginsCameras;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;
using UnityEditor.Presets;
using XCSJ.PluginTools.Inputs;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit
{
    /// <summary>
    /// 编辑器XRIT辅助类
    /// </summary>
    [LanguageFileOutput]
    public static class EditorXRITHelper
    {
        /// <summary>
        /// 绘制打开XR交互调试器
        /// </summary>
        [LanguageTuple("Open [XR Interactive Debugger]", "打开[XR交互调试器]")]
        public static void DrawOpenXRInteractionDebugger()
        {

#if XDREAMER_XR_INTERACTION_TOOLKIT

            if (GUILayout.Button("Open [XR Interactive Debugger]".Tr(typeof(EditorXRITHelper))))
            {
                EditorApplication.ExecuteMenuItem("Window/Analysis/XR Interaction Debugger");
            }
#endif
        }

        /// <summary>
        /// 绘制选中管理器
        /// </summary>
        [LanguageTuple("Select [{0}] Manager", "选中[{0}]管理器")]
        public static void DrawSelectManager()
        {
            if (GUILayout.Button(string.Format("Select [{0}] Manager".Tr(typeof(EditorXRITHelper)), typeof(XXRInteractionToolkitManager).Tr())) && XXRInteractionToolkitManager.instance)
            {
                Selection.activeObject = XXRInteractionToolkitManager.instance;
            }
        }

        private const string XRManagementName = "XR Plugin Management";

        /// <summary>
        /// 绘制打开XR插件管理器
        /// </summary>
        [LanguageTuple("Open [XR Plugin Management]", "打开[XR插件管理器]")]
        public static void DrawOpenXRPluginManagement()
        {
            if (GUILayout.Button("Open [XR Plugin Management]".Tr(typeof(EditorXRITHelper))))
            {
                EditorHelper.OpenProjectSettingsWindow(XRManagementName);
            }
        }

        /// <summary>
        /// 创建基于XR交互工具包框架的带自定义HMD、模拟控制器、模拟鼠标输入、拖拽工具的XR原点
        /// </summary>
        /// <param name="hmdFunc"></param>
        /// <returns></returns>
        public static (MonoBehaviour origin, CameraController hmd,  Transform leftHand,  Transform rightHand, Transform locomotionSystem) Create(Func<CameraController> hmdFunc = null)
        {
            var origin = Create(out var hmd, out var leftHand, out var rightHand, out var locomotionSystem, hmdFunc);
            if (!origin) return default;

            return (origin, hmd, leftHand, rightHand, locomotionSystem);
        }

        private const string CameraOffset = "相机偏移";
        private const string MainCamera = "主相机";
        private const string LeftController = "左手控制器";
        private const string RightController = "右手控制器";
        private const string LocomotionSystem = "运动系统";

        /// <summary>
        /// 创建基于XR交互工具包框架的带自定义HMD、模拟控制器、模拟鼠标输入、拖拽工具的XR原点
        /// </summary>
        /// <param name="hmd"></param>
        /// <param name="leftHand"></param>
        /// <param name="rightHand"></param>
        /// <param name="locomotionSystem"></param>
        /// <param name="hmdFunc"></param>
        /// <returns></returns>
        public static MonoBehaviour Create(out CameraController hmd, out Transform leftHand, out Transform rightHand, out Transform locomotionSystem, Func<CameraController> hmdFunc = null)
        {
            hmd = default;
            leftHand = default;
            rightHand = default;
            locomotionSystem = default;
            if (hmdFunc == null) hmdFunc = () => CameraHelperExtension.CreateMoveFlyCamera();

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return default;
#else

            //创建Unity默认的XR空间
            var monoBehaviour = CreateUnityDefault(out Transform cameraOffsetTransform, out Transform defaultMainCamera, out leftHand, out rightHand);

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            var origin = monoBehaviour as XROrigin;
#else
            var origin = monoBehaviour as XRRig;
#endif
            if (!origin) return origin;
            origin.transform.XResetLocalPRS();

            //相机偏移
            if (!cameraOffsetTransform) return origin;
            cameraOffsetTransform.XSetName(CameraOffset);

            //销毁Unity默认HMD
            if (defaultMainCamera)
            {
                defaultMainCamera.gameObject.XDestoryObject();
            }

            //销毁默认主相机
            CameraHelperExtension.HandleDefaultMainCamera();

            //创建自定义HMD
            hmd = hmdFunc();
            hmd.transform.XSetTransformParent(cameraOffsetTransform);
            hmd.transform.XResetLocalPRS();

            //将相机应用到XR装备/原点
            var mainCamera = hmd.cameraEntityController.mainCamera;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.XModifyProperty(() => origin.Camera = mainCamera);
#else
            origin.XModifyProperty(() => origin.cameraGameObject = mainCamera.gameObject);   
#endif
            //销毁Unity默认HMD
            if (defaultMainCamera)
            {
                defaultMainCamera.XSetName(MainCamera);
            }

            //左手
            if (leftHand)
            {
                leftHand.XSetName(LeftController);

                //销毁Unity默认XR控制器
                var leftController = leftHand.GetComponent<XRBaseController>();
                leftController.XDestoryObject();

                //创建模拟控制器、模拟鼠标输入
                var analogController = leftHand.XGetOrAddComponent<AnalogController>();
                var analogMouseInput = leftHand.XGetOrAddComponent<AnalogMouseInput>();

                if (analogController.analogMouseInput) { }
            }
            else
            {
                var leftController = cameraOffsetTransform.XCreateChild<AnalogController>(LeftController);
                leftHand = leftController.transform;
            }

            //右手
            if (rightHand)
            {
                rightHand.XSetName(RightController);

                //销毁Unity默认XR控制器
                var rightController = rightHand.GetComponent<XRBaseController>();
                rightController.XDestoryObject();

                //创建模拟控制器、模拟鼠标输入
                var analogController = leftHand.XGetOrAddComponent<AnalogController>();
                var analogMouseInput = leftHand.XGetOrAddComponent<AnalogMouseInput>();

                if (analogController.analogMouseInput) { }
            }
            else
            {
                var rightController = cameraOffsetTransform.XCreateChild<AnalogController>(RightController);
                rightHand = rightController.transform;
            }

            //为XR空间中的左右手创建一键拖拽工具功能
            XCSJ.EditorTools.ToolsMenu.CreateDragToolForXRSpace(origin.gameObject, leftHand, rightHand);

            //创建拥有者
            origin.XAddComponent<XROriginOwner>();

            //创建运动系统
            var sys = CreateLocomotionSystem(origin, origin.transform, out AnalogLocomotionProvider analogLocomotionProvider);
            if (sys)
            {
                sys.XSetName(LocomotionSystem);
                locomotionSystem = sys.transform;
            }
            return origin;
#endif
        }

        /// <summary>
        /// 创建基于XR交互工具包框架的Unity默认HMD、Unity默认XR控制器、模拟鼠标输入、拖拽工具的XR原点
        /// </summary>
        /// <param name="hmd"></param>
        /// <param name="leftHand"></param>
        /// <param name="rightHand"></param>
        /// <param name="locomotionSystem"></param>
        /// <returns></returns>
        public static MonoBehaviour CreateUnity(out Transform hmd, out Transform leftHand, out Transform rightHand, out Transform locomotionSystem)
        {
            hmd = default;
            leftHand = default;
            rightHand = default;
            locomotionSystem = default;

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return default;
#else

            //创建Unity默认的XR原点
            var monoBehaviour = CreateUnityDefault(out Transform cameraOffsetTransform, out Transform defaultMainCamera, out leftHand, out rightHand, true);

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            var origin = monoBehaviour as XROrigin;
#else
            var origin = monoBehaviour as XRRig;
#endif
            if (!origin) return origin;
            origin.transform.XResetLocalPRS();

            //相机偏移
            if (!cameraOffsetTransform) return origin;
            cameraOffsetTransform.XSetName(CameraOffset);

            //左手
            if (leftHand)
            {
                leftHand.XSetName(LeftController);

                //创建模拟鼠标输入
                var analogMouseInput = leftHand.XGetOrAddComponent<XRControllerAnalogMouseInput>();
                if (analogMouseInput.xrController) { }
            }
            else
            {
                var leftController = cameraOffsetTransform.XCreateChild<AnalogController>(LeftController);
                leftHand = leftController.transform;
            }

            //右手
            if (rightHand)
            {
                rightHand.XSetName(RightController);

                //创建模拟鼠标输入
                var analogMouseInput = rightHand.XGetOrAddComponent<XRControllerAnalogMouseInput>();
                if (analogMouseInput.xrController) { }
            }
            else
            {
                var rightController = cameraOffsetTransform.XCreateChild<AnalogController>(RightController);
                rightHand = rightController.transform;
            }

            //为XR空间中的左右手创建一键拖拽工具功能
            XCSJ.EditorTools.ToolsMenu.CreateDragToolForXRSpace(origin.gameObject, leftHand, rightHand);

            //创建拥有者
            origin.XAddComponent<XROriginOwner>();

            //创建运动系统
            var sys = CreateLocomotionSystem(origin, origin.transform, out AnalogLocomotionProvider analogLocomotionProvider);
            if (sys)
            {
                sys.XSetName(LocomotionSystem);
                locomotionSystem = sys.transform;
            }
            return origin;
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 创建运动系统
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="parent"></param>
        /// <param name="analogLocomotionProvider"></param>
        /// <returns></returns>
        private static LocomotionSystem CreateLocomotionSystem(MonoBehaviour origin, Transform parent, out AnalogLocomotionProvider analogLocomotionProvider)
        {
            var go = CommonFun.CreateGameObjectWithUniqueName(parent ? parent.gameObject : null, "Locomotion System");
            var system = go.XAddComponent<LocomotionSystem>();
            if (origin)
            {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
                system.XModifyProperty(() => system.xrOrigin = origin as XROrigin);
#else
                system.XModifyProperty(() => system.xrRig = origin as XRRig);
#endif
            }

            //添加模拟运动提供者
            {
                var provider = go.XAddComponent<AnalogLocomotionProvider>();
                provider.XModifyProperty(() => provider.system = system);
                analogLocomotionProvider = provider;
            }

            //添加传送提供者
            {
                var provider = go.XAddComponent<TeleportationProvider>();
                provider.XModifyProperty(() => provider.system = system);
            }

            // 添加移动和旋转
            {
                TryAddAndSetPreset<ActionBasedContinuousMoveProvider>(go);
                TryAddAndSetPreset<ActionBasedContinuousTurnProvider>(go);
            }

            return system;
        }

        /// <summary>
        /// 添加组件并设置与组件同类型的预设资源
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool TryAddAndSetPreset<TComponent>(GameObject gameObject) where TComponent : Component
        {
            return SetPresetTo(gameObject.XGetOrAddComponent<TComponent>());
        }

        /// <summary>
        /// 获取组件并设置与组件同类型的预设资源
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool SetPresetTo<TComponent>(GameObject gameObject) where TComponent : Component
        {
            return SetPresetTo<TComponent>(gameObject.GetComponent<TComponent>());
        }

        /// <summary>
        /// 设置与组件同类型的预设资源
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <param name="provider"></param>
        /// <param name="predicate">过滤条件</param>
        /// <returns></returns>
        public static bool SetPresetTo<TComponent>(TComponent provider, Func<Preset, bool> predicate = null) where TComponent : Component
        {
            return provider ? SetPresetTo(provider, EditorHelper.FindAllPresetsOfType(provider), predicate) : false;
        }

        private static bool SetPresetTo<TComponent>(TComponent provider, IEnumerable<Preset> presets, Func<Preset, bool> predicate = null) where TComponent : Component
        {
            if (provider)
            {
                var turnPreset = predicate == null ? presets.FirstOrDefault() : presets.FirstOrDefault(predicate);
                if (turnPreset)
                {
                    turnPreset.ApplyTo(provider);
                    return true;
                }
            }
            return false;
        }

#endif

        /// <summary>
        /// 创建Unity默认的XR原点
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="cameraOffset"></param>
        /// <param name="mainCamera"></param>
        /// <param name="leftHand"></param>
        /// <param name="rightHand"></param>
        /// <param name="addInputActioAsset">添加输入动作资源</param>
        /// <returns></returns>
        public static MonoBehaviour CreateUnityDefault(out Transform cameraOffset, out Transform mainCamera, out Transform leftHand, out Transform rightHand, bool addInputActioAsset = false)
        {
            cameraOffset = default;
            mainCamera = default;
            leftHand = default;
            rightHand = default;

#if !XDREAMER_XR_INTERACTION_TOOLKIT
            return default;
#else

            var origin = CreateInternal();
            if (!origin) return default;

            cameraOffset = origin.transform.Find("Camera Offset");
            if (!cameraOffset) return origin;

            mainCamera = cameraOffset.Find("Main Camera");
            leftHand = cameraOffset.Find("LeftHand Controller");
            rightHand = cameraOffset.Find("RightHand Controller");

            if (addInputActioAsset)
            {
                SetPresetTo(leftHand.GetComponent<XRBaseController>(), p => p.name.Contains("Left"));
                SetPresetTo(rightHand.GetComponent<XRBaseController>(), p => p.name.Contains("Right"));
                SetInputActionManagerPreset(origin.XGetOrAddComponent<InputActionManager>(), out _);
            }

            return origin;
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT

        /// <summary>
        /// 设置[输入动作管理器]预设动作
        /// </summary>
        /// <param name="inputActionManager"></param>
        /// <param name="hasInputActionAsset">存在资源</param>
        /// <returns></returns>
        public static bool SetInputActionManagerPreset(InputActionManager inputActionManager, out bool hasInputActionAsset)
        {
            hasInputActionAsset = false;
#if XDREAMER_INPUT_SYSTEM
            if (inputActionManager)
            {
                var inputActionAsset = EditorHelper.FindAllAssetsOfType<InputActionAsset>().FirstOrDefault(a => a.name.Contains("XRI Default"));

                hasInputActionAsset = inputActionAsset;
                if (hasInputActionAsset)
                {
                    var actionAssets = inputActionManager.actionAssets ?? new List<InputActionAsset>();
                    if (!actionAssets.Contains(inputActionAsset))
                    {
                        actionAssets.Add(inputActionAsset);
                        inputActionManager.actionAssets = actionAssets;
                        return true;
                    }
                }
            }
#endif
            return false;
        }

#endif
        private static T Create<T>() where T : MonoBehaviour => EditorHelper.ExecuteMenuItem<T>(GetMenuItemPath());

        private static string GetMenuItemPath()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_1_1_OR_NEWER
            return "GameObject/XR/XR Origin (VR)";
#elif XDREAMER_XR_INTERACTION_TOOLKIT_2_0_4_OR_NEWER
            return "GameObject/XR/XR Origin (Action-based)";
#elif XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            return "GameObject/XR/XR Origin";
#else
            return "GameObject/XR/XR Rig";
#endif
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 创建XR原点
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        private static XROrigin CreateInternal() => Create<XROrigin>();

#else

        /// <summary>
        /// 创建XR装备
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        private static XRRig CreateInternal() => Create<XRRig>();
        
#endif
#endif

    }
}
