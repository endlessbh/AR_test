using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.Extension.Characters.Base
{
    /// <summary>
    /// 基础角色相机控制器
    /// </summary>
    [Name("基础角色相机控制器")]
    public abstract class BaseCharacterCameraController : BaseCharacterCoreController
    {
        /// <summary>
        /// 相机主控制器
        /// </summary>
        public abstract BaseCameraMainController cameraMainController { get; }

        /// <summary>
        /// 主相机
        /// </summary>
        public Camera mainCamera => cameraMainController.cameraEntityController.mainCamera;
    }
}
