using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorXGUI.Base;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.EditorXGUI.Views.Dropdowns
{
    /// <summary>
    /// 下拉框组件检查器
    /// </summary>
    [Name("下拉框组件检查器")]
    [CustomEditor(typeof(DropdownMB), true)]
    public class DropdownMBInspecotr : DropdownMBInspecotr<DropdownMB> { }

    /// <summary>
    /// 下拉框组件检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DropdownMBInspecotr<T> : ViewInspector<T> where T : DropdownMB
    {
    }
}
