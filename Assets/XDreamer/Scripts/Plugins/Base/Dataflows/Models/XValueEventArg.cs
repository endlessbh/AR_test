using System;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Dataflows.Models
{
    /// <summary>
    /// 值事件参数
    /// </summary>
    public class XValueEventArg : EventArgs
    {
        /// <summary>
        /// 发送者
        /// </summary>
        public object sender { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public EDefaultEventType eventType = EDefaultEventType.None;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        public XValueEventArg(object sender)
        {
            this.sender = sender;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventType"></param>
        /// <param name="value"></param>
        public XValueEventArg(object sender, EDefaultEventType eventType, object value) : this(sender)
        {
            this.eventType = eventType;
            hasValue = true;
            this.value = value;
        }

        /// <summary>
        /// 有值
        /// </summary>
        public virtual bool hasValue { get; private set; } = false;

        /// <summary>
        /// 值
        /// </summary>
        public virtual object value { get; private set; } = null;
    }

    /// <summary>
    /// 事件类型
    /// </summary>
    [Name("事件类型")]
    public enum EDefaultEventType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 初始化事件
        /// </summary>
        [Name("初始化")]
        Init,
    }

}
