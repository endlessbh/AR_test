using XCSJ.Attributes;
using XCSJ.Extension;

namespace XCSJ.PluginXRSpaceSolution.CNScripts
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38656
        /// </summary>
        public const int Begin = (int)EExtensionID._0x2e;

        /// <summary>
        /// 结束值，38784-1=38783
        /// </summary>
        public const int End = (int)EExtensionID._0x2f - 1;
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptsID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,
    }
}
