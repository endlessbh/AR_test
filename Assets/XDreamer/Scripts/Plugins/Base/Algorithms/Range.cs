using UnityEngine;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 区间：用于限定值的范围；
    /// </summary>
    [LanguageFileOutput]
    public abstract class Range
    {
        /// <summary>
        /// 左值
        /// </summary>
        public abstract double left { get; }

        /// <summary>
        /// 右值
        /// </summary>
        public abstract double right { get; }

        /// <summary>
        /// 长度
        /// </summary>
        public double length => right - left;

        /// <summary>
        /// 相对左值的标准化
        /// </summary>
        /// <param name="value">值:已减去左值的值</param>
        /// <returns>返回值区间[0,1]</returns>
        public double NormalizeOfRelativeLeft(double value)
        {
            if (value <= 0) return 0;
            var len = length;
            if (MathX.ApproximatelyZero(len)) return 0;
            if (value >= len) return 1;
            return value / len;
        }

        /// <summary>
        /// 标准化
        /// </summary>
        /// <param name="value">值：会先减去左值再标准化</param>
        /// <returns>返回值区间[0,1]</returns>
        public double Normalize(double value) => NormalizeOfRelativeLeft(value - left);

        /// <summary>
        /// 相对左值的缩放
        /// </summary>
        /// <param name="value">值:已减去左值的值</param>
        /// <returns>返回值区间(-∞,+∞)</returns>
        public double ScaleOfRelativeLeft(double value) => MathX.Scale(value, length);

        /// <summary>
        /// 值：会先减去左值再缩放
        /// </summary>
        /// <param name="value">值：会先减去左值再缩放</param>
        /// <returns>返回值区间(-∞,+∞)</returns>
        public double Scale(double value) => ScaleOfRelativeLeft(value - left);

        /// <summary>
        /// 在区间内；值在[左值-误差,右值+误差]的闭区间内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool In(double value, double epsilon = Percent.Epsilon) => value >= left - epsilon && value <= right + epsilon;

        /// <summary>
        /// 在区间左：值在(-∞,左值-误差)的开区间内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Left(double value, double epsilon = Percent.Epsilon) => value < left - epsilon;

        /// <summary>
        /// 在区间左：值在(右值+误差,+∞)的开区间内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public bool Right(double value, double epsilon = Percent.Epsilon) => value > right + epsilon;

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() => left.ToString() + "/" + right.ToString();
    }
}
