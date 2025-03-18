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
    /// 角色移动速度通过输入
    /// </summary>
    [Name("角色移动速度通过输入")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName_Move, nameof(CharacterTransformer))]
    public class CharacterMoveSpeedByInput : BaseCharacterToolController
    {
        /// <summary>
        /// 速度切换输入轴:对应角色的移动速度变化控制
        /// </summary>
        [Name("速度切换输入轴")]
        [Tip("对应角色的移动速度变化控制", "Movement speed change control of corresponding character")]
        public InputAxis _speedSwitchInput = new InputAxis();

        /// <summary>
        /// 输入处理
        /// </summary>
        [Name("输入处理")]
        public InputHandler _inputHandler = new InputHandler();

        /// <summary>
        /// 覆盖速度
        /// </summary>
        [Name("覆盖速度")]
        [Group("速度配置")]
        public bool _overrideSpeed = true;

        /// <summary>
        /// 行走速度
        /// </summary>
        [Name("行走速度")]
        [HideInSuperInspector(nameof(_overrideSpeed), EValidityCheckType.Equal, false)]
        public float _walkSpeed = 2.5f;

        /// <summary>
        /// 跑步速度
        /// </summary>
        [Name("跑步速度")]
        [HideInSuperInspector(nameof(_overrideSpeed), EValidityCheckType.Equal, false)]
        public float _runSpeed = 5f;

        /// <summary>
        /// 保持输入
        /// </summary>
        [Name("保持输入")]
        [Tip("为True时,角色移动速度在【速度切换输入轴】保持按下态为低速,在非按下态为高速；为False时,角色移动速度【速度切换输入轴】按下弹起后在低速与高速之间切换", "When it is true, the character's moving speed remains low in the pressed state in [speed switching input axis], and high in the non pressed state; When it is false, the character's movement speed [speed switching input axis] will switch between low speed and high speed after pressing and bouncing up")]
        public bool _keepInput = false;

        private float orgSpeed = 0;
        private float orgMaxSpeed = 0;

        private float minSpeed => _overrideSpeed ? _walkSpeed : orgSpeed;
        private float maxSpeed => mainController.maxSpeed;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            orgSpeed = mainController.speed;
            orgMaxSpeed = mainController.maxSpeed;

            _walkSpeed = Mathf.Max(_walkSpeed, 0);
            _runSpeed = Mathf.Max(_walkSpeed, _runSpeed);

            if (_overrideSpeed)
            {
                // 初始设置为跑
                mainController.speed = _runSpeed;
                mainController.maxSpeed = _runSpeed;
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            mainController.speed = orgSpeed;
            mainController.maxSpeed = orgMaxSpeed;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (_inputHandler.GetInput() is IInput input)
            {
                if (_speedSwitchInput.CanInput(input))
                {
                    if (_keepInput)
                    {
                        mainController.speed = input.GetButton(_speedSwitchInput._inputName) ? minSpeed : maxSpeed;
                    }
                    else if (input.GetButtonUp(_speedSwitchInput._inputName))
                    {
                        mainController.speed = mainController.speed < maxSpeed ? maxSpeed : minSpeed;
                    }
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedSwitchInput._inputName = "Fire3";
            _speedSwitchInput._mouseButtons.Add(EMouseButton.Always);

            _inputHandler._inputWhenOnAnyUI = EInput.CharacterInput;
            _inputHandler._inputWhenHasTouch = EInput.CharacterInput;
            _inputHandler._input = EInput.CharacterInput | EInput.StandaloneInput;
        }
    }
}