using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.PluginPhysicses.Tools
{
    /// <summary>
    /// 基础物理组件
    /// </summary>
    [RequireManager(typeof(PhysicsManager))]
    [Owner(typeof(PhysicsManager))]
    public abstract class BasePhysicsMB : InteractObject
    {
    }
}
