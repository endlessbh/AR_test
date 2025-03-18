using UnityEditor;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Points;

namespace XCSJ.EditorTools.Points
{
    /// <summary>
    /// 线段检查器
    /// </summary>
    [CustomEditor(typeof(Segment), true)]
    [CanEditMultipleObjects]
    public class SegmentInspector : InteractableVirtualInspector<Segment>
    {

    }
}
