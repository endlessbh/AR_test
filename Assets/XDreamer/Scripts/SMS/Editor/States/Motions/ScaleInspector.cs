using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 缩放检查器
    /// </summary>
    [Name("缩放检查器")]
    [CustomEditor(typeof(Scale))]
    public class ScaleInspector : TransformMotionInspector<Scale>
    {
    }
}
