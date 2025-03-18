using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginVehicleDrive.States
{
    /// <summary>
    /// 车辆驾驶器属性比较
    /// </summary>
    [Name(Title, nameof(VehicleDriverPropertyCompare))]
    [Tip(Tip, "Comparison of attribute values such as vehicle direction, speed, speed, fuel volume, wheel angle and steering wheel angle")]
    [XCSJ.Attributes.Icon(EIcon.Car)]
    [Owner(typeof(VehicleDriveManger))]
    public class VehicleDriverPropertyCompare : BasePropertyCompare<VehicleDriverPropertyCompare>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "车辆驾驶器属性比较";

        /// <summary>
        /// 提示
        /// </summary>
        public const string Tip = "车辆的方向, 速度, 转速, 油量, 车轮转角和方向盘转角等属性值比较";

        /// <summary>
        /// 创建状态：车辆的方向, 速度, 转速, 油量, 车轮转角和方向盘转角等属性值比较
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VehicleDriveHelper.CategoryName, typeof(VehicleDriveManger))]
        [Name(Title, nameof(VehicleDriverPropertyCompare))]
        [StateComponentMenu(VehicleDriveHelper.CategoryName + "/" + Title, typeof(VehicleDriveManger))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip(Tip, "Comparison of attribute values such as vehicle direction, speed, speed, fuel volume, wheel angle and steering wheel angle")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        [Name("车辆驾驶器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 车辆属性名称
        /// </summary>
        [Name("车辆属性名称")]
        [EnumPopup]
        public EPropertyName _vehiclePropertyName = EPropertyName.Direction;

        /// <summary>
        /// 数值比较触发器
        /// </summary>
        [Name("数值比较触发器")]
        public FloatValueTrigger _numberValueTrigger = new FloatValueTrigger();

        [Name("完成一次")]
        [Tip("勾选时：条件成立即为完成状态；不勾选时：完成状态随着值不断变化", "When checked: the status is completed when the condition is established; If unchecked: the completion status changes with the value")]
        public bool finishOnce = true;

        /// <summary>
        /// 完成态
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            if (!_vehicleDriver) return false;

            if (finishOnce && finished)
            {
                return true;
            }
            finished = IsTrigger();

            return finished;
        }

        private bool IsTrigger()
        {
            switch (_vehiclePropertyName)
            {
                case EPropertyName.Direction: return _numberValueTrigger.IsTrigger(_vehicleDriver.direction);
                case EPropertyName.Speed: return _numberValueTrigger.IsTrigger(_vehicleDriver.speed);
                case EPropertyName.RPM:
                    {
                        if (_vehicleDriver.engine != null)
                        {
                            return _numberValueTrigger.IsTrigger(_vehicleDriver.engine.RPM);
                        }
                        break;
                    }
                case EPropertyName.FuelPercent:
                    {
                        if (_vehicleDriver.fuel != null)
                        {
                            return _numberValueTrigger.IsTrigger(_vehicleDriver.fuel.percent);
                        }
                        break;
                    }
                case EPropertyName.SteerAngle:
                    {
                        if (_vehicleDriver.steer != null)
                        {
                            return _numberValueTrigger.IsTrigger(_vehicleDriver.steer.steerAngle);
                        }
                        break;
                    }
                case EPropertyName.SteerWheelAngle:
                    {
                        if (_vehicleDriver.steer != null)
                        {
                            return _numberValueTrigger.IsTrigger(_vehicleDriver.steer.steerWheelAngle);
                        }
                        break;
                    }
            }
            return false;
        }

        public override bool DataValidity()
        {
            return _vehicleDriver;
        }

        /// <summary>
        /// 车辆属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 方向
            /// </summary>
            [Name("方向")]
            Direction,

            /// <summary>
            /// 速度
            /// </summary>
            [Name("速度")]
            Speed,

            /// <summary>
            /// 转速
            /// </summary>
            [Name("转速")]
            RPM,

            /// <summary>
            /// 油量
            /// </summary>
            [Name("油量")]
            FuelPercent,

            /// <summary>
            /// 车轮转角
            /// </summary>
            [Name("车轮转角")]
            SteerAngle,

            /// <summary>
            /// 方向盘转角
            /// </summary>
            [Name("方向盘转角")]
            SteerWheelAngle,
        }
    }

}
