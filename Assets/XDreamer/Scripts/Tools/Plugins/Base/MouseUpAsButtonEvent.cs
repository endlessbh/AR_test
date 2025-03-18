using System;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 鼠标弹起作为按钮事件
    /// </summary>
    [Name("鼠标弹起作为按钮事件")]
    public class MouseUpAsButtonEvent : InteractProvider
    {
        /// <summary>
        /// 当鼠标弹起作为按钮事件
        /// </summary>
        public static event Action<MouseUpAsButtonEvent> onMouseUpAsButton;

        /// <summary>
        /// 当鼠标弹起作为按钮
        /// </summary>
        private void OnMouseUpAsButton()
        {
            onMouseUpAsButton?.Invoke(this);
        }
    }
}
