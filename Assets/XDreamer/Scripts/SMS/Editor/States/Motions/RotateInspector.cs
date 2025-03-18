using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 旋转检查器
    /// </summary>
    [Name("旋转检查器")]
    [CustomEditor(typeof(Rotate))]
    public class RotateInspector : TransformMotionInspector<Rotate>
    {
    }
}
