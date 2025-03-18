using System;
using XCSJ.Algorithms;

namespace XCSJ.PluginXGUI.DataViews.Base
{
    /// <summary>
    /// 数据视图启用特性:用于修饰对象是否构建数据视图
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class DataViewEnableAttribute : Attribute
    {
        /// <summary>
        /// 启用
        /// </summary>
        public bool enable { get; private set; } = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="enable"></param>
        public DataViewEnableAttribute(bool enable = true)
        {
            this.enable = enable;   
        }
    }
}
