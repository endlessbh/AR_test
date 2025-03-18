using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Dashboards;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 速度仪表盘
    /// </summary>
    [Name("速度仪表盘")]
    public class SpeedDashboard : Dashboard
    {
        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        [Name("车辆驾驶器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 指针角度
        /// </summary>
        public override float needleAngle => _vehicleDriver.speed;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void ResetData()
        {
            base.ResetData();

            _unit = "KMH";
            _rotationAngleMultiplier = -1;
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
            _valid = _valid && _vehicleDriver;
        }
    }
}
