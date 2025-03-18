using UnityEngine;
using System;
using System.Collections;
using XCSJ.Attributes;
using System.Collections.Generic;
using XCSJ.PluginTools;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
	/// <summary>
	/// 车灯控制器：控制近光灯、远光灯、雾灯、刹车灯、倒车灯和指示灯的亮灭与闪烁
	/// </summary>
	[DisallowMultipleComponent]
	[Name("车灯控制器")]
	[Tip("控制近光灯、远光灯、雾灯、刹车灯、倒车灯和指示灯的亮灭与闪烁", "Control the on, off and flashing of low beam lamp, high beam lamp, fog lamp, brake lamp, reversing lamp and indicator lamp")]
    [XCSJ.Attributes.Icon(EIcon.VehicleLight)]
    [Tool(VehicleDriveHelper.CategoryComponentName)]
    [RequireManager(typeof(VehicleDriveManger))]
    public class VehicleLightController : VehicleDriverGetter, IVehicleLightController
	{
		/// <summary>
		/// 近光灯状态
		/// </summary>
		public bool lowBeamHeadLightsOn 
		{ 
			get => _lowBeamHeadLightsOn; 
			set 
			{
                if (_lowBeamHeadLightsOn != value)
                {
                    _lowBeamHeadLightsOn = value;
                    VehicleDriver.SendVehicleState(vehicleDriver, new LightEventArgs(ELightType.LowBeamHead, value));

                    // 近光灯打开，远光灯关闭
                    if (_lowBeamHeadLightsOn)
                    {
                        highBeamHeadLightsOn = false;
                    }
                }
            } 
		} 
		private bool _lowBeamHeadLightsOn = false;

		/// <summary>
		/// 远光灯状态
		/// </summary>
		public bool highBeamHeadLightsOn
		{
            get => _highBeamHeadLightsOn;
            set
            {
                if (_highBeamHeadLightsOn != value)
                {
                    _highBeamHeadLightsOn = value;
                    VehicleDriver.SendVehicleState(vehicleDriver, new LightEventArgs(ELightType.HighBeamHead, value));

                    // 远光灯打开，近光灯关闭
                    if (_highBeamHeadLightsOn)
                    {
                        lowBeamHeadLightsOn = false;
                    }
                }
            }
        }
        private bool _highBeamHeadLightsOn = false;

		/// <summary>
		/// 雾灯状态
		/// </summary>
		public bool fogLightOn 
		{ 
			get => _fogLightOn; 
			set
            {
				_fogLightOn = value;

                VehicleDriver.SendVehicleState(vehicleDriver, new LightEventArgs(ELightType.Fog, value));
            }
		}
		private bool _fogLightOn;

		/// <summary>
		/// 刹车灯状态
		/// </summary>
		public bool brakeLightOn { get; private set; } = false;

		/// <summary>
		/// 倒车灯状态
		/// </summary>
		public bool reverseLightOn { get; private set; } = false;

        /// <summary>
        /// 左转灯
        /// </summary>
		public bool leftIndicatorOn
        {
            get => indicatorLight == EIndicatorLight.Left;
            set
            {
                if (leftIndicatorOn != value)
                {
                    indicatorLight = value ? EIndicatorLight.Left : EIndicatorLight.Off;
                }
            }
        }

        /// <summary>
        /// 右转灯
        /// </summary>
        public bool rightIndicatorOn
        {
            get => indicatorLight == EIndicatorLight.Right;
            set
            {
                if (rightIndicatorOn != value)
                {
                    indicatorLight = value ? EIndicatorLight.Right : EIndicatorLight.Off;
                }
            }
        }

        /// <summary>
        /// 双闪灯
        /// </summary>
        public bool allIndicatorOn
        {
            get => indicatorLight == EIndicatorLight.All;
            set
            {
                if (allIndicatorOn != value)
                {
                    indicatorLight = value ? EIndicatorLight.All : EIndicatorLight.Off;
                }
            }
        }

        /// <summary>
        /// 指示灯
        /// </summary>
        public EIndicatorLight indicatorLight
		{ 
			get => _indicatorLight; 
			set
            {
                if (_indicatorLight != value)
                {
                    onIndicatorChanged?.Invoke(_indicatorLight == EIndicatorLight.Off);

					_indicatorLight = value;

                    VehicleDriver.SendVehicleState(vehicleDriver, new LightEventArgs(ELightType.LeftIndicator, leftIndicatorOn));
                    VehicleDriver.SendVehicleState(vehicleDriver, new LightEventArgs(ELightType.RightIndicator, rightIndicatorOn));
                    VehicleDriver.SendVehicleState(vehicleDriver, new LightEventArgs(ELightType.AllIndicator, allIndicatorOn));
                }
			}
		}
		private EIndicatorLight _indicatorLight = EIndicatorLight.Off;

        /// <summary>
        /// 指示灯开关回调
        /// </summary>
        public event Action<bool> onIndicatorChanged = null;

        protected override void Awake()
        {
            base.Awake();

			vehicleDriver.lightController = this;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
	    {
			brakeLightOn = vehicleDriver.brakeValue >= .1f;

			reverseLightOn = vehicleDriver.direction == -1;
        }

        /// <summary>
        /// 获取灯光开关
        /// </summary>
        /// <param name="lightType"></param>
        public bool GetLightState(ELightType lightType)
        {
            switch (lightType)
            {
                case ELightType.LowBeamHead: return lowBeamHeadLightsOn;
                case ELightType.HighBeamHead: return highBeamHeadLightsOn;
				case ELightType.Brake: return brakeLightOn;
				case ELightType.Reverse: return reverseLightOn;
                case ELightType.LeftIndicator: return leftIndicatorOn;
                case ELightType.RightIndicator: return rightIndicatorOn;
                case ELightType.Fog: return fogLightOn;
                case ELightType.AllIndicator: return allIndicatorOn;
            }
			return false;
        }

		/// <summary>
		/// 设置灯光开关
		/// </summary>
		/// <param name="lightType"></param>
		/// <param name="isOn"></param>
		public void SetLightState(ELightType lightType, bool isOn)
        {
            switch (lightType)
            {
                case ELightType.LowBeamHead: lowBeamHeadLightsOn = isOn; break;
                case ELightType.HighBeamHead: highBeamHeadLightsOn = isOn; break;
				case ELightType.Brake: brakeLightOn = isOn; break;
				case ELightType.Reverse: reverseLightOn = isOn; break;
				case ELightType.LeftIndicator: leftIndicatorOn = isOn; break;
				case ELightType.RightIndicator: rightIndicatorOn = isOn; break;
				case ELightType.Fog: fogLightOn = isOn; break;
                case ELightType.AllIndicator: allIndicatorOn = isOn; break;
            }
        }

        /// <summary>
        /// 设置灯光开关
        /// </summary>
        /// <param name="lightType"></param>
        public void SwitchLightState(ELightType lightType)
        {
			SetLightState(lightType, !GetLightState(lightType));
        }
	}
}
