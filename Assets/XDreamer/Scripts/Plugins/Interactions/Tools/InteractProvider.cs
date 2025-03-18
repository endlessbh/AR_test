using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 交互提供者：提供交互所需数据
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class InteractProvider : BaseInteractProvider
    {
    }

    /// <summary>
    /// 交互标签提供者
    /// </summary>
    public abstract class InteractTagProvider : InteractProvider
    {
        /// <summary>
        /// 标签属性
        /// </summary>
        [Name("标签属性")]
        public TagProperty _tagProperty = new TagProperty();
    }
}
