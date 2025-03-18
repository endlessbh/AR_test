using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using UnityEngine.XR;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 姿态IO通过XR手柄设备
    /// </summary>
    [Name("姿态IO通过XR手柄设备")]
    [Tip("通过XR手柄设备模拟控制器姿态的输入输出", "The input and output of controller attitude are simulated by XR handle device")]
    [RequireManager(typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.Target)]
    [DisallowMultipleComponent]
    public class PoseIOByXRHandDevice : BaseAnalogControllerIO, IPoseIO
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        [Name("设备类型")]
        [EnumPopup]
        public EVRDeviceType _deviceType = EVRDeviceType.Left;

#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public InputTrackingState TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation)
        {
            return InputDeviceHelper.TryGetPose(_deviceType, out position, out rotation);
        }

#else
        /// <summary>
        /// 尝试获取姿态
        /// </summary>
        /// <param name="analogController"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public PoseDataFlags TryGetPose(AnalogController analogController, out Vector3 position, out Quaternion rotation)
        {
            return InputDeviceHelper.TryGetPose(_deviceType, out position, out rotation);
        }
#endif

#endif
    }
}
