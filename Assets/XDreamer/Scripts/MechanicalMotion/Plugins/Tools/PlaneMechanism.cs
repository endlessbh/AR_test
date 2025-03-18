using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Maths;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 平面运动机构
    /// </summary>
    public abstract class PlaneMechanism : Mechanism
    {
        #region 平面数据

        /// <summary>
        /// 运动目标
        /// </summary>
        [Name("运动目标")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _motionTarget;

        /// <summary>
        /// 运动目标
        /// </summary>
        public Transform motionTarget
        {
            get
            {
                if (!_motionTarget)
                {
                    _motionTarget = GetComponent<Transform>();
                }
                return _motionTarget;
            }
        }

        /// <summary>
        /// 运动平面
        /// </summary>
        [Name("运动平面")]
        public PointDirectionData _plane = new PointDirectionData();

        [Name("初始朝向")]
        [Tip("初始朝向在平面内；该点在运动目标本地坐标系下定义")]
        public Vector3Data _initDirection = new Vector3Data();

        #endregion

        #region 速度量

        /// <summary>
        /// 初始朝向：世界坐标系
        /// </summary>
        public virtual Vector3 initDirection => _initDirection.data;

        /// <summary>
        /// 内部速度量：标量
        /// </summary>
        protected virtual double velocityInternal { get; set; }

        /// <summary>
        /// 速度量：标量
        /// </summary>
        public double velocity
        {
            get => velocityInternal;
            set
            {
                if (MathX.Approximately(velocityInternal, value)) return;

                var old = velocityInternal;
                velocityInternal = value;
                onEventCallback?.Invoke(this, new EventData(EEventType.VelocityChanged, old));
            }
        }

        #endregion

        #region 事件

        /// <summary>
        /// 事件回调：参数1=运动机构，参数2=事件数据
        /// </summary>
        public static event Action<PlaneMechanism, EventData> onEventCallback;

        public enum EEventType
        {
            [Name("速度改变")]
            VelocityChanged,

            [Name("值改变")]
            ValueChanged,

            [Name("到达最小值")]
            MinValue,

            [Name("到达最大值")]
            MaxValue,
        }

        /// <summary>
        /// 事件数据
        /// </summary>
        public class EventData
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public EEventType eventType { get; private set; }

            /// <summary>
            /// 旧值，可能是速度也可能是值
            /// </summary>
            public double oldValue { get; private set; }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="eventType"></param>
            public EventData(EEventType eventType)
            {
                this.eventType = eventType;
            }

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="eventType"></param>
            /// <param name="oldVelocity"></param>
            public EventData(EEventType eventType, double oldValue)
            {
                this.eventType = eventType;
                this.oldValue = oldValue;
            }
        }

        #endregion

        #region 数值量

        /// <summary>
        /// 限定运动
        /// </summary>
        [Group("运动设置", textEN = "Motion Settings")]
        [Name("限定运动")]
        public bool _isLimit = false;

        /// <summary>
        /// 限定范围
        /// </summary>
        [Name("限定范围")]
        [Tip("基于游戏对象初始量的相对值")]
        [LimitRange(-360, 360)]
        [HideInSuperInspector(nameof(_isLimit), EValidityCheckType.False)]
        public Vector2 _range = new Vector2(0, 90);

        /// <summary>
        /// 当前值
        /// </summary>
        public virtual double currentValue 
        { 
            get => _currentValue; 
            set
            {
                var newValue = MathX.Clamp(value, minValue, maxValue);
                if (MathX.Approximately(_currentValue, newValue)) return;

                var old = _currentValue;
                _currentValue = newValue;
                onEventCallback?.Invoke(this, new EventData(EEventType.ValueChanged, old));
            }
        }

        private double _currentValue;

        /// <summary>
        /// 最小值
        /// </summary>
        protected double _minValue { get; set; } = Double.MinValue;

        /// <summary>
        /// 最小值
        /// </summary>
        public double minValue
        {
            get => _isLimit ? _range.x : _minValue;
            protected set
            {
                if (value > maxValue) value = maxValue;

                if (_isLimit)
                {
                    _range.x = (float)value;
                }
                else
                {
                    _minValue = value;
                }
            }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        protected double _maxValue { get; set; } = Double.MaxValue;

        /// <summary>
        /// 最大值
        /// </summary>
        public double maxValue
        {
            get => _isLimit ? _range.y : _maxValue;
            protected set
            {
                if (value > minValue) value = minValue;

                if (_isLimit)
                {
                    _range.y = (float)value;
                }
                else
                {
                    _maxValue = value;
                }
            }
        }

        /// <summary>
        /// 偏移值：运动开始后与程序初始启动值的偏差
        /// </summary>
        public double offsetValue { get => currentValue - initValue; protected set => currentValue = initValue + value; }

        /// <summary>
        /// 位移偏差值
        /// </summary>
        public virtual double displacementOffset { get => offsetValue; set => offsetValue = value; }

        /// <summary>
        /// 目标值
        /// </summary>
        public double targetValue { get; private set; }

        private bool followTargetValue = false;

        /// <summary>
        /// 程序启动初始值
        /// </summary>
        private double initValue = 0;

        /// <summary>
        /// 程序启动初始速度
        /// </summary>
        private double initVelocity = 0;

        #endregion

        #region Unity生命周期事件

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (motionTarget) { }

            _plane.Reset(motionTarget);
            _initDirection.SetTransform(motionTarget);
            _initDirection._dataType = EVector3DataType.TransformForward;
        }

        /// <summary>
        /// 有效回调
        /// </summary>
        protected virtual void OnValidate()
        {
            if (_range.x > _range.y) _range.x = _range.y;

            currentValue = MathX.Clamp(currentValue, minValue, maxValue);
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            currentValue = MathX.Clamp(currentValue, minValue, maxValue);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            initValue = currentValue;
            initVelocity = velocity;
        }

        /// <summary>
        /// 重置为初始化
        /// </summary>
        public void ResetInit()
        {
            velocity = initVelocity;
            SetInitValue();
        }

        /// <summary>
        /// 能否执行运动
        /// </summary>
        /// <returns></returns>
        public override bool CanDoMotion()
        {
            // 速度不为0，并且没有达到最小最大值时，认为可以运动
            return (!MathX.ApproximatelyZero(velocity)
                && !(velocity < 0 && MathX.Approximately(currentValue, minValue))
                && !(velocity > 0 && MathX.Approximately(currentValue, maxValue)));
        }

        /// <summary>
        /// 执行运动
        /// </summary>
        public override void DoMotion()
        {
            double offset = velocityInternal * Time.deltaTime;

            var newValue = currentValue + offset;

            // 限定在最小和最大值之间
            offset = MathX.Clamp(newValue, minValue, maxValue) - currentValue;

            // 有目标时，使用补间方法
            if (followTargetValue)
            {
                // 到达或越过目标长度点, 则设定为到达目标
                if (MathX.Approximately(currentValue, targetValue)
                    || (currentValue < targetValue && newValue >= targetValue)
                    || (currentValue > targetValue && newValue <= targetValue))
                {
                    followTargetValue = false;
                    velocity = 0;
                    offset = targetValue - currentValue;
                }
            }
            // 到达最小值最大值则设置速度为0
            var reachMaxValue = (currentValue < maxValue && newValue >= maxValue);
            var reachMinValue = (currentValue > minValue && newValue <= minValue);

            currentValue += offset;

            // 先加当前值后进行回调
            if (reachMaxValue)
            {
                onEventCallback?.Invoke(this, new EventData(EEventType.MaxValue));
                velocity = 0;
            }

            if (reachMinValue)
            {
                onEventCallback?.Invoke(this, new EventData(EEventType.MinValue));
                velocity = 0;
            }
        }

        #endregion

        #region 设置值

        /// <summary>
        /// 设置期望到达的目标值和时间
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetTargetValueByTime(double value, double time)
        {
            var rs = TryGetValue(value, out var validValue);
            if (rs)
            {
                // 当时间为0时，表示瞬间到达目标
                if (MathX.ApproximatelyZero(time))
                {
                    SetValueInternal(value);
                }
                else // 时间不为0时，计算移动速度
                {
                    SetTargetValueVelocity(validValue, (targetValue - currentValue) / time);
                }
            }
            return rs;
        }

        /// <summary>
        /// 设置期望到达的目标值和速度
        /// </summary>
        /// <param name="value"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public bool SetTargetValueByVelocity(double value, double velocity)
        {
            var rs = TryGetValue(value, out var validValue);
            if (rs)
            {
                // 调整速度方向，保证对象能按正确方向移动到目标值
                SetTargetValueVelocity(validValue, validValue > currentValue ? velocity : -velocity);
            }
            return rs;
        }

        private void SetTargetValueVelocity(double value, double velocity)
        {
            targetValue = value;

            this.velocity = velocity;
            followTargetValue = true;
        }

        /// <summary>
        /// 直接设定目标值, 并将速度量设置为0
        /// </summary>
        /// <param name="value"></param>
        public bool SetValue(double value)
        {
            var rs = TryGetValue(value, out var validValue);
            if (rs)
            {
                SetValueInternal(validValue);
            }
            return rs;
        }

        private void SetValueInternal(double value)
        {
            velocity = 0;
            currentValue = value;
        }

        private bool TryGetValue(double inValue, out double outValue)
        {
            outValue = MathX.Clamp(inValue, minValue, maxValue);

            return !MathX.Approximately(currentValue, outValue);
        }

        /// <summary>
        /// 设置为最小值
        /// </summary>
        public virtual void SetMinValue() => SetValueInternal(minValue);

        /// <summary>
        /// 设置为最大值
        /// </summary>
        public virtual void SetMaxValue() => SetValueInternal(maxValue);

        /// <summary>
        /// 设置为初始值
        /// </summary>
        public virtual void SetInitValue() => SetValueInternal(initValue);

        #endregion

        #region 设置偏移值

        /// <summary>
        /// 设置偏移量
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetOffsetByTime(double offset, double time) => SetTargetValueByTime(currentValue + offset, time);

        /// <summary>
        /// 设置偏移量
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool SetOffsetByVelocity(double offset, double time) => SetTargetValueByTime(currentValue + offset, time);

        /// <summary>
        /// 设置偏移量
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool SetOffset(double offset) => SetValue(currentValue + offset); 

        #endregion
    }
}
