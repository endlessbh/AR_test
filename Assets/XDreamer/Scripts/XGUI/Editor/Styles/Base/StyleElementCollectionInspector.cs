using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.EditorXGUI.Styles.Base
{
    /// <summary>
    /// 样式元素集合检查器
    /// </summary>
    [Name("样式元素集合检查器")]
    [CustomEditor(typeof(StyleElementCollection))]
    public class StyleElementCollectionInspector : BaseStyleElementInspector
    {

    }
}
