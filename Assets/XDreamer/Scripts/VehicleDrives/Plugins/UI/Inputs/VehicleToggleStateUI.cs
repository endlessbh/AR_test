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
    /// 车辆切换状态UI
    /// </summary>
    [Name("引擎切换器")]
    [RequireComponent(typeof(Toggle))]
    public class VehicleToggleStateUI : VehicleUIInputGetter
    {
        /// <summary>
        /// 初始化状态
        /// </summary>
        [Name("初始化状态")]
        public bool _initState = true;

        /// <summary>
        /// 车辆切换状态
        /// </summary>
        [Name("车辆切换状态")]
        public VehicleDriverToggleState _vehicleToggleState = new VehicleDriverToggleState();

        /// <summary>
        /// 切换
        /// </summary>
        protected Toggle toggle;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            toggle = GetComponent<Toggle>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            toggle.onValueChanged.AddListener(OnToggleValueChanged);

            // 设置车辆驾驶器
            if (_vehicleToggleState._vehicleDriver==null )
            {
                _vehicleToggleState._vehicleDriver = vehicleDriver;
            }

            _vehicleToggleState.RemoveListener(ChangeToggleValue);
            
            if (_initState)
            {
                _vehicleToggleState.AddListenerAndInit(ChangeToggleValue);
            }
            else
            {
                _vehicleToggleState.AddListener(ChangeToggleValue);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);

            _vehicleToggleState.RemoveListener(ChangeToggleValue);
        }

        /// <summary>
        /// 切换
        /// </summary>
        /// <param name="running"></param>
        protected virtual void ChangeToggleValue(bool isOn)
        {
            toggle.isOn = isOn;
        }

        /// <summary>
        /// 值变化
        /// </summary>
        /// <param name="value"></param>
        protected void OnToggleValueChanged(bool value)
        {
            _vehicleToggleState.state = value;
        }
    }
}