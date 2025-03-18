using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 类别面板，资源库左侧显示资源类别的面板
    /// </summary>
    public class CategoryPanel : BasePanel
    {

        private Category selectCategory;

        private Category rootCategory;

        private List<Category> categories = new List<Category>();

        private bool _clickEnter = false;

        void ResetClickEnter()
        {
            _clickEnter = false;
        }

        #region BasePanel

        public override void Initialize()
        {
            categories.Clear();
            rootCategory = new Category("全部", "", null);
            rootCategory.categories.Clear();
            rootCategory.categories.AddRange(XAssetsDatabase.Instance().categories);

            categories.Add(rootCategory);
            categories.AddRange(XAssetsDatabase.Instance().categories);
        }

        public override void OnSelectedCategoryChanged(Category category, Category lastCategory)
        {
            base.OnSelectedCategoryChanged(category, lastCategory);
            selectCategory = category;
        }

        public override void OnSaveAll()
        {
            Initialize();
        }

        public override void OnCancelEditCategory(Model model)
        {
            Initialize();
        }

        public override void OnExpandCategory()
        {
            if (selectCategory != null)
            {
                if (selectCategory != rootCategory)
                {
                    if (selectCategory.parent == null) expandCategory = selectCategory;
                    selectCategory.expand = !selectCategory.expand;
                }
                
            }
        }

        private void OnOptionModified(Option option)
        {
            InitStyle();
        }

        /// <summary>
        /// 渲染类别窗口
        /// </summary>
        public override void Render()
        {
            Rect rect = AssetLibWindow.instance.position;
            rect.x = 0;
            rect.y = AssetLibWindowStyle.TopHeight;
            rect.height = rect.height - rect.y;
            rect.width = AssetLibWindow.instance.categoryVector.x;
            Event curentEvent = Event.current;

            //处理上下按键处理
            if (curentEvent.keyCode == KeyCode.UpArrow && curentEvent.type == EventType.KeyUp)
            {
                curentEvent.Use();
                SelectPrevCategory();
            }
            if (curentEvent.keyCode == KeyCode.DownArrow && curentEvent.type == EventType.KeyUp)
            {
                curentEvent.Use();
                SelectNextCategory();
            }
            if (curentEvent.keyCode == KeyCode.LeftArrow && curentEvent.type == EventType.KeyUp)
            {
                curentEvent.Use();
                SelectParentCategory();
            }
            if (curentEvent.keyCode == KeyCode.RightArrow && curentEvent.type == EventType.KeyUp)
            {
                curentEvent.Use();
                SelectChildCategory();
            }
            
            this.DrawCategoryArray();
        }

        #endregion BasePanel

        #region 绘制面板

        private string namePath;

        /// <summary>
        /// 绘制类别数组
        /// </summary>
        private void DrawCategoryArray()
        {
            InitStyle();
            if (rootCategory == null) Initialize();
            if (AssetLibWindow.instance.category == null)
            {
                namePath = AssetLibWindow.instance.CategoryNamePath;
                if (string.IsNullOrEmpty(namePath)) AssetLibWindow.instance.category = rootCategory;
            }
            else namePath = "";

            DrawCategory(this.categories);

            GUILayout.Space(5);
        }

        void SelectNextCategory()
        {
            var category = selectCategory;
            if (category.expand && category.categories.Count > 0)
            {
                if(AssetLibWindow.instance.inEdit) AssetLibWindow.instance.category = category.categories[0];
                else
                {
                    var tempCategory = category.categories[0];
                    if (tempCategory.visiableInPanel) AssetLibWindow.instance.category = tempCategory;
                    else SelectNextBrother(tempCategory);
                }
            }
            else
            {
                SelectNextBrother(category);
            }
        }

        void SelectNextBrother(Category category)
        {
            if (category == null) return;
            if (category.parent == null) SelectNextBrother(this.categories, category);
            else SelectNextBrother((category.parent as Category).categories, category);
        }

        void SelectNextBrother(List<Category> categories, Category category)
        {
            int count = categories.Count;
            if (count > 0)
            {
                int i = categories.FindIndex(0, c => c == category);
                if (i <= count - 2)
                {
                    if (AssetLibWindow.instance.inEdit) AssetLibWindow.instance.category = categories[i + 1];
                    else
                    {
                        var tempCategory = categories[i + 1];
                        if (tempCategory.visiableInPanel) AssetLibWindow.instance.category = tempCategory;
                        else SelectNextBrother(categories, tempCategory);
                    }
                }
                else
                {
                    if (category.parent != null) SelectNextBrother(category.parent as Category);
                    else AssetLibWindow.instance.category = rootCategory;
                }
            }
            else SelectNextBrother(category.parent as Category);
        }

        void SelectPrevCategory()
        {
            var category = selectCategory;
            SelectPrevBrother(category);
        }

        void SelectPrevBrother(Category category)
        {
            if (category == null) return;

            if (category.parent == null) SelectPrevBrother(this.categories, category);
            else SelectPrevBrother((category.parent as Category).categories, category);
        }

        void SelectPrevBrother(List<Category> categories, Category category)
        {
            int count = categories.Count;
            if (count > 0)
            {
                int i = categories.FindIndex(0, c => c == category);
                if (i > 0)
                {
                    if (categories[i - 1].expand) SelectPrevBrother(categories[i - 1].categories, categories[i - 1]);
                    else
                    {
                        if (AssetLibWindow.instance.inEdit) AssetLibWindow.instance.category = categories[i - 1];
                        else
                        {
                            var tempCategory = categories[i - 1];
                            if (tempCategory.visiableInPanel) AssetLibWindow.instance.category = tempCategory;
                            else SelectPrevBrother(categories, tempCategory);
                        }
                    }
                }
                else if (i == 0)
                {
                    if (category.parent != null) AssetLibWindow.instance.category = category.parent as Category;
                    else SelectPrevBrother(categories, null);
                }
                else
                {
                    if (categories[count - 1].expand) SelectPrevBrother(categories[count - 1].categories, categories[count - 1]);
                    else
                    {
                        if (AssetLibWindow.instance.inEdit) AssetLibWindow.instance.category = categories[count - 1];
                        else
                        {
                            var tempCategory = categories[count - 1];
                            if (tempCategory.visiableInPanel) AssetLibWindow.instance.category = tempCategory;
                            else SelectPrevBrother(categories, tempCategory);
                        }
                    }
                }
            }
            else if (category.parent != null) AssetLibWindow.instance.category = category.parent as Category;
        }

        void SelectParentCategory()
        {
            if (selectCategory.expand) selectCategory.expand = false;
            else
            {
                Category category = selectCategory.parent as Category;
                if (category == null) return;
                AssetLibWindow.instance.category = category;
            }
        }

        void SelectChildCategory()
        {
            bool selectNext = true;
            if(selectCategory != rootCategory)
            {
                var categories = selectCategory.categories;
                int countCategory = 0;
                if (!AssetLibWindow.instance.inEdit) countCategory = categories.FindAll(c => c.visable).Count;
                else countCategory = categories.Count;

                if (countCategory > 0)
                {
                    if (!selectCategory.expand)
                    {
                        if (selectCategory.parent == null) expandCategory = selectCategory;
                        selectCategory.expand = true;
                        return;
                    }
                    else
                    {
                        SelectNextCanexpandCategory(selectCategory.categories, null);
                        return;
                    }
                }
            }
            if(selectNext)
            {
                SelectNextCanexpandCategory(selectCategory);
            }
        }

        void SelectNextCanexpandCategory(Category category)
        {
            if (category == null) return;
            if (category.parent == null)
            {
                SelectNextCanexpandCategory(this.categories, category);
            }
            else SelectNextCanexpandCategory((category.parent as Category).categories, category);
        }

        void SelectNextCanexpandCategory(List<Category> categories, Category category)
        {
            int count = categories.Count;
            if (count > 0)
            {
                int i = categories.FindIndex(0, c => c == category);

                if (i <= count - 2)
                {
                    if (AssetLibWindow.instance.inEdit)
                    {
                        int countCategory = categories[i + 1].categories.Count;
                        if (countCategory > 0)
                        {
                            AssetLibWindow.instance.category = categories[i + 1];
                        }
                        else SelectNextCanexpandCategory(categories, categories[i + 1]);

                    }
                    else
                    {
                        int countCategory = categories[i + 1].categories.FindAll(c => c.visable).Count;
                        var tempCategory = categories[i + 1];
                        if (categories[i + 1] != rootCategory && (tempCategory.visiableInPanel && countCategory > 0))
                        {
                            AssetLibWindow.instance.category = tempCategory;
                        }
                        else SelectNextCanexpandCategory(categories, tempCategory);
                    }
                }
                else
                {
                    if (category.parent != null) SelectNextCanexpandCategory(category.parent as Category);
                    else SelectNextCanexpandCategory(categories, null);
                }
            }
            else SelectNextCanexpandCategory(category.parent as Category);
        }

        private Category expandCategory;

        /// <summary>
        /// 绘制类别树
        /// </summary>
        /// <param name="categories">类别列表</param>
        private void DrawCategory(List<Category> categories)
        {
            foreach (var c in categories)
            {
                if (c.parent == null)
                {
                    if(expandCategory != null && expandCategory != c) c.expand = false;
                    DrawRootCategory(c, c!= rootCategory);
                }
                else DrawCategory(c);
            }
        }

        private void ExpandAllCategory(Category category, bool expand)
        {
            category.expand = expand;
            category.categories.ForEach(c =>ExpandAllCategory(c, expand));
        }
        private bool clickTwice = false;
        private void DrawRootCategory(Category category, bool canExpand = true)
        {
            int countItem = 0;
            int countCategory = 0;

            if (!AssetLibWindow.instance.inEdit)
            {
                if (!category.visable) return;
                countCategory = category.categories.FindAll(c => c.visable).Count;
                countItem = category.visibleItemTotalCount;
            }
            else
            {
                countCategory = category.categories.Count;
                countItem = category.itemTotalCount;
            }

            EditorGUILayout.BeginHorizontal(AssetLibWindow.horizontalStyle, GUILayout.Width(AssetLibWindow.instance.categoryVector.x));

            if (countCategory > 0 && canExpand)
            {
                var lastExpand = category.expand;
                if(!string.IsNullOrEmpty(namePath))
                {
                    if (namePath == category.namePath) AssetLibWindow.instance.category = category;
                    else if (namePath.Contains(category.namePath)) category.expand = true;
                }
                EditorGUI.BeginDisabledGroup(_clickEnter);
                category.expand = EditorGUILayout.Toggle(category.expand, EditorStyles.foldout, GUILayout.Width(12));
                EditorGUI.EndDisabledGroup();
                if (category.expand)
                {
                    expandCategory = category;
                }
                if (lastExpand != category.expand &&  Event.current.alt)
                {
                    ExpandAllCategory(category, category.expand);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(namePath))
                {
                    if (namePath == category.namePath) AssetLibWindow.instance.category = category;
                }
                EditorGUILayout.LabelField("", GUILayout.Width(12));
            }
            GUIStyle categoryNowStyle = category == selectCategory ? categorySelectStyle : categoryStyle;

            var name =  CommonFun.TempContent(AssetLibWindow.instance.inEdit ? string.Format("{0}({1}/{2})", category.name, category.visibleItemTotalCount, category.itemTotalCount) : string.Format("{0}({1})", category.name, countItem), category.tip);
            clickTwice = Event.current.clickCount >= 2 ? true : clickTwice;

            if (GUILayout.Button(name, categoryNowStyle, GUILayout.ExpandWidth(true)))
            {
                AssetLibWindow.instance.category = category;
                if (canExpand)
                {
                    category.expand = !category.expand;
                    if(category.expand) expandCategory = category;
                }
            }

            DrawEditButtons(category);

            EditorGUILayout.EndHorizontal();

            if (category.expand && countCategory > 0)
            {
                category.scrollPosition = EditorGUILayout.BeginScrollView(category.scrollPosition, false, false, GUILayout.ExpandHeight(true));
                CommonFun.BeginLayout(true, false, 12);
                DrawCategory(category.categories);
                CommonFun.EndLayout();
                EditorGUILayout.EndScrollView();
            }
        }

        /// <summary>
        /// 绘制类别
        /// </summary>
        /// <param name="category">类别</param>
        private void DrawCategory(Category category)
        {
            int countItem = 0;
            int countCategory = 0;

            if (!AssetLibWindow.instance.inEdit)
            {
                if (!category.visable) return;
                countCategory = category.categories.FindAll(c=>c.visable).Count;
                countItem = category.visibleItemTotalCount;
            }
            else
            {
                countCategory = category.categories.Count;
                countItem = category.itemTotalCount;
            }

            EditorGUILayout.BeginHorizontal();

            if (countCategory > 0)
            {
                if (!string.IsNullOrEmpty(namePath))
                {
                    if (namePath == category.namePath) AssetLibWindow.instance.category = category;
                    else if (namePath.Contains(category.namePath)) category.expand = true;
                }
                EditorGUI.BeginDisabledGroup(_clickEnter);
                category.expand = EditorGUILayout.Toggle(category.expand, EditorStyles.foldout, GUILayout.Width(12));
                EditorGUI.EndDisabledGroup();

                if (category.parent == null && category.expand)
                {
                    expandCategory = category;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(namePath))
                {
                    if (namePath == category.namePath) AssetLibWindow.instance.category = category;
                }
                EditorGUILayout.LabelField("", GUILayout.Width(12));
            }
            GUIStyle categoryNowStyle = categoryStyle;
            if (category == selectCategory) categoryNowStyle = categorySelectStyle;
            clickTwice = (Event.current.clickCount >= 2 && Event.current.button == 0) ? true : clickTwice;
            var name = CommonFun.TempContent(AssetLibWindow.instance.inEdit ? string.Format("{0}({1}/{2})", category.name, category.visibleItemTotalCount, category.itemTotalCount) : string.Format("{0}({1})", category.name, countItem), category.tip);
            if (GUILayout.Button(name, categoryNowStyle, GUILayout.ExpandWidth(true)))
            {
                AssetLibWindow.instance.category = category;
                if (clickTwice)
                {
                    category.expand = !category.expand;
                    clickTwice = false;
                }
            }
            DrawEditButtons(category);

            EditorGUILayout.EndHorizontal();

            if (category.expand && countCategory > 0)
            {
                CommonFun.BeginLayout(true, false, 12);
                DrawCategory(category.categories);
                CommonFun.EndLayout();
            }
        }

        void DrawEditButtons(Category category)
        {
            if (AssetLibWindow.instance.inEdit)
            {
                if (category.jsonFileExists) GUILayout.Label(AssetLibWindowStyle.instance.jsonTexture, GUILayout.Width(15), GUILayout.Height(15));
                else GUILayout.Label(AssetLibWindowStyle.instance.jsonNonExistTexture, GUILayout.Width(15), GUILayout.Height(15));

                //编辑状态下绘制编辑按钮
                if (GUILayout.Button("", AssetLibWindowStyle.instance.editStyle, GUILayout.Width(15), GUILayout.Height(15)))
                {
                    AssetLibWindow.instance.CallEditModel(category);
                }
                GUIStyle visiableStyle = category.visable ? AssetLibWindowStyle.instance.visiableStyle : AssetLibWindowStyle.instance.nonVisiableStyle;
                if (GUILayout.Button("", visiableStyle, GUILayout.Width(15), GUILayout.Height(15)))
                {
                    category.MarkDirty(true);
                    category.visable = !category.visable;
                    AssetLibWindow.instance.CallVisibleModelChange(category);
                }
                //if (GUILayout.Button("", AssetLibWindowStyle.instance.closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
                //{
                //    category.MarkDirty(true);
                //    category.Delete();
                //}
            }
        }

        #endregion 绘制面板

        #region 按钮样式

        public GUIStyle categoryStyle = null;

        public GUIStyle categorySelectStyle = null;

        void InitStyle()
        {
            categoryStyle = new GUIStyle();
            categoryStyle.normal.textColor = EditorStyles.label.normal.textColor;
            categoryStyle.hover.background = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
            categoryStyle.hover.background.SetPixels(new Color[]
            {
                AssetLibWindowOption.weakInstance.ColorCategoryHover
            });
            categoryStyle.hover.background.Apply();
            categoryStyle.hover.textColor = Color.white;
            categoryStyle.fixedHeight = 20f;
            categoryStyle.alignment = TextAnchor.MiddleLeft;

            categorySelectStyle = new GUIStyle();
            categorySelectStyle.normal.textColor = AssetLibWindowOption.weakInstance.ColorText;
            categorySelectStyle.normal.background = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
            categorySelectStyle.normal.background.SetPixels(new Color[]
            {
                AssetLibWindowOption.weakInstance.ColorCategoryHover
            });
            categorySelectStyle.normal.background.Apply();
            categorySelectStyle.hover.textColor = AssetLibWindowOption.weakInstance.ColorText;
            categorySelectStyle.fixedHeight = 20f;
            categorySelectStyle.alignment = TextAnchor.MiddleLeft;
        }

        #endregion 按钮样式
    }
}
