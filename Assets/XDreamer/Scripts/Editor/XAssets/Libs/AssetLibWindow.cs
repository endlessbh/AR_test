using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.EditorWindows;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.EditorExtension.XAssets.Libs
{
    /// <summary>
    /// 本地资源库:用于访问XDreamer本地资源的窗口
    /// </summary>
    [Name(Title)]
    [Tip("用于访问XDreamer本地资源的窗口", "Window for accessing XDreamer local resources")]
    [XCSJ.Attributes.Icon(EIcon.Store)]
    [XDreamerEditorWindow(nameof(TrHelper.Common), index = 2)]
    public class AssetLibWindow : XEditorWindow<AssetLibWindow>
    {
        public const string Title = "本地资源库";

        [MenuItem(XDreamerMenu.NamePath + Product.Name + Title, priority = 8000)]
        public static void Init() => OpenAndFocus();

        [Name("首页")]
        [Tip("导航到首页", "Navigate to home page")]
        [XCSJ.Attributes.Icon(EIcon.MoveLeftBorder)]
        public static XGUIContent firstGUIContent { get; } = GetXGUIContent(nameof(firstGUIContent));

        [Name("上一页")]
        [Tip("向前导航", "Forward navigation")]
        [XCSJ.Attributes.Icon(EIcon.MoveLeft)]
        public static XGUIContent previousGUIContent { get; } = GetXGUIContent(nameof(previousGUIContent));

        [Name("下一页")]
        [Tip("向后导航", "Navigate backwards")]
        [XCSJ.Attributes.Icon(EIcon.MoveRight)]
        public static XGUIContent nextGUIContent { get; } = GetXGUIContent(nameof(nextGUIContent));

        [Name("尾页")]
        [Tip("导航到尾页", "Navigate to the last page")]
        [XCSJ.Attributes.Icon(EIcon.MoveRightBorder)]
        public static XGUIContent lastGUIContent { get; } = GetXGUIContent(nameof(lastGUIContent));

        [Name("保存")]
        [Tip("保存全部", "Save all")]
        [XCSJ.Attributes.Icon(EIcon.Save)]
        public static XGUIContent saveGUIContent { get; } = GetXGUIContent(nameof(saveGUIContent));

        [Name("列表")]
        [Tip("列表显示", "List display")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        public static XGUIContent listGUIContent { get; } = GetXGUIContent(nameof(listGUIContent));

        [Name("网格")]
        [Tip("网格显示", "Grid display")]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        public static XGUIContent gridGUIContent { get; } = GetXGUIContent(nameof(gridGUIContent));

        public static XGUIStyle horizontalStyle = new XGUIStyle(EGUIStyle.Box_Solid);

        private static XGUIContent GetXGUIContent(string propertyName) => new XGUIContent(typeof(AssetLibWindow), propertyName, true);

        [Name("窗口状态")]
        public enum EWindowState
        {
            Init,

            Declaration,

            Load,

            Loading,

            Loaded,

            EditAsset,

            EditCategory
        }

        /// <summary>
        /// 当前窗口状态
        /// </summary>
        public EWindowState windowState = EWindowState.Init;

        /// <summary>
        /// 目录面板大小向量
        /// </summary>
        public Vector2 categoryVector = new Vector2(200, 0);

        private bool initStyle = false;

        #region Unity函数

        protected override void OnEnable()
        {
            base.OnEnable();
            this.minSize = AssetLibWindowStyle.WindowSize;
            panels.Add(declarationPanel);
            panels.Add(loadingPanel);
            panels.Add(topPanel);
            panels.Add(categoryPanel);
            panels.Add(assetGridPanel);
            panels.Add(assetEditPanel);
            panels.Add(categoryEditPanel);
            panels.Add(assetNaviPanel);
            this.LoadImages();

            windowState = EWindowState.Init;
            Preferences.onOptionModified += OnOptionModified;
            OnOptionModified(AssetLibWindowOption.weakInstance);
            EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;
            CategoryNamePath = PlayerPrefs.GetString(CommonFun.Name(this, nameof(this.CategoryNamePath)));

            XDreamerEditor.onPostprocessAllAssets += OnPostprocessAllAssets;
        }

        private bool enterPlayMode = false;
        private void HandleOnPlayModeChanged(PlayModeStateChange playMode)
        {
            if (playMode.Equals(PlayModeStateChange.ExitingEditMode))
                enterPlayMode = true;
            else enterPlayMode = false;
        }

        protected override void OnDisable()
        {
            if (!enterPlayMode) PlayerPrefs.SetString(CommonFun.Name(this, nameof(this.CategoryNamePath)), "");
            Preferences.onOptionModified -= OnOptionModified;
            XDreamerEditor.onPostprocessAllAssets -= OnPostprocessAllAssets;
            base.OnDisable();
        }

        private void Update()
        {
            ImageDownloader.Instance.UpdateJobs();
            switch (windowState)
            {
                case EWindowState.Init:
                    {
                        break;
                    }
                case EWindowState.Declaration:
                    {
                        this.declarationPanel.Update();
                        break;
                    }
                case EWindowState.Load:
                    {
                        break;
                    }
                case EWindowState.Loading:
                    {
                        loadingPanel.UpdateLoadingAnimation();
                        break;
                    }
                case EWindowState.Loaded:
                    {
                        ImageDownloader.Instance.UpdateJobs();
                        this.topPanel.Update();
                        this.assetGridPanel.Update();
                        this.categoryPanel.Update();
                        break;
                    }
                case EWindowState.EditAsset:
                    {
                        break;
                    }
                case EWindowState.EditCategory:
                    {
                        break;
                    }
            }
        }

        protected override void OnGUI()
        {
            if (!initStyle)
            {
                initStyle = true;
                AssetLibWindowStyle.instance.InitStyles();
            }

            Rect rect = new Rect(0f, 0f, base.position.width, base.position.height);

            switch (windowState)
            {
                case EWindowState.Init:
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            if (AssetLibWindowOption.weakInstance.RecieveDeclaration)
                            {
                                windowState = EWindowState.Load;
                            }
                            else
                            {
                                windowState = EWindowState.Declaration;
                            }
                        }
                        break;
                    }
                case EWindowState.Declaration:
                    {
                        this.declarationPanel.Render();
                        break;
                    }
                case EWindowState.Load:
                    {
                        loadingPanel.Initialize();
                        loadingPanel.Render();
                        windowState = EWindowState.Loading;

                        UICommonFun.DelayCall(LoadAsset, 0.5f);
                        break;
                    }
                case EWindowState.Loading:
                    {
                        loadingPanel.Render();
                        break;
                    }
                case EWindowState.Loaded:
                    {
                        ShowPanel(this.assetGridPanel);
                        break;
                    }
                case EWindowState.EditAsset:
                    {
                        ShowPanel(this.assetEditPanel);
                        break;
                    }
                case EWindowState.EditCategory:
                    {
                        ShowPanel(this.categoryEditPanel);
                        break;
                    }
            }

            base.Repaint();
        }

        private void ShowPanel(BasePanel basePanel)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(AssetLibWindowStyle.TopHeight));
            this.topPanel.Render();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(categoryVector.x), GUILayout.ExpandHeight(true));
            this.categoryPanel.Render();
            EditorGUILayout.EndVertical();
            categoryVector = UICommonFun.ResizeSeparatorLayout(categoryVector, true, true, 200, 600, XGUIStyleLib.Get(EGUIStyle.Separator), GUILayout.Width(UICommonOption._separatorWidth), GUILayout.ExpandHeight(true));

            EditorGUILayout.BeginVertical();
            this.assetNaviPanel.Render();
            basePanel.Render();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

        }

        void OnDestroy()
        {
            GC.Collect();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (Check(importedAssets) || Check(deletedAssets) || Check(movedAssets) || Check(movedFromAssetPaths))
            {
                //重新加载
                windowState = EWindowState.Load;
            }
        }

        /// <summary>
        /// 检查本地资源库对应文件夹中是否有数据文件发生变化
        /// </summary>
        /// <param name="assets"></param>
        /// <returns></returns>
        bool Check(string[] assets)
        {
            return assets.Any(path =>
            {
                if (AssetLibWindowOption.weakInstance.assetPathConfigs.Any(c => path.StartsWith(c.path)))
                {
                    var ext = "." + Path.GetExtension(path);
                    if (string.Compare(ext, Define.FileExt, true) == 0) return false;

                    return true;
                }
                return false;
            });
        }

        #endregion Unity函数

        #region 首选项变化响应函数

        protected override void OnOptionModified(Option option)
        {
            base.OnOptionModified(option);
            if (option is AssetLibWindowOption alwo)
            {
                AssetLibWindowStyle.instance.InitStyles();

                panels.ForEach(p => p.OnOptionModified(alwo));
            }
        }

        #endregion 首选项变化响应函数

        #region 事件响应

        public void CallSelectedCategoryChanged(Category category, Category lastCategory) => panels.ForEach(p => p.OnSelectedCategoryChanged(category, lastCategory));

        private Category _category = null;
        public Category category
        {
            get => _category;
            set
            {
                if (windowState == EWindowState.Loaded)
                {
                    var tmp = _category;
                    _category = value;
                    PlayerPrefs.SetString(CommonFun.Name(this, nameof(this.CategoryNamePath)), _category.name == "全部" ? "" : _category.namePath);
                    CallSelectedCategoryChanged(_category, tmp);
                }
            }
        }

        [Name("AssetLibWindowCategory")]
        public string CategoryNamePath;


        public void CallSearchItems(string searchText, ESearchType searchType) => panels.ForEach(p => p.OnSearchItems(searchText, searchType));

        public void CallSaveAll()
        {
            XAssetsDatabase.instance.Save();
            panels.ForEach(p => p.OnSaveAll());
        }

        private void CallEditModeChange(bool edit) => panels.ForEach(p => p.OnEditModeChange(edit));
        private bool _inEdit = false;
        /// <summary>
        /// 当前是否处于编辑状态
        /// </summary>
        public bool inEdit
        {
            get => _inEdit;
            set
            {
                _inEdit = value;
                CallEditModeChange(_inEdit);
            }
        }

        public void CallEditModel(Model model)
        {
            if (model is Category) windowState = EWindowState.EditCategory;
            else if (model is Item) windowState = EWindowState.EditAsset;
            panels.ForEach(p => p.OnEditModel(model));
        }

        public void CallVisibleModelChange(Model model) => panels.ForEach(p => p.OnVisibleModelChange(model));

        public void CallCancelEdit()
        {
            assetNaviPanel.Initialize();
            windowState = EWindowState.Loaded;
            panels.ForEach(p => p.OnCancelEdit(category));
        }

        public void CallCancelEditCategory()
        {
            assetNaviPanel.Initialize();
            windowState = EWindowState.Loaded;
            panels.ForEach(p => p.OnCancelEditCategory(category));
        }

        public void CallSubmitEdit(Model model)
        {
            assetNaviPanel.Initialize();
            windowState = EWindowState.Loaded;
            panels.ForEach(p => p.OnSubmitEdit(model));
        }

        public void CallSelectPage(int index) => panels.ForEach(p => p.OnSelectPage(index));

        public void CallChangePlaceType(EPlaceType placeType) => panels.ForEach(p => p.OnChangePlaceType(placeType));

        #endregion 事件响应

        #region 外部按钮响应函数

        public void EnterLoad()
        {
            assetNaviPanel.Initialize();
            windowState = EWindowState.Loaded;
        }

        public void RecieveDeclaration()
        {
            AssetLibWindowOption.weakInstance.RecieveDeclaration = true;
            AssetLibWindowOption.weakInstance.MarkDirty();
            windowState = EWindowState.Load;
        }

        public void CloseWindow()
        {
            this.Close();
        }

        public void ShowDeclaration()
        {
            windowState = EWindowState.Declaration;
            _category = null;
            CategoryNamePath = PlayerPrefs.GetString(CommonFun.Name(this, nameof(this.CategoryNamePath)));
        }

        private void LoadAsset()
        {
            assetGridPanel.Initialize();
            topPanel.Initialize();
            _category = null;
            XAssetsDatabase.instance.Load(ELoadRule.LoadAndGenerate);

            categoryPanel.Initialize();
        }

        #endregion 外部按钮响应函数

        #region 面板

        public List<BasePanel> panels = new List<BasePanel>();

        /// <summary>
        /// 免责声明面板
        /// </summary>
        public DeclarationPanel declarationPanel = new DeclarationPanel();

        /// <summary>
        /// 顶部面板
        /// </summary>
        public TopPanel topPanel = new TopPanel();

        /// <summary>
        /// 分类列表面板，左侧面板
        /// </summary>
        public CategoryPanel categoryPanel = new CategoryPanel();

        /// <summary>
        /// 资源列表面板，右侧面板
        /// </summary>
        public AssetGridPanel assetGridPanel = new AssetGridPanel();

        /// <summary>
        /// 加载面板
        /// </summary>
        public LoadingPanel loadingPanel = new LoadingPanel();

        /// <summary>
        /// 资源编辑面板
        /// </summary>
        public AssetEditPanel assetEditPanel = new AssetEditPanel();

        /// <summary>
        /// 目录编辑面板
        /// </summary>
        public CategoryEditPanel categoryEditPanel = new CategoryEditPanel();

        /// <summary>
        /// 资源导航面板
        /// </summary>
        public AssetNaviPanel assetNaviPanel = new AssetNaviPanel();

        #endregion 面板

        #region 加载图

        public AnimatedImage spinnerImage = null;
        /// <summary>
        /// 加载图片
        /// </summary>
        private void LoadImages()
        {
            string path = UICommonFun.ToFullPath("Assets/XDreamer/EditorResources/XAssetLib/spinner.png");
            this.spinnerImage = new AnimatedImage(path, "", false, 16, 6, 4, 128, 128);
        }

        #endregion 加载图

        #region 静态函数

        public static Color[] CreateBoxWithBorder(int width, int height, Color fillcolor, Color bordercolor, bool left = true, bool right = true, bool top = true, bool bottom = true)
        {
            Color[] array = new Color[width * height];
            for (int i = 0; i < width * height; i++)
            {
                int num = i % width;
                int num2 = i / width;
                bool flag = (num == 0 & left) || (num == width - 1 & right) || (num2 == 0 & top) || (num2 == height - 1 & bottom);
                if (flag)
                {
                    array[i] = bordercolor;
                }
                else
                {
                    array[i] = fillcolor;
                }
            }
            return array;
        }

        #endregion 静态函数
    }

    /// <summary>
    /// 资源窗口样式类
    /// </summary>
    public class AssetLibWindowStyle : InstanceClass<AssetLibWindowStyle>
    {
        #region 窗口设置
        public static Vector2 WindowSize = new Vector2(250, 100);

        /// <summary>
        /// 顶部面板高度
        /// </summary>
        public static int TopHeight = 18;

        /// <summary>
        /// 导航栏高度
        /// </summary>
        public static int NavBarHeight = 24;

        /// <summary>
        /// 分隔条宽度
        /// </summary>
        public static int SeparatorWidth = 3;

        #endregion 窗口设置

        #region 颜色

        public Color colorWindowBackground;

        public Color colorCategoryHover;

        public Color colorTopBackground;

        public Color colorScrollBarBackground;

        public Color colorText;

        #endregion 颜色

        #region 样式

        public GUIStyle refreshStyle;

        public GUIStyle editStyle;

        public GUIStyle editHighlightStyle;

        public GUIStyle closeStyle;

        public GUIStyle visiableStyle;

        public GUIStyle nonVisiableStyle;

        public GUIStyle transparentStyle;

        public GUIStyle previousStyle;

        public GUIStyle nextStyle;

        #endregion 样式

        #region 贴图

        public Texture searchTexture = null;

        public Texture sourceLogoTexture = null;

        public Texture refreshTexture = null;

        public Texture refreshHoverTexture = null;

        public Texture editTexture = null;

        public Texture editHoverTexture = null;

        public Texture closeTexture = null;

        public Texture closeHoverTexture = null;

        public Texture visiableTexture = null;

        public Texture visiableHoverTexture = null;

        public Texture nonvisiableTexture = null;

        public Texture nonvisiableHoverTexture = null;

        public Texture nextTexture = null;

        public Texture nextHoverTexture = null;

        public Texture previousTexture = null;

        public Texture previousHoverTexture = null;

        public Texture jsonTexture = null;

        public Texture jsonNonExistTexture = null;

        #endregion 样式和贴图

        /// <summary>
        /// 加载图片
        /// </summary>
        private void LoadTextures()
        {
            this.searchTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "search.png");
            this.sourceLogoTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "logo.png");
            this.refreshTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "refresh.png");

            this.refreshHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "refresh_hover.png");

            this.editTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "edit.png");
            this.editHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "edit_hover.png");

            this.closeTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "close.png");
            this.closeHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "close_hover.png");

            this.visiableTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "visiable.png");
            this.visiableHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "visiable_hover.png");

            this.nonvisiableTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "nonvisiable.png");
            this.nonvisiableHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "nonvisiable_hover.png");

            this.nextTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "next.png");
            this.nextHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "next_hover.png");

            this.previousTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "previous.png");
            this.previousHoverTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "previous_hover.png");

            this.jsonTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "json.png");
            this.jsonNonExistTexture = UICommonFun.LoadFromXDreamer<Texture>("EditorResources/XAssetLib/", "json_non.png");
        }

        /// <summary>
        /// 初始化样式
        /// </summary>
        public void InitStyles()
        {
            LoadTextures();

            #region 从首选项初始化配置信息
            if (AssetLibWindowOption.weakInstance == null) return;

            //窗体背景色
            this.colorText = AssetLibWindowOption.weakInstance.ColorText;

            #endregion 从首选项初始化配置信息

            #region refreshStyle

            refreshStyle = new GUIStyle();
            refreshStyle.normal.background = refreshTexture as Texture2D;
            refreshStyle.onNormal.background = refreshTexture as Texture2D;
            refreshStyle.hover.background = refreshHoverTexture as Texture2D;
            refreshStyle.onHover.background = refreshHoverTexture as Texture2D;

            #endregion refreshStyle

            #region editStyle

            editStyle = new GUIStyle();
            editStyle.normal.background = editTexture as Texture2D;
            editStyle.onNormal.background = editTexture as Texture2D;
            editStyle.hover.background = editHoverTexture as Texture2D;
            editStyle.onHover.background = editHoverTexture as Texture2D;

            #endregion editStyle

            #region editHighlightStyle

            editHighlightStyle = new GUIStyle();
            editHighlightStyle.normal.background = editHoverTexture as Texture2D;
            editHighlightStyle.onNormal.background = editHoverTexture as Texture2D;
            editHighlightStyle.hover.background = editTexture as Texture2D;
            editHighlightStyle.onHover.background = editTexture as Texture2D;

            #endregion editHighlightStyle

            #region closeStyle

            closeStyle = new GUIStyle();
            closeStyle.normal.background = closeTexture as Texture2D;
            closeStyle.onNormal.background = closeTexture as Texture2D;
            closeStyle.hover.background = closeHoverTexture as Texture2D;
            closeStyle.onHover.background = closeHoverTexture as Texture2D;

            #endregion closeStyle

            #region visiableStyle

            visiableStyle = new GUIStyle();
            visiableStyle.normal.background = visiableTexture as Texture2D;
            visiableStyle.onNormal.background = visiableTexture as Texture2D;
            visiableStyle.hover.background = visiableHoverTexture as Texture2D;
            visiableStyle.onHover.background = visiableHoverTexture as Texture2D;

            #endregion visiableStyle

            #region nonVisiableStyle

            nonVisiableStyle = new GUIStyle();
            nonVisiableStyle.normal.background = nonvisiableTexture as Texture2D;
            nonVisiableStyle.onNormal.background = nonvisiableTexture as Texture2D;
            nonVisiableStyle.hover.background = nonvisiableHoverTexture as Texture2D;
            nonVisiableStyle.onHover.background = nonvisiableHoverTexture as Texture2D;

            #endregion nonVisiableStyle

            #region previousStyle

            previousStyle = new GUIStyle();
            previousStyle.normal.background = previousTexture as Texture2D;
            previousStyle.onNormal.background = previousTexture as Texture2D;
            previousStyle.hover.background = previousHoverTexture as Texture2D;
            previousStyle.onHover.background = previousHoverTexture as Texture2D;

            #endregion previousStyle

            #region nextStyle

            nextStyle = new GUIStyle();
            nextStyle.normal.background = nextTexture as Texture2D;
            nextStyle.onNormal.background = nextTexture as Texture2D;
            nextStyle.hover.background = nextHoverTexture as Texture2D;
            nextStyle.onHover.background = nextHoverTexture as Texture2D;

            #endregion nextStyle

            #region transparentStyle

            transparentStyle = new GUIStyle();
            transparentStyle.normal.background = null;

            #endregion transparentStyle
        }
    }

    [Name("搜索类型")]
    [Serializable]
    public enum ESearchType
    {
        [Name("名称")]
        Name,

        [Name("标签")]
        Tag,

        [Name("来源")]
        Source,

        [Name("模糊搜索")]
        Fuzzy,
    }
    [Serializable]
    [Name("排版类型")]
    public enum EPlaceType
    {

        [Name("网格")]
        Grid,

        [Name("列表")]
        List,
    }
}
