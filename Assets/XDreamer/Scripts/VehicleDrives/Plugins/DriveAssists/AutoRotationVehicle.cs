
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
	/// 翻车重置：当车辆Z轴超过设定角度时自动翻转车体
	/// </summary>
	[DisallowMultipleComponent]
	[Name("翻车重置")]
	[Tip("当车辆Z轴超过设定角度时自动翻转车体", "When the z-axis of the vehicle exceeds the set angle, the vehicle body will be turned over automatically")]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class AutoRotationVehicle : VehicleDriverGetter
	{
		/// <summary>
		/// 重置时间
		/// </summary>
		[Range(0, 1000)]
        [Name("重置时间")]
        public float _resetTime = 3;

		/// <summary>
		/// 重置Y偏移值
		/// </summary>
		[Name("翻转后高度偏移值")]
		public float _resetOffsetY = 3;

		/// <summary>
		/// 车辆速度：当车辆速度低于当前速度时，才会触发翻转
		/// </summary>
		[Name("车辆速度")]
        [Tip("当车辆速度低于当前速度时，才会触发翻转", "When the vehicle speed is lower than the current speed, it will be triggered")]
		[Range(0, 100)]
        public float _lowSpeed = 5;

		/// <summary>
		/// 正向角度：当车辆的朝向轴(Z轴)不在这个角度区间时，认为翻车
		/// </summary>
		[Name("正向角度")]
		[Tip("当车辆的朝向轴(Z轴)不在这个角度区间时，认为翻车", "When the vehicle's orientation axis (Z axis) is not in this angle range, it is considered to be overturned")]
        [LimitRange(-360, 360)]
        public Vector2 _angleRange = new Vector2(-45, 45);
	
	    private float timeCounter = 0;
	
	    /// <summary>
	    /// 重置车辆 ：对翻车情况进行旋转重置
	    /// </summary>
	    protected override void OnFixedUpdate()
	    {
			var t = vehicleDriver.transform;

			if (vehicleDriver.speed < _lowSpeed && t.eulerAngles.z < _angleRange.x && t.eulerAngles.z > _angleRange.y)
	        {
	            timeCounter += Time.deltaTime;
	            if (timeCounter > _resetTime)
	            {
					t.rotation = Quaternion.Euler(0f, t.eulerAngles.y, 0f);
					t.position = new Vector3(t.position.x, t.position.y + _resetOffsetY, t.position.z);
	                timeCounter = 0f;
	            }
	        }
	    }
	}
}