using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 背包
    /// </summary>
    [Name("背包")]
    public abstract class Bag : BagItem, IBag
    {
        /// <summary>
        /// 容量
        /// </summary>
        [Name("容量")]
        [Tip("背包容量", "Backpack Capacity")]
        public int _capacity = 1;

        /// <summary>
        /// 剩余容量
        /// </summary>
        public int remainingCapacity => _capacity - usedCapacity;

        /// <summary>
        /// 已使用容量
        /// </summary>
        public int usedCapacity => _bagItemDatas.Count(data => data._templateBagItem);

        /// <summary>
        /// 当数量清空时处理规则
        /// </summary>
        [Name("当数量清空时处理规则")]
        [EnumPopup]
        public EHandleRuleOnCountClear handleRuleOnCountClear = EHandleRuleOnCountClear.Remove;

        /// <summary>
        /// 当数量清空时处理规则
        /// </summary>
        [Name("当数量清空时处理规则")]
        public enum EHandleRuleOnCountClear
        {
            /// <summary>
            /// 移除:将背包中对应物品信息移除，会将所在背包的剩余容量增加
            /// </summary>
            [Name("移除")]
            [Tip("将背包中对应物品信息移除，会将所在背包的剩余容量增加", "Removing the corresponding item information from the backpack will increase the remaining capacity of the backpack")]
            Remove = 0,

            /// <summary>
            /// 保持:背包中对应物品信息保留，会一直占用所在背包
            /// </summary>
            [Name("保持")]
            [Tip("背包中对应物品信息保留，会一直占用所在背包", "The information of the corresponding items in the backpack is retained and will always occupy the backpack")]
            Keep,
        }

        /// <summary>
        /// 背包物品数据列表
        /// </summary>
        [Name("背包物品数据列表")]
        public List<BagItemData> _bagItemDatas = new List<BagItemData>();

        public abstract string GetDefaultCategoryUsage();
        public abstract List<string> GetCategoryUsages();
        public abstract bool TryGetBagItems(string usage, out List<IBagItem> bagItems);

        public void Add(IBagItem bagItem)
        {
            throw new NotImplementedException();
        }

        public void Remove(IBagItem bagItem)
        {
            throw new NotImplementedException();
        }

        public bool CanAdd(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        public bool TryAdd(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        public bool CanRemove(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(IBagItem bagItem, int count)
        {
            throw new NotImplementedException();
        }

        public bool TryHandleBagItemInteractable(IBagItem bagItem, IItemContext itemContext, IItemInteractor itemInteractor)
        {
            throw new NotImplementedException();
        }

        public List<string> GetBagItemInteractableUsages(IBagItem bagItem, IItemContext itemContext)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 背包物品数据
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("背包物品数据")]
    public class BagItemData
    {
        /// <summary>
        /// 背包
        /// </summary>
        [Name("背包")]
        public Bag _bag;

        /// <summary>
        /// 模版背包物品
        /// </summary>
        [Name("模版背包物品")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BagItem _templateBagItem;

        /// <summary>
        /// 数量：物品的当前堆叠数量
        /// </summary>
        [Name("数量")]
        [Tip("物品的当前堆叠数量", "Current stack quantity of items")]
        [ValidityCheck(EValidityCheckType.NotZero)]
        public int _count = 0;
    }

    /// <summary>
    /// 基础背包物品数据
    /// </summary>
    public interface IBagItemData : IBagItem
    {
        /// <summary>
        /// 总数量:可使用的总数量；可理解为背包中物品叠加上限；
        /// </summary>
        int totalCount { get; }

        /// <summary>
        /// 数量:可使用的数量；可理解为背包中物品当前叠加数；
        /// </summary>
        int count { get; }
    }

    /// <summary>
    /// 基础背包项数据
    /// </summary>
    public abstract class BaseBagItemData : IBagItemData
    {
        /// <summary>
        /// 原型物品
        /// </summary>
        public abstract IItem prototypeItem { get; set; }

        /// <summary>
        /// 总数量:可使用的总数量；可理解为背包中物品叠加上限；
        /// </summary>
        public abstract int totalCount { get; }

        /// <summary>
        /// 数量:可使用的数量；可理解为背包中物品当前叠加数；
        /// </summary>
        public abstract int count { get; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => prototypeItem?.name; set => prototypeItem.name = value; }

        /// <summary>
        /// 能否选择
        /// </summary>
        public bool canSelect
        {
            get
            {
                var prototypeItem = this.prototypeItem;
                return prototypeItem != null ? prototypeItem.canSelect : false;
            }
        }

        /// <summary>
        /// 能否激活
        /// </summary>
        public bool canActivated
        {
            get
            {
                var prototypeItem = this.prototypeItem;
                return prototypeItem != null ? prototypeItem.canActivated : false;
            }
        }

        public bool isSelected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public List<string> cmds => throw new NotImplementedException();

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanInteractable(InteractData interactData)
        {
            var prototypeItem = this.prototypeItem;
            if (prototypeItem == null)
            {
                return false;
            }
            return prototypeItem.CanInteractAsInteractable(interactData);
        }

        /// <summary>
        /// 尝试处理交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryHandleInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            var prototypeItem = this.prototypeItem;
            if (prototypeItem == null)
            {
                interactResult = EInteractResult.Aborted;
                return false;
            }
            return prototypeItem.TryInteractAsInteractable(interactData, out interactResult);
        }

        public string GetDefaultItemInteractableUsage(IItemContext itemContext) => prototypeItem?.GetDefaultItemInteractableUsage(itemContext);

        public List<string> GetItemInteractableUsages(IItemContext itemContext) => prototypeItem?.GetItemInteractableUsages(itemContext);

        public bool TryHandleItemInteractable(InteractData interactData)
        {
            var prototypeItem = this.prototypeItem;
            if (prototypeItem == null)
            {
                return false;
            }
            return prototypeItem.TryInteractAsInteractable(interactData, out _);
        }

        public void OnAfterClone(IItemContext itemContext) { }

        public bool TryHandleItemInteractable(IItemContext itemContext, IItemInteractor itemInteractor)
        {
            throw new NotImplementedException();
        }

        public List<string> GetWorkCmds(InteractData interactData)
        {
            throw new NotImplementedException();
        }

        public bool CanInteract(InteractData interactData)
        {
            throw new NotImplementedException();
        }

        public bool TryInteract(InteractData interactData, out EInteractResult interactResult)
        {
            throw new NotImplementedException();
        }

        public bool CanInteractAsInteractable(InteractData interactData)
        {
            throw new NotImplementedException();
        }

        public bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 基础背包项数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseBagItemData<T> : BaseBagItemData where T : class, IBagItem
    {
        /// <summary>
        /// 原型物品
        /// </summary>
        [Name("原型物品")]
        [Tip("具有原始标准基础信息的物品", "Items with original standard basic information")]
        [Json(false)]
        public T _prototypeItem;

        /// <summary>
        /// 原型物品名称
        /// </summary>
        public string prototypeItemName => _prototypeItem!=null ? _prototypeItem.name : "";

        /// <summary>
        /// 原型物品
        /// </summary>
        [Json(false)]
        public override IItem prototypeItem { get => _prototypeItem; set => _prototypeItem = value as T; }

        /// <summary>
        /// 总数量:可使用的总数量；可理解为背包中物品叠加上限；
        /// </summary>
        [Name("总数量")]
        [Tip("可使用的总数量；可理解为背包中物品叠加上限；", "Total usable quantity; It can be understood as the upper limit of items in the backpack;")]
        [Json(false)]
        public int _totalCount = 1;

        /// <summary>
        /// 总数量
        /// </summary>
        public override int totalCount => _totalCount;

        /// <summary>
        /// 数量:可使用的数量；可理解为背包中物品当前叠加数；
        /// </summary>
        [Name("数量")]
        [Tip("可使用的数量；可理解为背包中物品当前叠加数；", "Quantity available; It can be understood as the current superposition number of items in the backpack;")]
        [Json(false)]
        public int _count = 0;

        /// <summary>
        /// 数量
        /// </summary>
        public override int count => _count;

        /// <summary>
        /// 能否添加数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanAddCount(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "添加数量需大于0");
            return (_count + count) <= _totalCount;
        }

        /// <summary>
        /// 添加数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddCount(int count)
        {
            if (CanAddCount(count))
            {
                _count += count;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 能否移除数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool CanRemoveCount(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count), "移除数量需大于0");
            return (_count - count) >= 0;
        }

        /// <summary>
        /// 移除数量
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveCount(int count)
        {
            if (CanRemoveCount(count))
            {
                _count -= count;
                return true;
            }
            return false;
        }

        public void Handle(string usage, IBag bag)
        { }
    }

    public interface IBagData : IBag { }
}
