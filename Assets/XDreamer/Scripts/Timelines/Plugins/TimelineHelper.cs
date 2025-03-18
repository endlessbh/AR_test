using System;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.PluginTimelines
{
    /// <summary>
    /// 时间轴助手
    /// </summary>
    public static class TimelineHelper
    {
        /// <summary>
        /// 播放时间转换函数
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="timeFormat"></param>
        /// <returns></returns>
        public static string ConvertPlayerTimeFormat(double seconds, EPlayerTimeFormat timeFormat)
        {
            try
            {
                switch (timeFormat)
                {
                    case EPlayerTimeFormat.s:
                        {
                            return seconds.ToString();
                        }
                    case EPlayerTimeFormat.f:
                        {
                            return (seconds * 1000).ToString();
                        }
                    case EPlayerTimeFormat.mm__ss:
                        {
                            return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
                        }
                    case EPlayerTimeFormat.hh__mm__ss:
                        {
                            return TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
                        }
                    default: return "";
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return "";
            }
        }
    }


    /// <summary>
    /// 播放时间格式
    /// </summary>
    [Name("播放时间格式")]
    public enum EPlayerTimeFormat
    {
        [Name("秒")]
        s = 0,

        [Name("分:秒")]
        mm__ss,

        [Name("时:分:秒")]
        hh__mm__ss,

        [Name("毫秒")]
        f,
    }
}
