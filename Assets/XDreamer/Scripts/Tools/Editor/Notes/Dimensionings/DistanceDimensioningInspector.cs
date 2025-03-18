using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginTools.Notes.Dimensionings;

namespace XCSJ.EditorTools.Notes.Dimensionings
{
    /// <summary>
    /// 距离尺寸标注检查器
    /// </summary>
    [Name("距离尺寸标注检查器")]
    [CustomEditor(typeof(DistanceDimensioning))]
    public class DistanceDimensioningInspector: DimensioningInspector<DistanceDimensioning>
    {
    }
}
