namespace XCSJ.Extension.Base.Dataflows.Models
{
    /// <summary>
    /// 属性变换参数
    /// </summary>
    public class XPropertyChangeEventArgs : XValueEventArg
    {
        public XPropertyChangeEventArgs(object sender) : base(sender) { }

        public XPropertyChangeEventArgs(object sender, string propertyName, object newValue, object oldValue = null) : base(sender)
        {
            this.propertyName = propertyName;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// 是否将要发生改变
        /// </summary>
        public bool isWillChange { get; private set; } = false;

        /// <summary>
        /// 能否改变
        /// </summary>
        public bool canChange { get; set; } = true;

        /// <summary>
        /// 属性名称
        /// </summary>
        public string propertyName { get; private set; }

        /// <summary>
        /// 旧值
        /// </summary>
        public object oldValue { get; private set; }

        /// <summary>
        /// 新值
        /// </summary>
        public object newValue { get; private set; }

        /// <summary>
        /// 有值
        /// </summary>
        public override bool hasValue => true;

        /// <summary>
        /// 值
        /// </summary>
        public override object value => newValue;

        /// <summary>
        /// 将改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public static XPropertyChangeEventArgs CreateWillChangeEvent(object sender, string propertyName, object newValue, object oldValue = null)
        {
            return Create(sender, propertyName, newValue, oldValue, true);
        }

        /// <summary>
        /// 已改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public static XPropertyChangeEventArgs CreateChangedEvent(object sender, string propertyName, object newValue, object oldValue = null)
        {
            return Create(sender, propertyName, newValue, oldValue, false);
        }

        /// <summary>
        /// 创建事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        /// <param name="oldValue"></param>
        /// <param name="isWillChange"></param>
        /// <returns></returns>
        public static XPropertyChangeEventArgs Create(object sender, string propertyName, object newValue, object oldValue, bool isWillChange)
        {
            var eventArgs = new XPropertyChangeEventArgs(sender, propertyName, newValue, oldValue);
            eventArgs.isWillChange = isWillChange;
            return eventArgs;
        }
    }
}
