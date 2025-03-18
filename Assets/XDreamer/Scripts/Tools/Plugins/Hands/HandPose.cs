using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Hands
{
    /// <summary>
    /// 手姿态：包含手掌和每根手指的姿态
    /// </summary>
    [Name("手姿态")]
    public class HandPose : InteractProvider
    {
        /// <summary>
        /// 手姿态数据列表：可用于混合
        /// </summary>
        [Name("手姿态数据列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)] 
        public List<HandPoseData> _handPoseDatas = new List<HandPoseData>();

        /// <summary>
        /// 手姿态数量
        /// </summary>
        public int handPoseDataCount => _handPoseDatas.Count;

        /// <summary>
        /// 获取手姿态数据
        /// </summary>
        /// <returns></returns>
        public HandPoseData GetHandPose()
        {
            if (handPoseDataCount > 0)
            {
                return _handPoseDatas[0];
            }
            return null;
        }

        /// <summary>
        /// 设置被抓对象基于手的位置计算出来的位置
        /// </summary>
        /// <param name="grabbable"></param>
        /// <param name="hand"></param>
        public bool TryGetOffsetToHand(Hand hand, out Vector3 positionOffset, out Quaternion rotationOffset)
        {
            var data = GetHandPose();
            if (data != null)
            {
                positionOffset = -data._grabbableToHandPositionOffset;
                rotationOffset = Quaternion.Inverse(data._grabbableToHandRotationOffset);
                return true;
            }
            positionOffset = default;
            rotationOffset = default;
            return false;
        }

        /// <summary>
        /// 使用当前手姿态数据设置到手
        /// </summary>
        /// <param name="hand"></param>
        public void SetPose(Hand hand)
        {
            var data = GetHandPose();
            if (data != null)
            {
                data.DataToHand(hand);
            }
        }
    }

    /// <summary>
    /// 手姿态数据
    /// </summary>
    [Serializable]
    public class HandPoseData
    {
        /// <summary>
        /// 可抓物品与手的位置偏移
        /// </summary>
        [Name("可抓物品与手的位置偏移")]
        public Vector3 _grabbableToHandPositionOffset = Vector3.zero;

        /// <summary>
        /// 可抓物品与手的角度偏移
        /// </summary>
        [Name("可抓物品与手的角度偏移")]
        public Quaternion _grabbableToHandRotationOffset = Quaternion.identity;

        /// <summary>
        /// 手指姿势数据
        /// </summary>
        [Name("手指姿势数据")]
        public List<FingerPoseData> fingerPoseDatas = new List<FingerPoseData>();

        /// <summary>
        /// 手指数量
        /// </summary>
        public int fingerCount => fingerPoseDatas.Count;

        /// <summary>
        /// 构造函数
        /// </summary>
        public HandPoseData()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="hand"></param>
        public HandPoseData(Transform host, Hand hand)
        {
            HandToData(host, hand);
        }

        /// <summary>
        /// 读取手姿态并存储数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="hand"></param>
        public void HandToData(Transform grabbable, Hand hand)
        {
            if (!grabbable || !hand) return;

            _grabbableToHandPositionOffset = hand.transform.position - grabbable.transform.position;
            _grabbableToHandRotationOffset = hand.transform.rotation * Quaternion.Inverse(grabbable.rotation);
            //_grabbableRotationOffset = Quaternion.Slerp(hand.transform.rotation, handPose.transform.rotation, 1);

            fingerPoseDatas.Clear();

            foreach (var finger in hand._fingers)
            {
                if (finger)
                {
                    fingerPoseDatas.Add(new FingerPoseData(finger));
                }
            }
        }

        /// <summary>
        /// 使用数据设置手姿态
        /// </summary>
        /// <param name="grabbable"></param>
        /// <param name="hand"></param>
        public void DataToHand(Hand hand)
        {
            if (!hand) return;

            var count = fingerCount;
            if (count != hand.fingerCount) return;

            for (int i = 0; i < count; i++)
            {
                fingerPoseDatas[i].DataToFinger(hand._fingers[i]);
            }
        }
    }
}
