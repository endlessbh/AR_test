using XCSJ.PluginCommonUtils;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 手指姿势数据
    /// </summary>
    [Serializable]
    public class FingerPoseData
    {
        /// <summary>
        /// 手指关节姿态数据
        /// </summary>
        public Pose[] jointPoses = new Pose[0];

        /// <summary>
        /// 姿态数据数量
        /// </summary>
        public int dataCount => jointPoses.Length;

        /// <summary>
        /// 手指姿势数据构造函数
        /// </summary>
        public FingerPoseData() { }

        /// <summary>
        /// 手指姿势数据构造函数
        /// </summary>
        /// <param name="finger"></param>
        public FingerPoseData(Finger finger)
        {
            FingerToData(finger);
        }

        /// <summary>
        /// 使用姿态数据设置手指
        /// </summary>
        /// <param name="finger"></param>
        /// <returns></returns>
        public bool DataToFinger(Finger finger)
        {
            if (!finger) return false;

            var count = dataCount;
            if (count != finger.jointCount)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                var t = finger.joints[i];
                var p = jointPoses[i];
                t.XSetLocalPosition(p.position);
                t.XSetLocalRotation(p.rotation);
            }
            return true;
        }

        /// <summary>
        /// 读取手指姿态数据
        /// </summary>
        /// <param name="finger"></param>
        public void FingerToData(Finger finger)
        {
            if (!finger) return;

            int count = finger.jointCount;
            jointPoses = new Pose[count];
            for (int i = 0; i < count; i++)
            {
                var t = finger.joints[i];
                jointPoses[i] = new Pose(t.localPosition, t.localRotation);
            }
        }
    }
}
