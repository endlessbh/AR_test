using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 资源列表面板，右侧资源显示列表
    /// </summary>
    public class AssetGridPanel : BasePanel
    {

        #region 变量

        private bool _isDirty = false;

        public void MakeDirty(bool dirty)
        {
            _isDirty = dirty;
        }

        /// <summary>
        /// 资源数目
        /// </summary>
        private int numAssets = 0;

        /// <summary>
        /// 滚动条值
        /// </summary>
        private Vector2 scrollPosition = Vector2.zero;

        private Vector2 lastScrollPos = Vector2.one;

        /// <summary>
        /// 页数
        /// </summary>
        public int pageCount = 0;

        /// <summary>
        /// 当前页
        /// </summary>
        public int pageIndex = 0;

        public int pageIndexTemp = 0;

        public Category currentCategory;

        /// <summary>
        /// 单行单元格个数
        /// </summary>
        private int columnCount = 3;

        private int rowCount = 10;

        private int countPerPage = 30;

        private Rect rectPrePage;
        private Rect rectNexPage;
        private EPlaceType placeType = EPlaceType.Grid;

        public List<AssetPanel> allAssetPanels = new List<AssetPanel>();

        public List<AssetPanel> searchAssetPanels = new List<AssetPanel>();

        public List<AssetPanel> pageAssetPanels = new List<AssetPanel>();

        private string searchText = "";
        private ESearchType searchType = ESearchType.Fuzzy;

        #endregion 变量

        #region BasePanel

        public override void Initialize()
        {
            this.columnCount = AssetLibWindowOption.weakInstance.ColumnNumberPerGrid;
            this.rowCount = AssetLibWindowOption.weakInstance.RowNumberPerGrid;
            this.countPerPage = this.columnCount * this.rowCount;
        }

        public override void OnOptionModified(AssetLibWindowOption alwo)
        {
            this.columnCount = AssetLibWindowOption.weakInstance.ColumnNumberPerGrid;
            this.rowCount = AssetLibWindowOption.weakInstance.RowNumberPerGrid;
            int itemCount = this.columnCount * this.rowCount;
            if (this.countPerPage != itemCount)
            {
                //如果首选项中设置发生变化，重新计算待显示的资源项
                this.countPerPage = itemCount;
                ResetShowItems();
            }
        }

        public override void OnSelectedCategoryChanged(Category category, Category lastCategory)
        {
            base.OnSelectedCategoryChanged(category, lastCategory);
            SetupItems(category);
        }

        public override void OnSearchItems(string searchText, ESearchType searchType)
        {
            SearchItems(searchText, searchType);
        }

        public override void OnEditModeChange(bool edit)
        {
            _isDirty = true;
            SetupItems(currentCategory);
            _isDirty = false;
        }

        public override void OnVisibleModelChange(Model model)
        {
            _isDirty = true;
        }

        public override void OnSelectPage(int index)
        {
            if (currentCategory == null) return;
            SelectPage(index);
        }

        public override void OnChangePlaceType(EPlaceType placeType)
        {
            this.placeType = placeType;
        }

        /// <summary>
        /// 资源列表渲染
        /// </summary>
        public override void Render()
        {
            DrawPanel();
        }

        /// <summary>
        /// 更新函数
        /// </summary>
        public override void Update()
        {
            //AssetPanel.UpdateLoadingAnimation();
        }

        public override void OnSubmitEdit(Model model)
        {
            Item item = model as Item;
            for (int i = 0; i < pageAssetPanels.Count; i++)
            {
                if (pageAssetPanels[i].Asset.guid == item.guid) pageAssetPanels[i].ResetInfo();
            }
        }

        #endregion BasePanel

        #region 面板绘制
        private bool lockScroll = false;
        /// <summary>
        /// 绘制面板
        /// </summary>
        private void DrawPanel()
        {
            Rect position = AssetLibWindow.instance.position;
            position.width = position.width - AssetLibWindow.instance.categoryVector.x - AssetLibWindowStyle.SeparatorWidth -5;
            var height = position.height - AssetLibWindowStyle.TopHeight - AssetLibWindowStyle.NavBarHeight;
            var size = (int)((position.width - 18) / this.columnCount);
            size = Math.Max(size, AssetLibWindowOption.weakInstance.AssetPanelMinSize);

            int cellHeight = size;

            if (placeType == EPlaceType.List)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width));
                DrawListHeader();
            }

            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width));

            scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition, false, false, new GUILayoutOption[0]);
            int scrollRow = rowCount;
            if (placeType == EPlaceType.Grid)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    if (i * columnCount < pageAssetPanels.Count) GUILayout.Box("", AssetLibWindowStyle.instance.transparentStyle, GUILayout.Width(size * columnCount), GUILayout.Height(size));
                        
                    for (int j = columnCount - 1; j > -1; j--)
                    {
                        var index = i * columnCount + j;
                        if (index < pageAssetPanels.Count)
                        {
                            var rect = new Rect(size * j, i * size, size, size);

                            pageAssetPanels[index].Render(rect, true, scrollPosition.y);
                        }
                    }
                }
            }
            else if (placeType == EPlaceType.List)
            {
                scrollRow = rowCount * columnCount;
                cellHeight = 28;
                for (int i = 0; i < pageAssetPanels.Count; i++)
                {
                    GUIStyle backStyle = new GUIStyle();
                    if (i % 2 == 0)
                    {
                        backStyle = new GUIStyle("AC BoldHeader");
                        backStyle.padding = new RectOffset(0, 0, 6, 5);
                        backStyle.fixedHeight = 28;
                    }
                    else
                    {
                        backStyle.padding = new RectOffset(0, 0, 6, 5);
                        backStyle.fixedHeight = 16;
                    }

                    GUIStyle boxStyle = new GUIStyle();
                    boxStyle.padding = new RectOffset(0, 0, 0, 0);
                    GUILayout.Box("", boxStyle, GUILayout.Width(position.width - 15), GUILayout.Height(28));
                    var rect = new Rect(0, i * cellHeight, position.width>518? position.width:518, cellHeight);
                    pageAssetPanels[i].Render(rect, true, scrollPosition.y, placeType, backStyle, i + 1 + pageIndex * countPerPage);
                }
            }

            if (!lockScroll)
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    if (!float.Equals(lastScrollPos.y, scrollPosition.y))
                    {
                        lastScrollPos.y = scrollPosition.y;
                    }
                    else
                    {
                        lockScroll = true;
                        if (Event.current.delta.y < 0 && scrollPosition.y < 1)
                        {
                            UICommonFun.DelayCall(() => SelectPage(this.pageIndex - 1, cellHeight * scrollRow), 0.2f);
                        }
                        else if (Event.current.delta.y > 0 && scrollPosition.y > (cellHeight * scrollRow - height - 25))
                        {
                            UICommonFun.DelayCall(() => SelectPage(this.pageIndex + 1), 0.2f);
                        }
                        else lockScroll = false;
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();

            if (placeType == EPlaceType.List) EditorGUILayout.EndVertical();
        }

        void DrawListHeader()
        {
            GUILayout.BeginHorizontal();
            {
                GUIStyle btnStyle = AssetLibWindowStyle.instance.transparentStyle;
                btnStyle.fixedHeight = 18;
                GUILayout.Label("索引", GUILayout.Width(UICommonOption._32));

                GUILayout.Label("图标", GUILayout.Width(UICommonOption._48));
                GUILayout.Label("名称", GUILayout.Width(UICommonOption._200));
                GUILayout.Label("标签", GUILayout.ExpandWidth(true));
                GUILayout.Label("类型", GUILayout.Width(UICommonOption._100));
                GUILayout.Label("来源", GUILayout.Width(UICommonOption._100));
                if (AssetLibWindow.instance.inEdit)
                {
                    GUILayout.Label("操作", GUILayout.Width(60));
                }
                GUILayout.Space(15);
            }
            GUILayout.EndHorizontal();
        }

        #endregion 面板绘制

        #region 基础显示控制

        /// <summary>
        /// 清除数据
        /// </summary>
        public void Clear()
        {
            this.searchAssetPanels.Clear();
            this.pageAssetPanels.Clear();
            this.numAssets = 0;
        }

        /// <summary>
        /// 初始化设置资源条目
        /// </summary>
        /// <param name="category"></param>
        public void SetupItems(Category category)
        {
            if (category == null) return;
            if (currentCategory == category && !_isDirty) return;
            currentCategory = category;

            this.allAssetPanels.Clear();
            var items = category.SearchItem("", ESearchType.Fuzzy);
            items.ForEach(i => allAssetPanels.Add(new AssetPanel(i)));
            SearchItems(searchText, searchType);
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="searchText">匹配关键字</param>
        /// <param name="eSearchType">搜索类型</param>
        public void SearchItems(string searchText, ESearchType eSearchType = ESearchType.Name)
        {
            if (currentCategory == null) return;
            this.searchText = searchText;
            this.searchType = eSearchType;
            this.Clear();
            var items = currentCategory.SearchItem(searchText, eSearchType, !AssetLibWindow.instance.inEdit);
            items.ForEach(i => searchAssetPanels.Add(new AssetPanel(i)));
            ResetShowItems();
            this.scrollPosition.y = 0f;
            this.lastScrollPos.y = -1;
        }

        private void ResetShowItems()
        {
            this.pageAssetPanels.Clear();
            var currentPanels = AssetLibWindow.instance.inEdit && string.IsNullOrEmpty(this.searchText) ? allAssetPanels : searchAssetPanels;
            int assetCount = currentPanels.Count;
            this.pageCount = (assetCount / this.countPerPage) + ((assetCount % this.countPerPage) == 0 ? 0 : 1);
            this.pageIndex = 0;
            this.pageIndexTemp = 0;
            this.numAssets = this.pageCount > 1 ? this.countPerPage : assetCount;
            for (int i = 0; i < numAssets; i++)
            {
                this.pageAssetPanels.Add(currentPanels[i]);
            }
        }

        /// <summary>
        /// 选择页码
        /// </summary>
        /// <param name="page">页数</param>
        private void SelectPage(int page, float pos = 0)
        {
            lockScroll = false;
            if (page < 0) page = 0;
            if (page > pageCount - 1) page = pageCount - 1;
            if (page == pageIndex) return;
            this.pageIndex = page;
            this.pageIndexTemp = page;

            var currentPanels = AssetLibWindow.instance.inEdit && string.IsNullOrEmpty(this.searchText) ? allAssetPanels : searchAssetPanels;

            this.numAssets = this.pageCount - 1 > page ? this.countPerPage : currentPanels.Count - this.countPerPage * (this.pageCount - 1);
            this.pageAssetPanels.Clear();
            int start = this.countPerPage * page;
            for (int i = start; i < start + this.numAssets; i++)
            {
                this.pageAssetPanels.Add(currentPanels[i]);
            }

            this.scrollPosition.y = pos;
            this.lastScrollPos.y = pos - 1;

            //广播页码变化
            AssetLibWindow.instance.CallSelectPage(page);
        }

        #endregion 基础显示控制
    }
}
