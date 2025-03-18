using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Renderers
{
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class ToolRenderer : InteractProvider
    {
    }
}
