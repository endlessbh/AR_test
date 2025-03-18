using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机目标控制器
    /// </summary>
    [Name("相机目标控制器")]
    [DisallowMultipleComponent]
    public class CameraTargetController : BaseCameraTargetController
    {
        #region 位置

        /// <summary>
        /// 目标位置列表：目标列表中有效目标的实时位置列表
        /// </summary>
        public List<Vector3> targetPositions => _targets.WhereToList(t => t ? (true, t.position) : (false, Vector3.zero));

        /// <summary>
        /// 目标位置
        /// </summary>
        [Group("位置", defaultIsExpanded = false)]
        [Name("目标位置")]
        [Tip("目标位置的缓存值，本值基于世界坐标；", "Cache value of target location, which is based on world coordinates;")]
        public Vector3 _targetPosition = new Vector3();

        /// <summary>
        /// 目标位置的缓存值，本值基于世界坐标；
        /// </summary>
        public Vector3 targetPositionCache => _targetPosition;

        /// <summary>
        /// 目标位置：会实时计算目标位置并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        public override Vector3 targetPosition => UpdateTargetPosition();

        /// <summary>
        /// 目标位置模式
        /// </summary>
        [Name("目标位置模式")]
        [EnumPopup]
        public ETargetPositionMode _targetPositionMode = ETargetPositionMode.MainTarget;

        /// <summary>
        /// 闭合的目标列表多边形
        /// </summary>
        [Name("闭合的目标列表多边形")]
        [HideInSuperInspector(nameof(_targetPositionMode), EValidityCheckType.NotEqual, ETargetPositionMode.TargetsPolygonClosestPoint)]
        public bool _closedTargetsPolygon = false;

        /// <summary>
        /// 更新目标位置：会实时计算目标位置并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        /// <returns></returns>
        public override Vector3 UpdateTargetPosition() => _targetPosition = GetTargetPosition(_targetPositionMode);

        /// <summary>
        /// 获取目标位置
        /// </summary>
        /// <param name="targetPositionMode"></param>
        /// <returns></returns>
        public override Vector3 GetTargetPosition(int targetPositionMode) => GetTargetPosition((ETargetPositionMode)targetPositionMode);

        /// <summary>
        /// 获取目标位置
        /// </summary>
        /// <param name="targetPositionMode"></param>
        /// <returns></returns>
        public Vector3 GetTargetPosition(ETargetPositionMode targetPositionMode)
        {
            switch (targetPositionMode)
            {
                case ETargetPositionMode.None: return Vector3.zero;
                case ETargetPositionMode.Cache: break;
                case ETargetPositionMode.MainTarget:
                    {
                        var target = this.mainTarget;
                        if (target)
                        {
                            return target.position;
                        }
                        break;
                    }
                case ETargetPositionMode.FirstTarget:
                    {
                        var target = this.firstTarget;
                        if (target)
                        {
                            return target.position;
                        }
                        break;
                    }
                case ETargetPositionMode.LastTarget:
                    {
                        var target = this.lastTarget;
                        if (target)
                        {
                            return target.position;
                        }
                        break;
                    }
                case ETargetPositionMode.TargetsPolygonClosestPoint:
                    {
                        var cameraTransformer = this.cameraTransformer;
                        if (cameraTransformer && MathLibrary.TryGetClosestPointOnPolygon(targetPositions, cameraTransformer.position, out Vector3 closestPoint, _closedTargetsPolygon))
                        {
                            return closestPoint;
                        }
                        break;
                    }
                case ETargetPositionMode.MainTargetBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, mainTarget))
                        {
                            return bounds.center;
                        }
                        break;
                    }
                case ETargetPositionMode.FirstTargetBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, firstTarget))
                        {
                            return bounds.center;
                        }
                        break;
                    }
                case ETargetPositionMode.LastTargetBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, lastTarget))
                        {
                            return bounds.center;
                        }
                        break;
                    }
                case ETargetPositionMode.TargetsBoundsCenter:
                    {
                        if (CommonFun.GetBounds(out Bounds bounds, _targets))
                        {
                            return bounds.center;
                        }
                        break;
                    }
            }
            return _targetPosition;
        }

        /// <summary>
        /// 目标位置需要主目标
        /// </summary>
        public bool needMainTargetOfTargetPosition
        {
            get
            {
                switch (_targetPositionMode)
                {
                    case ETargetPositionMode.Cache:
                    case ETargetPositionMode.None:
                        {
                            return false;
                        }
                    default:
                        {
                            return true;
                        }
                }
            }
        }

        /// <summary>
        /// 尝试设置目标位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override bool TrySetTargetPosition(Vector3 position)
        {
            switch (_targetPositionMode)
            {
                case ETargetPositionMode.Cache:
                    {
                        _targetPosition = position;
                        return true;
                    }
            }
            return false;
        }

        #endregion

        #region 旋转

        /// <summary>
        /// 目标旋转角度:目标旋转角度的缓存值，本值基于世界坐标；
        /// </summary>
        [Group("旋转", defaultIsExpanded = false)]
        [Name("目标旋转角度")]
        [Tip("目标旋转角度的缓存值，本值基于世界坐标；", "Cache value of target rotation angle, which is based on world coordinates;")]
        public Vector3 _targetRotationAngle = new Vector3();

        /// <summary>
        /// 目标旋转角度缓存值
        /// </summary>
        public Vector3 targetRotationAngleCache => _targetRotationAngle;

        /// <summary>
        /// 目标旋转量缓存值
        /// </summary>
        public Quaternion targetRotationCache { get => Quaternion.Euler(_targetRotationAngle); set => _targetRotationAngle = value.eulerAngles; }

        /// <summary>
        /// 目标旋转角度：会实时计算目标旋转角度并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        public override Vector3 targetRotationAngle
        {
            get
            {
                UpdateTargetRotationAngle();
                return _targetRotationAngle;
            }
        }

        /// <summary>
        /// 目标旋转量：会实时计算目标旋转量并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        public override Quaternion targetRotation => UpdateTargetRotationAngle();

        /// <summary>
        /// 目标旋转模式
        /// </summary>
        [Name("目标旋转模式")]
        [EnumPopup]
        public ETargetRotationMode _targetRotationMode = ETargetRotationMode.MainTarget;

        /// <summary>
        /// 更新目标旋转角度：会更新目标旋转角度<see cref="_targetRotationAngle"/>的值；
        /// </summary>
        /// <returns></returns>
        public Quaternion UpdateTargetRotationAngle()
        {
            var rotation = GetTargetRotation(_targetRotationMode);
            _targetRotationAngle = rotation.eulerAngles;
            return rotation;
        }

        /// <summary>
        /// 获取目标旋转量
        /// </summary>
        /// <returns></returns>
        public Quaternion GetTargetRotation(ETargetRotationMode targetRotationMode)
        {
            switch (targetRotationMode)
            {
                case ETargetRotationMode.None: return Quaternion.identity;
                case ETargetRotationMode.Cache: break;
                case ETargetRotationMode.MainTarget:
                    {
                        var target = this.mainTarget;
                        if (target)
                        {
                            return target.rotation;
                        }
                        break;
                    }
                case ETargetRotationMode.FirstTarget:
                    {
                        var target = this.firstTarget;
                        if (target)
                        {
                            return target.rotation;
                        }
                        break;
                    }
                case ETargetRotationMode.LastTarget:
                    {
                        var target = this.lastTarget;
                        if (target)
                        {
                            return target.rotation;
                        }
                        break;
                    }
            }
            return targetRotationCache;
        }

        /// <summary>
        /// 目标旋转需要主目标
        /// </summary>
        public bool needMainTargetOfTargetRotation
        {
            get
            {
                switch (_targetRotationMode)
                {
                    case ETargetRotationMode.Cache:
                    case ETargetRotationMode.None:
                        {
                            return false;
                        }
                    default:
                        {
                            return true;
                        }
                }
            }
        }

        /// <summary>
        /// 尝试设置目标旋转
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public override bool TrySetTargetRotation(Quaternion rotation)
        {
            switch (_targetBoundsMode)
            {
                case ETargetBoundsMode.Cache:
                    {
                        targetRotationCache = rotation;
                        return true;
                    }
            }
            return false;
        }

        #endregion

        #region 包围盒

        /// <summary>
        /// 目标包围盒:目标包围盒的缓存值，本值基于世界坐标；
        /// </summary>
        [Group("包围盒", defaultIsExpanded = false)]
        [Name("目标包围盒")]
        [Tip("目标包围盒的缓存值，本值基于世界坐标；", "Cache value of target bounding box, which is based on world coordinates;")]
        public Bounds _targetBounds = new Bounds();

        /// <summary>
        /// 目标包围盒缓存值，本值基于世界坐标；
        /// </summary>
        public Bounds targetBoundsCache => _targetBounds;

        /// <summary>
        /// 目标包围盒：会实时计算目标包围盒并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        public override Bounds targetBounds => UpdateTargetBounds();

        /// <summary>
        /// 目标包围盒模式
        /// </summary>
        [Name("目标包围盒模式")]
        [EnumPopup]
        public ETargetBoundsMode _targetBoundsMode = ETargetBoundsMode.MainTarget;

        /// <summary>
        /// 包括效果渲染器:效果渲染器包括:<see cref="TrailRenderer"/>,<see cref="ParticleSystemRenderer"/>
        /// </summary>
        [Name("包括效果渲染器")]
        [Tip("效果渲染器包括:TrailRenderer,ParticleSystemRenderer", "Effect renderers include: TrailRenderer, ParticleSystemRenderer")]
        public bool _includeEffectsRenderer = true;

        /// <summary>
        /// 更新目标包围盒：会实时计算目标包围盒并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        /// <returns></returns>
        public Bounds UpdateTargetBounds() => _targetBounds = GetTargetBounds(_targetBoundsMode, _includeEffectsRenderer);

        /// <summary>
        /// 获取目标包围盒
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override Bounds GetTargetBounds(int mode) => GetTargetBounds((ETargetBoundsMode)mode, _includeEffectsRenderer);

        /// <summary>
        /// 获取目标包围盒
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public override Bounds GetTargetBounds(int mode, Func<Renderer, bool> func) => GetTargetBounds((ETargetBoundsMode)mode, func);

        /// <summary>
        /// 获取目标包围盒
        /// </summary>
        /// <param name="targetBoundsMode"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public Bounds GetTargetBounds(ETargetBoundsMode targetBoundsMode, Func<Renderer, bool> func)
        {
            switch (targetBoundsMode)
            {
                case ETargetBoundsMode.None: return new Bounds();
                case ETargetBoundsMode.Cache: break;
                case ETargetBoundsMode.MainTarget:
                    {
                        if (CameraHelperExtension.TryGetBounds(out Bounds bounds, mainTarget, func))
                        {
                            return bounds;
                        }
                        break;
                    }
                case ETargetBoundsMode.FirstTarget:
                    {
                        if (CameraHelperExtension.TryGetBounds(out Bounds bounds, firstTarget, func))
                        {
                            return bounds;
                        }
                        break;
                    }
                case ETargetBoundsMode.LastTarget:
                    {
                        if (CameraHelperExtension.TryGetBounds(out Bounds bounds, lastTarget, func))
                        {
                            return bounds;
                        }
                        break;
                    }
                case ETargetBoundsMode.Targets:
                    {
                        if (CameraHelperExtension.TryGetBounds(out Bounds bounds, _targets, func))
                        {
                            return bounds;
                        }
                        break;
                    }
            }
            return _targetBounds;
        }

        /// <summary>
        /// 获取目标包围盒
        /// </summary>
        /// <returns></returns>
        public Bounds GetTargetBounds(ETargetBoundsMode targetBoundsMode, bool includeEffectsRenderer)
        {
            return GetTargetBounds(targetBoundsMode, r => CameraHelperExtension.IncludeRendererFunc(r, includeEffectsRenderer));
        }

        /// <summary>
        /// 目标包围盒需要主目标
        /// </summary>
        public bool needMainTargetOfTargetBounds
        {
            get
            {
                switch (_targetBoundsMode)
                {
                    case ETargetBoundsMode.Cache:
                    case ETargetBoundsMode.None:
                        {
                            return false;
                        }
                    default:
                        {
                            return true;
                        }
                }
            }
        }

        /// <summary>
        /// 尝试设置目标包围盒中心
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public override bool TrySetTargetBoundsCenter(Vector3 boundsCenter)
        {
            switch (_targetBoundsMode)
            {
                case ETargetBoundsMode.Cache:
                    {
                        _targetBounds.center = boundsCenter;
                        return true;
                    }
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 需要主目标
        /// </summary>
        protected override bool needMainTarget => needMainTargetOfTargetPosition || needMainTargetOfTargetRotation || needMainTargetOfTargetBounds;

        #region MB方法

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            var targetPosition = this.targetPosition;
            switch (_targetPositionMode)
            {
                case ETargetPositionMode.TargetsPolygonClosestPoint:
                    {
                        XGizmos.DrawPath(Color.green, targetPositions.ToArray());
                        break;
                    }
            }

            var cameraTransformer = this.cameraTransformer;
            if (cameraTransformer)
            {
                XGizmos.DrawPath(Color.red, cameraTransformer.position, targetPosition);
            }
        }

        #endregion
    }

    /// <summary>
    /// 目标位置模式
    /// </summary>
    [Name("目标位置模式")]
    public enum ETargetPositionMode
    {
        /// <summary>
        /// 无:默认位置值（0,0,0）
        /// </summary>
        [Name("无")]
        [Tip("默认位置值（0,0,0）", "Default location value (0,0,0)")]
        None,

        /// <summary>
        /// 缓存:目标位置缓存值
        /// </summary>
        [Name("缓存")]
        [Tip("目标位置缓存值", "Target value cache location")]
        Cache,

        /// <summary>
        /// 主目标:主目标变换的位置
        /// </summary>
        [Name("主目标")]
        [Tip("主目标变换的位置", "Position of main target transformation")]
        MainTarget,

        /// <summary>
        /// 第一目标:第一目标变换的位置
        /// </summary>
        [Name("第一目标")]
        [Tip("第一目标变换的位置", "Position of the first target transformation")]
        FirstTarget,

        /// <summary>
        /// 末一目标:末一目标变换的位置
        /// </summary>
        [Name("末一目标")]
        [Tip("末一目标变换的位置", "Position of last target transformation")]
        LastTarget,

        /// <summary>
        /// 目标列表多边形最近点:以目标列表变换位置构成的多边形，相机变换器位置距离该多边形最近的点的位置
        /// </summary>
        [Name("目标列表多边形最近点")]
        [Tip("以目标列表变换位置构成的多边形，相机变换器位置距离该多边形最近的点的位置", "For a polygon formed by the transformation position of the target list, the position of the camera converter is the position of the nearest point to the polygon")]
        TargetsPolygonClosestPoint,

        /// <summary>
        /// 主目标包围盒中心:主目标包围盒中心的位置
        /// </summary>
        [Name("主目标包围盒中心")]
        [Tip("主目标包围盒中心的位置", "Location of the center of the main target bounding box")]
        MainTargetBoundsCenter,

        /// <summary>
        /// 第一目标包围盒中心:第一目标包围盒中心的位置
        /// </summary>
        [Name("第一目标包围盒中心")]
        [Tip("第一目标包围盒中心的位置", "Position of the center of the first target bounding box")]
        FirstTargetBoundsCenter,

        /// <summary>
        /// 末一目标包围盒中心:末一目标包围盒中心的位置
        /// </summary>
        [Name("末一目标包围盒中心")]
        [Tip("末一目标包围盒中心的位置", "Position of the center of the last target bounding box")]
        LastTargetBoundsCenter,

        /// <summary>
        /// 目标列表包围盒中心:以目标列表中所有变换的合集包围盒的中心
        /// </summary>
        [Name("目标列表包围盒中心")]
        [Tip("以目标列表中所有变换的合集包围盒的中心", "The center of the bounding box is surrounded by the aggregate of all transformations in the target list")]
        TargetsBoundsCenter,
    }

    /// <summary>
    /// :
    /// </summary>
    [Name("目标包围盒模式")]
    public enum ETargetBoundsMode
    {
        /// <summary>
        /// 无:默认位置（0,0,0）的空包围盒
        /// </summary>
        [Name("无")]
        [Tip("默认位置（0,0,0）的空包围盒", "Empty bounding box at default position (0,0,0)")]
        None,

        /// <summary>
        /// 缓存:目标包围盒缓存值
        /// </summary>
        [Name("缓存")]
        [Tip("目标包围盒缓存值", "Target bounding box cache value")]
        Cache,

        /// <summary>
        /// 主目标:主目标变换的包围盒
        /// </summary>
        [Name("主目标")]
        [Tip("主目标变换的包围盒", "Bounding box of main target transformation")]
        MainTarget,

        /// <summary>
        /// 第一目标:第一目标变换的包围盒
        /// </summary>
        [Name("第一目标")]
        [Tip("第一目标变换的包围盒", "Bounding box of the first target transformation")]
        FirstTarget,

        /// <summary>
        /// 末一目标:末一目标变换的包围盒
        /// </summary>
        [Name("末一目标")]
        [Tip("末一目标变换的包围盒", "Bounding box of last target transformation")]
        LastTarget,

        /// <summary>
        /// 目标列表:目标列表中所有变换的合集包围盒
        /// </summary>
        [Name("目标列表")]
        [Tip("目标列表中所有变换的合集包围盒", "The aggregate bounding box of all transformations in the target list")]
        Targets,
    }

    /// <summary>
    /// 目标旋转模式
    /// </summary>
    [Name("目标旋转模式")]
    public enum ETargetRotationMode
    {
        /// <summary>
        /// 无:默认角度值（0,0,0）
        /// </summary>
        [Name("无")]
        [Tip("默认角度值（0,0,0）", "Default angle value (0,0,0)")]
        None,

        /// <summary>
        /// 缓存:目标旋转缓存值
        /// </summary>
        [Name("缓存")]
        [Tip("目标旋转缓存值", "Target rotation cache value")]
        Cache,

        /// <summary>
        /// 主目标
        /// </summary>
        [Name("主目标")]
        MainTarget,

        /// <summary>
        /// 第一目标
        /// </summary>
        [Name("第一目标")]
        FirstTarget,

        /// <summary>
        /// 末一目标
        /// </summary>
        [Name("末一目标")]
        LastTarget,
    }
}
