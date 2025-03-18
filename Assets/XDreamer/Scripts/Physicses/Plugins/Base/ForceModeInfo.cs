using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginPhysicses.Base
{
    /// <summary>
    /// 施力模式信息
    /// </summary>
    [Serializable]
    [Name("施力模式")]
    public class ForceModeInfo
    {
        /// <summary>
        /// 施力模式:Force(持续力)，Impulse(脉冲力)，VelocityChange(速度)和Acceleration(加速度);当类型为VelocityChange(速度)和Acceleration(加速度)时忽略刚体质量;
        /// </summary>
        [Name("施力模式")]
        [Tip("Force(持续力)，Impulse(脉冲力)，VelocityChange(速度)和Acceleration(加速度);当类型为VelocityChange(速度)和Acceleration(加速度)时忽略刚体质量;", "Force, impulse, velocity change and acceleration; Ignore the rigid body mass when the types are velocity change and acceleration;")]
        public ForceMode _forceMode = ForceMode.VelocityChange;

        /// <summary>
        /// 持续力:持续在刚体上施加作用力
        /// </summary>
        [Name("持续力")]
        [Tip("持续在刚体上施加作用力", "Continuously apply force to the rigid body")]
        [HideInSuperInspector(nameof(_forceMode), EValidityCheckType.NotEqual, ForceMode.Force)]
        [OnlyMemberElements]
        public PositiveFloatPropertyValue _force = new PositiveFloatPropertyValue(500);

        /// <summary>
        /// 脉冲力:瞬时在刚体上施加作用力
        /// </summary>
        [Name("脉冲力")]
        [Tip("瞬时在刚体上施加作用力", "Momentarily exert a force on a rigid body")]
        [HideInSuperInspector(nameof(_forceMode), EValidityCheckType.NotEqual, ForceMode.Impulse)]
        [OnlyMemberElements]
        public PositiveFloatPropertyValue _impulse = new PositiveFloatPropertyValue(500);

        /// <summary>
        /// 速度:设置刚体的速度；忽略刚体质量
        /// </summary>
        [Name("速度")]
        [Tip("设置刚体的速度；忽略刚体质量", "Set the speed of the rigid body; Ignore rigid body mass")]
        [HideInSuperInspector(nameof(_forceMode), EValidityCheckType.NotEqual, ForceMode.VelocityChange)]
        [OnlyMemberElements]
        public PositiveFloatPropertyValue _velocity = new PositiveFloatPropertyValue(10);

        /// <summary>
        /// 加速度:设置刚体的加速度；忽略刚体质量
        /// </summary>
        [Name("加速度")]
        [Tip("设置刚体的加速度；忽略刚体质量", "Set the acceleration of the rigid body; Ignore rigid body mass")]
        [HideInSuperInspector(nameof(_forceMode), EValidityCheckType.NotEqual, ForceMode.Acceleration)]
        [OnlyMemberElements]
        public PositiveFloatPropertyValue _acceleration = new PositiveFloatPropertyValue(10);

        /// <summary>
        /// 根据施力模式，返回对应值
        /// </summary>
        /// <returns></returns>
        public float GetValue()
        {
            switch (_forceMode)
            {
                case ForceMode.Force: return _force.GetValue();
                case ForceMode.Acceleration: return _acceleration.GetValue();
                case ForceMode.Impulse: return _impulse.GetValue();
                case ForceMode.VelocityChange: return _velocity.GetValue();
                default: return 0;
            }
        }
    }
}
