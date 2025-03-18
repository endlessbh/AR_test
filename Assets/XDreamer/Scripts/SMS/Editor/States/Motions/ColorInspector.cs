using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 颜色检查器
    /// </summary>
    [Name("颜色检查器")]
    [CustomEditor(typeof(Color))]
    public class ColorInspector : MotionInspector<Color>
    {
    }
}
