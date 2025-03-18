using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginsCameras.Controllers;
using System;
using XCSJ.PluginsCameras;
using XCSJ.Languages;
using XCSJ.EditorExtension.Base;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using XCSJ.PluginVuforia;
#endif

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        #region 空间UI

        /// <summary>
        /// XR空间画布：创建可用于XR空间交互使用的XR空间画布组件对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("XR空间画布")]
        [Tip("创建可用于XR空间交互使用的XR空间画布组件对象", "Create an XR space canvas component object that can be used for XR space interaction")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(XXRInteractionToolkitManager))]
        public static void CreateCanvas(ToolContext toolContext)
        {
#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogError(XRITHelper.Title + "环境缺失！无法创建XR空间画布组件对象！");
#else
            EditorToolsHelperExtension.PopupAddComponentMenu(e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EComponentToolsMenu.AddComponentOnCurrentSelectedGameObject:
                        {
                            var gameObject = Selection.activeGameObject;
                            if (gameObject.GetComponent<Canvas>())
                            {
                                gameObject.XGetOrAddComponent<TrackedDeviceGraphicRaycaster>();
                            }
                            break;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectSibling:
                        {
                            var transform = Selection.activeGameObject.transform;
                            var canvas = CreateXRCanvas(transform.parent);
                            if (canvas)
                            {
                                canvas.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                            }
                            break;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectChildren:
                        {
                            CreateXRCanvas(Selection.activeGameObject.transform);
                            break;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsManagerChildren:
                        {
                            CreateXRCanvas();
                            break;
                        }
                }
            }, e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EComponentToolsMenu.AddComponentOnCurrentSelectedGameObject:
                        {
                            var gameObject = Selection.activeGameObject;
                            return gameObject && gameObject.GetComponent<Canvas>() && !gameObject.GetComponent<TrackedDeviceGraphicRaycaster>();
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectSibling:
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectChildren:
                        {
                            return Selection.activeGameObject;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsManagerChildren:
                        {
                            return true;
                        }
                }
                return true;
            });
#endif
        }

        private static Canvas CreateXRCanvas(Transform parent = null)
        {
            var canvas = UnityObjectExtension.CreateCanvas("XR空间画布", parent);
            canvas.renderMode = RenderMode.WorldSpace;
#if XDREAMER_XR_INTERACTION_TOOLKIT
            canvas.XGetOrAddComponent<TrackedDeviceGraphicRaycaster>();
#endif
            return canvas;
        }

        #endregion

        #region 可交互对象

        /// <summary>
        /// XR抓取可交互:创建可用于XR空间交互使用的XR抓取可交互组件对象
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("XR抓取可交互")]
        [Tip("创建可用于XR空间交互使用的XR抓取可交互组件对象", "Create XR grab interactive component objects that can be used for XR spatial interaction")]
        [XCSJ.Attributes.Icon(EIcon.Put)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(XXRInteractionToolkitManager))]
        public static void CreateGrabInteractable(ToolContext toolContext)
        {
#if !XDREAMER_XR_INTERACTION_TOOLKIT
            Debug.LogError(XRITHelper.Title + "环境缺失！无法创建XR抓取可交互组件对象！");
#else
            EditorToolsHelperExtension.PopupAddComponentMenu(e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EComponentToolsMenu.AddComponentOnCurrentSelectedGameObject:
                        {
                            var gameObject = Selection.activeGameObject;
                            if (gameObject && gameObject.GetComponent<Rigidbody>())
                            {
                                gameObject.XGetOrAddComponent<XRGrabInteractable>();
                            }
                            break;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectSibling:
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectChildren:
                        {
                            break;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsManagerChildren:
                        {
                            UnityObjectExtension.CreateGameObject<XRGrabInteractable>("XR抓取可交互");
                            break;
                        }
                }
            }, e =>
            {
                switch (e)
                {
                    case EditorToolsHelperExtension.EComponentToolsMenu.AddComponentOnCurrentSelectedGameObject:
                        {
                            var gameObject = Selection.activeGameObject;
                            return gameObject && gameObject.GetComponent<Rigidbody>() && !gameObject.GetComponent<XRGrabInteractable>();
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectSibling:
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsCurrentSelectedGameObjectChildren:
                        {
                            return false;
                        }
                    case EditorToolsHelperExtension.EComponentToolsMenu.CreateGameObjectAsManagerChildren:
                        {
                            return true;
                        }
                }
                return true;
            });
#endif
        }

        #endregion

        #region 运动系统-传送

        /// <summary>
        /// 创建传送锚点
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("传送锚点")]
        [Tip("创建一个直径1米的圆形传送锚点", "Create a circular transmission anchor with a diameter of 1 meter")]
        [XCSJ.Attributes.Icon(EIcon.Teleport)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(XXRInteractionToolkitManager))]
        public static void CreateTeleportationAnchor(ToolContext toolContext)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

            //创建传送层
            var gameObject = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(XRITHelper.CategoryName + "/传送锚点.prefab");
            gameObject.XSetUniqueName("传送锚点");

#else
            Debug.LogFormat("当前工程缺失插件[{0}]所需的包或是版本不匹配！", XRITHelper.Title);
#endif
        }

        /// <summary>
        /// 创建传送区域
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.Interact, nameof(XXRInteractionToolkitManager))]
        [Name("传送区域")]
        [Tip("创建一个10x10米的方形传送区域", "Create a 10x10 meter square transfer area")]
        [XCSJ.Attributes.Icon(EIcon.Teleport)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(XXRInteractionToolkitManager))]
        public static void CreateTeleportationArea(ToolContext toolContext)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

            //创建传送层            
            var gameObject = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(XRITHelper.CategoryName + "/传送区域.prefab");
            gameObject.XSetUniqueName("传送区域");

#else
            Debug.LogFormat("当前工程缺失插件[{0}]所需的包或是版本不匹配！", XRITHelper.Title);
#endif
        }

        /// <summary>
        /// 传送提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("传送提供者")]
        [Tip("在当前游戏对象上创建一个[传送提供者]组件对象", "Create a [TeleportationProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Teleport)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(XXRInteractionToolkitManager))]
        public static bool CreateTeleportationProvider(ToolContext toolContext) => CreateLocomotionProvider<TeleportationProvider>(toolContext);

        #endregion

        #region 运动系统

        /// <summary>
        /// 基于动作的连续移动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于动作的连续移动提供者")]
        [Tip("在当前游戏对象上创建一[个基于动作的连续移动提供者]组件对象", "Create an [ActionBasedContinuousMoveProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Move)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(ActionBasedContinuousMoveProvider))]
        public static bool CreateActionBasedContinuousMoveProvider(ToolContext toolContext) => CreateLocomotionProvider<ActionBasedContinuousMoveProvider>(toolContext);

        /// <summary>
        /// 基于设备的连续移动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于设备的连续移动提供者")]
        [Tip("在当前游戏对象上创建一个[基于设备的连续移动提供者]组件对象", "Create a [DeviceBasedContinuousMoveProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Move)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(DeviceBasedContinuousMoveProvider))]
        public static bool CreateDeviceBasedContinuousMoveProvider(ToolContext toolContext) => CreateLocomotionProvider<DeviceBasedContinuousMoveProvider>(toolContext);

        /// <summary>
        /// 基于动作的连续转动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于动作的连续转动提供者")]
        [Tip("在当前游戏对象上创建一个[基于动作的连续转动提供者]组件对象", "Create a [ActionBasedContinuousTurnProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(ActionBasedContinuousTurnProvider))]
        public static bool CreateActionBasedContinuousTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<ActionBasedContinuousTurnProvider>(toolContext);

        /// <summary>
        /// 基于设备的连续转动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于设备的连续转动提供者")]
        [Tip("在当前游戏对象上创建一个[基于设备的连续转动提供者]组件对象", "Create a [DeviceBasedContinuousTurnProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(DeviceBasedContinuousTurnProvider))]
        public static bool CreateDeviceBasedContinuousTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<DeviceBasedContinuousTurnProvider>(toolContext);

        /// <summary>
        /// 基于动作的快速转动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于动作的快速转动提供者")]
        [Tip("在当前游戏对象上创建一个[基于动作的快速转动提供者]组件对象", "Create a [ActionBasedSnapTurnProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(ActionBasedSnapTurnProvider))]
        public static bool CreateActionBasedSnapTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<ActionBasedSnapTurnProvider>(toolContext);

        /// <summary>
        /// 基于设备的捕捉转动提供者
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(XRITHelper.LocomotionSystem, nameof(AnalogLocomotionProvider))]
        [Name("基于设备的捕捉转动提供者")]
        [Tip("在当前游戏对象上创建一个[基于设备的捕捉转动提供者]组件对象", "Create a [DeviceBasedSnapTurnProvider] component object on the current GameObject")]
        [XCSJ.Attributes.Icon(EIcon.Rotate)]
        [RequireManager(typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(DeviceBasedSnapTurnProvider))]
        public static bool CreateDeviceBasedSnapTurnProvider(ToolContext toolContext) => CreateLocomotionProvider<DeviceBasedSnapTurnProvider>(toolContext);

        private static bool CreateLocomotionProvider<T>(ToolContext toolContext) where T : LocomotionProvider
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            var go = Selection.activeGameObject;
            if (!go) return false;

            var provider = go.GetComponent<T>();
            if (provider) return false;

            var system = go.GetComponentInParent<LocomotionSystem>();
            if (!system) return false;

            if (toolContext.toolFuncType == EToolFuncType.OnClick)
            {
                provider = go.XAddComponent<T>();
                provider.XModifyProperty(() => provider.system = system);
            }
            return true;
#else
            return false;
#endif
        }

        #endregion


#if !XDREAMER_XR_INTERACTION_TOOLKIT //不存在XR交互工具时的定义类

        #region 运动系统

        /// <summary>
        /// 运动提供者
        /// </summary>
        public class LocomotionProvider { }

        /// <summary>
        /// 基于设备的捕捉转动提供者
        /// </summary>
        public class DeviceBasedSnapTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于设备的连续转动提供者
        /// </summary>
        public class DeviceBasedContinuousTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于设备的连续移动提供者
        /// </summary>
        public class DeviceBasedContinuousMoveProvider : LocomotionProvider { }

        /// <summary>
        /// 基于动作的连续转动提供者
        /// </summary>
        public class ActionBasedContinuousTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于动作的快速转动提供者
        /// </summary>
        public class ActionBasedSnapTurnProvider : LocomotionProvider { }

        /// <summary>
        /// 基于动作的连续移动提供者
        /// </summary>
        public class ActionBasedContinuousMoveProvider : LocomotionProvider { }

        /// <summary>
        /// 传送提供者
        /// </summary>
        public class TeleportationProvider : LocomotionProvider { }

        #endregion

#endif
    }
}
