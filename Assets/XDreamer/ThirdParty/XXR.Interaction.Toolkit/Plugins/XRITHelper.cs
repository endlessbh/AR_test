using UnityEngine;

namespace XCSJ.PluginXXR.Interaction.Toolkit
{
    /// <summary>
    /// XRIT辅助类:XRIT为XR Interaction Toolkit的简写；
    /// </summary>
    public static class XRITHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XR交互工具包";

        /// <summary>
        /// 标题英文
        /// </summary>
        public const string TitleEN = "XR Interaction Toolkit";

        /// <summary>
        /// 目录名
        /// </summary>
        public const string CategoryName = "XR";

        /// <summary>
        /// XR空间解决方案
        /// </summary>
        public const string SpaceSolution = CategoryName + "空间解决方案";

        /// <summary>
        /// XR输入输出
        /// </summary>
        public const string IO = CategoryName + "输入输出";

        /// <summary>
        /// XR交互
        /// </summary>
        public const string Interact = CategoryName + "交互";

        /// <summary>
        /// XR运动系统
        /// </summary>
        public const string LocomotionSystem = CategoryName + "运动系统";

        /// <summary>
        /// 检查XR交互工具包是否存在
        /// </summary>
        /// <returns></returns>
        public static bool CheckPackage()
        {
#if XDREAMER_XR_INTERACTION_TOOLKIT
            return true;
#else
            Debug.LogWarning("插件[" + XRITHelper.Title + "]依赖库缺失,无法创建！");
            return false;
#endif
        }
    }
}
