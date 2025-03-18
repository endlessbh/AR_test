using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

#if XDREAMER_STEAMVR
using Valve.VR;
#endif

namespace XCSJ.PluginHTCVive.Base
{
    /// <summary>
    /// Vive Cosmos轴与按钮:统一Vive Cosmos手柄所有的轴与按钮的枚举
    /// </summary>
    [Name("Vive Cosmos轴与按钮")]
    public enum EViveCosmosAxisAndButton
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// VIVE 按钮
        /// </summary>
        [Name("VIVE 按钮")]
        ViveButton,

        /// <summary>
        /// B 按钮
        /// </summary>
        [Name("B 按钮")]
        B_Button,

        /// <summary>
        /// A 按钮
        /// </summary>
        [Name("A 按钮")]
        A_Button,

        /// <summary>
        /// Y 按钮
        /// </summary>
        [Name("Y 按钮")]
        Y_Button,

        /// <summary>
        /// X 按钮
        /// </summary>
        [Name("X 按钮")]
        X_Button,

        /// <summary>
        /// 菜单按键
        /// </summary>
        [Name("菜单按键")]
        Menu,

        /// <summary>
        /// 扳机
        /// </summary>
        [Name("扳机")]
        Trigger,

        /// <summary>
        /// 握把按钮
        /// </summary>
        [Name("握把按钮")]
        GripButton,

        /// <summary>
        /// 缓冲按钮
        /// </summary>
        [Name("缓冲按钮")]
        Bumper,

        /// <summary>
        /// 摇杆上
        /// </summary>
        [Name("摇杆上")]
        JoyStickUp,

        /// <summary>
        /// 摇杆下
        /// </summary>
        [Name("摇杆下")]
        JoyStickDown,

        /// <summary>
        /// 摇杆左
        /// </summary>
        [Name("摇杆左")]
        JoyStickLeft,

        /// <summary>
        /// 摇杆右
        /// </summary>
        [Name("摇杆右")]
        JoyStickRight,

        /// <summary>
        /// 摇杆点击
        /// </summary>
        [Name("摇杆点击")]
        JoyStickClick,
    }

    /// <summary>
    /// Vive Cosmos二维轴
    /// </summary>
    [Name("Vive Cosmos二维轴")]
    public enum EViveCosmosAxis2D
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 摇杆
        /// </summary>
        [Name("摇杆")]
        JoyStick,
    }

    /// <summary>
    /// Vive Cosmos轴与按钮扩展
    /// </summary>
    public static class ViveCosmosAxisAndButtonExtension
    {
        #region 获取二维轴值

        /// <summary>
        /// 获取轴区域的值
        /// </summary>
        /// <param name="axisArea"></param>
        /// <param name="deviceType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetAxis2DValue(this EViveCosmosAxis2D axisArea, EVRDeviceType deviceType, out Vector2 value)
        {
            if (SteamVRHelper.InitSteamVRInput())
            {
#if XDREAMER_STEAMVR
                if (SteamVRHelper.TryConvertDeviceTypeToInputSource(deviceType, out SteamVR_Input_Sources inputSource))
                {
                    switch (axisArea)
                    {
                        case EViveCosmosAxis2D.JoyStick:
                            {
                                value = SteamVRHelper.GetMoveAction(inputSource);
                                return true;
                            }
                    }
                }
#endif
            }

            value = default;
            return default;
        }

        /// <summary>
        /// 获取二维轴的值
        /// </summary>
        /// <param name="axisArea"></param>
        /// <param name="handType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetAxis2DValue(this EViveCosmosAxis2D axisArea, EHandRule handType, out Vector2 value)
        {
            switch (handType)
            {
                case EHandRule.Left: return TryGetAxis2DValue(axisArea, EVRDeviceType.Left, out value);
                case EHandRule.Right: return TryGetAxis2DValue(axisArea, EVRDeviceType.Right, out value);
                case EHandRule.Any:
                    {
                        return axisArea.TryGetAxis2DValue(EVRDeviceType.Left, out value) || axisArea.TryGetAxis2DValue(EVRDeviceType.Right, out value);
                    }
                case EHandRule.All:
                    {
                        var rs1 = axisArea.TryGetAxis2DValue(EVRDeviceType.Left, out var leftValue);
                        var rs2 = axisArea.TryGetAxis2DValue(EVRDeviceType.Right, out var rightValue);
                        value = leftValue + rightValue;
                        return rs1 && rs2;
                    }
            }
            value = default;
            return default;
        }

        #endregion

        #region 获取单轴值

        /// <summary>
        ///  获取轴与按钮的值：通过手规则
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="handRule"></param>
        /// <returns></returns>
        public static bool TryGetValue(this EViveCosmosAxisAndButton axisAndButton, EHandRule handRule, out float value)
        {
            switch (handRule)
            {
                case EHandRule.Left:
                    {
                        return axisAndButton.TryGetValue(EVRDeviceType.Left, out value);
                    }
                case EHandRule.Right:
                    {
                        return axisAndButton.TryGetValue(EVRDeviceType.Right, out value);
                    }
                case EHandRule.Any:
                    {
                        return axisAndButton.TryGetValue(EVRDeviceType.Left, out value) || axisAndButton.TryGetValue(EVRDeviceType.Right, out value);
                    }
                case EHandRule.All:
                    {
                        var rs1 = axisAndButton.TryGetValue(EVRDeviceType.Left, out var leftValue);
                        var rs2 = axisAndButton.TryGetValue(EVRDeviceType.Right, out var RightValue);
                        value = leftValue + RightValue;
                        return rs1 && rs2;
                    }
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 获取轴与按钮的绝对值
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="handRule"></param>
        /// <returns></returns>
        public static bool GetAbsValue(this EViveCosmosAxisAndButton axisAndButton, EHandRule handRule, out float value)
        {
            var rs = TryGetValue(axisAndButton, handRule, out value);
            value = Mathf.Abs(value);
            return rs;
        }

        /// <summary>
        /// 获取轴与按钮的绝对值
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static bool GetAbsValue(this EViveCosmosAxisAndButton axisAndButton, EVRDeviceType deviceType, out float value)
        {
            var rs = TryGetValue(axisAndButton, deviceType, out value);
            value = Mathf.Abs(value);
            return rs;
        }

        /// <summary>
        /// 获取轴与按钮的值：通过设备类型
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static bool TryGetValue(this EViveCosmosAxisAndButton axisAndButton, EVRDeviceType deviceType, out float value)
        {
#if XDREAMER_STEAMVR
            if (SteamVRHelper.InitSteamVRInput() && SteamVRHelper.TryConvertDeviceTypeToInputSource(deviceType, out SteamVR_Input_Sources inputSource))
            {
                return TryGetValue(axisAndButton, inputSource, out value);
            }
#endif
            value = default;
            return default;
        }

#if XDREAMER_STEAMVR
        private static bool TryGetValue(this EViveCosmosAxisAndButton axisAndButton, SteamVR_Input_Sources inputSource, out float value)
        {
            switch (axisAndButton)
            {
                case EViveCosmosAxisAndButton.ViveButton:
                case EViveCosmosAxisAndButton.Bumper:
                case EViveCosmosAxisAndButton.Menu:
                    {
                        return SteamVRHelper.TryGetAction(axisAndButton.ToString(), inputSource, out value);
                    }
                case EViveCosmosAxisAndButton.B_Button:
                    {
                        if (inputSource == SteamVR_Input_Sources.RightHand)
                        {
                            value = SteamVRHelper.GetResetButtonState(inputSource) ? 1 : 0;
                            return true;
                        }
                        value = default;
                        return false;
                    }
                case EViveCosmosAxisAndButton.A_Button:
                    {
                        if (inputSource == SteamVR_Input_Sources.RightHand)
                        {
                            value = SteamVRHelper.GetBrakeButtonState(inputSource) ? 1 : 0;
                            return true;
                        }
                        value = default;
                        return false;
                    }
                case EViveCosmosAxisAndButton.Y_Button:
                    {
                        if (inputSource == SteamVR_Input_Sources.LeftHand)
                        {
                            value = SteamVRHelper.GetResetButtonState(inputSource) ? 1 : 0;
                            return true;
                        }
                        value = default;
                        return false;
                    }
                case EViveCosmosAxisAndButton.X_Button:
                    {
                        if (inputSource == SteamVR_Input_Sources.LeftHand)
                        {
                            value = SteamVRHelper.GetBrakeButtonState(inputSource) ? 1 : 0;
                            return true;
                        }
                        value = default;
                        return false;
                    }
                case EViveCosmosAxisAndButton.Trigger:
                    {
#if XDREAMER_STEAMVR_INPUT
                        value = SteamVR_Actions.default_Squeeze.GetAxis(inputSource);
#else
                        value = 0;
#endif
                        return true;
                    }
                case EViveCosmosAxisAndButton.GripButton:
                    {
#if XDREAMER_STEAMVR_INPUT
                        value = SteamVR_Actions.default_GrabGrip.GetState(inputSource) ? 1 : 0;
#else
                        value = 0;
#endif
                        return true;
                    }
                case EViveCosmosAxisAndButton.JoyStickUp://y:[0,1]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.y >= 0)
                        {
                            value = move.y;
                            return true;
                        }
                        break;
                    }
                case EViveCosmosAxisAndButton.JoyStickDown://y:[-1,0]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.y <= 0)
                        {
                            value = move.y;
                            return true;
                        }
                        break;
                    }
                case EViveCosmosAxisAndButton.JoyStickLeft://x:[-1,0]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.x <= 0)
                        {
                            value = move.x;
                            return true;
                        }
                        break;
                    }
                case EViveCosmosAxisAndButton.JoyStickRight://x:[0,1]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.x >= 0)
                        {
                            value = move.x;
                            return true;
                        }
                        break;
                    }
                case EViveCosmosAxisAndButton.JoyStickClick:
                    {
#if XDREAMER_STEAMVR_INPUT
                        value = SteamVR_Actions.default_Teleport.GetState(inputSource) ? 1 : 0;
#else
                        value = 0;
#endif
                        return true;
                    }
            }
            value = 0;
            return false;
        }
#endif

        #endregion

        #region 获取按钮按下状态

        /// <summary>
        /// 判断轴或按钮是否按下：通过手规则
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="handRule"></param>
        /// <param name="_deadZone"></param>
        /// <returns></returns>
        public static bool IsPressed(this EViveCosmosAxisAndButton axisAndButton, EHandRule handRule, float _deadZone = 0.5f)
        {
            switch (handRule)
            {
                case EHandRule.None: return true;
                case EHandRule.Left: return axisAndButton.IsPressed(EVRDeviceType.Left, _deadZone);
                case EHandRule.Right: return axisAndButton.IsPressed(EVRDeviceType.Right, _deadZone);
                case EHandRule.Any: return axisAndButton.IsPressed(EVRDeviceType.Left, _deadZone) || axisAndButton.IsPressed(EVRDeviceType.Right, _deadZone);
                case EHandRule.All: return axisAndButton.IsPressed(EVRDeviceType.Left, _deadZone) && axisAndButton.IsPressed(EVRDeviceType.Right, _deadZone);
            }
            return false;
        }

        /// <summary>
        /// 判断轴与按钮是否被按压
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deadZone"></param>
        /// <returns></returns>
        public static bool IsPressed(this EViveCosmosAxisAndButton axisAndButton, EVRDeviceType deviceType, float deadZone = 0.5f)
        {
#if XDREAMER_STEAMVR
            if (SteamVRHelper.InitSteamVRInput())
            {
                if (SteamVRHelper.TryConvertDeviceTypeToInputSource(deviceType, out SteamVR_Input_Sources inputSource))
                {
                    switch (axisAndButton)
                    {
                        case EViveCosmosAxisAndButton.ViveButton:
                        case EViveCosmosAxisAndButton.Bumper:
                        case EViveCosmosAxisAndButton.Menu:
                            {
                                if (SteamVRHelper.TryGetActionState(axisAndButton.ToString(), inputSource, out var state))
                                {
                                    return state;
                                }
                                return false;
                            }
                        case EViveCosmosAxisAndButton.B_Button:
                            {
                                if (deviceType == EVRDeviceType.Right)
                                {
                                    return SteamVRHelper.GetResetButtonState(inputSource);
                                }
                                return false;
                            }
                        case EViveCosmosAxisAndButton.A_Button:
                            {
                                if (deviceType == EVRDeviceType.Right)
                                {
                                    return SteamVRHelper.GetBrakeButtonState(inputSource);
                                }
                                return false;
                            }
                        case EViveCosmosAxisAndButton.Y_Button:
                            {
                                if (deviceType == EVRDeviceType.Left)
                                {
                                    return SteamVRHelper.GetResetButtonState(inputSource);
                                }
                                return false;
                            }
                        case EViveCosmosAxisAndButton.X_Button:
                            {
                                if (deviceType == EVRDeviceType.Left)
                                {
                                    return SteamVRHelper.GetBrakeButtonState(inputSource);
                                }
                                return false;
                            }
                        case EViveCosmosAxisAndButton.Trigger:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_GrabPinch.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveCosmosAxisAndButton.GripButton:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_GrabGrip.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveCosmosAxisAndButton.JoyStickUp:
                            {
                                var move = SteamVRHelper.GetMoveAction(inputSource);
                                if (move.y >= 0)
                                {
                                    return Mathf.Abs(move.y) > deadZone ? true : false;
                                }
                                break;
                            }
                        case EViveCosmosAxisAndButton.JoyStickDown:
                            {
                                var move = SteamVRHelper.GetMoveAction(inputSource);
                                if (move.y <= 0)
                                {
                                    return Mathf.Abs(move.y) > deadZone ? true : false;
                                }
                                break;
                            }
                        case EViveCosmosAxisAndButton.JoyStickLeft:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_SnapTurnLeft.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveCosmosAxisAndButton.JoyStickRight:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_SnapTurnRight.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveCosmosAxisAndButton.JoyStickClick:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_Teleport.GetState(inputSource);
#else
                                return false;
#endif
                            }
                    }
                }
            }
#endif
                                return default;
        }

#endregion
    }

}
