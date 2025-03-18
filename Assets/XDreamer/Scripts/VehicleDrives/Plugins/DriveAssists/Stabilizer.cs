using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 二轮平衡器：用于维持只有两个轮的车辆平衡
    /// </summary>
    [Name("二轮平衡器")]
    [Tip("用于维持只有两个轮的车辆平衡", "Used to maintain the balance of a vehicle with only two wheels")]
    [XCSJ.Attributes.Icon(EIcon.VehicleAxis)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    public class Stabilizer : VehicleDriverGetter
    {
        /// <summary>
        /// 前轮
        /// </summary>
        [Name("前轮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _frontWheel;

        /// <summary>
        /// 后轮
        /// </summary>
        [Name("后轮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Readonly(EEditorMode.Runtime)]
        public WheelDriver _rearWheel;

        /// <summary>
        /// 纠正扭矩系数：在固定更新中对车辆刚体施加一个以刚体朝向为旋转轴的扭矩
        /// </summary>
        [Name("纠正扭矩系数")]
        [Tip("在固定更新中对车辆刚体施加一个以刚体朝向为旋转轴的扭矩", "In the fixed update, a torque with the orientation of the rigid body as the rotation axis is applied to the vehicle rigid body")]
        public float _torqueMultiplier = 1;

        /// <summary>
        /// 转向系数
        /// </summary>
        [Name("转向系数")]
        public float _steerMultiplier = -1;

        /// <summary>
        /// 固定更新
        /// </summary>
        protected override void OnFixedUpdate()
        {
            if (!_frontWheel || !_rearWheel) return;

            var torqueForce = Vector3.Cross(transform.up, Vector3.up) * 50;
            torqueForce.x = torqueForce.x * 0.4f;
            torqueForce -= vehicleDriver.rigid.angularVelocity;
            vehicleDriver.rigid.AddTorque(torqueForce * vehicleDriver.rigid.mass * 0.02f * _torqueMultiplier, ForceMode.Impulse);

            float rpmSign = Mathf.Sign(vehicleDriver.engine.RPM) * 0.02f;
            if (vehicleDriver.rigid.velocity.magnitude > 1.0f && _frontWheel.isGrounded && _rearWheel.isGrounded)
            {
                vehicleDriver.rigid.angularVelocity += new Vector3(0, vehicleDriver.steerInput * rpmSign * _steerMultiplier, 0);
            }
        }
    }
}