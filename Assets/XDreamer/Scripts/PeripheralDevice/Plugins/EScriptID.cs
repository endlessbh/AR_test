using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Scripts;

namespace XCSJ.PluginPeripheralDevice
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始35200
        /// </summary>
        public const int Begin = (int)EExtensionID._0x13;

        /// <summary>
        /// 结束35328-1=35327
        /// </summary>
        public const int End = (int)EExtensionID._0x14 - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//35200
        public const int MonoBehaviour = Begin + Fragment * 1;//35224
        public const int StateLib = Begin + Fragment * 2;//35248
        public const int Tools = Begin + Fragment * 3;//35272
        public const int Editor = Begin + Fragment * 4;//35296
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 外部设备输入-目录
        /// <summary>
        /// 拆装修理扩展
        /// </summary>
        [ScriptName("外部设备输入", nameof(PeripheralDevice), EGrammarType.Category)]
        [ScriptDescription("外部设备输入的相关脚本目录；")]
        #endregion
        PeripheralDevice,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
