using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTools.Items
{
    public class ItemCollection : InteractProvider
    {
        public int _maxCapacity = 10;

        public int currentCount => _items.Count;

        public List<Item> _items = new List<Item>();

        public List<Item> items => _items;

        public static event Action<ItemCollection, Item> onAddItem;

        public static event Action<ItemCollection, Item> onRemoveItem;

        protected virtual void Awake()
        {
            _items.ForEach(obj => obj.container = this);
        }

        public virtual bool Add(Item item)
        {
            if (item && currentCount < _maxCapacity)
            {
                item.Remove();
                
                item.container = this;
                _items.Add(item);
                onAddItem?.Invoke(this, item);
                return true;
            }
            return false;
        }

        public virtual bool Remove(Item item)
        {
            var rs = _items.Remove(item);
            if (rs)
            {
                onRemoveItem?.Invoke(this, item);
                item.container = null;
            }
            return rs;
        }

        public bool HasItems(Item item, int amount)
        {
            //return amount == _items.Count(obj => obj._id == item._id);
            return true;
        }

        public bool HasItems(List<ItemInfo> itemInfos, out List<ItemInfo> lostList)
        {
            lostList = new List<ItemInfo>();

            foreach (ItemInfo info in itemInfos)
            {
                var needAmount = info._amount;
                if (needAmount > 0)
                {
                    var hasCount = _items.Count(obj => obj.itemName == info._name);
                    if (hasCount < needAmount)
                    {
                        lostList.Add(new ItemInfo(info._name, needAmount-hasCount, info._pose));
                    }
                }
            }

            return lostList.Count==0;
        }

        public List<Item> GetItems(ItemInfo itemInfo)
        {
            var result = new List<Item>(); 
            var needAmount = itemInfo._amount;
            if (needAmount <= 0) return result;

            foreach (var item in _items)
            {
                if (item.itemName == itemInfo._name && result.Count < needAmount)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public bool Provide(List<ItemInfo> itemInfos, ItemCollection receiver)
        {
            if (!HasItems(itemInfos, out _)) return false;

            foreach (var info in itemInfos)
            {
                var result = GetItems(info);
                foreach (var item in result)
                {
                    info.SetPose(item.transform);
                    receiver.Add(item);
                    Remove(item);
                }
            }
            return true;
        }
    }

}
