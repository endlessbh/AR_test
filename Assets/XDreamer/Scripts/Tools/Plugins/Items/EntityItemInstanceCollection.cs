using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 实体物品实例集合
    /// </summary>
    [Name("实体物品实例集合")]
    public class EntityItemInstanceCollection : InteractProvider
    {
        /// <summary>
        /// 模版实体物品
        /// </summary>
        [Name("模版实体物品")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public EntityItem _templateEntityItem;

        /// <summary>
        /// 实例实体物品列表
        /// </summary>
        public List<EntityItem> instanceEntityItems { get; } = new List<EntityItem>();
    }
}
