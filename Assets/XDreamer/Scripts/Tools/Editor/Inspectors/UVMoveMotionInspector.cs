using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorTools.Inspectors
{
    /// <summary>
    /// UV平移检查器
    /// </summary>
    [Name("UV平移检查器")]
    [CustomEditor(typeof(UVMoveMotion))]
    [CanEditMultipleObjects]
    public class UVMoveMotionInspector : MBInspector<UVMoveMotion>
    {

    }
}
