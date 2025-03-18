using UnityEditor;
using XCSJ.Attributes;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 虚拟屏幕检查器
    /// </summary>
    [Name("虚拟屏幕检查器")]
    [CustomEditor(typeof(VirtualScreen))]
    public class VirtualScreenInspector : BaseScreenInspector<VirtualScreen>
    {
    }
}
