using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginVehicleDrive.UI.Inputs
{
    /// <summary>
    /// 车辆UI输入
    /// </summary>
    [Name("车辆UI输入")]
    public class VehicleUIInput : RootWindow
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
    /// 车辆UI输入控制器获取器
    /// </summary>
    public abstract class VehicleUIInputGetter : View
    {
        /// <summary>
        /// 车辆UI输入控制器
        /// </summary>
        [Name("车辆UI输入控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleUIInput _vehicleUIInput = null;

        /// <summary>
        /// 车辆UI输入控制器 
        /// </summary>
        public VehicleUIInput vehicleUIInput => this.XGetComponentInParent<VehicleUIInput>(ref _vehicleUIInput);

        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        public VehicleDriver vehicleDriver
        {
            get
            {
                if (vehicleUIInput && vehicleUIInput.vehicleController)
                {
                    return vehicleUIInput.vehicleController.vehicleDriver;
                }
                return null;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (vehicleUIInput) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (!vehicleUIInput)
            {
                Debug.LogErrorFormat("未关联{0}!", CommonFun.Name(typeof(VehicleUIInput)));
            }
        }
    }
}
