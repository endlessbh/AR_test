using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Helper;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 时间区间
    /// </summary>
    [Serializable]
    [Name("时间区间")]
    public class TimeRange : Range, ITimeClip
    {
        /// <summary>
        /// 默认最大时长
        /// </summary>
        public const double DefaultMaxTimeLength = 3600;

        /// <summary>
        /// 默认时长
        /// </summary>
        public const double DefaultTimeLength = 3;

        /// <summary>
        /// 时间区间
        /// </summary>
        [Name("时间区间")]
        [Tip("当前逻辑执行的时间区间", "Time interval of current logic execution")]
        public V2D timeRange = new V2D(0, DefaultTimeLength);

        /// <summary>
        /// 限定的最大时长
        /// </summary>
        public double _limitMaxTimeLength = DefaultMaxTimeLength;

        /// <summary>
        /// 开始时间
        /// </summary>
        public double beginTime { get => timeRange.x; set => timeRange.x = value; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public double endTime { get => timeRange.y; set => timeRange.y = value; }

        /// <summary>
        /// 时长
        /// </summary>
        public double timeLength { get => timeRange.y - timeRange.x; set => timeRange.y = timeRange.x + value; }

        /// <summary>
        /// 左值
        /// </summary>
        public override double left => timeRange.x;

        /// <summary>
        /// 右值
        /// </summary>
        public override double right => timeRange.y;
    }
}
