using System;
using XCSJ.Attributes;
using XCSJ.Algorithms;
using System.Collections.Generic;
using XCSJ.Caches;
#if UNITY_2018_3_OR_NEWER
using UnityEngine;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.InputSystems;
#endif

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
#endif

namespace XCSJ.PluginXBox.Base
{
    /// <summary>
    /// XBox控制器轴区域:可用于返回<see cref="Vector2"/>型的值；
    /// </summary>
    [Name("XBox控制器轴区域")]
    public enum EXBoxAxisArea
    {
        /// <summary>
        /// 左摇杆
        /// </summary>
        [Name("左摇杆")]
        LeftStick,

        /// <summary>
        /// 右摇杆
        /// </summary>
        [Name("右摇杆")]
        RightStick,

        /// <summary>
        /// DPad
        /// </summary>
        [Name("DPad")]
        DPad,

        /// <summary>
        /// 扳机:左扳机对应X；右扳机对应Y;
        /// </summary>
        [Name("扳机")]
        [Tip("左扳机对应X；右扳机对应Y;")]
        Triggers,
    }

    /// <summary>
    /// XBox控制器轴与按钮:统一XBox控制器所有的轴与按钮的枚举
    /// </summary>
    [Name("XBox控制器轴与按钮")]
    public enum EXBoxAxisAndButton
    {
        /// <summary>
        /// 无：不代表任何轴或按钮
        /// </summary>
        [Name("无")]
        [Tip("不代表任何轴或按钮")]
        None,

        /// <summary>
        /// Dpad上
        /// </summary>
        [Name("Dpad上")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.DPadUp)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadUp)]
#endif
        DpadUp,

        /// <summary>
        /// Dpad下
        /// </summary>
        [Name("Dpad下")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.DPadDown)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadDown)]
#endif
        DpadDown,

        /// <summary>
        /// Dpad左
        /// </summary>
        [Name("Dpad左")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.DPadLeft)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadLeft)]
#endif
        DpadLeft,

        /// <summary>
        /// Dpad右
        /// </summary>
        [Name("Dpad右")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.DPadRight)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadRight)]
#endif
        DpadRight,

        /// <summary>
        /// Y;别名有北(North)、三角(Triangle)
        /// </summary>
        [Name("Y")]
        [Tip("别名有北(North)、三角(Triangle)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.YButton)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.Y)]
#endif
        Y,

        /// <summary>
        /// B:别名有东(East)、圆(Circle)
        /// </summary>
        [Name("B")]
        [Tip("别名有东(East)、圆(Circle)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.BButton)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.B)]
#endif
        B,

        /// <summary>
        /// A:别名有南(South)、十字(Cross)
        /// </summary>
        [Name("A")]
        [Tip("别名有南(South)、十字(Cross)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.AButton)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.A)]
#endif
        A,

        /// <summary>
        /// X:别名有西(West)、方形(Square)
        /// </summary>
        [Name("X")]
        [Tip("别名有西(West)、方形(Square)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.XButton)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.X)]
#endif
        X,

        /// <summary>
        /// 左摇杆
        /// </summary>
        [Name("左摇杆")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.LeftStickClick)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.LeftStick)]
#endif
        LeftStick,

        /// <summary>
        /// 右摇杆
        /// </summary>
        [Name("右摇杆")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.RightStickClick)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.RightStick)]
#endif
        RightStick,

        /// <summary>
        /// 左减震:别名有左肩(LeftShoulder)
        /// </summary>
        [Name("左减震")]
        [Tip("别名有左肩(LeftShoulder)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.LeftBumper)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.LeftShoulder)]
#endif
        LeftBumper,

        /// <summary>
        /// 右减震:别名有右肩(RightShoulder)
        /// </summary>
        [Name("右减震")]
        [Tip("别名有右肩(RightShoulder)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.RightBumper)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.RightShoulder)]
#endif
        RightBumper,

        /// <summary>
        /// 开始:别名有菜单(Menu)
        /// </summary>
        [Name("开始")]
        [Tip("别名有菜单(Menu)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.StartButton)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.Start)]
#endif
        Start,

        /// <summary>
        /// 返回:别名有选择(Select)、视图(View)
        /// </summary>
        [Name("返回")]
        [Tip("别名有选择(Select)、视图(View)")]
#if UNITY_2018_3_OR_NEWER
        [EXBoxButton(EXBoxButton.BackButton)]
#endif
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.Select)]
#endif
        Back,

        /// <summary>
        /// 左扳机
        /// </summary>
        [Name("左扳机")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.LeftTrigger)]
#endif
        LeftTrigger,

        /// <summary>
        /// 右扳机
        /// </summary>
        [Name("右扳机")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.RightTrigger)]
#endif
        RightTrigger,

        /// <summary>
        /// 左摇杆上
        /// </summary>
        [Name("左摇杆上")]
        LeftStickUp,

        /// <summary>
        /// 左摇杆下
        /// </summary>
        [Name("左摇杆下")]
        LeftStickDown,

        /// <summary>
        /// 左摇杆左
        /// </summary>
        [Name("左摇杆左")]
        LeftStickLeft,

        /// <summary>
        /// 左摇杆右
        /// </summary>
        [Name("左摇杆右")]
        LeftStickRight,

        /// <summary>
        /// 右摇杆上
        /// </summary>
        [Name("右摇杆上")]
        RightStickUp,

        /// <summary>
        /// 右摇杆下
        /// </summary>
        [Name("右摇杆下")]
        RightStickDown,

        /// <summary>
        /// 右摇杆左
        /// </summary>
        [Name("右摇杆左")]
        RightStickLeft,

        /// <summary>
        /// 右摇杆右
        /// </summary>
        [Name("右摇杆右")]
        RightStickRight,
    }

    /// <summary>
    /// XBox控制器轴与按钮特性
    /// </summary>
    public class EXBoxAxisAndButtonAttribute : Attribute
    {
        /// <summary>
        /// XBox控制器轴与按钮
        /// </summary>
        public EXBoxAxisAndButton axisAndButton { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="axisAndButton"></param>
        public EXBoxAxisAndButtonAttribute(EXBoxAxisAndButton axisAndButton) { this.axisAndButton = axisAndButton; }

        /// <summary>
        /// 获取XBox控制器按钮
        /// </summary>
        /// <param name="e"></param>
        /// <param name="defaultEXBoxButton"></param>
        /// <returns></returns>
        public static EXBoxAxisAndButton GetEXBoxButton(Enum e, EXBoxAxisAndButton defaultEXBoxAxisAndButton = EXBoxAxisAndButton.None) => AttributeCache<EXBoxAxisAndButtonAttribute>.GetOfField(e) is EXBoxAxisAndButtonAttribute attribute ? attribute.axisAndButton : defaultEXBoxAxisAndButton;
    }

    /// <summary>
    /// XBox控制器轴与按钮扩展:仅在新版输入系统中可用
    /// </summary>
    public static class EXBoxAxisAndButtonExtension
    {
#if UNITY_2018_3_OR_NEWER

        /// <summary>
        /// 获取轴区域的值
        /// </summary>
        /// <param name="axisArea"></param>
        /// <returns></returns>
        public static Vector2 GetValue(this EXBoxAxisArea axisArea)
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            if (XBoxHelper.current is XInputControllerWindows input)
            {
                switch (axisArea)
                {
                    case EXBoxAxisArea.LeftStick:
                        {
                            return input.leftStick.ReadValue();
                        }
                    case EXBoxAxisArea.RightStick:
                        {
                            return input.rightStick.ReadValue();
                        }
                    case EXBoxAxisArea.DPad:
                        {
                            return input.dpad.ReadValue();
                        }
                    case EXBoxAxisArea.Triggers:
                        {
                            return new Vector2(input.leftTrigger.ReadValue(), input.rightTrigger.ReadValue());
                        }
                }
            }
#else
            switch (axisArea)
            {
                case EXBoxAxisArea.LeftStick:
                    {
                        return new Vector2(EXBoxAxis.LeftStickXAxis.GetAxis(), EXBoxAxis.LeftStickYAxis.GetAxis());
                    }
                case EXBoxAxisArea.RightStick:
                    {
                        return new Vector2(EXBoxAxis.RightStickXAxis.GetAxis(), EXBoxAxis.RightStickYAxis.GetAxis());
                    }
                case EXBoxAxisArea.DPad:
                    {
                        return new Vector2(EXBoxAxis.DPadXAxis.GetAxis(), EXBoxAxis.DPadYAxis.GetAxis());
                    }
                case EXBoxAxisArea.Triggers:
                    {
                        return new Vector2(EXBoxAxis.LeftTrigger.GetAxis(), EXBoxAxis.RightTrigger.GetAxis());
                    }
            }
#endif
            return default;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deadZoneRange"></param>
        /// <returns></returns>
        public static float GetValue(this EXBoxAxisAndButton axisAndButton, Vector2 deadZoneRange)
        {
            var value = axisAndButton.GetValue();
            if (value <= deadZoneRange.x) return 0;
            if (value >= deadZoneRange.y) return 1;
            return Mathf.Clamp01((value - deadZoneRange.x) / (deadZoneRange.y - deadZoneRange.x));
        }

        /// <summary>
        /// 获取轴与按钮的值
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <returns></returns>
        public static float GetValue(this EXBoxAxisAndButton axisAndButton)
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            return axisAndButton.GetButtonControl() is ButtonControl button ? button.ReadValue() : 0;
#else
            switch (axisAndButton)
            {
                case EXBoxAxisAndButton.None: return 0;
                case EXBoxAxisAndButton.LeftTrigger: return EXBoxAxis.LeftTrigger.GetAxis();
                case EXBoxAxisAndButton.RightTrigger: return EXBoxAxis.RightTrigger.GetAxis();
                case EXBoxAxisAndButton.LeftStickUp: return Mathf.Max(0, EXBoxAxis.LeftStickYAxis.GetAxis());
                case EXBoxAxisAndButton.LeftStickDown: return Mathf.Min(0, EXBoxAxis.LeftStickYAxis.GetAxis());
                case EXBoxAxisAndButton.LeftStickLeft: return Mathf.Min(0, EXBoxAxis.LeftStickXAxis.GetAxis());
                case EXBoxAxisAndButton.LeftStickRight: return Mathf.Max(0, EXBoxAxis.LeftStickXAxis.GetAxis());
                case EXBoxAxisAndButton.RightStickUp: return Mathf.Max(0, EXBoxAxis.RightStickYAxis.GetAxis());
                case EXBoxAxisAndButton.RightStickDown: return Mathf.Min(0, EXBoxAxis.RightStickYAxis.GetAxis());
                case EXBoxAxisAndButton.RightStickLeft: return Mathf.Min(0, EXBoxAxis.RightStickXAxis.GetAxis());
                case EXBoxAxisAndButton.RightStickRight: return Mathf.Max(0, EXBoxAxis.RightStickXAxis.GetAxis());
            }
            if (axisAndButton.TryToButton(out EXBoxButton button))
            {
                return button.GetButton() ? 1 : 0;
            }
            return 0;
#endif
        }

        /// <summary>
        /// 判断轴与按钮是否被按压
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="pressedRange">对于摇杆类的值在按压区间内时，则认为按压</param>
        /// <returns></returns>
        public static bool IsPressed(this EXBoxAxisAndButton axisAndButton, Vector2 pressedRange)
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            return axisAndButton.GetButtonControl() is ButtonControl button ? button.isPressed : false;
#else
            if (axisAndButton.TryToButton(out EXBoxButton button))
            {
                return button.GetButton();
            }

            var value = Mathf.Abs(axisAndButton.GetValue());
            return value >= pressedRange.x && value <= pressedRange.y;
#endif
        }

        /// <summary>
        /// 判断轴与按钮是否被按压
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="deadZone">对于摇杆类的值在大于等于死区值时，则认为按压</param>
        /// <returns></returns>
        public static bool IsPressed(this EXBoxAxisAndButton axisAndButton, float deadZone = 0.5f)
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            return axisAndButton.GetButtonControl() is ButtonControl button ? button.isPressed : false;
#else
            if (axisAndButton.TryToButton(out EXBoxButton button))
            {
                return button.GetButton();
            }
            
            return Mathf.Abs(axisAndButton.GetValue()) >= deadZone;
#endif
        }

        /// <summary>
        /// 尝试转按钮
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool TryToButton(this EXBoxAxisAndButton axisAndButton, out EXBoxButton button)
        {
            button = EXBoxButtonCache.GetCacheValue(axisAndButton);
            return button != EXBoxButton.None;
        }

        /// <summary>
        /// 转按钮
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <returns></returns>
        public static EXBoxButton ToButton(this EXBoxAxisAndButton axisAndButton) => EXBoxButtonCache.GetCacheValue(axisAndButton);

        /// <summary>
        /// XBox控制器按钮缓存
        /// </summary>
        class EXBoxButtonCache : TICache<EXBoxButtonCache, EXBoxAxisAndButton, EXBoxButton>
        {
            /// <summary>
            /// 创建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, EXBoxButton> CreateValue(EXBoxAxisAndButton key1)
            {
                return new KeyValuePair<bool, EXBoxButton>(true, EXBoxButtonAttribute.GetEXBoxButton(key1));
            }
        }

#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS

        /// <summary>
        /// 获取按钮控制对象
        /// </summary>
        /// <param name="axisAndButton"></param>
        /// <returns></returns>
        public static ButtonControl GetButtonControl(this EXBoxAxisAndButton axisAndButton)
        {
            return GetButtonControl(XBoxHelper.current, axisAndButton);
        }

        /// <summary>
        /// 获取按钮控制器<see cref="GetButtonControl"/>
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static ButtonControl GetButtonControl(XInputControllerWindows input, EXBoxAxisAndButton axisAndButton)
        {
            if (input == null) return default;
            switch (axisAndButton)
            {
                case EXBoxAxisAndButton.DpadUp: return input.dpad.up;
                case EXBoxAxisAndButton.DpadDown: return input.dpad.down;
                case EXBoxAxisAndButton.DpadLeft: return input.dpad.left;
                case EXBoxAxisAndButton.DpadRight: return input.dpad.right;
                case EXBoxAxisAndButton.Y: return input.yButton;
                case EXBoxAxisAndButton.B: return input.bButton;
                case EXBoxAxisAndButton.A: return input.aButton;
                case EXBoxAxisAndButton.X: return input.xButton;
                case EXBoxAxisAndButton.LeftStick: return input.leftStickButton;
                case EXBoxAxisAndButton.RightStick: return input.rightStickButton;
                case EXBoxAxisAndButton.LeftBumper: return input.leftShoulder;
                case EXBoxAxisAndButton.RightBumper: return input.rightShoulder;
                case EXBoxAxisAndButton.Start: return input.startButton;
                case EXBoxAxisAndButton.Back: return input.selectButton;
                case EXBoxAxisAndButton.LeftTrigger: return input.leftTrigger;
                case EXBoxAxisAndButton.RightTrigger: return input.rightTrigger;
                case EXBoxAxisAndButton.LeftStickUp: return input.leftStick.up;
                case EXBoxAxisAndButton.LeftStickDown: return input.leftStick.down;
                case EXBoxAxisAndButton.LeftStickLeft: return input.leftStick.left;
                case EXBoxAxisAndButton.LeftStickRight: return input.leftStick.right;
                case EXBoxAxisAndButton.RightStickUp: return input.rightStick.up;
                case EXBoxAxisAndButton.RightStickDown: return input.rightStick.down;
                case EXBoxAxisAndButton.RightStickLeft: return input.rightStick.left;
                case EXBoxAxisAndButton.RightStickRight: return input.rightStick.right;
            }
            return default;
        }

#endif

#endif
    }
}
