using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCameras;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginTools.Cameras;

namespace XCSJ.EditorTools.Cameras
{
    /// <summary>
    /// 相机背景幕布检查器
    /// </summary>
    [Name("相机背景幕布检查器")]
    [CustomEditor(typeof(CameraBackdrop))]
    public class CameraBackdropInspector : ToolMBForUnityEditorSelectionInspector<CameraBackdrop>
    {
        private static CameraBackdrop GetOrAddModelBackdrop(Camera camera)
        {
            if (!camera) return null;

            var backdrop = camera.GetComponentInChildren<CameraBackdrop>(true);
            if (backdrop) return backdrop;

            var go = UnityObjectHelper.CreateGameObject(CommonFun.Name(typeof(CameraBackdrop)));
            go.XSetParent(camera.gameObject);
            go.transform.XResetLocalPRS();
            backdrop = go.XAddComponent<CameraBackdrop>();
            var renderer = backdrop.GetComponent<Renderer>();
            renderer.material = AssetDatabase.LoadAssetAtPath(UICommonFun.Assets + "/XDreamer-Assets/基础/Materials/常用/Grid.mat", typeof(Material)) as Material;
            return backdrop;
        }

        private static Image GetOrAddImageBackdrop(Camera camera)
        {
            if (!camera) return null;

            var canvas = camera.XCreateChild<Canvas>();
            canvas.XAddComponent<CanvasScaler>();
            //canvas.XAddComponent<GraphicRaycaster>();
            canvas.XModifyProperty(() =>
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
            });

            var image = canvas.XCreateChild<RawImage>();// 使用贴图
            image.rectTransform.XStretchHV();

            return null;
        }

        /// <summary>
        /// 创建相机幕布
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Tool(CameraHelperExtension.ControllersCategoryName, nameof(CameraEntityController))]
        [Name(CameraBackdrop.Title)]
        [XCSJ.Attributes.Icon(EIcon.Image)]
        [RequireManager(typeof(CameraManager))]
        public static void Create(ToolContext toolContext)
        {
            MenuHelper.DrawMenu(nameof(CameraBackdrop), m =>
            {
                m.AddMenuItem("图像背景幕布(推荐)", CreateImageBackdrop);
                m.AddMenuItem("模型背景幕布", CreateModelBackdrop);
            });

            var gameObject = Selection.activeGameObject;
           
        }

        private static bool TryGetCameras(GameObject gameObject ,out List<Camera> cameras)
        {
            if (!gameObject)
            {
                Debug.LogWarning("请选择具有【Camera】或【CameraEntityController】组件的游戏对象！");
                cameras = default;
                return false;
            }

            cameras = new List<Camera>();
            var entity = gameObject.GetComponent<CameraEntityController>();
            if (entity)
            {
                cameras.AddRange(entity.cameras);
            }

            if (gameObject.GetComponent<Camera>() is Camera cam && cam)
            {
                cameras.Add(cam);
            }
            return cameras.Count > 0;
        }

        private static void CreateModelBackdrop() => CreateModelBackdrop(Selection.activeGameObject);

        private static void CreateModelBackdrop(GameObject gameObject)
        {
            if (TryGetCameras(gameObject, out var cameras))
            {
                cameras.ForEach(c => GetOrAddModelBackdrop(c));
            }
            else
            {
                Debug.LogWarning("请选择具有【Camera】或【CameraEntityController】组件的游戏对象！");
            }
        }

        private static void CreateImageBackdrop() => CreateImageBackdrop(Selection.activeGameObject);

        private static void CreateImageBackdrop(GameObject gameObject)
        {
            if (TryGetCameras(gameObject, out var cameras))
            {
                cameras.ForEach(c => GetOrAddImageBackdrop(c));
            }
            else
            {
                Debug.LogWarning("请选择具有【Camera】或【CameraEntityController】组件的游戏对象！");
            }
        }
    }
}
