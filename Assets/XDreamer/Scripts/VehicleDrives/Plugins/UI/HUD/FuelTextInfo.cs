using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 燃料文本信息
    /// </summary>
    [Name("燃料文本信息")]
    public class FuelTextInfo : View
    {
        /// <summary>
        /// 燃料
        /// </summary>
        [Name("燃料")]
        public Fuel _fuel;

        /// <summary>
        /// 燃料信息类型
        /// </summary>
        [Name("燃料信息类型")]
        [EnumPopup]
        public EFuelInfoType _fuelInfoType = EFuelInfoType.Amount;

        /// <summary>
        /// 文本
        /// </summary>
        [Name("文本")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _text;

        /// <summary>
        /// 小数点位数
        /// </summary>
        [Name("小数点位数")]
        [Range(0, 10)]
        public int _decimalPointDigit = 0;

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            Init();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            Init();
        }

        private void Init()
        {
            if (!_fuel)
            {
                var hud = GetComponentInParent<VehicleHUD>();
                if (hud && hud.vehicleController)
                {
                    _fuel = hud.vehicleController.GetComponentInChildren<Fuel>();
                }
            }

            if (!_text)
            {
                _text = GetComponent<Text>();
            }
        }

        /// <summary>
        /// 延时更新
        /// </summary>
        protected void LateUpdate()
        {
            if (!_fuel || !_text) return;

            float value = 0;
            switch (_fuelInfoType)
            {
                case EFuelInfoType.Amount: value = _fuel._amount; break;
                case EFuelInfoType.Capacity: value = _fuel._capacity; break;
                case EFuelInfoType.Percent: value = _fuel.percent; break;
                case EFuelInfoType.ConsumptionRate: value = _fuel._consumptionRate; break;
                default: break;
            }
            _text.text = value.ToString("F" + _decimalPointDigit);
        }

        /// <summary>
        /// 燃料信息类型
        /// </summary>
        [Name("燃料信息类型")]
        public enum EFuelInfoType
        {
            [Name("剩余量")]
            Amount,

            [Name("容量")]
            Capacity,

            [Name("剩余百分比")]
            Percent,

            [Name("消耗率")]
            ConsumptionRate,
        }
    }
}
