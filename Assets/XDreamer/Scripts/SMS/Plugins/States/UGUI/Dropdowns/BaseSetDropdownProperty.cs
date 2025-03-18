using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.XGUI.Dropdowns;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 基础设置下拉框属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSetDropdownProperty<T> : LifecycleExecutor<T> , ISetDropdownProperty
        where T : BaseSetDropdownProperty<T>
    {
        /// <summary>
        /// 下拉框
        /// </summary>
        [Name("下拉框")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Dropdown))]
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
