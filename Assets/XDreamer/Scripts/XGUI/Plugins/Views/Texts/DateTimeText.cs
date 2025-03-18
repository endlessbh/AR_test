using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Views.Texts
{
    /// <summary>
    /// 日期时间文本:将日期时间信息格式化为字符串后显示到文本
    /// </summary>
    [Name("日期时间文本")]
    [Tip("将日期时间信息格式化为字符串后显示到文本", "Format the date and time information into a string and display it in the text")]
    [XCSJ.Attributes.Icon(EIcon.Timer)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    [Tool(XGUICategory.Component, nameof(XGUIManager))]
    public class DateTimeText : BaseText
    {
        /// <summary>
        /// 日期时间规则
        /// </summary>
        [Name("日期时间规则")]
        public enum EDataTimeRule
        {
            [Name("当前系统时间")]
            CurrentSystem,

            [Name("秒表")]
            Stopwatch
        }

        /// <summary>
        /// 日期时间规则
        /// </summary>
        [Name("日期时间规则")]
        [EnumPopup]
        public EDataTimeRule _dataTimeRule = EDataTimeRule.CurrentSystem;

        /// <summary>
        /// 格式:日期时间格式
        /// </summary>
        [Name("格式")]
        [Tip("日期时间格式", "Date time format")]
        [EnumPopup]
        public EDateTimeFormat format = EDateTimeFormat.Default;

        /// <summary>
        /// 格式化字符串:用户自定义的日期时间格式
        /// </summary>
        [Name("格式化字符串")]
        [Tip("用户自定义的日期时间格式", "User defined date time format")]
        [HideInSuperInspector(nameof(format), EValidityCheckType.NotEqual, EDateTimeFormat.Custom)]
        public string formatString = "";

        /// <summary>
        /// 更新间隔时间:文本刷新的间隔时间；单位为秒；
        /// </summary>
        [Name("更新间隔时间")]
        [Tip("文本刷新的间隔时间；单位为秒；", "The interval between text refreshes; The unit is second;")]
        public float intervalTime = 1;

        #region 当前系统时间

        /// <summary>
        /// 时间跨度滴答数:将系统时间偏移参数指定的时间跨度后显示；单位为:滴答，1秒=10000000滴答，即1滴答=0.0000001秒;
        /// </summary>
        [Name("时间跨度滴答数")]
        [Tip("将系统时间偏移参数指定的时间跨度后显示；单位为:滴答，1秒=10000000滴答，即1滴答=0.0000001秒;", "Display after the time span specified by the system time offset parameter; Unit: tick, 1 second = 10000000 tick, i.e. 1 tick = 0.0000001 second;")]
        [HideInSuperInspector(nameof(_dataTimeRule), EValidityCheckType.NotEqual, EDataTimeRule.CurrentSystem)]
        public long timeSpanTicks = 0;

        /// <summary>
        /// 时间跨度
        /// </summary>
        public TimeSpan timeSpan => new TimeSpan(timeSpanTicks);

        /// <summary>
        /// 日期时间
        /// </summary>
        public DateTime dateTime
        {
            get
            {
                try
                {
                    return DateTime.Now + timeSpan;
                }
                catch
                {
                    return DateTime.Now;
                }
            }
        }

        #endregion

        #region 秒表

        /// <summary>
        /// 秒表规则
        /// </summary>
        [Name("秒表规则")]
        public enum EStopwatchRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 启用开始禁用停止
            /// </summary>
            [Name("启用开始禁用停止")]
            BeginOnEnable_EndOnDisable,
        }

        /// <summary>
        /// 秒表规则
        /// </summary>
        [Name("秒表规则")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_dataTimeRule), EValidityCheckType.NotEqual, EDataTimeRule.Stopwatch)]
        public EStopwatchRule _stopwatchRule = EStopwatchRule.BeginOnEnable_EndOnDisable;

        private DateTime _beginStopwatch;

        private bool inStopwatch = false;

        /// <summary>
        /// 开始秒表计数
        /// </summary>
        public void BeginStopwatch()
        {
            if (inStopwatch) return;
            inStopwatch = true;
            _beginStopwatch = DateTime.Now;
        }

        /// <summary>
        /// 结束秒表计数
        /// </summary>
        public void EndStopwatch()
        {
            if (!inStopwatch) return;

            inStopwatch = false;
        }

        /// <summary>
        /// 重置秒表计数
        /// </summary>
        public void ResetStopwatch()
        {
            _beginStopwatch = DateTime.Now;
        }

        /// <summary>
        /// 获取秒表计数值
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetStopwatch() => inStopwatch ? DateTime.Now - _beginStopwatch : TimeSpan.Zero;

        #endregion

        private float time = 0;

        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="dateTime">日期时间</param>
        /// <returns></returns>
        private string Text(DateTime dateTime) => dateTime.Format(format, formatString);

        /// <summary>
        /// 获取文本,基于日期时间规则；
        /// </summary>
        /// <returns></returns>
        public string Text()
        {
            switch (_dataTimeRule)
            {
                case EDataTimeRule.CurrentSystem:
                    {
                        return Text(dateTime);
                    }
                case EDataTimeRule.Stopwatch:
                    {
                        return Text(new DateTime(GetStopwatch().Ticks));
                    }
                default: return "";
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            //保证启用后可以立即刷新
            time = intervalTime;

            switch (_stopwatchRule)
            {
                case EStopwatchRule.BeginOnEnable_EndOnDisable:
                    {
                        BeginStopwatch();
                        break;
                    }
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            switch (_stopwatchRule)
            {
                case EStopwatchRule.BeginOnEnable_EndOnDisable:
                    {
                        EndStopwatch();
                        break;
                    }
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!text) return;

            time += Time.deltaTime;
            if (time < intervalTime) return;
            time = 0;

            text.text = Text();
        }
    }
}
