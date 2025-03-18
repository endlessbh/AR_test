using UnityEditor;
using UnityEngine;
using XCSJ.PluginART;

namespace XCSJ.EditorART
{
    /// <summary>
    /// 编辑器ART辅助类
    /// </summary>
    public static class EditorARTHelper
    {
        /// <summary>
        /// 绘制选中<see cref="ARTManager"/>
        /// </summary>
        public static void DrawSelectARTManager()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("选中[" + ARTHelper.Title + "]管理器") && ARTManager.instance)
            {
                Selection.activeObject = ARTManager.instance;
            }
        }
    }
}
