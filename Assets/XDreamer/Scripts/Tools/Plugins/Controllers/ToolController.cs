using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Controllers
{
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class ToolController : InteractProvider
    {
    }
}
