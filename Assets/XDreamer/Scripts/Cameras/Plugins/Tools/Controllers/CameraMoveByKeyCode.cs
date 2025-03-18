using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动通过键码:默认通过键盘WASDQE键码控制相机的移动
    /// </summary>
    [Name("相机移动通过键码")]
    [Tip("默认通过键盘WASDQE键码控制相机的移动", "By default, the movement of the camera is controlled through the WASDQE key code on the keyboard")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, /*nameof(CameraController),*/ nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveByKeyCode : BaseCameraMoveController
    {
        #region 键码

        /// <summary>
        /// 左移:对应X轴移动，X减
        /// </summary>
        [Name("左移")]
        [Tip("对应X轴移动，X减", "Corresponding to X-axis movement, X minus")]
        public KeyCode _left = KeyCode.A;

        /// <summary>
        /// 右移:对应X轴移动，X增
        /// </summary>
        [Name("右移")]
        [Tip("对应X轴移动，X增", "Corresponding to the movement of X axis, X increases")]
        public KeyCode _right = KeyCode.D;

        /// <summary>
        /// 上移:对应Y轴移动，Y增
        /// </summary>
        [Name("上移")]
        [Tip("对应Y轴移动，Y增", "Corresponding to the movement of Y axis, Y increases")]
        public KeyCode _up = KeyCode.Q;

        /// <summary>
        /// 下移:对应Y轴移动，Y减
        /// </summary>
        [Name("下移")]
        [Tip("对应Y轴移动，Y减", "Corresponding to Y-axis movement, Y minus")]
        public KeyCode _down = KeyCode.E;

        /// <summary>
        /// 前进:对应Z轴移动，Z增
        /// </summary>
        [Name("前进")]
        [Tip("对应Z轴移动，Z增", "Corresponding to the movement of Z axis, Z increases")]
        public KeyCode _forward = KeyCode.W;

        /// <summary>
        /// 后退:对应Z轴移动，Z减
        /// </summary>
        [Name("后退")]
        [Tip("对应Z轴移动，Z减", "Corresponding to Z-axis movement, Z minus")]
        public KeyCode _back = KeyCode.S;

        #endregion

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();
            var speedRealtime = this.speedRealtime;

            var hasInput = false;

            if (Input.GetKey(_left))
            {
                _offset.x -= speedRealtime.x;
                hasInput = true;
            }
            if (Input.GetKey(_right))
            {
                _offset.x += speedRealtime.x;
                hasInput = true;
            }

            if (Input.GetKey(_up))
            {
                _offset.y += speedRealtime.y;
                hasInput = true;
            }
            if (Input.GetKey(_down))
            {
                _offset.y -= speedRealtime.y;
                hasInput = true;
            }

            if (Input.GetKey(_forward))
            {
                _offset.z += speedRealtime.z;
                hasInput = true;
            }
            if (Input.GetKey(_back))
            {
                _offset.z -= speedRealtime.z;
                hasInput = true;
            }

            if (hasInput)
            {
                Move();
            }
        }
    }
}
