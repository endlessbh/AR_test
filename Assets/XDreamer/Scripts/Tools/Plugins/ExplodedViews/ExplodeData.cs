using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.ExplodedViews
{
    /// <summary>
    /// 爆炸数据
    /// </summary>
    [Serializable]
    [Name("爆炸数据")]
    public class ExplodeData : IRecover
    {
        /// <summary>
        /// 变换
        /// </summary>
        public Transform transform;

        /// <summary>
        /// 变换的原始位置
        /// </summary>
        public Vector3 oriPosition;

        /// <summary>
        /// 标识当前数据是否有效
        /// </summary>
        public bool valid = false;

        /// <summary>
        /// 起始位置
        /// </summary>
        public Vector3 startPosition;

        /// <summary>
        /// 结束位置
        /// </summary>
        public Vector3 endPosition;

        /// <summary>
        /// 包围盒
        /// </summary>
        public Bounds bounds;

        /// <summary>
        /// 爆炸中心
        /// </summary>
        public Vector3 center;

        /// <summary>
        /// 爆炸方向
        /// </summary>
        public Vector3 dir;

        /// <summary>
        /// 爆炸系数
        /// </summary>
        public double explodeValue = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public ExplodeData() { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="transform"></param>
        public ExplodeData(Transform transform)
        {
            this.Init(transform);
        }

        /// <summary>
        /// 初始化变换；仅可调用一次；
        /// </summary>
        /// <param name="transform"></param>
        public void Init(Transform transform)
        {
            if (this.transform) throw new InvalidProgramException("数据已被初始化，不可重复初始化！");
            this.transform = transform;
            if (transform && CommonFun.GetBounds(out bounds, transform.gameObject))
            {
                oriPosition = transform.position;
                startPosition = endPosition = bounds.center;
                valid = true;
            }
            else
            {
                valid = false;
            }
        }

        /// <summary>
        /// 设置爆炸的中心与方向
        /// </summary>
        /// <param name="center"></param>
        /// <param name="dir"></param>
        public void Init(Vector3 center, Vector3 dir)
        {
            this.center = center;
            if (MathX.ApproximatelyZero(dir.magnitude))
            {
                this.dir = Vector3.zero;
            }
            else
            {
                this.dir = dir.normalized;
            }
        }

        /// <summary>
        /// 开始爆炸
        /// </summary>
        public void BeginExplode()
        {
            this.explodeValue = 0;
            bounds.center = startPosition;
        }

        /// <summary>
        /// 更新爆炸
        /// </summary>
        /// <param name="intervalValue"></param>
        public void UpdateExplode(double intervalValue)
        {
            this.explodeValue += intervalValue;
            bounds.center = startPosition + dir * (float)this.explodeValue;
        }

        /// <summary>
        /// 结束爆炸
        /// </summary>
        public void EndExplode()
        {
            bounds.center = endPosition = startPosition + dir * (float)this.explodeValue;
        }

        /// <summary>
        /// 恢复；数据、变换位置恢复到爆炸前的状态
        /// </summary>
        public void Recover()
        {
            if (valid)
            {
                explodeValue = 0;
                bounds.center = endPosition = startPosition;

                if (transform) transform.position = oriPosition;
            }
        }

        /// <summary>
        /// 更新百分比
        /// </summary>
        /// <param name="percent"></param>
        public void UpatePercent(double percent)
        {
            bounds.center = Vector3.LerpUnclamped(startPosition, endPosition, (float)percent);
        }

        /// <summary>
        /// 更新变换的位置信息
        /// </summary>
        public void UpdateTranform()
        {
            if (valid)
            {
                transform.position = oriPosition + bounds.center - startPosition;
            }
        }
    }
}
