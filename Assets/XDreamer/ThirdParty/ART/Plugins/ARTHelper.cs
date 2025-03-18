using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Maths;
using XCSJ.PluginART.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginART
{
    /// <summary>
    /// ART组手类
    /// </summary>
    public static class ARTHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "ART";

		/// <summary>
		/// 普通坐标系
		/// </summary>
		public const ECoordinateSystem NormalCoordinateSystem = ECoordinateSystem.XR_YF_ZU;

		/// <summary>
		/// Powerwall坐标系
		/// </summary>
		public const ECoordinateSystem PowerwallCoordinateSystem = ECoordinateSystem.XR_YU_ZB;

		/// <summary>
		/// PowerwallStanding坐标系
		/// </summary>
		public const ECoordinateSystem PowerwallStandingCoordinateSystem = ECoordinateSystem.XL_YF_ZD;
    }

    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38272
        /// </summary>
        public const int Begin = (int)EExtensionID._0x2b;

        /// <summary>
        /// 结束值，38400-1=38399
        /// </summary>
        public const int End = (int)EExtensionID._0x2c - 1;
    }
}
