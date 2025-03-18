using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMechanicalMotion;
using XCSJ.PluginPhysicses.Tools.Weapons;
using XCSJ.PluginTools;

namespace XCSJ.EditorPhysicses
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 物理系统发射器
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool("物理系统", rootType = typeof(ToolsManager), groupRule = EToolGroupRule.None)]
        [Name("发射器")]
        [XCSJ.Attributes.Icon(EIcon.Mono)]
        [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
        [Manual(typeof(Shooter))]
        public static void CreateGun(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("物理系统/发射器.prefab"));
        }
    }
}
