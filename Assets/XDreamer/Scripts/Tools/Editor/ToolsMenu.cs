using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.ProjectSettings;
using XCSJ.EditorTools.Inspectors;
using XCSJ.Languages;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.UI;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginTools.GameObjects;
using XCSJ.PluginTools.Inputs;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.Notes;
using XCSJ.PluginTools.Notes.Dimensionings;
using XCSJ.PluginTools.Notes.Tips;
using XCSJ.PluginTools.Points;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginTools.UI;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI.Widgets;
using XCSJ.PluginXGUI.Windows;
using XCSJ.PluginXRSpaceSolution.Base;

namespace XCSJ.EditorTools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        #region 标注
      
        /// <summary>
        /// 缩放式热点
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [Tool("标注", rootType = typeof(ToolsManager), index = 1, groupRule = EToolGroupRule.Create)]
        [Tool("常用", categoryIndex = 0, rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Hotspot)]
        [Name("热点")]
        [Tip("包含移入热点和点击提示热点", "Including move in hotspot and click prompt hotspot")]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(Tip))]
        public static void CreateHotspot(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("热点菜单", m =>
            {
                m.AddMenuItem("移入提示热点", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/移入提示热点.prefab");
                    if (go)
                    {
                        CreateTip(go.GetComponent<Tip>());
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });

                m.AddMenuItem("点击提示热点", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/点击提示热点.prefab");
                    if (go)
                    {
                        CreateTip(go.GetComponent<Tip>());
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });
            });
        }

        /// <summary>
        /// 创建3D标注
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tool("常用", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [Name("批注")]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(UGUILineNote3D))]
        public static void Create3DNote(ToolContext toolContext)
        {
            MenuHelper.DrawMenu("热点菜单", m =>
            {
                m.AddMenuItem("静态批注", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/批注-3D.prefab");
                    if (go)
                    {
                        RectTransform button = null;
                        if (go.GetComponent<UGUILineNote3D>() is UGUILineNote3D line)
                        {
                            button = UGUILineHelper.CreateButtonNote();
                            button.name = "批注文字";
                            button.GetComponentInChildren<Text>().text = "批注文字";
                            button.sizeDelta = new Vector2(80, button.sizeDelta.y);
                            line.rectTransform = button;
                        }

                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                        if (button) GameObjectUtility.EnsureUniqueNameForSibling(button.gameObject);
                    }
                });

                m.AddMenuItem("动画批注", () =>
                {
                    var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/动画批注.prefab");
                    if (go)
                    {
                        EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                    }
                });
            });

            
        }

        /// <summary>
        /// 创建提示
        /// </summary>
        /// <param name="tip"></param>
        public static void CreateTip(Tip tip)
        {
            if (tip)
            {
                var go = EditorXGUI.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/提示.prefab"), "提示组");
                if (go) 
                {
                    tip._tipPopupAsset._view = go.GetComponentInChildren<TipPopup>();
                }
            }
        }

        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tip("点击两点确定一个距离尺寸标注", "Click two points to determine a distance dimension")]
        [Name("距离标注")]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Manual(typeof(DistanceDimensioning))]
        public static void CreateDistanceDimensioningClickPointPicker(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/距离测量.prefab"));
        }

        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.Create)]
        [Tip("点击三个点确定一个角度尺寸标注", "Click three points to determine an angular dimension")]
        [Name("角度标注")]
        [XCSJ.Attributes.Icon(EIcon.Note)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Manual(typeof(AngleDimensioning))]
        public static void CreateAngleDimensioningClickPointPicker(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("标注/角度测量.prefab"));
        }

        [Tool("标注", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
        [Tip("可用于测量长度、高度、角度和面积", "It can be used to measure length, height, angle and area")]
        [Name("测量工具")]
        [XCSJ.Attributes.Icon(EIcon.Tool)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Manual(typeof(SegmentCreater))]
        public static void CreateMeasureTool(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("线段创建器.prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
            var segmentCreater = go.GetComponentInChildren<SegmentCreater>();

            var ugui = XCSJ.EditorXGUI.ToolsMenu.LoadPrefab_DefaultXGUIPath("测量控制窗口.prefab");
            XCSJ.EditorXGUI.ToolsMenu.CreateUIInCanvas(() => ugui);
            ugui.transform.position += new Vector3(-50, 0, 0);
            var segmentViewController = ugui.GetComponentInChildren<SegmentCreaterViewController>();
            segmentViewController._segmentCreater = segmentCreater;
        }

        #endregion

        #region 全息影像

        public const string HolographicImageCameraPath = "全息图像/全息影像相机.prefab";

        public const string HolographicImageCameraName = "全息影像相机";

        [Tool("相机", rootType = typeof(CameraManager))]
        [Name(HolographicImageCameraName)]
        [Tip("将视图划分为斜45度的四个区域，并将4个相机图像投影到对应的4个区域中。", "The view is divided into four areas with an inclination of 45 degrees, and four camera images are projected into the corresponding four areas.")]
        [XCSJ.Attributes.Icon(EIcon.AR)]
        [RequireManager(typeof(CameraManager))]
        [Manual(typeof(Camera))]
        public static void CreateHolographicImageCamera(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath(HolographicImageCameraPath));
        }

        #endregion

        #region 拖拽功能

        /// <summary>
        /// 可抓对象列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("可抓对象列表")]
        [Tip("可抓对象列表", "Grabbable Object list")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [Tool(XGUICategory.Table, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("游戏对象", nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager))]
        [Manual(typeof(GrabbableList))]
        public static GameObject CreateGrabbableListWindow(ToolContext toolContext)
        {
            var gameObjectList = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("可抓对象列表.prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, gameObjectList);
            return gameObjectList;
        }

        /// <summary>
        /// 创建一键拖拽工具(拥有相机射线摆放、相机视图平面平移、三轴平移旋转缩放等功能)
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("一键拖拽工具")]
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager), index = 1)]
        [Tool("常用", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Tip("拥有相机射线摆放、相机视图平面平移、三轴平移旋转缩放等功能", "It has the functions of camera ray placement, camera view plane translation, three-axis translation, rotation and zoom")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(CombinationDragToolController))]
        public static void CreateDragTool(ToolContext toolContext)
        {
            CreateDragTool(toolContext, true);
        }

        /// <summary>
        /// 为XR空间中的左右手创建一键拖拽工具功能
        /// </summary>
        /// <param name="XRSpaceGameObject"></param>
        public static void CreateDragToolForXRSpace(GameObject XRSpaceGameObject,Transform left,Transform right)
        {
            CreateDragToolForXRSpaceInternal(XRSpaceGameObject, left, "左", new Vector3(-500, -10, 0));
            CreateDragToolForXRSpaceInternal(XRSpaceGameObject, right, "右", new Vector3(500, -10, 0));
        }

        private static void CreateDragToolForXRSpaceInternal(GameObject XRSpaceGameObject, Transform handController, string windowTitle, Vector3 position)
        {
            if (!handController) return;

            var go = CreateDragTool(null, true, viewController =>
            {
                var window = viewController.GetComponentInChildren<UGUIWindow>();
                if (window)
                {
                    window.titleText = windowTitle;
                    window.positon = position;
                }
            });
            if (go)
            {
                go.XSetParent(handController);
                go.transform.XResetLocalPRS();

                // 射线拖拽工具默认使用固定距离拖拽
                foreach (var dragByRay in go.GetComponentsInChildren<DragByRay>())
                {
                    dragByRay._rayDragRule = DragByRay.ERayDragRule.FixedDistance;
                }

                // 将所有鼠标输入改成模拟输入模式
                foreach (var mouseInput in go.GetComponentsInChildren<MouseInput>())
                {
                    mouseInput.SetInputMode(RayCmd.EInputMode.Analog);
                }

                // 设置一键拖拽射线对象为控制器对象
                var cdController = go.GetComponentInChildren<CombinationDragToolController>();
                if (cdController && cdController.analogMouseInputController)
                {
                    // 隐藏原来的射线表示对象
                    var oldRayTransform = cdController.analogMouseInputController.rayTransform;
                    if (oldRayTransform && oldRayTransform != handController.transform)
                    {
                        oldRayTransform.gameObject.SetActive(false);
                    }

                    if (cdController.analogMouseInputController)
                    {
                        cdController.analogMouseInputController.rayTransform = handController.transform;

                        var input = cdController.analogMouseInputController.analogMouseInput;
                        if (input)
                        {
                            // 设置模拟鼠标输入的点和方向对象
                            input._rayGenerater.SetPointDirectionTarget(handController.transform);
                        }

                        // 设置广告牌追踪对象
                        var billboard = cdController.analogMouseInputController.GetComponentInChildren<Billboard>();
                        if (billboard)
                        {
                            billboard._targetTransform = handController.transform;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建射线拖拽摆放工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name("射线拖拽摆放工具")]
        [Tip("创建[选择集修改器]、[射线拖拽摆放工具]和[选择集渲染器]三个组件,实现拖拽功能。", "One click to create [selection set modifier], [selection set drag tool] and [selection set boundary renderer] to realize the drag function.")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(DragByRay))]
        public static void CreateSelectionDragByRay(ToolContext toolContext)
        {
            CreateDragTool(toolContext, false, controller => controller.dragByRay = true);
        }

        /// <summary>
        /// 创建相机视图平面拖拽工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name("相机视图平面拖拽工具")]
        [Tip("创建[选择集修改器]、[相机视图平面拖拽工具]和[选择集渲染器]三个组件,实现拖拽功能。", "One click to create [selection set modifier], [selection set drag tool] and [selection set boundary renderer] to realize the drag function.")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(DragByCameraView))]
        public static void CreateSelectionDragToolByOnce(ToolContext toolContext)
        {
            CreateDragTool(toolContext, false, controller => controller.dragByCameraView = true);
        }

        /// <summary>
        /// 创建平移旋转缩放工具
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name("平移旋转缩放工具")]
        [Tip("创建[选择集修改器]、[平移旋转缩放工具]和[选择集渲染器]三个组件,实现拖拽功能。", "One click to create [selection set modifier], [selection set drag tool] and [selection set boundary renderer] to realize the drag function.")]
        [XCSJ.Attributes.Icon(EIcon.Drag)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(TRSHandle))]
        public static void CreateSelectionTRSTool(ToolContext toolContext)
        {
            CreateDragTool(toolContext, false, controller => controller.trsHandleMove = true);
        }

        private static GameObject CreateDragTool(ToolContext toolContext, bool activeUI = true, Action<CombinationDragToolViewController> onCreateFun = null)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("一键拖拽工具.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);

                var viewController = go.GetComponentInChildren<CombinationDragToolViewController>();
                onCreateFun?.Invoke(viewController);

                viewController.gameObject.XSetActive(activeUI);
            }
            return go;
        }

        [Tool("渲染器", disallowMultiple = true, rootType = typeof(ToolsManager))]
        [Name(RenderTextureCamera.Title)]
        [Tip("用于表现选择游戏对象的效果", "Used to represent the effect of selecting game objects")]
        [XCSJ.Attributes.Icon(EIcon.Camera)]
        [RequireManager(typeof(ToolsManager))]
        [Manual(typeof(RenderTextureCamera))]
        public static RenderTextureCamera CreateRenderTextureCamera(ToolContext toolContext)
        {
            var rt = GameObject.FindObjectOfType<RenderTextureCamera>();
            if (!rt)
            {
                var go = UnityObjectHelper.CreateGameObject(RenderTextureCamera.Title);
                if (go)
                {
                    go.XSetParent(ToolsManager.instance.gameObject);

                    // 相机专用渲染图层
                    XTagManager.AddLayer(RenderTextureCamera.Layer);
                    rt = go.XAddComponent<RenderTextureCamera>();
                    rt.renderCamera.cullingMask = 1 << LayerMask.NameToLayer(RenderTextureCamera.Layer);

                    EditorGUIUtility.PingObject(go);
                }
            }
            return rt;
        }

        [Name("材质列表")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [RequireManager(typeof(XGUIManager))]
        [Tool(XGUICategory.Table, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Manual(typeof(MaterialList))]
        public static void CreateMaterialList(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("材质列表.prefab"));
        }

        #endregion
    }
}
