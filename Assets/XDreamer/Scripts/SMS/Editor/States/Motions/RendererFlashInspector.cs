using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 渲染器闪烁检查器
    /// </summary>
    [Name("渲染器闪烁检查器")]
    [CustomEditor(typeof(RendererFlash))]
    public class RendererFlashInspector : FlashInspector<RendererFlash>
    {
    }
}
