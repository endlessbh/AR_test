using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorTools.Windows.Layouts
{
    public class LayoutHelper
    {
        private static void HandleInternal(List<Transform> transforms, Transform transform, Action<Transform> eachoneAction)
        {
            if (!transform || transforms == null || eachoneAction==null) return;
            transforms = transforms.Where(t => t).ToList();

            foreach (var t in transforms)
            {
                eachoneAction(t);
            }
        }

        public static void SamePositionX(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.position = new Vector3(transform.position.x, t.position.y, t.position.z));
            HandleInternal(transforms, transform, t => t.XSetPosition(new Vector3(transform.position.x, t.position.y, t.position.z)));
        }

        public static void SamePositionY(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.position = new Vector3(t.position.x, transform.position.y, t.position.z));
        }

        public static void SamePositionZ(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.position = new Vector3(t.position.x, t.position.y, transform.position.z));
        }

        public static void SameLocalScaleX(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.localScale = new Vector3(transform.localScale.x, t.localScale.y, t.localScale.z));
        }

        public static void SameLocalScaleY(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.localScale = new Vector3(t.localScale.x, transform.localScale.y, t.localScale.z));
        }

        public static void SameLocalScaleZ(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.localScale = new Vector3(t.localScale.x, t.localScale.y, transform.localScale.z));
        }

        public static void SameEulerAnglesX(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.eulerAngles = new Vector3(transform.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z));
        }

        public static void SameEulerAnglesY(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.eulerAngles = new Vector3(t.eulerAngles.x, transform.eulerAngles.y, t.eulerAngles.z));
        }

        public static void SameEulerAnglesZ(List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.eulerAngles = new Vector3(t.eulerAngles.x, t.eulerAngles.y, transform.eulerAngles.z));
        }

        public static void CenterSameSpace(List<Transform> transforms, Transform transform1, Transform transform2)
        {
            if (!transform1 || !transform2 || transforms == null) return;
            transforms = transforms.Where(t => t).ToList();

            var min = transform1.position;
            var max = transform2.position;
            var space = transforms.Count <= 1 ? Vector3.zero : ((max - min) / (transforms.Count - 1));

            for (int i = 0; i < transforms.Count; ++i)
            {
                transforms[i].position = min + space * i;
            }
        }


        public static void LeftAlign(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var left = rectTransform.position.x - rectTransform.rect.width / 2;

            foreach (var t in rectTransforms)
            {
                t.position = new Vector3(left + t.rect.width / 2, t.position.y, t.position.z);
            }
        }

        public static void RightAlign(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var right = rectTransform.position.x + rectTransform.rect.width / 2;

            foreach (var t in rectTransforms)
            {
                t.position = new Vector3(right - t.rect.width / 2, t.position.y, t.position.z);
            }
        }

        public static void TopAlign(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var top = rectTransform.position.y + rectTransform.rect.height / 2;

            foreach (var t in rectTransforms)
            {
                t.position = new Vector3(t.position.x, top - t.rect.height / 2, t.position.z);
            }
        }

        public static void BottomAlign(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var bottom = rectTransform.position.y - rectTransform.rect.height / 2;

            foreach (var t in rectTransforms)
            {
                t.position = new Vector3(t.position.x, bottom + t.rect.height / 2, t.position.z);
            }
        }

        public static void CenterHorizontalAlign(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var center = rectTransform.position.y;

            foreach (var t in rectTransforms)
            {
                t.position = new Vector3(t.position.x, center, t.position.z);
            }
        }

        public static void CenterVerticalAlign(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var center = rectTransform.position.x;

            foreach (var t in rectTransforms)
            {
                t.position = new Vector3(center, t.position.y, t.position.z);
            }
        }

        public static void CenterHorizontalSameSpace(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var leftX = rectTransform1.position.x;
            var rightX = rectTransform2.position.x;
            var space = rectTransforms.Count <= 1 ? 0f : ((rightX - leftX) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];
                t.position = new Vector3(leftX + i * space, t.position.y, t.position.z);
            }
        }

        public static void CenterVerticalSameSpace(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var topY = rectTransform1.position.y;
            var bottomY = rectTransform2.position.y;

            var space = rectTransforms.Count <= 1 ? 0f : ((topY - bottomY) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];
                t.position = new Vector3(t.position.x, topY - i * space, t.position.z);
            }
        }

        public static void BoundsHorizontalSameSpace(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();
            if (rectTransforms.Count <= 2) return;

            var leftX = rectTransform1.position.x;
            var rightX = rectTransform2.position.x;

            float w = 0;
            foreach (var t in rectTransforms)
            {
                w += t.rect.width;
            }
            w -= (rectTransforms[0].rect.width / 2 + rectTransforms[rectTransforms.Count - 1].rect.width / 2);

            var space = (rightX - leftX - w) / (rectTransforms.Count - 1);
            var x = leftX - rectTransforms[0].rect.width / 2;
            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];
                x += t.rect.width / 2;
                t.position = new Vector3(x, t.position.y, t.position.z);
                x += (space + t.rect.width / 2);
            }
        }

        public static void BoundsVerticalSameSpace(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();
            if (rectTransforms.Count <= 2) return;

            var topY = rectTransform1.position.y;
            var bottomY = rectTransform2.position.y;

            float h = 0;
            foreach (var t in rectTransforms)
            {
                h += t.rect.height;
            }
            h -= (rectTransforms[0].rect.height / 2 + rectTransforms[rectTransforms.Count - 1].rect.height / 2);

            var space = (topY - bottomY - h) / (rectTransforms.Count - 1);
            var y = topY + rectTransforms[0].rect.height / 2;
            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];
                y -= t.rect.height / 2;
                t.position = new Vector3(t.position.x, y, t.position.z);
                y -= (space + t.rect.height / 2);
            }
        }

        public static void SameWidth(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var size = rectTransform.sizeDelta;

            foreach (var t in rectTransforms)
            {
                t.sizeDelta = new Vector2(size.x, t.sizeDelta.y);
            }
        }

        public static void SameHeight(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var size = rectTransform.sizeDelta;

            foreach (var t in rectTransforms)
            {
                t.sizeDelta = new Vector2(t.sizeDelta.x, size.y);
            }
        }

        public static void SameSize(List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            SameWidth(rectTransforms, rectTransform);
            SameHeight(rectTransforms, rectTransform);
        }

        public static void DirectionReset(List<RectTransform> rectTransforms)
        {
            SetRightDirection(rectTransforms, Vector3.right);
        }

        public static void SetRightDirection(List<RectTransform> rectTransforms, Vector3 right)
        {
            if (rectTransforms == null) return;
            rectTransforms.ForEach(t =>
            {
                if (t) t.right = right;
            });
        }

        public static void IncreaseWidth(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var min = rectTransform1.rect.width;
            var max = rectTransform2.rect.width;

            var space = rectTransforms.Count <= 1 ? 0f : ((max - min) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; i++)
            {
                var t = rectTransforms[i];
                t.sizeDelta = new Vector2(min + i * space, t.sizeDelta.y);
            }
        }

        public static void IncreaseHeight(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var min = rectTransform1.rect.height;
            var max = rectTransform2.rect.height;

            var space = rectTransforms.Count <= 1 ? 0f : ((max - min) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; i++)
            {
                var t = rectTransforms[i];
                t.sizeDelta = new Vector2(t.sizeDelta.x, min + i * space);
            }
        }

        public static void IncreaseSize(List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            IncreaseWidth(rectTransforms, rectTransform1, rectTransform2);
            IncreaseHeight(rectTransforms, rectTransform1, rectTransform2);
        }
    }
}
