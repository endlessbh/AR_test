using UnityEditor;
using XCSJ.Extension.Base.InputSystems;
using XCSJ.Helper;

namespace XCSJ.EditorExtension.Base
{
    /// <summary>
    /// 输入系统辅助类
    /// </summary>
    public static class EditorInputSystemHelper
    {
        /// <summary>
        /// 输入系统宏：本宏如果被定义说明Unity输入系统启用并且对应的包存在；
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM = new Macro(nameof(XDREAMER_INPUT_SYSTEM), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统Android宏：Unity输入系统启用并且为Android平台
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_ANDROID = new Macro(nameof(XDREAMER_INPUT_SYSTEM_ANDROID), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统iOS宏：Unity输入系统启用并且为iOS平台
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_IOS = new Macro(nameof(XDREAMER_INPUT_SYSTEM_IOS), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统WebGL宏：Unity输入系统启用并且为WebGL平台
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_WEBGL = new Macro(nameof(XDREAMER_INPUT_SYSTEM_WEBGL), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 输入系统Standalone宏：Unity输入系统启用并且为Standalone平台
        /// </summary>
        public static readonly Macro XDREAMER_INPUT_SYSTEM_STANDALONE = new Macro(nameof(XDREAMER_INPUT_SYSTEM_STANDALONE), XDreamerEditor.DefaultBuildTargetGroups);

        /// <summary>
        /// 初始化
        /// </summary>
        [Macro]
        public static void Init()
        {
            //编辑器运行时不处理编译宏
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            if (TypeHelper.ExistsAndAssemblyFileExists(InputSystemHelper.InputDeviceTypeFullName))
            {
                XDREAMER_INPUT_SYSTEM.DefineIfNoDefined();
            }
            else
            {
                XDREAMER_INPUT_SYSTEM.UndefineWithSelectedBuildTargetGroup();
            }

#if XDREAMER_INPUT_SYSTEM //输入系统启用情况下

#if UNITY_ANDROID
            XDREAMER_INPUT_SYSTEM_ANDROID.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_ANDROID.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_IOS
            XDREAMER_INPUT_SYSTEM_IOS.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_IOS.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_WEBGL
            XDREAMER_INPUT_SYSTEM_WEBGL.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_WEBGL.UndefineWithSelectedBuildTargetGroup();
#endif

#if UNITY_STANDALONE
            XDREAMER_INPUT_SYSTEM_STANDALONE.DefineIfNoDefined();
#else
            XDREAMER_INPUT_SYSTEM_STANDALONE.UndefineWithSelectedBuildTargetGroup();
#endif

#else //输入系统未启用情况下
            XDREAMER_INPUT_SYSTEM_ANDROID.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_IOS.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_WEBGL.UndefineWithSelectedBuildTargetGroup();
            XDREAMER_INPUT_SYSTEM_STANDALONE.UndefineWithSelectedBuildTargetGroup();

#endif

        }
    }
}
