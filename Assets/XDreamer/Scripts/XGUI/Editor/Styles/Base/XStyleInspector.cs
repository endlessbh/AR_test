using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;

namespace XCSJ.EditorXGUI.Styles.Base
{
    /// <summary>
    /// 样式检查器
    /// </summary>
    [Name("样式检查器")]
    [CustomEditor(typeof(XStyle))]
    public class XStyleInspector : SOInspector<XStyle>
    {

    }
}
