using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机调节视场角通过目标
    /// </summary>
    [Name("相机调节视场角通过目标")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController), nameof(CameraTransformer), */nameof(CameraTargetController))]
    [XCSJ.Attributes.Icon(EIcon.Camera)]
    [DisallowMultipleComponent]
    public class CameraAdjustFOVByTarget : BaseCameraToolController
    {
        /// <summary>
        /// 调节时间:将当前视野调整到所需目标视野量所需的时间；单位为秒；
        /// </summary>
        [Name("调节时间")]
        [Tip("将当前视野调整到所需目标视野量所需的时间；单位为秒；", "The time required to adjust the current field of view to the required target field of view; The unit is second;")]
        public float _adjustTime = 1;

        /// <summary>
        /// 放大倍数
        /// </summary>
        [Name("放大倍数")]
        public float _zoomAmountMultiplier = 2;

        /// <summary>
        /// 目标包围盒规则
        /// </summary>
        [Name("目标包围盒规则")]
        [EnumPopup]
        public ETargetBoundsRule _targetBoundsRule = ETargetBoundsRule.TargetBounds;

        /// <summary>
        /// 目标包围盒模式
        /// </summary>
        [Name("目标包围盒模式")]
        [HideInSuperInspector(nameof(_targetBoundsRule), EValidityCheckType.NotEqual, ETargetBoundsRule.NewTargetBoundsMode)]
        [EnumPopup]
        public ETargetBoundsMode _targetBoundsMode = ETargetBoundsMode.MainTarget;

        /// <summary>
        /// 包括效果渲染器:效果渲染器包括:<see cref="TrailRenderer"/>,<see cref="ParticleSystemRenderer"/>
        /// </summary>
        [Name("包括效果渲染器")]
        [Tip("效果渲染器包括:TrailRenderer,ParticleSystemRenderer", "Effect renderers include: TrailRenderer, ParticleSystemRenderer")]
        [HideInSuperInspector(nameof(_targetBoundsRule), EValidityCheckType.NotEqual, ETargetBoundsRule.NewTargetBoundsMode)]
        public bool _includeEffectsRenderer = true;

        /// <summary>
        /// 目标包围盒
        /// </summary>
        [Name("目标包围盒")]
        [Readonly]
        public Bounds _targetBounds = new Bounds();

        /// <summary>
        /// 更新目标包围盒
        /// </summary>
        [Name("更新目标包围盒")]
        [Tip("如果为True，在更新时均会使用最新的目标包围盒；如果为False，则使用在开始时计算的包围盒；", "If true, the latest target bounding box will be used when updating; If false, the bounding box calculated at the beginning is used;")]
        public bool _updateTargetBounds = false;

        /// <summary>
        /// 相机影响模式
        /// </summary>
        [Name("相机影响模式")]
        [EnumPopup]
        public ECameraAdjustMode _cameraEffectMode = ECameraAdjustMode.All;

        /// <summary>
        /// 包围盒最长边的半长度
        /// </summary>
        public float boundsLongestSideExtents
        {
            get
            {
                var extents = _targetBounds.extents;
                return Mathf.Max(extents.x, extents.y, extents.z);
            }
        }

        private float mainVelocity = 0;
        private float firstVelocity = 0;
        private float lastVelocity = 0;

        private Dictionary<int, float> velocities = new Dictionary<int, float>();

        /// <summary>
        /// 开始时
        /// </summary>
        public void Start()
        {
            UpdateTargetBounds();
        }

        /// <summary>
        /// 更新时
        /// </summary>
        protected virtual void Update()
        {
            switch (_cameraEffectMode)
            {
                case ECameraAdjustMode.All:
                    {
                        float fov;
                        var mainCamera = cameraEntityController.mainCamera;
                        if (mainCamera)
                        {
                            fov = GetFOV();
                            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, fov, ref mainVelocity, _adjustTime);
                        }
                        else//如果无主相机，那么相机列表肯定为空
                        {
                            break;
                        }

                        //相机列表
                        var cameras = cameraEntityController.cameras;
                        for (int i = 0; i < cameras.Length; i++)
                        {
                            var camera = cameras[i];
                            if (camera)
                            {
                                if (!velocities.TryGetValue(i, out float velocity)) velocity = 0;

                                camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fov, ref velocity, _adjustTime);

                                velocities[i] = velocity;
                            }
                        }
                        break;
                    }
                case ECameraAdjustMode.MainCamera:
                    {
                        var camera = cameraEntityController.mainCamera;
                        if (camera)
                        {
                            var fov = GetFOV();
                            camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fov, ref mainVelocity, _adjustTime);
                        }
                        break;
                    }
                case ECameraAdjustMode.FirstCamera:
                    {
                        var camera = cameraEntityController.cameras.FirstOrDefault();
                        if (camera)
                        {
                            var fov = GetFOV();
                            camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fov, ref firstVelocity, _adjustTime);
                        }
                        break;
                    }
                case ECameraAdjustMode.LastCamera:
                    {
                        var camera = cameraEntityController.cameras.LastOrDefault();
                        if (camera)
                        {
                            var fov = GetFOV();
                            camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fov, ref lastVelocity, _adjustTime);
                        }
                        break;
                    }
                case ECameraAdjustMode.Cameras:
                    {
                        var cameras = cameraEntityController.cameras;
                        var length = cameras.Length;
                        if (length > 0)
                        {
                            var fov = GetFOV();
                            for (int i = 0; i < length; i++)
                            {
                                var camera = cameras[i];
                                if (camera)
                                {
                                    if (!velocities.TryGetValue(i, out float velocity)) velocity = 0;

                                    camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fov, ref velocity, _adjustTime);

                                    velocities[i] = velocity;
                                }
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 获取最新的FOV值
        /// </summary>
        /// <returns></returns>
        public float GetFOV()
        {
            if (_updateTargetBounds)
            {
                UpdateTargetBounds();
            }
            var targetPosition = cameraTargetController.targetPosition;
            var position = cameraTransformer.position;

            var distance = (position - targetPosition).magnitude;
            return Mathf.Atan2(boundsLongestSideExtents, distance) * Mathf.Rad2Deg * _zoomAmountMultiplier;
        }

        private void UpdateTargetBounds()
        {
            switch (_targetBoundsRule)
            {
                case ETargetBoundsRule.TargetBounds:
                    {
                        _targetBounds = cameraTargetController.targetBounds;
                        break;
                    }
                case ETargetBoundsRule.NewTargetBoundsMode:
                    {
                        _targetBounds = cameraTargetController.GetTargetBounds((int)_targetBoundsMode, renderer => CameraHelperExtension.IncludeRendererFunc(renderer, _includeEffectsRenderer));
                        break;
                    }
            }
        }

        /// <summary>
        /// 目标包围盒规则
        /// </summary>
        [Name("目标包围盒规则")]
        public enum ETargetBoundsRule
        {
            /// <summary>
            /// 目标包围盒
            /// </summary>
            [Name("目标包围盒")]
            TargetBounds,

            /// <summary>
            /// 新目标包围盒模式
            /// </summary>
            [Name("新目标包围盒模式")]
            NewTargetBoundsMode,
        }

        /// <summary>
        /// 相机调节模式
        /// </summary>
        [Name("相机调节模式")]
        public enum ECameraAdjustMode
        {
            /// <summary>
            /// 无:不做任何操作；
            /// </summary>
            [Name("无")]
            [Tip("不做任何操作；", "Do nothing;")]
            None,

            /// <summary>
            /// 所有:所有相机都调节，包括主相机与相机列表中相机；
            /// </summary>
            [Name("所有")]
            [Tip("所有相机都调节，包括主相机与相机列表中相机；", "All cameras are adjusted, including the main camera and the camera in the camera list;")]
            All,

            /// <summary>
            /// 主相机:仅对主相机做调节；
            /// </summary>
            [Name("主相机")]
            [Tip("仅对主相机做调节；", "Adjust the main camera only")]
            MainCamera,

            /// <summary>
            /// 第一相机:仅对第一相机做调节
            /// </summary>
            [Name("第一相机")]
            [Tip("仅对第一相机做调节；", "Adjust only the first camera;")]
            FirstCamera,

            /// <summary>
            /// 末一相机:仅对末一相机做调节；
            /// </summary>
            [Name("末一相机")]
            [Tip("仅对末一相机做调节；", "Adjust only the last camera;")]
            LastCamera,

            /// <summary>
            /// 相机列表:对相机列表中所有相机均做调节；
            /// </summary>
            [Name("相机列表")]
            [Tip("对相机列表中所有相机均做调节；", "Adjust all cameras in the camera list;")]
            Cameras,
        }
    }
}
