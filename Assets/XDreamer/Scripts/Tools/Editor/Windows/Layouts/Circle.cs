using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.EditorTools.Windows.Layouts
{
    public class Circle
    {
        [Name("方向")]
        public enum EDirection
        {
            [Name("无")]
            [Tip("不对方向做调整", "Do not adjust the direction")]
            None,

            [Name("正X轴")]
            [Tip("变换的正X轴方向朝向圆形", "The transformed positive X-axis direction is toward the circle")]
            X,

            [Name("负X轴")]
            [Tip("变换的负X轴方向朝向圆形", "The transformed negative X-axis direction is toward the circle")]
            NX,

            [Name("正Y轴")]
            [Tip("变换的正Y轴方向朝向圆形", "The positive y-axis direction of the transformation is toward the circle")]
            Y,

            [Name("负Y轴")]
            [Tip("变换的负Y轴方向朝向圆形", "The transformed negative Y-axis direction is toward the circle")]
            NY,

            [Name("正X轴")]
            [Tip("变换的正X轴方向朝向圆形", "The transformed positive X-axis direction is toward the circle")]
            Z,

            [Name("负X轴")]
            [Tip("变换的负X轴方向朝向圆形", "The transformed negative X-axis direction is toward the circle")]
            NZ,

            [Name("正负X轴")]
            [Tip("变换的正负X轴方向朝向圆形", "The positive and negative X-axis direction of the transformation is towards the circle")]
            PNX,
        }

        public static void UpdateDirection(List<Transform> rectTransforms, Vector3 center, EDirection direction)
        {
            if (rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();
            if (rectTransforms.Count == 0) return;

            foreach(var t in rectTransforms)
            {
                UpdateDirectionInternal(t, center, direction);
            }
        }

        private static void UpdateDirectionInternal(Transform transform, Vector3 center, EDirection direction)
        {            
            switch (direction)
            {
                case EDirection.X:
                    {
                        transform.right = transform.position - center;
                        break;
                    }
                case EDirection.NX:
                    {
                        transform.right = center - transform.position;
                        break;
                    }
                case EDirection.Y:
                    {
                        transform.up = transform.position - center;
                        break;
                    }
                case EDirection.NY:
                    {
                        transform.up = center - transform.position;
                        break;
                    }
                case EDirection.Z:
                    {
                        transform.forward = transform.position - center;
                        break;
                    }
                case EDirection.NZ:
                    {
                        transform.forward = center - transform.position;
                        break;
                    }
                case EDirection.PNX:
                    {
                        var d = transform.position - center;
                        transform.right = d.x >= 0 ? d : -d;
                        break;
                    }
            }
        }

        public static void Layout(List<RectTransform> rectTransforms, Vector3 center, float r, float angle = 0, EDirection direction = EDirection.None)
        {
            if (rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();
            if (rectTransforms.Count == 0) return;

            var d = Mathf.PI * 2 / rectTransforms.Count;
            angle = angle / Mathf.PI;

            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];

                var a = angle + i * d;
                var x = r * Mathf.Cos(a);
                var y = r * Mathf.Sin(a);

                t.position = center + new Vector3(x, y, t.position.z);
                UpdateDirectionInternal(t, center, direction);
            }
        }

        public static void Layout(Vector3 planeNormal, List<Transform> transforms, Vector3 center, float r, float angle = 0, EDirection direction = EDirection.None)
        {
            if (transforms == null) return;
            transforms = transforms.Where(t => t).ToList();
            if (transforms.Count == 0) return;

            Plane plane = new Plane(planeNormal, center);

            var rightInPlane = plane.ClosestPointOnPlane(Vector3.right);
            var right = rightInPlane - center;
            if (right == Vector3.zero)
            {
                var upInPlane = plane.ClosestPointOnPlane(Vector3.up);
                right = upInPlane - center;
            }
            right.Normalize();

            var d = 360f / transforms.Count;

            for (int i = 0; i < transforms.Count; ++i)
            {
                var t = transforms[i];
                t.up = planeNormal;

                var a = angle + i * d;
                var q = Quaternion.AngleAxis(a, planeNormal);

                var p = q * right * r;

                t.position = center + p;
                UpdateDirectionInternal(t, center, direction);
            }
        }
    }
}
