using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 制动器
    /// </summary>
    [Name("制动器")]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class Brake : VehicleDriverGetter, IBrake
    {
        /// <summary>
        /// 刹车扭矩
        /// </summary>
        [Name("刹车扭矩")]
        [Tip("作用于车轮的扭矩,单位(牛顿.米)", "Torque acting on the wheel, in Newton meters")]
        [Min(1)]
        public float _torque = 2000f;

        /// <summary>
        /// 刹车扭矩
        /// </summary>
        public float torque { get => _torque; }

        /// <summary>
        /// ABS阈值
        /// </summary>
        [Name("ABS阈值")]
        [Range(.05f, .5f)]
        public float _ABSThreshold = .35f;

        /// <summary>
        /// ABS工作状态
        /// </summary>
        public bool ABSEnable { get; private set; } = false;

        /// <summary>
        /// 手刹
        /// </summary>
        public bool handbrakeOn 
        { 
            get => _handbrakeEnable; 
            set
            {
                _handbrakeEnable = value;

                VehicleDriver.SendVehicleState(vehicleDriver, new BrakeEventArgs(_handbrakeEnable));
            }
        } 
        private bool _handbrakeEnable = false;

        /// <summary>
        /// 脚刹启用禁用
        /// </summary>
        public bool brakeEnable { get; set; } = true;

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            vehicleDriver.brake = this;
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected override void OnFixedUpdate()
        {
            foreach (var wheelDriver in vehicleDriver.wheelDrivers)
            {
                if (handbrakeOn && wheelDriver._canHandbrake)
                {
                    wheelDriver.SetBrakeTorque(_torque * wheelDriver._handbrakeMultiplier);
                }
                else if (wheelDriver.canBrake && brakeEnable)
                {
                    SetBrakeTorque(wheelDriver, vehicleDriver.brakeValue * _torque * wheelDriver._brakeMultiplier);
                }
            }
        }

        /// <summary>
        /// 应用刹车扭矩
        /// </summary>
        public void SetBrakeTorque(WheelDriver wheelDriver, float torque)
        {
            // 检查前项滑动摩擦力，轮子失去牵引力，则不使用刹车
            ABSEnable = Mathf.Abs(wheelDriver.wheelHit.forwardSlip) * Mathf.Clamp01(torque) >= _ABSThreshold;

            if (!ABSEnable)
            {
                wheelDriver.SetBrakeTorque(torque);
            }
        }
    }
}
