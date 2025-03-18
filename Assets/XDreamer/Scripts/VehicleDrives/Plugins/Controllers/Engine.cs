using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 发动机
    /// </summary>
    [Name("发动机")]
    [XCSJ.Attributes.Icon(EIcon.Engine)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class Engine : VehicleDriverGetter, IEngine
    {
        /// <summary>
        /// 动力倍数
        /// </summary>
        [Name("动力倍数")]
        [Min(1)]
        public float _ratio = 3.5f;

        /// <summary>
        /// RPM动能曲线
        /// </summary>
        [Name("RPM动能曲线")]
        [Tip("曲线横坐标是RPM，纵坐标是输出动能")]
        public AnimationCurve _torqueCurve = new AnimationCurve();

        /// <summary>
        /// RPM范围
        /// </summary>
        [Name("RPM范围")]
        [LimitRange(100, 30000)]
        public Vector2 _RPMRange = new Vector2(1000f, 7000f);

        /// <summary>
        /// 最小RPM
        /// </summary>
        public float minRPM => _RPMRange.x;

        /// <summary>
        /// 最大RPM
        /// </summary>
        public float maxRPM => _RPMRange.y;

        /// <summary>
        /// 每分钟转速
        /// </summary>
        public float RPM { get; protected set; }

        /// <summary>
        /// RPM变化速度
        /// </summary>
        [Name("RPM变化速度")]
        [Tip("值越低，发动机反应越快")]
        [Range(0, 1)]
        public float _engineInertia = .15f;

        /// <summary>
        /// TCS乘积
        /// </summary>
        [Name("TCS乘积")]
        [Range(.05f, 1f)]
        public float _TCSStrength = 1f;

        /// <summary>
        /// TCS工作状态
        /// </summary>
        public bool TCSWorking { get; private set; } = false;

        /// <summary>
        /// 引擎启动
        /// </summary>
        public bool running { get; set; }

        /// <summary>
        /// 重置
        /// </summary>
        protected override void Reset()
        {
            base.Reset();

            // 创建默认发动机动能曲线
            _torqueCurve = new AnimationCurve();
            _torqueCurve.AddKey(0f, 0f);
            _torqueCurve.AddKey(5000f, 250f);
            _torqueCurve.AddKey(_RPMRange.y, 250f / 1.5f);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if(vehicleDriver) vehicleDriver.engine = this;
        }

        /// <summary>
        /// 引擎工作：计算速度与RPM
        /// </summary>
        protected override void OnFixedUpdate()
        {
            if (!vehicleDriver || vehicleDriver.gearBox == null || vehicleDriver.fuel==null) return;

            var clutch = vehicleDriver.gearBox.clutchInput;
            var unClutch = 1 - clutch;
            var gearBoxRadio = vehicleDriver.gearBox.currentRadio;

            var targetRPM = Mathf.Lerp(running ? minRPM : 0, maxRPM, clutch * vehicleDriver.throttleValue)
                + vehicleDriver.GetPowerWheelRPM() * _ratio * gearBoxRadio * unClutch;

            float velocity = 0f;
            RPM = Mathf.SmoothDamp(RPM, targetRPM * vehicleDriver.fuel.GetPower(), ref velocity, _engineInertia);
            RPM = Mathf.Clamp(RPM, 0f, maxRPM);

            float motorTorque = 0;
            if (vehicleDriver.speed < vehicleDriver.maxSpeed)
            {
                motorTorque = vehicleDriver.direction * unClutch * vehicleDriver.throttleValue * vehicleDriver.boostValue * _torqueCurve.Evaluate(RPM) * gearBoxRadio * _ratio / vehicleDriver.powerWheelCount;
            }

            foreach (var wheelDriver in vehicleDriver.powerWheelDrivers)
            {
                wheelDriver.SetMotorTorque(OnTCS(wheelDriver, motorTorque));
            }
        }

        /// <summary>
        /// TCS
        /// </summary>
        private float OnTCS(WheelDriver wheelDriver, float torque)
        {
            TCSWorking = false;
            if (Mathf.Abs(wheelDriver.wheelCollider.rpm) >= 100 && wheelDriver.currentFrictions != null)
            {
                TCSWorking = wheelDriver.wheelHit.forwardSlip > wheelDriver.currentFrictions.slip;
                var t = torque * wheelDriver.wheelHit.forwardSlip * _TCSStrength;
                torque += TCSWorking ? -Mathf.Clamp(t, 0f, Mathf.Infinity) : Mathf.Clamp(t, -Mathf.Infinity, 0f);
            }
            return torque;
        }
    }
}
