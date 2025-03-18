using UnityEditor;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Base;
using XCSJ.EditorTools.PropertyDatas;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginXRSpaceSolution;
using XCSJ.PluginXRSpaceSolution.Tools;
using XCSJ.PluginXXR.Interaction.Toolkit;

namespace XCSJ.EditorXRSpaceSolution.Tools
{
    /// <summary>
    /// XR空间检查器
    /// </summary>
    [CustomEditor(typeof(XRSpace))]
    [Name("XR空间检查器")]
    public class XRSpaceInspector : InteractProviderInspector<XRSpace>
    {
        /// <summary>
        /// 所有屏幕相机关联
        /// </summary>
        [Name("所有屏幕相机关联")]
        [Tip("标识当前XR空间对象所管理的屏幕与相机是否全部已关联，即相机投影组件全部启用", "All XR space objects in the current scene")]
        public bool _allLink = true;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var linked = mb.AllScreenCameraLinked();
            var newLink = EditorGUILayout.Toggle(TrLabel(nameof(_allLink)), linked);
            if (linked != newLink)
            {
                mb.SetScreenCameraLink(newLink);
            }

            EditorXRSpaceSolutionHelper.DrawSelectManager();
        }
    }
}
