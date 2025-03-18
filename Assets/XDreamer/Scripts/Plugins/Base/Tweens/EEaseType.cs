using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Tweens
{
    /// <summary>
    /// 缓动类型枚举
    /// </summary>
    [Name("缓动类型")]
    public enum EEaseType
    {
        /// <summary>
        /// 线性
        /// </summary>
        [Name("线性")]
        [Tip("以线性(匀速)方式执行逻辑;默认执行方式;\n公式:\nx∈[0,1]时y=x", "Execute logic in a linear (uniform) manner; Default execution method; \n formula: \n y = x when x ∈ [0,1]")]
        Linear,

        /// <summary>
        /// 弹性
        /// </summary>
        [Name("弹性")]
        [Tip("以弹性方式执行逻辑;", "Execute logic in a flexible manner;")]
        Spring,

        /// <summary>
        /// EaseInQuad
        /// </summary>
        [Name("二次方缓入")]
        [Tip("以匀加速方式执行逻辑;\n公式:\nx∈[0,1]时y=x^2", "Execute logic in a uniform acceleration manner; \n formula: \n y = x ^ 2 when x ∈ [0,1]")]
        EaseInQuad,

        /// <summary>
        /// 二次方缓出
        /// </summary>
        [Name("二次方缓出")]
        [Tip("以匀减速方式执行逻辑;\n公式:\nx∈[0,1]时y=1-(1-x)^2", "Execute logic in a uniform deceleration mode; \n formula: \n when x ∈ [0,1], y = 1 - (1-x) ^ 2")]
        EaseOutQuad,

        /// <summary>
        /// 二次方缓入缓出
        /// </summary>
        [Name("二次方缓入缓出")]
        [Tip("以先匀加速后匀减速方式执行逻辑;\n公式:\nx∈[0,0.5]时y=(2*x)^2\nx∈[0.5,1]时y=0.5+(1-(1-2*(x-0.5))^2)/2", "Execute the logic in the way of uniform acceleration first and then uniform deceleration; \n formula: \n y = (2 * x) ^ 2 when x ∈ [0,0.5] \n y = 0.5 + (1 - (1-2 * (x-0.5)) ^ 2) / 2 when x ∈ [0.5,1]")]
        EaseInOutQuad,

        /// <summary>
        /// 三次方缓入
        /// </summary>
        [Name("三次方缓入")]
        [Tip("以加速方式执行逻辑;\n公式:\nx∈[0,1]时y=x^3", "Execute logic in an accelerated manner; \n formula: \n y = x ^ 3 when x ∈ [0,1]")]
        EaseInCubic,

        /// <summary>
        /// 三次方缓出
        /// </summary>
        [Name("三次方缓出")]
        [Tip("以减速方式执行逻辑;\n公式:\nx∈[0,1]时y=1-(1-x)^3", "Execute logic in deceleration mode; \n formula: \n when x ∈ [0,1], y = 1 - (1-x) ^ 3")]
        EaseOutCubic,

        /// <summary>
        /// 三次方缓入缓出
        /// </summary>
        [Name("三次方缓入缓出")]
        [Tip("以先加速后减速方式执行逻辑;", "Execute logic by accelerating first and then decelerating;")]
        EaseInOutCubic,

        /// <summary>
        /// 四次方缓入
        /// </summary>
        [Name("四次方缓入")]
        [Tip("以加速方式执行逻辑;\n公式:\nx∈[0,1]时y=x^4", "Execute logic in an accelerated manner; \n formula: \n y = x ^ 4 when x ∈ [0,1]")]
        EaseInQuart,

        /// <summary>
        /// 四次方缓出
        /// </summary>
        [Name("四次方缓出")]
        [Tip("以减速方式执行逻辑;\n公式:\nx∈[0,1]时y=1-(1-x)^4", "Execute logic in deceleration mode; \n formula: \n when x ∈ [0,1], y = 1 - (1-x) ^ 4")]
        EaseOutQuart,

        /// <summary>
        /// 四次方缓入缓出
        /// </summary>
        [Name("四次方缓入缓出")]
        [Tip("以先加速后减速方式执行逻辑;", "Execute logic by accelerating first and then decelerating;")]
        EaseInOutQuart,

        /// <summary>
        /// 五次方缓入
        /// </summary>
        [Name("五次方缓入")]
        [Tip("以加速方式执行逻辑;\n公式:\nx∈[0,1]时y=x^5", "Execute logic in an accelerated manner; \n formula: \n y = x ^ 5 when x ∈ [0,1]")]
        EaseInQuint,

        /// <summary>
        /// 五次方缓出
        /// </summary>
        [Name("五次方缓出")]
        [Tip("以减速方式执行逻辑;\n公式:\nx∈[0,1]时y=1-(1-x)^5", "Execute logic in deceleration mode; \n formula: \n when x ∈ [0,1], y = 1 - (1-x) ^ 5")]
        EaseOutQuint,

        /// <summary>
        /// 五次方缓入缓出
        /// </summary>
        [Name("五次方缓入缓出")]
        [Tip("以先加速后减速方式执行逻辑;", "Execute logic by accelerating first and then decelerating;")]
        EaseInOutQuint,

        /// <summary>
        /// 正弦缓入
        /// </summary>
        [Name("正弦缓入")]
        [Tip("以正弦缓入方式执行逻辑;", "Execute logic in sinusoidal ramp mode;")]
        EaseInSine,

        /// <summary>
        /// 正弦缓出
        /// </summary>
        [Name("正弦缓出")]
        [Tip("以正弦缓出方式执行逻辑;", "Execute logic in sinusoidal slow out mode;")]
        EaseOutSine,

        /// <summary>
        /// 正弦缓入缓入
        /// </summary>
        [Name("正弦缓入缓入")]
        [Tip("以先正弦缓入后正弦缓出方式执行逻辑;", "Execute the logic in the way of sinusoidal slow in and then sinusoidal slow out;")]
        EaseInOutSine,

        /// <summary>
        /// 指数缓入
        /// </summary>
        [Name("指数缓入")]
        [Tip("以指数缓入方式执行逻辑;", "Execute logic in an exponential ramp;")]
        EaseInExpo,

        /// <summary>
        /// 指数缓出
        /// </summary>
        [Name("指数缓出")]
        [Tip("以指数缓出方式执行逻辑;", "Execute logic in exponential slow out mode;")]
        EaseOutExpo,

        /// <summary>
        /// 指数缓入缓入
        /// </summary>
        [Name("指数缓入缓入")]
        [Tip("以先指数缓入后指数缓出方式执行逻辑;", "Execute logic in the way of index in first and index out later;")]
        EaseInOutExpo,

        /// <summary>
        /// 圆形缓入
        /// </summary>
        [Name("圆形缓入")]
        [Tip("以圆形缓入方式执行逻辑;", "Execute logic in a circular jog mode;")]
        EaseInCirc,

        /// <summary>
        /// 圆形缓出
        /// </summary>
        [Name("圆形缓出")]
        [Tip("以圆形缓出方式执行逻辑;", "Execute logic in a circular buffer mode;")]
        EaseOutCirc,

        /// <summary>
        /// 圆形缓入缓入
        /// </summary>
        [Name("圆形缓入缓入")]
        [Tip("以先圆形缓入后圆形缓出方式执行逻辑;", "Execute the logic in the way of circular buffer in first and then circular buffer out;")]
        EaseInOutCirc,

        /// <summary>
        /// 反弹缓入
        /// </summary>
        [Name("反弹缓入")]
        [Tip("以反弹缓入方式执行逻辑;", "Execute logic in rebound and slow in mode;")]
        EaseInBounce,

        /// <summary>
        /// 反弹缓出
        /// </summary>
        [Name("反弹缓出")]
        [Tip("以反弹缓出方式执行逻辑;", "Execute logic in rebound and slow out mode;")]
        EaseOutBounce,

        /// <summary>
        /// 反弹缓入缓出
        /// </summary>
        [Name("反弹缓入缓出")]
        [Tip("以先反弹缓入后反弹缓出方式执行逻辑;", "Slow in and then rebound;")]
        EaseInOutBounce,

        /// <summary>
        /// 回归缓入
        /// </summary>
        [Name("回归缓入")]
        [Tip("以回归缓入方式执行逻辑;", "Execute logic in a regression slow in manner;")]
        EaseInBack,

        /// <summary>
        /// 回归缓出
        /// </summary>
        [Name("回归缓出")]
        [Tip("以回归缓出方式执行逻辑;", "Execute logic in the way of regression and slow out;")]
        EaseOutBack,

        /// <summary>
        /// 回归缓入缓出
        /// </summary>
        [Name("回归缓入缓出")]
        [Tip("以先回归缓入后回归缓出方式执行逻辑;", "Execute logic in the way of regression slow in and then regression slow out;")]
        EaseInOutBack,

        /// <summary>
        /// 弹性缓入
        /// </summary>
        [Name("弹性缓入")]
        [Tip("以弹性缓入方式执行逻辑;", "Execute logic in a flexible ramp in manner;")]
        EaseInElastic,

        /// <summary>
        /// 弹性缓出
        /// </summary>
        [Name("弹性缓出")]
        [Tip("以弹性缓出方式执行逻辑;", "Execute the logic in a flexible way;")]
        EaseOutElastic,

        /// <summary>
        /// 弹性缓入缓出
        /// </summary>
        [Name("弹性缓入缓出")]
        [Tip("以先弹性缓入后弹性缓出方式执行逻辑;", "Execute the logic in the way of elastic slow in first and then elastic slow out;")]
        EaseInOutElastic,

        /// <summary>
        /// 重击
        /// </summary>
        [Name("重击")]
        [Tip("以重击方式执行逻辑;目前不支持,以线性方式提供;", "Execute logic by heavy hitting; It is not supported at present and is provided in a linear manner;")]
        Punch,
    }
}
