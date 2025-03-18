using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.EditorXGUI.Views.Dropdowns
{
    /// <summary>
    /// 下拉框事件触发器检查器
    /// </summary>
    [Name("下拉框事件触发器检查器")]
    [CustomEditor(typeof(DropdownEventTrigger), true)]
    public class DropdownEventTriggerInspector : DropdownMBInspecotr<DropdownEventTrigger>
    {
    }
}
