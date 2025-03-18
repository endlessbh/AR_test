using XCSJ.Extension;

namespace XCSJ.PluginVoxelTracker
{
    /// <summary>
    /// VoxelTracker辅助类
    /// </summary>
    public static class VoxelTrackerHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Voxel Tracker";
    }

    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38400
        /// </summary>
        public const int Begin = (int)EExtensionID._0x2c;

        /// <summary>
        /// 结束值，38528-1=38527
        /// </summary>
        public const int End = (int)EExtensionID._0x2d - 1;
    }
}
