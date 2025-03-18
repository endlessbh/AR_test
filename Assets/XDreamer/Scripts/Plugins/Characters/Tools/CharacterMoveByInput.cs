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
    /// 角色移动通过输入
    /// </summary>
    [Name("角色移动通过输入")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName_Move, /*nameof(XCharacterController),*/ nameof(CharacterTransformer))]
    public class CharacterMoveByInput : BaseCharacterToolController
    {
        /// <summary>
        /// 移动模式
        /// </summary>
        [Name("移动模式")]
        [EnumPopup]
        public EMoveMode _moveMode = EMoveMode.Local;

        #region 输入

        /// <summary>
        /// 左右移动输入轴:对应角色的X轴变化控制
        /// </summary>
        [Name("左右移动输入轴")]
        [Tip("对应角色的X轴变化控制", "X-axis change control of corresponding character")]
        public InputAxis _leftRightInput = new InputAxis();

        /// <summary>
        /// 跳跃输入轴:对应角色的Y轴变化控制
        /// </summary>
        [Name("跳跃输入轴")]
        [Tip("对应角色的Y轴变化控制", "Y-axis change control of corresponding character")]
        public InputAxis _jumpInput = new InputAxis();

        /// <summary>
        /// 前进后退输入轴:对应角色的Z轴变化控制
        /// </summary>
        [Name("前进后退输入轴")]
        [Tip("对应角色的Z轴变化控制", "Z-axis change control of corresponding character")]
        public InputAxis _forwardBackInput = new InputAxis();

        #endregion

        #region 输入处理

        /// <summary>
        /// 输入处理
        /// </summary>
        [Name("输入处理")]
        public InputHandler _inputHandler = new InputHandler();

        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (_inputHandler.GetInput() is IInput input)
            {
                var moveDirection = Vector3.zero;

                if (_leftRightInput.CanInput(input))
                {
                    moveDirection.x = input.GetAxisRaw(_leftRightInput._inputName);
                }
                if (_jumpInput.CanInput(input))
                {
                    moveDirection.y = input.GetButton(_jumpInput._inputName) ? CharacterTransformer.JumpValue : 0;
                }
                if (_forwardBackInput.CanInput(input))
                {
                    moveDirection.z = input.GetAxisRaw(_forwardBackInput._inputName);
                }

                mainController.Move(moveDirection, (int)_moveMode);
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _leftRightInput._inputName = "Horizontal";
            _leftRightInput._mouseButtons.Add(EMouseButton.Always);

            _jumpInput._inputName = "Jump";
            _jumpInput._mouseButtons.Add(EMouseButton.Always);

            _forwardBackInput._inputName = "Vertical";
            _forwardBackInput._mouseButtons.Add(EMouseButton.Always);

            _inputHandler._inputWhenOnAnyUI = EInput.CharacterInput;
            _inputHandler._inputWhenHasTouch = EInput.CharacterInput;
            _inputHandler._input = EInput.CharacterInput | EInput.StandaloneInput;
        }
    }
}
