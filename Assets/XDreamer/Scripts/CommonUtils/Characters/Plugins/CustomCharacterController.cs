using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// 
    /// Example Character Controller
    /// 
    /// This example shows how to extend the 'BaseCharacterController' adding support for different
    /// character speeds (eg: walking, running, etc), plus how to handle custom input extending the
    /// HandleInput method and make the movement relative to camera view direction.
    /// 
    /// </summary>
    [Name("自定义角色控制器")]
    public sealed class CustomCharacterController : BaseCharacterController
    {
        #region 编辑器公开字段

        [Group("自定义控制器")]
        [Name("玩家相机")]
        [Tip("角色的跟随相机", "Character's follow camera")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform playerCamera;

        [Name("玩家相机非激活时禁用移动")]
        public bool _disableMoveWhenPlayerCameraInActive = true;

        [Name("行走速度")]
        [Tip("角色的行走速度", "Character's walking speed")]
        [SerializeField]
        private float _walkSpeed = 2.5f;

        [Name("跑步速度")]
        [Tip("角色的跑步速度", "Character's running speed")]
        [SerializeField]
        private float _runSpeed = 5.0f;

        [Name("行走输入")]
        [Tip("对应输入按钮按下保持时,将使用行走速度,；否则使用跑步速度；", "When the corresponding input button is pressed and held, the travel speed will be used,; Otherwise, use running speed;")]
        [Input]
        public string walkInput = "Fire3";

        #endregion

        #region 属性

        /// <summary>
        /// The character's walk speed.
        /// </summary>
        public float walkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The character's run speed.
        /// </summary>
        public float runSpeed
        {
            get { return _runSpeed; }
            set { _runSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Walk input command.
        /// </summary>
        public bool walk { get; private set; }

        #endregion

        #region 方法

        /// <summary>
        /// Get target speed based on character state (eg: running, walking, etc).
        /// </summary>
        private float GetTargetSpeed()
        {
            return walk ? walkSpeed : runSpeed;
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' CalcDesiredVelocity method to handle different speeds,
        /// eg: running, walking, etc.
        /// </summary>
        protected override Vector3 CalcDesiredVelocity()
        {
            // Set 'BaseCharacterController' speed property based on this character state
            speed = GetTargetSpeed();

            // Return desired velocity vector
            return base.CalcDesiredVelocity();
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' Animate method.
        /// 
        /// This shows how to handle your characters' animation states using the Animate method.
        /// The use of this method is optional, for example you can use a separate script to manage your
        /// animations completely separate of movement controller.
        /// 
        /// </summary>
        protected override void Animate()
        {
            if (animator == null) return;

            // 计算局部空间中的运动矢量
            var move = transform.InverseTransformDirection(moveDirection);

            // 更新animator参数
            var forwardAmount = animator.applyRootMotion
                ? move.z
                : Mathf.InverseLerp(0.0f, runSpeed, movement.forwardSpeed);

            animator.SetFloat(nameof(ECustomParameter.Forward), forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(nameof(ECustomParameter.Turn), Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool(nameof(ECustomParameter.OnGround), movement.isGrounded);

            if (!movement.isGrounded)
                animator.SetFloat(nameof(ECustomParameter.Jump), movement.velocity.y, 0.1f, Time.deltaTime);

            // 计算哪条腿在后面，以便在跳跃动画中保持该条腿在后面
            // 此代码依赖于动画中的特定运行周期偏移，假设一条腿在0.0和0.5的标准化剪裁时间内通过另一条腿
            var runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
            var jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;

            if (movement.isGrounded)
                animator.SetFloat(nameof(ECustomParameter.JumpLeg), jumpLeg);
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput,
        /// to perform custom controller input.
        /// </summary>
        protected override void HandleInput()
        {
            if (!enableInput) return;

            // Handle your custom input here...

            // 玩家相机非激活时禁用移动
            if (_disableMoveWhenPlayerCameraInActive && playerCamera && !playerCamera.gameObject.activeInHierarchy)
            {
                return;
            }

            moveDirection = new Vector3
            {
                x = XInput.GetAxisRaw(leftRightInput),
                y = 0.0f,
                z = XInput.GetAxisRaw(forwardBackInput)
            };

            walk = XInput.GetButton(walkInput);

            jump = XInput.GetButton(jumpInput);


            // Transform moveDirection vector to be relative to camera view direction
            if (playerCamera)
            {
                moveDirection = moveDirection.RelativeTo(playerCamera);
            }
        }

        #endregion

        #region MB方法

        /// <summary>
        /// Overrides 'BaseCharacterController' OnValidate method,
        /// to perform this class editor exposed fields validation.
        /// </summary>
        public override void OnValidate()
        {
            // Validate 'BaseCharacterController' editor exposed fields

            base.OnValidate();

            // Validate this editor exposed fields

            walkSpeed = _walkSpeed;
            runSpeed = _runSpeed;
        }

        #endregion
    }
}
