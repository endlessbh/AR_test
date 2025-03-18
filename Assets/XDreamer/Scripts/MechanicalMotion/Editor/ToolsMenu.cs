using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Logs.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginMechanicalMotion;

namespace XCSJ.EditorMechanicalMotion
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 平面运动机构速度设置面板
        /// </summary>
        /// <param name="toolContext"></param>
        /// <returns></returns>
        [Name("平面运动机构速度设置面板")]
        [XCSJ.Attributes.Icon(EIcon.UI)]
        [Tool(MechanicalMotionManager.Title, nameof(MechanicalMotionManager), rootType = typeof(Canvas), groupRule = EToolGroupRule.None, needRootParentIsNull = true)]
        [RequireManager(typeof(MechanicalMotionManager))]
        [Manual(typeof(MechanicalMotionManager))]
        public static void CreatePlaneMechanismVelocityPanel(ToolContext toolContext)
        {
            EditorXGUI.ToolsMenu.CreateUIInCanvas(() => EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath(MechanicalMotionManager.Title + "/UI/平面运动机构速度设置面板.prefab"));
        }
    }
}