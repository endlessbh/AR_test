using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;

namespace XCSJ.PluginTools.Notes.Dimensionings
{
    /// <summary>
    /// 尺寸标注辅助类
    /// </summary>
    public class DimensioningHelper
    {
        /// <summary>
        /// 距离
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="direction"></param>
        /// <param name="offsetDistance"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        public static void Distance(Vector3 position0, Vector3 position1, Vector3 direction, float offsetDistance, out Vector3 p0, out Vector3 p1)
        {
            var p0_1 = position1 - position0;
            if (Vector3.Dot(direction, p0_1) > 0)
            {
                Ray ray = new Ray(position0, direction);
                var offset = ray.direction * offsetDistance;

                p0 = ray.GetClosestPointOnRay(position1) + offset;
                p1 = position1 + offset;
            }
            else
            {
                Ray ray = new Ray(position1, direction);
                var offset = ray.direction * offsetDistance;

                p0 = position0 + offset;
                p1 = ray.GetClosestPointOnRay(position0) + offset;
            }
        }

        /// <summary>
        /// 角度
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="center"></param>
        /// <param name="normal"></param>
        /// <param name="offsetDistance"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="angle"></param>
        public static void Angle(Vector3 position0, Vector3 position1, Vector3 center, Vector3 normal, float offsetDistance, out Vector3 p0, out Vector3 p1, out float angle)
        {
            var plane = new Plane(normal, center);
            var p0InPlane = plane.ClosestPointOnPlane(position0);
            var p1InPlane = plane.ClosestPointOnPlane(position1);

            Angle(p0InPlane, p1InPlane, center, offsetDistance, out p0, out p1, out angle);
        }

        /// <summary>
        /// 角度
        /// </summary>
        /// <param name="position0"></param>
        /// <param name="position1"></param>
        /// <param name="center"></param>
        /// <param name="offsetDistance"></param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="angle"></param>
        public static void Angle(Vector3 position0, Vector3 position1, Vector3 center, float offsetDistance, out Vector3 p0, out Vector3 p1, out float angle)
        {
            var p0_center = position0 - center;
            var p1_center = position1 - center;

            angle = Vector3.Angle(p0_center, p1_center);
            p0 = p0_center.normalized * offsetDistance + center;
            p1 = p1_center.normalized * offsetDistance + center;
        }
    }
}
