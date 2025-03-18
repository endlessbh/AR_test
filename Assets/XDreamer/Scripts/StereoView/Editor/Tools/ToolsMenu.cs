using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools.Renderers;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXBox.Tools;
using System;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.Languages;
using XCSJ.PluginZVR;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 虚拟屏幕
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(StereoViewHelper.Title, nameof(StereoViewManager), rootType = typeof(StereoViewManager), groupRule = EToolGroupRule.None)]
        [Name(VirtualScreen.Title)]
        [Tip("现实世界屏幕在虚拟世界中的虚拟对象", "Virtual object of real world screen in virtual world")]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [RequireManager(typeof(StereoViewManager))]
        [Manual(typeof(VirtualScreen))]
        public static void CreateScreen(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, VirtualScreen.CreateScreen().gameObject);
        }

        /// <summary>
        /// 相机透视:根据屏幕与相机位置实时更新相机透视矩阵的工具组件
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraEntityController))]
        [Name(CameraProjection.Title)]
        [Tip("根据屏幕与相机位置实时更新相机透视矩阵的工具组件", "Tool component for updating camera perspective matrix in real time according to screen and camera position")]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [RequireManager(typeof(StereoViewManager), typeof(CameraManager))]
        [Manual(typeof(CameraEntityController))]
        public static void CreateCameraProjection(ToolContext toolContext)
        {
            var gameObject = Selection.activeGameObject;
            if (!gameObject)
            {
                Debug.LogWarning("请选择具有【Camera】或【CameraEntityController】组件的游戏对象！");
                return;
            }

            var cameras = new List<Camera>();
            var entity = gameObject.GetComponent<CameraEntityController>();
            if (entity)
            {
                cameras.AddRange(entity.cameras);
            }

            if (gameObject.GetComponent<Camera>() is Camera cam && cam)
            {
                cameras.Add(cam);
            }

            if (cameras.Count > 0)
            {
                cameras.ForEach(c => c.XGetOrAddComponent<CameraProjection>());
            }
            else
            {
                Debug.LogWarning("请选择具有【Camera】或【CameraEntityController】组件的游戏对象！");
            }
        }
    }
}
