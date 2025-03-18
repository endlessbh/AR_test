using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Base.Components
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [Icon]
    public abstract class BaseController : Interactor, IController { }

    /// <summary>
    /// 控制器
    /// </summary>
    public interface IController { }
}
