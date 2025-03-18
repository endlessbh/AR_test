using System;
using UnityEngine;
using XCSJ.Interfaces;
using XCSJ.Maths;

namespace XCSJ.Extension.Base.Algorithms
{
    public interface IBaseLoopWorkClip : IPercentClip
    {
        /// <summary>
        /// 循环类型
        /// </summary>
        ELoopType loopType { get; }

        /// <summary>
        /// 循环次数
        /// </summary>
        double loopCount { get; }

        /// <summary>
        /// 单次百分比长
        /// </summary>
        double oncePercentLength { get; }

        /// <summary>
        /// 工作曲线
        /// </summary>
        AnimationCurve workCurve { get; }

        /// <summary>
        /// 超出工作区间候是否继续循环
        /// </summary>
        bool continueLoopAfterWorkRange { get; }

        /// <summary>
        /// 超出工作区间时百分比
        /// </summary>
        double percentOnAfterWorkRange { get; }
    }

    /// <summary>
    /// 百分比：用于工作剪辑计算包括经过循环类型、工作曲线等处理后的百分比数值；
    /// </summary>
    public class Percent
    {
        /// <summary>
        /// 精度
        /// </summary>
        public const double Epsilon = 1E-05F;

        /// <summary>
        /// 状态的百分比进度(范围[-∞,+∞])
        /// </summary>
        public double percentOfState { get; private set; }

        /// <summary>
        /// 状态组件百分比进度(范围[-∞,+∞]):相对当前状态组件有效工作区间的百分比进度
        /// </summary>
        public double percent { get; private set; }

        /// <summary>
        /// 循环类型计算后的状态组件百分比进度(范围[0,1])
        /// </summary>
        public double percentOfLoopType { get; private set; }

        /// <summary>
        /// 状态组件百分比进度(范围[0,1]):对percentOfLoopType根据前状态组件有效工作区间规则进行计算纠正
        /// </summary>
        public double percent01 { get; private set; }

        /// <summary>
        /// 工作曲线百分比进度(理论范围[0,1]),使用工作曲线对percent01计算后的值；
        /// </summary>
        public double percentOfWorkCurve { get; private set; }

        /// <summary>
        /// 工作曲线百分比进度(范围[0,1])
        /// </summary>
        public double percent01OfWorkCurve => MathX.Clamp01(percentOfWorkCurve);

        /// <summary>
        /// 循环工作剪辑
        /// </summary>
        public IBaseLoopWorkClip loopWorkClip { get; private set; }

        /// <summary>
        /// 标识当前百分比是否在区间左
        /// </summary>
        public bool leftRange => percent < -Epsilon;

        /// <summary>
        /// 标识当前百分比是否在区间右
        /// </summary>
        public bool rightRange => percent > loopWorkClip.loopCount + Epsilon;

        /// <summary>
        /// 标识当前百分比是否在区间内
        /// </summary>
        public bool inRange => percent >= -Epsilon && percent <= loopWorkClip.loopCount + Epsilon;

        /// <summary>
        /// 零百分比对象
        /// </summary>
        public static Percent Zero => new Percent();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="loopWorkClip"></param>
        public void Init(IBaseLoopWorkClip loopWorkClip)
        {
            this.loopWorkClip = loopWorkClip ?? throw new ArgumentNullException(nameof(loopWorkClip));
        }

        /// <summary>
        /// 更新百分比数据
        /// </summary>
        /// <param name="percentOfState">状态的百分比</param>
        /// <returns>返回当前百分比对象</returns>
        public Percent Update(double percentValue)
        {
            this.percentOfState = percentValue;
            percent = MathX.Scale(percentValue - loopWorkClip.beginPercent, loopWorkClip.oncePercentLength);
            switch (loopWorkClip.loopType)
            {
                case ELoopType.None:
                    {
                        percent01 = percentOfLoopType = MathX.Clamp01(percent);
                        percentOfWorkCurve = loopWorkClip.workCurve.Evaluate((float)percent01);
                        break;
                    }
                case ELoopType.Loop:
                    {
                        percentOfLoopType = Loop01(percent);
                        if (leftRange)
                        {
                            percent01 = 0;
                        }
                        else if (rightRange)
                        {
                            if (loopWorkClip.continueLoopAfterWorkRange)
                            {
                                percent01 = percentOfLoopType;
                            }
                            else
                            {
                                percent01 = Loop01(loopWorkClip.percentOnAfterWorkRange);
                            }
                        }
                        else//in
                        {
                            percent01 = percentOfLoopType;
                        }
                        percentOfWorkCurve = loopWorkClip.workCurve.Evaluate((float)percent01);
                        break;
                    }
                case ELoopType.PingPong:
                    {
                        percentOfLoopType = PingPong01(percent);
                        if (leftRange)
                        {
                            percent01 = 0;
                        }
                        else if (rightRange)
                        {
                            if (loopWorkClip.continueLoopAfterWorkRange)
                            {
                                percent01 = percentOfLoopType;
                            }
                            else
                            {
                                percent01 = PingPong01(loopWorkClip.percentOnAfterWorkRange);
                            }
                        }
                        else//in
                        {
                            percent01 = percentOfLoopType;
                        }
                        percentOfWorkCurve = loopWorkClip.workCurve.Evaluate((float)percent01);
                        break;
                    }
            }
            return this;
        }

        /// <summary>
        /// 重置：将所有数据信息清零
        /// </summary>
        public void Reset()
        {
            percentOfState = 0;
            percent = 0;
            percentOfLoopType = 0;
            percent01 = 0;
            percentOfWorkCurve = 0;
        }

        /// <summary>
        /// 以Loop模式计算的百分比值
        /// </summary>
        /// <param name="v"></param>
        /// <returns>范围[0,1]</returns>
        public static double Loop01(double v)
        {
            var endFlag = v >= 1 || MathX.Approximately(v, 1);
            var value = MathX.DecimalPart(v);
            return endFlag && MathX.ApproximatelyZero(value) ? 1 : value;
        }

        /// <summary>
        /// 以PingPong模式计算的百分比值
        /// </summary>
        /// <param name="v"></param>
        /// <returns>范围[0,1]</returns>
        public static double PingPong01(double v)
        {
            var percent01 = MathX.DecimalPart(v);
            return (MathX.FloorToInt(v) % 2 == 0) ? percent01 : (1 - percent01);
        }
    }
}
