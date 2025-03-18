using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    [Name("车辆控制器输入")]
    [XCSJ.Attributes.Icon(EIcon.Car)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    public class VehicleDriverInput : VehicleDriverGetter
    {
        /// <summary>
        /// 移动输入轴:控制车辆的前进或后退
        /// </summary>
        [Name("移动输入轴")]
        [Tip("控制车辆的前进或后退", "Control the forward or backward movement of the vehicle")]
        public InputAxis _moveInput = new InputAxis();

        /// <summary>
        /// 转向输入轴:控制车辆车轮转向
        /// </summary>
        [Name("转向输入轴")]
        [Tip("控制车辆车轮转向", "Control the forward or backward movement of the vehicle")]
        public InputAxis _rotateInput = new InputAxis();

        /// <summary>
        /// 输入处理
        /// </summary>
        [Name("输入处理")]
        public InputHandler _inputHandler = new InputHandler();

        /// <summary>
        /// 重置
        /// </summary>
        protected override void Reset()
        {
            base.Reset();

            _moveInput._inputName = "Vertical";
            _moveInput._mouseButtons.Add(EMouseButton.Always);

            _rotateInput._inputName = "Horizontal";
            _rotateInput._mouseButtons.Add(EMouseButton.Always);

            _inputHandler._inputWhenOnAnyUI = EInput.VehicleInput;
            _inputHandler._inputWhenHasTouch = EInput.VehicleInput;
            _inputHandler._input = EInput.StandaloneInput | EInput.VehicleInput;
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected override void OnFixedUpdate()
        {
            if (_inputHandler.GetInput() is IInput input)
            {
                if (_moveInput.CanInput(input))
                {
                    vehicleDriver.throttleInput = input.GetAxis(_moveInput._inputName);
                }
                if (_rotateInput.CanInput(input))
                {
                    vehicleDriver.steerInput = input.GetAxis(_rotateInput._inputName);
                }
            }
        }
    }
}
