using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Algorithms;
using System.Collections.Generic;
using XCSJ.Caches;
using XCSJ.Extension.Base.InputSystems;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput;
#endif

namespace XCSJ.PluginXBox.Base
{
    /// <summary>
    /// XBox控制器按钮枚举：详情参考<seealso cref="http://wiki.unity3d.com/index.php?title=Xbox360Controller#Buttons"/>
    /// </summary>
    [Name("XBox控制器按钮")]
    public enum EXBoxButton
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [EnumFieldName("无")]
        [Tip("不做任何检测的按钮")]
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        None,

        /// <summary>
        /// A按钮
        /// </summary>
        [Name("A按钮")]
        [EnumFieldName("A按钮")]
        [Tip("对应A按钮（A Button）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.A)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton0)]
        [JoystickButtonID(0)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton16)]
        [JoystickButtonID(16)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton0)]
        [JoystickButtonID(0)]
#endif
        AButton,

        /// <summary>
        /// B按钮
        /// </summary>
        [Name("B按钮")]
        [EnumFieldName("B按钮")]
        [Tip("对应B按钮（B Button）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.B)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton1)]
        [JoystickButtonID(1)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton17)]
        [JoystickButtonID(17)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton1)]
        [JoystickButtonID(1)]
#endif
        BButton,

        /// <summary>
        /// X按钮
        /// </summary>
        [Name("X按钮")]
        [EnumFieldName("X按钮")]
        [Tip("对应X按钮（X Button）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.X)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton2)]
        [JoystickButtonID(2)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton18)]
        [JoystickButtonID(18)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton2)]
        [JoystickButtonID(2)]
#endif
        XButton,

        /// <summary>
        /// Y按钮
        /// </summary>
        [Name("Y按钮")]
        [EnumFieldName("Y按钮")]
        [Tip("对应Y按钮（Y Button）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.Y)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton3)]
        [JoystickButtonID(3)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton19)]
        [JoystickButtonID(19)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton3)]
        [JoystickButtonID(3)]
#endif
        YButton,

        /// <summary>
        /// 左减震
        /// </summary>
        [Name("左减震")]
        [EnumFieldName("左减震")]
        [Tip("对应左减震按钮（Left Bumper）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.LeftShoulder)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton4)]
        [JoystickButtonID(4)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton13)]
        [JoystickButtonID(13)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton4)]
        [JoystickButtonID(4)]
#endif
        LeftBumper,

        /// <summary>
        /// 右减震
        /// </summary>
        [Name("右减震")]
        [EnumFieldName("右减震")]
        [Tip("对应右减震按钮（Right Bumper）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.RightShoulder)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton5)]
        [JoystickButtonID(5)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton14)]
        [JoystickButtonID(14)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton5)]
        [JoystickButtonID(5)]
#endif
        RightBumper,

        /// <summary>
        /// 返回按钮
        /// </summary>
        [Name("返回按钮")]
        [EnumFieldName("返回按钮")]
        [Tip("对应返回按钮（Back Button）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.Select)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton6)]
        [JoystickButtonID(6)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton10)]
        [JoystickButtonID(10)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton6)]
        [JoystickButtonID(6)]
#endif
        BackButton,

        /// <summary>
        /// 开始按钮
        /// </summary>
        [Name("开始按钮")]
        [EnumFieldName("开始按钮")]
        [Tip("对应开始按钮（Start Button）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.Start)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton7)]
        [JoystickButtonID(7)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton9)]
        [JoystickButtonID(9)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton7)]
        [JoystickButtonID(7)]
#endif
        StartButton,

        /// <summary>
        /// 左杆点击
        /// </summary>
        [Name("左杆点击")]
        [EnumFieldName("左杆点击")]
        [Tip("对应左杆点击按钮（Left Stick Click）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.LeftStick)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton8)]
        [JoystickButtonID(8)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton11)]
        [JoystickButtonID(11)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton9)]
        [JoystickButtonID(9)]
#endif
        LeftStickClick,

        /// <summary>
        /// 左杆点击
        /// </summary>
        [Name("左杆点击")]
        [EnumFieldName("左杆点击")]
        [Tip("对应左杆点击按钮（Right Stick Click）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.RightStick)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton9)]
        [JoystickButtonID(9)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton12)]
        [JoystickButtonID(12)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton10)]
        [JoystickButtonID(10)]
#endif
        RightStickClick,

        /// <summary>
        /// D-Pad上
        /// </summary>
        [Name("D-Pad上")]
        [EnumFieldName("D-Pad上")]
        [Tip("对应D-Pad上按钮（D-Pad Up）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadUp)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
//#if !XDREAMER_INPUT_SYSTEM
//        [Obsolete("旧版输入管理器在当前平台下，不支持本按钮功能！推荐启用输入系统！")]
//#endif
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton5)]
        [JoystickButtonID(5)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton13)]
        [JoystickButtonID(13)]
#endif
        DPadUp,

        /// <summary>
        /// D-Pad下
        /// </summary>
        [Name("D-Pad下")]
        [EnumFieldName("D-Pad下")]
        [Tip("对应D-Pad下按钮（D-Pad Down）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadDown)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
//#if !XDREAMER_INPUT_SYSTEM
//        [Obsolete("旧版输入管理器在当前平台下，不支持本按钮功能！推荐启用输入系统！")]
//#endif
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton6)]
        [JoystickButtonID(6)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton14)]
        [JoystickButtonID(14)]
#endif
        DPadDown,

        /// <summary>
        /// D-Pad左
        /// </summary>
        [Name("D-Pad左")]
        [EnumFieldName("D-Pad左")]
        [Tip("对应D-Pad左按钮（D-Pad Left）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadLeft)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
//#if !XDREAMER_INPUT_SYSTEM
//        [Obsolete("旧版输入管理器在当前平台下，不支持本按钮功能！推荐启用输入系统！")]
//#endif
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton7)]
        [JoystickButtonID(7)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton11)]
        [JoystickButtonID(11)]
#endif
        DPadLeft,

        /// <summary>
        /// D-Pad右
        /// </summary>
        [Name("D-Pad右")]
        [EnumFieldName("D-Pad右")]
        [Tip("对应D-Pad右按钮（D-Pad Right）")]
#if XDREAMER_INPUT_SYSTEM
        [GamepadButton(GamepadButton.DpadRight)]
#endif
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
//#if !XDREAMER_INPUT_SYSTEM
//        [Obsolete("旧版输入管理器在当前平台下，不支持本按钮功能！推荐启用输入系统！")]
//#endif
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton8)]
        [JoystickButtonID(8)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton12)]
        [JoystickButtonID(12)]
#endif
        DPadRight,

        /// <summary>
        /// Xbox按钮
        /// </summary>
        [Name("Xbox按钮")]
        [EnumFieldName("Xbox按钮")]
        [Tip("对应Xbox按钮（Xbox Button）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        [Obsolete("当前平台不支持本按钮")]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton15)]
        [JoystickButtonID(15)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        [Obsolete("当前平台不支持本按钮")]
#endif
        XBoxButton,
    }

    /// <summary>
    /// XBox控制器按钮特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class EXBoxButtonAttribute : Attribute
    {
        /// <summary>
        /// 按钮
        /// </summary>
        public EXBoxButton button { get; private set; }

        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="button"></param>
        public EXBoxButtonAttribute(EXBoxButton button)
        {
            this.button = button;
        }

        /// <summary>
        /// 获取XBox控制器按钮
        /// </summary>
        /// <param name="e"></param>
        /// <param name="defaultEXBoxButton"></param>
        /// <returns></returns>
        public static EXBoxButton GetEXBoxButton(Enum e, EXBoxButton defaultEXBoxButton = EXBoxButton.None) => AttributeCache<EXBoxButtonAttribute>.GetOfField(e) is EXBoxButtonAttribute attribute ? attribute.button : defaultEXBoxButton;

        /// <summary>
        /// 尝试获取XBox控制器按钮
        /// </summary>
        /// <param name="e"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool TryGetEXBoxButton(Enum e,out EXBoxButton button)
        {
            if (AttributeCache<EXBoxButtonAttribute>.GetOfField(e) is EXBoxButtonAttribute attribute)
            {
                button = attribute.button;
                return button != EXBoxButton.None;
            }
            button = default;
            return false;
        }
    }

    /// <summary>
    /// XBox控制器按钮扩展
    /// </summary>
    public static class EXBoxButtonExtension
    {
        /// <summary>
        /// 获取按钮是否按下中
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool GetButton(this EXBoxButton button)
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            if (XBoxHelper.current is XInputControllerWindows input)
            {
                try
                {
                    return input[button.ToGamepadButton()].isPressed;
                }
                catch { }
            }
            return false;
#else
            return Input.GetButton(button.ToString());
#endif
        }

        /// <summary>
        /// 判断是否是有效按钮
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool IsValidButton(this EXBoxButton button) => JoystickButtonIDAttribute.GetJoystickButtonID(button) >= 0;

#if XDREAMER_INPUT_SYSTEM

        /// <summary>
        /// 转游戏手柄按钮<see cref="GamepadButton"/>
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static GamepadButton ToGamepadButton(this EXBoxButton button) => GamepadButtonCache.GetCacheValue(button);

        /// <summary>
        /// 游戏手柄按钮缓存
        /// </summary>
        class GamepadButtonCache : TICache<GamepadButtonCache, EXBoxButton, GamepadButton>
        {
            /// <summary>
            /// 创建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, GamepadButton> CreateValue(EXBoxButton key1)
            {
                return new KeyValuePair<bool, GamepadButton>(true, GamepadButtonAttribute.GetGamepadButton(key1));
            }
        }

#endif

        /// <summary>
        /// 转键码<see cref="KeyCode"/>
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static KeyCode ToKeyCode(this EXBoxButton button) => KeyCodeCache.GetCacheValue(button);

        /// <summary>
        /// 键码缓存
        /// </summary>
        class KeyCodeCache : TICache<KeyCodeCache, EXBoxButton, KeyCode>
        {
            /// <summary>
            /// 创建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, KeyCode> CreateValue(EXBoxButton key1)
            {
                return new KeyValuePair<bool, KeyCode>(true, KeyCodeAttribute.GetKeyCode(key1));
            }
        }

        /// <summary>
        /// 转操纵杆按键ID
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static int ToJoystickButtonID(this EXBoxButton button) => JoystickButtonIDCache.GetCacheValue(button);

        /// <summary>
        /// 操纵杆按键ID缓存
        /// </summary>
        class JoystickButtonIDCache : TICache<JoystickButtonIDCache, EXBoxButton, int>
        {
            /// <summary>
            /// 创建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, int> CreateValue(EXBoxButton key1)
            {
                return new KeyValuePair<bool, int>(true, JoystickButtonIDAttribute.GetJoystickButtonID(key1));
            }
        }
    }
}
