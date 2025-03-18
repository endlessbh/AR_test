using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Dashboards;
using XCSJ.PluginVehicleDrive.DriveAssists;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 发动机热量仪表盘
    /// </summary>
    [Name("发动机热量仪表盘")]
    public class EngineHeatDashboard : WarmingDashboard
    {
        /// <summary>
        /// 发动机热量
        /// </summary>
        [Name("发动机热量")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public EngineHeat _engineHeat = null;

        /// <summary>
        /// 指针角度
        /// </summary>
        public override float needleAngle => _engineHeat.heat;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void ResetData()
        {
            base.ResetData();

            _rotationAngleMultiplier = -270 / 110;
            _warmingValue = 100f;
            _warmingCompareRule = EWarmingCompareRule.Greater;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (!_engineHeat)
            {
                var hud = GetComponentInParent<VehicleHUD>();
                if (hud && hud.vehicleController)
                {
                    _engineHeat = hud.vehicleController.GetComponentInChildren<EngineHeat>();
                }
            }
            _valid = _valid && _engineHeat;
        }
    }
}
