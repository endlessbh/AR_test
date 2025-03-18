
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
	/// <summary>
	/// 发动机过热
	/// </summary>
	[DisallowMultipleComponent]
    [Name("发动机热量")]
    [XCSJ.Attributes.Icon(EIcon.Engine)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class EngineHeat : VehicleDriverGetter
	{
        /// <summary>
        /// 水冷阈值
        /// </summary>
        [Name("水冷阈值")]
        public float _coolingWaterThreshold = 60f;

        /// <summary>
        /// 发热系数
        /// </summary>
        [Name("发热系数")]
        public float _heatRate = 1f;

        /// <summary>
        /// 冷却率
        /// </summary>
        [Name("冷却率")]
        public float _coolRate = 1f;                             

        /// <summary>
        /// 冷却
        /// </summary>
        public float heat { get; private set; } = 15f;

        /// <summary>
        /// 防侧倾杆
        /// </summary>
        protected override void OnFixedUpdate()
	    {
            if (vehicleDriver.engine == null) return;

	        var addValue = _heatRate * Time.fixedDeltaTime;
	        heat += vehicleDriver.engine.RPM * addValue / 10000f;
	
	        if (heat > _coolingWaterThreshold) heat -= addValue;
	
	        heat -= addValue / 10f;
	        heat = Mathf.Clamp(heat, 15f, 120f);
	    }
	}
}