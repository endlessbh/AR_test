using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXBox.Tools;

namespace XCSJ.EditorXBox.Tools
{
    /// <summary>
    /// 变换通过XBox检查器
    /// </summary>
    [Name("变换通过XBox检查器")]
    [CustomEditor(typeof(TransformByXBox), true)]
    public class TransformByXBoxInspector : MBInspector<TransformByXBox>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorXBoxHelper.DrawSelectXBoxManager();

            if (GUILayout.Button("设置为默认 移动"))
            {
                targetObject.SetDefaultMove();
            }
            if (GUILayout.Button("设置为默认 世界Y旋转"))
            {
                targetObject.SetDefaultRotateWorldY();
            }
            if (GUILayout.Button("设置为默认 本地X旋转"))
            {
                targetObject.SetDefaultRotateLocalX();
            }
        }
    }
}
