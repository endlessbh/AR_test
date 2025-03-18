using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 转向器 ：控制转向轮转向的机构
    /// </summary>
    [Name("转向器")]
    [Tip("控制转向轮转向的机构", "Mechanism for controlling steering wheel steering")]
    [XCSJ.Attributes.Icon(EIcon.SteerWheel)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class Steer : VehicleDriverGetter, ISteer
    {
        /// <summary>
        /// 转角区间 ：高速时，转向角会趋向与小值
        /// </summary>
        [Name("转角区间")]
        [Tip("高速时，转向角会趋向与小值", "At high speed, the steering angle tends to be smaller than the normal value")]
        [LimitRange(0, 90)]
        public Vector2 _steerAngelRange = new Vector3(5, 40);

        /// <summary>
        /// 转向速度 ：值越大越容易转向
        /// </summary>
        [Name("转向速度")]
        [Tip("值越大越容易转向", "The higher the value, the easier it is to turn")]
        [Range(0.01f, 1000)]
        public float _steerSpeed = 100f;

        /// <summary>
        /// 方向盘 ：方向盘是绕自身Z轴旋转
        /// </summary>
        [Name("方向盘")]
        [Tip("方向盘是绕自身Z轴旋转", "The steering wheel rotates around its own Z axis")]
        public GameObject _steerWheel = null;

        /// <summary>
        /// 方向盘本地旋转轴 ：方向盘会绕着本地坐标系的X或Y或Z旋转
        /// </summary>
        [Name("方向盘本地旋转轴")]
        [Tip("方向盘会绕着本地坐标系的X或Y或Z旋转", "The steering wheel rotates around the X or Y or Z of the local coordinate system")]
        [HideInSuperInspector(nameof(_steerWheel), EValidityCheckType.Null)]
        public ELocationRotationAxis _LocationRotationAxis = ELocationRotationAxis.Z;

        /// <summary>
        /// 方向盘转动角倍数 ：当车轮转动n度时，方向盘转动角为n与当前值的乘积;此时方向盘是绕自身Z轴旋转
        /// </summary>
        [Name("方向盘转动角倍数")]
        [Tip("当车轮转动n度时，方向盘转动角为n与当前值的乘积;此时方向盘是绕自身Z轴旋转", "When the wheel rotates n degrees, the steering wheel rotation angle is the product of N and the current value; At this time, the steering wheel rotates around its own Z axis")]
        [HideInSuperInspector(nameof(_steerWheel), EValidityCheckType.Null)]
        public float _steeringWheelMultiplier = 10f;

        /// <summary>
        /// 转向角
        /// </summary>
        public float steerAngle => _steerWheelCollider ? _steerWheelCollider.steerAngle : 0;

        /// <summary>
        /// 转向轮
        /// </summary>
        public float steerWheelAngle { get; private set; }

        private WheelCollider _steerWheelCollider = null;

        private Vector3 _localEulerAngles = Vector3.zero;

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            if (vehicleDriver)
            {
                vehicleDriver.steer = this;

                if (vehicleDriver.steerWheelDrivers.Count > 0)
                {
                    _steerWheelCollider = vehicleDriver.steerWheelDrivers[0].wheelCollider;
                }
            }

            if (_steerWheel)
            {
                _localEulerAngles = _steerWheel.transform.localEulerAngles;
            }
        }

        /// <summary>
        /// 引擎工作：计算速度与RPM
        /// </summary>
        protected override void OnFixedUpdate()
        {
            // 转向
            var steerValue = Mathf.Clamp(vehicleDriver.steerInput, -1f, 1f) * Mathf.Lerp(_steerAngelRange.y, _steerAngelRange.x, vehicleDriver.speed / _steerSpeed);

            foreach (var wheelDriver in vehicleDriver.steerWheelDrivers)
            {
                wheelDriver.SetSteer(steerValue);
            }

            // 方向盘设置
            if (_steerWheel)
            {
                steerWheelAngle = -steerAngle * _steeringWheelMultiplier;
                switch (_LocationRotationAxis)
                {
                    case ELocationRotationAxis.X: _steerWheel.transform.localRotation = Quaternion.Euler(steerWheelAngle, _localEulerAngles.y, _localEulerAngles.z); break;
                    case ELocationRotationAxis.Y: _steerWheel.transform.localRotation = Quaternion.Euler(_localEulerAngles.x, steerWheelAngle, _localEulerAngles.z); break;
                    case ELocationRotationAxis.Z: _steerWheel.transform.localRotation = Quaternion.Euler(_localEulerAngles.x, _localEulerAngles.y, steerWheelAngle); break;
                }
            }
        }

        /// <summary>
        /// 本地旋转轴
        /// </summary>
        [Name("本地旋转轴")]
        public enum ELocationRotationAxis
        {
            [Name("X")]
            X,

            [Name("X")]
            Y,

            [Name("X")]
            Z,
        }
    }
}
