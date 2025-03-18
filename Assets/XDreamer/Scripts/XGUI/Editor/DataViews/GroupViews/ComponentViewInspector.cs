using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.DataViews.GroupViews;

namespace XCSJ.EditorXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 组件数据视图检查器
    /// </summary>
    [Name("组件数据视图检查器")]
    [CustomEditor(typeof(ComponentView), true)]
    public class ComponentViewInspector : GroupViewInspector<ComponentView>
    {
    }
}
