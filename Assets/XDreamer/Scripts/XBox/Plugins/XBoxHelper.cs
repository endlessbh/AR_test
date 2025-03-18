using System.Linq;
using UnityEngine;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginXBox.Base;
using System.Collections.Generic;
using XCSJ.Extension.Base.Extensions;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
#endif

namespace XCSJ.PluginXBox
{
    /// <summary>
    /// XBox辅助类
    /// </summary>
    public static class XBoxHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XBox";

#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS

        /// <summary>
        /// 获取当前XBox设备
        /// </summary>
        public static XInputControllerWindows current => InputSystem.GetDevice<XInputControllerWindows>();

        /// <summary>
        /// 获取XBox设备
        /// </summary>
        /// <returns></returns>
        public static XInputControllerWindows GetXBoxDevice() => current;

        /// <summary>
        /// 获取XBox设备枚举
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<XInputControllerWindows> GetXBoxDevices() => InputSystem.devices.WhereCast(d => (d is XInputControllerWindows, d as XInputControllerWindows));

#endif

        /// <summary>
        /// 检测是否存在XBox360手柄设备；
        /// </summary>
        /// <returns></returns>
        public static bool HasXBox360()
        {
#if XDREAMER_INPUT_SYSTEM_XBOX_WINDOWS
            if (current != null) return true;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER || !XDREAMER_INPUT_SYSTEM
            if (Input.GetJoystickNames().Any(name => name.Contains("Xbox 360"))) return true;
#endif
            return false;
        }
    }   
}
