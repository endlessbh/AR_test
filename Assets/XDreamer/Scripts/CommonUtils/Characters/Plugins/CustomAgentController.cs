﻿using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.PluginCharacters
{
    /// <summary>
    /// 
    /// Example Agent Controller
    /// 
    /// This example shows how to extend the 'BaseAgentController' adding support for different
    /// character speeds (eg: walking, running, etc), plus how to handle custom input extending the
    /// HandleInput method.
    /// 
    /// </summary>
    [Name("自定义代理控制器")]
    public sealed class CustomAgentController : BaseAgentController
    {
        #region 编辑器公开字段

        [Group("自定义控制器")]
        [Name("行走速度")]
        [Tip("角色的行走速度", "Character's walking speed")]
        [SerializeField]
        private float _walkSpeed = 2.0f;

        [Name("跑步速度")]
        [Tip("角色的跑步速度", "Character's running speed")]
        [SerializeField]
        private float _runSpeed = 5.0f;

        [Name("行走输入")]
        [Tip("对应输入按钮按下保持时,将使用行走速度,；否则使用跑步速度；", "When the corresponding input button is pressed and held, the travel speed will be used,; Otherwise, use running speed;")]
        [Input]
        public string walkInput = "Fire3";

        #endregion

        #region PROPERTIES

        /// <summary>
        /// The character's walk speed.
        /// </summary>

        public float walkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = value; }
        }

        /// <summary>
        /// The character's run speed.
        /// </summary>

        public float runSpeed
        {
            get { return _runSpeed; }
            set { _runSpeed = value; }
        }

        /// <summary>
        /// Walk input command.
        /// </summary>

        public bool walk { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Get target speed based on character state (eg: running, walking, etc).
        /// </summary>

        private float GetTargetSpeed()
        {
            return walk ? walkSpeed : runSpeed;
        }

        /// <summary>
        /// Overrides 'BaseAgentController' CalcDesiredVelocity method to handle different speeds,
        /// eg: running, walking, etc.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            // Modify base controller speed based on this character state

            speed = GetTargetSpeed();

            // Call the parent class' version of method

            return base.CalcDesiredVelocity();
        }

        /// <summary>
        /// Overrides 'BaseAgentController' Animate method.
        /// 
        /// This shows how to handle your characters' animation states using the Animate method.
        /// The use of this method is optional, for example you can use a separate script to manage your
        /// animations completely separate of movement controller.
        /// </summary>

        protected override void Animate()
        {
            // If no animator, return

            if (animator == null)
                return;

            // Compute move vector in local space

            var move = transform.InverseTransformDirection(moveDirection);

            // Update the animator parameters

            var forwardAmount = animator.applyRootMotion
                ? move.z * brakingRatio
                : Mathf.InverseLerp(0.0f, runSpeed, movement.forwardSpeed);

            animator.SetFloat(nameof(ECustomParameter.Forward), forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat(nameof(ECustomParameter.Turn), Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool(nameof(ECustomParameter.OnGround), movement.isGrounded);

            if (!movement.isGrounded)
                animator.SetFloat(nameof(ECustomParameter.Jump), movement.velocity.y, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// Overrides 'BaseAgentController' HandleInput,
        /// extending it to add walk input.
        /// </summary>

        protected override void HandleInput()
        {
            if (!enableInput) return;

            // Call the parent class' version of method,
            // the one performs click-to-move

            base.HandleInput();

            // Adds walk input
            walk = XInput.GetButton(walkInput);
        }

        #endregion
    }
}
