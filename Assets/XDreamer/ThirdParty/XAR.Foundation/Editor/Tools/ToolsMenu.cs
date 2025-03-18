using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginXAR.Foundation;
using XCSJ.PluginXAR.Foundation.Images.Tools;
using System;
using XCSJ.Collections;
using System.Linq;
using XCSJ.PluginXAR.Foundation.Base.Tools;
using XCSJ.Languages;
using XCSJ.PluginXAR.Foundation.Planes.Tools;
using XCSJ.PluginXAR.Foundation.Faces.Tools;
using XCSJ.PluginsCameras;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
#endif

namespace XCSJ.EditorXAR.Foundation.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        private static void Create<TTracker>(ToolContext toolContext, Type managerType) where TTracker : BaseTracker
        {
#if XDREAMER_AR_FOUNDATION

            if (!UnityEngine.Object.FindObjectOfType<ARSession>())
            {
                EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session");
            }

            if (!UnityEngine.Object.FindObjectsOfType<ARSessionOrigin>().Any(origin => !origin.transform.parent))
            {
                EditorApplication.ExecuteMenuItem("GameObject/XR/AR Session Origin");
            }

            var gameObject = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(XARFoundationHelper.Title + "/" + CommonFun.Name(typeof(TTracker), ELanguageType.Chinese) + ".prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, gameObject, out GameObject root, out GameObject group);

            if(root)//AR Session Origin对象
            {
                root.XGetOrAddComponent(managerType);
                var camera = root.GetComponentInChildren<Camera>();
                if (string.IsNullOrEmpty(camera.tag) || camera.tag == "Untagged")//设置相机层
                {
                    camera.tag = CameraHelperExtension.DefaultMainCameraTag;
                }
            }

            if (gameObject)
            {
                var tracker = gameObject.GetComponent<TTracker>();
                if (tracker) tracker.Reset();
            }  
            
            CameraHelperExtension.HandleDefaultMainCamera();
            
#else
            Debug.LogFormat("当前工程缺失插件[{0}]所需的包或是版本不匹配！", XARFoundationHelper.Title);
#endif
        }

        /// <summary>
        /// 图像跟踪器
        /// </summary>
        /// <param name="toolContext"></param>
#if XDREAMER_AR_FOUNDATION
        [Tool(XARFoundationHelper.Title, nameof(XARFoundationManager), rootType = typeof(ARSessionOrigin), needRootParentIsNull = true, groupRule = EToolGroupRule.Create)]
#else
        [Tool(XARFoundationHelper.Title, nameof(XARFoundationManager), groupRule = EToolGroupRule.Create)]
#endif
        [Name(ImageTracker.Title)]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [RequireManager(typeof(XARFoundationManager))]
        public static void CreateImageTracker(ToolContext toolContext)
        {
#if XDREAMER_AR_FOUNDATION
            Create<ImageTracker>(toolContext, typeof(ARTrackedImageManager));
#else            
            Create<ImageTracker>(toolContext, null);
#endif
        }

        /// <summary>
        /// 平面跟踪器
        /// </summary>
        /// <param name="toolContext"></param>
#if XDREAMER_AR_FOUNDATION
        [Tool(XARFoundationHelper.Title, nameof(XARFoundationManager), rootType = typeof(ARSessionOrigin), needRootParentIsNull = true, groupRule = EToolGroupRule.Create)]
#else
        [Tool(XARFoundationHelper.Title, nameof(XARFoundationManager), groupRule = EToolGroupRule.Create)]
#endif
        [Name(PlaneTracker.Title)]
        [XCSJ.Attributes.Icon(EIcon.WireFrame)]
        [RequireManager(typeof(XARFoundationManager))]
        public static void CreatePlaneTracker(ToolContext toolContext)
        {
#if XDREAMER_AR_FOUNDATION
            Create<PlaneTracker>(toolContext, typeof(ARPlaneManager));
#else            
            Create<PlaneTracker>(toolContext, null);
#endif
        }

        /// <summary>
        /// 面部跟踪器
        /// </summary>
        /// <param name="toolContext"></param>
#if XDREAMER_AR_FOUNDATION
        [Tool(XARFoundationHelper.Title, nameof(XARFoundationManager), rootType = typeof(ARSessionOrigin), needRootParentIsNull = true, groupRule = EToolGroupRule.Create)]
#else
        [Tool(XARFoundationHelper.Title, nameof(XARFoundationManager), groupRule = EToolGroupRule.Create)]
#endif
        [Name(FaceTracker.Title)]
        [XCSJ.Attributes.Icon(EIcon.Authentication)]
        [RequireManager(typeof(XARFoundationManager))]
        public static void CreateFaceTracker(ToolContext toolContext)
        {
#if XDREAMER_AR_FOUNDATION
            Create<FaceTracker>(toolContext, typeof(ARFaceManager));
#else            
            Create<FaceTracker>(toolContext, null);
#endif
        }
    }
}
