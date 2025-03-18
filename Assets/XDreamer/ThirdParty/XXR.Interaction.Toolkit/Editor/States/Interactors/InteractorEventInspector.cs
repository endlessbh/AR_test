using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.States.Interactors;

namespace XCSJ.EditorXXR.Interaction.Toolkit.States.Interactors
{
    /// <summary>
    /// 交互器事件检查器
    /// </summary>
    [Name("交互器事件检查器")]
    [CustomEditor(typeof(InteractorEvent))]
    public class InteractorEventInspector : TriggerInspector<InteractorEvent>
    {
    }
}
