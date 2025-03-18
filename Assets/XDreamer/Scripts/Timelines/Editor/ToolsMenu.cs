using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSMS;
using XCSJ.PluginTimelines;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginXGUI;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginTimelines.Tools;
using XCSJ.PluginTimelines.UI;

namespace XCSJ.EditorTimelines
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        /// <summary>
        /// 视频播放器界面
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(TimelineManager.ToolLib, nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool("常用", nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Name("视频播放器")]
        [XCSJ.Attributes.Icon(EIcon.Video)]
        [RequireManager(typeof(TimelineManager))]
        [Manual(typeof(VideoPlayableContent))]
        public static void CreateVideoPlayer(ToolContext toolContext)
        {
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("时间轴/视频播放窗口.prefab"));
        }

        /// <summary>
        /// 可播放内容列表
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("可播放内容列表")]
        [XCSJ.Attributes.Icon(EIcon.List)]
        [RequireManager(typeof(TimelineManager))]
        [Manual(typeof(PlayableContentList))]
        [Tool(TimelineManager.Title, nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool(TimelineManager.ToolLib, nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool(XGUICategory.Table, nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        public static void CreatePlayableContentList(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("时间轴/可播放内容列表.prefab");
            if (go)
            {
                EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
                var rectTransform = go.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.XStretchHV();
                }
            }
        }

        /// <summary>
        /// 创建时间轴播放器
        /// </summary>
        /// <param name="toolContext"></param>
        [Name("时间轴播放器")]
        [XCSJ.Attributes.Icon(EIcon.Play)]
        [Manual(typeof(PlayerController))]
        [Tool(XGUICategory.Window, nameof(XGUIManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [Tool(TimelineManager.Title, nameof(TimelineManager), rootType = typeof(Canvas), needRootParentIsNull = true, groupRule = EToolGroupRule.None)]
        [RequireManager(typeof(XGUIManager), typeof(ToolsManager), typeof(SMSManager), typeof(TimelineManager))]
        public static GameObject CreateTimeLinePlayer(ToolContext toolContext)
        {
            var go = EditorToolsHelperExtension.LoadPrefab_DefaultXDreamerPath("时间轴/时间轴播放器界面.prefab");
            EditorToolsHelperExtension.FindOrCreateRootAndGroup(toolContext, go);
            return go;
        }
    }
}
