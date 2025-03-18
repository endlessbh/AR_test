using System;
using XCSJ.Attributes;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.States
{
    /// <summary>
    /// 车辆的发动机、手刹、空挡和车灯状态变化触发器
    /// </summary>
    [Name(Title, nameof(VehicleDriverStateTrigger))]
    [Tip("用于车辆中具有开和关两种状态的触发器，例如发动机开关、手刹开关、空挡和车灯开关等", "It is used for triggers with on and off states in the vehicle, such as engine switch, handbrake switch, neutral gear and lamp switch, etc")]
    [Owner(typeof(VehicleDriveManger))]
    public class VehicleDriverStateTrigger : ToggleTrigger<VehicleDriverStateTrigger>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "车辆驾驶器状态触发器";

        /// <summary>
        /// 创建状态：车辆的发动机、手刹、档位和车灯状态变化触发器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VehicleDriveHelper.CategoryName, typeof(VehicleDriveManger))]
        [StateComponentMenu(VehicleDriveHelper.CategoryName + "/" + Title, typeof(VehicleDriveManger))]
        [Name(Title, nameof(VehicleDriverStateTrigger))]
        [Tip("车辆的发动机、手刹、空挡和车灯状态变化触发器", "Trigger for state change of engine, handbrake, neutral gear and lamp of vehicle")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 车辆切换状态
        /// </summary>
        [Name("车辆切换状态")] 
        public VehicleDriverToggleState _vehicleToggleState = new VehicleDriverToggleState();

        /// <summary>
        /// 切换状态
        /// </summary>
        protected override bool toggleState
        {
            get
            {
                return _vehicleToggleState.state;
            }
            set
            {
                _vehicleToggleState.state = value;
            }
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            _vehicleToggleState.RemoveListener(OnValueChanged);
            _vehicleToggleState.AddListener(OnValueChanged);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            _vehicleToggleState.RemoveListener(OnValueChanged);

            base.OnExit(data);
        }

        private void OnValueChanged(bool value)
        {
            switch (triggerType)
            {
                case EToggleTriggerType.Switch:
                    {
                        finished = true;
                        break;
                    }
                case EToggleTriggerType.SwitchOn:
                    {
                        if (value) finished = true;
                        break;
                    }
                case EToggleTriggerType.SwitchOff:
                    {
                        if (!value) finished = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _vehicleToggleState.ToFriendlyString();
        }

        /// <summary>
        /// 有效
        /// </summary>
        protected void OnValidate()
        {
            if (parent.isActive)
            {
                _vehicleToggleState.RemoveListener(OnValueChanged);
                _vehicleToggleState.AddListener(OnValueChanged);
            }
        }
    }
}
