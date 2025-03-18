using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Languages;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机位置限定器
    /// </summary>
    [Name("相机位置限定器")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Position)]
    public class CameraPositionLimiter : BaseCameraLimiter
    {
        /// <summary>
        /// 限定区域形状
        /// </summary>
        [Name("限定区域形状")]
        [EnumPopup]
        public ELimitedAreaShape _limitedAreaShape = ELimitedAreaShape.Sphere;

        /// <summary>
        /// 限定区域形状
        /// </summary>
        public ELimitedAreaShape limitedAreaShape => _limitedAreaShape;

        /// <summary>
        /// 包围盒限定器
        /// </summary>
        [Name("包围盒限定器")]
        [HideInSuperInspector(nameof(_limitedAreaShape), EValidityCheckType.NotEqual, ELimitedAreaShape.Bounds)]
        public BoundsLimiter _boundsLimiter = new BoundsLimiter();

        /// <summary>
        /// 球体限定器
        /// </summary>
        [Name("球体限定器")]
        [HideInSuperInspector(nameof(_limitedAreaShape), EValidityCheckType.NotEqual, ELimitedAreaShape.Sphere)]
        public SphereLimiter _sphereLimiter = new SphereLimiter();

        private Dictionary<ELimitedAreaShape, PositionLimiter> limiters = null;

        /// <summary>
        /// 尝试获取限定器
        /// </summary>
        /// <param name="positionLimiter"></param>
        /// <returns></returns>
        public bool TryGetPositionLimiter(out PositionLimiter positionLimiter)
        {
            if (limiters == null)
            {
                limiters = new Dictionary<ELimitedAreaShape, PositionLimiter>();
                limiters.Add(ELimitedAreaShape.Bounds, _boundsLimiter);
                limiters.Add(ELimitedAreaShape.Sphere, _sphereLimiter);
            }
            return limiters.TryGetValue(_limitedAreaShape, out positionLimiter);
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        public void LateUpdate()
        {
            if (TryGetPositionLimiter(out PositionLimiter positionLimiter))
            {
                positionLimiter.Limit(this);
            }
        }

        /// <summary>
        /// 选中时绘制Gizmos
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (TryGetPositionLimiter(out PositionLimiter positionLimiter))
            {
                positionLimiter.DrawGizmos(this);
            }
        }

        /// <summary>
        /// 在编辑态执行限定
        /// </summary>
        protected override void ExcuteLimitInEdit()
        {
            //base.ExcuteLimitInEdit();
            if (TryGetPositionLimiter(out PositionLimiter positionLimiter))
            {
                positionLimiter.Limit(this);
            }
        }
    }

    #region 限定区域形状

    /// <summary>
    /// 限定区域形状
    /// </summary>
    [Name("限定区域形状")]
    public enum ELimitedAreaShape
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 包围盒
        /// </summary>
        [Name("包围盒")]
        Bounds,

        /// <summary>
        /// 球体
        /// </summary>
        [Name("球体")]
        Sphere,
    }

    /// <summary>
    /// 位置限定器
    /// </summary>
    [Name("位置限定器")]
    [Serializable]
    [LanguageFileOutput]
    public abstract class PositionLimiter
    {
        /// <summary>
        /// 限定
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        public abstract void Limit(CameraPositionLimiter cameraPositionLimiter);

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        public abstract void DrawGizmos(CameraPositionLimiter cameraPositionLimiter);
    }

    /// <summary>
    /// 闭合区域模式
    /// </summary>
    [Name("闭合区域模式")]
    public enum EClosedAreaMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 内部
        /// </summary>
        [Name("内部")]
        Inside,

        /// <summary>
        /// 表面
        /// </summary>
        [Name("表面")]
        Surface,

        /// <summary>
        /// 外部
        /// </summary>
        [Name("外部")]
        Outside,
    }

    /// <summary>
    /// 闭合区域位置限定器
    /// </summary>
    [Name("闭合区域位置限定器")]
    [Serializable]
    public abstract class ClosedAreaPositionLimiter : PositionLimiter
    {
        /// <summary>
        /// 闭合区域模式
        /// </summary>
        [Name("闭合区域模式")]
        [EnumPopup]
        public EClosedAreaMode _closedAreaMode = EClosedAreaMode.Inside;
    }

    #endregion    

    #region 包围盒

    /// <summary>
    /// 基础包围盒限定器
    /// </summary>
    [Name("基础包围盒限定器")]
    public abstract class BaseBoundsLimiter : ClosedAreaPositionLimiter
    {
        /// <summary>
        /// 点类型
        /// </summary>
        [Name("点类型")]
        public enum EPointType
        {
            /// <summary>
            /// 投影点
            /// </summary>
            [Name("投影点")]
            Projection,

            /// <summary>
            /// 最近点
            /// </summary>
            [Name("最近点")]
            Closest,
        }

        /// <summary>
        /// 点类型
        /// </summary>
        [Name("点类型")]
        [EnumPopup]
        public EPointType _pointType = EPointType.Projection;

        /// <summary>
        /// 尝试获取包围盒
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public abstract bool TryGetBounds(CameraPositionLimiter cameraPositionLimiter, out Bounds bounds);

        /// <summary>
        /// 限定
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        public override void Limit(CameraPositionLimiter cameraPositionLimiter)
        {
            if (TryGetBounds(cameraPositionLimiter, out Bounds bounds))
            {
                var transformer = cameraPositionLimiter.cameraTransformer;
                switch (_closedAreaMode)
                {
                    case EClosedAreaMode.Inside:
                        {
                            var position = transformer.position;
                            switch (_pointType)
                            {
                                case EPointType.Projection:
                                    {
                                        if (!bounds.Contains(position))//外部时纠正
                                        {
                                            if (bounds.TryGetProjectionPointOnBoundsSurface(position, out Vector3 projectionPoint))
                                            {
                                                transformer.position = projectionPoint;
                                            }
                                            else
                                            {
                                                //中心重合??
                                            }
                                        }
                                        break;
                                    }
                                case EPointType.Closest:
                                    {
                                        transformer.position = bounds.ClosestPoint(position);
                                        break;
                                    }
                            }
                            break;
                        }
                    case EClosedAreaMode.Surface:
                        {
                            var position = transformer.position;
                            switch (_pointType)
                            {
                                case EPointType.Projection:
                                    {
                                        if (bounds.TryGetProjectionPointOnBoundsSurface(position, out Vector3 projectionPoint))
                                        {
                                            transformer.position = projectionPoint;
                                        }
                                        else
                                        {
                                            //中心重合??
                                        }
                                        break;
                                    }
                                case EPointType.Closest:
                                    {
                                        if (bounds.TryGetClosestPointOnBoundsSurface(position, out Vector3 closestPoint))
                                        {
                                            transformer.position = closestPoint;
                                        }
                                        else
                                        {
                                            //中心重合??
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    case EClosedAreaMode.Outside:
                        {
                            var position = transformer.position;
                            switch (_pointType)
                            {
                                case EPointType.Projection:
                                    {
                                        if (bounds.Contains(position))//内部时纠正
                                        {
                                            if (bounds.TryGetProjectionPointOnBoundsSurface(position, out Vector3 projectionPoint))
                                            {
                                                transformer.position = projectionPoint;
                                            }
                                            else
                                            {
                                                //中心重合??
                                            }
                                        }
                                        break;
                                    }
                                case EPointType.Closest:
                                    {
                                        if (bounds.TryGetClosestPointOnBoundsNotInside(transformer.position, out Vector3 closestPoint))
                                        {
                                            transformer.position = closestPoint;
                                        }
                                        else
                                        {
                                            //中心重合??
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        public override void DrawGizmos(CameraPositionLimiter cameraPositionLimiter)
        {
            if (TryGetBounds(cameraPositionLimiter, out Bounds bounds))
            {
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
    }

    /// <summary>
    /// 包围盒限定器
    /// </summary>
    [Name("包围盒限定器")]
    [Serializable]
    public class BoundsLimiter : BaseBoundsLimiter
    {
        /// <summary>
        /// 中心位置
        /// </summary>
        [Name("中心位置")]
        public Position _centerPosition = new Position();

        /// <summary>
        /// 尺寸
        /// </summary>
        [Name("尺寸")]
        public Vector3 _size = Vector3.one;

        /// <summary>
        /// 尝试获取包围盒
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public override bool TryGetBounds(CameraPositionLimiter cameraPositionLimiter, out Bounds bounds)
        {
            if (_centerPosition.TryGetPosition(cameraPositionLimiter.cameraController, out Vector3 center))
            {
                bounds = new Bounds(center, _size);
                return true;
            }
            bounds = default;
            return false;
        }
    }

    #endregion

    #region 球体

    /// <summary>
    /// 基础球体限定器
    /// </summary>
    [Name("基础球体限定器")]
    public abstract class BaseSphereLimiter : ClosedAreaPositionLimiter
    {
        /// <summary>
        /// 尝试获取球体
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public abstract bool TryGetSphere(CameraPositionLimiter cameraPositionLimiter, out Vector3 center, out float radius);

        /// <summary>
        /// 限定
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        public override void Limit(CameraPositionLimiter cameraPositionLimiter)
        {
            if (TryGetSphere(cameraPositionLimiter, out Vector3 center, out float radius))
            {
                var transformer = cameraPositionLimiter.cameraTransformer;
                switch (_closedAreaMode)
                {
                    case EClosedAreaMode.Inside:
                        {
                            transformer.position = MathLibrary.GetClosestPointOnSphere(center, radius, transformer.position);
                            break;
                        }
                    case EClosedAreaMode.Surface:
                        {
                            if (MathLibrary.TryGetClosestPointOnSphereSurface(center, radius, transformer.position, out Vector3 position))
                            {
                                transformer.position = position;
                            }
                            //中心重合??
                            break;
                        }
                    case EClosedAreaMode.Outside:
                        {
                            if (MathLibrary.TryGetClosestPointOnSphereNotInside(center, radius, transformer.position, out Vector3 position))
                            {
                                transformer.position = position;
                            }
                            //中心重合??
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 绘制Gizmos
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        public override void DrawGizmos(CameraPositionLimiter cameraPositionLimiter)
        {
            if (TryGetSphere(cameraPositionLimiter, out Vector3 center, out float radius))
            {
                Gizmos.DrawWireSphere(center, radius);
            }
        }
    }

    /// <summary>
    /// 球体限定器
    /// </summary>
    [Name("球体限定器")]
    [Serializable]
    public class SphereLimiter : BaseSphereLimiter
    {
        /// <summary>
        /// 中心位置
        /// </summary>
        [Name("中心位置")]
        public Position _centerPosition = new Position();

        /// <summary>
        /// 半径
        /// </summary>
        [Name("半径")]
        public float _radius = 0.1f;

        /// <summary>
        /// 尝试获取球体
        /// </summary>
        /// <param name="cameraPositionLimiter"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public override bool TryGetSphere(CameraPositionLimiter cameraPositionLimiter, out Vector3 center, out float radius)
        {
            if (_centerPosition.TryGetPosition(cameraPositionLimiter.cameraController, out center))
            {
                radius = _radius;
                return true;
            }
            center = default;
            radius = default;
            return false;
        }
    }

    #endregion
}
