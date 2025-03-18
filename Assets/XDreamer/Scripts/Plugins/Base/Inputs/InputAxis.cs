using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Inputs
{
    [Name("输入轴")]
    [Serializable]
    public class InputAxis
    {
        [Name("输入名称")]
        [Tip("期望处理的输入名称", "Input name expected to be processed")]
        [Input]
        public string _inputName = "";

        [Name("鼠标按键列表")]
        [Tip("列表中任意一项满足鼠标按键事件成立条件，则可处理输入；如果列表为空或是无一项满足鼠标按键事件成立条件，则不能处理输入；", "If any item in the list meets the establishment conditions of mouse button event, the input can be processed; If the list is empty or none of the items meets the conditions for mouse button events, the input cannot be processed;")]
        [EnumPopup(typeof(EMouseButton))]
        public List<EMouseButton> _mouseButtons = new List<EMouseButton>();

        /// <summary>
        /// 能否输入
        /// </summary>
        /// <returns></returns>
        public bool CanInput(IInput input)
        {
            return !string.IsNullOrEmpty(_inputName) && _mouseButtons.GetAnyMouseButton(input);
        }
    }

    /// <summary>
    /// 键码特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class KeyCodeAttribute : Attribute
    {
        /// <summary>
        /// 键码
        /// </summary>
        public KeyCode keyCode { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="keyCode"></param>
        public KeyCodeAttribute(KeyCode keyCode) { this.keyCode = keyCode; }

        /// <summary>
        /// 获取键码
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static KeyCode GetKeyCode(Enum e, KeyCode defaultKeyCode = KeyCode.None) => AttributeCache<KeyCodeAttribute>.GetOfField(e) is KeyCodeAttribute attribute ? attribute.keyCode : defaultKeyCode;
    }

    /// <summary>
    /// 操纵杆按键ID特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class JoystickButtonIDAttribute : Attribute
    {
        /// <summary>
        /// 操纵杆按键ID
        /// </summary>
        public int id { get; private set; } = -1;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="id"></param>
        public JoystickButtonIDAttribute(int id = -1) { this.id = id; }

        /// <summary>
        /// 操纵杆ID
        /// </summary>
        public int joystickID { get; set; } = -1;

        /// <summary>
        /// 获取操纵杆按键ID
        /// </summary>
        /// <param name="e"></param>
        /// <param name="defaultID"></param>
        /// <returns></returns>
        public static int GetJoystickButtonID(Enum e, int defaultID = -1) => AttributeCache<JoystickButtonIDAttribute>.GetOfField(e) is JoystickButtonIDAttribute attribute ? attribute.id : defaultID;

        /// <summary>
        /// 尝试获取操纵杆按键ID
        /// </summary>
        /// <param name="e"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool TryGetJoystickButtonID(Enum e, out int id)
        {
            if (AttributeCache<JoystickButtonIDAttribute>.GetOfField(e) is JoystickButtonIDAttribute attribute)
            {
                id = attribute.id;
                return true;
            }
            id = default;
            return false;
        }

        /// <summary>
        /// 尝试获取ID
        /// </summary>
        /// <param name="e"></param>
        /// <param name="id">操纵杆按键ID</param>
        /// <param name="joystickID">操纵杆ID</param>
        /// <returns></returns>
        public static bool TryID(Enum e, out int id, out int joystickID)
        {
            if (AttributeCache<JoystickButtonIDAttribute>.GetOfField(e) is JoystickButtonIDAttribute attribute)
            {
                id = attribute.id;
                joystickID = attribute.joystickID;
                return true;
            }
            id = default;
            joystickID = default;
            return false;
        }
    }
}
