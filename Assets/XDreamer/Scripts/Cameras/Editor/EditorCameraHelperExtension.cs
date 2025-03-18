using System;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Kernel;

namespace XCSJ.EditorCameras
{
    /// <summary>
    /// 编辑器相机组手扩展
    /// </summary>
    public class EditorCameraHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            DefaultCameraHandler.Init();
        }

        /// <summary>
        /// 绘制选中<see cref="CameraManager"/>
        /// </summary>
        public static void DrawSelectCameraManager()
        {
            //EditorGUILayout.Separator();

            if (GUILayout.Button("选中[" + CameraHelperExtension.Title + "]管理器") && CameraManager.instance)
            {
                Selection.activeObject = CameraManager.instance;
            }
        }

        /// <summary>
        /// 绘制设置当前相机为初始化相机
        /// </summary>
        /// <param name="getCurrentCamera"></param>
        public static void DrawCurrentCameraAsInitCamera(Func<BaseCameraMainController> getCurrentCamera)
        {
            if (GUILayout.Button("设置当前相机为初始相机"))
            {
                var provider = CameraManager.instance.GetProvider();
                if (!provider) return;

                var camera = getCurrentCamera?.Invoke();
                if (!camera) return;

                provider.initCameraController = camera;
            }
        }

        /// <summary>
        /// 与视图对齐
        /// </summary>
        /// <param name="transform"></param>
        public static void AlignWithView(Transform transform)
        {
            if (transform && GUILayout.Button("与视图对齐"))
            {
                var view = SceneView.lastActiveSceneView;
                if (view.camera)
                {
                    transform.XSetPosition(view.camera.transform.position);
                    transform.XSetRotation(view.camera.transform.rotation);
                }
            }
        }
    }
}
