using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Helpers
{
    /// <summary>
    /// 布局助手
    /// </summary>
    public static class LayoutHelper
    {
        #region 变换

        private static void HandleInternal(List<Transform> transforms, Transform transform, Action<Transform> eachoneAction)
        {
            if (!transform || transforms == null || eachoneAction == null) return;
            transforms = transforms.Where(t => t).ToList();

            foreach (var t in transforms)
            {
                eachoneAction(t);
            }
        }

        public static void SamePositionX(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetPosition(new Vector3(transform.position.x, t.position.y, t.position.z)));
        }

        public static void SamePositionY(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetPosition(new Vector3(t.position.x, transform.position.y, t.position.z)));
        }

        public static void SamePositionZ(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetPosition(new Vector3(t.position.x, t.position.y, transform.position.z)));
        }

        public static void SameLocalScaleX(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetLocalScale(new Vector3(transform.localScale.x, t.localScale.y, t.localScale.z)));
        }

        public static void SameLocalScaleY(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetLocalScale(new Vector3(t.localScale.x, transform.localScale.y, t.localScale.z)));
        }

        public static void SameLocalScaleZ(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetLocalScale(new Vector3(t.localScale.x, t.localScale.y, transform.localScale.z)));
        }

        public static void SameEulerAnglesX(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetRotation(new Vector3(transform.eulerAngles.x, t.eulerAngles.y, t.eulerAngles.z)));
        }

        public static void SameEulerAnglesY(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetRotation(new Vector3(t.eulerAngles.x, transform.eulerAngles.y, t.eulerAngles.z)));
        }

        public static void SameEulerAnglesZ(this List<Transform> transforms, Transform transform)
        {
            HandleInternal(transforms, transform, t => t.XSetRotation(new Vector3(t.eulerAngles.x, t.eulerAngles.y, transform.eulerAngles.z)));
        }

        public static void CenterSameSpace(this List<Transform> transforms, Transform transform1, Transform transform2)
        {
            if (!transform1 || !transform2 || transforms == null) return;
            transforms = transforms.Where(t => t).ToList();

            var min = transform1.position;
            var max = transform2.position;
            var space = transforms.Count <= 1 ? Vector3.zero : ((max - min) / (transforms.Count - 1));

            for (int i = 0; i < transforms.Count; ++i)
            {
                transforms[i].XSetPosition(min + space * i);
            }
        }

        #endregion

        #region 矩形变换

        public static void LeftAlign(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var left = rectTransform.position.x - rectTransform.rect.width / 2;

            foreach (var t in rectTransforms)
            {
                t.XSetPosition(new Vector3(left + t.rect.width / 2, t.position.y, t.position.z));
            }
        }

        public static void RightAlign(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var right = rectTransform.position.x + rectTransform.rect.width / 2;

            foreach (var t in rectTransforms)
            {
                t.XSetPosition(new Vector3(right - t.rect.width / 2, t.position.y, t.position.z));
            }
        }

        public static void TopAlign(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var top = rectTransform.position.y + rectTransform.rect.height / 2;

            foreach (var t in rectTransforms)
            {
                t.XSetPosition(new Vector3(t.position.x, top - t.rect.height / 2, t.position.z));
            }
        }

        public static void BottomAlign(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var bottom = rectTransform.position.y - rectTransform.rect.height / 2;

            foreach (var t in rectTransforms)
            {
                t.XSetPosition(new Vector3(t.position.x, bottom + t.rect.height / 2, t.position.z));
            }
        }

        public static void CenterHorizontalAlign(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var center = rectTransform.position.y;

            foreach (var t in rectTransforms)
            {
                t.XSetPosition(new Vector3(t.position.x, center, t.position.z));
            }
        }

        public static void CenterVerticalAlign(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var center = rectTransform.position.x;

            foreach (var t in rectTransforms)
            {
                t.XSetPosition(new Vector3(center, t.position.y, t.position.z));
            }
        }

        public static void CenterHorizontalSameSpace(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var leftX = rectTransform1.position.x;
            var rightX = rectTransform2.position.x;
            var space = rectTransforms.Count <= 1 ? 0f : ((rightX - leftX) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];
                t.XSetPosition(new Vector3(leftX + i * space, t.position.y, t.position.z));
            }
        }

        public static void CenterVerticalSameSpace(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var topY = rectTransform1.position.y;
            var bottomY = rectTransform2.position.y;

            var space = rectTransforms.Count <= 1 ? 0f : ((topY - bottomY) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; ++i)
            {
                var t = rectTransforms[i];
                t.XSetPosition(new Vector3(t.position.x, topY - i * space, t.position.z));
            }
        }

        public static void BoundsHorizontalSameSpace(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
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
                t.XSetPosition(new Vector3(x, t.position.y, t.position.z));
                x += (space + t.rect.width / 2);
            }
        }

        public static void BoundsVerticalSameSpace(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
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
                t.XSetPosition(new Vector3(t.position.x, y, t.position.z));
                y -= (space + t.rect.height / 2);
            }
        }

        public static void SameWidth(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var size = rectTransform.sizeDelta;

            foreach (var t in rectTransforms)
            {
                t.XSetSizeDelta(new Vector2(size.x, t.sizeDelta.y));
            }
        }

        public static void SameHeight(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            if (!rectTransform || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var size = rectTransform.sizeDelta;

            foreach (var t in rectTransforms)
            {
                t.XSetSizeDelta(new Vector2(t.sizeDelta.x, size.y));
            }
        }

        public static void SameSize(this List<RectTransform> rectTransforms, RectTransform rectTransform)
        {
            SameWidth(rectTransforms, rectTransform);
            SameHeight(rectTransforms, rectTransform);
        }

        public static void DirectionReset(this List<RectTransform> rectTransforms)
        {
            SetRightDirection(rectTransforms, Vector3.right);
        }

        public static void SetRightDirection(this List<RectTransform> rectTransforms, Vector3 right)
        {
            if (rectTransforms == null) return;
            rectTransforms.ForEach(t =>
            {
                if (t) t.right = right;
            });
        }

        public static void IncreaseWidth(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var min = rectTransform1.rect.width;
            var max = rectTransform2.rect.width;

            var space = rectTransforms.Count <= 1 ? 0f : ((max - min) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; i++)
            {
                var t = rectTransforms[i];
                t.XSetSizeDelta(new Vector2(min + i * space, t.sizeDelta.y));
            }
        }

        public static void IncreaseHeight(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            if (!rectTransform1 || !rectTransform2 || rectTransforms == null) return;
            rectTransforms = rectTransforms.Where(t => t).ToList();

            var min = rectTransform1.rect.height;
            var max = rectTransform2.rect.height;

            var space = rectTransforms.Count <= 1 ? 0f : ((max - min) / (rectTransforms.Count - 1));

            for (int i = 0; i < rectTransforms.Count; i++)
            {
                var t = rectTransforms[i];
                t.XSetSizeDelta(new Vector2(t.sizeDelta.x, min + i * space));
            }
        }

        public static void IncreaseSize(this List<RectTransform> rectTransforms, RectTransform rectTransform1, RectTransform rectTransform2)
        {
            IncreaseWidth(rectTransforms, rectTransform1, rectTransform2);
            IncreaseHeight(rectTransforms, rectTransform1, rectTransform2);
        }

        /// <summary>
        /// 获取最小
        /// </summary>
        /// <param name="rectTransforms"></param>
        /// <returns></returns>
        public static RectTransform GetRectTransform_AnchoredPositionMinY(this List<RectTransform> rectTransforms)
        {
            if (rectTransforms == null) return null;

            var min = float.MaxValue;
            var current = rectTransforms.FirstOrDefault();
            if (current)
            {
                min = current.anchoredPosition.y;
            }
            for (int i = 1; i < rectTransforms.Count; i++)
            {
                var item = rectTransforms[i];
                if (item && item.anchoredPosition.y < min)
                {
                    min = current.anchoredPosition.y;
                    current = item;
                }
            }
            return current;
        }

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, int> compare, Func<TSource, TResult> cast, TResult defaultResult = default)
        {
            if (source == null) return defaultResult;
            if (compare == null) return defaultResult;
            if (cast == null) return defaultResult;
            TSource value = default;
            bool hasValue = false;

            foreach (var s in source)
            {
                if (hasValue)
                {
                    if (compare(value, s) > 0)
                    {
                        value = s;
                    }
                }
                else
                {
                    hasValue = true;
                    value = s;
                }
            }
            return cast(value);
        }
        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, int> compare, Func<TSource, TResult> cast, TResult defaultResult = default)
        {
            if (source == null) return defaultResult;
            if (compare == null) return defaultResult;
            if (cast == null) return defaultResult;
            TSource value = default;
            bool hasValue = false;

            foreach (var s in source)
            {
                if (hasValue)
                {
                    if (compare(value, s) < 0)
                    {
                        value = s;
                    }
                }
                else
                {
                    hasValue = true;
                    value = s;
                }
            }
            return cast(value);
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, int> compare, TSource defaultResult = default)
        {
            return Min(source, compare, s => s, defaultResult);
        }


        public static TSource Max<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, int> compare, TSource defaultResult = default)
        {
            return Max(source, compare, s => s, defaultResult);
        }

        /// <summary>
        /// 获取最小
        /// </summary>
        /// <param name="rectTransforms"></param>
        /// <returns></returns>
        public static RectTransform GetRectTransform_AnchoredPositionMinY(this IEnumerable<RectTransform> rectTransforms)
        {
            return rectTransforms?.Where(r => r).Min((r1, r2) => r1.anchoredPosition.y <= r2.anchoredPosition.y ? -1 : 1);
        }

        /// <summary>
        /// 获取最大
        /// </summary>
        /// <param name="rectTransforms"></param>
        /// <returns></returns>
        public static RectTransform GetRectTransform_AnchoredPositionMaxY(this IEnumerable<RectTransform> rectTransforms)
        {
            return rectTransforms?.Where(r => r).Max((r1, r2) => r1.anchoredPosition.y <= r2.anchoredPosition.y ? -1 : 1);
        }

        #endregion
    }
}
