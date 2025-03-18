using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Transitions.CNScripts;

namespace XCSJ.EditorSMS.Transitions.CNScripts
{
    /// <summary>
    /// 生命周期事件检查器
    /// </summary>
    [Name("生命周期事件检查器")]
    [CustomEditor(typeof(LifecycleEvent))]
    public class LifecycleEventInspector : TransitionScriptComponentInspector<LifecycleEvent, ELifecycleEvent, LifecycleEventFunction, LifecycleEventFunctionCollection>
    {
    }
}
