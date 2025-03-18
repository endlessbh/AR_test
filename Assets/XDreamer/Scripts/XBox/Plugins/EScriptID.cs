using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Scripts;

namespace XCSJ.PluginXBox
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始35328
        /// </summary>
        public const int Begin = (int)EExtensionID._0x14;

        /// <summary>
        /// 结束35456-1=35455
        /// </summary>
        public const int End = (int)EExtensionID._0x15 - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//35328
        public const int MonoBehaviour = Begin + Fragment * 1;//35352
        public const int StateLib = Begin + Fragment * 2;//35376
        public const int Tools = Begin + Fragment * 3;//35400
        public const int Editor = Begin + Fragment * 4;//35424
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        _Begin = IDRange.Begin,

        #region XBox-目录
        /// <summary>
        /// XBox 360
        /// </summary>
        [ScriptName(nameof(XBox), nameof(XBox), EGrammarType.Category)]
        [ScriptDescription("XBox的相关脚本目录；")]
        #endregion
        XBox,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
