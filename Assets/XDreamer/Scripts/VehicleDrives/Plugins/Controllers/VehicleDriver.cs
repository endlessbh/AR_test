using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 车辆控制器 ：车辆控制的主对象，由发动机、变速箱、制动器、转向器、燃料和车灯等核心子对象构成
    /// </summary>
    [Name(Title)]
    [Tip("车辆控制的主对象，由发动机、变速箱、制动器、转向器、燃料和车灯等核心子对象构成", "The main object of vehicle control is composed of core sub objects such as engine, gearbox, brake, steering gear, fuel and lamp")]
    [XCSJ.Attributes.Icon(EIcon.Car)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    [AddComponentMenu(VehicleDriveHelper.ComponentMenuPath + Title)]
    public class VehicleDriver : VehicleControllerGetter, IVehicleController
    {
        public const string Title = "车辆驾驶控制器";

        #region 基础属性

        /// <summary>
        /// 刚体
        /// </summary>
        public Rigidbody rigid
        {
            get
            {
                if (!_rigid)
                {
                    _rigid = GetComponent<Rigidbody>();
                }
                return _rigid;
            }
        }
        private Rigidbody _rigid = null;

        /// <summary>
        /// 速度向量
        /// </summary>
        public Vector3 velocity => rigid.velocity;

        /// <summary>
        /// 质量
        /// </summary>
        public float mass => rigid.mass;

        /// <summary>
        /// 车辆速度 : 单位 KMH
        /// </summary>
        public float speed => rigid.velocity.magnitude * 3.6f;

        /// <summary>
        /// 方向:1表示前进，-1表示后退
        /// </summary>
        public int direction { get; set; } = 1;

        /// <summary>
        /// 程序运行时启动引擎
        /// </summary>
        [Name("程序运行时启动引擎")]
        public bool _runEngineAtStart = true;

        /// <summary>
        /// 最大速度 ：单位千米每小时（KMH）
        /// </summary>
        [Name("最大速度")]
        [Tip("单位：千米/小时（KM/H）", "Unit: km / h")]
        public float maxSpeed = 240f;

        #endregion

        #region 车辆核心部件

        /// <summary>
        /// 引擎
        /// </summary>
        public IEngine engine { get; set; }

        /// <summary>
        /// 刹车控制器
        /// </summary>
        public IBrake brake { get; set; }

        /// <summary>
        /// 变速箱
        /// </summary>
        public IGearBox gearBox { get; set; }

        /// <summary>
        /// 方向盘
        /// </summary>
        public ISteer steer { get; set; }

        /// <summary>
        /// 燃料对象
        /// </summary>
        public IFuel fuel { get; set; }

        /// <summary>
        /// 车辆灯光控制器
        /// </summary>
        public IVehicleLightController lightController { get; set; }

        #endregion

        #region 动力与转向输入

        /// <summary>
        /// 动力输入
        /// </summary>
        public float throttleInput { get; set; }

        /// <summary>
        /// 转向输入
        /// </summary>
        public float steerInput { get; set; }

        /// <summary>
        /// 动力
        /// </summary>
        public float throttleValue { get; private set; } = 0f;

        /// <summary>
        /// 扩展动力
        /// </summary>
        public float boostValue { get; set; } = 1;

        /// <summary>
        /// 刹车
        /// </summary>
        public float brakeValue { get; private set; } = 0f;

        #endregion

        /// <summary>
        /// 车辆所有轮子控制器
        /// </summary>
        public List<WheelDriver> wheelDrivers { get; private set; }

        /// <summary>
        /// 驱动轮列表
        /// </summary>
        public List<WheelDriver> powerWheelDrivers { get; private set; } = new List<WheelDriver>();

        /// <summary>
        /// 转向轮
        /// </summary>
        public List<WheelDriver> steerWheelDrivers { get; private set; } = new List<WheelDriver>();

        /// <summary>
        /// 动力轮数量
        /// </summary>
        public int powerWheelCount { get; private set; } = 0;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // 配置车轮碰撞器
            wheelDrivers = GetComponentsInChildren<WheelDriver>().ToList();
            foreach (var wd in wheelDrivers)
            {
                if (wd._canPower)
                {
                    powerWheelDrivers.Add(wd);
                }
                if (wd._canSteer)
                {
                    steerWheelDrivers.Add(wd);
                }
            }
            powerWheelCount = powerWheelDrivers.Count;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            if (_runEngineAtStart)
            {
                Invoke(nameof(StartEngine), 1);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            ValidCheck();
        }

        private void ValidCheck()
        {
            if (wheelDrivers.Count < 2)
            {
                Debug.LogError("车轮碰撞器必须大于等于2！");
            }

            if (GetComponentsInChildren<Collider>().Length <= wheelDrivers.Count)
            {
                Debug.LogErrorFormat("{0}对象下必须存在至少一个以上的非车轮碰撞器的碰撞器用于模拟车身碰撞！", name);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if (engine == null || fuel == null || gearBox == null) return;

            var orgThrottleInput = Mathf.Clamp01(throttleInput);
            var orgBrakeInput = Mathf.Abs(Mathf.Clamp(throttleInput, -1f, 0f));

            // 动力
            if (!engine.running || fuel.GetPower() <= 0f || gearBox.changingGear)
            {
                throttleValue = 0f;
            }
            else
            {
                if (gearBox.auto)
                {
                    throttleValue = direction == 1 ? Mathf.Clamp01(orgThrottleInput) : Mathf.Clamp01(orgBrakeInput);
                }
                else
                {
                    throttleValue = orgThrottleInput;
                }
            }

            // 刹车
            if (gearBox.auto)
            {
                brakeValue = direction == 1 ? Mathf.Clamp01(orgBrakeInput) : Mathf.Clamp01(orgThrottleInput);
            }
            else
            {
                brakeValue = Mathf.Clamp01(orgBrakeInput);
            }

            gearBox.ChangeGear();
        }

        /// <summary>
        /// 获取平均滚轮RPM
        /// </summary>
        /// <returns></returns>
        public float GetPowerWheelRPM()
        {
            if (powerWheelCount == 0) return 0;

            float wheelRPM = 0;
            powerWheelDrivers.ForEach(wd => wheelRPM += wd.wheelCollider.rpm);
            return wheelRPM / powerWheelCount;
        }

        #region 发动机启动停止

        /// <summary>
        /// 设置引擎开关
        /// </summary>
        public void SetEngine(bool running)
        {
            if (running)
            {
                StartEngine();
            }
            else
            {
                StopEngine();
            }
        }

        /// <summary>
        /// 切换引擎启动或关闭
        /// </summary>
        public void StartOrStopEngine()
        {
            if (engine.running)
            {
                StopEngine();
            }
            else
            {
                StartEngine();
            }
        }

        /// <summary>
        /// 启动引擎
        /// </summary>
        public void StartEngine()
        {
            if (!engine.running) StartCoroutine(StartEngineDelayed());
        }

        private IEnumerator StartEngineDelayed()
        {
            if (!engine.running)
            {
                SendVehicleState(this, new EngineEventArgs(true));

                yield return new WaitForSeconds(1f);

                engine.running = true;
            }
            yield return new WaitForSeconds(1f);
        }

        /// <summary>
        /// 启用引擎
        /// </summary>
        public void StopEngine()
        {
            if (engine.running) StartCoroutine(StopEngineDelayed());
        }

        private IEnumerator StopEngineDelayed()
        {
            engine.running = false;

            yield return new WaitForSeconds(0.5f);

            SendVehicleState(this, new EngineEventArgs(false));
        }

        #endregion

        #region 事件

        /// <summary>
        /// 引擎启动停止
        /// </summary>
        public static event Action<VehicleDriver, VehicleEventArgs> onVehicleStateChanged = null;

        internal static void SendVehicleState(VehicleDriver vehicle, VehicleEventArgs vehicleEventArgs)
        {
            onVehicleStateChanged?.Invoke(vehicle, vehicleEventArgs);
        }

        #endregion

        /// <summary>
        /// 绘制
        /// </summary>
        protected void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                foreach (var wd in wheelDrivers)
                {
                    wd.wheelCollider.GetGroundHit(out WheelHit hit);

                    var temp = Gizmos.matrix;
                    Gizmos.matrix = Matrix4x4.TRS(wd.transform.position, Quaternion.AngleAxis(-90, Vector3.right), Vector3.one);
                    var r = hit.force / rigid.mass / 5f;
                    Gizmos.color = new Color(r, -r, 0f);
                    Gizmos.DrawFrustum(Vector3.zero, 2f, hit.force / rigid.mass, .1f, 1f);
                    Gizmos.matrix = temp;
                }
            }
#endif
        }
    }

    /// <summary>
    /// 车辆获取器
    /// </summary>
    public abstract class VehicleDriverGetter : BaseVehicle
    {
        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        [Name("车辆驾驶器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 车辆控制父对象 
        /// </summary>
        public VehicleDriver vehicleDriver => this.XGetComponentInParent<VehicleDriver>(ref _vehicleDriver);

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (vehicleDriver) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (vehicleDriver) { }
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected virtual void FixedUpdate() { if (_vehicleDriver) OnFixedUpdate(); }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected virtual void OnFixedUpdate() { }
    }

    /// <summary>
    /// 车辆切换状态:用于监听车辆切换类型事件，例如发动起启动停止、手刹拉上与松开
    /// </summary>
    [Serializable]
    [Name("车辆切换状态")]
    public class VehicleDriverToggleState
    {
        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        [Name("车辆驾驶器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 车辆状态类型
        /// </summary>
        [Name("车辆状态类型")]
        [EnumPopup]
        public EVehicleStateType _vehicleStateType = EVehicleStateType.EngineRunning;

        /// <summary>
        /// 灯光类型
        /// </summary>
        [Name("灯光类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_vehicleStateType), EValidityCheckType.NotEqual, EVehicleStateType.Light)]
        public ELightType _lightType = ELightType.LowBeamHead;

        private event Action<bool> _onValueChanged = null;

        /// <summary>
        /// 添加监听并使用回调函数初始化
        /// </summary>
        public void AddListenerAndInit(Action<bool> onValueChanged)
        {
            AddListener(onValueChanged);

            onValueChanged?.Invoke(state);
        }

        /// <summary>
        /// 添加监听回调函数
        /// </summary>
        /// <param name="onValueChanged"></param>
        public void AddListener(Action<bool> onValueChanged)
        {
            VehicleDriver.onVehicleStateChanged += OnVehicleStateChanged;

            _onValueChanged += onValueChanged;
        }

        /// <summary>
        /// 移除监听回调函数
        /// </summary>
        public void RemoveListener(Action<bool> onValueChanged)
        {
            VehicleDriver.onVehicleStateChanged -= OnVehicleStateChanged;

            _onValueChanged -= onValueChanged;
        }

        private void OnVehicleStateChanged(VehicleDriver vehicleController, VehicleEventArgs vehiclePartEventArgs)
        {
            if (_vehicleDriver != vehicleController) return;

            switch (_vehicleStateType)
            {
                case EVehicleStateType.EngineRunning:
                    {
                        if (vehiclePartEventArgs is EngineEventArgs engineEventArgs)
                        {
                            _onValueChanged?.Invoke(engineEventArgs.running);
                        }
                        break;
                    }
                case EVehicleStateType.Handbrake:
                    {
                        if (vehiclePartEventArgs is BrakeEventArgs brakeEventArgs)
                        {
                            _onValueChanged?.Invoke(brakeEventArgs.handBrakeOn);
                        }
                        break;
                    }
                case EVehicleStateType.NGear:
                    {
                        if (vehiclePartEventArgs is GearBoxEventArgs gearBoxEventArgs)
                        {
                            _onValueChanged?.Invoke(gearBoxEventArgs.NGear);
                        }
                        break;
                    }
                case EVehicleStateType.Light:
                    {
                        if (vehiclePartEventArgs is LightEventArgs lightEventArgs && lightEventArgs.lightType == _lightType)
                        {
                            _onValueChanged?.Invoke(lightEventArgs.isOn);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public bool state
        {
            get
            {
                if (!_vehicleDriver) return false;

                switch (_vehicleStateType)
                {
                    case EVehicleStateType.EngineRunning:
                        {
                            if (_vehicleDriver.engine != null)
                            {
                                return _vehicleDriver.engine.running;
                            }
                            break;
                        }
                    case EVehicleStateType.Handbrake:
                        {
                            if (_vehicleDriver.brake != null)
                            {
                                return _vehicleDriver.brake.handbrakeOn;
                            }
                            break;
                        }
                    case EVehicleStateType.NGear:
                        {
                            if (_vehicleDriver.gearBox != null)
                            {
                                return _vehicleDriver.gearBox.NGear;
                            }
                            break;
                        }
                    case EVehicleStateType.Light:
                        {
                            if (_vehicleDriver.lightController != null)
                            {
                                return _vehicleDriver.lightController.GetLightState(_lightType);
                            }
                            break;
                        }
                }
                return false;
            }

            set
            {
                if (!_vehicleDriver) return;

                switch (_vehicleStateType)
                {
                    case EVehicleStateType.EngineRunning:
                        {
                            if (_vehicleDriver.engine != null)
                            {
                                _vehicleDriver.engine.running = value;
                            }
                            break;
                        }
                    case EVehicleStateType.Handbrake:
                        {
                            if (_vehicleDriver.brake != null)
                            {
                                _vehicleDriver.brake.handbrakeOn = value;
                            }
                            break;
                        }
                    case EVehicleStateType.NGear:
                        {
                            if (_vehicleDriver.brake != null)
                            {
                                _vehicleDriver.gearBox.NGear = value;
                            }
                            break;
                        }
                    case EVehicleStateType.Light:
                        {
                            var lightCtrl = _vehicleDriver.lightController;
                            if (lightCtrl != null)
                            {
                                lightCtrl.SetLightState(_lightType, value); break;
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            return _vehicleStateType == EVehicleStateType.Light ? CommonFun.Name(_lightType) : CommonFun.Name(_vehicleStateType);
        }

        /// <summary>
        /// 车辆状态类型
        /// </summary>
        [Name("车辆状态类型")]
        public enum EVehicleStateType
        {
            /// <summary>
            /// 发动机运行
            /// </summary>
            [Name("发动机运行")]
            EngineRunning,

            /// <summary>
            /// 手刹
            /// </summary>
            [Name("手刹")]
            Handbrake,

            /// <summary>
            /// 空挡
            /// </summary>
            [Name("空挡")]
            NGear,

            /// <summary>
            /// 车灯
            /// </summary>
            [Name("车灯")]
            Light,
        }
    }
}
