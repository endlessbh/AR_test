using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.Kernel;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.Notes;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Views.Sliders;
using XCSJ.PluginXGUI.Views.Texts;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows;
using XCSJ.PluginXGUI.Windows.ColorPickers;
using XCSJ.PluginXGUI.Windows.ImageBrowers;
using XCSJ.PluginXGUI.Windows.MiniMaps;
using XCSJ.PluginXGUI.Windows.Weathers;

namespace XCSJ.EditorXGUI
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 缩放UI类型工具
        /// </summary>
        /// <param name="toolContext"></param>
        /// <param name="createTool"></param>
        public static void CreateUIToolAndStretchHV(ToolContext toolContext, Func<GameObject> createTool)
        {
            if (toolContext==null || createTool==null) return;

            var go = createTool.Invoke();
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                // 纠正坐标
                var rectTransform = go.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.XStretchHV();
                }
            }
        }

        /// <summary>
        /// 创建UI元素：
        /// 在父节点为空的画布下创建组和UI元素，并将UI元素挂接在组下
        /// 当组名为空时，不创建组，直接将UI元素挂在画布下
        /// </summary>
        /// <param name="createUI"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static RectTransform CreateUIInCanvas(Func<GameObject> createUI, string groupName = "")
        {
            if (createUI == null) return null;

            var canvas = EditorXGUIHelper.FindOrCreateRootCanvas();
            if (!canvas) return null;

            var go = createUI.Invoke();
            if (go)
            {
                var parent = canvas;
                if (!string.IsNullOrEmpty(groupName))
                {
                    var group = GameObject.Find(groupName);
                    if (!group)
                    {
                        group = EditorHandler.Create(groupName, canvas.transform);
                    }
                    if (group)
                    {
                        parent = group;
                    }
                }
                // 如果场景中选中的游戏对象在根画布下，则设置为父级
                else if (Selection.activeGameObject && Selection.activeGameObject.transform.IsChildOf(canvas.transform))
                {
                    parent = Selection.activeGameObject;
                }
                go.XSetParent(parent);
                go.XModifyProperty(() => GameObjectUtility.EnsureUniqueNameForSibling(go));
                var rect = go.GetComponent<RectTransform>();
                rect.XCenterHV();

                EditorGUIUtility.PingObject(go);
                return rect;
            }
            return null;
        }

        /// <summary>
        /// 加载XGUI预制体
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab_DefaultXGUIPath(string path)
        {
            if (!path.EndsWith(".prefab"))
            {
                path += ".prefab";
            }
            return EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("XGUI/" + path);
        }

        /// <summary>
        /// 创建Unity输入系统
        /// </summary>
        /// <returns></returns>
        public static EventSystem CreateEventSystem()
        {
            // 添加UI事件系统
            var es = ComponentCache.GetComponent<EventSystem>(true);
            if (!es)
            {
                var go = UnityObjectHelper.CreateGameObject(nameof(EventSystem));
                if (go)
                {
                    go.XAddComponent<StandaloneInputModule>();
                    return go.XGetOrAddComponent<EventSystem>();
                }
            }
            return default;
        }

        /// <summary>
        /// 创建弹出提示框游戏对象
        /// </summary>
        public static void EnsureTipPopupExist()
        {
            if (inLoading || !XGUIManager.instance) return;

            var asset = XGUIManager.instance.XGetOrAddComponent<XGUICommonAssets>();
            if (asset._tipPopup) return;

            inLoading = true;
            try
            {
                var go = CreateCanvasAndLoadPrefabByXGUIPath(TipPopupName);
                if (go)
                {
                    var tipPopup = go.GetComponentInChildren<TipPopup>();
                    if (tipPopup)
                    {
                        asset.XModifyProperty(() => asset._tipPopup = tipPopup);
                    }
                }
            }
            finally
            {
                inLoading = false;
            }
        }

        private static bool inLoading = false;

        /// <summary>
        /// 数据视图模版列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("数据视图模版列表")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        //[Tool(XGUICategory.DataTypeView, nameof(XGUIManager), rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        public static void CreateDataViewTemplateList(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("DataViewTemplates/数据视图模版列表.prefab"));
        }

        /// <summary>
        /// 创建画布并使用默认XGUI预制体默认路径创建对象
        /// </summary>
        /// <param name="path"></param>
        public static RectTransform CreateCanvasAndLoadPrefabByXGUIPath(string prefabName)
        {
            return CreateUIInCanvas(() => XCSJ.EditorXGUI.ToolsMenu.LoadPrefab_DefaultXGUIPath(prefabName));
        }

        #region 画布

        /// <summary>
        /// 加载场景画布
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("加载场景画布")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(Canvas))]
        public static void CreateLoadSceneCanvas(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("加载场景画布.prefab"));
        }

        #endregion

        #region Window

        /// <summary>
        /// 创建全屏窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("全屏窗口")]
        [Tip("在根窗口下，依赖画布的对象", "In the root window, select the object that depends on the canvas")]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        //[Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(XRootWindow), index = 0)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(UGUIWindow))]
        public static void CreateWindow(ToolContext toolContext)
        {
            var xrw = FindOrCreateXRootWindow();
            if (xrw)
            {
                var go = UnityObjectHelper.CreateGameObject("全屏窗口");
                var rectTransform = go.XAddComponent<RectTransform>();
                go.XAddComponent<Window>();
                go.XAddComponent<Image>();

                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                // 纠正坐标
                rectTransform.XStretchHV();
            }
        }

        /// <summary>
        /// 查找和创建根窗口对象
        /// </summary>
        /// <returns></returns>
        public static RootWindow FindOrCreateXRootWindow()
        {
            var xrw = UnityEngine.Object.FindObjectOfType<RootWindow>() as RootWindow;
            if (!xrw)
            {
                var go = EditorXGUIHelper.FindOrCreateRootCanvas();
                Undo.AddComponent<RootWindow>(go);
            }
            return xrw;
        }

        #endregion

        #region SubWindow

        /// <summary>
        /// 即在
        /// </summary>
        /// <param name="title"></param>
        /// <param name="window"></param>
        /// <param name="titleText"></param>
        /// <param name="contentText"></param>
        /// <returns></returns>
        public static GameObject LoadWindowPrefab(string title, out UGUIWindow window, out Text titleText, out Text contentText)
        {
            window = default;
            titleText = default;
            contentText = default;

            var windowGO = LoadPrefab_DefaultXGUIPath("窗口模版.prefab");
            if (windowGO)
            {
                windowGO.XSetName(title);
                window = windowGO.GetComponent<UGUIWindow>();
                if (window)
                {
                    if (window.title)
                    {
                        var titleBar = window.title.GetComponent<TitleBar>();
                        if (titleBar && titleBar._title)
                        {
                            titleText = titleBar._title.GetComponent<Text>();
                            if (titleText)
                            {
                                titleText.text = title;
                            }
                        }
                    }
                    if (window.content)
                    {
                        var textTransform = window.content.Find("Text");
                        if (textTransform)
                        {
                            contentText = textTransform.GetComponent<Text>();
                        }
                    }
                }
            }
            return windowGO;
        }

        /// <summary>
        /// 垂直收缩窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("窗口模版")]
        [Tip("窗口展开和收缩方向为垂直方向", "The window expands and contracts vertically")]
        [XCSJ.Attributes.Icon(EIcon.Window)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), index = 1, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(UGUIWindow))]
        public static void CreateVerticalWindow(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("窗口模版.prefab"));
        }

        /// <summary>
        /// 全息影像UI
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("全息影像UI")]
        [Tip("将视图划分为斜45度的四个区域，并将4个相机图像投影到对应的4个区域中。", "The view is divided into four areas with an inclination of 45 degrees, and four camera images are projected into the corresponding four areas.")]
        [XCSJ.Attributes.Icon(EIcon.AR)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Canvas))]
        public static void CreateHolographicImageUGUI(ToolContext toolContext)
        {
            CreateUIToolAndStretchHV(toolContext, () => EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("全息图像/全息影像UI.prefab"));
        }

        /// <summary>
        /// 调色板
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("调色板")]
        [Tip("通过拾取面板颜色改变模型材质颜色", "Change the model material color by picking the panel color")]
        [XCSJ.Attributes.Icon(EIcon.ColorPicker)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(ColorPicker))]
        public static void CreateColorPicker(ToolContext toolContext)
        {
            // 如果通用资源中没有对话框则设置
            var go = LoadPrefab_DefaultXGUIPath("调色板.prefab");
            XGUICommonAssets.SetAssetIfEmpty<ColorPicker>(go);
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        /// <summary>
        /// 导航图
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("导航图")]
        [Tip("导航图", "Navigation map")]
        [XCSJ.Attributes.Icon(EIcon.MiniMap)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
        [Tool("导航图", nameof(MiniMap), rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(XGUIManager), groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(MiniMap))]
        public static void CreateMiniMap(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("导航图.prefab"));
        }

        /// <summary>
        /// 图片浏览器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("图片浏览器")]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [Tip("基于UGUI的用于图片浏览的控件", "Ugui based control for picture browsing")]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(ImageBrower))]
        public static void CreatePictureBrowser(ToolContext toolContext)
        {
            ToolsMenu.CreateUIToolAndStretchHV(toolContext, () => LoadPrefab_DefaultXGUIPath("图片浏览器.prefab"));
        }

        /// <summary>
        /// 天气面板
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("天气面板")]
        [Tip("显示指定城市的天气状况", "Displays the weather conditions for the specified city")]
        [XCSJ.Attributes.Icon(EIcon.Weather)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Weather))]
        public static void CreateWeatherPanel(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("天气面板.prefab"));
        }

        /// <summary>
        /// 变换属性窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("变换属性窗口")]
        [Tip("用于显示变换的位置、旋转和缩放数值的窗口", "A window that displays the position, rotation, and scale values of the transform")]
        [XCSJ.Attributes.Icon(EIcon.GameObject)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(Transform))]
        public static void CreateTransformInspectorWindow(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("变换属性窗口.prefab"));
        }

        /// <summary>
        /// 游戏对象属性窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("游戏对象属性窗口")]
        [Tip("用于显示游戏对象下的组件及组件公有字段、属性和空参数方法的窗口", "A window used to display components under the game object and component public fields, properties, and null parameter methods")]
        [XCSJ.Attributes.Icon(EIcon.GameObject)]
        //[Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        public static void CreateGameObjectInspectorWindow(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("游戏对象属性窗口.prefab"));
        }

        /// <summary>
        /// 灯光属性窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("灯光属性窗口")]
        [Tip("用于显示灯光的组件的角度、颜色和强度的窗口", "A window that displays the angle, color, and intensity of the light's components")]
        [XCSJ.Attributes.Icon(EIcon.VehicleLight)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(Light))]
        public static void CreateLightInspectorWindow(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("灯光属性窗口.prefab"));
        }

        #endregion

        #region 小功能窗口

        /// <summary>
        /// 弹出菜单窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("弹出菜单")]
        [Tip("基于交互系统弹出菜单执行", "Pop up menu execution based on interactive system")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(MenuGenerator))]
        public static void CreatePopupMenu(ToolContext toolContext)
        {
            // 如果通用资源中没有弹出菜单则设置
            var go = LoadPrefab_DefaultXGUIPath("弹出菜单.prefab");
            XGUICommonAssets.SetAssetIfEmpty<MenuGenerator>(go);
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        /// <summary>
        /// 日志窗口
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("日志窗口")]
        [Tip("用于显示日志的窗口", "Window for displaying log")]
        [XCSJ.Attributes.Icon(EIcon.Log)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(LogViewController))]
        public static void CreateLogWindow(ToolContext toolContext)
        {
            // 如果通用资源中没有日志窗口则设置
            var go = LoadPrefab_DefaultXGUIPath("日志窗口.prefab");
            XGUICommonAssets.SetAssetIfEmpty<LogViewController>(go);
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }


        /// <summary>
        /// 对话框
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("对话框")]
        [XCSJ.Attributes.Icon(EIcon.Chat)]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(DialogBox))]
        public static void CreateDialogBox(ToolContext toolContext)
        {
            // 如果通用资源中没有对话框则设置
            var go = LoadPrefab_DefaultXGUIPath("对话框.prefab");
            XGUICommonAssets.SetAssetIfEmpty<DialogBox>(go);
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
        }

        #endregion

        #region 控件

        /// <summary>
        /// 时间文本
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("时间文本")]
        [Tip("显示当前时间的文本", "Displays the text for the current time")]
        [XCSJ.Attributes.Icon(EIcon.Timer)]
        [Tool(XGUICategory.View, nameof(XGUIManager), rootType = typeof(Canvas), index = 2, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(XGUIManager), rootType = typeof(Canvas), index = 2, needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(DateTimeText))]
        public static void CreateDataTimeText(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("时间文本.prefab"));
        }

        /// <summary>
        /// 输入框滑动条
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("输入框滑动条")]
        [Tip("滑动条与输入框关联，修改两者任意一个，另外一个数值也将发生变换", "The slider is associated with the input box. If you modify either of them, the other value will also change")]
        [XCSJ.Attributes.Icon(EIcon.Slider)]
        [Tool(XGUICategory.View, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(SliderToInputFieldValidator))]
        public static void CreateInputFieldSlider(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath("输入框滑动条.prefab"));
        }

        /// <summary>
        /// 提示弹出框名称
        /// </summary>
        public const string TipPopupName = "提示弹出框";

        /// <summary>
        /// 提示弹出框
        /// </summary>
        /// <param name="toolContext"></param>
        [Name(TipPopupName)]
        [Tip("鼠标悬停在UI或碰撞体上弹出提示框", "Hover the mouse over the UI or collision body to pop up a prompt box")]
        [XCSJ.Attributes.Icon(EIcon.Tip)]
        [Tool(XGUICategory.View, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(TipPopup))]
        public static void CreateTipPopup(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, LoadPrefab_DefaultXGUIPath(TipPopupName));
        }

        #endregion

        #region UnityUGUI

        private const string UnityUGUI = "UGUI";

        /// <summary>
        /// 文本
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("文本")]
        [XCSJ.Attributes.Icon(EIcon.Text)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 1, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Text))]
        public static void CreateText(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Text>().gameObject);
        }

        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("按钮")]
        [XCSJ.Attributes.Icon(EIcon.Button)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 2, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Button))]
        public static void CreateButton(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Button>().gameObject);
        }

        /// <summary>
        /// 切换
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("切换")]
        [XCSJ.Attributes.Icon(EIcon.Toggle)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 3, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Toggle))]
        public static void CreateToggle(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Toggle>().gameObject);
        }

        /// <summary>
        /// 精灵图像
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("精灵图像")]
        [XCSJ.Attributes.Icon(EIcon.RawImage)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 4, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Image))]
        public static void CreateImage(ToolContext toolContext)
        {
            CreateUIInCanvas(() => EditorXGUIHelper.CreateUGUI<Image>().gameObject);
        }

        /// <summary>
        /// 贴图图像
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("贴图图像")]
        [XCSJ.Attributes.Icon(EIcon.RawImage)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, index = 5, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(RawImage))]
        public static void CreateRawImage(ToolContext toolContext)
        {
            CreateUIInCanvas(() => EditorXGUIHelper.CreateUGUI<RawImage>().gameObject);
        }

        /// <summary>
        /// 下拉列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("下拉列表")]
        [XCSJ.Attributes.Icon(EIcon.Dropdown)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Dropdown))]
        public static void CreateDropdown(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Dropdown>().gameObject);
        }

        /// <summary>
        /// 输入框
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("输入框")]
        [XCSJ.Attributes.Icon(EIcon.InputField)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(InputField))]
        public static void CreateInputField(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<InputField>().gameObject);
        }

        /// <summary>
        /// 面板
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("面板")]
        [XCSJ.Attributes.Icon(EIcon.RawImage)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Image))]
        public static void CreatePanel(ToolContext toolContext)
        {
            var rect = CreateUIInCanvas(() => LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Panel.prefab")));
            if (rect)
            {
                rect.XStretchHV();
            }
        }

        /// <summary>
        /// 滚动条
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("滚动条")]
        [XCSJ.Attributes.Icon(EIcon.ScrollBar)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Scrollbar))]
        public static void CreateScrollbar(ToolContext toolContext)
        {
            CreateUIInCanvas(() => EditorXGUIHelper.CreateUGUI<Scrollbar>().gameObject);
        }

        /// <summary>
        /// 滚动视图
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("滚动视图")]
        [XCSJ.Attributes.Icon(EIcon.ScrollBar)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(ScrollRect))]
        public static void CreateScrollView(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<ScrollRect>().gameObject);
        }

        /// <summary>
        /// 滑动条
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("滑动条")]
        [XCSJ.Attributes.Icon(EIcon.Slider)]
        [Tool(UnityUGUI, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
        [Manual(typeof(Slider))]
        public static void CreateSlider(ToolContext toolContext)
        {
            CreateUIInCanvas(() => CreateUIWithStyle<Slider>().gameObject);
        }

        private static string GetUnityUGUIPath(string fileName) => "UnityDefaultUI/" + fileName;

        /// <summary>
        /// 创建带风格的UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateUIWithStyle<T>() where T : UIBehaviour
        {
            GameObject go = null;
            if (typeof(T) == typeof(Button))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Button.prefab"));
            }
            else if (typeof(T) == typeof(Dropdown))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Dropdown.prefab"));
            }
            else if (typeof(T) == typeof(InputField))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("InputField.prefab"));
            }
            else if (typeof(T) == typeof(ScrollRect))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("ScrollView.prefab"));
            }
            else if (typeof(T) == typeof(Slider))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Slider.prefab"));
            }
            else if (typeof(T) == typeof(Text))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Text.prefab"));
            }
            else if (typeof(T) == typeof(Toggle))
            {
                go = LoadPrefab_DefaultXGUIPath(GetUnityUGUIPath("Toggle.prefab"));
            }

            var ui = default(T);
            if (go)
            {
                UndoHelper.RegisterCreatedObjectUndo(go);
                ui = go.GetComponent<T>();
            }

            return ui;
        }

        #endregion
    }
}
