using UnityEngine;
using XCSJ.Caches;
using XCSJ.Helper;

namespace XCSJ.PluginVuforia
{
    /// <summary>
    /// Vuforia组手
    /// </summary>
    public static class VuforiaHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Vuforia";

        /// <summary>
        /// 无效版本
        /// </summary>
        public const string InvalidVersion = "0";

        /// <summary>
        /// 版本：在Vuforia编译宏存在时直接返回其版本
        /// </summary>
        public static string Version
        {
            get
            {
#if XDREAMER_VUFORIA
                return Vuforia.VuforiaConfiguration.Instance.Vuforia.Version;
#else
                return InvalidVersion;
#endif
            }
        }

        /// <summary>
        /// 版本弱：用反射方式尝试获取Vuforia版本
        /// </summary>
        public static string VersionWeak
        {
            get
            {
                try
                {
                    if (TypeHelper.ExistsAndAssemblyFileExists("AddVuforiaEnginePackage", out var type))
                    {
                        var fieldInfo = FieldInfoCache.Get(type, "VUFORIA_VERSION");
                        var version = TypeHelper.GetValue(fieldInfo, null) as string;
                        if (!string.IsNullOrEmpty(version)) return version;
                    }
                    if (TypeHelper.ExistsAndAssemblyFileExists("Vuforia.VuforiaConfiguration", out var type1))
                    {
                        var propertyInfo_Instance = PropertyInfoCache.Get(type1, "Instance");
                        var Instance = TypeHelper.GetValue(propertyInfo_Instance, default(object));
                        if (Instance != null)
                        {
                            var propertyInfo_Vuforia = PropertyInfoCache.Get(type1, "Vuforia");
                            var Vuforia = TypeHelper.GetValue(propertyInfo_Vuforia, Instance);
                            if (Vuforia != null)
                            {
                                var propertyInfo_Version = PropertyInfoCache.Get(Vuforia.GetType(), "Version");
                                var Version = TypeHelper.GetValue(propertyInfo_Version, Vuforia) as string;
                                if (!string.IsNullOrEmpty(Version)) return Version;
                            }
                        }
                    }
                }
                catch { }
                return InvalidVersion;
            }
        }
    }
}
