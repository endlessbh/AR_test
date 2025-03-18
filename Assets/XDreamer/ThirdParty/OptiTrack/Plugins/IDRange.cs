using XCSJ.Extension;

namespace XCSJ.PluginOptiTrack
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，37888
        /// </summary>
        public const int Begin = (int)EExtensionID._0x28;

        /// <summary>
        /// 结束值，38016-1=38015
        /// </summary>
        public const int End = (int)EExtensionID._0x29 - 1;
    }
}
