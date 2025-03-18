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
    /// 相机旋转通过XBox：默认通过XBox右摇杆控制相机的旋转
    /// </summary>
    [Name("相机旋转通过XBox")]
    [Tip("默认通过XBox右摇杆控制相机的旋转", "默认通过XBox右摇杆控制相机的旋转")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Rotate, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [Tool(XBoxHelper.Title)]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [RequireManager(typeof(XBoxManager))]
    [Owner(typeof(XBoxManager))]
    public class CameraRotateByXBox : BaseCameraRotateController, IXBox
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
                Rotate();
            }
        }
    }
}
