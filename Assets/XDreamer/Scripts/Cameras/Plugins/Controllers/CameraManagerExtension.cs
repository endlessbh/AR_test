using System;
using UnityEngine;
using XCSJ.PluginCamera;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Controllers
{
    /// <summary>
    /// 相机管理器扩展：用于扩展提供相机的各种接口
    /// </summary>
    public static class CameraManagerExtension
    {
        /// <summary>
        /// 获取提供者
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static CameraManagerProvider GetProvider(this CameraManager manager)
        {
            return (CameraManagerProvider)manager.cameraManagerProvider;
        }

        /// <summary>
        /// 获取当前相机控制器
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static BaseCameraMainController GetCurrentCameraController(this CameraManager manager)
        {
            return manager.GetProvider().currentCameraController;
        }

        /// <summary>
        /// 获取初始相机控制器
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static BaseCameraMainController GetInitCameraController(this CameraManager manager)
        {
            return manager.GetProvider().initCameraController;
        }

        /// <summary>
        /// 切换相机控制器
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="targetCameraController"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="mustSwitch"></param>
        /// <returns></returns>
        public static bool SwitchCameraController(this CameraManager manager, GameObject targetCameraController, float duration, Action onCompleted, bool mustSwitch)
        {
            return manager.GetProvider().SwitchCameraController(targetCameraController, duration, onCompleted, mustSwitch);
        }

        /// <summary>
        /// 切换相机控制器
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="targetCameraController"></param>
        /// <param name="duration"></param>
        /// <param name="onCompleted"></param>
        /// <param name="mustSwitch"></param>
        /// <returns></returns>
        public static bool SwitchCameraController(this CameraManager manager, BaseCameraMainController targetCameraController, float duration, Action onCompleted, bool mustSwitch)
        {
            return manager.GetProvider().SwitchCameraController(targetCameraController, duration, onCompleted, mustSwitch);
        }
    }
}
