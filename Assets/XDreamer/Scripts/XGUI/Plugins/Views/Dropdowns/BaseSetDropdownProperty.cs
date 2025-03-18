using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 基础设置下拉框属性
    /// </summary>
    [Name("基础设置下拉框属性")]
    public class BaseSetDropdownProperty : DropdownMB, ISetDropdownProperty
    {
    }

    /// <summary>
    /// 设置下拉框属性接口
    /// </summary>
    public interface ISetDropdownProperty : IDropdown
    {
    }
}
