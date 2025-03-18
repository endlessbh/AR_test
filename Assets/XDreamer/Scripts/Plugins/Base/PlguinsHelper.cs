using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Kernel;
using XCSJ.Extension.CNScripts;
using XCSJ.Extension.GenericStandards;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Products;

namespace XCSJ.Extension.Base
{
    /// <summary>
    /// 插件组手类
    /// </summary>
    public static class PlguinsHelper
    {
        /// <summary>
        /// UnityEngine前缀
        /// </summary>
        public const string UnityEngine_Prefix = nameof(UnityEngine) + ".";

        /// <summary>
        /// UnityEngineInternal前缀
        /// </summary>
        public const string UnityEngineInternal_Prefix = nameof(UnityEngineInternal) + ".";

        /// <summary>
        /// UnityEngine.EventSystems前缀
        /// </summary>
        public const string UnityEngine_EventSystems_Prefix = UnityEngine_Prefix + nameof(UnityEngine.EventSystems) + ".";

        /// <summary>
        /// UnityEngine.UI前缀
        /// </summary>
        public const string UnityEngine_UI_Prefix = UnityEngine_Prefix + nameof(UnityEngine.UI) + ".";

        private static bool initialized = false;

        /// <summary>
        /// 初始化
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            ConverterExtension.Init();
            PluginsHandlerExtension.Init();
            HelperInit();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            XDreamer.getLocalDogServerPath += GetLocalDogServerPath;
#endif
        }

        private static void HelperInit()
        {
            ComponentManagerExtension.Init();
            TypeCacheExtension.Init();
            CNScriptHelper.Init();
        }

        /// <summary>
        /// 获取获取本地加密狗服务路径：仅在Windows平台可用，通过注册表方式查找当前计算机上已安装的XDreamer认证服务；
        /// </summary>
        /// <returns></returns>
        public static LocalDogServerPath GetLocalDogServerPath()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

            //当前工程对应的设置中获取
            var exePath = PlayerPrefs.GetString(ProductServer.AuthenticationServer.LastLocalDogServerPath, "");
            if (FileHelper.Exists(exePath))
            {
                return new LocalDogServerPath(exePath, 1);
            }

#if UNITY_EDITOR

            //在Unity编辑器内时，从软件官方的公司产品配置中获取
            var companyName = UnityEditor.PlayerSettings.companyName;
            var productName = UnityEditor.PlayerSettings.productName;
            try
            {
                UnityEditor.PlayerSettings.companyName = nameof(XCSJ);
                UnityEditor.PlayerSettings.productName = ProductServer.Name;

                exePath = PlayerPrefs.GetString(ProductServer.AuthenticationServer.LastLocalDogServerPath, "");
                if (FileHelper.Exists(exePath))
                {
                    return new LocalDogServerPath(exePath, 1);
                }
            }
            finally
            {
                UnityEditor.PlayerSettings.companyName = companyName;
                UnityEditor.PlayerSettings.productName = productName;
            }
#endif
            return new LocalDogServerPath("", 1);
#else
            return default;
#endif

        }
    }
}
