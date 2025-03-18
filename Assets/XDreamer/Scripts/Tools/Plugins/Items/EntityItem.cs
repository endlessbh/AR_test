using XCSJ.Attributes;
using XCSJ.Helper;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 实体物品：可用于交互的实体物品
    /// </summary>
    [Name("实体物品")]
    [Tip("可用于交互的实体物品")]
    public class EntityItem : BaseItem, IEntityItem
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        [Name("唯一编号")]
        [Tip("由当前实体物品对象再次实例化后，保持本编号不变")]
        public string _guid = "";

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _guid = GuidHelper.GetNewGuid();
        }
    }
}
