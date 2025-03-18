using System;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.GenericStandards
{
    /// <summary>
    /// 系统云信使信息
    /// </summary>
    public class SystemRuntimeInfo
    {
        /// <summary>
        /// 全屏前的分辨率宽度
        /// </summary>
        public static int width = 1920;

        /// <summary>
        /// 全屏前的分辨率高度
        /// </summary>
        public static int height = 1080;

        /// <summary>
        /// 初始化启动时分辨率宽度
        /// </summary>
        public static readonly int initWidth = 1920;

        /// <summary>
        /// 初始化启动时分辨率高度
        /// </summary>
        public static readonly int initHeight = 1080;

        /// <summary>
        /// 初始化启动时全屏状态
        /// </summary>
        public static readonly bool initFullScreen = false;

        /// <summary>
        /// 系统的启动时间；
        /// </summary>
        public static DateTime startTime => Product.startTime;

        static SystemRuntimeInfo()
        {
            initFullScreen = Screen.fullScreen;
            initHeight = Screen.height;
            initWidth = Screen.width;
        }
    }
}