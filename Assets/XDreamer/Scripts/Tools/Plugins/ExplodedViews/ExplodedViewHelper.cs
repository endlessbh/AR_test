using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Extension.Base.Maths;
using XCSJ.Maths;

namespace XCSJ.PluginTools.ExplodedViews
{
    /// <summary>
    /// 爆炸图辅助类
    /// </summary>
    public class ExplodedViewHelper
    {
        /// <summary>
        /// 爆炸
        /// </summary>
        /// <param name="explodeType">爆炸视图类型</param>
        /// <param name="datas">待爆炸的数据列表</param>
        /// <param name="center">爆炸中心；根据爆炸视图类型不同，有不同的解释；</param>
        /// <param name="dir">爆炸方向；根据爆炸视图类型不同，有不同的解释；</param>
        /// <param name="intervalValue">间隔值</param>
        /// <param name="minIntervalValue">最小间隔值：爆炸完成后，任意两个对象包围盒之间的最小间隔距离</param>
        /// <param name="sortRule">排序规则</param>
        /// <returns></returns>
        public static List<ExplodeData> Explode(EExplodeType explodeType, List<ExplodeData> datas, Vector3 center, Vector3 dir, double intervalValue, double minIntervalValue, ESortRule sortRule)
        {
            switch (explodeType)
            {
                case EExplodeType.Point:
                    {
                        return Point(datas, center, dir, intervalValue, minIntervalValue, sortRule);
                    }
                case EExplodeType.Line:
                    {
                        return Line(datas, center, dir, intervalValue, minIntervalValue, sortRule);
                    }
                case EExplodeType.Plane:
                    {
                        return Plane(datas, center, dir, intervalValue, minIntervalValue, sortRule);
                    }
                default: throw new ArgumentException("无效参数值:" + explodeType.ToString(), nameof(explodeType));
            }
        }

        /// <summary>
        /// 点爆
        /// </summary>
        /// <param name="datas">待爆炸的数据列表</param>
        /// <param name="center">爆炸点中心</param>
        /// <param name="dir">当有多个数据原始位置与爆炸点中心重合时，使用的爆炸方向</param>
        /// <param name="intervalValue"></param>
        /// <param name="minIntervalValue">最小间隔值：爆炸完成后，任意两个对象包围盒之间的最小间隔距离</param>
        /// <param name="sortRule">排序规则</param>
        public static List<ExplodeData> Point(List<ExplodeData> datas, Vector3 center, Vector3 dir, double intervalValue, double minIntervalValue, ESortRule sortRule)
        {
            List<ExplodeData> explodedDatas = new List<ExplodeData>();

            //设置中心与爆炸方向
            datas.ForEach(data =>
            {
                if (data.valid)
                {
                    data.Init(center, data.startPosition - center);
                    if (MathX.ApproximatelyZero(data.dir.magnitude))
                    {
                        data.dir = dir.normalized;
                    }
                    explodedDatas.Add(data);
                }
            });

            //排序
            Sort(explodedDatas, center, sortRule);

            //爆炸
            Explode(explodedDatas, intervalValue, minIntervalValue);

            return explodedDatas;
        }

        /// <summary>
        /// 线爆
        /// </summary>
        /// <param name="datas">待爆炸的数据列表</param>
        /// <param name="center">爆炸时约束射线的原点</param>
        /// <param name="dir">爆炸时约束射线的方向向量（即法向量）</param>
        /// <param name="intervalValue">爆炸增量值</param>
        /// <param name="minIntervalValue">最小间隔值：爆炸完成后，任意两个对象包围盒之间的最小间隔距离</param>
        /// <param name="sortRule">排序规则</param>
        public static List<ExplodeData> Line(List<ExplodeData> datas, Vector3 center, Vector3 dir, double intervalValue, double minIntervalValue, ESortRule sortRule)
        {
            List<ExplodeData> explodedDatas = new List<ExplodeData>();

            //轴向爆炸时的约束射线
            var ray = new Ray(center, dir);

            //设置中心与爆炸方向
            datas.ForEach(data =>
            {
                if (data.valid)
                {
                    var c = ray.GetClosestPointOnRay(data.startPosition);
                    data.Init(c, data.startPosition - c);
                    if (MathX.ApproximatelyZero(data.dir.magnitude))
                    {
                        var tmpDir = data.center - center;
                        if (MathX.ApproximatelyZero(tmpDir.magnitude))
                        {
                            data.dir = dir.normalized;
                        }
                        else
                        {
                            data.dir = tmpDir.normalized;
                        }
                    }
                    explodedDatas.Add(data);
                }
            });

            //排序
            Sort(explodedDatas, center, sortRule);

            //爆炸
            Explode(explodedDatas, intervalValue, minIntervalValue);

            return explodedDatas;
        }

        /// <summary>
        /// 面爆
        /// </summary>
        /// <param name="datas">待爆炸的数据列表</param>
        /// <param name="center">爆炸时约束平面上的点</param>
        /// <param name="dir">爆炸时约束平面方向向量（即法向量）</param>
        /// <param name="intervalValue">爆炸增量值</param>
        /// <param name="minIntervalValue">最小间隔值：爆炸完成后，任意两个对象包围盒之间的最小间隔距离</param>
        /// <param name="sortRule">排序规则</param>
        public static List<ExplodeData> Plane(List<ExplodeData> datas, Vector3 center, Vector3 dir, double intervalValue, double minIntervalValue, ESortRule sortRule)
        {
            List<ExplodeData> explodedDatas = new List<ExplodeData>();

            //轴向爆炸时的约束平面
            var plane = new Plane(dir, center);

            //设置中心与爆炸方向
            datas.ForEach(data =>
            {
                if (data.valid)
                {
                    var c = plane.ClosestPointOnPlane(data.startPosition);
                    data.Init(c, data.startPosition - c);
                    if (MathX.ApproximatelyZero(data.dir.magnitude))
                    {
                        var tmpDir = data.center - center;
                        if (MathX.ApproximatelyZero(tmpDir.magnitude))
                        {
                            data.dir = dir.normalized;
                        }
                        else
                        {
                            data.dir = tmpDir.normalized;
                        }
                    }
                    explodedDatas.Add(data);
                }
            });

            //排序
            Sort(explodedDatas, center, sortRule);

            //爆炸
            Explode(explodedDatas, intervalValue, minIntervalValue);

            return explodedDatas;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="center"></param>
        /// <param name="sortRule"></param>
        private static void Sort(List<ExplodeData> datas, Vector3 center, ESortRule sortRule)
        {
            switch(sortRule)
            {
                case ESortRule.DistanceAsc:
                    {
                        Sort(datas, center);
                        break;
                    }
                case ESortRule.DistanceDesc:
                    {
                        Sort(datas, center);
                        datas.Reverse();
                        break;
                    }
            }
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="center"></param>
        private static void Sort(List<ExplodeData> datas,Vector3 center)
        {
            datas.Sort((x, y) =>
            {
                //判断起始包围盒中心与对象爆炸中心点的距离，由近到远
                var xDistance = (x.startPosition - x.center).sqrMagnitude;
                var yDistance = (y.startPosition - y.center).sqrMagnitude;
                if (!MathX.Approximately(xDistance, yDistance)) return xDistance.CompareTo(yDistance);

                //判断原始爆炸中心与对象爆炸中心的距离，由近到远
                var xDistance1 = (center - x.center).sqrMagnitude;
                var yDistance1 = (center - y.center).sqrMagnitude;
                if (!MathX.Approximately(xDistance1, yDistance1)) return xDistance1.CompareTo(yDistance1);

                //判断包围盒的体积，由大到小
                var xSize = x.bounds.size;
                var ySize = y.bounds.size;
                var xV = xSize.x * xSize.y * xSize.z;
                var yV = ySize.x * ySize.y * ySize.z;
                if (!MathX.Approximately(xV, yV)) return yV.CompareTo(xV);

                //判断包围盒对角线的长度，由小到大
                var xLen = xSize.sqrMagnitude;
                var yLen = ySize.sqrMagnitude;
                return xLen.CompareTo(yLen);
            });
        }

        /// <summary>
        /// 爆炸
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="intervalValue"></param>
        /// <param name="minIntervalValue">最小间隔值：爆炸完成后，任意两个对象包围盒之间的最小间隔距离</param>
        private static void Explode(List<ExplodeData> datas, double intervalValue, double minIntervalValue)
        {
            //已爆炸的数据列表
            List<ExplodeData> explodedDatas = new List<ExplodeData>();
            foreach (var data in datas)
            {
                //开始爆炸
                data.BeginExplode();

                var tmpIntervalValue = intervalValue;

                //开启包围盒交叉检测
                while (explodedDatas.Any(tmpData =>
                {
                    //检查包围盒交集
                    if (tmpData.bounds.TryGetIntersects(data.bounds, out Bounds intersection))
                    {
                        return true;
                    }

                    //检查包围盒之间的最小间隔值
                    var offset = tmpData.bounds.GetMinOffsetForIntersects(data.bounds);
                    if (offset.magnitude < minIntervalValue)
                    {
                        return true;
                    }
                    return false;
                }))
                {
                    //更新爆炸
                    data.UpdateExplode(tmpIntervalValue);
                }

                //结束爆炸
                data.EndExplode();

                //加入已爆炸的数据列表
                explodedDatas.Add(data);
            }
        }
    }
}
