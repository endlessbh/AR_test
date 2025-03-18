using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginPhysicses.Tools.Gadgets;

namespace XCSJ.EditorPhysicses.Tools.Gadgets
{
    /// <summary>
    /// 旋转机关检查器
    /// </summary>
    [Name("旋转机关检查器")]
    [CustomEditor(typeof(RotationJoint))]
    [CanEditMultipleObjects]
    public class RotationJointInspector : InteractorInspector<RotationJoint>
    {
        /// <summary>
        /// 场景图像首选项
        /// </summary>
        private SceneGraphOption sceneGraphOption;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            sceneGraphOption = SceneGraphOption.weakInstance;
        }

        [LanguageTuple("Init Direction", "初始朝向")]
        [LanguageTuple("Rotation Axle", "旋转轴")]
        private void OnSceneGUI()
        {
            var orgin = Application.isPlaying ? targetObject.orgin : targetObject.grabbable.targetTransform;
            var radius = sceneGraphOption.sphereRadius * 4;
            var lineColor = sceneGraphOption.lineColor;
            var sphereRadius = sceneGraphOption.smallSphereRadius;

            // 旋转轴
            var axle = targetObject.worldRotationAxis;

            // 中心
            var center = orgin.position;

            // 旋转轴
            SceneGraphHelper.DrawArrow(center, axle, sceneGraphOption.arrowLength, Tr("Rotation Axle"));

            // 初始朝向
            var initDir = orgin.TransformDirection(targetObject._localInitDirection);

            // 最小值
            var minDirction = Quaternion.AngleAxis(targetObject._range.x, axle) * initDir;

            // 扇形
            float range = targetObject.rangeSize;
            SceneGraphHelper.DrawWireArc(center, axle, minDirction, range, radius, lineColor, false);
            SceneGraphHelper.DrawSolidArc(center, axle, minDirction, range, radius, sceneGraphOption.planeColor, false);

            if (targetObject._useJoint)
            {
                SceneGraphHelper.DrawSphereArrow(center, minDirction, radius, sphereRadius, targetObject.minValue.ToString(), lineColor, lineColor);

                var maxDirction = Quaternion.AngleAxis(targetObject._range.y, axle) * initDir;
                SceneGraphHelper.DrawSphereArrow(center, maxDirction, radius, sphereRadius, targetObject.maxValue.ToString(), lineColor, lineColor);
            }
            else
            {
                for (int i = 0; i <= targetObject._stepCount; i++)
                {
                    var currentDir = Quaternion.AngleAxis(targetObject.stepSize * i, axle) * minDirction;

                    SceneGraphHelper.DrawSphereArrow(center, currentDir, radius, sphereRadius, i.ToString(), lineColor, lineColor);
                }
            }

            ///初始朝向
            SceneGraphHelper.DrawSphereArrow(center, initDir, radius/2, sphereRadius, Tr("Init Direction"), Color.red, Color.red);
        }
    }
}