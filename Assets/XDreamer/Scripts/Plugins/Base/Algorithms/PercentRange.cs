using System;
using UnityEngine;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 百分比区间
    /// </summary>
    [Serializable]
    [Name("百分比区间")]
    public class PercentRange : Range, IPercentClip
    {
        //[LimitRange(0, 1)]
        [Name("百分比区间")]
        [Tip("当前逻辑执行的百分比区间", "Percentage interval of current logic execution")]
        public V2D percentRange = new V2D(0, 1);

        /// <summary>
        /// 开始百分比
        /// </summary>
        public double beginPercent { get => percentRange.x; set => percentRange.x = value; }

        /// <summary>
        /// 结束百分比
        /// </summary>
        public double endPercent { get => percentRange.y; set => percentRange.y = value; }

        /// <summary>
        /// 百分比长度
        /// </summary>
        public double percentLength { get => percentRange.y - percentRange.x; set => percentRange.y = MathX.Clamp01(percentRange.x + value); }

        public override double left => percentRange.x;

        public override double right => percentRange.y;
    }
}
