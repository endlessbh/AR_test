using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCSJ.Attributes;

namespace XCSJ.PluginTools.ExplodedViews
{
    /// <summary>
    /// 爆炸类型
    /// </summary>
    [Name("爆炸类型")]
    public enum EExplodeType
    {
        /// <summary>
        /// 点爆:所有待爆炸对象以相同的爆炸点执行对外爆炸；可理解为球形爆炸；
        /// </summary>
        [Name("点爆")]
        [Tip("所有待爆炸对象以相同的爆炸点执行对外爆炸；可理解为球形爆炸；", "All objects to be exploded shall be exploded at the same explosion point; It can be understood as spherical explosion;")]
        Point,

        /// <summary>
        /// 线爆:所有待爆炸对象的包围盒投影到线上后，以线上投影点为爆炸点执行对外爆炸；可理解为管道膨胀型爆炸；
        /// </summary>
        [Name("线爆")]
        [Tip("所有待爆炸对象的包围盒投影到线上后，以线上投影点为爆炸点执行对外爆炸；可理解为管道膨胀型爆炸；", "After the bounding boxes of all objects to be exploded are projected onto the line, the external explosion shall be carried out with the projection point on the line as the explosion point; It can be understood as pipeline expansion explosion;")]
        Line,

        /// <summary>
        /// 面爆:所有待爆炸对象的包围盒投影到面上后，以面上投影点为爆炸点执行对外爆炸；可理解为特定方向的轴向爆炸；
        /// </summary>
        [Name("面爆")]
        [Tip("所有待爆炸对象的包围盒投影到面上后，以面上投影点为爆炸点执行对外爆炸；可理解为特定方向的轴向爆炸；", "After the bounding boxes of all objects to be exploded are projected on the surface, the external explosion shall be carried out with the projection point on the surface as the explosion point; It can be understood as axial explosion in a specific direction;")]
        Plane,
    }

    /// <summary>
    /// 排序规则
    /// </summary>
    [Name("排序规则")]
    public enum  ESortRule
    {
        /// <summary>
        /// 无:不做排序，即使用传入爆炸数据队列的顺序做爆炸；
        /// </summary>
        [Name("无")]
        [Tip("不做排序，即使用传入爆炸数据队列的顺序做爆炸；", "Do not sort, that is, use the sequence of incoming explosion data queue to explode;")]
        None,

        /// <summary>
        /// 距离升序：对传入爆炸数据队列根据距离对应爆炸中心的长度做升序排序；
        /// </summary>
        [Name("距离升序")]
        [Tip("对传入爆炸数据队列根据距离对应爆炸中心的长度做升序排序；", "Sort the incoming explosion data queue in ascending order according to the length from the corresponding explosion center;")]
        DistanceAsc,

        /// <summary>
        /// 距离降序：对传入爆炸数据队列根据距离对应爆炸中心的长度做降序排序；
        /// </summary>
        [Name("距离降序")]
        [Tip("对传入爆炸数据队列根据距离对应爆炸中心的长度做降序排序；", "The incoming explosion data queue is sorted in descending order according to the length from the corresponding explosion center;")]
        DistanceDesc,
    }

    /// <summary>
    /// 中心类型
    /// </summary>
    [Name("中心类型")]
    public enum ECenterType
    {
        /// <summary>
        /// 位置
        /// </summary>
        [Name("位置")]
        Postion,

        /// <summary>
        /// 变换位置
        /// </summary>
        [Name("变换位置")]
        TransformPosition = 10,

        /// <summary>
        /// 变换包围盒中心
        /// </summary>
        [Name("变换包围盒中心")]
        TransformBoundsCenter,

        /// <summary>
        /// 包围盒中心:所有待爆炸的对象包围盒的最小并集包围盒的中心
        /// </summary>
        [Name("包围盒中心")]
        [Tip("所有待爆炸的对象包围盒的最小并集包围盒的中心", "The minimum union of the bounding box of all objects to be exploded is the center of the bounding box")]
        BoundsCenter = 100,
    }

    /// <summary>
    /// 方向类型
    /// </summary>
    [Name("方向类型")]
    public enum EDirectionType
    {
        /// <summary>
        /// 向量
        /// </summary>
        [Name("向量")]
        Vector,

        /// <summary>
        /// TransformX
        /// </summary>
        [Name("变换的X方向")]
        TransformX = 10,

        /// <summary>
        /// 变换的Y方向
        /// </summary>
        [Name("变换的Y方向")]
        TransformY,

        /// <summary>
        /// 变换的Z方向
        /// </summary>
        [Name("变换的Z方向")]
        TransformZ,

        /// <summary>
        /// 中心到变换方向
        /// </summary>
        [Name("中心到变换方向")]
        CenterToTransform,
    }
}
