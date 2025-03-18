using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 循环类型枚举
    /// </summary>
    [Name("循环类型")]
    public enum ELoopType
    {
        /// <summary>
        /// 无:仅执行一次，即无循环逻辑
        /// </summary>
        [Name("无")]
        [Tip("仅执行一次，即无循环逻辑")]
        None,

        /// <summary>
        /// 循环:循环逻辑
        /// </summary>
        [Name("循环")]
        [Tip("循环逻辑")]
        Loop,

        /// <summary>
        /// 乒乓:类似乒乓球(或单摆)做往返型循环逻辑
        /// </summary>
        [Name("乒乓")]
        [Tip("类似乒乓球(或单摆)做往返型循环逻辑")]
        PingPong,
    }

    /// <summary>
    /// 循环工作剪辑接口
    /// </summary>
    [Name("循环工作剪辑")]
    public interface ILoopWorkClip : IBaseLoopWorkClip, IWorkClip
    {
       
    }
}
