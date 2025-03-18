using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.States.Locomotions;

namespace XCSJ.EditorXXR.Interaction.Toolkit.States.Locomotions
{
    /// <summary>
    /// 运动事件检查器
    /// </summary>
    [Name("运动事件检查器")]
    [CustomEditor(typeof(LocomotionEvent))]
    public class LocomotionEventInspector : TriggerInspector<LocomotionEvent>
    {
    }
}
