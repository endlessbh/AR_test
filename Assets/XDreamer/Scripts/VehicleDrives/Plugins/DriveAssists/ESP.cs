using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 车身电子稳定系统:是对旨在提升车辆的操控表现的同时、有效地防止汽车达到其动态极限时失控的系统或程序的通称。电子稳定程序能提升车辆的安全性和操控性。
    /// </summary>
    [DisallowMultipleComponent]
    [Name("车身电子稳定系统")]
    [Tip("是对旨在提升车辆的操控表现的同时、有效地防止汽车达到其动态极限时失控的系统或程序的通称。电子稳定程序能提升车辆的安全性和操控性。")]
    [XCSJ.Attributes.Icon(EIcon.VehicleStability)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class ESP : VehicleDriverGetter
    {
        /// <summary>
        /// 左前轮
        /// </summary>
        [Name("左前轮")]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _frontLeftWheel;

        /// <summary>
        /// 右前轮
        /// </summary>
        [Name("右前轮")]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _frontRightWheel;

        /// <summary>
        /// 左后轮
        /// </summary>
        [Name("左后轮")]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _rearLeftWheel;

        /// <summary>
        /// 右后轮
        /// </summary>
        [Name("右后轮")]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _rearRightWheel;

        /// <summary>
        /// ESP阈值
        /// </summary>
        [Name("ESP阈值")]
        [Range(.05f, .5f)] 
		public float _threshold = .5f;

        /// <summary>
        /// ESP乘积
        /// </summary>
        [Name("ESP乘积")]
        [Range(.05f, 1f)]
        public float _multiplier = .25f;

        private Brake brake => vehicleDriver.brake as Brake;

        /// <summary>
        /// ESP工作
        /// </summary>
        public bool ESPEnable { get; private set; }

        private bool objValid = false;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            objValid = _frontLeftWheel && _frontRightWheel && _rearLeftWheel && _rearRightWheel;
        }

        /// <summary>
        ///所有车轮都有单独的制动器。
        ///当车辆失去控制时，相应的车轮制动，重新获得控制。
        /// </summary>
        protected override void OnFixedUpdate()
	    {
            if (!objValid || brake==null || brake.handbrakeOn) return;

            var frontSlip = _frontLeftWheel.wheelHit.sidewaysSlip + _frontRightWheel.wheelHit.sidewaysSlip;
            var rearSlip = _rearLeftWheel.wheelHit.sidewaysSlip + _rearRightWheel.wheelHit.sidewaysSlip;

            var underSteering = Mathf.Abs(frontSlip) >= _threshold;
            var overSteering = Mathf.Abs(rearSlip) >= _threshold;

            var bt = brake.torque * _multiplier;

            if (underSteering)
            {
                brake.SetBrakeTorque(_frontLeftWheel, bt * Mathf.Clamp(-rearSlip, 0f, Mathf.Infinity));
                brake.SetBrakeTorque(_frontRightWheel, bt * Mathf.Clamp(rearSlip, 0f, Mathf.Infinity));
            }

            if (overSteering)
            {
                brake.SetBrakeTorque(_rearLeftWheel, bt * Mathf.Clamp(-frontSlip, 0f, Mathf.Infinity));
                brake.SetBrakeTorque(_rearRightWheel, bt * Mathf.Clamp(frontSlip, 0f, Mathf.Infinity));
            }

            ESPEnable = (overSteering || underSteering) ? true : false;
            brake.brakeEnable = !ESPEnable;
        }
	}
}
