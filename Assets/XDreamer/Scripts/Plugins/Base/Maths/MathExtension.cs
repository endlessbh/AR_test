using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Maths;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class MathExtension
    {
        /// <summary>
        /// 检查值在公差内是否接近零。Checks whether value is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this float value)
        {
            const float tolerance = 0.0000000001f;

            return Mathf.Abs(value) < tolerance;
        }

        /// <summary>
        /// 返回给定向量的副本，其中只有向量的X分量。Returns a copy of given vector with only X component of the vector.
        /// </summary>
        public static Vector3 OnlyX(this Vector3 vector3)
        {
            vector3.y = 0.0f;
            vector3.z = 0.0f;

            return vector3;
        }

        /// <summary>
        /// 返回给定向量的一个副本，其中只有向量的Y分量。Returns a copy of given vector with only Y component of the vector.
        /// </summary>
        public static Vector3 OnlyY(this Vector3 vector3)
        {
            vector3.x = 0.0f;
            vector3.z = 0.0f;

            return vector3;
        }

        /// <summary>
        /// 返回给定向量的一个副本，其中只有向量的Z分量。Returns a copy of given vector with only Z component of the vector.
        /// </summary>
        public static Vector3 OnlyZ(this Vector3 vector3)
        {
            vector3.x = 0.0f;
            vector3.y = 0.0f;

            return vector3;
        }

        /// <summary>
        /// 返回给定向量的副本，其中只有向量的X和Z分量。Returns a copy of given vector with only X and Z components of the vector.
        /// </summary>
        public static Vector3 OnlyXZ(this Vector3 vector3)
        {
            vector3.y = 0.0f;

            return vector3;
        }

        /// <summary>
        /// 检查矢量在公差内是否接近零。Checks whether vector is near to zero within a tolerance.
        /// </summary>
        public static bool IsZero(this Vector3 vector3)
        {
            return vector3.sqrMagnitude < 9.99999943962493E-11;
        }

        /// <summary>
        /// 检查矢量的三轴量是否近似与值；即各值是否于目标值在近似零值的范围内
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="value"></param>
        /// <param name="approximatelyZero"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector3 vector3, Vector3 value, Vector3 approximatelyZero)
        {
            return MathX.Approximately(vector3.x, value.x, approximatelyZero.x)
                && MathX.Approximately(vector3.y, value.y, approximatelyZero.y)
                && MathX.Approximately(vector3.z, value.z, approximatelyZero.z);
        }

        /// <summary>
        /// 检查矢量的三轴量是否近似与值；即各值是否于目标值在近似零值的范围内
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="value"></param>
        /// <param name="approximatelyZero"></param>
        /// <returns></returns>
        public static bool Approximately(this Vector3 vector3, Vector3 value, float approximatelyZero = 1E-06F)
        {
            return MathX.Approximately(vector3.x, value.x, approximatelyZero)
                && MathX.Approximately(vector3.y, value.y, approximatelyZero)
                && MathX.Approximately(vector3.z, value.z, approximatelyZero);
        }
        /// <summary>
        /// 检查矢量是否超出小误差范围内的幅值。Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>
        public static bool IsExceeding(this Vector3 vector3, float magnitude)
        {
            // 允许1%的误差容限，以解释数值的不精确性。Allow 1% error tolerance, to account for numeric imprecision.

            const float errorTolerance = 1.01f;

            return vector3.sqrMagnitude > magnitude * magnitude * errorTolerance;
        }

        /// <summary>
        /// 返回大小为1的给定向量的副本，Returns a copy of given vector with a magnitude of 1,
        /// 并在正常化之前超过它的大小。and outs its magnitude before normalization.
        /// 
        /// 如果向量太小而无法规范化，则返回零向量。If the vector is too small to be normalized a zero vector will be returned.
        /// </summary>
        public static Vector3 Normalized(this Vector3 vector3, out float magnitude)
        {
            magnitude = vector3.magnitude;
            if (magnitude > 9.99999974737875E-06)
                return vector3 / magnitude;

            magnitude = 0.0f;

            return Vector3.zero;
        }

        /// <summary>
        /// 返回给定向量的一个副本，其大小固定为0和1，Returns a copy of given vector with its magnitude clamped to 0 and 1,
        /// 在钳制前就把它的大小去掉。and outs its magnitude before clamp.
        /// </summary>
        public static Vector3 Clamped(this Vector3 vector3, out float magnitude)
        {
            magnitude = vector3.magnitude;

            return magnitude > 1.0f ? vector3 / magnitude : vector3;
        }

        /// <summary>
        /// 返回给定向量的一个副本，其大小固定为maxLength。Returns a copy of given vector with its magnitude clamped to maxLength.
        /// </summary>
        public static Vector3 ClampedTo(this Vector3 vector3, float maxLength)
        {
            if (vector3.sqrMagnitude > maxLength * (double)maxLength)
                return vector3.normalized * maxLength;

            return vector3;
        }

        /// <summary>
        /// 将给定的向量转换为与目标变换相对应的向量。Transform a given vector to be realtive to target transform.
        /// 例如：用于执行相对于相机视角方向的移动。Eg: Use to perform movement relative to camera's view direction.
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="target"></param>
        /// <param name="onlyLateral">仅横向,仅XZ值</param>
        /// <returns></returns>
        public static Vector3 RelativeTo(this Vector3 vector3, Transform target, bool onlyLateral = true)
        {
            var forward = onlyLateral ? target.forward.OnlyXZ() : target.forward;

            return Quaternion.LookRotation(forward) * vector3;
        }

        /// <summary>
        /// 角度单位转化：弧度转度
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Vector3 RadianToDegree(this Vector3 radian) => radian * Mathf.Rad2Deg;

        /// <summary>
        /// 角度单位转化：度转弧度
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector3 DegreeToRadian(this Vector3 degree) => degree * Mathf.Deg2Rad;

        /// <summary>
        /// 获取点在射线上最近的点，即已知点在射线上的投影点
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnRay(this Ray ray, Vector3 point)
        {
            return ray.GetPoint(Vector3.Dot(ray.direction, point - ray.origin));
        }

        /// <summary>
        /// 获取包围盒的六个面，面的法向量由包围盒中心指向面中心的方向；
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns>六个面的顺序依次为：上下左右前后</returns>
        public static Plane[] GetBoundsPlanes(this Bounds bounds)
        {
            var extents = bounds.extents;
            var center = bounds.center;

            var planes = new Plane[6];

            //上
            planes[0] = new Plane(Vector3.up, new Vector3(center.x, center.y + extents.y, center.z));

            //下
            planes[1] = new Plane(Vector3.down, new Vector3(center.x, center.y - extents.y, center.z));

            //左
            planes[2] = new Plane(Vector3.left, new Vector3(center.x - extents.x, center.y, center.z));

            //左
            planes[3] = new Plane(Vector3.right, new Vector3(center.x + extents.x, center.y, center.z));

            //前
            planes[4] = new Plane(Vector3.forward, new Vector3(center.x, center.y, center.z + extents.z));

            //后
            planes[5] = new Plane(Vector3.back, new Vector3(center.x, center.y, center.z - extents.z));

            return planes;
        }

        /// <summary>
        /// 尝试获取点相对包围盒中心投影到表面上的投影点
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="point"></param>
        /// <param name="projectionPoint"></param>
        /// <returns>当点与包围盒中心重合时，返回false</returns>
        public static bool TryGetProjectionPointOnBoundsSurface(this Bounds bounds, Vector3 point, out Vector3 projectionPoint)
        {
            var dir = point - bounds.center;
            if (dir == Vector3.zero)
            {
                projectionPoint = point;
                return false;
            }

            dir = dir.normalized * 10;
            do
            {
                point += dir;
            } while (bounds.Contains(point));

            var ray = new Ray(point, -dir);
            if (bounds.IntersectRay(ray, out float dis))
            {
                projectionPoint = ray.GetPoint(dis);
                return true;
            }
            projectionPoint = point;
            return false;
        }

        /// <summary>
        /// 尝试获取包围盒内部的点到包围盒表面上最近的点;即点在包围盒六个面上的最近的投影点；
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="insidePoint">包围盒内部的点</param>
        /// <param name="closestPoint"></param>
        /// <returns></returns>
        private static bool TryGetClosestPointOnBoundsSurfaceInternal(this Bounds bounds, Vector3 insidePoint, out Vector3 closestPoint)
        {
            var dir = insidePoint - bounds.center;
            if (dir == Vector3.zero)
            {
                closestPoint = insidePoint;
                return false;
            }

            var planes = bounds.GetBoundsPlanes();
            var dis = float.MaxValue;
            var point = Vector3.zero;
            foreach (var p in planes)
            {
                var tmpPoint = p.ClosestPointOnPlane(insidePoint);
                var tmpDis = (tmpPoint - insidePoint).sqrMagnitude;
                if (tmpDis < dis)
                {
                    point = tmpPoint;
                    dis = tmpDis;
                }
            }
            closestPoint = point;
            return true;
        }

        /// <summary>
        /// 尝试获取点到包围盒表面上最近的点
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns>当点与包围盒中心重合时，返回false</returns>
        public static bool TryGetClosestPointOnBoundsSurface(this Bounds bounds, Vector3 point, out Vector3 closestPoint)
        {
            if (!bounds.Contains(point))//外部点
            {
                closestPoint = bounds.ClosestPoint(point);
                return true;
            }
            return TryGetClosestPointOnBoundsSurfaceInternal(bounds, point, out closestPoint);
        }

        /// <summary>
        /// 获取点到包围盒非内部最近的点；即表面或外部的点；
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns>当点与包围盒中心重合时，返回false</returns>
        public static bool TryGetClosestPointOnBoundsNotInside(this Bounds bounds, Vector3 point, out Vector3 closestPoint)
        {
            if (!bounds.Contains(point))//外部点
            {
                closestPoint = point;
                return true;
            }
            return TryGetClosestPointOnBoundsSurfaceInternal(bounds, point, out closestPoint);
        }

        /// <summary>
        /// 尝试获取交集
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        public static bool TryGetIntersects(this Bounds a, Bounds b, out Bounds intersection)
        {
            if (a.Intersects(b))
            {
                intersection = GetIntersects(a, b);
                return true;
            }
            intersection = default(Bounds);
            return false;
        }

        /// <summary>
        /// 获取交集
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Bounds GetIntersects(this Bounds a, Bounds b)
        {
            var intersection = new Bounds();
            intersection.SetMinMax(Vector3.Max(a.min, b.min), Vector3.Min(a.max, b.max));
            return intersection;
        }

        /// <summary>
        /// 尝试获取产生交集的最小偏移量；即满足a包围盒偏移结果向量后与b包围盒有交集
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="offset"></param>
        /// <returns>如果已经有交集返回false；无交集时范围true与最小偏移量</returns>
        public static bool TryGetMinOffsetForIntersects(this Bounds a, Bounds b, out Vector3 offset)
        {
            if (a.Intersects(b))
            {
                offset = Vector3.zero;
                return false;
            }
            offset = GetMinOffsetForIntersects(a, b);
            return true;
        }

        /// <summary>
        /// 获取产生交集的最小偏移量；即满足a包围盒偏移结果向量后与b包围盒有交集
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector3 GetMinOffsetForIntersects(this Bounds a, Bounds b)
        {
            var dir = b.center - a.center;

            var x = 0f;
            if (!MathX.ApproximatelyZero(dir.x))
            {
                if (dir.x < 0)
                {
                    x = b.max.x - a.min.x;
                    if (x > 0) x = 0;
                }
                else
                {
                    x = b.min.x - a.max.x;
                    if (x < 0) x = 0;
                }
            }

            var y = 0f;
            if (!MathX.ApproximatelyZero(dir.y))
            {
                if (dir.y < 0)
                {
                    y = b.max.y - a.min.y;
                    if (y > 0) y = 0;
                }
                else
                {
                    y = b.min.y - a.max.y;
                    if (y < 0) y = 0;
                }
            }

            var z = 0f;
            if (!MathX.ApproximatelyZero(dir.z))
            {
                if (dir.z < 0)
                {
                    z = b.max.z - a.min.z;
                    if (z > 0) z = 0;
                }
                else
                {
                    z = b.min.z - a.max.z;
                    if (z < 0) z = 0;
                }
            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 将值转换为到[0,wrapValue]范围的区间的值
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="wrapValue">换行值：要求大于0的值；</param>
        /// <returns>返回[0,wrapValue]区间的值</returns>
        public static float Wrap(this float value, float wrapValue)
        {
            if (wrapValue <= 0) throw new ArgumentException("参数值需大于0", nameof(wrapValue));

            while (value > wrapValue) return value -= wrapValue;
            while (value < 0f) return value += wrapValue;
            return value;
        }

        /// <summary>
        /// 将角度值转换到[0, 360]的值
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns></returns>
        public static float WrapDegrees(this float degrees) => Wrap(degrees, 360);

        /// <summary>
        /// 将角度值转换到[-180, 180]的值
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float WrapDegrees180(this float degrees)
        {
            while (degrees < -180) degrees += 360;
            while (degrees > 180) degrees -= 360;
            return degrees;
        }

        /// <summary>
        /// 将矢量各角度值转换到[-180, 180]的值
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector3 WrapDegrees180(this Vector3 degrees)
        {
            degrees.x = WrapDegrees180(degrees.x);
            degrees.y = WrapDegrees180(degrees.y);
            degrees.z = WrapDegrees180(degrees.z);
            return degrees;
        }

        /// <summary>
        /// 转三维向量
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this V3F v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }

        /// <summary>
        /// 转二维向量
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this V2F v2)
        {
            return new Vector2(v2.x, v2.y);
        }

        /// <summary>
        /// 转矩形
        /// </summary>
        /// <param name="rectF"></param>
        /// <returns></returns>
        public static Rect ToRect(this RectF rectF)
        {
            return new Rect(rectF.x, rectF.y, rectF.width, rectF.height);
        }
    }
}