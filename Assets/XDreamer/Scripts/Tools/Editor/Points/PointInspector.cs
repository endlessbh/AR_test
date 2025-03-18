using UnityEditor;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.Points;

namespace XCSJ.EditorTools.Points
{
    /// <summary>
    /// 点检查器
    /// </summary>
    [CustomEditor(typeof(Point), true)]
    [CanEditMultipleObjects]
    public class PointInspector : MBInspector<Point>
    {

    }
}
