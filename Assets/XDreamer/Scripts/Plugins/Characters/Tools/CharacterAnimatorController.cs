using UnityEngine;
using XCSJ.Attributes;
using XCSJ.CommonUtils.PluginCharacters;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Characters.Base;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.Extension.Characters.Tools
{
    /// <summary>
    /// 角色动画器控制器
    /// </summary>
    [Name("角色动画器控制器")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName, nameof(XCharacterController))]
    public class CharacterAnimatorController : BaseCharacterToolController, IAnimatorParameter
    {
        /// <summary>
        /// 动画器
        /// </summary>
        [Name("动画器")]
        [ComponentPopup]
        public Animator _animator;

        /// <summary>
        /// 动画器
        /// </summary>
        public Animator animator
        {
            get
            {
                if (!_animator)
                {
                    if (mainController)
                    {
                        _animator = mainController.GetComponentInChildren<Animator>();
                    }
                    else
                    {
                        _animator = GetComponentInChildren<Animator>();
                    }
                }
                return _animator;
            }
        }

        /// <summary>
        /// 前进:用于控制角色的移动；类型：Float
        /// </summary>
        [Name("前进")]
        [Tip("用于控制角色的移动；类型：Float", "Used to control the movement of characters; Type: float")]
        [AnimatorParameterPopup(AnimatorControllerParameterType.Float)]
        public string _forward = "";

        /// <summary>
        /// 转向:用于控制角色的旋转；类型：Float
        /// </summary>
        [Name("转向")]
        [Tip("用于控制角色的旋转；类型：Float", "Used to control the rotation of characters; Type: float")]
        [AnimatorParameterPopup(AnimatorControllerParameterType.Float)]
        public string _turn = "";

        /// <summary>
        /// 下蹲:用于控制角色的下蹲；类型：Bool
        /// </summary>
        [Name("下蹲")]
        [Tip("用于控制角色的下蹲；类型：Bool", "Used to control the character's squat; Type: bool")]
        [AnimatorParameterPopup(AnimatorControllerParameterType.Bool)]
        public string _crouch = "";

        /// <summary>
        /// 地面上:控制角色是否在地面；类型：Bool
        /// </summary>
        [Name("地面上")]
        [Tip("控制角色是否在地面；类型：Bool", "Control whether the character is on the ground; Type: bool")]
        [AnimatorParameterPopup(AnimatorControllerParameterType.Bool)]
        public string _onGround = "";

        /// <summary>
        /// 跳跃:控制角色是否跳跃；类型：Float
        /// </summary>
        [Name("跳跃")]
        [Tip("控制角色是否跳跃；类型：Float", "Control whether the character jumps; Type: float")]
        [AnimatorParameterPopup(AnimatorControllerParameterType.Float)]
        public string _jump = "";

        /// <summary>
        /// 跳跃腿:控制角色的腿；类型：Float
        /// </summary>
        [Name("跳跃腿")]
        [Tip("控制角色的腿；类型：Float", "Control the character's legs; Type: float")]
        [AnimatorParameterPopup(AnimatorControllerParameterType.Float)]
        public string _jumpLeg = "";

        /// <summary>
        /// 移动方向
        /// </summary>
        public Vector3 moveDirection
        {
            get
            {
                if (characterAnimatorLocomotion != null)
                {
                    return characterAnimatorLocomotion.moveDirection;
                }
                return mainController.moveDirection;
            }
        }

        /// <summary>
        /// 前进速度
        /// </summary>
        public float forwardSpeed
        {
            get
            {
                if (characterAnimatorLocomotion != null)
                {
                    return characterAnimatorLocomotion.forwardSpeed;
                }
                return movement.forwardSpeed;
            }
        }

        /// <summary>
        /// 角色动画运动接口
        /// </summary>
        public ICharacterAnimatorLocomotion characterAnimatorLocomotion { get; private set; }

        /// <summary>
        /// 注册角色动画运动接口
        /// </summary>
        /// <param name="characterAnimatorLocomotion"></param>
        public void RegisteCharacterAnimatorLocomotion(ICharacterAnimatorLocomotion characterAnimatorLocomotion)
        {
            this.characterAnimatorLocomotion = characterAnimatorLocomotion;
        }

        /// <summary>
        /// 注销角色动画运动接口
        /// </summary>
        /// <param name="characterAnimatorLocomotion"></param>
        public void UnregisteCharacterAnimatorLocomotion(ICharacterAnimatorLocomotion characterAnimatorLocomotion)
        {
            if (this.characterAnimatorLocomotion == characterAnimatorLocomotion)
            {
                this.characterAnimatorLocomotion = null;
            }
        }

        /// <summary>
        /// 速度
        /// </summary>
        public float speed => mainController.speed;

        /// <summary>
        /// 最大速度
        /// </summary>
        public float maxSpeed => mainController.maxSpeed;

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            var animator = this.animator;
            if (!animator) return;

            var movement = this.movement;
            if (!movement) return;

            var deltaTime = Time.deltaTime;
            var isGrounded = movement.isGrounded;

            // 计算局部空间中的运动矢量
            var move = characterTransform.InverseTransformDirection(moveDirection);

            // 更新animator参数
            var forwardAmount = animator.applyRootMotion
                ? move.z
                : Mathf.InverseLerp(0.0f, maxSpeed, forwardSpeed);

            if (!string.IsNullOrEmpty(_forward)) animator.SetFloat(_forward, forwardAmount, 0.1f, deltaTime);
            if (!string.IsNullOrEmpty(_turn)) animator.SetFloat(_turn, Mathf.Atan2(move.x, move.z), 0.1f, deltaTime);

            if (!string.IsNullOrEmpty(_onGround)) animator.SetBool(_onGround, isGrounded);

            if (!isGrounded)//不在地面，跳跃状态中
            {
                if (!string.IsNullOrEmpty(_jump)) animator.SetFloat(_jump, movement.velocity.y, 0.1f, deltaTime);
            }
            else//在地面时
            {
                if (!string.IsNullOrEmpty(_jumpLeg))
                {
                    // 计算哪条腿在后面，以便在跳跃动画中保持该条腿在后面
                    // 此代码依赖于动画中的特定运行周期偏移，假设一条腿在0.0和0.5的标准化剪裁时间内通过另一条腿
                    var runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.2f, 1.0f);
                    var jumpLeg = (runCycle < 0.5f ? 1.0f : -1.0f) * forwardAmount;

                    animator.SetFloat(_jumpLeg, jumpLeg);
                }
            }
        }

        /// <summary>
        /// 获取动画器
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="animator"></param>
        /// <returns></returns>
        public bool TryGetAnimator(string propertyPath, out Animator animator)
        {
            animator = this.animator;
            return animator;
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _forward = nameof(ECustomParameter.Forward);
            _turn = nameof(ECustomParameter.Turn);
            _crouch = nameof(ECustomParameter.Crouch);
            _onGround = nameof(ECustomParameter.OnGround);
            _jump = nameof(ECustomParameter.Jump);
            _jumpLeg = nameof(ECustomParameter.JumpLeg);

            if (!animator) { }
        }
    }

    /// <summary>
    /// 角色动画运动接口
    /// </summary>
    public interface ICharacterAnimatorLocomotion
    {
        /// <summary>
        /// 移动方向
        /// </summary>
        Vector3 moveDirection { get; }

        /// <summary>
        /// 移动速度
        /// </summary>
        float forwardSpeed { get; }
    }
}
