using XCSJ.Attributes;
using XCSJ.Extension;

namespace XCSJ.PluginXXR.Interaction.Toolkit
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，36480
        /// </summary>
        public const int Begin = (int)EExtensionID._0x1d;

        /// <summary>
        /// 结束值，36608-1=36607
        /// </summary>
        public const int End = (int)EExtensionID._0x1e - 1;
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
