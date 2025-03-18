using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 基础设置下拉框选项
    /// </summary>
    public abstract class BaseSetDropdownOption<T>: BaseSetDropdownProperty<T> where T : BaseSetDropdownOption<T>
    {
        /// <summary>
        /// 选项文本
        /// </summary>
        public virtual string optionText
        {
            get => dropdown.GetTextValue();
            set => dropdown.SetTextValue(value);
        }

        /// <summary>
        /// 选项索引
        /// </summary>
        public virtual int optionIndex
        {
            get => dropdown ? dropdown.value : -1;
            set
            {
                if (dropdown) dropdown.value = value;
            }
        }
    }
}
