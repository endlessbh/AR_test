using System;
using System.ComponentModel;
using UnityEngine.UI;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.XGUI.Dropdowns;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 下拉框切换：触发型状态组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseDropdownSwitch<T> : Trigger<DropdownSwitch> where T : BaseDropdownSwitch<T>
    {
        /// <summary>
        /// 下拉框
        /// </summary>
        [Name("下拉框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Dropdown))]
        [FormerlySerializedAs(nameof(dropdown))]
        [Readonly(EEditorMode.Runtime)]
        public Dropdown _dropdown;

        /// <summary>
        /// 下拉框
        /// </summary>
        public Dropdown dropdown { get => _dropdown; set => _dropdown = value; }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && dropdown;

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => dropdown ? dropdown.name : "";
    }
}
