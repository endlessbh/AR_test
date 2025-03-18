using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.EditorTools.Windows
{
    /// <summary>
    /// 工具对象查看器
    /// </summary>
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.Tool)]
    [XDreamerEditorWindow(nameof(TrHelper.Common), index = 7)]
    [XDreamerEditorWindow(TrHelper.ToolLibrary_EN)]
    public class ToolObjectViewerWindow : XEditorWindowWithScrollView<ToolObjectViewerWindow>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "工具对象查看器";

        /// <summary>
        /// 打开
        /// </summary>
        [MenuItem(XDreamerMenu.NamePath + Title, false, 341)]
        public static void Open() => OpenAndFocus();

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="toolName">工具名</param>
        public static void Open(string toolName)
        {
            OpenAndFocus();
            UICommonFun.DelayCall(() =>
            {
                var window = instance;
                if (window)
                {
                    window.UpdateSelect(toolName);
                }
            });
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="component"></param>
        public static void Open(Component component) => Open(component ? component.GetType() : default);

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="componentType"></param>
        public static void Open(Type componentType) => Open(CommonFun.Name(componentType));

        /// <summary>
        /// 左分割条值
        /// </summary>
        public Vector2 leftSeparator = new Vector2(200, 0);

        /// <summary>
        /// 左区域滚动位置
        /// </summary>
        public Vector2 leftAreaScrollPosition = new Vector2(0, 0);

        /// <summary>
        /// 提示
        /// </summary>
        public static XObject<GUIContent> tipContent { get; } = new XObject<GUIContent>(x =>
        {
            GUIContent content = new GUIContent
            {
                image = EditorIconHelper.GetIconInLib(EIcon.Warning)
            };
            return content;
        });

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            XDreamerEvents.onAnyAssetsOrOptionChanged += OnAnyChanged;
            OnAnyChanged();

        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            XDreamerEvents.onAnyAssetsOrOptionChanged -= OnAnyChanged;
        }

        private Dictionary<Type, List<Component>> components = new Dictionary<Type, List<Component>>();

        class Category : ITreeNodeGraph
        {
            public Category Clone()
            {
                var category = new Category();
                category.componentType = this.componentType;
                category.components.AddRange(this.components);
                category.components.AddRange(this.components);
                return category;
            }

            public Type componentType;

            public List<Component> components { get; private set; } = new List<Component>();

            public string countText { get; private set; } = "0";

            public List<Category> categories = new List<Category>();

            public Category parent;
            public Category[] children => categories.ToArray();

            public GUIContent display => CommonFun.NameTip(componentType);

            public bool enable { get; set; } = true;
            public bool visible { get; set; } = true;

            public int depth => parent == null ? 0 : parent.depth + 1;

            public bool expanded { get; set; } = true;
            public bool selected { get; set; } = false;

            public string displayName => CommonFun.Name(componentType);

            ITreeNode ITreeNode.parent => parent;

            ITreeNodeGraph ITreeNodeGraph.parent => parent;

            ITreeNode[] ITreeNode.children => children;

            ITreeNodeGraph[] ITreeNodeGraph.children => children;

            string IName.name { get => CommonFun.Name(componentType); set { } }

            public bool Match(string searchText)
            {
                if (string.IsNullOrEmpty(searchText)) return true;
                if (componentType.FullName.SearchMatch(searchText) || CommonFun.Name(componentType).SearchMatch(searchText)) return true;
                return false;
            }

            public void OnClick()
            {
                //Debug.Log("OnClick");
            }

            /// <summary>
            /// 获取或创建子级
            /// </summary>
            /// <param name="componentType"></param>
            /// <returns></returns>
            public Category GetOrCreateChild(Type componentType)
            {
                if (componentType == null || !typeof(Component).IsAssignableFrom(componentType)) return default;
                if (this.componentType == componentType) return this;
                if (categories.FirstOrDefault(c => c.componentType == componentType) is Category category) return category;
                var baseTypes = componentType.GetBaseTypes(typeof(Component));
                baseTypes.Reverse();
                Type childType = null;
                bool nextType = false;
                foreach (var t in baseTypes)
                {
                    if (nextType)
                    {
                        childType = t;
                        break;
                    }
                    if (t == this.componentType)
                    {
                        nextType = true;
                    }
                }
                if (childType != null)
                {
                    if (categories.FirstOrNew(c => c.componentType == childType, c =>
                    {

                        c.componentType = childType;
                        c.parent = this;
                        this.categories.Add(c);
                    }) is Category category1)
                    {
                        return category1.GetOrCreateChild(componentType);
                    }
                }
                return default;
            }

            public Category GetChild(Type componentType)
            {
                if (componentType == null) return default;
                Category category = default;
                this.Any(n =>
                {
                    if (n is Category category1 && category1.componentType == componentType)
                    {
                        category = category1;
                        return true;
                    }
                    return false;
                });
                return category;
            }

            /// <summary>
            /// 添加组件
            /// </summary>
            /// <param name="components"></param>
            public void AddComponents(List<Component> components)
            {
                this.visible = true;
                this.components.AddRange(components);
                countText = this.components.Count.ToString();

                parent?.AddComponents(components);
            }
        }

        List<Category> categories = new List<Category>();

        List<Category> displayCategories = new List<Category>();

        Category displayCategory = new Category();

        Category displayCategoryGreat = new Category();

        Type _selectComponentType;

        private Type selectComponentType
        {
            get => _selectComponentType;
            set
            {
                viewerEditor?.OnDisable();
                viewerEditor = ToolObjectViewerEditor.GetEditor(_selectComponentType = value);
                if (viewerEditor != null)
                {
                    viewerEditor.components.Clear();
                    if (displayCategory.GetChild(value) is Category category)
                    {
                        viewerEditor.components.AddRange(category.components);
                    }
                    viewerEditor.OnEnable();
                }
            }
        }

        private ToolObjectViewerEditor viewerEditor;

        private void OnAnyChanged()
        {
            components.Clear();
            categories.Clear();
            displayCategories.Clear();
            foreach (var group in ComponentCache.Get(typeof(ITool), true).components.GroupBy(c => c.GetType()))
            {
                var type = group.Key;
                var list = new List<Component>(group);
                components.Add(type, list);
                var category = new Category() { componentType = type };
                category.AddComponents(list);
                categories.Add(category);
            }
            SortCategories();
            UpdateDisplayCategories();
        }

        private static XGUIStyle categoryCountStyle = new XGUIStyle(EGUIStyle.Label_Normal_Right);

        private static XGUIStyle selectedStyle = new XGUIStyle(EGUIStyle.Label_Selected_14);

        private static XGUIStyle normalStyle = new XGUIStyle(EGUIStyle.Label_Normal_14);

        private static XGUIStyle GetLeftTextStyle(bool selected) => selected ? selectedStyle : normalStyle;

        /// <summary>
        /// 搜索文本
        /// </summary>
        private string _searchText = "";

        /// <summary>
        /// 搜索文本
        /// </summary>
        public string searchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText)
                {
                    _searchText = value;
                    UpdateDisplayCategories();
                }
            }
        }

        private void SortCategories()
        {
            categories.Sort((x, y) =>
            {
                var value = string.Compare(CommonFun.Name(x.componentType), CommonFun.Name(y.componentType));
                if (value != 0) return value;
                value = string.Compare(x.componentType.Name, y.componentType.Name);
                if (value != 0) return value;
                return string.Compare(x.componentType.FullName, y.componentType.FullName);
            });
        }

        private void UpdateSelect(Type componentType)
        {
            selectComponentType = displayCategories.FirstOrDefault(c => componentType == c.componentType)?.componentType ?? displayCategories.FirstOrDefault()?.componentType;
        }

        private void UpdateSelect(string toolName)
        {
            selectComponentType = displayCategories.FirstOrDefault(c => CommonFun.Name(c.componentType) == toolName)?.componentType ?? displayCategories.FirstOrDefault()?.componentType;
        }

        [LanguageTuple("Tool type: {0} / {1} \nTool object: {2} / {3}", "工具类型: {0}/{1}\n工具对象: {2}/{3}")]
        private void UpdateDisplayCategories()
        {
            displayCategories.Clear();
            displayCategories.AddRange(categories.Where(c => c.Match(searchText)));

            UpdateSelect(selectComponentType);

            tipContent.obj.tooltip = string.Format("Tool type: {0} / {1} \nTool object: {2} / {3}".Tr(this),
                displayCategories.Count.ToString(),
                categories.Count.ToString(),
                displayCategories.Sum(c => c.components.Count).ToString(),
                categories.Sum(c => c.components.Count).ToString()
                );

            //displayCategory.componentType = typeof(Component);
            //displayCategory.componentType = typeof(MonoBehaviour);
            displayCategory.componentType = typeof(MB);
            displayCategory.Foreach(node =>
            {
                if (node is Category category)
                {
                    category.visible = false;
                    category.components.Clear();
                }
            });
            foreach (var c in displayCategories)
            {
                var category = displayCategory.GetOrCreateChild(c.componentType);
                if (category != null)
                {
                    category.AddComponents(c.components);
                }
            }
        }

        /// <summary>
        /// 模式
        /// </summary>
        [Name("模式")]
        public enum EMode
        {
            [XCSJ.Attributes.Icon(EIcon.List)]
            [Tip("列表显示", "List display")]
            List,

            [XCSJ.Attributes.Icon(EIcon.Category)]
            [Tip("树形显示", "Tree display")]
            Tree,
        }

        /// <summary>
        /// 模式
        /// </summary>
        public EMode mode = EMode.List;

        /// <summary>
        /// 有索引编号
        /// </summary>
        public bool hasIndex = true;

        private void DrawSearch()
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            searchText = UICommonFun.SearchTextField(searchText);
            if (UICommonFun.ButtonToggle(CommonFun.NameTip(EMode.List, ENameTip.EmptyTextWhenHasImage), mode == EMode.List, EditorStyles.miniButtonLeft, UICommonOption.Width24, GUILayout.Height(18)))
            {
                mode = EMode.List;
            }
            if (UICommonFun.ButtonToggle(CommonFun.NameTip(EMode.Tree, ENameTip.EmptyTextWhenHasImage), mode == EMode.Tree, EditorStyles.miniButtonRight, UICommonOption.Width24, GUILayout.Height(18)))
            {
                mode = EMode.Tree;
            }
            if (hasIndex = UICommonFun.ButtonToggle(CommonFun.NameTip(EIcon.ID), hasIndex, EditorStyles.miniButton, UICommonOption.Width24, GUILayout.Height(18)))
            {
            }
            GUILayout.Label(tipContent, UICommonOption.Width24, GUILayout.Height(18));
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolObjects()
        {
            switch (mode)
            {
                case EMode.Tree:
                    {
                        DrawToolObjects_Tree();
                        break;
                    }
                case EMode.List:
                    {
                        DrawToolObjects_List();
                        break;
                    }
            }
        }

        private void DrawToolObjects_List()
        {
            int i = 1;
            foreach (var category in displayCategories)
            {
                var type = category.componentType;

                EditorGUILayout.BeginHorizontal();
                if (hasIndex)
                {
                    GUILayout.Label((i++).ToString(), UICommonOption.WH24x16);
                }
                if (GUILayout.Button(type.TrLabel(), GetLeftTextStyle(type == selectComponentType), UICommonOption.Height24))
                {
                    selectComponentType = type;
                }
                EditorGUILayout.EndHorizontal();

                //数量
                {
                    var btnRect = GUILayoutUtility.GetLastRect();
                    btnRect.x += (btnRect.width - 32 - 4);
                    btnRect.width = 32;
                    GUI.Label(btnRect, category.countText, categoryCountStyle);
                }
            }
        }

        private void DrawToolObjects_Tree()
        {
            TreeView.Draw(displayCategory,
                null,
                (s, n, i) => hasIndex ? (i + 1).ToString() : "",
                (n, c) =>
                {
                    var category = (Category)n;
                    if (GUILayout.Button(category.componentType.TrLabel(), GetLeftTextStyle(category.componentType == selectComponentType), UICommonOption.Height24))
                    {
                        selectComponentType = category.componentType;
                    }

                    //数量
                    {
                        var btnRect = GUILayoutUtility.GetLastRect();
                        btnRect.x += (btnRect.width - 32 - 4);
                        btnRect.width = 32;
                        GUI.Label(btnRect, category.countText, categoryCountStyle);
                    }
                }, 8, 12);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected override void OnGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();

            //左侧区域
            EditorGUILayout.BeginVertical(GUILayout.Width(leftSeparator.x), GUILayout.ExpandHeight(true));
            {
                DrawSearch();

                EditorGUILayout.Separator();

                leftAreaScrollPosition = EditorGUILayout.BeginScrollView(leftAreaScrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                DrawToolObjects();

                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();

            //左分割条
            leftSeparator = UICommonFun.ResizeSeparatorLayout(leftSeparator, true, true, 160, position.width, XGUIStyleLib.Get(EGUIStyle.Separator), UICommonOption.SeparatorWidth, GUILayout.ExpandHeight(true));

            //右侧区域
            base.OnGUI();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制右侧GUI
        /// </summary>
        [LanguageTuple("Edit [Tool Component] script", "编辑[工具组件]脚本")]
        [LanguageTuple("Edit [Tool Component] inspector script", "编辑[工具组件]检查器脚本")]
        [LanguageTuple("Edit [Tool Object Viewer] script", "编辑[工具对象查看器编辑器]脚本")]
        public override void OnGUIWithScrollView()
        {
#if XDREAMER_EDITION_XDREAMERDEVELOPER
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Edit [Tool Component] script".Tr(this)))
            {
                EditorHelper.OpenMonoScript(viewerEditor?.componentType);
            }
            if (GUILayout.Button("Edit [Tool Component] inspector script".Tr(this)))
            {
                EditorHelper.OpenInspectorMonoScript(viewerEditor?.componentType);
            }
            if (GUILayout.Button("Edit [Tool Object Viewer] script".Tr(this)))
            {
                EditorHelper.OpenMonoScript(viewerEditor?.GetType());
            }
            EditorGUILayout.EndHorizontal();
#endif
            viewerEditor?.OnGUI();
        }
    }
}
