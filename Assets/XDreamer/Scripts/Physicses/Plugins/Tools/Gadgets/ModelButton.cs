using System;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 模型按钮：通过物理刚体运动来模拟按钮按下弹起。按钮运动方向为其Y轴负方向
    /// </summary>
    [Name("模型按钮")]
    [XCSJ.Attributes.Icon(EIcon.Button)]
    [Tool(PhysicsManager.Title, nameof(InteractableVirtual), rootType = typeof(PhysicsManager))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireManager(typeof(PhysicsManager))]
    [Owner(typeof(PhysicsManager))]
    public class ModelButton : Grabbable
    {
        /// <summary>
        /// 向下偏移长度
        /// </summary>
        [Name("向下偏移长度")]
        [Min(0)]
        public float _downOffsetLength = 1;

        /// <summary>
        /// 按下触发阈值
        /// </summary>
        [Name("按下触发阈值")]
        [Tip("从最低点到当前值的区域内为按下状态；本值必须小于【向下偏移长度】", "The area from the lowest point to the current value is in the pressed state; This value must be less than [downward offset length]")]
        [Min(0)]
        public float _threshHold = 0.8f;

        /// <summary>
        /// 向上弹力
        /// </summary>
        [Name("向上弹力")]
        public float _upForceOfSpring = 10;

        /// <summary>
        /// 忽略碰撞其他碰撞体
        /// </summary>
        [Name("忽略碰撞其他碰撞体")]
        public Collider[] _collidersToIgnore;

        [Name("按下音频剪辑")]
        public AudioClip _pressedSound;

        [Name("按下事件")]
        public UnityEvent _onPressedDown;

        [Name("弹起音频剪辑")]
        public AudioClip _releasedSound;

        [Name("弹起事件")]
        public UnityEvent _onPressedUp;

        private float upperLowerDiff;

        /// <summary>
        /// 按下态
        /// </summary>
        public bool isPressed
        {
            get => _isPressed;
            private set
            {
                if (_isPressed == value) return;

                _isPressed = value;

                if (isPressed)
                {
                    _onPressedDown?.Invoke();
                    if (_pressedSound)
                    {
                        AudioSource.PlayClipAtPoint(_pressedSound, transform.position);
                    }
                }
                else
                {
                    _onPressedUp?.Invoke();
                    if (_releasedSound)
                    {
                        AudioSource.PlayClipAtPoint(_releasedSound, transform.position);
                    }
                }
                onPressed?.Invoke(this, value);
            }
        }
        public bool _isPressed;

        /// <summary>
        /// 物理按钮事件
        /// </summary>
        public static event Action<ModelButton, bool> onPressed;

        public override bool isConnectToOther => true;

        private void OnValidate()
        {
            _threshHold = Mathf.Min(_threshHold, _downOffsetLength);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _collidersToIgnore = GetComponentsInParent<Collider>();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            locationYOnEnable = transform.localPosition.y;

            ownRigidbody.useGravity = false;
            rigidbodyConstraintsOnEnable = ownRigidbody.constraints;
            ownRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            ownColliders = GetComponentsInChildren<Collider>();
            PhysicsHelper.IgnoreCollision(ownColliders, _collidersToIgnore, true);
        }

        private float locationYOnEnable = 0;
        private RigidbodyConstraints rigidbodyConstraintsOnEnable;
        private Collider[] ownColliders = new Collider[0];

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            ownRigidbody.constraints = rigidbodyConstraintsOnEnable;
            PhysicsHelper.IgnoreCollision(ownColliders, _collidersToIgnore, false);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // 判断本地坐标Y值往下时，施加向上的力
            if (transform.localPosition.y < locationYOnEnable)
            {
                ownRigidbody.AddForce(transform.up * _upForceOfSpring * Time.deltaTime);
            }

            // 使用按钮的上朝向向下偏移的量
            var localPosition = transform.localPosition;
            localPosition.y = Mathf.Clamp(localPosition.y, locationYOnEnable - _downOffsetLength, locationYOnEnable);
            transform.localPosition = localPosition;

            isPressed = (localPosition.y - locationYOnEnable) < -_threshHold;
        }
    }
}
