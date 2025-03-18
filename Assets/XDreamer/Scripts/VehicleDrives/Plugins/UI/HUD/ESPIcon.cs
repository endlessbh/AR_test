using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.DriveAssists;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginVehicleDrive.UI.HUD
{
    /// <summary>
    /// ESP图标
    /// </summary>
    [Name("ESP图标")]
    public class ESPIcon : VehicleHUDGetter
    {
        /// <summary>
        /// ESP
        /// </summary>
        [Name("ESP")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public ESP _esp = null;

        /// <summary>
        /// 图像
        /// </summary>
        [Name("图像")]
        public Image _image;

        /// <summary>
        /// 变色
        /// </summary>
        [Name("变色")]
        public Color _changeColor = Color.yellow;

        private Color _orgColor = Color.white;


        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            if (!_esp)
            {
                _esp = vehicleDriver.GetComponentInChildren<ESP>();
            }

            if (_image)
            {
                _orgColor = _image.color;
            }
        }

        /// <summary>
        /// 延时更新
        /// </summary>
        void LateUpdate()
        {
            if (_image && _esp) _image.color = _esp.ESPEnable ? _changeColor : _orgColor;
        }
	}
}
