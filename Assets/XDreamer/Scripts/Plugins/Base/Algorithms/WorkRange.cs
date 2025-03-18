using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Maths;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils;
using XCSJ.Languages;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 工作区间
    /// </summary>
    [Serializable]
    [LanguageFileOutput]
    [Name("工作区间")]
    public class WorkRange : ITimeClip, IPercentClip, ITTL
    {
        static WorkRange()
        {
            Converter.instance.Register<WorkRange, string>(i => i.ToString());
            Converter.instance.Register<string, WorkRange>(i => StringToWorkRange(i));
        }

        /// <summary>
        /// 百分比区间
        /// </summary>
        [Name("百分比区间")]
        public PercentRange percentRange = new PercentRange();

        /// <summary>
        /// 时间区间
        /// </summary>
        [Name("时间区间")]
        public TimeRange timeRange = new TimeRange();

        /// <summary>
        /// 构造
        /// </summary>
        public WorkRange() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="range"></param>
        public WorkRange(Vector4 range)
        {
            percentRange.percentRange = new V2D(range.x, range.y);
            timeRange.timeRange = new V2D(range.z, range.w);
        }

        public double totalTimeLength
        {
            get => MathX.Scale(timeRange.length, percentRange.length);
            set => timeRange.timeRange = value * percentRange.percentRange;
        }

        public double beginTime { get => timeRange.beginTime; set => timeRange.beginTime = value; }
        public double endTime { get => timeRange.endTime; set => timeRange.endTime = value; }
        public double timeLength { get => timeRange.timeLength; set => timeRange.timeLength = value; }

        public double beginPercent { get => percentRange.beginPercent; set => percentRange.beginPercent = value; }
        public double endPercent { get => percentRange.endPercent; set => percentRange.endPercent = value; }
        public double percentLength { get => percentRange.percentLength; set => percentRange.percentLength = value; }

        public override string ToString() => string.Format("{0}/{1}", percentRange.ToString(), timeRange.ToString());

        public static string WorkRangeToString(WorkRange workRange) => workRange != null ? workRange.ToString() : "";

        public static WorkRange StringToWorkRange(string value) => new WorkRange(CommonFun.StringToVector4(value));
    }
}
