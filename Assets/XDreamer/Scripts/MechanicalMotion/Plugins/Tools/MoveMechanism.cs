using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 平移机构：在一条限定长度的路径上移动
    /// </summary>
    [Name("平移机构")]
    [XCSJ.Attributes.Icon()]
    [Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    public class MoveMechanism : PlaneMechanism
    {
        /// <summary>
        /// 当前偏移长度
        /// </summary>
        [Name("当前偏移长度")]
        public double _currentLength = 0;

        /// <summary>
        /// 当前长度：已移动长度
        /// 设置长度：起点=平面上的点，终点=起点+朝向*长度
        /// </summary>
        public override double currentValue
        {
            get => _currentLength;
            set
            {
                var offset = value - currentValue;
                _currentLength = value;

                if (transform == motionTarget.transform)
                {
                    motionTarget.transform.position += initDirection.normalized * (float)offset;
                }
                else
                {
                    motionTarget.transform.position = transform.position + initOffset + initDirection.normalized * (float)_currentLength;
                }
            }
        }

        /// <summary>
        /// 移动速度
        /// </summary>
        [Name("移动速度(米/秒)")]
        public double _velocity = 0;

        /// <summary>
        /// 内部速度量
        /// </summary>
        protected override double velocityInternal { get => _velocity; set => _velocity = value; }

        private Vector3 initOffset = Vector3.zero;

        /// <summary>
        /// 有效验证
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            _currentLength = MathX.Clamp(_currentLength, minValue, maxValue);
        }

        protected override void Awake()
        {
            base.Awake();

            initOffset = motionTarget.transform.position - transform.position - initDirection.normalized * (float)_currentLength;
        }
    }
}
