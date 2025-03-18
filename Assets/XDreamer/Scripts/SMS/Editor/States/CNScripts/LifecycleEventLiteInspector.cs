using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.CNScripts;

namespace XCSJ.EditorSMS.States.CNScripts
{
    /// <summary>
    /// 生命周期事件简版检查器
    /// </summary>
    [Name("生命周期事件简版检查器")]
    [CustomEditor(typeof(LifecycleEventLite))]
    public class LifecycleEventLiteInspector : StateScriptComponentInspector<LifecycleEventLite, ELifecycleEventLite, LifecycleEventLiteFunction, LifecycleEventLiteFunctionCollection>
    {
    }
}
