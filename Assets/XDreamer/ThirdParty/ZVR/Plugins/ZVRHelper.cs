using XCSJ.Extension;

namespace XCSJ.PluginZVR
{
    /// <summary>
    /// ZVR助手类
    /// </summary>
    public static class ZVRHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "ZVR";
    }

    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38016
        /// </summary>
        public const int Begin = (int)EExtensionID._0x29;

        /// <summary>
        /// 结束值，38144-1=38143
        /// </summary>
        public const int End = (int)EExtensionID._0x2a - 1;
    }
}
