using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorRepairman.Inspectors;
using Tool = XCSJ.PluginRepairman.States.Tool;

namespace XCSJ.EditorRepairman.States
{
    /// <summary>
    /// 工具检查器
    /// </summary>
    [Name("工具检查器")]
    [CustomEditor(typeof(Tool), true)]
    public class ToolInspector : ItemInspector
    {

    }
}
