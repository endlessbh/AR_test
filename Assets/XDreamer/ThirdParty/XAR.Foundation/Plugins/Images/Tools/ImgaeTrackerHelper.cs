using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

#if XDREAMER_AR_FOUNDATION
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#endif

namespace XCSJ.PluginXAR.Foundation.Images.Tools
{
    /// <summary>
    /// 图像跟踪器辅助类
    /// </summary>
    public static class ImgaeTrackerHelper
    {
#if XDREAMER_AR_FOUNDATION

        #region 添加

        /// <summary>
        /// 添加到库
        /// </summary>
        /// <param name="trackedImageManager"></param>
        /// <param name="image"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Coroutine AddToLibrary<T>(this ARTrackedImageManager trackedImageManager, T image, Action<bool, IEnumerable<T>> onFinished = null)
            where T : IImageProcessor
        {
            return AddToLibrary(trackedImageManager, trackedImageManager, image, onFinished);
        }

        /// <summary>
        /// 添加到库
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="trackedImageManager"></param>
        /// <param name="image"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Coroutine AddToLibrary<T>(this MonoBehaviour behaviour, ARTrackedImageManager trackedImageManager, T image, Action<bool, IEnumerable<T>> onFinished = null)
            where T : IImageProcessor
        {
            return AddToLibrary(behaviour, trackedImageManager, new T[] { image }, onFinished);
        }

        /// <summary>
        /// 添加到库
        /// </summary>
        /// <param name="trackedImageManager"></param>
        /// <param name="images"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Coroutine AddToLibrary<T>(this ARTrackedImageManager trackedImageManager, IEnumerable<T> images, Action<bool, IEnumerable<T>> onFinished = null)
            where T : IImageProcessor
        {
            return AddToLibrary(trackedImageManager, trackedImageManager, images, onFinished);
        }

        /// <summary>
        /// 添加到库
        /// </summary>
        /// <param name="behaviour"></param>
        /// <param name="trackedImageManager"></param>
        /// <param name="images"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public static Coroutine AddToLibrary<T>(this MonoBehaviour behaviour, ARTrackedImageManager trackedImageManager, IEnumerable<T> images, Action<bool, IEnumerable<T>> onFinished = null)
            where T : IImageProcessor
        {
            if (!behaviour || !trackedImageManager || images == null) return default;
            return behaviour.StartCoroutine(_AddToLibrary(trackedImageManager, images, onFinished));
        }

        private static IEnumerator _AddToLibrary<T>(ARTrackedImageManager trackedImageManager, IEnumerable<T> images, Action<bool, IEnumerable<T>> onFinished)
            where T : IImageProcessor
        {
            yield return new WaitForEndOfFrame();

            if (trackedImageManager.referenceLibrary is MutableRuntimeReferenceImageLibrary mutableLibrary)
            {
                var list = new List<T>();
                foreach (var image in images)
                {
                    if (image.AddToLibrary(mutableLibrary))
                    {
                        list.Add(image);
                    }
                }

                yield return new WaitForEndOfFrame();

                yield return new WaitUntil(() => list.All(image => image.addCompleted));

                onFinished?.Invoke(true, list);
            }
            else
            {
                onFinished?.Invoke(false, images);
            }
        }

        #endregion

        #region 移除

        /// <summary>
        /// 从库中移除
        /// </summary>
        /// <param name="trackedImageManager"></param>
        /// <param name="image"></param>
        /// <param name="onFinished"></param>
        public static void RemoveFromLibiary<T>(this ARTrackedImageManager trackedImageManager, T image, Action<bool, T> onFinished = null)
            where T : IImageProcessor
        {
            if (!trackedImageManager || image == null) return;
        }

        #endregion

#endif
    }

    /// <summary>
    /// 图像处理器接口
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// 纹理
        /// </summary>
        Texture2D texture { get; }

        /// <summary>
        /// 标识是否已添加完成
        /// </summary>
        bool addCompleted { get; }

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 添加倒库
        /// </summary>
        /// <param name="mutableLibrary"></param>
        /// <returns></returns>
        bool AddToLibrary(MutableRuntimeReferenceImageLibrary mutableLibrary);

#endif
    }
}
