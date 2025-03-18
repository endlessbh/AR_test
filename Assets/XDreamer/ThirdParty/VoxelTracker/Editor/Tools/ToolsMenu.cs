using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSamsungWMR;
using XCSJ.PluginVoxelTracker;

namespace XCSJ.EditorVoxelTracker.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        private static bool CheckPackage()
        {
#if XDREAMER_VOXELTRACKER
            return true;
#else
            Debug.LogWarning("插件[" + VoxelTrackerHelper.Title + "]依赖库缺失！");
            return false;
#endif
        }

        /// <summary>
        /// VoxelStation官方预制体
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(VoxelTrackerHelper.Title)]
        [Name("VoxelStation官方预制体")]
        [XCSJ.Attributes.Icon(EIcon.GameObject)]
        [RequireManager(typeof(VoxelTrackerManager))]
        [Manual(typeof(VoxelTrackerManager))]
        public static void CreateDefaultVoxelStation(ToolContext toolContext)
        {
            if (!CheckPackage()) return;
            EditorToolsHelperExtension.ClonePrefab("Assets/VoxelStation/Prefabs/VoxelStation.prefab");
        }
    }
}
