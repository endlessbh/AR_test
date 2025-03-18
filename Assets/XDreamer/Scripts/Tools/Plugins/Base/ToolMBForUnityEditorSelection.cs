using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.SelectionUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// Unity编辑器选择集专用的工具组件
    /// </summary>
    [Name("Unity编辑器选择集专用的工具组件")]
    [RequireManager(typeof(ToolsManager))]
    public class ToolMBForUnityEditorSelection : InteractProvider
    {
    }
}
