using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.SpatialTracking;
#endif

namespace XCSJ.PluginXXR.Interaction.Toolkit.Base
{
    /// <summary>
    /// 输入设备助手
    /// </summary>
    public static class InputDeviceHelper
    {
#if XDREAMER_XR_INTERACTION_TOOLKIT
        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static InputDevice GetDevice(this EVRDeviceType deviceType)
        {
            switch (deviceType)
            {
                case EVRDeviceType.HMD: return InputDevices.GetDeviceAtXRNode(XRNode.Head);
                case EVRDeviceType.Left: return InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
                case EVRDeviceType.Right: return InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            }
            return default;
        }

#if XDREAMER_XR_INTERACTION_TOOLKIT_2_0_0_OR_NEWER

        /// <summary>
        /// 获取头盔的位置与旋转
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static InputTrackingState TryGetPose(EVRDeviceType deviceType, out Vector3 position, out Quaternion rotation)
        {
            var inputTrackingState = InputTrackingState.None;

            switch (deviceType)
            {
                case EVRDeviceType.HMD:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyePosition, out position))
                            {
                                inputTrackingState |= InputTrackingState.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyeRotation, out rotation))
                            {
                                inputTrackingState |= InputTrackingState.Rotation;
                            }
                            return inputTrackingState;
                        }
                        break;
                    }
                case EVRDeviceType.Left:
                case EVRDeviceType.Right:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                            {
                                inputTrackingState |= InputTrackingState.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                            {
                                inputTrackingState |= InputTrackingState.Rotation;
                            }
                            return inputTrackingState;
                        }
                        break;
                    }
            }
            position = default;
            rotation = default;
            return inputTrackingState;
        }

#else

        /// <summary>
        /// 获取头盔的位置与旋转
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static PoseDataFlags TryGetPose(EVRDeviceType deviceType, out Vector3 position, out Quaternion rotation)
        {
            var poseDataFlag = PoseDataFlags.NoData;

            switch (deviceType)
            {
                case EVRDeviceType.HMD:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyePosition, out position))
                            {
                                poseDataFlag |= PoseDataFlags.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.centerEyeRotation, out rotation))
                            {
                                poseDataFlag |= PoseDataFlags.Rotation;
                            }
                            return poseDataFlag;
                        }
                        break;
                    }
                case EVRDeviceType.Left:
                case EVRDeviceType.Right:
                    {
                        var dev = deviceType.GetDevice();
                        if (dev.isValid)
                        {
                            if (dev.TryGetFeatureValue(CommonUsages.devicePosition, out position))
                            {
                                poseDataFlag |= PoseDataFlags.Position;
                            }
                            if (dev.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
                            {
                                poseDataFlag |= PoseDataFlags.Rotation;
                            }
                            return poseDataFlag;
                        }
                        break;
                    }
            }
            position = default;
            rotation = default;
            return poseDataFlag;
        }
#endif

#endif
    }

    /// <summary>
    /// VR设备类型
    /// </summary>
    [Name("VR设备类型")]
    public enum EVRDeviceType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 头盔
        /// </summary>
        [Name("头盔")]
        HMD,

        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        Left,

        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        Right,
    }

    /// <summary>
    /// 手柄规则
    /// </summary>
    [Name("手柄规则")]
    public enum EHandRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 左
        /// </summary>
        [Name("左")]
        Left,

        /// <summary>
        /// 右
        /// </summary>
        [Name("右")]
        Right,

        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        [Tip("左或右任意一个", "Either left or right")]
        Any,

        /// <summary>
        /// 左和右
        /// </summary>
        [Name("左和右")]
        [Tip("左和右同时", "Left and right at the same time")]
        All,
    }
}
