using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.States
{
    /// <summary>
    /// 车辆控制器属性设置
    /// </summary>
    [Name(Title, nameof(VehicleControllerPropertySet))]
    [Tip("设置车辆状态属性")]
    public class VehicleControllerPropertySet : BasePropertySet<VehicleControllerPropertySet>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "车辆控制器属性设置";

        /// <summary>
        /// 创建状态：设置车辆状态属性
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //[StateLib(VehicleDriveHelper.CategoryName, typeof(VehicleDriveManger))]
        //[StateComponentMenu(VehicleDriveHelper.CategoryName + "/" + Title, typeof(VehicleDriveManger))]
        [Name(Title, nameof(VehicleControllerPropertySet))]
        [Tip("设置车辆状态属性")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 车辆控制器
        /// </summary>
        [Name("车辆控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleController _vehicleDriver = null;

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
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            switch (_propertyName)
            {
                case EVehicleStatePropertyName.State:
                    {
                        //if (_vehicleDriver)
                        //{
                        //    _vehicleDriver.vehicleState = _stateProperty.propertyValue;
                        //}
                        break;
                    }
            }
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_stateProperty.propertyValue);
        }
    }

    /// <summary>
    /// 车辆状态属性名称
    /// </summary>
    [Name("车辆状态属性名称")]
    public enum EVehicleStatePropertyName
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 状态
        /// </summary>
        [Name("状态")]
        State,
    }


    /// <summary>
    /// 车辆状态属性值
    /// </summary>
    [Serializable]
    [Name("车辆状态属性值")]
    public class EVehicleStatePropertyValue : EnumPropertyValue<EVehicleState> { }
}
