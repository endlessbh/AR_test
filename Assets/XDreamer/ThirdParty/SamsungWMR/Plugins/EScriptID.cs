using XCSJ.Attributes;
using XCSJ.Extension;

namespace XCSJ.PluginSamsungWMR
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38528
        /// </summary>
        public const int Begin = (int)EExtensionID._0x2d;

        /// <summary>
        /// 结束值，38656-1=38655
        /// </summary>
        public const int End = (int)EExtensionID._0x2e - 1;
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
