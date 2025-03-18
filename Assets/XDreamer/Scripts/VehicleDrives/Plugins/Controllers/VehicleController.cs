using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.DriveAssists;
using XCSJ.PluginVehicleDrive.UI.Inputs;
using XCSJ.PluginVehicleDrive.UI.HUD;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 车辆控制器 ：车辆控制的主对象，由驾驶器、显示系统、UI输入控制器和划痕管理器对象构成
    /// </summary>
    [Name(Title)]
    [Tip("车辆控制的主对象，由驾驶器、显示系统、UI输入控制器和划痕管理器对象构成", "The main object of vehicle control is composed of driver, display system, UI input controller and scratch manager object")]
    [XCSJ.Attributes.Icon(EIcon.Car)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    [AddComponentMenu(VehicleDriveHelper.ComponentMenuPath + Title)]
    public class VehicleController : BaseVehicle
    {
        public const string Title = "车辆控制器";

        ///// <summary>
        ///// 车辆状态
        ///// </summary>
        //[Name("车辆状态")]
        //[EnumPopup]
        //public EVehicleState _vehicleState = EVehicleState.Empty;

        ///// <summary>
        ///// 车辆状态
        ///// </summary>
        //public EVehicleState vehicleState
        //{
        //    get => _vehicleState;
        //    set
        //    {
        //        if (_vehicleState!= value)
        //        {
        //            _vehicleState = value;
        //            onVehicleControllerStateChanged?.Invoke(this, new VehicleStateEventArgs(value));
        //        }

        //        switch (_vehicleState)
        //        {
        //            case EVehicleState.Empty:
        //                {
        //                    ActiveVehicle(false, true);
        //                    break;
        //                }
        //            case EVehicleState.PlayerDrive:
        //                {
        //                    ActiveVehicle(true);
        //                    break;
        //                }
        //            case EVehicleState.OtherDrive:
        //                {
        //                    ActiveVehicle(false);
        //                    break;
        //                }
        //        }

        //        // 激活对象列表
        //        _activeObjectWhenPlayerDrive.ForEach(go =>
        //        {
        //            if (go)
        //            {
        //                go.SetActive(_vehicleState == EVehicleState.PlayerDrive);
        //            }
        //        });
        //    }
        //}

        /// <summary>
        /// 车辆控制器状态改变回调函数
        /// </summary>
        //public static event Action<VehicleController, VehicleStateEventArgs> onVehicleControllerStateChanged = null;

        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        [Name("车辆驾驶器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 车辆驾驶器
        /// </summary>
        public VehicleDriver vehicleDriver
        {
            get
            {
                if (!_vehicleDriver)
                {
                    _vehicleDriver = GetComponentInChildren<VehicleDriver>();
                }
                return _vehicleDriver;
            }
        }

        /// <summary>
        /// 车辆显示系统
        /// </summary>
        [Name("车辆显示系统")]
        public VehicleHUD _vehicleHUD = null;

        /// <summary>
        /// 车辆显示系统
        /// </summary>
        public VehicleHUD vehicleHUD
        {
            get
            {
                if (!_vehicleHUD)
                {
                    _vehicleHUD = GetComponentInChildren<VehicleHUD>();
                }
                return _vehicleHUD;
            }
        }

        /// <summary>
        /// 车辆UI输入控制器
        /// </summary>
        [Name("车辆UI输入控制器")]
        public VehicleUIInput _vehicleUIInput = null;

        /// <summary>
        /// 车辆显示系统
        /// </summary>
        public VehicleUIInput vehicleUIInput
        {
            get
            {
                if (!_vehicleUIInput)
                {
                    _vehicleUIInput = GetComponentInChildren<VehicleUIInput>();
                }
                return _vehicleUIInput;
            }
        }

        /// <summary>
        /// 车辆划痕管理器
        /// </summary>
        [Name("车辆划痕管理器")]
        public VehicleSkidmark _vehicleSkidmark = null;

        /// <summary>
        /// 车辆显示系统
        /// </summary>
        public VehicleSkidmark vehicleSkidmark
        {
            get
            {
                if (!_vehicleSkidmark)
                {
                    _vehicleSkidmark = GetComponentInChildren<VehicleSkidmark>();
                }
                return _vehicleSkidmark;
            }
        }

        /// <summary>
        /// 玩家驾驶时激活对象列表
        /// </summary>
        [Name("玩家驾驶时激活对象列表")]
        public List<GameObject> _activeObjectWhenPlayerDrive = new List<GameObject>();

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            Init();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            Init();

            if (!_vehicleDriver)
            {
                Debug.LogErrorFormat("缺失必需对象{0}", CommonFun.Name(typeof(VehicleDriver)));
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            Init();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            //vehicleState = _vehicleState;
        }

        private void Init()
        {
            if (vehicleDriver) { }

            if (vehicleHUD) { }

            if (vehicleSkidmark) { }
        }

        private void ActiveVehicle(bool active, bool stopEngine = false)
        {
            if (vehicleDriver)
            {
                if (stopEngine)
                {
                    vehicleDriver.StopEngine();
                }
                vehicleDriver.enabled = active;
            }
            if (vehicleHUD)
            {
                vehicleHUD.gameObject.SetActive(active);
            }
            if (vehicleUIInput)
            {
                vehicleUIInput.gameObject.SetActive(active);
            }
        }
    }

    /// <summary>
    /// 车辆状态
    /// </summary>
    [Name("车辆状态")]
    public enum EVehicleState
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 空车
        /// </summary>
        [Name("空车")]
        Empty,

        /// <summary>
        /// 玩家驾驶
        /// </summary>
        [Name("玩家驾驶")]
        PlayerDrive,

        /// <summary>
        /// 其他驾驶
        /// </summary>
        [Name("其他驾驶")]
        OtherDrive,
    }

    /// <summary>
    /// 车辆控制器获取器
    /// </summary>
    public abstract class VehicleControllerGetter : BaseVehicle
    {
        /// <summary>
        /// 车辆控制器
        /// </summary>
        [Name("车辆控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleController _vehicleController = null;

        /// <summary>
        /// 车辆控制父对象 
        /// </summary>
        public VehicleController vehicleController => this.XGetComponentInParent<VehicleController>(ref _vehicleController);

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (vehicleController) { }
        }

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
}
