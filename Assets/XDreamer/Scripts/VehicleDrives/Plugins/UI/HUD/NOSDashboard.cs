using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Dashboards;
using XCSJ.PluginVehicleDrive.DriveAssists;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 氮氧增压仪表盘
    /// </summary>
    [Name("氮氧增压仪表盘")]
    public class NOSDashboard : Dashboard
    {
        /// <summary>
        /// 氮氧增压系统
        /// </summary>
        [Name("氮氧增压系统")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NOS _NOS = null;

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool dataValid => _NOS;

        /// <summary>
        /// 指针角度
        /// </summary>
        public override float needleAngle => _NOS.percent;

        /// <summary>
        /// 重置数据
        /// </summary>
        protected override void ResetData()
        {
            base.ResetData();

            _rotationAngleMultiplier = -270/100;
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (!_NOS)
            {
                var hud = GetComponentInParent<VehicleHUD>();
                if (hud && hud.vehicleController)
                {
                    _NOS = hud.vehicleController.GetComponentInChildren<NOS>();
                }
            }
            _valid = _valid && _NOS;
        }
    }
}
