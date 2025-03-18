using UnityEditor;
using UnityEngine;
using XCSJ.PluginXBox;

namespace XCSJ.EditorXBox
{
    /// <summary>
    /// 编辑器XBox辅助类
    /// </summary>
    public static class EditorXBoxHelper
    {
        /// <summary>
        /// 绘制选中<see cref="XBoxManager"/>
        /// </summary>
        public static void DrawSelectXBoxManager()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("选中[" + XBoxHelper.Title + "]管理器") && XBoxManager.instance)
            {
                Selection.activeObject = XBoxManager.instance;
            }
        }
    }
}
