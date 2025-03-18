using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.Extension.XGUI.Dropdowns
{
    /// <summary>
    /// 基础下拉框事件触发器
    /// </summary>
    public abstract class BaseDropdownEventTrigger : DropdownMB, IOnDisable
    {
        /// <summary>
        /// 自定义基础下拉框事件列表
        /// </summary>
        public abstract IEnumerable<BaseCustomDropdownEvent> customDropdownEvents { get; }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (dropdown)
            {
                dropdown.onValueChanged.AddListener(OnValueChanged);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (dropdown)
            {
                dropdown.onValueChanged.RemoveListener(OnValueChanged);
            }
        }

        /// <summary>
        /// 值变更时回调
        /// </summary>
        /// <param name="value"></param>
        protected virtual void OnValueChanged(int value)
        {
            customDropdownEvents.Foreach(e => e.TryInvoke(dropdown));
        }
    }
}
