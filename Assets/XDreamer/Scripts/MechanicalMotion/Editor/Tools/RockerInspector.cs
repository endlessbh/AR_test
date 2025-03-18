using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base;
using XCSJ.Languages;
using XCSJ.PluginMechanicalMotion.Tools;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 摇杆检查器
    /// </summary>
    [Name("摇杆检查器")]
    [CustomEditor(typeof(Rocker))]
    [CanEditMultipleObjects]
    public class RockerInspector : MechanismInspector<Rocker>
    {
        /// <summary>
        /// 场景绘制
        /// </summary>
        [LanguageTuple("Axle", "参考轴")]
        [LanguageTuple("Origin", "原点")]
        public override void DrawScene()
        {
            var origin = targetObject.transform.position;

            // 圆心
            SceneGraphHelper.DrawSphere(origin, sceneGraphOption.sphereRadius, Tr("Origin"), sceneGraphOption.keyPointColor);
            SceneGraphHelper.DrawArrow(origin, targetObject._lookatUpAxis.data, sceneGraphOption.arrowLength, Tr("Axle"));

            if (targetObject._lookatJoint)
            {
                SceneGraphHelper.DrawSphereArrow(origin, targetObject._lookatJoint.position, sceneGraphOption.sphereRadius, TrLabelByTarget(nameof(Rocker._lookatJoint)).text, SceneGraphOption.weakInstance.keyPointColor, SceneGraphOption.weakInstance.lineColor);
            }
        }
    }
}