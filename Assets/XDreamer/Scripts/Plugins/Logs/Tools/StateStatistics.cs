using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Logs.Tools
{
    /// <summary>
    /// 状态统计
    /// </summary>
    [Name("状态统计")]
    [Tip("运行时的状态统计", "Status statistics at runtime")]
    public class StateStatistics : InteractProvider
    {
        /// <summary>
        /// 统计
        /// </summary>
        [Name("统计")]
        public EStatistics _statistics = (EStatistics)(-1);

        /// <summary>
        /// 统计输出模式
        /// </summary>
        [Name("统计输出模式")]
        [EnumPopup]
        public EStatisticsOuputMode _statisticsOuputMode = EStatisticsOuputMode.NameAndValue;

        /// <summary>
        /// 文本:用于显示运行时状态统计的文本对象
        /// </summary>
        [Name("文本")]
        [Tip("用于显示运行时状态统计的文本对象", "A text object used to display runtime status statistics")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Text _text;

        /// <summary>
        /// 文本
        /// </summary>
        public Text text
        {
            get => this.XGetComponent(ref _text);
            set => this.XModifyProperty(ref _text, value);
        }

        /// <summary>
        /// 刷新间隔:刷新文本的间隔时间；单位：秒；
        /// </summary>
        [Name("刷新间隔")]
        [Tip("刷新文本的间隔时间；单位：秒；", "The interval between refreshing the text; Unit: second;")]
        [Range(0, 10)]
        public float _refreshInterval = 1;

        private float interval = 0;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            interval = _refreshInterval + 1;
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_text) return;
            if (interval < _refreshInterval)
            {
                interval += Time.deltaTime;
                return;
            }
            interval = 0;
            _text.text = _statistics.GetStatisticsValueText(_statisticsOuputMode);
        }
    }

    /// <summary>
    /// 统计
    /// </summary>
    [Flags]
    [Name("统计")]
    public enum EStatistics
    {
        /// <summary>
        /// FPS
        /// </summary>
        [Name("FPS")]
        [EnumFieldName("FPS")]
        [Tip("即时帧速率FPS", "Real time frame rate FPS")]
        FPS = 1 << 0,

        /// <summary>
        /// 自关卡加载后时间：最后一个非累加型场景加载后加载后时间；单位：秒；
        /// </summary>
        [Name("自关卡加载后时间")]
        [EnumFieldName("自关卡加载后时间")]
        [Tip("最后一个非累加型场景加载后加载后时间；单位：秒；", "Time after loading the last non cumulative scene; Unit: second;")]
        TimeSinceLevelLoad
    }

    /// <summary>
    /// 统计输出模式
    /// </summary>
    [Name("统计输出模式")]
    public enum EStatisticsOuputMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 名称
        /// </summary>
        [Name("名称")]
        Name,

        /// <summary>
        /// 值
        /// </summary>
        [Name("值")]
        Value,

        /// <summary>
        /// 名称与值
        /// </summary>
        [Name("名称与值")]
        NameAndValue,
    }

    /// <summary>
    /// 运行时状态统计组手
    /// </summary>
    public static class RuntimeStateStatisticsHelper
    {
        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        public static List<EStatistics> GetEnumValues(this EStatistics statistics)
        {
            var values = new List<EStatistics>();
            statistics.Foreah(s => values.Add(s));
            return values;
        }

        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="action"></param>
        public static void Foreah(this EStatistics statistics, Action<EStatistics> action)
        {
            if (action == null || statistics == 0) return;
            foreach (var s in EnumCache<EStatistics>.Array)
            {
                if ((s & statistics) == s)
                {
                    action(s);
                }
            }
        }

        /// <summary>
        /// 获取统计值
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        public static List<(EStatistics statistics, object value)> GetStatisticsValue(this EStatistics statistics)
        {
            var values = new List<(EStatistics statistics, object value)>();
            statistics.Foreah(s => values.Add((s, s.GetStatisticsSingleValue())));
            return values;
        }

        /// <summary>
        /// 获取统计值文本
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="statisticsOuputMode"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetStatisticsValueText(this EStatistics statistics, EStatisticsOuputMode statisticsOuputMode, string separator = ":")
        {
            var sb = new StringBuilder();
            bool hasValue = false;
            statistics.Foreah(s =>
            {
                if (hasValue) sb.AppendLine();
                else hasValue = true;
                sb.Append(s.GetStatisticsSingleValueText(statisticsOuputMode, separator));
            });
            return sb.ToString();
        }

        /// <summary>
        /// 获取统计单一值文本
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="statisticsOuputMode"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetStatisticsSingleValueText(this EStatistics statistics, EStatisticsOuputMode statisticsOuputMode, string separator = ":")
        {
            switch (statisticsOuputMode)
            {
                case EStatisticsOuputMode.Name: return CommonFun.Name(statistics);
                case EStatisticsOuputMode.Value: return Converter.instance.ConvertTo<string>(statistics.GetStatisticsSingleValue());
                case EStatisticsOuputMode.NameAndValue: return CommonFun.Name(statistics) + separator + Converter.instance.ConvertTo<string>(statistics.GetStatisticsSingleValue());
                default: return "";
            }
        }

        /// <summary>
        /// 获取统计单一值
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        public static object GetStatisticsSingleValue(this EStatistics statistics)
        {
            switch (statistics)
            {
                case EStatistics.FPS: return 1f / Time.deltaTime;
#if UNITY_20201_OR_NEWER
                case EStatistics.TimeSinceLevelLoad: return Time.timeSinceLevelLoadAsDouble;
#else
                case EStatistics.TimeSinceLevelLoad: return Time.timeSinceLevelLoad;
#endif
                default: return "";
            }
        }
    }
}
