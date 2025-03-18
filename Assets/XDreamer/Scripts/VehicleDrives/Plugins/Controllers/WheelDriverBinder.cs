using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 车轮模型绑定器
    /// </summary>
    [Name("车轮模型绑定器")]
    [Tip("用于将模型与车轮驱动器关联，并将车辆驱动器所在游戏对象与模型位置与角度进行对齐", "Used to associate the model with the wheel drive and align the position and angle of the game object where the vehicle drive is located with the model")]
    [XCSJ.Attributes.Icon(EIcon.Wheel)]
    [Tool(VehicleDriveHelper.CategoryComponentName)]
    [RequireManager(typeof(VehicleDriveManger))]
    public class WheelDriverBinder : InteractProvider
    {
        /// <summary>
        /// 车轮驱动器
        /// </summary>
        [Name("车轮驱动器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public WheelDriver _wheelDriver = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_wheelDriver)
            {
                _wheelDriver._wheelModel = transform;

                _wheelDriver.AlignWithModel();
            }
        }
    }
}
