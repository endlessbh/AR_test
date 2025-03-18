using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Tweens
{
    /// <summary>
    /// 线条类型
    /// </summary>
    [Name("线条类型")]
    public enum ELineType
    {
        /// <summary>
        /// 贝塞尔曲线模式,根据关键点,补间生成贝塞尔曲线;即两个关键点之间,曲线连接;补间插值时,拐弯位置的百分比非常密集；
        /// </summary>
        [Name("贝塞尔")]
        [Tip("根据关键点,补间生成贝塞尔曲线;即两个关键点之间,曲线连接;补间插值时,拐弯位置的百分比非常密集；")]
        Bezier = 0,

        /// <summary>
        /// 线性模式，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;"
        /// </summary>
        [Name("线性")]
        [Tip("根据关键点,补间生成连续折线;即两个关键点之间,直线连接;")]
        Liner,

        /// <summary>
        /// 贝塞尔多边形模式，即根据关键点,补间生成贝塞尔多边形线;补间插值时,使用线性方式进行补间；
        /// </summary>
        [Name("贝塞尔多边形")]
        [Tip("根据关键点,补间生成贝塞尔多边形线;补间插值时,使用线性方式进行补间；")]
        BezierPolygon,
    }
}
