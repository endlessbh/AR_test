using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginTools.Draggers;

namespace XCSJ.EditorTools.Draggers
{
    /// <summary>
    /// 单一插槽检查器
    /// </summary>
    [Name("单一插槽检查器")]
    [CustomEditor(typeof(SingleSocket), true)]
    [CanEditMultipleObjects]
    public class SingleSocketInspector : BaseSocketInspector<SingleSocket>
    {
    }

    /// <summary>
    /// 基础插槽检查器
    /// </summary>
    [CustomEditor(typeof(BaseSocket), true)]
    [CanEditMultipleObjects]
    public class BaseSocketInspector : BaseSocketInspector<BaseSocket>
    {

    }

    /// <summary>
    /// 基础插槽检查器模板
    /// </summary>
    public class BaseSocketInspector<T> : InteractUsageObjectInspector<T> where T : BaseSocket
    {
    }
}
