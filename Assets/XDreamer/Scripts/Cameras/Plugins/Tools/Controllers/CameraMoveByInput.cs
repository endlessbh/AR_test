using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动通过输入:默认通过输入鼠标XY与滚轮的偏移量控制相机的移动
    /// </summary>
    [Name("相机移动通过输入")]
    [Tip("默认通过输入鼠标XY与滚轮的偏移量控制相机的移动", "By default, the movement of the camera is controlled by entering the offset between the mouse xy and the wheel")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController), */nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveByInput : BaseCameraMoveController
    {
        #region 输入

        /// <summary>
        /// X输入轴:输入轴的值对应X轴移动量；类似左右移动；
        /// </summary>
        [Name("X输入轴")]
        [Tip("输入轴的值对应X轴移动量；类似左右移动；", "The value of the input axis corresponds to the movement of the X axis; Similar to left-right movement;")]
        public InputAxis _xInputAxis = new InputAxis();

        /// <summary>
        /// Y输入轴:输入轴的值对应Y轴移动量；类似上下移动；
        /// </summary>
        [Name("Y输入轴")]
        [Tip("输入轴的值对应Y轴移动量；类似上下移动；", "The value of the input axis corresponds to the movement of the Y axis; Similar to moving up and down;")]
        public InputAxis _yInputAxis = new InputAxis();

        /// <summary>
        /// Z输入轴:输入轴的值对应Z轴移动量；类似前后移动；
        /// </summary>
        [Name("Z输入轴")]
        [Tip("输入轴的值对应Z轴移动量；类似前后移动；", "The value of the input axis corresponds to the movement of the Z axis; Move back and forth similarly;")]
        public InputAxis _zInputAxis = new InputAxis();

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
                base.Update();
                var speedRealtime = this.speedRealtime;

                var hasInput = false;
                if (_xInputAxis.CanInput(input))
                {
                    _offset.x += input.GetAxis(_xInputAxis._inputName) * speedRealtime.x;
                    hasInput = true;
                }

                if (_yInputAxis.CanInput(input))
                {
                    _offset.y += input.GetAxis(_yInputAxis._inputName) * speedRealtime.y;
                    hasInput = true;
                }

                if (_zInputAxis.CanInput(input))
                {
                    _offset.z += input.GetAxis(_zInputAxis._inputName) * speedRealtime.z;
                    hasInput = true;
                }

                if(hasInput)
                {
                    Move();
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedInfo.Reset(new Vector3(-15, -15, 20));

            _xInputAxis._inputName = "Mouse X";
            _xInputAxis._mouseButtons.Add(EMouseButton.Middle);

            _yInputAxis._inputName = "Mouse Y";
            _yInputAxis._mouseButtons.Add(EMouseButton.Middle);

            _zInputAxis._inputName = "Mouse ScrollWheel";
            _zInputAxis._mouseButtons.Add(EMouseButton.Always);

            _inputHandler._inputWhenOnAnyUI = EInput.CameraInput;
            _inputHandler._inputWhenHasTouch = EInput.CameraInput;
            _inputHandler._input = EInput.CameraInput | EInput.StandaloneInput;
        }
    }
}
