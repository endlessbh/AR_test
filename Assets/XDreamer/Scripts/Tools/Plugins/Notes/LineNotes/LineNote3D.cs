using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 标注线
    /// </summary>
    public abstract class LineNote3D : LineNote
    {
        [Name("终点目标")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject endTarget;

        [Name("可见夹角")]
        [Tip("相机朝向的反方向与注释起点终点向量构成的夹角。", "The angle between the opposite direction of the camera and the annotation start and end vector.")]
        [Range(0,180)]
        public float visibleAngle = 180;

        private bool biggerThanVisibleAngle = true;

        /// <summary>
        /// 显示
        /// </summary>
        protected override bool display => !biggerThanVisibleAngle && base.display;

        /// <summary>
        /// 结束点
        /// </summary>
        public override Vector3 endPoint => endTarget ? endTarget.transform.position : Vector3.zero;
        
        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (cam)
            {
                biggerThanVisibleAngle = Vector3.Angle(-cam.transform.forward, (endPoint - beginPoint)) < visibleAngle ? false : true;
            }
        }

        /// <summary>
        /// 绘制线
        /// </summary>
        protected void OnDrawGizmos()
        {
            XGizmos.DrawPath(ELineType.Liner, new Vector3[2] { beginPoint, endPoint}, Color.magenta);
        }
    }
}
