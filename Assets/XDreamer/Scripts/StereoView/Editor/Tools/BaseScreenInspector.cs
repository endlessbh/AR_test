using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginStereoView.Tools;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 基础屏幕检查器
    /// </summary>
    [Name("基础屏幕检查器")]
    [CustomEditor(typeof(BaseScreen),true)]
    public class BaseScreenInspector : MBInspector<BaseScreen>
    {
    }

    /// <summary>
    /// 基础屏幕检查器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseScreenInspector<T> : MBInspector<BaseScreen>
        where T : BaseScreen
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("屏幕变更"))
            {
                targetObject.CallScreenChanged();
            }

            EditorStereoViewHelper.DrawSelectManager();
        }
    }
}
