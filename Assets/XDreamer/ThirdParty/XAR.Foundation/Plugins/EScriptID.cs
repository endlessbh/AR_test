using XCSJ.Attributes;
using XCSJ.Extension;

namespace XCSJ.PluginXAR.Foundation
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，36864
        /// </summary>
        public const int Begin = (int)EExtensionID._0x20;

        /// <summary>
        /// 结束值，36992-1=36991
        /// </summary>
        public const int End = (int)EExtensionID._0x21 - 1;
    }

    /// <summary>
    /// 脚本ID枚举
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 开始值
        /// </summary>
        _Begin = IDRange.Begin,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
