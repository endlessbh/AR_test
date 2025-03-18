using XCSJ.Extension;

namespace XCSJ.PluginTools
{
    /// <summary>
    /// 工具扩展的ID区间
    /// </summary>
    public static class ExtensionIDRange
    {
        /// <summary>
        /// 开始35840
        /// </summary>
        public const int Begin = (int)EExtensionID._0x18;

        /// <summary>
        /// 结束36351
        /// </summary>
        public const int End = (int)EExtensionID._0x1c - 1;

        public const int Fragment = 0x40;//64

        public const int Common = Begin + Fragment * 0;//35840
        public const int MonoBehaviour = Begin + Fragment * 1;//35904
        public const int StateLib = Begin + Fragment * 2;//35968
        public const int Tools = Begin + Fragment * 3;//36032
        public const int Editor = Begin + Fragment * 6;//36224
    }
}
