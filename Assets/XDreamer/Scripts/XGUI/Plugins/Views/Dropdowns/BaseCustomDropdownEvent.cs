using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 基础自定义下拉框事件
    /// </summary>
    public abstract class BaseCustomDropdownEvent : UnityEvent<string>
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        public abstract string displayText { get; }

        /// <summary>
        /// 判断能否执行：检测下拉框的选择文本与显示文本<see cref="displayText"/>是否相等；
        /// </summary>
        /// <param name="dropdown"></param>
        /// <returns></returns>
        public virtual bool CanInvoke(Dropdown dropdown)
        {
            if (!dropdown) return false;
            return dropdown.GetTextValue() == displayText;
        }

        /// <summary>
        /// 尝试执行：如果符合条件则执行,即判断<see cref="CanInvoke"/>能否执行，如成立则调用执行；
        /// </summary>
        /// <param name="dropdown"></param>
        /// <returns></returns>
        public virtual bool TryInvoke(Dropdown dropdown)
        {
            if (CanInvoke(dropdown))
            {
                Invoke(displayText);
                return true;
            }
            return false;
        }
    }
}
