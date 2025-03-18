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
    /// 车辆控制器状态触发器
    /// </summary>
    [Name(Title, nameof(VehicleControllerStateTrigger))]
    [Tip("用于车辆状态改变触发器", "Trigger for vehicle state change")]
    public class VehicleControllerStateTrigger : Trigger<VehicleControllerStateTrigger>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "车辆控制器状态触发器";

        /// <summary>
        /// 创建状态：用于车辆状态改变触发器
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //[StateLib(VehicleDriveHelper.CategoryName, typeof(VehicleDriveManger))]
        //[StateComponentMenu(VehicleDriveHelper.CategoryName + "/" + Title, typeof(VehicleDriveManger))]
        [Name(Title, nameof(VehicleControllerStateTrigger))]
        [Tip("用于车辆状态改变触发器", "Trigger for vehicle state change")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 车辆控制器
        /// </summary>
        [Name("车辆控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleController _vehicleController = null;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EVehicleStatePropertyName _propertyName = EVehicleStatePropertyName.State;

        /// <summary>
        /// 车辆状态
        /// </summary>
        [Name("车辆状态")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehicleStatePropertyName.State)]
        public EVehicleStatePropertyValue _stateProperty = new EVehicleStatePropertyValue();

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            //VehicleController.onVehicleControllerStateChanged += OnVehicleControllerStateChanged;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            //VehicleController.onVehicleControllerStateChanged -= OnVehicleControllerStateChanged;

            base.OnExit(data);
        }

        private void OnVehicleControllerStateChanged(VehicleController vehicleController, VehicleStateEventArgs vehicleStateEventArg)
        {
            if (_vehicleController && _vehicleController == vehicleController)
            {
                finished = _stateProperty.propertyValue == vehicleStateEventArg.vehicleState;
            }
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_stateProperty.propertyValue);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _vehicleController;
        }
    }
}
