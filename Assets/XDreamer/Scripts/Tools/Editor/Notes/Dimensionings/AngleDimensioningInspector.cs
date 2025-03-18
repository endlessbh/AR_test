using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginTools.Notes.Dimensionings;

namespace XCSJ.EditorTools.Notes.Dimensionings
{
    /// <summary>
    /// 角度尺寸标注检查器
    /// </summary>
    [Name("角度尺寸标注检查器")]
    [CustomEditor(typeof(AngleDimensioning))]
    public class AngleDimensioningInspector : DimensioningInspector<AngleDimensioning>
    {
    }
}
