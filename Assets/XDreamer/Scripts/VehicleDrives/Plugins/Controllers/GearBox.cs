using System;
using System.Collections;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 变速箱
    /// </summary>
    [Name("变速箱")]
    [XCSJ.Attributes.Icon(EIcon.Config)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class GearBox : VehicleDriverGetter, IGearBox
    {
        /// <summary>
        /// 当前速率
        /// </summary>
        public float currentRadio => gears[currentGearIndex].ratio;

        /// <summary>
        /// 当前齿轮
        /// </summary>
        public Gear currentGear => gears[currentGearIndex];

        /// <summary>
        /// 齿轮
        /// </summary>
        [System.Serializable]
        [Name("齿轮")]
        public class Gear
        {
            /// <summary>
            /// 比例乘积
            /// </summary>
            [Name("比例乘积")]
            public float ratio;

            /// <summary>
            /// 最大速度
            /// </summary>
            [Name("最大速度")]
            public int maxSpeed;

            /// <summary>
            /// 目标速度
            /// </summary>
            [Name("目标速度")]
            public int targetSpeedForNextGear;

            /// <summary>
            /// 构造函数
            /// </summary>
            public Gear(float ratio, int speed, int targetSpeed)
            {
                this.ratio = ratio;
                maxSpeed = speed;
                this.targetSpeedForNextGear = targetSpeed;
            }
        }

        /// <summary>
        /// 自动倒挡：前进时，按下后退，自动转换为倒挡
        /// </summary>
        [Name("自动倒挡")]
        public bool autoReverse = true;

        /// <summary>
        /// 自动变速
        /// </summary>
        [Name("自动变速")]
        public bool _auto = true;

        /// <summary>
        /// 自动变速
        /// </summary>
        public bool auto { get => _auto; }

        /// <summary>
        /// 齿轮总数
        /// </summary>
        [Name("齿轮总数")]
        [Range(1, 8)]
        public int _totalGearCount = 6;

        /// <summary>
        /// 齿轮总数
        /// </summary>
        public int totalGearCount { get => _totalGearCount; }

        /// <summary>
        /// 变速箱切换阈值
        /// </summary>
        [Name("变速箱切换阈值")]
        [Range(.25f, 1)]
        public float gearShiftingThreshold = .75f;

        /// <summary>
        /// 当前齿轮索引
        /// </summary>
        public int currentGearIndex { get; private set; } = 0;

        /// <summary>
        /// 齿轮组
        /// </summary>
        [Name("齿轮组")]
        public Gear[] gears;

        /// <summary>
        /// 空挡
        /// </summary>
        [Name("空挡")]
        public bool _NGear = false;

        /// <summary>
        /// 空挡
        /// </summary>
        public bool NGear { get => _NGear; set => _NGear = value; }

        /// <summary>
        /// 齿轮切换延时
        /// </summary>
        [Name("齿轮切换延时")]
        [Range(0f, .5f)]
        public float gearShiftingDelay = .35f;

        /// <summary>
        /// 离合惯性
        /// </summary>
        [Name("离合惯性")]
        [Range(.1f, .9f)]
        [Tip("值较小时，匹配离合越快;值较大时，离合越平滑", "When the value is small, the faster the matching clutch is; When the value is large, the clutch is smoother")]
        public float clutchInertia = .25f;

        /// <summary>
        /// 升挡RPM
        /// </summary>
        [Name("升挡RPM")]
        [Tip("当转速达到或超过当前值时，自动升一档", "When the speed reaches or exceeds the current value, it will automatically upshift by one gear")]
        public float gearShiftUpRPM = 3750f;

        /// <summary>
        /// 离合
        /// </summary>
        public float clutchInput { get; set; } = 0f;

        /// <summary>
        /// 正在换挡
        /// </summary>
        public bool changingGear { get; set; } = false;

        private bool canGoReverseNow { get; set; } = false;

        private float launched { get; set; } = 0f;

        /// <summary>
        /// 重置
        /// </summary>
        protected override void Reset()
        {
            base.Reset();

            InitGears();
        }

        /// <summary>
        /// 基于速度初始化变速箱
        /// </summary>
        public void InitGears()
        {
            var vehicleDriver = this.vehicleDriver;
            if (!vehicleDriver) return;

            var maxSpeedForGear = new int[_totalGearCount];
            for (int i = 0; i < _totalGearCount; i++)
            {
                maxSpeedForGear[i] = (int)(vehicleDriver.maxSpeed * (i + 1) / _totalGearCount);
            }

            // 变速表
            float[] gearRatio = new float[] { };
            switch (_totalGearCount)
            {
                case 1: gearRatio = new float[] { 1.0f }; break;
                case 2: gearRatio = new float[] { 2.0f, 1.0f }; break;
                case 3: gearRatio = new float[] { 2.0f, 1.5f, 1.0f }; break;
                case 4: gearRatio = new float[] { 2.86f, 1.62f, 1.0f, .72f }; break;
                case 5: gearRatio = new float[] { 4.23f, 2.52f, 1.66f, 1.22f, 1.0f, }; break;
                case 6: gearRatio = new float[] { 4.35f, 2.5f, 1.66f, 1.23f, 1.0f, .85f }; break;
                case 7: gearRatio = new float[] { 4.5f, 2.5f, 1.66f, 1.23f, 1.0f, .9f, .8f }; break;
                case 8: gearRatio = new float[] { 4.6f, 2.5f, 1.86f, 1.43f, 1.23f, 1.05f, .9f, .72f }; break;
            }
            gears = new Gear[_totalGearCount];
            for (int i = 0; i < _totalGearCount; i++)
            {
                gears[i] = new Gear(gearRatio[i], maxSpeedForGear[i], (int)Mathf.Lerp(0, vehicleDriver.maxSpeed * Mathf.Lerp(0f, 1f, gearShiftingThreshold), ((i + 1) / (float)gears.Length)));
            }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            if(vehicleDriver) vehicleDriver.gearBox = this;

            if (gears == null || gears.Length == 0) InitGears();
        }

        /// <summary>
        /// 定时更新
        /// </summary>
        protected override void OnFixedUpdate()
        {
            if (!vehicleDriver || vehicleDriver.brake==null) return;

            // 自动逆向
            if (autoReverse)
            {
                canGoReverseNow = true;
            }
            else
            {
                if (vehicleDriver.brakeValue < .5f && vehicleDriver.speed < 5)
                {
                    canGoReverseNow = true;
                }
                else if (vehicleDriver.brakeValue > 0 && transform.InverseTransformDirection(vehicleDriver.rigid.velocity).z > 1f)
                {
                    canGoReverseNow = false;
                }
            }

            if (auto && !changingGear)
            {
                if (currentGearIndex < gears.Length - 1 && vehicleDriver.speed >= gears[currentGearIndex].targetSpeedForNextGear && vehicleDriver.engine.RPM >= gearShiftUpRPM)
                {
                    StartCoroutine(ChangeGear(currentGearIndex + 1));
                }

                if (currentGearIndex > 0 && vehicleDriver.speed < (gears[currentGearIndex - 1].targetSpeedForNextGear * .5f) && vehicleDriver.direction != -1)
                {
                    StartCoroutine(ChangeGear(currentGearIndex - 1));
                }
            }

            Clutch();
        }

        /// <summary>
        /// 换挡
        /// </summary>
        public void ChangeGear()
        {
            if (!changingGear && auto)
            {
                var inverseDir = transform.InverseTransformDirection(vehicleDriver.rigid.velocity);

                if (vehicleDriver.brakeValue > .9f && inverseDir.z < 1f && canGoReverseNow && vehicleDriver.direction != -1)
                {
                    StartCoroutine(ChangeGear(-1));
                }
                else if (vehicleDriver.throttleValue < .1f && inverseDir.z > -1f && vehicleDriver.direction == -1)
                {
                    StartCoroutine(ChangeGear(0));
                }
            }
        }

        /// <summary>
        /// 离合器
        /// </summary>
        private void Clutch()
        {
            if (vehicleDriver.brake.handbrakeOn || _NGear)
            {
                clutchInput = 1f;
            }
            else if (currentGearIndex == 0)
            {
                if (vehicleDriver.throttleValue >= .1f)
                {
                    launched += vehicleDriver.throttleValue * Time.deltaTime;
                }
                else
                {
                    launched -= Time.deltaTime;
                }

                launched = Mathf.Clamp01(launched);

                clutchInput = (launched >= .25f) ?
                    Mathf.Lerp(clutchInput, Mathf.Lerp(1f, Mathf.Lerp(clutchInertia, 0f, vehicleDriver.GetPowerWheelRPM() / gears[0].targetSpeedForNextGear), Mathf.Abs(vehicleDriver.throttleValue)), Time.fixedDeltaTime * 5f) :
                    Mathf.Lerp(clutchInput, 1f / vehicleDriver.speed, Time.fixedDeltaTime * 5f);
            }
            else
            {
                clutchInput = Mathf.Lerp(clutchInput, changingGear ? 1 : 0, Time.fixedDeltaTime * 5f);
            }

            clutchInput = Mathf.Clamp01(clutchInput);
        }

        /// <summary>
        /// 换挡回调
        /// </summary>
        public event Action onChangeGear = null;

        /// <summary>
        /// 换挡
        /// </summary>
        /// <returns>The gear.</returns>
        /// <param name="gear">Gear.</param>
        public IEnumerator ChangeGear(int gear)
        {
            changingGear = true;

            onChangeGear?.Invoke();

            yield return new WaitForSeconds(gearShiftingDelay);

            if (gear == -1)
            {
                currentGearIndex = 0;
                vehicleDriver.direction = !_NGear ? -1 : 0;
            }
            else
            {
                currentGearIndex = gear;
                vehicleDriver.direction = !_NGear ? 1 : 0;
            }
            changingGear = false;
        }

        /// <summary>
        /// 升档
        /// </summary>
        public void ShiftUp()
        {
            if (auto) return;

            if (currentGearIndex < gears.Length - 1 && !changingGear)
            {
                StartCoroutine(ChangeGear(vehicleDriver.direction != -1 ? currentGearIndex + 1 : 0));
            }
        }

        /// <summary>
        /// 切换齿轮至指定档位
        /// </summary>
        public void ShiftTo(int gear)
        {
            if (gear >= 0 && gear < gears.Length && gear != currentGearIndex)
            {
                StartCoroutine(ChangeGear(gear));
            }
        }

        /// <summary>
        /// 降档
        /// </summary>
        public void ShiftDown()
        {
            if (auto) return;

            if (currentGearIndex >= 0) StartCoroutine(ChangeGear(currentGearIndex - 1));
        }
    }
}
