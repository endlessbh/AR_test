using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.EditorExtension.Base;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 旋转机构检查器
    /// </summary>
    [Name("旋转机构检查器")]
    [CustomEditor(typeof(RotationMechanism))]
    [CanEditMultipleObjects]
    public class RotationMechanismInspector : PlaneMechanismInspector<RotationMechanism>
    {
        /// <summary>
        /// 圆形半径
        /// </summary>
        protected override float circleRadius => (float)targetObject._radius;

        /// <summary>
        /// 绘制扇形或圆形
        /// </summary>
        protected override void DrawCircle()
        {
            if (targetObject._isLimit)
            {
                var transform = targetObject.motionTarget;
                var radius = sceneGraphOption.sphereRadius * 5;
                var lineColor = sceneGraphOption.lineColor;
                var sphereRadius = sceneGraphOption.smallSphereRadius;

                // 旋转轴
                var axle = axis;

                // 中心
                var c = center;

                var planeNormal = targetObject._plane.direction;

                // 初始朝向
                var initDir = Quaternion.AngleAxis(-(float) targetObject.offsetValue, planeNormal) * targetObject.initDirection;
                SceneGraphHelper.DrawSphereArrow(c, initDir, radius, sphereRadius, "0", Color.yellow, Color.yellow);

                // 最小值
                var minValue = (float)targetObject.minValue;
                var from = Quaternion.AngleAxis(minValue, planeNormal) * initDir;
                if (!Mathf.Approximately(minValue, 0))
                {
                    SceneGraphHelper.DrawSphereArrow(c, from, radius, sphereRadius, minValue.ToString(), lineColor, lineColor);
                }

                // 当前值
                var currentValue = (float)targetObject.currentValue;
                var current = Quaternion.AngleAxis(currentValue, planeNormal) * initDir;
                if (!Mathf.Approximately(currentValue, 0))
                {
                    SceneGraphHelper.DrawSphereArrow(c, current, radius, sphereRadius, currentValue.ToString(), Color.red, Color.red);
                }

                // 最大值
                var maxValue = (float)targetObject.maxValue;
                var to = Quaternion.AngleAxis(maxValue, planeNormal) * initDir;
                if (!Mathf.Approximately(maxValue, 0))
                {
                    SceneGraphHelper.DrawSphereArrow(c, to, radius, sphereRadius, maxValue.ToString(), lineColor, lineColor);
                }

                // 扇形
                float range = (float)(targetObject.maxValue - targetObject.minValue);
                var r = (float)targetObject._radius;
                SceneGraphHelper.DrawWireArc(c, axle, from, range, r, lineColor, false);
                SceneGraphHelper.DrawSolidArc(c, axle, from, range, r, sceneGraphOption.planeColor, false);
            }
            else
            {
                base.DrawCircle();
            }
        }
    }
}