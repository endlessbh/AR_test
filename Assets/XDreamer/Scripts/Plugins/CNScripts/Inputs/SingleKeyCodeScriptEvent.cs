using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 脚本单一按键脚本事件类型
    /// </summary>
    [Name("脚本单一按键脚本事件类型")]
    public enum ESingleKeyCodeScriptEventType
    {
        /// <summary>
        /// 按下时
        /// </summary>
        [Name("按下时执行")]
        Down,

        /// <summary>
        /// 按下中时
        /// </summary>
        [Name("按下中时执行")]
        Pressed,

        /// <summary>
        /// 弹起时
        /// </summary>
        [Name("弹起时执行")]
        Up,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 脚本单一按键脚本事件函数
    /// </summary>
    [Name("脚本单一按键脚本事件函数")]
    [Serializable]
    public class SingleKeyCodeScriptEventFunction : EnumFunction<ESingleKeyCodeScriptEventType> { }

    /// <summary>
    /// 脚本单一按键脚本事件函数集合
    /// </summary>
    [Name("脚本单一按键脚本事件函数集合")]
    [Serializable]
    public class SingleKeyCodeScriptEventFunctionCollection : EnumFunctionCollection<ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventFunction> { }

    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class SingleKeyCodeScriptEvent : BaseScriptEvent<ESingleKeyCodeScriptEventType, SingleKeyCodeScriptEventFunction, SingleKeyCodeScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "单一按键脚本事件";

        /// <summary>
        /// 按键码
        /// </summary>
        [Name("按键码")]
        [Tip("单一按键码脚本事件检测的按键码类型", "Key code type of single key code script event detection")]
        public KeyCode keyCode = KeyCode.None;

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Start);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (Input.GetKeyDown(keyCode))
            {
                ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Down, keyCode.ToString());
            }
            else if (Input.GetKey(keyCode))
            {
                ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Pressed, keyCode.ToString());
            }
            else if (Input.GetKeyUp(keyCode))
            {
                ExecuteScriptEvent(ESingleKeyCodeScriptEventType.Up, keyCode.ToString());
            }
        }
    }
}
