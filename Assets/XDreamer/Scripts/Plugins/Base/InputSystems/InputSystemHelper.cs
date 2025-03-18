using System;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.Extension.Base.Attributes;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem.LowLevel;
#endif

namespace XCSJ.Extension.Base.InputSystems
{
    /// <summary>
    /// 输入系统辅助类
    /// </summary>
    public static class InputSystemHelper
    {
        /// <summary>
        /// 输入设备类型全名称
        /// </summary>
        public const string InputDeviceTypeFullName = "UnityEngine.InputSystem.InputDevice";

        /// <summary>
        /// 显示输入设备类型全名称
        /// </summary>
        public const string DisplayInputDeviceTypeFullName = "UnityEngine/InputSystem/InputDevice";

        /// <summary>
        /// 输入设备类型
        /// </summary>
        public static Type inputDeviceType => GetInputDeviceType(InputDeviceTypeFullName);

        private static Type[] _inputDeviceTypes;

        /// <summary>
        /// 输入设备类型数组
        /// </summary>
        public static Type[] inputDeviceTypes
        {
            get
            {
                if (_inputDeviceTypes == null)
                {
                    var baseType = TypeCache.Get("UnityEngine.InputSystem.InputDevice");
                    _inputDeviceTypes = baseType != null ? TypeHelper.FindTypeInAppWithClass(baseType, true, true).ToArray() : Empty<Type>.Array;
                }
                return _inputDeviceTypes;
            }
        }

        private static string[] _displayInputDeviceTypeFullNames;

        /// <summary>
        /// 显示输入设备类型全名称数组
        /// </summary>
        public static string[] displayInputDeviceTypeFullNames
        {
            get
            {
                if (_displayInputDeviceTypeFullNames == null)
                {
                    var list = inputDeviceTypes.ToList(type => TypeHelper.FullNameToHierarchyString(type));
                    list.Sort();
                    _displayInputDeviceTypeFullNames = list.ToArray();
                }
                return _displayInputDeviceTypeFullNames;
            }
        }

        /// <summary>
        /// 获取输入设备类型
        /// </summary>
        /// <param name="inputDeviceFullName"></param>
        /// <returns></returns>
        public static Type GetInputDeviceType(string inputDeviceFullName) => TypeCache.Get(inputDeviceFullName);
    }

    /// <summary>
    /// 输入设备类型弹出特性,需要遵守基类特性<see cref="DropdownPopupAttribute"/>的使用规则；
    /// </summary>
    public class InputDeviceTypePopupAttribute: DropdownPopupAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        public InputDeviceTypePopupAttribute() : base(nameof(InputDeviceTypePopupAttribute)) { }
    }

#if XDREAMER_INPUT_SYSTEM

    /// <summary>
    /// 游戏手柄按钮特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class GamepadButtonAttribute : Attribute
    {
        /// <summary>
        /// 按钮
        /// </summary>
        public GamepadButton button { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="button"></param>
        public GamepadButtonAttribute(GamepadButton button) { this.button = button; }

        /// <summary>
        /// 获取键码
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static GamepadButton GetGamepadButton(Enum e, GamepadButton defaultGamepadButton = (GamepadButton)(-1)) => AttributeCache<GamepadButtonAttribute>.GetOfField(e) is GamepadButtonAttribute attribute ? attribute.button : defaultGamepadButton;
    }

#endif
}
