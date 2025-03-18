using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginSMS.States.Components
{
    /// <summary>
    /// 碰撞体触发器:碰撞体触发器组件是碰撞体与碰撞体之间发生碰撞的触发器。当碰撞发生时，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ColliderTrigger))]
    [Tip("碰撞体触发器组件是碰撞体与碰撞体之间发生碰撞的触发器。当碰撞发生时，组件切换为完成态。", "The collider trigger component is a trigger for collision between colliders. When the collision occurs, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33601)]
    public class ColliderTrigger : Trigger<ColliderTrigger>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "碰撞体触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.ComponentCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.ComponentCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ColliderTrigger))]
        [Tip("碰撞体触发器组件是碰撞体与碰撞体之间发生碰撞的触发器。当碰撞发生时，组件切换为完成态。", "The collider trigger component is a trigger for collision between colliders. When the collision occurs, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [ComponentPopup]
        [Name("碰撞体触发器")]
        [Tip("待碰撞的触发器对象;一般为静止不动的游戏对象;碰撞体触发器对应游戏对象最好具有刚体组件，且为触发器；", "Trigger object to be collided; It is generally a stationary game object; The collision trigger corresponding to the game object preferably has a rigid body component and is a trigger;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Collider colliderTrigger;

        [Name("触发类型")]
        public enum ETriggerType
        {
            [Name("进入")]
            Enter,

            [Name("保留")]
            Stay,

            [Name("退出")]
            Exit,
        }

        [Name("触发类型")]
        [EnumPopup]
        public ETriggerType triggerType = ETriggerType.Enter;

        [Name("当前主相机")]
        public bool currentMainCamera = true;

        [Name("游戏对象")]
        [Tip("碰撞体所在游戏对象；一般为可发生移动的游戏对象;", "The game object where the collision body is located; Generally, it is a movable game object;")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(currentMainCamera), EValidityCheckType.True)]
        public GameObject gameObject;

        [Name("自动添加碰撞体")]
        [Tip("当前主相机/游戏对象 没有碰撞体时动态添加碰撞体", "Add colliders dynamically when the current main camera / GameObject has no colliders")]
        public bool autoAddCollider = true;

        [Name("碰撞体类型")]
        public enum EColliderType
        {
            [Name("盒碰撞体")]
            BoxCollider = 0,

            [Name("球碰碰撞体")]
            SphereCollider,

            [Name("胶囊碰撞体")]
            CapsuleCollider,

            [Name("网格碰撞体")]
            MeshCollider,

            [Name("车轮碰撞体")]
            WheelCollider,

            [Name("地形碰撞体")]
            TerrainCollider,
        }

        [Name("碰撞体类型")]
        [HideInSuperInspector(nameof(autoAddCollider), EValidityCheckType.False)]
        [EnumPopup]
        public EColliderType colliderType = EColliderType.BoxCollider;

        /// <summary>
        /// 碰撞体触发器组件
        /// </summary>
        private XCSJ.PluginTools.Items.ColliderTrigger _colliderTrigger;

        /// <summary>
        /// 碰撞体游戏对象
        /// </summary>
        private GameObject colliderGO;

        /// <summary>
        /// 碰撞体
        /// </summary>
        [Name("碰撞体")]
        [Readonly]
        public Collider _collider;

        private bool colliderEnabled = true;

        public static Type GetType(EColliderType colliderType)
        {
            switch (colliderType)
            {
                case EColliderType.BoxCollider: return typeof(BoxCollider);
                case EColliderType.CapsuleCollider: return typeof(CapsuleCollider);
                case EColliderType.MeshCollider: return typeof(MeshCollider);
                case EColliderType.SphereCollider: return typeof(SphereCollider);
                case EColliderType.WheelCollider: return typeof(WheelCollider);
                case EColliderType.TerrainCollider: return typeof(TerrainCollider);
                default: return null;
            }
        }

        private GameObject GetGameObject()
        {
            if (!currentMainCamera) return gameObject;

            //使用当前主相机
            var camera = CameraHelperExtension.currentCamera;
            if (!camera) return null;

            //相机控制器
            var cameraController = camera.GetComponentInParent<BaseCameraMainController>();
            if (!cameraController) return camera.gameObject;

            //相机拥有者
            var cameraOwner = cameraController.cameraOwner;
            if (cameraOwner == null || !cameraOwner.ownerGameObject) return camera.gameObject;

            //相机拥有者游戏对象
            return cameraOwner.ownerGameObject;
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InteractObject.onInteractFinished += OnInteractFinish;

            colliderGO = GetGameObject();
            if (colliderGO)
            {
                AddTrigger();
            }
            else
            {
                _collider = null;
            }
        }

        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            var newGO = GetGameObject();
            if (newGO != colliderGO)
            {
                RemoveTrigger();

                colliderGO = newGO;

                AddTrigger();
            }
        }

        public override void OnExit(StateData data)
        {
            RemoveTrigger();

            base.OnExit(data);

            InteractObject.onInteractFinished -= OnInteractFinish;
        }

        private void AddTrigger()
        {
            if (autoAddCollider)
            {
                _collider = CommonFun.GetOrAddComponent<Collider>(colliderGO, GetType(colliderType));
            }
            else
            {
                _collider = colliderGO.GetComponent<Collider>();
            }
            if (_collider)
            {
                colliderEnabled = _collider.enabled;
                _collider.enabled = true;
            }

            _colliderTrigger = CommonFun.GetOrAddComponent<XCSJ.PluginTools.Items.ColliderTrigger>(colliderGO);
            if (_colliderTrigger)
            {
                _colliderTrigger.enabled = true;
            }
        }

        private void RemoveTrigger()
        {
            if (_colliderTrigger)
            {
                _colliderTrigger.enabled = false;
            }
            if (_collider)
            {
                _collider.enabled = colliderEnabled;
            }
        }

        private void OnInteractFinish(InteractObject interactObject, InteractData interactData)
        {
            if (interactObject is XCSJ.PluginTools.Items.ColliderTrigger ct && ct && ct == _colliderTrigger)
            {
                if (interactData is ColliderInteractData colliderInteractData && colliderInteractData.collider == colliderTrigger)
                {
                    var ecmd = (EColliderTriggerResultCmd)colliderInteractData.cmdEnum;
                    switch (triggerType)
                    {
                        case ETriggerType.Enter:
                            {
                                finished = (ecmd == EColliderTriggerResultCmd.OnTriggerEnter);
                                break;
                            }
                        case ETriggerType.Stay:
                            {
                                finished = (ecmd == EColliderTriggerResultCmd.OnTriggerStay);
                                break;
                            }
                        case ETriggerType.Exit:
                            {
                                finished = (ecmd == EColliderTriggerResultCmd.OnTriggerExit);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => colliderTrigger && (currentMainCamera || gameObject);

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => colliderTrigger ? colliderTrigger.name : "";
    }
}
