using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 鼠标按钮脚本事件类型
    /// </summary>
    [Name("鼠标按钮脚本事件类型")]
    public enum EMouseButtonScriptEventType
    {
        /// <summary>
        /// 左键按下时
        /// </summary>
        [Name("左键按下时执行")]
        LeftDown,

        /// <summary>
        /// 左键按下中时
        /// </summary>
        [Name("左键按下中时执行")]
        Left,

        /// <summary>
        /// 左键弹起时
        /// </summary>
        [Name("左键弹起时执行")]
        LeftUp,

        /// <summary>
        /// 右键按下时
        /// </summary>
        [Name("右键按下时执行")]
        RightDown,

        /// <summary>
        /// 右键按下中时
        /// </summary>
        [Name("右键按下中时执行")]
        Right,

        /// <summary>
        /// 右键弹起时
        /// </summary>
        [Name("右键弹起时执行")]
        RightUp,

        /// <summary>
        /// 中键按下时
        /// </summary>
        [Name("中键按下时执行")]
        MiddleDown,

        /// <summary>
        /// 中键按下中时
        /// </summary>
        [Name("中键按下中时执行")]
        Middle,

        /// <summary>
        /// 中键弹起时
        /// </summary>
        [Name("中键弹起时执行")]
        MiddleUp,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// 鼠标按钮脚本事件函数
    /// </summary>
    [Name("鼠标按钮脚本事件函数")]
    [Serializable]
    public class MouseButtonScriptEventFunction : EnumFunction<EMouseButtonScriptEventType> { }

    /// <summary>
    /// 鼠标按钮脚本事件函数集合
    /// </summary>
    [Name("鼠标按钮脚本事件函数集合")]
    [Serializable]
    public class MouseButtonScriptEventFunctionCollection : EnumFunctionCollection<EMouseButtonScriptEventType, MouseButtonScriptEventFunction> { }

    /// <summary>
    /// 脚本MouseButton事件集合类
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class MouseButtonScriptEvent : BaseScriptEvent<EMouseButtonScriptEventType, MouseButtonScriptEventFunction, MouseButtonScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "鼠标按钮脚本事件";

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(EMouseButtonScriptEventType.Start);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (!Input.anyKey && !Input.anyKeyDown) return;

            if (Input.GetMouseButtonDown((int)EMouseButtonType.Left))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.LeftDown);
            }
            else if (Input.GetMouseButton((int)EMouseButtonType.Left))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.Left);
            }
            else if (Input.GetMouseButtonUp((int)EMouseButtonType.Left))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.LeftUp);
            }

            if (Input.GetMouseButtonDown((int)EMouseButtonType.Right))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.RightDown);
            }
            else if (Input.GetMouseButton((int)EMouseButtonType.Right))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.Right);
            }
            else if (Input.GetMouseButtonUp((int)EMouseButtonType.Right))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.RightUp);
            }

            if (Input.GetMouseButtonDown((int)EMouseButtonType.Middle))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.MiddleDown);
            }
            else if (Input.GetMouseButton((int)EMouseButtonType.Middle))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.Middle);
            }
            else if (Input.GetMouseButtonUp((int)EMouseButtonType.Middle))
            {
                ExecuteScriptEvent(EMouseButtonScriptEventType.MiddleUp);
            }
        }
    }
}
