using UnityEngine;
using XCSJ.Extension.Base.Recorders;

namespace XCSJ.Extension.Base.Components
{
    /// <summary>
    /// 变换主控制器
    /// </summary>
    public interface ITransformMainController : IMainController, ITransformMainControllerMembers { }

    /// <summary>
    /// 变换子控制器
    /// </summary>
    public interface ITransformSubController : ISubController, ITransformMainControllerMembers
    {
        /// <summary>
        /// 变换的主控制器
        /// </summary>
        ITransformMainController transformMainController { get; }
    }

    /// <summary>
    /// 变换主控制器成员
    /// </summary>
    public interface ITransformMainControllerMembers
    {
        /// <summary>
        /// 变换器
        /// </summary>
        ITransformer transformer { get; }
    }

    /// <summary>
    /// 变换器：用于变换信息的记录、修改更新、还原等操作的对象;
    /// </summary>
    public interface ITransformer : ITransformSubController
    {
        #region 变换

        /// <summary>
        /// 主变换
        /// </summary>
        Transform mainTransform { get; }

        /// <summary>
        /// 变换列表
        /// </summary>
        Transform[] transforms { get; }

        /// <summary>
        /// 变换列表中第一个变换
        /// </summary>
        Transform firstTransform { get; }

        #endregion

        #region 变换记录器

        /// <summary>
        /// 变换记录器列表
        /// </summary>
        ITransformRecorder[] transformRecorders { get; }

        /// <summary>
        /// 变换记录器列表中第一个记录器
        /// </summary>
        ITransformRecorder transformRecorder { get; }

        /// <summary>
        /// 变换记录器列表中第一个记录器的第一条变换记录
        /// </summary>
        ITransformRecord firstTransformRecord { get; }

        /// <summary>
        /// 变换记录器列表中末一个记录器
        /// </summary>
        ITransformRecorder lastTransformRecorder { get; }

        #endregion

        #region TR

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        void Move(Vector3 value, int moveMode);

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="rotateMode"></param>
        void Rotate(Vector3 value, int rotateMode);

        #endregion
    }

    /// <summary>
    /// 变换控制器：用于分析对变换的各种控制命令，并将最终控制数据提交给变换器去做最终的执行；
    /// </summary>
    public interface ITransformController : ITransformSubController { }

    /// <summary>
    /// 变换移动器：用于分析对变换的移动控制命令，并将最终移动控制数据提交给变换器去做最终的执行；
    /// </summary>
    public interface ITransformMover : ITransformController
    {
        void Move(Vector3 value, int moveMode);
    }

    /// <summary>
    /// 变换旋转器：用于分析对变换的旋转控制命令，并将最终旋转控制数据提交给变换器去做最终的执行；
    /// </summary>
    public interface ITransformRotator : ITransformController
    {
        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="rotateMode"></param>
        void Rotate(Vector3 value, int rotateMode);
    }
}
