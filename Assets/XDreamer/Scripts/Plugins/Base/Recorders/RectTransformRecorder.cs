using System;
using System.Collections.Generic;
using UnityEngine;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 矩形变换记录器:用于记录矩形变换的信息；
    /// </summary>
    public class RectTransformRecorder : Recorder<RectTransform, RectTransformRecorder.Info>, IRectTransformRecorder
    {
        /// <summary>
        /// 矩形变换记录列表
        /// </summary>
        public IRectTransformRecord[] transformRecords => _records.ToArray();

        /// <summary>
        /// 记录游戏对象的变换
        /// </summary>
        /// <param name="gameObject"></param>
        public void Record(GameObject gameObject)
        {
            if (gameObject)
            {
                var rectTransform = gameObject.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    Record(rectTransform);
                }
            }
        }

        /// <summary>
        /// 批量记录游戏对象的变换
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Record(IEnumerable<GameObject> gameObjects)
        {
            if (gameObjects == null) return;
            foreach (var go in gameObjects)
            {
                Record(go);
            }
        }

        /// <summary>
        /// 信息
        /// </summary>
        public class Info : IRectTransformRecord
        {
            /// <summary>
            /// 变换
            /// </summary>
            public RectTransform rectTransform { get; set; }

            /// <summary>
            /// 本地位置
            /// </summary>
            public Vector2 offsetMax { get; set; }

            /// <summary>
            /// 本地旋转
            /// </summary>
            public Vector2 offsetMin { get; set; }

            /// <summary>
            /// 本地缩放
            /// </summary>
            public Vector3 anchoredPosition3D { get; set; }

            /// <summary>
            /// 世界位置
            /// </summary>
            public Vector2 pivot { get; set; }

            /// <summary>
            /// 世界旋转
            /// </summary>
            public Vector2 sizeDelta { get; set; }

            /// <summary>
            /// 世界位置
            /// </summary>
            public Vector2 anchorMax { get; set; }

            /// <summary>
            /// 世界旋转
            /// </summary>
            public Vector2 anchoredPosition { get; set; }

            /// <summary>
            /// 世界旋转
            /// </summary>
            public Vector2 anchorMin { get; set; }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="transform"></param>
            public virtual void Record(RectTransform rectTransform)
            {
                if (rectTransform)
                {
                    this.rectTransform = rectTransform;

                    offsetMax = rectTransform.offsetMax;
                    offsetMin = rectTransform.offsetMin;
                    anchoredPosition3D = rectTransform.anchoredPosition3D;

                    pivot = rectTransform.pivot;
                    sizeDelta = rectTransform.sizeDelta;

                    anchorMax = rectTransform.anchorMax;
                    anchoredPosition = rectTransform.anchoredPosition;
                    anchorMin = rectTransform.anchorMin;
                }
            }

            /// <summary>
            /// 记录
            /// </summary>
            /// <param name="arg1"></param>
            public void Record(GameObject arg1)
            {
                var rectTransform = arg1.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    Record(rectTransform);
                }
            }

            /// <summary>
            /// 恢复：基于本地信息执行恢复
            /// </summary>
            public virtual void Recover()
            {
                if (rectTransform)
                {
                    rectTransform.offsetMax = offsetMax;
                    rectTransform.offsetMin = offsetMin;
                    rectTransform.anchoredPosition3D = anchoredPosition3D;

                    rectTransform.pivot = pivot;
                    rectTransform.sizeDelta = sizeDelta;

                    rectTransform.anchorMax = anchorMax;
                    rectTransform.anchoredPosition = anchoredPosition;
                    rectTransform.anchorMin = anchorMin;
                }
            }
        }
    }

    /// <summary>
    /// 矩形变换记录接口
    /// </summary>
    public interface IRectTransformRecord : ISingleRecord<RectTransform>, ISingleRecord<GameObject>
    {
        /// <summary>
        /// 矩形转换
        /// </summary>
        RectTransform rectTransform { get; set; }

        /// <summary>
        /// 本地位置
        /// </summary>
        Vector2 offsetMax { get; set; }

        /// <summary>
        /// 本地旋转
        /// </summary>
        Vector2 offsetMin { get; set; }

        /// <summary>
        /// 本地缩放
        /// </summary>
        Vector3 anchoredPosition3D { get; set; }

        /// <summary>
        /// 世界位置
        /// </summary>
        Vector2 pivot { get; set; }

        /// <summary>
        /// 世界旋转
        /// </summary>
        Vector2 sizeDelta { get; set; }

        /// <summary>
        /// 世界位置
        /// </summary>
        Vector2 anchorMax { get; set; }

        /// <summary>
        /// 世界旋转
        /// </summary>
        Vector2 anchoredPosition { get; set; }

        /// <summary>
        /// 世界旋转
        /// </summary>
        Vector2 anchorMin { get; set; }
    }

    /// <summary>
    /// 变换记录器接口
    /// </summary>
    public interface IRectTransformRecorder : IBatchRecorder<RectTransform>, IBatchRecorder<GameObject>
    {
        /// <summary>
        /// 记录列表
        /// </summary>
        IRectTransformRecord[] transformRecords { get; }
    }
}
