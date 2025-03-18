using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorXRSpaceSolution;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Extension.CNScripts.UGUI;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// HMD-ART型飞行相机
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ARTHelper.Title, nameof(ARTManager), rootType = typeof(ARTManager), groupRule = EToolGroupRule.None)]
        [Name("HMD-ART型飞行相机")]
        [XCSJ.Attributes.Icon(EIcon.FlyCamera)]
        [RequireManager(typeof(ARTManager), typeof(StereoViewManager))]
        [Manual(typeof(ARTManager))]
        public static void CreateARTCamera(ToolContext toolContext)
        {
            var cameraController = CreateARTCamera();
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        private static CameraController CreateARTCamera() => XRSpaceSolutionHelper.CreateHMDFlyCamera<CameraTransformByART>("HMD-ART型平移飞行相机");

        /// <summary>
        /// ART型XR原点:创建基于ART跟踪定位的XR装备/原点
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ARTHelper.Title, nameof(ARTManager))]
        [Name("ART型XR原点")]
        [Tip("创建基于ART跟踪定位的XR装备/原点", "Create XR equipment / origin based on art tracking and positioning")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(ARTManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager))]
        [Manual(typeof(ARTManager))]
        public static void Create(ToolContext toolContext) => Create(out _, out _, out _);

        private static bool CheckPackage()
        {
            return true;
        }

        /// <summary>
        /// 创建基于ART跟踪定位的XR装备
        /// </summary>
        /// <returns></returns>
        private static MonoBehaviour Create(out CameraController hmd, out Transform leftHand, out Transform rightHand)
        {
            hmd = default;
            leftHand = default;
            rightHand = default;

            if (!CheckPackage()) return default;

            var origin = EditorXRITHelper.Create(out hmd, out leftHand, out rightHand, out var locamotionSystem, CreateARTCamera);
            if (!origin) return origin;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.gameObject.XSetUniqueName("XR原点 - ART");
#else
            origin.gameObject.XSetUniqueName("XR装备 - ART");
#endif
            if (hmd)
            {
                hmd.cameraTransformer.XGetOrAddComponent<CameraTransformByART>();
            }

            if (leftHand)
            {
                var pose = leftHand.XAddComponent<PoseIOByART>();
                if (pose)
                {
                    pose.dataType = EDataType.FlyStick;
                    pose.rigidBodyID = 0;
                }
            }

            if (rightHand)
            {
                var pose = rightHand.XAddComponent<PoseIOByART>();
                if (pose)
                {
                    pose.dataType = EDataType.FlyStick;
                    pose.rigidBodyID = 1;
                }
            }
            return origin;
        }

        private static XRSpace CreateXRSpace(EScreenLayoutMode screenLayoutMode)
        {
            if (screenLayoutMode == EScreenLayoutMode.None || !CheckPackage()) return default;

            var xrSpace = EditorXRSpaceSolutionHelper.CreateXRSpace(screenLayoutMode.GetScreenCount(), (screenGroup, screens) =>
            {
                VirtualScreen.LayoutScreen(screens, screenLayoutMode);
            }, () =>
            {
                var origin = Create(out CameraController hmd, out Transform leftHand, out Transform rightHand);
                if (origin)//XRRig->XROrigin
                {
                    if (leftHand)
                    {
                        var interact = leftHand.XGetOrAddComponent<InteractIOByART>();
                        if (interact)
                        {
                        }
                    }

                    if (rightHand)
                    {
                        var interact = rightHand.XGetOrAddComponent<InteractIOByART>();
                        if (interact)
                        {
                            interact.flysitckID = 1;
                        }
                    }
                }
                return (origin, hmd, leftHand, rightHand);
            }, (i, camera, screen) =>
            {
                VirtualScreen.LayoutCamera(i, camera, screenLayoutMode);
            }, XRSpaceTitle);

            return xrSpace;
        }

        const string XRSpaceTitle = "ART+Flystick+单机多通道主动立体型XR交互空间";

        /// <summary>
        /// 创建XR空间，由ART型XR装备/原点、Flystick、单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution, nameof(ARTManager))]
        [Tool(ARTHelper.Title, nameof(ARTManager))]
        [Name(XRSpaceTitle)]
        [Tip("创建XR空间，由ART型XR装备/原点、Flystick、单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；", "Create XR space, which is an XR interactive space constructed by art XR equipment / origin, flystick, single machine multi-channel dynamic three-dimensional virtual screen and other tool components;")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(ARTManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager))]
        [Manual(typeof(ARTManager))]
        public static void CreateXRSpace_Flystick_MultiScreen_ActiveStereo(ToolContext toolContext)
        {
            MenuHelper.DrawMenu(nameof(CreateXRSpace_Flystick_MultiScreen_ActiveStereo), m =>
            {
                EnumCache<EScreenLayoutMode>.Array.Foreach(e =>
                {
                    m.AddMenuItem(CommonFun.Name(e), () => CreateXRSpace(e), () => true);
                });
            });
        }
    }
}
