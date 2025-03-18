using UnityEngine;
using XCSJ.EditorExtension.Base;
using XCSJ.Languages;
using XCSJ.PluginMechanicalMotion.Tools;

namespace XCSJ.EditorMechanicalMotion.Tools
{
    /// <summary>
    /// 平面运动机构检查器
    /// </summary>
    public abstract class PlaneMechanismInspector<T> : MechanismInspector<T> where T : PlaneMechanism
    {
        /// <summary>
        /// 轴心
        /// </summary>
        protected virtual Vector3 center => targetObject._plane.point;

        /// <summary>
        /// 轴
        /// </summary>
        protected virtual Vector3 axis => targetObject._plane.direction;

        /// <summary>
        /// 绘制场景辅助图形
        /// </summary>
        public override void DrawScene()
        {
            DrawCenter();

            DrawCircle();

            DrawArrow();
        }

        /// <summary>
        /// 绘制轴中心
        /// </summary>
        [LanguageTuple("Origin", "原点")]
        protected virtual void DrawCenter()
        {
            // 原点
            SceneGraphHelper.DrawSphere(center, sceneGraphOption.sphereRadius, Tr("Origin"), sceneGraphOption.keyPointColor);
        }

        /// <summary>
        /// 圆形半径
        /// </summary>
        protected virtual float circleRadius => sceneGraphOption.radius;

        /// <summary>
        /// 绘制旋转圆形
        /// </summary>
        protected virtual void DrawCircle()
        {
            // 轮廓线圆
            SceneGraphHelper.DrawSolidCircleWithOutline(center, axis, circleRadius, sceneGraphOption.planeColor, sceneGraphOption.lineColor, false);
        }

        /// <summary>
        /// 绘制旋转轴箭头
        /// </summary>
        [LanguageTuple("Axle", "参考轴")]
        public virtual void DrawArrow()
        {
            SceneGraphHelper.DrawArrow(center, axis, sceneGraphOption.arrowLength, Tr("Axle"));
        }
    }
}