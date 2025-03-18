using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.Tools
{
    /// <summary>
    /// 相机移动通过XBox:默认通过XBox左摇杆与Dpad控制相机的移动
    /// </summary>
    [Name("相机移动通过XBox")]
    [Tip("默认通过XBox左摇杆与Dpad控制相机的移动", "By default, the movement of the camera is controlled through the Xbox left rocker and dpad")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController),*/ nameof(CameraTransformer)/*, nameof(CameraTargetController)*/)]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [RequireManager(typeof(XBoxManager))]
    [Owner(typeof(XBoxManager))]
    public class CameraMoveByXBox : BaseCameraMoveController, IXBox
    {
        /// <summary>
        /// 控制数据
        /// </summary>
        [Name("控制数据")]
        public XBoxControlData _controlData = new XBoxControlData();

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();
            var speedRealtime = this.speedRealtime;

            _offset = _controlData.GetOffset();
            _offset.Scale(speedRealtime);

            if (_offset != Vector3.zero)
            {
                Move();
            }
        }
    }
}
