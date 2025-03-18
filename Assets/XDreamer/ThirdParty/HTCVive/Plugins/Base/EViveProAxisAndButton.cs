using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

#if XDREAMER_STEAMVR
using Valve.VR;
#endif

namespace XCSJ.PluginHTCVive.Base
{
    /// <summary>
    /// HTC Vive Pro轴与按钮:统一HTC Vive Pro手柄所有的轴与按钮的枚举
    /// </summary>
    [Name("HTC Vive Pro轴与按钮")]
    public enum EViveProAxisAndButton
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

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
        /// 抓握按钮
        /// </summary>
        [Name("抓握按钮")]
        GripButton,

        /// <summary>
        /// 触控板上
        /// </summary>
        [Name("触控板上")]
        ThumbStickUp,

        /// <summary>
        /// 触控板下
        /// </summary>
        [Name("触控板下")]
        ThumbStickDown,

        /// <summary>
        /// 触控板左
        /// </summary>
        [Name("触控板左")]
        ThumbStickLeft,

        /// <summary>
        /// 触控板右
        /// </summary>
        [Name("触控板右")]
        ThumbStickRight,

        /// <summary>
        /// 触控板点击
        /// </summary>
        [Name("触控板点击")]
        ThumbStickClick,
    }

    /// <summary>
    /// HTC Vive Pro二维轴
    /// </summary>
    [Name("HTC Vive Pro二维轴")]
    public enum EViveProAxis2D
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 触控板
        /// </summary>
        [Name("触控板")]
        ThumbStick,
    }

    /// <summary>
    /// HTC Vive Pro轴与按钮扩展
    /// </summary>
    public static class HTCViveProAxisAndButtonExtension
    {
        #region 获取二维轴值

        /// <summary>
        /// 获取轴区域的值
        /// </summary>
        /// <param name="axisArea"></param>
        /// <param name="deviceType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetAxis2DValue(this EViveProAxis2D axisArea, EVRDeviceType deviceType, out Vector2 value)
        {
            if (SteamVRHelper.InitSteamVRInput())
            {
#if XDREAMER_STEAMVR
                if (SteamVRHelper.TryConvertDeviceTypeToInputSource(deviceType, out SteamVR_Input_Sources inputSource))
                {
                    switch (axisArea)
                    {
                        case EViveProAxis2D.ThumbStick:
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
        public static bool TryGetAxis2DValue(this EViveProAxis2D axisArea, EHandRule handType, out Vector2 value)
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
        public static bool TryGetValue(this EViveProAxisAndButton axisAndButton, EHandRule handRule, out float value)
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
        public static bool GetAbsValue(this EViveProAxisAndButton axisAndButton, EHandRule handRule, out float value)
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
        public static bool GetAbsValue(this EViveProAxisAndButton axisAndButton, EVRDeviceType deviceType, out float value)
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
        public static bool TryGetValue(this EViveProAxisAndButton axisAndButton, EVRDeviceType deviceType, out float value)
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
        private static bool TryGetValue(this EViveProAxisAndButton axisAndButton, SteamVR_Input_Sources inputSource, out float value)
        {
            switch (axisAndButton)
            {
                case EViveProAxisAndButton.Menu:
                    {
                        return SteamVRHelper.TryGetAction(axisAndButton.ToString(), inputSource, out value);
                    }
                case EViveProAxisAndButton.Trigger:
                    {
#if XDREAMER_STEAMVR_INPUT
                        value = SteamVR_Actions.default_Squeeze.GetAxis(inputSource);
#else
                        value = 0f;
#endif
                        return true;
                    }
                case EViveProAxisAndButton.GripButton:
                    {
#if XDREAMER_STEAMVR_INPUT
                        value = SteamVR_Actions.default_GrabGrip.GetState(inputSource) ? 1 : 0;
#else
                        value = 0;
#endif
                        return true;
                    }
                case EViveProAxisAndButton.ThumbStickUp://y:[0,1]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.y >= 0)
                        {
                            value = move.y;
                            return true;
                        }
                        break;
                    }
                case EViveProAxisAndButton.ThumbStickDown://y:[-1,0]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.y <= 0)
                        {
                            value = move.y;
                            return true;
                        }
                        break;
                    }
                case EViveProAxisAndButton.ThumbStickLeft://x:[-1,0]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.x <= 0)
                        {
                            value = move.x;
                            return true;
                        }
                        break;
                    }
                case EViveProAxisAndButton.ThumbStickRight://x:[0,1]
                    {
                        var move = SteamVRHelper.GetMoveAction(inputSource);
                        if (move.x >= 0)
                        {
                            value = move.x;
                            return true;
                        }
                        break;
                    }
                case EViveProAxisAndButton.ThumbStickClick:
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
                        public static bool IsPressed(this EViveProAxisAndButton axisAndButton, EHandRule handRule, float _deadZone = 0.5f)
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
        public static bool IsPressed(this EViveProAxisAndButton axisAndButton, EVRDeviceType deviceType, float deadZone = 0.5f)
        {
#if XDREAMER_STEAMVR
            if (SteamVRHelper.InitSteamVRInput())
            {
                if (SteamVRHelper.TryConvertDeviceTypeToInputSource(deviceType, out SteamVR_Input_Sources inputSource))
                {
                    switch (axisAndButton)
                    {
                        case EViveProAxisAndButton.Menu:
                            {
                                if( SteamVRHelper.TryGetActionState(axisAndButton.ToString(), inputSource, out var state))
                                {
                                    return state;
                                }
                                return false;
                            }
                        case EViveProAxisAndButton.Trigger:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_GrabPinch.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveProAxisAndButton.GripButton:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_GrabGrip.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveProAxisAndButton.ThumbStickUp:
                            {
                                var move = SteamVRHelper.GetMoveAction(inputSource);
                                if (move.y >= 0)
                                {
                                    return Mathf.Abs(move.y) > deadZone ? true : false;
                                }
                                break;
                            }
                        case EViveProAxisAndButton.ThumbStickDown:
                            {
                                var move = SteamVRHelper.GetMoveAction(inputSource);
                                if (move.y <= 0)
                                {
                                    return Mathf.Abs(move.y) > deadZone ? true : false;
                                }
                                break;
                            }
                        case EViveProAxisAndButton.ThumbStickLeft:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_SnapTurnLeft.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveProAxisAndButton.ThumbStickRight:
                            {
#if XDREAMER_STEAMVR_INPUT
                                return SteamVR_Actions.default_SnapTurnRight.GetState(inputSource);
#else
                                return false;
#endif
                            }
                        case EViveProAxisAndButton.ThumbStickClick:
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
