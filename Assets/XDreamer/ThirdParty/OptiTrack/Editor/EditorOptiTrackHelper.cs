using UnityEditor;
using UnityEngine;
using XCSJ.PluginOptiTrack;

namespace XCSJ.EditorOptiTrack
{
    /// <summary>
    /// 编辑器OptiTrack辅助类
    /// </summary>
    public static class EditorOptiTrackHelper
    {
        /// <summary>
        /// 绘制选中<see cref="OptiTrackManager"/>
        /// </summary>
        public static void DrawSelectOptiTrackManager()
        {
            EditorGUILayout.Separator();
            if (GUILayout.Button("选中[" + OptiTrackHelper.Title + "]管理器") && OptiTrackManager.instance)
            {
                Selection.activeObject = OptiTrackManager.instance;
            }
        }
    }
}
