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
    /// 车辆UI显示
    /// </summary>
    [Name("车辆显示系统")]
    public class VehicleHUD : RootWindow
    {
        /// <summary>
        /// 车辆控制器
        /// </summary>
        public VehicleController vehicleController
        {
            get
            {
                if (!_vehicleController)
                {
                    _vehicleController = GetComponentInParent<VehicleController>();
                }
                return _vehicleController;
            }
        }

        /// <summary>
        /// 车辆控制器
        /// </summary>
        [Name("车辆控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleController _vehicleController;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (!vehicleController)
            {
                Debug.LogErrorFormat("未关联{0}!", CommonFun.Name(typeof(VehicleController)));
            }
        }
    }


    /// <summary>
    /// 车辆显示系统获取器
    /// </summary>
    public abstract class VehicleHUDGetter : View
    {
        /// <summary>
        /// 车辆显示系统
        /// </summary>
        [Name("车辆显示系统")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleHUD _vehicleHUD = null;

        /// <summary>
        /// 车辆显示系统 
        /// </summary>
        public VehicleHUD vehicleHUD => this.XGetComponentInParent<VehicleHUD>(ref _vehicleHUD);

        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        public VehicleDriver vehicleDriver
        {
            get
            {
                if (vehicleHUD && vehicleHUD.vehicleController)
                {
                    return vehicleHUD.vehicleController.vehicleDriver;
                }
                return null;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (vehicleHUD) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (!vehicleHUD)
            {
                Debug.LogErrorFormat("未关联{0}!", CommonFun.Name(typeof(VehicleHUD)));
            }
        }
    }
}
