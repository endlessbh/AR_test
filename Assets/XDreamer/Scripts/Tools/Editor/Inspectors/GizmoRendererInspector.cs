using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// Gizmo渲染器检查器
    /// </summary>
    [Name("Gizmo渲染器检查器")]
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GizmoRenderer))]
    public class GizmoRendererInspector : MBInspector<GizmoRenderer>
    {
    }
}
