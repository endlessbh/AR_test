using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorXRSpaceSolution;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXBox;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginZVR;
using XCSJ.PluginZVR.Tools;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorZVR.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// HMD-ZVR型飞行相机
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ZVRHelper.Title, nameof(ZVRManager), rootType = typeof(ZVRManager), groupRule = EToolGroupRule.None)]
        [Name("HMD-ZVR型飞行相机")]
        [XCSJ.Attributes.Icon(EIcon.FlyCamera)]
        [RequireManager(typeof(ZVRManager), typeof(StereoViewManager))]
        [Manual(typeof(ZVRManager))]
        public static void CreateZVRCamera(ToolContext toolContext)
        {
            var cameraController = CreateZVRCamera();
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, cameraController.gameObject);
        }

        private static CameraController CreateZVRCamera() => XRSpaceSolutionHelper.CreateHMDFlyCamera<CameraTransformByZVR>("HMD-ZVR型平移飞行相机");

        /// <summary>
        /// ZVR型XR装备:创建基于ZVR跟踪定位的XR装备
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(ZVRHelper.Title, nameof(ZVRManager))]
        [Name("ZVR型XR装备")]
        [Tip("创建基于ZVR跟踪定位的XR装备", "Create XR equipment based on ZVR tracking and positioning")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(ZVRManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager))]
        [Manual(typeof(ZVRManager))]
        public static void CreateXRRig(ToolContext toolContext) => CreateXRRig(out _, out _, out _);

        private static bool CheckPackage()
        {
#if XDREAMER_ZVR
            return true;
#else
            Debug.LogWarning("插件[" + ZVRHelper.Title + "]依赖库缺失！");
            return false;
#endif
        }

        /// <summary>
        /// 创建基于ZVR跟踪定位的XR装备
        /// </summary>
        /// <returns></returns>
        private static MonoBehaviour CreateXRRig(out CameraController hmd, out Transform leftHand, out Transform rightHand)
        {
            hmd = default;
            leftHand = default;
            rightHand = default;

            if (!CheckPackage()) return default;

            var origin = EditorXRITHelper.Create(out hmd, out leftHand, out rightHand, out var locomotionSystem, CreateZVRCamera);
            if (!origin) return origin;
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.gameObject.XSetUniqueName("XR原点 - ZVR");
#else
            origin.gameObject.XSetUniqueName("XR装备 - ZVR");
#endif
            if (hmd)
            {
                hmd.cameraTransformer.XGetOrAddComponent<CameraTransformByZVR>();
            }

            if (leftHand)
            {
                var pose = leftHand.XAddComponent<PoseIOByZVR>();
                if (pose) pose.rigidBodyID = 1;
            }

            if (rightHand)
            {
                var pose = rightHand.XAddComponent<PoseIOByZVR>();
                if (pose) pose.rigidBodyID = 2;
            }
            return origin;
        }

        private static XRSpace CreateXRSpace(EScreenLayoutMode screenLayoutMode)
        {
            if (screenLayoutMode == EScreenLayoutMode.None || !CheckPackage()) return default;

            return EditorXRSpaceSolutionHelper.CreateXRSpace_XBox(screenLayoutMode.GetScreenCount(), (screenGroup, screens) =>
            {
                VirtualScreen.LayoutScreen(screens, screenLayoutMode);
            }, () =>
            {
                var origin = CreateXRRig(out CameraController hmd, out Transform leftHand, out Transform rightHand);
                return (origin, hmd, leftHand, rightHand);
            }, (i, camera, screen) =>
            {
                VirtualScreen.LayoutCamera(i, camera, screenLayoutMode);
            }, XRSpaceTitle);
        }

        const string XRSpaceTitle = "ZVR+XBox+单机多通道主动立体型XR交互空间";

        /// <summary>
        /// 创建XR空间，由ZVR型XR装备、XBox、单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution, nameof(ZVRManager))]
        [Tool(ZVRHelper.Title, nameof(ZVRManager))]
        [Name(XRSpaceTitle)]
        [Tip("创建XR空间，由ZVR型XR装备、XBox、单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；", "Create XR space, which is an XR interactive space constructed by ZVR XR equipment, Xbox, single machine multi-channel dynamic three-dimensional virtual screen and other tool components;")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(ZVRManager), typeof(XXRInteractionToolkitManager), typeof(StereoViewManager), typeof(XBoxManager))]
        [Manual(typeof(ZVRManager))]
        public static void CreateXRSpace_XBox_MultiScreen_ActiveStereo(ToolContext toolContext)
        {
            MenuHelper.DrawMenu(nameof(CreateXRSpace_XBox_MultiScreen_ActiveStereo), m =>
            {
                EnumCache<EScreenLayoutMode>.Array.Foreach(e =>
                {
                    m.AddMenuItem(CommonFun.Name(e), () => CreateXRSpace(e), () => true);
                });
            });
        }        
    }
}
