using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Recorders
{
    /// <summary>
    /// 恢复接口
    /// </summary>
    public interface IRecover
    {
        /// <summary>
        /// 恢复
        /// </summary>
        void Recover();
    }

    /// <summary>
    /// 记录器接口
    /// </summary>
    public interface IRecorder: IRecover
    {
        void Clear();
    }

    /// <summary>
    /// 记录器泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRecorder<T> : IRecorder, IRecord<T>
    {
    }

    /// <summary>
    /// 百分比记录器泛型接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPercentRecorder<T> : IRecorder<T>, ISetPercent
    {
    }

    /// <summary>
    /// 设置百分比接口
    /// </summary>
    public interface ISetPercent
    {
        void SetPercent(Percent percent);
    }
}
