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
    /// 车辆驾驶器属性设置
    /// </summary>
    [Name(Title, nameof(VehicleDriverPropertySet))]
    [Tip("设置车辆的发动机、手刹、空挡的开关和换档位", "Set the engine, handbrake, neutral switch and shift position of the vehicle")]
    [Owner(typeof(VehicleDriveManger))]
    public class VehicleDriverPropertySet : BasePropertySet<VehicleDriverPropertySet>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "车辆驾驶器属性设置";

        /// <summary>
        /// 创建状态：设置车辆的发动机、手刹、空挡的开关和换档位
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(VehicleDriveHelper.CategoryName, typeof(VehicleDriveManger))]
        [StateComponentMenu(VehicleDriveHelper.CategoryName + "/" + Title, typeof(VehicleDriveManger))]
        [Name(Title, nameof(VehicleDriverPropertySet))]
        [Tip("设置车辆的发动机、手刹、空挡的开关和换档位", "Set the engine, handbrake, neutral switch and shift position of the vehicle")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        [Name("车辆驾驶器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        [EnumPopup]
        public EVehiclePropertyName _propertyName = EVehiclePropertyName.Engine;

        /// <summary>
        /// 属性设置
        /// </summary>
        [Name("发动机启动")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehiclePropertyName.Engine)]
        [OnlyMemberElements]
        public EBoolPropertyValue _engine = new EBoolPropertyValue();

        /// <summary>
        /// 拉手刹
        /// </summary>
        [Name("拉手刹")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehiclePropertyName.Handbrake)]
        [OnlyMemberElements]
        public EBoolPropertyValue _handbrakeInput = new EBoolPropertyValue();

        /// <summary>
        /// 空挡
        /// </summary>
        [Name("空挡")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehiclePropertyName.NGear)]
        [OnlyMemberElements]
        public EBoolPropertyValue _NGear = new EBoolPropertyValue();

        /// <summary>
        /// 档位
        /// </summary>
        [Name("档位")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehiclePropertyName.Gear)]
        public GearNOPropertyValue _gearNO = new GearNOPropertyValue();

        /// <summary>
        /// 车灯类型
        /// </summary>
        [Name("车灯类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehiclePropertyName.Light)]
        public ELightPropertyName _lightPropertyName = ELightPropertyName.LowBeamHead;

        /// <summary>
        /// 属性设置
        /// </summary>
        [Name("属性值")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EVehiclePropertyName.Light)]
        [OnlyMemberElements]
        public EBoolPropertyValue _lightPropertyValue = new EBoolPropertyValue();

        /// <summary>
        /// 档位属性值
        /// </summary>
        [Serializable]
        [Name("档位属性值")]
        public class GearNOPropertyValue : BasePropertyValue<int>
        {
            /// <summary>
            /// 档位
            /// </summary>
            [Name("档位")]
            [Range(1, 8)]
            [HideInSuperInspector(nameof(_propertyValueType), EValidityCheckType.NotEqual, EPropertyValueType.Value)]
            public int _value = default;

            /// <summary>
            /// 档位
            /// </summary>
            public override int propertyValue => _value;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (!DataValidity()) return;

            switch (_propertyName)
            {
                case EVehiclePropertyName.Engine:
                    {
                        switch (_engine.GetValue(Scripts.EBool.None))
                        {
                            case Scripts.EBool.Yes: _vehicleDriver.StartEngine(); break;
                            case Scripts.EBool.No: _vehicleDriver.StopEngine(); break;
                            case Scripts.EBool.Switch: _vehicleDriver.StartOrStopEngine(); break;
                        }
                        break;
                    }
                case EVehiclePropertyName.Handbrake:
                    {
                        switch (_handbrakeInput.GetValue(Scripts.EBool.None))
                        {
                            case Scripts.EBool.Yes: _vehicleDriver.brake.handbrakeOn = true; break;
                            case Scripts.EBool.No: _vehicleDriver.brake.handbrakeOn = false; break;
                            case Scripts.EBool.Switch: _vehicleDriver.brake.handbrakeOn = !_vehicleDriver.brake.handbrakeOn; break;
                        }
                        break;
                    }
                case EVehiclePropertyName.NGear:
                    {
                        switch (_NGear.GetValue(Scripts.EBool.None))
                        {
                            case Scripts.EBool.Yes: _vehicleDriver.gearBox.NGear = true; break;
                            case Scripts.EBool.No: _vehicleDriver.gearBox.NGear = false; break;
                            case Scripts.EBool.Switch: _vehicleDriver.gearBox.NGear = !_vehicleDriver.gearBox.NGear; break;
                        }
                        break;
                    }
                case EVehiclePropertyName.Gear:
                    {
                        _vehicleDriver.gearBox.ShiftTo(_gearNO.GetValue());
                        break;
                    }
                case EVehiclePropertyName.Light:
                    {
                        SetLightValue();
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _vehicleDriver;
        }

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _propertyName == EVehiclePropertyName.Light? CommonFun.Name(_lightPropertyName) : CommonFun.Name(_propertyName);
        }

        private void SetLightValue()
        {
            var lightControl = _vehicleDriver.lightController;
            if (lightControl == null) return;
            var lightValue = _lightPropertyValue.GetValue();

            switch (_lightPropertyName)
            {
                case ELightPropertyName.LowBeamHead:
                    {
                        SetLightState(ELightType.LowBeamHead, lightValue);
                        break;
                    }
                case ELightPropertyName.HighBeamHead:
                    {
                        SetLightState(ELightType.HighBeamHead, lightValue);
                        break;
                    }
                case ELightPropertyName.Fog:
                    {
                        SetLightState(ELightType.Fog, lightValue);
                        break;
                    }
                case ELightPropertyName.LeftIndicator:
                    {
                        SetLightState(ELightType.LeftIndicator, lightValue);
                        break;
                    }
                case ELightPropertyName.RightIndicator:
                    {
                        SetLightState(ELightType.RightIndicator, lightValue);
                        break;
                    }
                case ELightPropertyName.LeftRightIndicator:
                    {
                        SetLightState(ELightType.AllIndicator, lightValue);
                        break;
                    }
            }
        }

        private void SetLightState(ELightType lightType, Scripts.EBool lightState)
        {
            var lightControl = _vehicleDriver.lightController;
            if (lightControl == null) return;

            switch (lightState)
            {
                case Scripts.EBool.Yes: lightControl.SetLightState(lightType, true); break;
                case Scripts.EBool.No: lightControl.SetLightState(lightType, false); break;
                case Scripts.EBool.Switch: lightControl.SwitchLightState(lightType); break;
            }
        }


        /// <summary>
        /// 车辆控制器属性名称
        /// </summary>
        [Name("车辆属性名称")]
        public enum EVehiclePropertyName
        {
            [Name("发动机启动")]
            Engine,

            [Name("拉手刹")]
            Handbrake,

            [Name("设置空挡")]
            NGear,

            [Name("档位")]
            Gear,

            [Name("灯光")]
            Light,
        }

        /// <summary>
        /// 设置车辆灯光类型
        /// </summary>
        [Name("灯光属性名称")]
        public enum ELightPropertyName
        {
            [Name("近光灯")]
            LowBeamHead,

            [Name("远光灯")]
            HighBeamHead,

            [Name("雾灯")]
            Fog,

            [Name("左转灯")]
            LeftIndicator,

            [Name("右转灯")]
            RightIndicator,

            [Name("双闪灯")]
            LeftRightIndicator
        }
    }

}
