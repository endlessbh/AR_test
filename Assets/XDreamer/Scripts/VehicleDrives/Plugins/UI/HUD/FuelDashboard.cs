using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Dashboards;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 燃料仪表盘
    /// </summary>
    [Name("燃料仪表盘")]
    public class FuelDashboard : WarmingDashboard
    {
        /// <summary>
        /// 燃料
        /// </summary>
        [Name("燃料对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Fuel _fuel = null;

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool dataValid => _fuel;

        /// <summary>
        /// 指针角度
        /// </summary>
        public override float needleAngle => _fuel.percent;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void ResetData()
        {
            base.ResetData();

            _unit = "L";
            _rotationAngleMultiplier = -270;
            _warmingValue = 0.1f; 
            _warmingCompareRule = EWarmingCompareRule.Less;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (!_fuel)
            {
                var hud = GetComponentInParent<VehicleHUD>();
                if (hud && hud.vehicleController)
                {
                    _fuel = hud.vehicleController.GetComponentInChildren<Fuel>();
                }
            }
            _valid = _valid && _fuel;
        }
    }
}
