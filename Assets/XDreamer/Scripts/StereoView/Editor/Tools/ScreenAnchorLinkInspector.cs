using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 屏幕锚点关联检查器
    /// </summary>
    [Name("屏幕锚点关联检查器")]
    [CustomEditor(typeof(ScreenAnchorLink))]
    public class ScreenAnchorLinkInspector : MBInspector<ScreenAnchorLink>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("更新屏幕TRS"))
            {
                targetObject.UpdateScreen();
            }
        }
    }
}
