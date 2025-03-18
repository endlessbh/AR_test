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
    /// 相机旋转通过输入：默认通过输入鼠标XY的偏移量控制相机的旋转
    /// </summary>
    [Name("相机旋转通过输入")]
    [Tip("默认通过输入鼠标XY的偏移量控制相机的旋转", "By default, the rotation of the camera is controlled by entering the offset of the mouse XY")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Rotate, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    public class CameraRotateByInput : BaseCameraRotateController
    {
        #region 输入

        /// <summary>
        /// X输入轴:输入轴的值对应X轴旋转量；类似抬头低头的旋转；
        /// </summary>
        [Name("X输入轴")]
        [Tip("输入轴的值对应X轴旋转量；类似抬头低头的旋转；", "The value of the input axis corresponds to the rotation of the X axis; Similar to the rotation of raising the head and lowering the head;")]
        public InputAxis _xInputAxis = new InputAxis();

        /// <summary>
        /// Y输入轴:输入轴的值对应Y轴旋转量；类似左右转头的旋转；
        /// </summary>
        [Name("Y输入轴")]
        [Tip("输入轴的值对应Y轴旋转量；类似左右转头的旋转；", "The value of the input axis corresponds to the rotation of the Y axis; Similar to the rotation of left and right rotating head;")]
        public InputAxis _yInputAxis = new InputAxis();

        /// <summary>
        /// Z输入轴:输入轴的值对应Z轴旋转量；类似左右倾斜（顺时针或逆时针）的旋转；
        /// </summary>
        [Name("Z输入轴")]
        [Tip("输入轴的值对应Z轴旋转量；类似左右倾斜（顺时针或逆时针）的旋转；", "The value of the input axis corresponds to the rotation of the Z axis; Rotation similar to left-right tilt (clockwise or counterclockwise);")]
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

                if (hasInput)
                {
                    //Debug.Log("CameraRotateByInput y: " + input.GetAxis(_yInputAxis._inputName) + " ==> " + speedRealtime + " ==> " + Time.deltaTime);
                    //if (Mathf.Abs(_offset.y) > 1)
                    //{
                    //    Debug.Log("--------------------------------CameraRotateByInput: " + _offset);
                    //    Debug.Log("CameraRotateByInput input: " + input.name);
                    //}
                    Rotate();
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedInfo.Reset(new Vector3(-60, 60, 60));

            _xInputAxis._inputName = "Mouse Y";
            _xInputAxis._mouseButtons.Add(EMouseButton.Left);
            _xInputAxis._mouseButtons.Add(EMouseButton.Right);

            _yInputAxis._inputName = "Mouse X";
            _yInputAxis._mouseButtons.Add(EMouseButton.Left);
            _yInputAxis._mouseButtons.Add(EMouseButton.Right);//角色时需禁用本项

            _zInputAxis._mouseButtons.Add(EMouseButton.Always);

            _inputHandler._inputWhenOnAnyUI = EInput.CameraInput;
            _inputHandler._inputWhenHasTouch = EInput.CameraInput;
            _inputHandler._input = EInput.CameraInput | EInput.StandaloneInput;
        }
    }
}
