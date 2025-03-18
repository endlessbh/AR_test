using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Characters.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.Extension.Characters.Tools
{
    /// <summary>
    /// 角色旋转通过输入
    /// </summary>
    [Name("角色旋转通过输入")]
    [XCSJ.Attributes.Icon(EIcon.WalkCamera)]
    [Tool(CharacterHelper.ControllersCategoryName_Rotate, /*nameof(XCharacterController),*/ nameof(CharacterTransformer))]
    public class CharacterRotateByInput : BaseCharacterRotateController
    {
        #region 输入

        //[Name("X输入轴")]
        //[Tip("输入轴的值对应X轴旋转量；类似抬头低头的旋转；")]
        //public InputAxis _xInputAxis = new InputAxis();

        /// <summary>
        /// Y输入轴:输入轴的值对应Y轴旋转量；类似左右转身的旋转；
        /// </summary>
        [Name("Y输入轴")]
        [Tip("输入轴的值对应Y轴旋转量；类似左右转身的旋转；", "The value of the input axis corresponds to the rotation of the Y axis; A rotation similar to turning left and right;")]
        public InputAxis _yInputAxis = new InputAxis();

        //[Name("Z输入轴")]
        //[Tip("输入轴的值对应Z轴旋转量；类似左右倾斜（顺时针或逆时针）的旋转；")]
        //public InputAxis _zInputAxis = new InputAxis();

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
        protected override void Update()
        {
            if (_inputHandler.GetInput() is IInput input)
            {
                //base.Update();
                //var speedRealtime = _speedInfo.valueRealtime;

                //if (_xInputAxis.CanInput()) offset.x += input.GetAxis(_xInputAxis._inputName) * speedRealtime.x;

                if (_yInputAxis.CanInput(input))
                {
                    base.Update();
                    //var speedRealtime = _speedInfo.valueRealtime;
                    _offset.y += input.GetAxis(_yInputAxis._inputName) * speedRealtime.y;
                    Rotate();

                    return;
                }

                //if (_zInputAxis.CanInput()) offset.z += input.GetAxis(_zInputAxis._inputName) * speedRealtime.z;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            //_xInputAxis._inputName = "Mouse Y";
            //_xInputAxis._mouseButtons.Add(EMouseButton.Right);

            _yInputAxis._inputName = "Mouse X";
            _yInputAxis._mouseButtons.Add(EMouseButton.Right);

            //_zInputAxis._mouseButtons.Add(EMouseButton.Always);

            _inputHandler._inputWhenOnAnyUI = EInput.CharacterInput;
            _inputHandler._inputWhenHasTouch = EInput.CharacterInput;
            _inputHandler._input = EInput.CharacterInput | EInput.StandaloneInput;
        }
    }
}
