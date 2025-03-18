using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Maths;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 旋转平面运动机构:在指定的平面上旋转
    /// </summary>
    [Name("旋转机构")]
    [XCSJ.Attributes.Icon(EMemberRule.DeclaringType)]
    [Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    public class RotationMechanism : PlaneMechanism
    {
        /// <summary>
        /// 线速度
        /// </summary>
        public double linearVelocity
        {
            get => GetLinearVelocity(_radius);
            set => SetLinearVelocity(value, _radius);
        }


        /// <summary>
        ///  线速度 = 角速度 * 半径 * PI / 180; （弧长公式/时间）
        /// </summary>
        /// <param name="radius">输入半径</param>
        /// <returns></returns>
        public double GetLinearVelocity(double radius)
        {
            return velocity * radius * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 使用线速度值设置，将转换为角速度值
        /// </summary>
        /// <param name="linearVelocity"></param>
        /// <param name="radius"></param>
        public void SetLinearVelocity(double linearVelocity, double radius)
        {
            velocity = linearVelocity / (radius * Mathf.Deg2Rad);
        }


        /// <summary>
        /// 旋转机构默认位移量
        /// </summary>
        public override double displacementOffset
        {
            get => GetDisplacementOffset(_radius);
            set => SetDisplacementOffset(value, _radius);
        }

        /// <summary>
        /// 位移量 = 角度偏差 * 半径 * PI / 180; (弧长公式)
        /// </summary>
        public double GetDisplacementOffset(double radius) => offsetValue * radius * Mathf.Deg2Rad;

        /// <summary>
        /// 设置位移量
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public void SetDisplacementOffset(double displacement, double radius) => offsetValue = displacement / (radius * Mathf.Deg2Rad);

        /// <summary>
        /// 当前偏移角度：与初始朝向的夹角
        /// </summary>
        [Name("当前偏移角度")]
        [Tip("与初始朝向的夹角")]
        public double _currentAngle = 0;

        /// <summary>
        /// 当前值
        /// </summary>
        public override double currentValue 
        { 
            get => _currentAngle; 
            set
            {
                var offset = value - currentValue;

                _currentAngle = value;

                motionTarget.RotateAround(_plane.point, _plane.direction, (float)offset);
            }
        }

        /// <summary>
        /// 角速度
        /// </summary>
        [Name("角速度(角度/秒)")]
        public double _angularVelocity;

        /// <summary>
        /// 角速度：标量(角度/秒)
        /// </summary>
        protected override double velocityInternal { get => _angularVelocity; set => _angularVelocity = value; }

        /// <summary>
        /// 半径
        /// </summary>
        [Name("半径")]
        [Min(0f)]
        public double _radius = 1;

        /// <summary>
        /// 有效性检查
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            _currentAngle = MathX.Clamp(_currentAngle, minValue, maxValue);
        }
    }
}
