using UnityEditor;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.EditorExtension.Base.Interactions.Tools
{
    /// <summary>
    /// 可交互虚体检查器
    /// </summary>
    [CustomEditor(typeof(InteractableVirtual), true)]
    [CanEditMultipleObjects]
    public class InteractableVirtualInspector : InteractableVirtualInspector<InteractableVirtual>
    {

    }

    /// <summary>
    /// 可交互虚体检查器模板
    /// </summary>
    public class InteractableVirtualInspector<T> : InteractUsageObjectInspector<T> where T : InteractableVirtual
    {

    }
}
