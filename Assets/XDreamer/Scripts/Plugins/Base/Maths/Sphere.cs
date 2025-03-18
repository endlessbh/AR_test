using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Extension.Base.Maths;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 球体
    /// </summary>
    public struct Sphere
    {
        /// <summary>
        /// 球体中心；圆心；
        /// </summary>
        public Vector3 _center;

        /// <summary>
        /// 球体中心；圆心；
        /// </summary>
        public Vector3 center { get => _center; set => _center = value; }

        /// <summary>
        /// 球体半径
        /// </summary>
        public float _radius;

        /// <summary>
        /// 球体半径
        /// </summary>
        public float radius { get => _radius; set => _radius = value; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public Sphere(Vector3 center, float radius)
        {
            _center = center;
            _radius = radius;
        }

        /// <summary>
        /// 包含：点是否在球体内部或表面上；
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3 point) => (point - center).sqrMagnitude <= (radius * radius);

        /// <summary>
        /// 获取点在球体上最近的点；最近点为球体内部或表面上的点
        /// </summary>
        /// <param name="point">点；如果点在球体内部或球体表面时，则最近点即本参数点；如果点在球体外部，则最近点为本参数点在球体表面的投影点；</param>
        /// <returns>返回球体内部或是球体表面的点</returns>
        public Vector3 GetClosestPoint(Vector3 point) => MathLibrary.GetClosestPointOnSphere(center, radius, point);

        /// <summary>
        /// 尝试获取点在球体表面上最近的点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns>当点与包围盒中心重合时，返回false</returns>
        public bool TryGetClosestPointOnSurface(Vector3 point, out Vector3 closestPoint) => MathLibrary.TryGetClosestPointOnSphereSurface(center, radius, point, out closestPoint);

        /// <summary>
        /// 获取点在球体表面上最近的点；即表面或外部的点；
        /// </summary>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns>当点与中心重合时，返回false</returns>
        public bool TryGetClosestPointNotInside(Vector3 point, out Vector3 closestPoint) => MathLibrary.TryGetClosestPointOnSphereNotInside(center, radius, point, out closestPoint);

        /// <summary>
        /// 检测球体与设想是否相交
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public bool IntersectRay(Ray ray) => IntersectRay(ray, out float distance);

        /// <summary>
        /// 检测球体与设想是否相交
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="distance">射线原点到相交点的距离</param>
        /// <returns></returns>
        public bool IntersectRay(Ray ray, out float distance) => MathLibrary.RaySphere(ray.origin, ray.direction, center, radius, out distance);

        /// <summary>
        /// 检测球体与设想是否相交
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="point">射线与球体的相交点</param>
        /// <returns></returns>
        public bool IntersectRay(Ray ray, out Vector3 point)
        {
            if (IntersectRay(ray, out float distance))
            {
                point = ray.GetPoint(distance);
                return true;
            }
            point = default;
            return false;
        }
    }
}
