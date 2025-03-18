using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginTools;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Maths;
using System;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 摆放助手
    /// </summary>
    public static class PutHelper
    {
        /// <summary>
        /// 获取射线碰撞对象面中心
        /// </summary>
        /// <param name="raycastHit"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Vector3 GetRayHitObjectFaceCenter(RaycastHit raycastHit, Bounds bounds)
        {
            return raycastHit.transform.position + raycastHit.transform.GetBoundsTangentOffset(bounds, raycastHit.normal);
        }

        /// <summary>
        /// 依据放置规则设置变换的位置
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="hitTransform"></param>
        /// <param name="putRule"></param>
        /// <param name="putPoint"></param>
        /// <param name="putNormal"></param>
        public static void Put(Transform transform, EPutRule putRule, Vector3 putPoint, Vector3 putNormal)
        {
            if (!transform) return;

            if (CommonFun.GetBounds(out Bounds bounds, transform.gameObject, true, false, false))
            {
                Put(transform, null, bounds, putRule, putPoint, putNormal);
            }
        }

        /// <summary>
        /// 讲变换依据包围盒放置在一个点上
        /// </summary>
        /// <param name="putTransform"></param>
        /// <param name="hitTransform"></param>
        /// <param name="bounds"></param>
        /// <param name="putRule"></param>
        /// <param name="putPoint"></param>
        /// <param name="putNormal"></param>
        public static Vector3 Put(Transform putTransform, Transform hitTransform, Bounds bounds, EPutRule putRule, Vector3 putPoint, Vector3 putNormal)
        {
            if (TryGetPutPosition(putTransform, hitTransform, bounds, putRule, putPoint, putNormal, out var position))
            {
                putTransform.position = putPoint;
            }
            return position;
        }

        /// <summary>
        /// 尝试获取摆放位置
        /// </summary>
        /// <param name="putTransform">摆放对象</param>
        /// <param name="hitTransform">碰撞对象</param>
        /// <param name="bounds"></param>
        /// <param name="putRule"></param>
        /// <param name="putPoint"></param>
        /// <param name="putNormal"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool TryGetPutPosition(Transform putTransform, Transform hitTransform, Bounds bounds, EPutRule putRule, Vector3 putPoint, Vector3 putNormal, out Vector3 position)
        {
            return TryGetPutPosition(putRule, putPoint, putNormal, bounds, bounds.center - putTransform.position, hitTransform, out position);
        }

        /// <summary>
        /// 获取摆放位置
        /// </summary>
        /// <param name="putRule"></param>
        /// <param name="putPoint"></param>
        /// <param name="putNormal"></param>
        /// <param name="bounds"></param>
        /// <param name="putObjectToboundsOffset"></param>
        /// <param name="hitTransform"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static bool TryGetPutPosition(EPutRule putRule, Vector3 putPoint, Vector3 putNormal, Bounds bounds, Vector3 putObjectToboundsOffset, Transform hitTransform, out Vector3 position)
        {
            position = default;

            switch (putRule)
            {
                case EPutRule.Transform:
                    {
                        break;
                    }
                case EPutRule.BoundsCenter:
                    {
                        putPoint += putObjectToboundsOffset;
                        break;
                    }
                case EPutRule.BoundsBottomCenter:
                    {
                        putPoint += putObjectToboundsOffset + new Vector3(0, bounds.size.y / 2, 0);
                        break;
                    }
                case EPutRule.BoundsTangent:
                    {
                        if (!hitTransform) return false;

                        putPoint += putObjectToboundsOffset + hitTransform.GetBoundsTangentOffset(bounds, putNormal);
                        break;
                    }
                case EPutRule.SpereBoundsTangentPoint:
                    {
                        putPoint += putNormal * bounds.size.magnitude / 2;
                        break;
                    }
            }
            position = putPoint;
            return true;
        }

        /// <summary>
        /// 求射线在游戏对象上的切点偏移量
        /// </summary>
        /// <param name="transform">变换</param>
        /// <param name="bounds">法线</param>
        /// <param name="putNormal">射线碰撞点法线</param>
        /// <returns></returns>
        public static Vector3 GetBoundsTangentOffset(this Transform transform, Bounds bounds, Vector3 putNormal)
        {
            var offset = Vector3.zero;

            var rs = Vector3.Dot(transform.up, putNormal);
            if (MathX.ApproximatelyZero(rs))
            {

            }
            else if (rs> 0)
            {
                offset += bounds.size.y / 2 * Vector3.up;
            }
            else if (-rs > 0)
            {
                offset += -bounds.size.y / 2 * Vector3.up;
            }

            rs = Vector3.Dot(transform.forward, putNormal);
            if (MathX.ApproximatelyZero(rs))
            {

            }
            else if (rs > 0)
            {
                offset += bounds.size.z / 2 * Vector3.forward;
            }
            else if (-rs > 0)
            {
                offset += -bounds.size.z / 2 * Vector3.forward;
            }

            rs = Vector3.Dot(transform.right, putNormal);
            if (MathX.ApproximatelyZero(rs))
            {

            }
            else if (rs > 0)
            {
                offset += bounds.size.x / 2 * Vector3.right;
            }
            else if (-rs > 0)
            {
                offset += -bounds.size.x / 2 * Vector3.right;
            }
            return offset;
        }

        /// <summary>
        /// 旋转游戏对象
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="rotationRule"></param>
        public static void Rotate(this Transform transform, Transform referenceTransform, ERotationRule rotationRule)
        {
            switch (rotationRule)
            {
                case ERotationRule.LookAt:
                    {
                        transform.LookAt(referenceTransform);
                        break;
                    }
                case ERotationRule.FaceAroundUp:
                    {
                        transform.Face(referenceTransform);
                        break;
                    }
                case ERotationRule.BackLookAt:
                    {
                        transform.LookAt(referenceTransform);
                        transform.RotateAround(transform.position, transform.up, 180);
                        break;
                    }
                case ERotationRule.BackFaceAroundUp:
                    {
                        transform.Face(referenceTransform);
                        transform.RotateAround(transform.position, transform.up, 180);
                        break;
                    }
            }
        }

        /// <summary>
        /// 变换绕自身垂直轴，面向参考对象
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="referenceTransform"></param>
        public static void Face(this Transform transform, Transform referenceTransform)
        {
            var pos = Vector3.ProjectOnPlane(referenceTransform.position, Vector3.up);
            pos.y += transform.position.y;
            transform.LookAt(pos);
        }

    }

    /// <summary>
    /// 放置规则
    /// </summary>
    [Name("放置规则")]
    public enum EPutRule
    {
        [Name("无")]
        None,

        [Name("变换")]
        Transform,

        [Name("包围盒中心")]
        BoundsCenter,

        [Name("包围盒底面中心")]
        BoundsBottomCenter,

        [Name("包围相切")]
        [Tip("沿碰撞法线所在包围盒面法线方向移动半个边长的距离，确保包围盒相切与碰撞面", "Move half the length of the edge along the normal direction of the bounding box face where the collision normal is located to ensure that the bounding box is tangent to the collision face")]
        BoundsTangent,

        [Name("球形包围盒切点")]
        SpereBoundsTangentPoint,
    }

    /// <summary>
    /// 包围盒规则
    /// </summary>
    [Name("包围盒规则")]
    public enum EBoundsRule
    {
        [Name("无")]
        None,

        [Name("自身")]
        Self,

        [Name("自身与子级")]
        SelfAndChildren,

        [Name("包围盒")]
        Bounds,
    }

    /// <summary>
    /// 旋转规则
    /// </summary>
    [Name("旋转规则")]
    public enum ERotationRule
    {
        [Name("无")]
        None,

        [Name("面向")]
        LookAt,

        [Name("垂直面向")]
        FaceAroundUp,

        [Name("背向")]
        BackLookAt,

        [Name("垂直背向")]
        BackFaceAroundUp,
    }

}
