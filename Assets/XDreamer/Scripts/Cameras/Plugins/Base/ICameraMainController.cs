using UnityEngine;
using XCSJ.Extension.Base.Components;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 相机主控制器
    /// </summary>
    public interface ICameraMainController : ITransformMainController, ICameraMainControllerMembers
    {
    }

    public interface ICameraMainControllerMembers : ITransformMainControllerMembers
    {
        /// <summary>
        /// 相机实体控制器
        /// </summary>
        ICameraEntityController cameraEntityController { get; }

        /// <summary>
        /// 相机变换器
        /// </summary>
        ICameraTransformer cameraTransformer { get; }

        /// <summary>
        /// 相机目标控制器
        /// </summary>
        ICameraTargetController cameraTargetController { get; }
    }

    /// <summary>
    /// 相机子控制器
    /// </summary>
    public interface ICameraSubController : ITransformSubController, ICameraMainControllerMembers
    {
        /// <summary>
        /// 相机主控制器
        /// </summary>
        ICameraMainController cameraMainController { get; }
    }

    /// <summary>
    /// 相机变换控制器
    /// </summary>
    public interface ICameraTransformController : ICameraSubController, ITransformController { }

    /// <summary>
    /// 相机实体控制器
    /// </summary>
    public interface ICameraEntityController : ICameraSubController
    {
        /// <summary>
        /// 主相机
        /// </summary>
        Camera mainCamera { get; }

        /// <summary>
        /// 相机列表
        /// </summary>
        Camera[] cameras { get; }
    }

    /// <summary>
    /// 相机变换器
    /// </summary>
    public interface ICameraTransformer : ICameraSubController, ITransformer, ICameraMover, ICameraRotator
    {
        /// <summary>
        /// 将相机的变换信息重置到缺省状态，即相对父级位置、旋转均为0，缩放全为1的初始状态；
        /// </summary>
        void ResetCamera();

        /// <summary>
        /// 将相机的变换信息重置到程序启动时记录的状态
        /// </summary>
        void ResetCameraToStart();
    }

    /// <summary>
    /// 相机移动器
    /// </summary>
    public interface ICameraMover : ICameraTransformController, ITransformMover
    {
        /// <summary>
        /// 速度系数
        /// </summary>
        Vector3 moveSpeedCoefficient { get; }

        /// <summary>
        /// 阻尼系数
        /// </summary>
        float moveDampingCoefficient { get; }
    }

    /// <summary>
    /// 相机旋转器
    /// </summary>
    public interface ICameraRotator : ICameraTransformController, ITransformRotator
    {
        /// <summary>
        /// 速度系数
        /// </summary>
        Vector3 rotateSpeedCoefficient { get; }

        /// <summary>
        /// 阻尼系数
        /// </summary>
        float rotateDampingCoefficient { get; }
    }

    /// <summary>
    /// 相机目标控制器
    /// </summary>
    public interface ICameraTargetController : ICameraSubController
    {
        #region 目标

        /// <summary>
        /// 主目标
        /// </summary>
        Transform mainTarget { get; set; }

        /// <summary>
        /// 目标列表
        /// </summary>
        Transform[] targets { get; set; }

        /// <summary>
        /// 第一目标
        /// </summary>
        Transform firstTarget { get; }

        /// <summary>
        /// 末一目标
        /// </summary>
        Transform lastTarget { get; }

        #endregion

        #region 位置

        /// <summary>
        /// 目标位置
        /// </summary>
        Vector3 targetPosition { get; }

        #endregion

        #region 旋转

        /// <summary>
        /// 目标旋转角度 旋转
        /// </summary>
        Vector3 targetRotationAngle { get; }

        /// <summary>
        /// 目标旋转量
        /// </summary>
        Quaternion targetRotation { get; }

        #endregion

        #region 包围盒

        /// <summary>
        /// 目标包围盒
        /// </summary>
        Bounds targetBounds { get; }

        #endregion
    }

    /// <summary>
    /// 相机速度
    /// </summary>
    public interface ICameraSpeed
    {
        /// <summary>
        /// 相机速度
        /// </summary>
        Vector3 speed { get; }
    }

    /// <summary>
    /// 相机阻尼
    /// </summary>
    public interface ICameraDamping
    {
        /// <summary>
        /// 表示是否使用阻尼
        /// </summary>
        bool useDamping { get; set; }

        /// <summary>
        /// 阻尼系数
        /// </summary>
        float dampingCoefficient { get; }
    }

    /// <summary>
    /// 相机移动阻尼
    /// </summary>
    public interface ICameraMoveDamping : ICameraDamping { }

    /// <summary>
    /// 相机旋转阻尼
    /// </summary>
    public interface ICameraRotateDamping : ICameraDamping { }
}
