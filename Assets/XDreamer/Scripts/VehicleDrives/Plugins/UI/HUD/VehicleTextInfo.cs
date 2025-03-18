using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 车辆信息文本
    /// </summary>

    [Name("车辆文本信息")]
    public class VehicleTextInfo : VehicleHUDGetter
    {
        /// <summary>
        /// 车辆信息类型
        /// </summary>
        [Name("车辆信息类型")]
        [EnumPopup]
        public EVehicleControllerInfo _vehicleControllerInfo = EVehicleControllerInfo.Speed;

        /// <summary>
        /// 小数点位数
        /// </summary>
        [Name("小数点位数")]
        [Range(0, 10)]
        [HideInSuperInspector(nameof(_vehicleControllerInfo), EValidityCheckType.NotEqual | EValidityCheckType.Or, EVehicleControllerInfo.Speed, nameof(_vehicleControllerInfo), EValidityCheckType.NotEqual, EVehicleControllerInfo.RPM)]
        public int _decimalPointDigit = 0;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _text;

        /// <summary>
        /// 附加单位
        /// </summary>
        [Name("附加字符串")]
        public string _unit;

        /// <summary>
        /// 车辆控制信息类型
        /// </summary>
        [Name("车辆控制信息类型")]
        public enum EVehicleControllerInfo
        {
            [Name("速度")]
            Speed,

            [Name("转速")]
            RPM,

            [Name("档位")]
            Gear
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            _text = GetComponent<Text>();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            _vehicleDriver = vehicleDriver;
        }

        private VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 延迟更新
        /// </summary>
        protected void LateUpdate()
        {
            if (!_vehicleDriver || !_text) return;

            string info = "";
            switch (_vehicleControllerInfo)
            {
                case EVehicleControllerInfo.Speed:
                    {
                        info = _vehicleDriver.speed.ToString("F" + _decimalPointDigit.ToString());
                        break;
                    }
                case EVehicleControllerInfo.RPM:
                    {
                        if (_vehicleDriver.engine!=null)
                        {
                            info = _vehicleDriver.engine.RPM.ToString("F" + _decimalPointDigit.ToString());
                        }
                        break;
                    }
                case EVehicleControllerInfo.Gear:
                    {
                        if (_vehicleDriver.gearBox != null)
                        {
                            if (!_vehicleDriver.gearBox.NGear && !_vehicleDriver.gearBox.changingGear)
                            {
                                info = _vehicleDriver.direction == 1 ? (_vehicleDriver.gearBox.currentGearIndex + 1).ToString() : "R";
                            }
                            else
                            {
                                info = "N";
                            }
                        }
                        break;
                    }
                default:
                    break;
            }
            info += _unit;
            _text.text = info;
        }
    }
}
