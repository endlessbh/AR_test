using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// 车辆图标显示
    /// </summary>
    [Name("车辆图标")]
    public class VehicleIcon : VehicleHUDGetter
    {
        /// <summary>
        /// 车灯控制器
        /// </summary>
        [Name("车灯控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleLightController _vehicleLightController = null;

        /// <summary>
        /// 图标类型
        /// </summary>
        [Name("图标类型")]
        [EnumPopup]
        public EVehicleIconType _vehicleIconType = EVehicleIconType.ABS;

        /// <summary>
        /// 图标显示方式
        /// </summary>
        [Name("图标显示方式")]
        [EnumPopup]
        public EVehicleIconShowRule _vehicleIconShowRule = EVehicleIconShowRule.Active;

        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        public Image _image = null;

        /// <summary>
        /// 游戏对象
        /// </summary>
        [Name("游戏对象")]
        [Tip("当图标显示方式是变色时，将修改游戏对象主材质颜色", "When the icon display mode is color change, the main material color of the game object will be modified")]
        public GameObject _gameObject = null;

        /// <summary>
        /// 工作颜色
        /// </summary>
        [Name("工作颜色")]
        [HideInSuperInspector(nameof(_vehicleIconShowRule), EValidityCheckType.NotEqual, EVehicleIconShowRule.ChangeColor)]
        public Color _workingColor = Color.yellow;

        private Color _imageOrgColor = Color.white;

        private Material _material = null;
        private Color _gameObjectOrgColor = Color.white;

        /// <summary>
        /// 车灯
        /// </summary>
        private VehicleLightController vehicleLight
        {
            get
            {
                return _vehicleDriver ? _vehicleDriver.lightController as VehicleLightController : null;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        protected override void Reset()
        {
            base.Reset();

            Init();
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            Init();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            _vehicleDriver = vehicleDriver;
        }

        private VehicleDriver _vehicleDriver = null;

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void Init()
        {
            this.XGetComponentInParentOrGlobal(ref _vehicleLightController);

            if (!_image)
            {
                _image = GetComponent<Image>();
            }

            if (_image)
            {
                _imageOrgColor = _image.color;
            }

            if (_gameObject && _gameObject.GetComponent<Renderer>() is Renderer r && r && r.material)
            {
                _material = r.material;
                _gameObjectOrgColor = _material.color;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            BindVehicleLightIndicator();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            UnbindVehicleLightIndicator();
        }

        private void BindVehicleLightIndicator()
        {
            if (vehicleLight && !bindVehicleLight)
            {
                vehicleLight.onIndicatorChanged += OnIndicator;
                bindVehicleLight = true;
            }
        }

        private void UnbindVehicleLightIndicator()
        {
            if (vehicleLight && bindVehicleLight)
            {
                vehicleLight.onIndicatorChanged -= OnIndicator;
                bindVehicleLight = false;
            }
        }

        private bool bindVehicleLight = false;

        /// <summary>
        /// 响应指示灯变化函数
        /// </summary>
        /// <param name="isOn"></param>
        private void OnIndicator(bool isOn)
        {
            flashCounter = 0;
        }

        /// <summary>
        /// 延时更新
        /// </summary>
        protected void LateUpdate()
        {
            if (!_vehicleDriver) return;

            BindVehicleLightIndicator();

            switch (_vehicleIconType)
            {
                case EVehicleIconType.ABS:
                    {
                        if ( _vehicleDriver.brake != null)
                        {
                            SetIcon(_vehicleDriver.brake.ABSEnable);
                        }
                        break;
                    }
                case EVehicleIconType.Handbrake:
                    {
                        if (_vehicleDriver.brake != null)
                        {
                            SetIcon(_vehicleDriver.brake.handbrakeOn);
                        }
                        break;
                    }
                case EVehicleIconType.LowBeamHeadlight:
                    {
                        if (vehicleLight)
                        {
                            SetIcon(vehicleLight.lowBeamHeadLightsOn);
                        }
                        break;
                    }
                case EVehicleIconType.HighBeamHeadLight:
                    {
                        if (vehicleLight)
                        {
                            SetIcon(vehicleLight.highBeamHeadLightsOn);
                        }
                        break;
                    }
                case EVehicleIconType.FogLight:
                    {
                        if (vehicleLight)
                        {
                            SetIcon(vehicleLight.fogLightOn);
                        }
                        break;
                    }
                case EVehicleIconType.LeftIndicator:
                    {
                        if (vehicleLight)
                        {
                            SetIcon(vehicleLight.leftIndicatorOn || vehicleLight.allIndicatorOn);
                        }
                        break;
                    }
                case EVehicleIconType.RightIndicator:
                    {
                        if (vehicleLight)
                        {
                            SetIcon(vehicleLight.rightIndicatorOn || vehicleLight.allIndicatorOn);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private void SetLightIcon()
        {

        }

        private float flashCounter = 0;

        private void SetIcon(bool working)
        {
            switch (_vehicleIconShowRule)
            {
                case EVehicleIconShowRule.Active:
                    {
                        if (_image) _image.enabled = working;
                        if (_gameObject) _gameObject.SetActive(working);
                        break;
                    }
                case EVehicleIconShowRule.Flash:
                    {
                        if (working)
                        {
                            flashCounter += Time.deltaTime;
                            if (flashCounter > 1) flashCounter = 0;

                            var flag = flashCounter > 0.5 ? false : true;
                            if (_image) _image.enabled = flag;
                            if (_gameObject) _gameObject.SetActive(flag);
                        }
                        else
                        {
                            flashCounter = 0;

                            if (_image) _image.enabled = false;
                            if (_gameObject) _gameObject.SetActive(false);
                        }
                        break;
                    }
                case EVehicleIconShowRule.ChangeColor:
                    {
                        if (_image) _image.color = working ? _workingColor : _imageOrgColor;
                        if (_material) _material.color = working ? _workingColor : _gameObjectOrgColor;
                        break;
                    }
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// 车辆图标类型
    /// </summary>
    [Name("车辆图标类型")]
    public enum EVehicleIconType
    {
        [Name("ABS")]
        ABS,

        [Name("手刹")]
        Handbrake,

        [Name("近光灯")]
        LowBeamHeadlight,

        [Name("远光灯")]
        HighBeamHeadLight,

        [Name("雾灯")]
        FogLight,

        [Name("左转灯")]
        LeftIndicator,

        [Name("右转灯")]
        RightIndicator,
    }

    /// <summary>
    /// 车辆图标显示方式
    /// </summary>
    [Name("车辆图标显示方式")]
    public enum EVehicleIconShowRule
    {
        [Name("激活")]
        Active,

        [Name("闪烁")]
        Flash,

        [Name("变色")]
        ChangeColor,
    }
}
