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
    /// 曲柄摇杆检查器
    /// </summary>
    [Name("曲柄摇杆检查器")]
    [CustomEditor(typeof(CrankRocker))]
    [CanEditMultipleObjects]
    public class CrankRockerInspector : MechanismInspector<CrankRocker>
    {
        /// <summary>
        /// 场景绘制
        /// </summary>
        public override void DrawScene()
        {
            if (targetObject._crank && targetObject._crank.transform != targetObject.transform)
            {
                var editor = BaseInspector.GetEditor(targetObject._crank) as RotationMechanismInspector;
                if (editor != null)
                {
                    editor.OnSceneGUI();
                }
            }

            if (targetObject.valid)
            {
                var sceneGraphOption = SceneGraphOption.weakInstance;
                var r = sceneGraphOption.sphereRadius;
                var sphereColor = sceneGraphOption.keyPointColor;

                // 连杆
                var pos1 = targetObject._connectingRod.position;
                SceneGraphHelper.DrawSphere(pos1, r, TrLabelByTarget(nameof(CrankRocker._connectingRod)).text, sphereColor);

                // 关节
                var pos2 = targetObject._linkAndRockerJoint.position;
                SceneGraphHelper.DrawSphere(pos2, r, TrLabelByTarget(nameof(CrankRocker._linkAndRockerJoint)).text, sphereColor);

                // 摇杆
                var pos3 = targetObject._rocker.position;
                SceneGraphHelper.DrawSphere(pos3, r, TrLabelByTarget(nameof(CrankRocker._rocker)).text, sphereColor);

                // 连线  
                SceneGraphHelper.DrawColor(sceneGraphOption.lineColor, () => Handles.DrawPolyLine(pos1, pos2, pos3));
            }
        }
        
    }
}