using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using UnityEngine.XR;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 相机变换通过XR头盔设备：使用XR头盔设备的定位数据控制相机变换的姿态（位置与旋转）
    /// </summary>
    [Name("相机变换通过XR头盔设备")]
    [Tip("使用XR头盔设备的定位数据控制相机变换的姿态（位置与旋转）", "Use the positioning data of XR helmet equipment to control the attitude (position and rotation) of the camera")]
    [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraTransformer))]
    //[Tool(XRITHelper.Title)]
    [RequireManager(typeof(XXRInteractionToolkitManager), typeof(CameraManager))]
    [XCSJ.Attributes.Icon(EIcon.Camera)]
    [DisallowMultipleComponent]
    [Owner(typeof(XXRInteractionToolkitManager))]
    public class CameraTransformByXRHMDDevice : BaseCameraTransformController
    {
        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.Local;

        /// <summary>
        /// 空间类型
        /// </summary>
        public ESpaceType spaceType => _spaceType;

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER
            var inputTrackingState = InputDeviceHelper.TryGetPose(EVRDeviceType.HMD, out Vector3 position, out Quaternion rotation);

            if ((inputTrackingState & InputTrackingState.Position) != 0)
            {
                cameraTransformer.SetPosition(_spaceType, position);
            }

            if ((inputTrackingState & InputTrackingState.Rotation) != 0)
            {
                cameraTransformer.SetRotation(_spaceType, rotation);
            }

#else
            var poseData = InputDeviceHelper.TryGetPose(EVRDeviceType.HMD, out Vector3 position, out Quaternion rotation);

            if ((poseData & PoseDataFlags.Position) != 0)
            {
                cameraTransformer.SetPosition(_spaceType, position);
            }

            if ((poseData & PoseDataFlags.Rotation) != 0)
            {
                cameraTransformer.SetRotation(_spaceType, rotation);
            }
#endif

#endif
        }
    }
}
