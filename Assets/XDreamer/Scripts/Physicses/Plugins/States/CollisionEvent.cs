using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginPhysicses.Tools.Collisions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 碰撞比较器
    /// </summary>
    [Serializable]
    public class ColliderTriggerComparer : InteractComparer<EColliderTriggerResultCmd, ColliderTrigger, CollisionInteractable> { }

    /// <summary>
    /// 碰撞体触发器事件：刚体与碰撞体发生碰撞产生事件
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(CollisionEvent))]
    [Tip("刚体与碰撞体发生碰撞产生事件", "The rigid body collides with the colliding body to generate events")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [DisallowMultipleComponent]
    [Owner(typeof(PhysicsManager))]
    public class CollisionEvent : Trigger<CollisionEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体触发器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [StateLib(InteractionCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
        [Name(Title, nameof(CollisionEvent))]
        [Tip("刚体与碰撞体发生碰撞产生事件", "Use to apply thrust or torque to objects with rigid bodies")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 碰撞比较器
        /// </summary>
        [Name("碰撞比较器")]
        public ColliderTriggerComparer _collisionComparer = new ColliderTriggerComparer();

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InteractObject.onInteractFinished += OnInteractFinished;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            InteractObject.onInteractFinished -= OnInteractFinished;
        }

        private void OnInteractFinished(InteractObject interactor, InteractData interactData)
        {
            if (!finished && interactData.cmdEnum is EColliderTriggerResultCmd collisionCmd)
            {
                if (_collisionComparer.IsMatch(interactData))
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _collisionComparer.DataValidity();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_collisionComparer._cmd);
    }
}