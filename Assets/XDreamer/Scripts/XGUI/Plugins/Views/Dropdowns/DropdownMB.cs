using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 下拉框组件
    /// </summary>
    public abstract class DropdownMB : View, IOnEnable
    {
        /// <summary>
        /// 下拉框
        /// </summary>
        [Name("下拉框")]
        [Tip("如当前参数无效，会在启用时从当前组件所在游戏对象上查找本参类期望类型的组件", "If the current parameter is invalid, it will find the component of the expected type of this parameter class from the game object where the current component is located when enabled")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Dropdown))]
        public Dropdown _dropdown;

        /// <summary>
        /// 下拉框
        /// </summary>
        public Dropdown dropdown { get => _dropdown; set => _dropdown = value; }

        /// <summary>
        /// 启用时执行
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!_dropdown) _dropdown = GetComponent<Dropdown>();
        }
    }

    /// <summary>
    /// 下拉框接口
    /// </summary>
    public interface IDropdown
    {
        /// <summary>
        /// 下拉框
        /// </summary>
        Dropdown dropdown { get; set; }
    }
}
