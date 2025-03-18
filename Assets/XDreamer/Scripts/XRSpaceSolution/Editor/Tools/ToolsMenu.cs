using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginHTCVive;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorXRSpaceSolution.Tools
{
    /// <summary>
    /// 工具库菜单
    /// </summary>
    [LanguageFileOutput]
    public static class ToolsMenu
    {
        const string XRSpaceTitle = "单机多通道XR交互空间";

        /// <summary>
        /// 创建XR空间，创建单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；
        /// </summary>
        /// <param name="toolContext"></param>
        [Tool(XRITHelper.SpaceSolution)]
        [Name(XRSpaceTitle)]
        [Tip("创建单机多通道动立体虚拟屏幕等工具组件构建的XR交互空间；", "Create XR interactive space constructed by single machine multi-channel dynamic three-dimensional virtual screen and other tool components;")]
        [XCSJ.Attributes.Icon(EIcon.State)]
        [RequireManager(typeof(XRSpaceSolutionManager), typeof(StereoViewManager))]
        [Manual(typeof(XRSpaceSolutionManager))]
        public static void CreateXRSpace_MultiScreen_ActiveStereo(ToolContext toolContext)
        {
            EditorXRSpaceSolutionHelper.CreateXRIS(XRSpaceTitle);
        }
    }
}
