using System;

namespace XCSJ.Extension.Base.Units
{
    /// <summary>
    /// 单位特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class UnitAttribute : Attribute
    {
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; private set; } = "";

        /// <summary>
        /// 比例尺：即当前1单位转为默认标准1单位时的值；
        /// </summary>
        public double scale { get; private set; } = 1;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="unit">单位</param>
        /// <param name="scale">比例尺：即当前1单位转为默认标准1单位时的值；</param>
        public UnitAttribute(string unit, double scale = 1)
        {
            this.unit = unit;
            this.scale = scale;
        }
    }
}
