using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorTools;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginStereoView;
using XCSJ.PluginStereoView.Tools;
using XCSJ.PluginTools.Renderers;

namespace XCSJ.EditorStereoView.Tools
{
    /// <summary>
    /// 相机透视检查器
    /// </summary>
    [Name("相机透视检查器")]
    [CustomEditor(typeof(CameraProjection))]
    public class CameraProjectionInspector : MBInspector<CameraProjection>
    {
        /// <summary>
        /// 立体分离
        /// </summary>
        [Name("立体分离")]
        [Tip("虚拟眼睛之间的距离；单位为米；使用此选项可以查询或设置当前的眼睛间距；请注意，大多数VR设备都提供该值，在这种情况下，设置该值将没有任何效果；", "The distance between the virtual eyes. Unit: meter. Use this to query or set the current eye separation. Note that most VR devices provide this value, in which case setting the value will have no effect.")]
        public float stereoSeparation;

        /// <summary>
        /// 立体融合
        /// </summary>
        [Name("立体融合")]
        [Tip("到虚拟眼睛汇聚点的距离；单位为米；", "Distance to a point where virtual eyes converge. Unit: meter.")]
        public float stereoConvergence;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var thisCamera = mb.thisCamera;
            if (thisCamera)
            {
                thisCamera.stereoSeparation = EditorGUILayout.FloatField(TrLabel(nameof(stereoSeparation)), thisCamera.stereoSeparation);
                thisCamera.stereoConvergence = EditorGUILayout.FloatField(TrLabel(nameof(stereoConvergence)), thisCamera.stereoConvergence);
            }
            EditorStereoViewHelper.DrawSelectManager();
        }
    }
}
