using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Algorithms;
using System.Collections.Generic;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
#endif

namespace XCSJ.PluginXBox.Base
{
    /// <summary>
    /// XBox控制器轴枚举：详情参考<seealso cref="http://wiki.unity3d.com/index.php?title=Xbox360Controller#Axises"/>，可用于对接旧版输入管理器；
    /// </summary>
    [Name("XBox控制器轴")]
    public enum EXBoxAxis
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [EnumFieldName("无")]
        [Tip("不做任何检测的轴")]
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        None,

        /// <summary>
        /// 左摇杆X轴
        /// </summary>
        [Name("左摇杆X轴")]
        [EnumFieldName("左摇杆X轴")]
        [Tip("对应左摇杆X轴（Left Stick X Axis）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton0)]
        [JoystickButtonID(0)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton0)]
        [JoystickButtonID(0)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton0)]
        [JoystickButtonID(0)]
#endif
        LeftStickXAxis,

        /// <summary>
        /// 左摇杆Y轴
        /// </summary>
        [Name("左摇杆Y轴")]
        [EnumFieldName("左摇杆Y轴")]
        [Tip("对应左摇杆Y轴（Left Stick Y Axis）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton1)]
        [JoystickButtonID(1)]
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton1)]
        [JoystickButtonID(1)]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton1)]
        [JoystickButtonID(1)]
#endif
        LeftStickYAxis,

        /// <summary>
        /// 右摇杆X轴
        /// </summary>
        [Name("右摇杆X轴")]
        [EnumFieldName("右摇杆X轴")]
        [Tip("对应右摇杆X轴（Right Stick X Axis）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton4)]
        [JoystickButtonID(3)]//4
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton3)]
        [JoystickButtonID(2)]//3
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton4)]
        [JoystickButtonID(3)]//4
#endif
        RightStickXAxis,

        /// <summary>
        /// 右摇杆Y轴
        /// </summary>
        [Name("右摇杆Y轴")]
        [EnumFieldName("右摇杆Y轴")]
        [Tip("对应右摇杆Y轴（Right Stick Y Axis）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton5)]
        [JoystickButtonID(4)]//5
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton4)]
        [JoystickButtonID(3)]//4
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton5)]
        [JoystickButtonID(4)]//5
#endif
        RightStickYAxis,

        /// <summary>
        /// D-Pad X轴
        /// </summary>
        [Name("D-Pad X轴")]
        [EnumFieldName("D-Pad X轴")]
        [Tip("对应D-Pad X轴（D-Pad X Axis）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton6)]
        [JoystickButtonID(5)]//6
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        [Obsolete("当前平台不支持本轴")]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton7)]
        [JoystickButtonID(6)]//7
#endif
        DPadXAxis,

        /// <summary>
        /// D-Pad Y轴
        /// </summary>
        [Name("D-Pad Y轴")]
        [EnumFieldName("D-Pad Y轴")]
        [Tip("对应D-Pad Y轴（D-Pad Y Axis）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton7)]
        [JoystickButtonID(6)]//7
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        [Obsolete("当前平台不支持本轴")]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton8)]
        [JoystickButtonID(7)]//8
#endif
        DPadYAxis,

        /// <summary>
        /// 扳机
        /// </summary>
        [Name("扳机")]
        [EnumFieldName("扳机")]
        [Tip("对应扳机（Triggers）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton3)]
        [JoystickButtonID(2)]//3
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        [Obsolete("当前平台不支持本轴")]
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.None)]
        [JoystickButtonID()]
        [Obsolete("当前平台不支持本轴")]
#endif
        Triggers,

        /// <summary>
        /// 左扳机
        /// </summary>
        [Name("左扳机")]
        [EnumFieldName("左扳机")]
        [Tip("对应左扳机（Left Trigger）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton9)]
        [JoystickButtonID(8)]//9
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton5)]
        [JoystickButtonID(4)]//5
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton3)]
        [JoystickButtonID(2)]//3
#endif
        LeftTrigger,

        /// <summary>
        /// 右扳机
        /// </summary>
        [Name("右扳机")]
        [EnumFieldName("右扳机")]
        [Tip("对应左扳机（Right Trigger）")]
#if UNITY_STANDALONE_WIN
        [KeyCode(KeyCode.JoystickButton10)]
        [JoystickButtonID(9)]//10
#elif UNITY_STANDALONE_OSX
        [KeyCode(KeyCode.JoystickButton6)]
        [JoystickButtonID(5)]//6
#elif UNITY_STANDALONE_LINUX
        [KeyCode(KeyCode.JoystickButton6)]
        [JoystickButtonID(5)]//6
#endif
        RightTrigger,
    }

    /// <summary>
    /// XBox控制器轴扩展
    /// </summary>
    [Name("XBox控制器轴扩展")]
    public static class XBoxAxisExtension
    {
        /// <summary>
        /// 获取轴的值
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static float GetAxis(this EXBoxAxis axis)
        {
#if XDREAMER_INPUT_SYSTEM
            switch (axis)
            {
                case EXBoxAxis.LeftStickXAxis:
                    {
                        return EXBoxAxisAndButton.LeftStickLeft.GetValue() + EXBoxAxisAndButton.LeftStickRight.GetValue();
                    }
                case EXBoxAxis.LeftStickYAxis:
                    {
                        return EXBoxAxisAndButton.LeftStickUp.GetValue() + EXBoxAxisAndButton.LeftStickDown.GetValue();
                    }
                case EXBoxAxis.RightStickXAxis:
                    {
                        return EXBoxAxisAndButton.RightStickLeft.GetValue() + EXBoxAxisAndButton.RightStickRight.GetValue();
                    }
                case EXBoxAxis.RightStickYAxis:
                    {
                        return EXBoxAxisAndButton.RightStickUp.GetValue() + EXBoxAxisAndButton.RightStickDown.GetValue();
                    }
                case EXBoxAxis.DPadXAxis:
                    {
                        return EXBoxAxisAndButton.DpadLeft.GetValue() + EXBoxAxisAndButton.DpadRight.GetValue();
                    }
                case EXBoxAxis.DPadYAxis:
                    {
                        return EXBoxAxisAndButton.DpadUp.GetValue() + EXBoxAxisAndButton.DpadDown.GetValue();
                    }
                case EXBoxAxis.Triggers:
                    {
                        return EXBoxAxisAndButton.LeftTrigger.GetValue() + EXBoxAxisAndButton.RightTrigger.GetValue();
                    }
                case EXBoxAxis.LeftTrigger:
                    {
                        return EXBoxAxisAndButton.LeftTrigger.GetValue();
                    }
                case EXBoxAxis.RightTrigger:
                    {
                        return EXBoxAxisAndButton.RightTrigger.GetValue();
                    }
            }
            return 0;
#else
            return Input.GetAxis(axis.ToString());
#endif
        }

        /// <summary>
        /// 判断是否是有效轴
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static bool IsValidAxis(this EXBoxAxis axis) => JoystickButtonIDAttribute.GetJoystickButtonID(axis) >= 0;

        /// <summary>
        /// 转键码<see cref="KeyCode"/>
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static KeyCode ToKeyCode(this EXBoxAxis axis) => KeyCodeCache.GetCacheValue(axis);

        /// <summary>
        /// 键码缓存
        /// </summary>
        class KeyCodeCache : TICache<KeyCodeCache, EXBoxAxis, KeyCode>
        {
            /// <summary>
            /// 创建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, KeyCode> CreateValue(EXBoxAxis key1)
            {
                return new KeyValuePair<bool, KeyCode>(true, KeyCodeAttribute.GetKeyCode(key1));
            }
        }

        /// <summary>
        /// 转操纵杆按键ID
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public static int ToJoystickButtonID(this EXBoxAxis axis) => JoystickButtonIDCache.GetCacheValue(axis);

        /// <summary>
        /// 操纵杆按键ID缓存
        /// </summary>
        class JoystickButtonIDCache : TICache<JoystickButtonIDCache, EXBoxAxis, int>
        {
            /// <summary>
            /// 创建值
            /// </summary>
            /// <param name="key1"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, int> CreateValue(EXBoxAxis key1)
            {
                return new KeyValuePair<bool, int>(true, JoystickButtonIDAttribute.GetJoystickButtonID(key1));
            }
        }
    }
}
