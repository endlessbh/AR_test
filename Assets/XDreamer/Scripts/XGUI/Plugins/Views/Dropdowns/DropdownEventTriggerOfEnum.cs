using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 枚举型下拉框事件触发器：用于捕获下拉框的值变更事件，并根据不同值做不同的事件处理；
    /// </summary>
    [Name(Title, nameof(DropdownEventTrigger))]
    [Tip("用于捕获下拉框的值变更事件，并根据不同值做不同的事件处理；", "It is used to capture the value change event of the drop-down box and handle different events according to different values;")]
    public class DropdownEventTriggerOfEnum : BaseDropdownEventTriggerOfEnum
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "枚举型下拉框事件触发器";

        /// <summary>
        /// 自定义枚举型下拉框事件列表
        /// </summary>
        [Name("自定义枚举型下拉框事件列表")]
        public List<CustomDropdownEventOfEnum> _customDropdownEventOfEnums = new List<CustomDropdownEventOfEnum>();

        /// <summary>
        /// 自定义基础下拉框事件列表
        /// </summary>
        public override IEnumerable<BaseCustomDropdownEvent> customDropdownEvents => _customDropdownEventOfEnums.Cast(e => (BaseCustomDropdownEvent)e);

        /// <summary>
        /// 自定义枚举型下拉框事件列表
        /// </summary>
        public override IEnumerable<BaseCustomDropdownEventOfEnum> customDropdownEventOfEnums => _customDropdownEventOfEnums.Cast(e => (BaseCustomDropdownEventOfEnum)e);
    }
}
