using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 背包窗口通过IMGUI
    /// </summary>
    [Name("背包窗口通过IMGUI")]
    public class BagWindowByIMGUI : InteractProvider
    {
        /// <summary>
        /// 物品
        /// </summary>
        [Name("物品")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseItem _item;

        /// <summary>
        /// 背包
        /// </summary>
        public BaseItem item => this.XGetComponent(ref _item);

        /// <summary>
        /// 背包窗口
        /// </summary>
        [Name("背包窗口")]
        public BagWindow _bagWindow = new BagWindow();

        /// <summary>
        /// 初始化
        /// </summary>
        public void Awake()
        {
            _bagWindow.bagWindowByIMGUI = this;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _bagWindow.SetWindowAligin();
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected void OnGUI()
        {
            _bagWindow.OnGUI();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (item) { }
        }
    }

    /// <summary>
    /// 背包窗口
    /// </summary>
    [Serializable]
    [Name("背包窗口")]
    public class BagWindow : BaseGUIWindow
    {
        /// <summary>
        /// 自动布局
        /// </summary>
        public override bool autoLayout => true;

        /// <summary>
        /// 默认分类用途
        /// </summary>
        [Name("默认分类用途")]
        public bool _defaultCategoryUsage = true;

        /// <summary>
        /// 背包窗口通过IMGUI
        /// </summary>
        public BagWindowByIMGUI bagWindowByIMGUI { get; internal set; }

        /// <summary>
        /// 分类用途
        /// </summary>
        [Name("分类用途")]
        [Readonly]
        public string _categoryUsage = "";

        private BagItemContext bagItemContext = new BagItemContext(null, null);

        /// <summary>
        /// 自动布局方式绘制内容
        /// </summary>
        protected override void OnDrawContentLayout()
        {
            if (!bagWindowByIMGUI) return;
            var item = bagWindowByIMGUI.item;
            if (!item) return;

            if (item is IBag bag)
            {
                bagItemContext.bag = bag;
                bagItemContext.context = _categoryUsage;

                if (GUILayout.Button("背包:" + bag.name))
                {
                    bag.DefaultDebugLog();
                }

                var bagUsages = bag.GetItemInteractableUsages(bagItemContext);
                if (bagUsages != null && bagUsages.Count > 0)
                {
                    CommonFun.BeginLayout();
                    foreach (var usage in bagUsages)
                    {
                        if (GUILayout.Button(usage))
                        {
                            //bag.TryHandleItemInteractable(bagItemContext, null, usage.ToItemInput(), out _);
                        }
                    }
                    CommonFun.EndLayout();
                }

                if (_defaultCategoryUsage)
                {
                    this._categoryUsage = bag.GetDefaultCategoryUsage();
                }
                else
                {
                    GUILayout.Label("用途:");
                    foreach (var usage in bag.GetCategoryUsages())
                    {
                        if (GUILayout.Button(usage))
                        {
                            this._categoryUsage = usage;
                        }
                    }
                }

                GUILayout.Label("[" + _categoryUsage + "]物品:"); 
                if (bag.TryGetBagItems(_categoryUsage, out var list))
                {
                    foreach (var i in list)
                    {
                        if(i.IsInvalid())
                        {
                            if (GUILayout.Button("无效项"))
                            {

                            }
                            continue;
                        }

                        string text;
                        if (i is IBagItemData bagItemData)
                        {
                            text = string.Format("{0} {1}/{2}", bagItemData.name, bagItemData.count, bagItemData.totalCount);
                            bagItemContext.bagItemData = bagItemData;
                        }
                        else
                        {
                            text = i?.name;
                            bagItemContext.bagItemData = default;
                        }
                        bagItemContext.item = i;

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button(text))
                        {
                            i.DefaultDebugLog();
                        }

                        GUILayout.EndHorizontal();

                        //物品交互用途
                        var bagItemUsages = bag.GetBagItemInteractableUsages(i, bagItemContext);
                        if (bagItemUsages != null && bagItemUsages.Count > 0)
                        {
                            CommonFun.BeginLayout();
                            foreach (var usage in bagItemUsages)
                            {
                                if (GUILayout.Button(usage))
                                {
                                    bag.TryHandleBagItemInteractable(i, bagItemContext, null);
                                }
                            }
                            CommonFun.EndLayout();
                        }
                    }
                }
            }
        }
    }
}
