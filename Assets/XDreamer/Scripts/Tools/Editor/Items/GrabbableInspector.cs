using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.PluginTools.Items;

namespace XCSJ.EditorTools.Items
{
    /// <summary>
    /// 可抓对象检查器
    /// </summary>
    [Name("可抓对象检查器")]
    [CustomEditor(typeof(Grabbable), true)]
    [CanEditMultipleObjects]
    public class GrabbableInspector : InteractableVirtualInspector<Grabbable>
    {
    }
}
