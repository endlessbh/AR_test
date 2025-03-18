using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Models;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{

	/// <summary>
	/// 能源模拟
	/// 用于模拟油箱或动力是否足够
	/// </summary>
	[DisallowMultipleComponent]
    [Name("燃料")]
    [XCSJ.Attributes.Icon(EIcon.Fuel)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class Fuel : VehicleDriverGetter, IFuel
	{
		/// <summary>
		/// 容量 ：单位为升(L)
		/// </summary>
		[Name("容量")]
		[Tip("单位为升(L)", "Unit: liter (L)")]
		[Range(1, 1000)]
	    public float _capacity = 60f;
        public float capacity { get => _capacity; }

		/// <summary>
		/// 当前量 ：单位为升(L)
		/// </summary>
		[Name("剩余量")]
        [Tip("单位为升(L)", "Unit: liter (L)")]
		[Min(0)]
        public float _amount = 60f;

		public float amount { get => _amount; }

		/// <summary>
		/// 消耗率 ：每秒燃料消耗率，与转速RPM成正比
		/// </summary>
		[Name("消耗率")]
		[Tip("每秒燃料消耗率，与转速RPM成正比", "每秒燃料消耗率，与转速RPM成正比")]
        public float _consumptionRate = .1f;

		/// <summary>
		/// 燃料剩余百分比
		/// </summary>
        public float percent { get; private set; }

		/// <summary>
		/// 启用
		/// </summary>
		protected override void Awake()
	    {
			base.Awake();

			vehicleDriver.fuel = this;

	        _amount = Mathf.Clamp(_amount, 0f, _capacity);

			UpdatePercent();
		}
	
	    /// <summary>
	    /// 更新
	    /// </summary>
	    protected override void OnFixedUpdate()
	    {
	        if (vehicleDriver.engine!=null && _amount > 0)
	        {
	            _amount -= vehicleDriver.engine.RPM / 10000f * _consumptionRate * Time.fixedDeltaTime;
				if (_amount < 0) _amount = 0;
	        }

			UpdatePercent();
		}

		/// <summary>
		/// 燃料提供的动力
		/// </summary>
		/// <returns></returns>
		public float GetPower()
        {
            return _amount > 0 ? 1 : 0;
        }

		private void UpdatePercent()
        {
			percent = _amount / _capacity;
		}

		/// <summary>
		/// 加油
		/// </summary>
		/// <param name="amount"></param>
		public void Add(float amount)
        {
			_amount += amount;
            if (_amount > _capacity)
            {
                _amount = _capacity;
            }
        }
	}
}