using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Collisions
{
    /// <summary>
    /// 碰撞可交互对象
    /// </summary>
    [Name("碰撞可交互对象")]
    [RequireManager(typeof(PhysicsManager))]
    [Tool(PhysicsManager.Title, nameof(InteractableVirtual), rootType = typeof(PhysicsManager))]
    [Tool(InteractionCategory.InteractEvent, rootType = typeof(PhysicsManager), index = InteractionCategory.InteractableIndex + 3)]
    [Owner(typeof(PhysicsManager))]
    public class CollisionInteractable : ColliderTrigger
    {
        /// <summary>
        /// 碰撞进入相对最小速度值：碰撞进入发生时两个碰撞体之间的最小相对速度值大于当前值才产生交互
        /// </summary>
        [Group("碰撞进入设置", textEN = "Collision Enter Settings")]
        [Name("进入相对最小速度值")]
        [Tip("碰撞进入发生时两个碰撞体之间的最小相对速度值(标量)大于当前值才产生交互", "Interaction occurs when the minimum relative velocity value (scalar) between two collision bodies is greater than the current value when the collision enters")]
        [Min(0)]
        public float _relativeMinVelocityOnEnter = 10f;

        /// <summary>
        /// 上次碰撞进入相对速度值
        /// </summary>
        [Readonly]
        [Name("上次碰撞进入相对速度值")]
        public float _lastCollisionEnterRelativeVelocity = 0;

        /// <summary>
        /// 排除刚体抓取器碰撞
        /// </summary>
        [Name("排除刚体抓取器碰撞")]
        public bool _excludeRigidbodyGrabber = true;

        /// <summary>
        /// 排除碰撞对象列表
        /// </summary>
        [Name("排除碰撞对象列表")]
        public List<Collider> _excludeColliders = new List<Collider>();

        /// <summary>
        /// 调用碰撞完成
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="collision"></param>
        protected override void CallCollisionFinish(EColliderTriggerResultCmd cmd, Collision collision)
        {
            if (_excludeRigidbodyGrabber)
            {
                if (collision.transform.GetComponentInParent<RigidbodyGrabber>())
                {
                    return;
                }
            }
            if (_excludeColliders.Contains(collision.collider))
            {
                return;
            }

            _lastCollisionEnterRelativeVelocity = collision.relativeVelocity.magnitude;
            if (collision.relativeVelocity.magnitude >= _relativeMinVelocityOnEnter)
            {
                base.CallCollisionFinish(cmd, collision);
            }
        }
    }
}
