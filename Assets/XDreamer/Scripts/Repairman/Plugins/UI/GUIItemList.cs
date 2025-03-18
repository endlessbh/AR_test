using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginRepairman.Utils;

namespace XCSJ.PluginRepairman.UI
{
    /// <summary>
    /// 物品列表
    /// </summary>
    [Name("物品列表")]
    public class GUIItemList : BaseRepairmanGUI
    {
        /// <summary>
        /// 物品模版
        /// </summary>
        [Name("物品模版")]
        public GameObject itemButtonPrefab;

        /// <summary>
        /// 物品缓存池
        /// </summary>
        private WorkObjectPool<GUIItem> guiItemPool = new WorkObjectPool<GUIItem>();

        /// <summary>
        /// 选中颜色
        /// </summary>
        [Name("选中颜色")]
        public Color selectedColor = new Color(1, 0.6f, 0, 1);

        /// <summary>
        /// 未选中颜色
        /// </summary>
        [Name("未选中颜色")]
        public Color unselectedColor = Color.white;

        private GUIItem prefabItem;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            if (itemButtonPrefab)
            {
                itemButtonPrefab.gameObject.SetActive(false);
                prefabItem = itemButtonPrefab.GetComponentInChildren<GUIItem>();
            }

            guiItemPool.Init(
                () =>
                {
                    if (prefabItem is GUIItemButton)
                    {
                        return GameObjectUtils.CloneGameObject<GUIItemButton>(itemButtonPrefab);
                    }
                    else
                    {
                        return GameObjectUtils.CloneGameObject<GUIItemToggle>(itemButtonPrefab);
                    }

                },
                guiItem => guiItem.gameObject.SetActive(true),
                guiItem => guiItem.gameObject.SetActive(false),
                guiItem => guiItem);
        }

        /// <summary>
        /// 创建物品列表
        /// </summary>
        /// <param name="items"></param>
        protected virtual void CreateItemList(IList items)
        {
            if (!itemButtonPrefab)
            {
                Debug.Log("没有[物品模板]资源，无法创建物品列表。");
                return;
            }
            var itemList = new List<IItem>();
            foreach (var o in items)
            {
                itemList.Add((IItem)o);
            }
            itemList.Sort((x, y) => string.Compare(x.showName, y.showName));
            for (int i = 0; i < itemList.Count; ++i)
            {
                var itemButton = guiItemPool.Alloc();
                if (itemButton)
                {
                    itemButton.SetData(itemList[i], this);                    
                    itemButton.gameObject.SetActive(true);
                    itemButton.gameObject.transform.SetSiblingIndex(i);
                }
            }
        }

        /// <summary>
        /// 清除物品列表
        /// </summary>
        protected void ClearItemList()
        {
            guiItemPool.Clear();
        }
    }
}
