using UnityEngine;
using UnityEngine.XR;
using XCSJ.Attributes;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginSamsungWMR.Base
{
    /// <summary>
    /// 轴与按钮:统一三星手柄所有的轴与按钮的枚举
    /// </summary>
    [Name("三星手柄轴与按钮")]
    public enum ESamsungWMRAxisAndButton
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
        MenuButton,

        /// <summary>
        /// 扳机
        /// </summary>
        [Name("扳机")]
        Trigger,

        /// <summary>
        /// 握把
        /// </summary>
        [Name("握把")]
        Grip,

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
    /// 三星手柄二维轴
    /// </summary>
    [Name("三星手柄二维轴")]
    public enum ESamsungWMRAxis2D
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

        /// <summary>
        /// 触控板
        /// </summary>
        [Name("触控板")]
        ThumbStick,
    }

    /// <summary>
    /// 三星手柄扩展:仅在新版输入系统中可用
    /// </summary>
    public static class SamsungWMRExtension
    {
        #region 获取二维轴值

        public static bool TryGetAxis2DValue(this ESamsungWMRAxis2D axisArea, EHandRule handType, out Vector2 value)
        {
            switch (handType)
            {
                case EHandRule.Left: return TryGetAxis2DValue(axisArea, EVRDeviceType.Left, out value);
                case EHandRule.Right: return TryGetAxis2DValue(axisArea, EVRDeviceType.Right, out value);
                case EHandRule.Any:
                    {
                        return TryGetAxis2DValue(axisArea, EVRDeviceType.Left, out value) || TryGetAxis2DValue(axisArea, EVRDeviceType.Right, out value);
                    }
                case EHandRule.All:
                    {
                        var rs1 = TryGetAxis2DValue(axisArea, EVRDeviceType.Left, out var leftValue);
                        var rs2 = TryGetAxis2DValue(axisArea, EVRDeviceType.Right, out var rightValue);
                        value = leftValue + rightValue;
                        return rs1 && rs2;
                    }
            }
            value = default;
            return default;
        }

        /// <summary>
        /// 获取轴区域的值
        /// </summary>
        /// <param name="axisArea"></param>
        /// <returns></returns>
        public static bool TryGetAxis2DValue(this ESamsungWMRAxis2D axisArea, EVRDeviceType deviceType, out Vector2 value)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            var dev = deviceType.GetDevice();
            if (dev.isValid)
            {
                switch (axisArea)
                {
                    case ESamsungWMRAxis2D.JoyStick:
                        {
                            return dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out value);
                        }
                    case ESamsungWMRAxis2D.ThumbStick:
                        {
                            return dev.TryGetFeatureValue(CommonUsages.secondary2DAxis, out value);
                        }
                }
            }
#endif
            value = default;
            return default;
        }

        #endregion

        #region 获取单轴值

        /// <summary>
        /// 获取轴与按钮的值
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static bool TryGetValue(this ESamsungWMRAxisAndButton axisAndButton, EVRDeviceType deviceType, out float value)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            var dev = deviceType.GetDevice();
            switch (axisAndButton)
            {
                case ESamsungWMRAxisAndButton.MenuButton:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.menuButton, out var pressed))
                        {
                            value = pressed ? 1 : 0;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.Trigger:
                    {
                        return dev.TryGetFeatureValue(CommonUsages.trigger, out value);
                    }
                case ESamsungWMRAxisAndButton.Grip:
                    {
                        return dev.TryGetFeatureValue(CommonUsages.grip, out value);
                    }
                case ESamsungWMRAxisAndButton.JoyStickUp://y:[0,1]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.secondary2DAxis, out var value2D) && value2D.y >= 0)
                        {
                            value = value2D.y;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.JoyStickDown://y:[-1,0]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.secondary2DAxis, out var value2D) && value2D.y <= 0)
                        {
                            value = value2D.y;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.JoyStickLeft://x:[-1,0]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.secondary2DAxis, out var value2D) && value2D.x <= 0)
                        {
                            value = value2D.x;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.JoyStickRight://x:[0,1]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.secondary2DAxis, out var value2D) && value2D.x >= 0)
                        {
                            value = value2D.x;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.JoyStickClick:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out var pressed))
                        {
                            value = pressed ? 1 : 0;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.ThumbStickUp://y:[0,1]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out var value2D) && value2D.y >= 0)
                        {
                            value = value2D.y;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.ThumbStickDown://y:[-1,0]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out var value2D) && value2D.y <= 0)
                        {
                            value = value2D.y;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.ThumbStickLeft://x:[-1,0]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out var value2D) && value2D.x <= 0)
                        {
                            value = value2D.x;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.ThumbStickRight://x:[0,1]
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxis, out var value2D) && value2D.x >= 0)
                        {
                            value = value2D.x;
                            return true;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.ThumbStickClick:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var pressed))
                        {
                            value = pressed ? 1 : 0;
                            return true;
                        }
                        break;
                    }
            }
#endif
            value = default;
            return default;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="handleType"></param>
        /// <returns></returns>
        public static bool TryGetValue(this ESamsungWMRAxisAndButton axisAndButton, EHandRule handRule, out float value)
        {
            switch (handRule)
            {
                case EHandRule.Left: return axisAndButton.TryGetValue(EVRDeviceType.Left, out value);
                case EHandRule.Right: return axisAndButton.TryGetValue(EVRDeviceType.Right, out value);
                case EHandRule.Any:
                    {
                        return axisAndButton.TryGetValue(EVRDeviceType.Left, out value) || axisAndButton.TryGetValue(EVRDeviceType.Right, out value);
                    }
                case EHandRule.All:
                    {
                        var rs1 = axisAndButton.TryGetValue(EVRDeviceType.Left, out var leftValue);
                        var rs2 = axisAndButton.TryGetValue(EVRDeviceType.Right, out var rightValue);
                        value = leftValue + rightValue;
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
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public static bool TryGetAbsValue(this ESamsungWMRAxisAndButton axisAndButton, EHandRule handRule, out float value)
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
        public static bool TryGetAbsValue(this ESamsungWMRAxisAndButton axisAndButton, EVRDeviceType deviceType, out float value)
        {
            var rs = TryGetValue(axisAndButton, deviceType, out value);
            value = Mathf.Abs(value);
            return rs;
        }

        #endregion

        #region 获取按钮按下状态

        /// <summary>
        /// 判断轴与按钮是否被按压
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deviceType"></param>
        /// <param name="deadZone"></param>
        /// <returns></returns>
        public static bool IsPressed(this ESamsungWMRAxisAndButton axisAndButton, EVRDeviceType deviceType, float deadZone = 0.5f)
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            var dev = deviceType.GetDevice();
            switch (axisAndButton)
            {
                case ESamsungWMRAxisAndButton.MenuButton:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.menuButton, out var value))
                        {
                            return value;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.Trigger:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.triggerButton, out var value))
                        {
                            return value;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.Grip:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.gripButton, out var value))
                        {
                            return value;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.JoyStickUp:
                case ESamsungWMRAxisAndButton.JoyStickDown:
                case ESamsungWMRAxisAndButton.JoyStickLeft:
                case ESamsungWMRAxisAndButton.JoyStickRight:
                case ESamsungWMRAxisAndButton.ThumbStickUp:
                case ESamsungWMRAxisAndButton.ThumbStickDown:
                case ESamsungWMRAxisAndButton.ThumbStickLeft:
                case ESamsungWMRAxisAndButton.ThumbStickRight:
                    {
                        if (axisAndButton.TryGetValue(deviceType, out var value))
                        {
                            return IsPressed(value, deadZone);
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.JoyStickClick:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.secondary2DAxisClick, out var value))
                        {
                            return value;
                        }
                        break;
                    }
                case ESamsungWMRAxisAndButton.ThumbStickClick:
                    {
                        if (dev.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out var value))
                        {
                            return value;
                        }
                        break;
                    }
            }
#endif
            return default;
        }

        private static bool IsPressed(float value, float deadZone)
        {
            return Mathf.Abs(value) > deadZone ? true : false;
        }

        /// <summary>
        /// 判断轴与按钮是否被按压
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="handRule"></param>
        /// <param name="_deadZone"></param>
        /// <returns></returns>
        public static bool IsPressed(this ESamsungWMRAxisAndButton axisAndButton, EHandRule handRule, float _deadZone = 0.5f)
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

        #endregion
    }
}
