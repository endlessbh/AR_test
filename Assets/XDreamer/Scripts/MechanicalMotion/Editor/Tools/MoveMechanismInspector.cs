using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.Languages;
using XCSJ.PluginMechanicalMotion.Tools;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 移动机构检查器
    /// </summary>
    [Name("移动机构检查器")]
    [CustomEditor(typeof(MoveMechanism), true)]
    [CanEditMultipleObjects]
    public class MoveMechanismInspector : PlaneMechanismInspector<MoveMechanism>
    {
        /// <summary>
        /// 场景绘制
        /// </summary>
        public override void DrawScene()
        {
            // 初始点
            var dir = targetObject.initDirection;
            Vector3 initPoint = (targetObject.motionTarget != targetObject.transform) ? targetObject.transform.position
                : targetObject._plane.point - dir * (float)targetObject.offsetValue;

            SceneGraphHelper.DrawLimitLine(initPoint, dir, (float)targetObject.currentValue, (float)targetObject.minValue, (float)targetObject.maxValue);
        }
    }
}