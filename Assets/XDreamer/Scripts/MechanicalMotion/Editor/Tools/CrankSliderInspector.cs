using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMechanicalMotion.Tools;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 曲柄滑块检查器
    /// </summary>
    [Name("曲柄滑块检查器")]
    [CustomEditor(typeof(CrankSlider))]
    [CanEditMultipleObjects]
    public class CrankSliderInspector : MechanismInspector<CrankSlider>
    {
        /// <summary>
        /// 场景绘制
        /// </summary>
        public override void DrawScene()
        {
            if (targetObject && targetObject.valid)
            {
                var sceneGraphOption = SceneGraphOption.weakInstance;
                var r = sceneGraphOption.sphereRadius;
                var sphereColor = sceneGraphOption.keyPointColor;

                // 连杆
                var pos1 = targetObject._connectingRod.position;
                SceneGraphHelper.DrawSphere(pos1, r, TrLabelByTarget(nameof(CrankSlider._connectingRod)).text, sphereColor);

                // 滑块
                var pos2 = targetObject._slider.position;
                SceneGraphHelper.DrawSphere(targetObject._slider.position, r, TrLabelByTarget(nameof(CrankSlider._slider)).text, sphereColor);

                // 连线  
                SceneGraphHelper.DrawColor(sceneGraphOption.lineColor, () => Handles.DrawPolyLine(pos1, pos2));
            }
        }

    }
}