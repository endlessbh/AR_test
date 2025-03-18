using UnityEditor;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Points;

namespace XCSJ.EditorTools.Points
{
    /// <summary>
    /// 线段创建器检查器
    /// </summary>
    [CustomEditor(typeof(SegmentCreater), true)]
    [CanEditMultipleObjects]
    public class SegmentCreaterInspector : InteractorInspector<SegmentCreater>
    {

    }
}
