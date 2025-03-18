using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;
using XCSJ.PluginPico.CNScripts;
using XCSJ.Helper;
using XCSJ.Caches;

namespace XCSJ.PluginPico
{
    /// <summary>
    /// Pico组手
    /// </summary>
    public class PicoHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "PICO";

        /// <summary>
        /// 无效版本
        /// </summary>
        public const string InvalidVersion = "0";

        /// <summary>
        /// 版本：在Pico编译宏存在时直接返回其版本
        /// </summary>
        public static string Version
        {
            get
            {
#if XDREAMER_PICO
                return Unity.XR.PXR.PXR_Plugin.System.UPxr_GetSDKVersion();
#else
                return InvalidVersion;
#endif
            }
        }

        /// <summary>
        /// 版本弱：用反射方式尝试获取Pico版本
        /// </summary>
        public static string VersionWeak
        {
            get
            {
                try
                {
                    if (TypeHelper.ExistsAndAssemblyFileExists("Unity.XR.PXR.PXR_Plugin+System", out var type1))
                    {
                        var methodInfo = MethodInfoCache.Get(type1, "UPxr_GetSDKVersion");
                        return TypeHelper.GetValue(methodInfo, default(object)) as string ?? InvalidVersion;
                    }
                }
                catch { }
                return InvalidVersion;
            }
        }
    }
}
