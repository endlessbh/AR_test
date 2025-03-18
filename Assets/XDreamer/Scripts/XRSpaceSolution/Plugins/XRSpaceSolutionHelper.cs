using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.PluginXRSpaceSolution
{
    /// <summary>
    /// XR空间解决方案组手
    /// </summary>
    public static class XRSpaceSolutionHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XR空间解决方案";

        /// <summary>
        /// 创建HMD平移飞行相机
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CameraController CreateHMDFlyCamera<T>(string name = "HMD-平移飞行相机") where T : Component
        {
            var cameraController = CameraHelperExtension.CreateMoveFlyCamera(name);
            
            cameraController.cameraTransformer.XGetOrAddComponent<T>();

            var mainCamera = cameraController.cameraEntityController.mainCamera;
            mainCamera.XGetOrAddComponent<CameraProjection>();
            return cameraController;
        }       
    }
}
