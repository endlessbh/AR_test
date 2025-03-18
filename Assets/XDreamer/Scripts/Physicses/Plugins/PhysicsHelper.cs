using System;
using System.Collections.Generic;
using UnityEngine;

namespace XCSJ.PluginPhysicses
{
    /// <summary>
    /// 物理助手
    /// </summary>
    public static class PhysicsHelper
    {
        /// <summary>
        /// 用于纠正物理对象起始位置的误差。误差原因在于场景启动后物理引擎对游戏对象位置进行设定使其开始位置产生微小偏移。基于百分比
        /// </summary>
        public const float DeadZoneOfPercent = 0.025f;

        /// <summary>
        /// 设置两组碰撞器集合之间是否忽略碰撞
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <param name="ignore"></param>
        public static void IgnoreCollision(IEnumerable<Collider> sources, IEnumerable<Collider> targets, bool ignore)
        {
            if (sources == null || targets == null) return;

            foreach (var c1 in sources)
            {
                if (c1)
                {
                    foreach (var c2 in targets)
                    {
                        if (c2)
                        {
                            Physics.IgnoreCollision(c1, c2, ignore);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置刚体线速度和角速度为0
        /// </summary>
        /// <param name="rigidbody"></param>
        public static void SetVelocityZero(this Rigidbody rigidbody)
        {
            rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// 设置刚体的位置和旋转量
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void SetRigidbodyPose(this Rigidbody rigidbody, Vector3 position, Quaternion rotation)
        {
            rigidbody.rotation = rotation;
            rigidbody.position = position;
        }
    }
}
