using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Dashboards;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 转速仪表盘
    /// </summary>
    [Name("转速仪表盘")]
    public class RPMDashboard : WarmingDashboard
    {
        /// <summary>
        /// 车辆控制
        /// </summary>
        [Name("车辆控制")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool dataValid => _vehicleDriver;

        /// <summary>
        /// 指针角度
        /// </summary>
        public override float needleAngle => _vehicleDriver.engine.RPM;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void ResetData()
        {
            base.ResetData();

            _unit = "RPM";
            _rotationAngleMultiplier = -1.0f / 50;
            _warmingValue = 6000;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (!_vehicleDriver)
            {
                var hud = GetComponentInParent<VehicleHUD>();
                if (hud && hud.vehicleController)
                {
                    _vehicleDriver = hud.vehicleController.vehicleDriver;
                }
            }
            _valid = _valid && _vehicleDriver && _vehicleDriver.engine!=null;
        }
    }
}
