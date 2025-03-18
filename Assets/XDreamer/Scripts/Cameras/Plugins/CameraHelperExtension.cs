using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Characters;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginsCameras.Tools.Controllers;
using ERotateMode = XCSJ.PluginsCameras.Controllers.ERotateMode;

namespace XCSJ.PluginsCameras
{
    /// <summary>
    /// 相机组手扩展
    /// </summary>
    public static class CameraHelperExtension
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机";

        /// <summary>
        /// 工具库中相机目录名称
        /// </summary>
        public const string CategoryName = Title;

        /// <summary>
        /// 工具库中相机控制目录名称
        /// </summary>
        public const string ControllersCategoryName = "相机组件";

        /// <summary>
        /// 工具库中相机控制目录名称
        /// </summary>
        public const string ControllersCategoryName_Move = "相机组件-移动";

        /// <summary>
        /// 工具库中相机控制目录名称
        /// </summary>
        public const string ControllersCategoryName_Rotate = "相机组件-旋转";

        /// <summary>
        /// 状态库中相机控制目录名称
        /// </summary>
        public const string StateLibCategoryName = CategoryName;

        /// <summary>
        /// 最大阻尼系数
        /// </summary>
        public const float MaxDampingCoefficient = 10;

        /// <summary>
        /// 默认自带的主相机
        /// </summary>
        public const string DefaultMainCamera = "Main Camera";

        /// <summary>
        /// 默认自带的主相机标签
        /// </summary>
        public const string DefaultMainCameraTag = "MainCamera";

        /// <summary>
        /// 包含渲染器函数
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="includeEffectsRenderer"></param>
        /// <returns></returns>
        public static bool IncludeRendererFunc(Renderer renderer, bool includeEffectsRenderer)
        {
            if (includeEffectsRenderer) return true;
            if (renderer is TrailRenderer || renderer is ParticleSystemRenderer)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 尝试获取包围盒
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="transform"></param>
        /// <param name="includeRendererFunc"></param>
        /// <returns></returns>
        public static bool TryGetBounds(out Bounds bounds, Transform transform, Func<Renderer, bool> includeRendererFunc)
        {
            return CommonFun.GetBounds(out bounds, transform, go => true, go => true, includeRendererFunc);
        }

        /// <summary>
        /// 尝试获取包围盒
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="transforms"></param>
        /// <param name="includeRendererFunc"></param>
        /// <returns></returns>
        public static bool TryGetBounds(out Bounds bounds, IEnumerable<Transform> transforms, Func<Renderer, bool> includeRendererFunc)
        {
            return CommonFun.GetBounds(out bounds, transforms, go => true, go => true, includeRendererFunc);
        }

        /// <summary>
        /// 当前相机
        /// </summary>
        public static Camera currentCamera => Camera.main ? Camera.main : Camera.current;

        /// <summary>
        /// 获取当前相机
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool TryConvertWorldToScreenPoint(Vector3 position, out Vector3 screenPoint)
        {
            var cam = currentCamera;
            screenPoint = cam ? cam.WorldToScreenPoint(position) : default;
            return cam;
        }

        #region 使用相机生成渲染贴图

        /// <summary>
        /// 创建渲染贴图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static RenderTexture Render(this Camera camera, Vector2Int textureSize, GameObject gameObject)
        {
            return Render(camera, textureSize, gameObject, gameObject.transform.position);
        }

        /// <summary>
        /// 创建渲染贴图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static RenderTexture Render(this Camera camera, Vector2Int textureSize, GameObject gameObject, Vector3 position)
        {
            if (gameObject && textureSize != Vector2Int.zero && CommonFun.GetBounds(out Bounds bounds, gameObject, true, false))
            {
                return camera.Render(textureSize, bounds.center, camera.GetCameraFitDistance(bounds), camera.transform.rotation.eulerAngles, gameObject);
            }
            return null;
        }

        /// <summary>
        /// 创建渲染贴图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <param name="gameObject"></param>
        /// <param name="useBoundsCenterAsViewCenter"></param>
        /// <param name="viewDistance"></param>
        /// <param name="viewOffsetAngle"></param>
        /// <returns></returns>
        public static RenderTexture Render(this Camera camera, Vector2Int textureSize, GameObject gameObject, bool useBoundsCenterAsViewCenter, float viewDistance, Vector3 viewOffsetAngle)
        {
            if (!gameObject) return null;

            var viewCenter = gameObject.transform.position;
            if (useBoundsCenterAsViewCenter && CommonFun.GetBounds(out Bounds bounds, gameObject, true, false))
            {
                viewCenter = bounds.center;
            }
            return camera.Render(textureSize, viewCenter, viewDistance, viewOffsetAngle, gameObject);
        }

        /// <summary>
        /// 创建渲染贴图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <param name="viewCenter"></param>
        /// <param name="viewDistance"></param>
        /// <param name="viewOffsetAngle"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static RenderTexture Render(this Camera camera, Vector2Int textureSize, Vector3 viewCenter, float viewDistance, Vector3 viewOffsetAngle, GameObject gameObject)
        {
            if (!gameObject) return null;

            // 记录游戏对象及子对象层
            var gameObjectRecorder = new GameObjectRecorder();

            // 激活父级保证被拍摄对象可以呈现
            var parentGameObjects = CommonFun.GetParentsGameObject(gameObject, false);
            gameObjectRecorder.Record(parentGameObjects);
            parentGameObjects.ForEach(go => go.SetActive(true));// 启用父级游戏对象

            var childrenGameObjects = gameObject.GetComponentsInChildren<Transform>().Cast(t => t.gameObject);
            gameObjectRecorder.Record(childrenGameObjects);

            gameObject.SetActive(true);
            try
            {
                // 设置游戏及子游戏对象对象层为拍照相机的层
                if (camera.TryGetFirstValidCullingMask(out int layer))
                {
                    foreach (var item in childrenGameObjects)
                    {
                        item.layer = layer;
                    }
                }

                return camera.Render(textureSize, viewCenter, viewDistance, viewOffsetAngle);
            }
            finally
            {
                gameObjectRecorder.Recover();
            }
        }

        /// <summary>
        /// 相机渲染贴图
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="viewCenter"></param>
        /// <param name="viewDistance"></param>
        /// <param name="viewOffsetAngle"></param>
        /// <param name="textureSize"></param>
        /// <param name="orthographic"></param>
        /// <returns></returns>
        public static RenderTexture Render(this Camera camera, Vector2Int textureSize, Vector3 viewCenter, float viewDistance, Vector3 viewOffsetAngle)
        {
            // 记录相机转换
            var recorder = new TransformRecorder();
            var camTransform = camera.transform;
            recorder.Record(camTransform);

            try
            {
                // 调整相机对游戏对象角度和距离
                camTransform.rotation = Quaternion.Euler(viewOffsetAngle);
                camTransform.position = viewCenter - camTransform.forward * viewDistance;

                //// 相机绕着目标点分别以自身轴进行旋转
                //camTransform.RotateAround(viewCenter, camTransform.right, viewOffsetAngle.x);
                //camTransform.RotateAround(viewCenter, Vector3.up, viewOffsetAngle.y);
                //camTransform.RotateAround(viewCenter, camTransform.forward, viewOffsetAngle.z);

                return camera.Render(textureSize);
            }
            finally
            {
                recorder.Recover();
            }
        }

        /// <summary>
        /// 渲染相机当前画面
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="textureSize"></param>
        /// <returns></returns>
        public static RenderTexture Render(this Camera camera, Vector2Int textureSize)
        {
            if (!camera) return default;

            var gameObjectRecorder = new GameObjectRecorder();
            var cameraRecorder = new CameraRecorder();
            try
            {
                var parentGameObjects = CommonFun.GetParentsGameObject(camera.gameObject, true);
                gameObjectRecorder.Record(parentGameObjects);
                parentGameObjects.ForEach(go => go.SetActive(true));// 启用父级游戏对象

                cameraRecorder.Record(camera);

                camera.enabled = true;
                camera.targetTexture = new RenderTexture(textureSize.x, textureSize.y, 24, RenderTextureFormat.ARGB32);

                camera.pixelRect = new Rect(0, 0, textureSize.x, textureSize.y);
                camera.aspect = (float)textureSize.x / (float)textureSize.y;
                camera.Render();
                camera.ResetAspect();// 需重置纵横比，不然相机不会随着平面尺寸动态变换
                return camera.targetTexture;
            }
            finally
            {
                cameraRecorder.Recover();
                gameObjectRecorder.Recover();
            }
        }

        /// <summary>
        /// 渲染相机当前画面
        /// </summary>
        /// <param name="viewSize"></param>
        public static RenderTexture Render(this BaseCameraMainController cameraController, Vector2Int textureSize)
        {
            if (!cameraController || !cameraController.cameraEntityController) return null;

            return Render(cameraController.cameraEntityController.mainCamera, textureSize);
        }

        /// <summary>
        /// 获取相机到游戏对象最佳距离
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static float GetCameraFitDistance(this Camera camera, GameObject gameObject)
        {
            if (CommonFun.GetBounds(out Bounds bounds, gameObject, true, false))
            {
                return camera.GetCameraFitDistance(bounds);
            }
            return 0;
        }

        /// <summary>
        /// 获取相机到包围盒最佳距离
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static float GetCameraFitDistance(this Camera camera, Bounds bounds)
        {
            var num = bounds.size.magnitude * 0.5f;
            var f = camera.fieldOfView * 0.5f * 0.0174532924f;
            var tmp = Mathf.Sin(f) * Mathf.Cos(f);
            if (tmp > 0)
            {
                return num / tmp;
            }
            return 0;
        }

        public static bool TryGetFirstValidCullingMask(this Camera camera, out int layer)
        {
            layer = -1;
            if (camera)
            {
                for (int i = 0; i < 32; ++i)
                {
                    if (((1 << i) & camera.cullingMask) > 0)
                    {
                        layer = i;
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 切换前一个相机
        /// </summary>
        /// <param name="duration">切换时间</param>
        /// <param name="onCompleted">切换完成回调函数</param>
        /// <param name="museSwitch">强制切换</param>
        /// <returns></returns>
        public static bool SwitchPreviousCamera(float duration, Action onCompleted, bool museSwitch)
        {
            if (TryGetCameraInfo(out CameraManager manager, out IEnumerable<BaseCameraMainController> cameraControllers,
                out BaseCameraMainController current))
            {
                BaseCameraMainController switchTo = null;
                foreach (var c in cameraControllers)
                {
                    if (c == current) break;
                    switchTo = c;
                }

                //如果上一个相机控制器无效，则使用最后一个相机控制器
                return manager.GetProvider().SwitchCameraController(switchTo ? switchTo : cameraControllers.LastOrDefault(), duration, onCompleted, museSwitch);
            }
            return false;
        }

        /// <summary>
        /// 切换下一个相机
        /// </summary>
        /// <param name="duration">切换时间</param>
        /// <param name="onCompleted">切换完成回调函数</param>
        /// <param name="museSwitch">强制切换</param>
        /// <returns></returns>
        public static bool SwitchNextCamera(float duration, Action onCompleted, bool museSwitch)
        {
            if (TryGetCameraInfo(out CameraManager manager, out IEnumerable<BaseCameraMainController> cameraControllers,
                out BaseCameraMainController current))
            {
                BaseCameraMainController switchTo = null;
                bool isNext = false;
                foreach (var c in cameraControllers)
                {
                    if (isNext)
                    {
                        switchTo = c;
                        break;
                    }
                    if (c == current)
                    {
                        isNext = true;
                    }
                }

                //如果上一个相机控制器无效，则使用最后一个相机控制器
                return manager.GetProvider().SwitchCameraController(switchTo ? switchTo : cameraControllers.FirstOrDefault(), duration, onCompleted, museSwitch);
            }
            return false;
        }

        private static bool TryGetCameraInfo(out CameraManager manager, out IEnumerable<BaseCameraMainController> cameraControllers, out BaseCameraMainController current)
        {
            cameraControllers = default;
            current = default;

            //获取相机管理器
            manager = CameraManager.instance;
            if (!manager) return false;

            //获取相机控制器枚举迭代器
            cameraControllers = BaseCameraMainController.cameraControllers;
            if (cameraControllers == null) return false;

            current = manager.GetCurrentCameraController();
            if (!current) current = cameraControllers.FirstOrDefault();

            return true;
        }

        /// <summary>
        /// 尝试获取位置限定器
        /// </summary>
        /// <param name="cameraMainController"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool TryGetPositionLimiter(this BaseCameraMainController cameraMainController, out CameraPositionLimiter min, out CameraPositionLimiter max)
        {
            CameraPositionLimiter minPositionLimiter = default;
            CameraPositionLimiter maxPositionLimiter = default;
            if (cameraMainController)
            {
                var cameras = cameraMainController.cameraTransformer.GetComponentsInChildren<CameraPositionLimiter>();
                cameras.Foreach(c =>
                {
                    if (c.limitedAreaShape == ELimitedAreaShape.Sphere)
                    {
                        switch (c._sphereLimiter._closedAreaMode)
                        {
                            case EClosedAreaMode.Inside:
                                {
                                    maxPositionLimiter = c;
                                    break;
                                }
                            case EClosedAreaMode.Outside:
                                {
                                    minPositionLimiter = c;
                                    break;
                                }
                        }
                    }
                });
            }
            min = minPositionLimiter;
            max = maxPositionLimiter;
            return minPositionLimiter && maxPositionLimiter;
        }

        #region 相机

        /// <summary>
        /// 获取默认主相机
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Camera> GetDefaultMainCameras()
        {
            return CommonFun.GetRootGameObjects().WhereCast(go =>
            {
                if (go.name != DefaultMainCamera) return (false, default(Camera));
                var camera = go.GetComponent<Camera>();
                return (camera, camera);
            });
        }

        /// <summary>
        /// 处理场景创建时默认自带的主相机
        /// </summary>
        /// <param name="destoryOrDisactive">销毁还是禁用默认主相机</param>
        /// <returns></returns>
        public static Camera HandleDefaultMainCamera(bool destoryOrDisactive = true)
        {
            Camera mainCamera = GetDefaultMainCameras().FirstOrDefault();
            if (mainCamera)
            {
                if (destoryOrDisactive)
                {
                    mainCamera.gameObject.XDestoryObject();
                }
                else
                {
                    mainCamera.gameObject.XSetActive(false);
                }
            }
            return mainCamera;
        }

        private static Camera SetCamera(GameObject cameraGO, Transform parent, string name = "相机")
        {
            var camera = cameraGO.XGetOrAddComponent<Camera>();
            cameraGO.XModifyProperty(() =>
            {
                cameraGO.name = name;
                cameraGO.tag = DefaultMainCameraTag;
            });
            camera.XModifyProperty(() =>
            {
                camera.nearClipPlane = 0.01f;
            });
            cameraGO.XGetOrAddComponent<AudioListener>();
            if (parent) cameraGO.XSetParent(parent);

            return camera;
        }

        /// <summary>
        /// 创建相机
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Camera CreateCamera(Transform parent, string name = "相机")
        {
            //相机
            var cameraGO = UnityObjectHelper.CreateGameObject(nameof(Camera));
            return SetCamera(cameraGO, parent, name);
        }

        /// <summary>
        /// 克隆或创建相机
        /// </summary>
        /// <param name="cloneFromCamera"></param>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Camera CloneOrCreateCamera(Camera cloneFromCamera, Transform parent, string name = "相机")
        {
            if (!cloneFromCamera) return CreateCamera(parent, name);
            var cameraGO = UnityObjectHelper.XCloneObject(cloneFromCamera.gameObject);
            return SetCamera(cameraGO, parent, name);
        }

        /// <summary>
        /// 创建相机
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Camera CreateCamera(string name = "相机", Transform parent = null) => CreateCamera(parent, name);

        /// <summary>
        /// 创建相机控制器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="multiGameObjectMode">多游戏对象模式</param>
        /// <returns></returns>
        public static CameraController CreateCameraController(string name, bool multiGameObjectMode = true)
        {
            var cameraControllerGO = UnityObjectHelper.CreateGameObject(name);

            //相机控制器
            var cameraController = cameraControllerGO.XAddComponent<CameraController>();

            if (multiGameObjectMode)
            {
                cameraControllerGO.XCreateChild<CameraEntityController>();
                cameraControllerGO.XCreateChild<CameraTransformer>();
                cameraControllerGO.XCreateChild<CameraTargetController>();
            }
            else
            {
                cameraControllerGO.XAddComponent<CameraEntityController>();
                cameraControllerGO.XAddComponent<CameraTransformer>();
                cameraControllerGO.XAddComponent<CameraTargetController>();
            }

            //相机
            CreateCamera("相机", cameraController.cameraEntityController.transform);
            cameraController.cameraEntityController.Reset();

            //事件
            var cameraControllerEvent = cameraControllerGO.XAddComponent<Tools.Controllers.CameraControllerEvent>();
            cameraControllerEvent.Add(ECameraControllerEvent.OnSwitchedToCurrent, cameraController.gameObject, true);
            cameraControllerEvent.Add(ECameraControllerEvent.OnWillSwitchToLast, cameraController.gameObject, false);

            cameraController.Reset();

            //禁用场景创建时默认自带的主相机
            HandleDefaultMainCamera();

            return cameraController;
        }

        /// <summary>
        /// 创建角色专用的相机控制器:未与角色控制器产生任何交互；
        /// </summary>
        /// <param name="name"></param>
        /// <param name="multiGameObjectMode"></param>
        /// <returns></returns>
        public static CameraController CreateCameraControllerForCharacter(string name, bool multiGameObjectMode = true)
        {
            //创建相机控制器
            var cameraController = CreateCameraController(name, multiGameObjectMode);

            //添加移动组件
            var cameraMoveByInput = cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();
            cameraMoveByInput.XModifyProperty(() =>
            {
                cameraMoveByInput._xInputAxis._inputName = "";
                cameraMoveByInput._yInputAxis._inputName = "";
            });

            //触摸控制相机前后移动
            var cameraMoveByTouch = cameraController.cameraTransformer.XAddComponent<CameraMoveByTouch>();
            cameraMoveByTouch.XModifyProperty(ref cameraMoveByTouch._twoTouchMode, CameraMoveByTouch.ETwoTouchMode.Zoom_Z);

            //添加旋转组件
            var cameraRotateByInput = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            cameraRotateByInput.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;
            cameraRotateByInput.XModifyProperty(() =>
            {
                cameraRotateByInput._yInputAxis._mouseButtons.Remove(EMouseButton.Right);
            });

            //添加绕目标旋转组件
            var cameraRotateByTarget = cameraController.cameraTransformer.XAddComponent<CameraRotateByTarget>();

            //触摸控制相机旋转
            var cameraRotateByTouch = cameraController.cameraTransformer.XAddComponent<CameraRotateByTouch>();
            cameraRotateByTouch.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;
            cameraRotateByTouch.oneTouchMode = CameraRotateByTouch.EOneTouchMode.Y__X;

            //添加各种功能组件
            cameraController.cameraTargetController.XAddComponent<CameraLookAtLimiter>();
            cameraController.cameraTargetController.XAddComponent<CameraTargetRaycaster>();

            //添加位置限定-近
            var near = cameraController.cameraTransformer.XAddComponent<CameraPositionLimiter>();
            near.XModifyProperty(() =>
            {
                near._sphereLimiter._closedAreaMode = EClosedAreaMode.Outside;
                near._sphereLimiter._radius = 0.05f;
            });

            //添加位置限定-远
            var far = cameraController.cameraTransformer.XAddComponent<CameraPositionLimiter>();
            far.XModifyProperty(() =>
            {
                far._sphereLimiter._closedAreaMode = EClosedAreaMode.Inside;
                far._sphereLimiter._radius = 10;
            });

            return cameraController;
        }

        /// <summary>
        /// 创建飞行相机
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateFlyCamera(string name = "飞行相机")
        {
            var cameraController = CreateCameraController(name);

            cameraController.cameraTargetController.XSetEnable(false);

            cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();
            cameraController.cameraTransformer.XAddComponent<CameraMoveByKeyCode>();
            cameraController.cameraTransformer.XAddComponent<CameraMoveByTouch>();

            cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            cameraController.cameraTransformer.XAddComponent<CameraRotateByKeyCode>();

            //触摸控制相机旋转
            cameraController.cameraTransformer.XAddComponent<CameraRotateByTouch>();
            return cameraController;
        }

        /// <summary>
        /// 创建定点相机
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateFixedCamera(string name = "定点相机")
        {
            var cameraController = CreateCameraController(name);

            cameraController.cameraTargetController.XSetEnable(false);

            cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            cameraController.cameraTransformer.XAddComponent<CameraRotateByKeyCode>();

            var cameraRotateByKeyCode = cameraController.cameraTransformer.XAddComponent<CameraRotateByKeyCode>();
            cameraRotateByKeyCode.XModifyProperty(() =>
            {
                cameraRotateByKeyCode._up = KeyCode.W;
                cameraRotateByKeyCode._down = KeyCode.S;
                cameraRotateByKeyCode._left = KeyCode.A;
                cameraRotateByKeyCode._right = KeyCode.D;
            });

            //触摸控制相机旋转
            cameraController.cameraTransformer.XAddComponent<CameraRotateByTouch>();
            return cameraController;
        }

        /// <summary>
        /// 创建定点注视相机
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateFixedLookAtCamera(string name = "定点注视相机")
        {
            var cameraController = CreateCameraController(name);

            cameraController.cameraTargetController.XAddComponent<CameraLookAtLimiter>();
            return cameraController;
        }

        /// <summary>
        /// 创建跟随相机
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateFollowCamera(string name = "跟随相机")
        {
            var cameraController = CreateCameraController(name);

            cameraController.cameraTransformer.XAddComponent<CameraMoveByTarget>();

            var cameraRotateByInput = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            cameraRotateByInput.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            cameraController.cameraTransformer.XAddComponent<CameraRotateByTarget>();

            //触摸控制相机旋转
            var cameraRotateByTouch = cameraController.cameraTransformer.XAddComponent<CameraRotateByTouch>();
            cameraRotateByTouch.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            return cameraController;
        }

        /// <summary>
        /// 创建绕物相机
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateAroundCamera(string name = "绕物相机")
        {
            var cameraController = CreateCameraController(name);

            //滚轮控制前后移动
            var fbMove = cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();
            fbMove.XModifyProperty(() =>
            {
                fbMove._xInputAxis._inputName = "";
                fbMove._yInputAxis._inputName = "";
            });

            //键盘控制前后移动
            var fbMove1 = cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();
            fbMove1.XModifyProperty(() =>
            {
                fbMove1._xInputAxis._inputName = "";
                fbMove1._yInputAxis._inputName = "";
                fbMove1._zInputAxis._inputName = EInputName.Vertical.GetInputName();
            });

            //触摸控制相机前后移动
            var cameraMoveByTouch = cameraController.cameraTransformer.XAddComponent<CameraMoveByTouch>();
            cameraMoveByTouch.XModifyProperty(ref cameraMoveByTouch._twoTouchMode, CameraMoveByTouch.ETwoTouchMode.Zoom_Z);

            //键盘控制左右旋转
            var lrRotate = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            lrRotate.XModifyProperty(() =>
            {
                lrRotate._xInputAxis._inputName = "";
                lrRotate._xInputAxis._mouseButtons.Clear();

                lrRotate._yInputAxis._inputName = EInputName.Horizontal.GetInputName();
                lrRotate._yInputAxis._mouseButtons.Clear();
                lrRotate._yInputAxis._mouseButtons.Add(EMouseButton.Always);

                lrRotate._zInputAxis._inputName = "";
                lrRotate._zInputAxis._mouseButtons.Clear();

                lrRotate._inputHandler._inputWhenOnAnyUI = EInput.StandaloneInput;
                lrRotate._inputHandler._inputWhenHasTouch = EInput.StandaloneInput;
                lrRotate._inputHandler._input = EInput.StandaloneInput;
            });
            lrRotate.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            //鼠标控制上下左右旋转
            var udRotate1 = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            udRotate1.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            //键盘控制上下旋转
            var udRotate = cameraController.cameraTransformer.XAddComponent<CameraRotateByKeyCode>();
            udRotate.XModifyProperty(() =>
            {
                udRotate._left = KeyCode.None;
                udRotate._right = KeyCode.None;
                udRotate._up = KeyCode.Q;
                udRotate._down = KeyCode.E;
                udRotate._speedInfo.Reset(new Vector3(-60, 60, 60));
            });
            udRotate.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            //触摸控制相机旋转
            var cameraRotateByTouch = cameraController.cameraTransformer.XAddComponent<CameraRotateByTouch>();
            cameraRotateByTouch.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            //旋转的角度限定
            cameraController.cameraTransformer.XAddComponent<CameraAngleLimiter>();

            //查看目标
            cameraController.cameraTargetController.XAddComponent<CameraLookAtLimiter>();

            var positionLimiter = cameraController.cameraTransformer.XAddComponent<CameraPositionLimiter>();
            positionLimiter.XModifyProperty(() =>
            {
                positionLimiter._sphereLimiter._closedAreaMode = EClosedAreaMode.Outside;
            });

            var positionLimiter1 = cameraController.cameraTransformer.XAddComponent<CameraPositionLimiter>();
            positionLimiter1.XModifyProperty(() =>
            {
                positionLimiter1._sphereLimiter._radius = 10;
                positionLimiter1._sphereLimiter._closedAreaMode = EClosedAreaMode.Inside;
            });

            return cameraController;
        }

        /// <summary>
        /// 创建平移绕物相机
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateMoveAroundCamera(string name = "平移绕物相机")
        {
            var cameraController = CreateCameraController(name);

            cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();

            //触摸控制相机前后移动
            var cameraMoveByTouch = cameraController.cameraTransformer.XAddComponent<CameraMoveByTouch>();
            cameraMoveByTouch.XModifyProperty(ref cameraMoveByTouch._twoTouchMode, CameraMoveByTouch.ETwoTouchMode.Zoom_Z);

            var cameraRotateByInput = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            cameraRotateByInput.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            //触摸控制相机旋转
            var cameraRotateByTouch = cameraController.cameraTransformer.XAddComponent<CameraRotateByTouch>();
            cameraRotateByTouch.rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;

            return cameraController;
        }

        /// <summary>
        /// 创建角色相机:空角色相机，交互控制方式与主流PC版MMORPG游戏（如魔兽世界网游）控制方式相似
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static (XCharacterController, CameraController) CreateCharacterCamera(string name = "角色相机")
        {
            //创建空角色相机
            var characterController = CharacterHelper.CreateEmptyCharacter(name);
            characterController.XRename(name);
            return (characterController, characterController.characterCameraController.cameraMainController as CameraController);
        }

        /// <summary>
        /// 创建行走相机:特殊的空角色相机，交互控制方式与旧版行走相机相似；
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static (XCharacterController, CameraController) CreateWalkCamera(string name = "行走相机")
        {
            //创建空角色相机
            var characterController = CharacterHelper.CreateEmptyCharacter(name);
            characterController.XRename(name);
            var cameraController = characterController.GetComponentInChildren<CameraController>();
            cameraController.transform.XSetLocalPosition(new Vector3(0, 1.4f, -0.08f));

            var far = cameraController.cameraTransformer.GetComponents<CameraPositionLimiter>().FirstOrDefault(c => c._sphereLimiter._closedAreaMode == EClosedAreaMode.Inside);
            if (far)
            {
                far.XModifyProperty(() =>
                {
                    far._sphereLimiter._radius = 0.1f;
                });
            }

            return (characterController, characterController.characterCameraController.cameraMainController as CameraController);
        }

        /// <summary>
        /// 平移飞行相机:特殊的飞行相机，交互控制方式与Dota、魔兽争霸、红警等游戏的控制方式相似；
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateMoveFlyCamera(string name = "平移飞行相机")
        {
            var cameraController = CreateCameraController(name);

            //相机目标-调整为缓存模式
            var cameraTargetController = cameraController.cameraTargetController as CameraTargetController;
            if (cameraTargetController)
            {
                cameraTargetController.XModifyProperty(() =>
                {
                    cameraTargetController._targetPositionMode = ETargetPositionMode.Cache;
                    cameraTargetController._targetRotationMode = ETargetRotationMode.Cache;
                    cameraTargetController._targetBoundsMode = ETargetBoundsMode.Cache;
                });
            }

            //默认位置与旋转
            var cameraTransform = cameraController.cameraTransformer.mainTransform;
            cameraTransform.XSetLocalPosition(new Vector3(0, 2, 0));
            cameraTransform.XSetLocalRotation(new Vector3(45, 0, 0));

            //键盘的WASDQE的控制
            cameraController.cameraTransformer.XAddComponent<CameraMoveByKeyCode>();

            //鼠标中键按下时，投影面内的XZ运动；鼠标中键滚动时前后移动
            var cameraMoveByInput = cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();
            cameraMoveByInput.XModifyProperty(() =>
            {
                cameraMoveByInput._moveMode = Controllers.EMoveMode.Self_LocalYProjectToWorldX0Z;
                //cameraMoveByInput._xInputAxis._inputName = "";
                //cameraMoveByInput._yInputAxis._inputName = "";
            });

            //鼠标中键按下时，投影面内的XZ运动；鼠标中键滚动时前后移动
            var cameraMoveByInput1 = cameraController.cameraTransformer.XAddComponent<CameraMoveByInput>();
            cameraMoveByInput1.XModifyProperty(() =>
            {
                cameraMoveByInput1._moveMode = Controllers.EMoveMode.Self_LocalYProjectToWorldX0Z;

                cameraMoveByInput1. _speedInfo.Reset(new Vector3(15, 15, 20));
                cameraMoveByInput1._xInputAxis._inputName = "Horizontal";
                cameraMoveByInput1._yInputAxis._inputName = "Vertical";
                cameraMoveByInput1._zInputAxis._inputName = "";
            });

            //通过鼠标在屏幕窗口中位置控制相机的移动
            var cameraMoveByMouse = cameraController.cameraTransformer.XAddComponent<CameraMoveByMouse>();
            cameraMoveByMouse.moveMode = Controllers.EMoveMode.Self_World;

            //相机位置限定
            var cameraPositionLimiter = cameraController.cameraTransformer.XAddComponent<CameraPositionLimiter>();
            cameraPositionLimiter.XSetEnable(false);
            cameraPositionLimiter.XModifyProperty(() =>
            {
                cameraPositionLimiter._limitedAreaShape = ELimitedAreaShape.Bounds;
                cameraPositionLimiter._boundsLimiter._pointType = BaseBoundsLimiter.EPointType.Closest;
                cameraPositionLimiter._boundsLimiter._centerPosition._positionType = EPositionType.Original;
            });

            //相机距离限定
            var cameraDistanceLimiter = cameraController.cameraTransformer.XAddComponent<CameraDistanceLimiter>();
            cameraDistanceLimiter.XModifyProperty(() =>
            {
                cameraDistanceLimiter._limitEffectInEdit = false;
                cameraDistanceLimiter._distanceLimitRule = CameraDistanceLimiter.EDistanceLimitRule.Ray;
                //cameraDistanceLimiter._distanceKeepRule = CameraDistanceLimiter.EDistanceKeepRule.TryKeepDistance;
                cameraDistanceLimiter.TryUpdateDistance();
            });

            //光标绕自身旋转
            cameraController.cameraTransformer.XAddComponent<CameraRotateByKeyCode>();

            //左键绕自身旋转
            var left = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            if (left)
            {
                left.XModifyProperty(() =>
                {
                    left._xInputAxis._mouseButtons.RemoveAll(mb => mb == EMouseButton.Right);
                    left._yInputAxis._mouseButtons.RemoveAll(mb => mb == EMouseButton.Right);
                });
            }

            //右键绕目标旋转
            var right = cameraController.cameraTransformer.XAddComponent<CameraRotateByInput>();
            if (right)
            {
                right.XModifyProperty(() =>
                {
                    right._rotateMode = ERotateMode.Target_LocalX__Targert_WorldY;
                    right._xInputAxis._mouseButtons.RemoveAll(mb => mb == EMouseButton.Left);
                    right._yInputAxis._mouseButtons.RemoveAll(mb => mb == EMouseButton.Left);
                });
            }

            return cameraController;
        }

        #endregion
    }
}
