using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCameras.Base;
using XCSJ.PluginOptiTrack;
using XCSJ.PluginOptiTrack.Tools;

namespace XCSJ.EditorOptiTrack.Tools
{
    /// <summary>
    /// 相机变换通过OptiTrack检查器
    /// </summary>
    [Name("相机变换通过OptiTrack检查器")]
    [CustomEditor(typeof(CameraTransformByOptiTrack))]
    public class CameraTransformByOptiTrackInspector : BaseCameraToolControllerInspector<CameraTransformByOptiTrack>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorOptiTrackHelper.DrawSelectOptiTrackManager();
        }
    }
}
