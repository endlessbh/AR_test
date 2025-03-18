using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.PluginTools.Notes.Dimensionings;

namespace XCSJ.EditorTools.Notes.Dimensionings
{
    /// <summary>
    /// 尺寸标注检查器
    /// </summary>
    [Name("尺寸标注检查器")]
    [CustomEditor(typeof(Dimensioning))]
    public class DimensioningInspector : DimensioningInspector<Dimensioning> { }

    /// <summary>
    /// 尺寸标注检查器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DimensioningInspector<T> : InteractProviderInspector<T> where T : Dimensioning
    {
    }
}
