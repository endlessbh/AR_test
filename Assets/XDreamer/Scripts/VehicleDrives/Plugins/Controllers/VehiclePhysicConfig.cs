using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 车辆物理属性配置
    /// </summary>

    [Name("车辆物理属性配置")]
    [XCSJ.Attributes.Icon(EIcon.Config)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    public class VehiclePhysicConfig : VehicleDriverGetter
    {
        /// <summary>
        /// 质量中心
        /// </summary>
        [Group("物理配置")]
        [Name("质量中心")]
        public Transform COM;

        /// <summary>
        /// 向下力：可防止侧翻
        /// </summary>
        [Name("向下力")]
        [Tip("可防止侧翻", "Prevent rollover")]
        public float downForce = 25f;

        /// <summary>
        /// 牵引力
        /// </summary>
        [Name("牵引力")]
        public bool tractionHelper = true;

        /// <summary>
        /// 牵引力乘积
        /// </summary>
        [Name("牵引力乘积")]
        [Range(0f, 1f)]
        public float tractionHelperStrength = .1f;

        /// <summary>
        /// 刚体最大角向量
        /// </summary>
        [Name("刚体最大角向量")]
        [Range(.5f, 20f)]
        public float _maxAngularVelocity = 6;

        /// <summary>
        /// 应用高速角阻力增大
        /// </summary>
        [Name("应用高速角阻力增大")]
        public bool angularDragHelper = false;

        /// <summary>
        /// 角阻力乘积
        /// </summary>
        [Name("角阻力乘积")]
        [Range(0f, 1f)]
        public float angularDragHelperStrength = .1f;

        private float _oldRotation;

        /// <summary>
        /// 角阻力乘积
        /// </summary>
        [Group("转向辅助")]
        [Name("转向辅助启用")]
        public bool steeringHelper = true;

        //[Name("转向速度乘积")]
        //[Range(0f, 1f)]
        //public float steerHelperLinearVelStrength = .1f;

        /// <summary>
        /// 转向角度乘积
        /// </summary>
        [Name("转向角度乘积")]
        [Range(0f, 1f)]
        public float steerHelperAngularVelStrength = .1f;

        /// <summary>
        /// 速度向量参考对象
        /// </summary>
        [Name("速度向量参考对象")]
        public GameObject velocityDirectionGO;

        /// <summary>
        /// 转向参考对象
        /// </summary>
        [Name("转向参考对象")]
        public GameObject steeringDirectionGO;

        /// <summary>
        /// 速度阈值
        /// </summary>
        [Group("车轮计算模式")]
        [Name("速度阈值")]
        [Tip("每次发生固定更新时，车辆仿真都会将此固定增量时间拆分为较小的子步骤，并计算每个较小增量的悬架力和轮胎力。然后，它将汇总所有产生的力和扭矩，对其进行积分，然后将其应用于车身。", "Each time a fixed update occurs, the vehicle simulation will split the fixed increment time into smaller sub steps, and calculate the suspension force and tire force for each smaller increment. Then, it will summarize all the generated forces and torques, integrate them, and then apply them to the body.")]
        public float _speedThreshold = 10f;

        /// <summary>
        /// 速度阈值以下
        /// </summary>
        [Name("速度阈值以下")]
        [Tip("车辆速度低于speedThreshold时，模拟子步骤的数量", "The number of simulation sub steps when the vehicle speed is lower than speedthreshold")]
        public int _stepsBelowThreshold = 5;

        /// <summary>
        /// 速度阈值以上
        /// </summary>
        [Name("速度阈值以上")]
        [Tip("车辆速度高于speedThreshold时，仿真子步骤的数量", "The number of simulation sub steps when the vehicle speed is higher than speedthreshold")]
        public int _stepsAboveThreshold = 5;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Start()
        {
            if (vehicleDriver)
            {
                // 设置刚体最大旋转角
                vehicleDriver.rigid.maxAngularVelocity = _maxAngularVelocity;

                foreach (var wc in vehicleDriver.wheelDrivers)
                {
                    wc.wheelCollider.ConfigureVehicleSubsteps(_speedThreshold, _stepsBelowThreshold, _stepsAboveThreshold);
                }
            }
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected override void OnFixedUpdate()
        {
            SteerHelper();

            if (tractionHelper)
            {
                var velocity = (vehicleDriver.rigid.velocity - transform.up * Vector3.Dot(vehicleDriver.rigid.velocity, transform.up)).normalized;
                var angle = -Mathf.Asin(Vector3.Dot(Vector3.Cross(transform.forward, velocity), transform.up));

                var tmp = 1f - Mathf.Clamp01(tractionHelperStrength * Mathf.Abs(vehicleDriver.rigid.angularVelocity.y));
                foreach (var wc in vehicleDriver.wheelDrivers)
                {
                    wc.SetSidewaysStiffness(angle, tmp);
                }
            }

            if (angularDragHelper)
            {
                vehicleDriver.rigid.angularDrag = Mathf.Lerp(0f, 10f, vehicleDriver.speed * angularDragHelperStrength / 1000f);
            }

            // 设置质量中心
            if (COM)
            {
                vehicleDriver.rigid.centerOfMass = transform.InverseTransformPoint(COM.transform.position);
            }
            vehicleDriver.rigid.AddRelativeForce(Vector3.down * vehicleDriver.speed * downForce, ForceMode.Force);
        }


        /// <summary>
        /// 转向助手
        /// </summary>
        private void SteerHelper()
        {
            if (!steeringHelper) return;

            if (!steeringDirectionGO)
            {
                steeringDirectionGO = new GameObject("Steering Direction");
                steeringDirectionGO.transform.SetParent(transform, false);
                steeringDirectionGO.transform.localPosition = new Vector3(1f, 2f, 0f);
            }

            if (!velocityDirectionGO)
            {
                velocityDirectionGO = new GameObject("Velocity Direction");
                velocityDirectionGO.transform.SetParent(transform, false);
                velocityDirectionGO.transform.localPosition = new Vector3(-1f, 2f, 0f);
            }

            if (vehicleDriver.wheelDrivers.Exists(wd => wd.wheelHit.normal== Vector3.zero))
            {
                return;
            }

            var v = vehicleDriver.rigid.angularVelocity;
            var vz = transform.InverseTransformDirection(vehicleDriver.rigid.velocity).z;
            var velocityAngle = v.y * Mathf.Clamp(vz, -1f, 1f) * Mathf.Rad2Deg;

            velocityDirectionGO.transform.localRotation = Quaternion.Lerp(velocityDirectionGO.transform.localRotation, Quaternion.AngleAxis(Mathf.Clamp(velocityAngle / 3f, -45f, 45f), Vector3.up), Time.fixedDeltaTime * 20f);

            steeringDirectionGO.transform.localRotation = Quaternion.Euler(0f, vehicleDriver.steer.steerAngle, 0f);

            var angle2 = Quaternion.Angle(velocityDirectionGO.transform.localRotation, steeringDirectionGO.transform.localRotation) * (steeringDirectionGO.transform.localRotation.y > velocityDirectionGO.transform.localRotation.y ? 1 : -1);

            vehicleDriver.rigid.AddRelativeTorque(Vector3.up * angle2 * steerHelperAngularVelStrength * Mathf.Clamp(vz, -10f, 10f) / 1000f, ForceMode.VelocityChange);

            var offsetYAngle = transform.eulerAngles.y - _oldRotation;
            if (Mathf.Abs(offsetYAngle) < 10f)
            {
                vehicleDriver.rigid.velocity = Quaternion.AngleAxis(offsetYAngle * 0.05f, Vector3.up) * vehicleDriver.rigid.velocity;
            }
            _oldRotation = transform.eulerAngles.y;
        }
    }
}
