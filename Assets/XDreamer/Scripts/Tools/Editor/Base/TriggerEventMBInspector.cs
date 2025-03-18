using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.Base;

namespace XCSJ.EditorTools.Base
{
    /// <summary>
    /// 触发事件组件检查器
    /// </summary>
    [Name("触发事件组件检查器")]
    [CustomEditor(typeof(TriggerEventMB), true)]
    public class TriggerEventMBInspector : TriggerEventMBInspector<TriggerEventMB>
    {
    }

    /// <summary>
    /// 触发事件组件检查器泛型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TriggerEventMBInspector<T> : MBInspector<T> where T : TriggerEventMB
    {
    }
}
