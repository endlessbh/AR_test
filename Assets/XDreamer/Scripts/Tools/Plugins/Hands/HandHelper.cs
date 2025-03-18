using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 手助手
    /// </summary>
    public static class HandHelper
    {
        /// <summary>
        /// 混合两组手指姿态数据，并使用结果姿态数据设置手指
        /// </summary>
        /// <param name="sourceData">源手指姿态数据</param>
        /// <param name="targetData">目标手指姿态数据</param>
        /// <param name="blend01">混合值</param>
        /// <param name="finger">手指</param>
        public static void BlendPoseDataToFinger(this FingerPoseData sourceData, FingerPoseData targetData, float blend01, Finger finger)
        {
            if (sourceData == null || targetData == null || !finger) return;

            // 保证姿态数据数量一致
            var count = sourceData.dataCount;
            if (count != targetData.dataCount || count != finger.jointCount)
            {
                return;
            }

            // 线性插值混合
            blend01 = Mathf.Clamp01(blend01);
            for (int i = 0; i < count; i++)
            {
                var t = finger.joints[i];
                var p1 = sourceData.jointPoses[i];
                var p2 = targetData.jointPoses[i];
                t.XSetLocalPosition(Vector3.Lerp(p1.position, p2.position, blend01));
                t.XSetLocalRotation(Quaternion.Lerp(p1.rotation, p2.rotation, blend01));
            }
        }
    }
}
