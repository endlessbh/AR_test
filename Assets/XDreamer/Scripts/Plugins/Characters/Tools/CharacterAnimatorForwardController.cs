using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Characters.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.Extension.Characters.Tools
{
    /// <summary>
    /// 角色动画动作控制器
    /// </summary>
    [Name("角色动画朝向控制器")]
    [Tip("控制角色动画游戏对象的朝向和移动动画参数", "Controls the orientation and movement animation parameters of character animation game objects")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName, nameof(XCharacterController))]
    [RequireComponent(typeof(CharacterAnimatorController))]
    [DisallowMultipleComponent]
    public class CharacterAnimatorForwardController : BaseCharacterToolController, ICharacterAnimatorLocomotion
    {
        /// <summary>
        /// 移动方向
        /// </summary>
        public Vector3 moveDirection { get; private set; } = Vector3.zero;

        /// <summary>
        /// 前进速度
        /// </summary>
        public float forwardSpeed { get; private set; } = 0;

        private CharacterAnimatorController characterAnimatorController;

        /// <summary>
        /// 唤醒
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            characterAnimatorController = GetComponent<CharacterAnimatorController>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (characterAnimatorController)
            {
                characterAnimatorController.RegisteCharacterAnimatorLocomotion(this);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (characterAnimatorController)
            {
                characterAnimatorController.UnregisteCharacterAnimatorLocomotion(this);
            }
        }

        /// <summary>
        /// 延迟更新
        /// </summary>
        protected void FixedUpdate()
        {
            if (!mainController) return;

            var v = mainController.movement.velocity;
            var dir = new Vector3(v.x, 0, v.z);
            if (dir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }

            forwardSpeed = Mathf.Abs(Vector3.Dot(mainController.movement.velocity, transform.forward));
        }
    }
}
