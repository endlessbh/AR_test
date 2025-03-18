using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.Transitions.CNScripts;

namespace XCSJ.EditorSMS.Transitions.CNScripts
{
    /// <summary>
    /// 生命周期事件简版检查器
    /// </summary>
    [Name("生命周期事件简版检查器")]
    [CustomEditor(typeof(LifecycleEventLite))]
    public class LifecycleEventLiteInspector : TransitionScriptComponentInspector<LifecycleEventLite, ELifecycleEventLite, LifecycleEventLiteFunction, LifecycleEventLiteFunctionCollection>
    {
    }
}
