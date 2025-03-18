using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 车辆动力推进
    /// </summary>
    public interface IVehicleBoost
    {
        bool boost { get; }
    }

    /// <summary>
    /// 汽车氮气加速系统。它可以使车辆引擎在瞬间爆发出强大的动力，从而极大的提高机车的行驶速度。
    /// 氮气加速系统使用的气体是一氧化二氮（N2O），俗称"笑气"。常温下稳定，高温下分解成氮气和氧气，有助燃作用。
    /// </summary>
    [Name("氮氧增压系统")]
    [DisallowMultipleComponent]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class NOS : VehicleDriverGetter, IVehicleBoost
    {
        /// <summary>
        /// 开启氮氧增压
        /// </summary>
        public bool boost { get; set; }

        /// <summary>
        /// 容量
        /// </summary>
        [Name("容量")]
        [Range(1, 1000)]
        public float _capacity = 174f;

        /// <summary>
        /// 当前量
        /// </summary>
        [Name("当前量")]
        public float _amount = 174f;

        /// <summary>
        /// 消耗率
        /// </summary>
        [Name("消耗率")] 
        public float _consumeRate = 0.1f;

        [Name("动能增加倍数")]
        [Range(1,100)]
        public float _increatePowerMultipler = 1.5f;

        /// <summary>
        /// NOS 工作状态回调
        /// </summary>
        public event Action<bool> onNOSWork = null;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            UpdatePercent();
        }

        /// <summary>
        /// NOS.
        /// </summary>
        protected override void OnFixedUpdate()
        {
            if (boost && _amount > 0)
            {
                _amount -= _consumeRate * Time.fixedDeltaTime;
                UpdatePercent();

                onNOSWork?.Invoke(true);

                vehicleDriver.boostValue = _increatePowerMultipler;
            }
            else
            {
                vehicleDriver.boostValue = 1;

                onNOSWork?.Invoke(false);
            }
        }

        /// <summary>
        /// 氮氧剩余百分比
        /// </summary>
        public float percent { get; protected set; }

        private void UpdatePercent()
        {
            percent = _amount / _capacity;
        }
    }
}
