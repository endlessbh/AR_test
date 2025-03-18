using System;

namespace XCSJ.Extension.Base.Dataflows.Models
{
    /// <summary>
    /// 用于主动触发值的事件
    /// </summary>
    public interface ISendEvent
    {
        /// <summary>
        /// 发送事件
        /// </summary>
        event Action<XValueEventArg> sendEvent;
    }
}
