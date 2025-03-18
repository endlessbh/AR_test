using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 组件闪烁检查器
    /// </summary>
    [Name("组件闪烁检查器")]
    [CustomEditor(typeof(ComponentFlash))]
    public class ComponentFlashInspector : FlashInspector<ComponentFlash>
    {
    }
}
