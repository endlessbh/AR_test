using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 组件启用区间检查器
    /// </summary>
    [Name("组件启用区间检查器")]
    [CustomEditor(typeof(ComponentEnabledRange))]
    public class ComponentEnabledRangeInspector : RangeHandleInspector<ComponentEnabledRange>
    {
    }
}
