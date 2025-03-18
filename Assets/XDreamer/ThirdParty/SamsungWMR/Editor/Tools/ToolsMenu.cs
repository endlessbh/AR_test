using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorXXR.Interaction.Toolkit;
using XCSJ.EditorXXR.Interaction.Toolkit.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPico;
using XCSJ.PluginSamsungWMR;
using XCSJ.PluginSamsungWMR.Tools;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.EditorSamsungWMR.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        const string XRSpaceTitle = "三星玄龙型XR交互空间";

        /// <summary>
        /// 三星玄龙型XR空间，创建三星玄龙装备/原点，由头盔与左右两个手柄构成；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution)]
        [Tool(SamsungWMRManager.Title)]
        [Name(XRSpaceTitle)]
        [Tip("创建三星玄龙装备/原点，由头盔与左右两个手柄构成；", "Create Samsung Xuanlong equipment / origin, which is composed of helmet and left and right handles;")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(SamsungWMRManager), typeof(XXRInteractionToolkitManager))]
        [Manual(typeof(SamsungWMRManager))]
        public static void CreateSamsungWMRXRRig(ToolContext toolContext)
        {
            CreateXRSpace();
        }

        /// <summary>
        /// 创建三星玄龙型XR空间
        /// </summary>
        /// <returns></returns>
        public static GameObject CreateXRSpace()
        {
            if (!CheckPackage()) return default;

            var origin = EditorXRITHelper.Create(out CameraController hmd, out Transform leftHand, out Transform rightHand, out var locomotionSystem, null);
            if (!origin) return null;

            origin.transform.XResetLocalPRS();
#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            origin.gameObject.XSetUniqueName("XR原点 - 三星玄龙");
#else
            origin.gameObject.XSetUniqueName("XR装备 - 三星玄龙");
#endif
            if (hmd)
            {
                hmd.cameraTransformer.XGetOrAddComponent<CameraTransformByXRHMDDevice>();
            }

            if (leftHand)
            {
                var obj = leftHand.XAddComponent<InteractIOBySamsungWMR>();
                if (obj) obj._handleType = EHandRule.Left;

                var pose = leftHand.XAddComponent<PoseIOByXRHandDevice>();
                if (pose) pose._deviceType = EVRDeviceType.Left;
            }

            if (rightHand)
            {
                var obj = rightHand.XAddComponent<InteractIOBySamsungWMR>();
                if (obj) obj._handleType = EHandRule.Right;

                var pose = rightHand.XAddComponent<PoseIOByXRHandDevice>();
                if (pose) pose._deviceType = EVRDeviceType.Right;
            }

            if (locomotionSystem)
            {
                var mover = locomotionSystem.XAddComponent<TransformBySamsungWMR>();
                if (mover)
                {
                    mover._targetTransform = origin.transform;
                    mover.SetDefaultMove();
                }

                var rotater = locomotionSystem.XAddComponent<TransformBySamsungWMR>();
                if (rotater)
                {
                    rotater._targetTransform = origin.transform;
                    rotater.SetDefaultRotateWorldY();
                }
            }

            return origin.gameObject;
        }

        private static bool CheckPackage() => XRITHelper.CheckPackage();
    }
}
