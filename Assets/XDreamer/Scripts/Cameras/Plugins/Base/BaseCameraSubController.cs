using XCSJ.Extension.Base.Components;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机子控制器
    /// </summary>
    [RequireManager(typeof(CameraManager))]
    [Owner(typeof(CameraManager))]
    public abstract class BaseCameraSubController : BaseSubController<BaseCameraMainController>, ICameraSubController
    {
        #region 主控制器

        public BaseCameraMainController cameraController => mainController;

        /// <summary>
        /// 相机主控制器
        /// </summary>
        public BaseCameraMainController cameraMainController => mainController;

        ITransformMainController ITransformSubController.transformMainController => mainController;

        ICameraMainController ICameraSubController.cameraMainController => mainController;

        #endregion

        #region 相机

        /// <summary>
        /// 相机实体控制器
        /// </summary>
        public BaseCameraEntityController cameraEntityController => mainController.cameraEntityController;

        ICameraEntityController ICameraMainControllerMembers.cameraEntityController => mainController.cameraEntityController;

        #endregion

        #region 变换

        /// <summary>
        /// 相机变换器
        /// </summary>
        public BaseCameraTransformer cameraTransformer => mainController.cameraTransformer;

        ITransformer ITransformMainControllerMembers.transformer => mainController.cameraTransformer;

        ICameraTransformer ICameraMainControllerMembers.cameraTransformer => mainController.cameraTransformer;

        #endregion

        #region 目标

        /// <summary>
        /// 相机目标控制器
        /// </summary>
        public BaseCameraTargetController cameraTargetController => mainController.cameraTargetController;

        ICameraTargetController ICameraMainControllerMembers.cameraTargetController => mainController.cameraTargetController;

        #endregion
    }
}
