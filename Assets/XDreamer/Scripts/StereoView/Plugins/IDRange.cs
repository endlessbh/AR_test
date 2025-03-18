using XCSJ.Extension;

namespace XCSJ.PluginStereoView
{
    /// <summary>
    /// ID区间
    /// </summary>
    public static class IDRange
    {
        /// <summary>
        /// 开始，35584
        /// </summary>
        public const int Begin = (int)EExtensionID._0x16;

        /// <summary>
        /// 结束，35712-1=35711
        /// </summary>
        public const int End = (int)EExtensionID._0x17 - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//35584
        public const int MonoBehaviour = Begin + Fragment * 1;//35608
        public const int StateLib = Begin + Fragment * 2;//35632
        public const int Tools = Begin + Fragment * 3;//35656
        public const int Editor = Begin + Fragment * 4;//35680
    }
}
