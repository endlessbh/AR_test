using XCSJ.Extension;

namespace XCSJ.PluginVuforia.CNScripts
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始
        /// </summary>
        public const int Begin = (int)EExtensionID._0x32;

        /// <summary>
        /// 结束
        /// </summary>
        public const int End = (int)EExtensionID._0x33 - 1;
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

    }
}
