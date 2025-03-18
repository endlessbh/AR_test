using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 脚本触发检查器
    /// </summary>
    [Name("脚本触发检查器")]
    [CustomEditor(typeof(CNScriptTrigger))]
    public class CNScriptTriggerInspector : StateScriptComponentInspector<CNScriptTrigger, ECNScriptTrigger, CNScriptTriggerFunction, CNScriptTriggerFunctionCollection>
    {
    }
}
