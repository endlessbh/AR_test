using UnityEditor;
using UnityEngine;
using XCSJ.PluginZVR;

namespace XCSJ.EditorZVR
{
    /// <summary>
    /// 编辑器ZVR辅助类
    /// </summary>
    public static class EditorZVRHelper
    {
        /// <summary>
        /// 绘制选中<see cref="ZVRManager"/>
        /// </summary>
        public static void DrawSelectZVRManager()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("选中[" + ZVRHelper.Title + "]管理器") && ZVRManager.instance)
            {
                Selection.activeObject = ZVRManager.instance;
            }
        }
    }
}
