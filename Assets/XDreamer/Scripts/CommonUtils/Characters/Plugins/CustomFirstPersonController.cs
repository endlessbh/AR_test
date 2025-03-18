using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.CommonUtils.PluginCharacters
{
    [Name("自定义第一人称控制器")]
    public class CustomFirstPersonController: BaseFirstPersonController
    {
        /// <summary>
        /// Overrides 'BaseFirstPersonController' Animate method.
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
                ? move.z 
                : Mathf.InverseLerp(0.0f, speed, movement.forwardSpeed);

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
    }
}
