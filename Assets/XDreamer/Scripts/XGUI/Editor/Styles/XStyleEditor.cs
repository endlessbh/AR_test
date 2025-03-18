using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.EditorXGUI.Styles.Base;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Styles.Base;
using XCSJ.PluginXGUI.Styles.Tools;

namespace XCSJ.EditorXGUI.Styles
{
    /// <summary>
    /// 样式编辑器
    /// </summary>
    [Name(Title)]
    [Tip(Title, "Style Editor")]
    [XCSJ.Attributes.Icon(EIcon.UI)]
    [XDreamerEditorWindow(nameof(TrHelper.Common), index = 10)]
    public sealed class XStyleEditor : XEditorWindowWithScrollView<XStyleEditor>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "样式编辑器";

        /// <summary>
        /// 开始菜单
        /// </summary>
        [MenuItem(XDreamerMenu.EditorWindow + Title)]
        public static void Init()
        {
            OpenAndFocus();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            // 编辑器启动后重新加载对象
            XStyleFileHelper.Load();

            selectedStyle = XStyleCache.styles.FirstOrDefault();

            LoadStyleElementData();
        }

        /// <summary>
        /// 绘制
        /// </summary>
        protected override void OnGUI()
        {
            DrawStyleOperation();

            DrawToolBar();

            DrawStyleDetailInformation();
        }

        /// <summary>
        /// 实时刷新
        /// </summary>
        public override bool timedRepaint => true;

        /// <summary>
        /// 刷新间隔
        /// </summary>
        public override float timeRepaintIntervalTime => 0.1f;

        private string _inputName = "";

        private XStyle selectedStyle
        {
            get => _selectedStyle;
            set
            {
                if (_selectedStyle != value)
                {
                    _selectedStyle = value;
                    _selectedStyleName = _selectedStyle ? _selectedStyle.name : "";
                }
                LoadStyleElementData();
            }
        }
        private XStyle _selectedStyle = null;

        private string _selectedStyleName = "";

        private string defaultStyleName => XStyleCache.defaultStyle ? XStyleCache.defaultStyle.name : "";

        private enum EStyleChooserOperation
        {
            [Name("全部添加")]
            [Tip("为场景中所有RectTransform类型组件对象添加StyleLinker组件", "Add a StyleLinker component to all RectTransform type component objects in the scene")]
            AddAllOnScene,

            [Name("选择对象及子对象添加")]
            [Tip("为场景中选中的具有RectTransform类型组件的游戏对象添加StyleLinker组件", "Add a StyleLinker component to the selected GameObject in the scene with a RectTransform type component")]
            AddSelectionChildren,

            [Name("选择对象添加")]
            [Tip("为场景中选中的具有RectTransform类型组件的游戏对象添加StyleLinker组件", "Add a StyleLinker component to the selected GameObject in the scene with a RectTransform type component")]
            AddSelection,

            [Name("全部移除")]
            [Tip("删除场景中所有StyleLinker组件", "Delete all StyleLinker components in the scene")]
            DeleteAllOnScene,

            [Name("选择对象及子对象移除")]
            [Tip("删除场景中选中的游戏对象的StyleLinker组件", "Delete the StyleLinker component of the selected game object in the scene")]
            DeleteChildrenSelection,

            [Name("选择对象移除")]
            [Tip("删除场景中选中的游戏对象的StyleLinker组件", "Delete the StyleLinker component of the selected game object in the scene")]
            DeleteSelection,
        }

        private EStyleChooserOperation _styleChooserOperation = EStyleChooserOperation.AddSelectionChildren;

        /// <summary>
        /// 场景操作
        /// </summary>
        private void DrawStyleOperation()
        {
            // 默认样式设定
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("场景默认样式", UICommonOption.Width80);
                EditorGUI.BeginChangeCheck();
                var styleName = UICommonFun.Popup(defaultStyleName, XStyleCache.GetNames(), false, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    XStyleCache.defaultStyle = XStyleCache.GetStyle(styleName);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 样式选择器
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(CommonFun.Name(typeof(StyleLinker)), UICommonOption.Width80);

                _styleChooserOperation = (EStyleChooserOperation)UICommonFun.EnumPopup(_styleChooserOperation);

                if (GUILayout.Button("执行", UICommonOption.Width60))
                {
                    switch (_styleChooserOperation)
                    {
                        case EStyleChooserOperation.AddAllOnScene:
                            {
                                StyleHelper.linkStyleTypes.ForEach(t =>
                                {
                                    AddStyleLinker(FindObjectsOfType(t));
                                });
                                break;
                            }
                        case EStyleChooserOperation.AddSelectionChildren:
                            {
                                AddStyleLinker(GetSelectionGameObjectAndChildren());
                                break;
                            }
                        case EStyleChooserOperation.AddSelection:
                            {
                                AddStyleLinker(Selection.gameObjects);
                                break;
                            }
                        case EStyleChooserOperation.DeleteAllOnScene:
                            {
                                RemoveStyleLinker(FindObjectsOfType<StyleLinker>());
                                break;
                            }
                        case EStyleChooserOperation.DeleteChildrenSelection:
                            {
                                RemoveStyleLinker(GetSelectionGameObjectAndChildren());
                                break;
                            }
                        case EStyleChooserOperation.DeleteSelection:
                            {
                                RemoveStyleLinker(Selection.gameObjects);
                                break;
                            }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // 创建样式
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("样式名称", UICommonOption.Width80);
                _inputName = EditorGUILayout.TextField(_inputName, GUILayout.ExpandWidth(true));

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_inputName));
                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    selectedStyle = XStyleFileHelper.Create(_inputName);
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            // 样式下拉列表
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("样式列表", UICommonOption.Width80);

                EditorGUI.BeginChangeCheck();
                _selectedStyleName = UICommonFun.Popup(_selectedStyleName, XStyleCache.GetNames(), false, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    selectedStyle = XStyleCache.GetStyle(_selectedStyleName);
                }

                // 复制
                EditorGUI.BeginDisabledGroup(!_selectedStyle || string.IsNullOrEmpty(_inputName));
                if (GUILayout.Button(UICommonOption.Copy, EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
                {
                    var newStyle = XStyleFileHelper.Copy(selectedStyle, _inputName);
                    if (newStyle)
                    {
                        selectedStyle = newStyle;
                    }
                }
                EditorGUI.EndDisabledGroup();

                // 删除
                EditorGUI.BeginDisabledGroup(!selectedStyle);
                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    if (EditorUtility.DisplayDialog("删除警告", "删除样式将移除对应的磁盘文件，不可恢复！", "确定", "取消"))
                    {
                        XStyleFileHelper.Delete(_selectedStyleName);
                        selectedStyle = XStyleCache.styles.FirstOrDefault();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();
        }   
        
        private List<GameObject> GetSelectionGameObjectAndChildren()
        {
            var list = new List<GameObject>();
            Selection.gameObjects.Foreach(go => list.AddRange(go.GetComponentsInChildren<Transform>(true).Cast(t => t.gameObject)));
            return list;
        }

        /// <summary>
        /// 添加样式链接器
        /// </summary>
        /// <param name="rectTransforms"></param>
        private void AddStyleLinker(IEnumerable<UnityEngine.Object> objects)
        {
            foreach (var obj in objects)
            {
                if (obj is Component component && component)
                {
                    component.XGetOrAddComponent<StyleLinker>();
                }
                else if (obj is GameObject go && go)
                {
                    go.XGetOrAddComponent<StyleLinker>();
                }
            }
        }

        private void AddStyleLinker(IEnumerable<GameObject> gameObjects)
        {
            foreach (var go in gameObjects)
            {
                if (StyleHelper.linkStyleTypes.Exists(t => go.GetComponent(t)))
                {
                    go.XGetOrAddComponent<StyleLinker>();
                }
            }
        }

        /// <summary>
        /// 移除风样式链接器
        /// </summary>
        /// <param name="styleLinkers"></param>
        private void RemoveStyleLinker(IEnumerable<StyleLinker> styleLinkers)
        {
            styleLinkers.Foreach(c => UndoHelper.DestroyObjectWithRegisterObjectReferenceInScene(c));
        }

        private void RemoveStyleLinker(IEnumerable<GameObject> gameObjects)
        {
            gameObjects.Foreach(go =>
            {
                var c = go.GetComponent<StyleLinker>();
                if (c)
                {
                    UndoHelper.DestroyObjectWithRegisterObjectReferenceInScene(c);
                }
            });
        }

        /// <summary>
        /// 滚动视图
        /// </summary>
        public override void OnGUIWithScrollView() { }

        /// <summary>
        /// 工具条
        /// </summary>
        private enum EToolbar
        {
            /// <summary>
            /// 基础元素
            /// </summary>
            [Name("基础元素")]
            [Tip("基础元素", "Basic element")]
            [XCSJ.Attributes.Icon(EIcon.Model)]
            Base,

            /// <summary>
            /// 组合元素
            /// </summary>
            [Name("组合元素")]
            [Tip("组合元素", "Composite element")]
            [XCSJ.Attributes.Icon(EIcon.Models)]
            Collection,
        }

        /// <summary>
        /// 被选择的工具条
        /// </summary>
        private static EToolbar toolbar = EToolbar.Base;

        /// <summary>
        /// 绘制工具条
        /// </summary>
        private void DrawToolBar()
        {
            EditorGUI.BeginChangeCheck();
            toolbar = UICommonFun.Toolbar(toolbar, ENameTip.Image, GUILayout.Height(24));
            if (EditorGUI.EndChangeCheck())
            {
                LoadStyleElementData();
            }
        }

        private void DrawStyleDetailInformation()
        {
            var rect = EditorGUILayout.BeginHorizontal("box");
            
            // 左侧列表
            leftScrollbarPosition = EditorGUILayout.BeginScrollView(leftScrollbarPosition, GUILayout.Width(splitter.x));
            if (selectedStyle)
            {
                switch (toolbar)
                {
                    case EToolbar.Base: DrawBaseLeft(); break;
                    case EToolbar.Collection: DrawCollectionLeft(); break;
                }
            }
            EditorGUILayout.EndScrollView();

            // 分割线
            splitter = UICommonFun.ResizeSeparatorLayout(splitter, true, true, UICommonOption._separatorMinLeftWidth, rect.width - UICommonOption._separatorWidth - UICommonOption._separatorMinRightWidth, XGUIStyleLib.Get(EGUIStyle.Separator), UICommonOption.SeparatorWidth, GUILayout.ExpandHeight(true));

            // 右侧列表
            if (selectedStyle)
            {
                switch (toolbar)
                {
                    case EToolbar.Base: DrawBaseRight(); break;
                    case EToolbar.Collection: DrawCollectionRight(); break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void LoadStyleElementData()
        {
            switch (toolbar)
            {
                case EToolbar.Base:
                    {
                        LoadBaseElements();
                        break;
                    }
                case EToolbar.Collection:
                    {
                        LoadCollections();
                        LoadCollectionElements();
                        break;
                    }
            }
        }

        #region 基础元素

        private int _baseLeftIndex = 0;

        private List<BaseStyleElement> _baseStyleElements = new List<BaseStyleElement>();

        private Type _baseType => StyleHelper.baseStyleElementTypes[_baseLeftIndex];

        private void DrawBaseLeft()
        {
            var index = DrawLeftNameList(StyleHelper.baseStyleElementTypeNames, _baseLeftIndex);
            if (index != _baseLeftIndex)
            {
                _baseLeftIndex = index;
                LoadBaseElements();
            }
        }

        private void DrawBaseRight()
        {
            DrawRightElement(selectedStyle, _baseStyleElements, CreateBaseElement, LoadBaseElements);
        }

        private void CreateBaseElement() => selectedStyle.CreateElement(_baseType);

        private void LoadBaseElements()
        {
            _baseStyleElements.Clear();
            _baseStyleElements.AddRange(selectedStyle.GetElements(_baseType));
            BaseStyleElementInspector.parent = _selectedStyle;
        }

        #endregion

        #region 组合元素

        private int _collectionLeftIndex = 0;

        private string[] _collectionNames = new string[0];

        private List<StyleElementCollection> _collections = new List<StyleElementCollection>();

        private StyleElementCollection _currentCollection = null;

        private List<BaseStyleElement> _currentCollectionStyleElements = new List<BaseStyleElement>();

        private string selectedBaseStyleNameInCollection
        {
            get
            {
                if (string.IsNullOrEmpty(_selectedBaseStyleNameInCollection))
                {
                    _selectedBaseStyleNameInCollection = allBaseElementName[0];
                }
                return _selectedBaseStyleNameInCollection;
            }
            set
            {
                if (_selectedBaseStyleNameInCollection!=value)
                {
                    _selectedBaseStyleNameInCollection = value;
                    LoadCollectionElements();
                }
            }
        }
        private string _selectedBaseStyleNameInCollection = "";

        private const string AllBaseElementName = "全部";

        private string[] allBaseElementName
        {
            get
            {
                if (_allBaseElementName.Length == 0)
                {
                    _allBaseElementName = new string[StyleHelper.baseStyleElementTypeNames.Length + 1];
                    _allBaseElementName[0] = AllBaseElementName;
                    StyleHelper.baseStyleElementTypeNames.CopyTo(_allBaseElementName, 1);
                }
                return _allBaseElementName;
            }
        }
        private string[] _allBaseElementName = new string[0];

        private void DrawCollectionLeft()
        {
            var index = DrawLeftNameList(_collectionNames, _collectionLeftIndex);
            if (index != _collectionLeftIndex)
            {
                _collectionLeftIndex = index;
                LoadCollectionElements();
            }
        }

        private void DrawCollectionRight()
        {
            EditorGUILayout.BeginVertical();
            {
                DrawCollectionCreateOrDelete();

                UICommonFun.SeparatorLine(Color.black, true, 1, 1);

                DrawRightElement(_currentCollection, _currentCollectionStyleElements, CreateElementInCollection, LoadCollectionElements, DrawBaseStyleElmentDropdownList);
            }
            EditorGUILayout.EndVertical();
        } 

        private void DrawCollectionCreateOrDelete()
        {
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button(new GUIContent("组合元素", EditorIconHelper.GetIconInLib(EIcon.Add)), EditorStyles.miniButtonLeft, UICommonOption.Height18))
                {
                    selectedStyle.CreateElement<StyleElementCollection>();
                    LoadCollections();
                }

                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.Height18))
                {
                    selectedStyle.DeleteElement(_currentCollection);
                    LoadCollections();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawBaseStyleElmentDropdownList()
        {
            selectedBaseStyleNameInCollection = UICommonFun.Popup(selectedBaseStyleNameInCollection, allBaseElementName, false, UICommonOption.Width80);
        }

        private void CreateElementInCollection()
        {
            MenuHelper.DrawMenu("创建样式基础元素", m =>
            {
                foreach (var name in StyleHelper.baseStyleElementTypeNames)
                {
                    m.AddMenuItem(name, (obj) =>
                    {
                        var type = StyleHelper.baseStyleElementTypes[StyleHelper.baseStyleElementTypeNames.IndexOf(name)];
                        var e = _currentCollection ? _currentCollection.CreateElement(type) : null;
                        if (e)
                        {
                            LoadCollectionElements();
                        }
                    }, name);
                }
            });
        }

        private void LoadCollections()
        {
            _collections.Clear();
            _collections.AddRange(selectedStyle.GetElements<StyleElementCollection>());
            _collectionNames = _collections.Cast(c => c.name).ToArray();
        }

        private void LoadCollectionElements()
        {
            _currentCollectionStyleElements.Clear();
            if (_collections.Count > 0)
            {
                _collectionLeftIndex = Mathf.Clamp(_collectionLeftIndex, 0, _collections.Count-1);
                _currentCollection = _collections[_collectionLeftIndex];
                if (_currentCollection)
                {
                    BaseStyleElementInspector.parent = _currentCollection;
                    Selection.activeObject = _currentCollection;

                    if (selectedBaseStyleNameInCollection == AllBaseElementName)
                    {
                        _currentCollectionStyleElements.AddRange(_currentCollection.elements);
                    }
                    else
                    {
                        _currentCollectionStyleElements.AddRange(_currentCollection.GetElements(StyleHelper.baseStyleElementTypes[StyleHelper.baseStyleElementTypeNames.IndexOf(selectedBaseStyleNameInCollection)]));
                    }
                }
            }
        }

        #endregion

        private int DrawLeftNameList(string[] names, int selectedIndex)
        {
            for (int i = 0; i < names.Length; i++)
            {
                if (GUILayout.Button(names[i], GetXGUIStyle(i == selectedIndex), UICommonOption.Height24))
                {
                    selectedIndex = i;
                }
            }
            return selectedIndex;
        }

        private void DrawRightElement(StyleElementCollection currentCollection, List<BaseStyleElement> drawElements, Action onCreateElement, Action onElementChanged, Action drawBeforeCreateButton = null)
        {
            bool changed = false;

            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label("编号", UICommonOption.Width32);
                GUILayout.Label(CommonFun.TempContent("名称", ""));

                drawBeforeCreateButton?.Invoke();

                // 创建按钮
                if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                {
                    onCreateElement.Invoke();
                    changed = true;
                }
            }
            EditorGUILayout.EndHorizontal();

            UICommonFun.SeparatorLine(Color.black, true, 1, 1);

            rightScrollbarPosition = EditorGUILayout.BeginScrollView(rightScrollbarPosition, GUILayout.ExpandWidth(true));
            for (int i = 0; i < drawElements.Count; i++)
            {
                GUILayout.Space(2);
                var e = drawElements[i];
                UICommonFun.BeginHorizontal(i);
                {
                    GUILayout.Space(4);
                    EditorGUILayout.BeginHorizontal(Selection.activeObject == e ? "LightmapEditorSelectedHighlight" : GUIStyle.none);
                    {
                        GUILayout.Label((i + 1).ToString(), UICommonOption.Width32);
                        if (GUILayout.Button(e.name, EditorStyles.label, GUILayout.ExpandWidth(true), UICommonOption.Height18))
                        {
                            Selection.activeObject = e;
                        }

                        if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonLeft, UICommonOption.WH24x16))
                        {
                            changed = true;
                            currentCollection.CloneElement(e);
                        }
                        if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                        {
                            changed = true;

                            currentCollection.DeleteElement(e);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(4);
                }
                UICommonFun.EndHorizontal();
                GUILayout.Space(2);
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            if (changed)
            {
                onElementChanged.Invoke();
            }
        }

        private XGUIStyle GetXGUIStyle(bool selected)
        {
            return selected ? XGUIStyleLib.Get(EGUIStyle.Label_Selected_14) : XGUIStyleLib.Get(EGUIStyle.Label_Normal_14);
        }

        private Vector2 leftScrollbarPosition = Vector2.zero;

        private Vector2 rightScrollbarPosition = Vector2.zero;

        private Vector2 splitter = Vector2.zero;

    }
}
