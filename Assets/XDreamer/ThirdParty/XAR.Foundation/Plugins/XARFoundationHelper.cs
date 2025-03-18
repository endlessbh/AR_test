#if XDREAMER_AR_FOUNDATION
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using XCSJ.PluginXAR.Foundation.Base.Tools;
#endif

namespace XCSJ.PluginXAR.Foundation
{
    /// <summary>
    /// AR Foundation辅助类
    /// </summary>
    public static class XARFoundationHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "AR Foundation";

#if XDREAMER_AR_FOUNDATION

        /// <summary>
        /// 尝试通过索引获取对应的项
        /// </summary>
        /// <typeparam name="TTrackable"></typeparam>
        /// <param name="trackables"></param>
        /// <param name="index"></param>
        /// <param name="trackable"></param>
        /// <returns></returns>
        public static bool TryGetByIndex<TTrackable>(this TrackableCollection<TTrackable> trackables, int index, out TTrackable trackable)
        {
            if (trackables != null && index >= 0 && index < trackables.count)
            {
                int i = 0;
                foreach (var t in trackables)
                {
                    if (i == index)
                    {
                        trackable = t;
                        return true;
                    }
                    i++;
                }
            }
            trackable = default;
            return false;
        }

        /// <summary>
        /// 判断平面是否被包含在另一个平面中；即平面是否是另一个平面的子集；
        /// </summary>
        /// <param name="plane">平面</param>
        /// <param name="subsumedBy">包含在平面</param>
        /// <returns>如[平面]是[包含在平面]的子集(或是相同的有效对象)时，返回True；参数无效或不是子集时，返回False；</returns>
        public static bool IsSubsumedBy(this ARPlane plane, ARPlane subsumedBy)
        {
            if (!plane || !subsumedBy) return false;
            if (plane == subsumedBy) return true;
            return IsSubsumedBy(plane.subsumedBy, subsumedBy);
        }

        /// <summary>
        /// 处理跟踪事件
        /// </summary>
        /// <param name="lastTrackingState"></param>
        /// <param name="trackingState"></param>
        /// <param name="onTrackEvent"></param>
        public static void HandleTrackerEvent(TrackingState lastTrackingState, TrackingState trackingState, Action<ETrackEvent> onTrackEvent)
        {
            switch (lastTrackingState)//上次的状态
            {
                case TrackingState.None:
                    {
                        switch (trackingState)//当前状态
                        {
                            case TrackingState.None:
                                {
                                    onTrackEvent(ETrackEvent.OnNoneAlways);
                                    onTrackEvent(ETrackEvent.OnNone);
                                    break;
                                }
                            case TrackingState.Limited:
                                {
                                    onTrackEvent(ETrackEvent.OnNoneToLimited);
                                    onTrackEvent(ETrackEvent.OnToLimited);
                                    onTrackEvent(ETrackEvent.OnLimited);
                                    break;
                                }
                            case TrackingState.Tracking:
                                {
                                    onTrackEvent(ETrackEvent.OnNoneToTracking);
                                    onTrackEvent(ETrackEvent.OnToTracking);
                                    onTrackEvent(ETrackEvent.OnTracking);
                                    break;
                                }
                        }
                        break;
                    }
                case TrackingState.Limited:
                    {
                        switch (trackingState)//当前状态
                        {
                            case TrackingState.None:
                                {
                                    onTrackEvent(ETrackEvent.OnLimitedToNone);
                                    onTrackEvent(ETrackEvent.OnToNone);
                                    onTrackEvent(ETrackEvent.OnNone);
                                    break;
                                }
                            case TrackingState.Limited:
                                {
                                    onTrackEvent(ETrackEvent.OnLimitedAlways);
                                    onTrackEvent(ETrackEvent.OnLimited);
                                    break;
                                }
                            case TrackingState.Tracking:
                                {
                                    onTrackEvent(ETrackEvent.OnLimitedToTracking);
                                    onTrackEvent(ETrackEvent.OnToTracking);
                                    onTrackEvent(ETrackEvent.OnTracking);
                                    break;
                                }
                        }
                        break;
                    }
                case TrackingState.Tracking:
                    {
                        switch (trackingState)//当前状态
                        {
                            case TrackingState.None:
                                {
                                    onTrackEvent(ETrackEvent.OnTrackingToNone);
                                    onTrackEvent(ETrackEvent.OnToNone);
                                    onTrackEvent(ETrackEvent.OnNone);
                                    break;
                                }
                            case TrackingState.Limited:
                                {
                                    onTrackEvent(ETrackEvent.OnTrackingToLimited);
                                    onTrackEvent(ETrackEvent.OnToLimited);
                                    onTrackEvent(ETrackEvent.OnLimited);
                                    break;
                                }
                            case TrackingState.Tracking:
                                {
                                    onTrackEvent(ETrackEvent.OnTrackingAlways);
                                    onTrackEvent(ETrackEvent.OnTracking);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

#endif
    }
}
