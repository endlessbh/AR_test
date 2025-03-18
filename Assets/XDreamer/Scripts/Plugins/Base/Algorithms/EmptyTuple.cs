using System;
using XCSJ.Algorithms;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 空元组数据类
    /// </summary>
    public sealed class EmptyTupleData : ITupleData
    {
        /// <summary>
        /// 默认空元组数据类对象
        /// </summary>
        public static EmptyTupleData Default => Default<EmptyTupleData>.Instance;

        /// <summary>
        /// 根据索引获取对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index] => null;

        /// <summary>
        /// 长度
        /// </summary>
        public int Length => 0;
    }

    /// <summary>
    /// 元组数据接口;模拟.Net4.X中的ITuple接口；
    /// </summary>
    public interface ITupleData
    {
        /// <summary>
        /// 根据索引获取对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object this[int index] { get; }

        /// <summary>
        /// 长度
        /// </summary>
        int Length { get; }
    }

    /// <summary>
    /// 元组数据
    /// </summary>
    public sealed class TupleData : ITupleData
    {
        public object[] objects { get; private set; }

        public int Length => objects.Length;

        public object this[int index] => objects[index];

        public TupleData(params object[] objects)
        {
            this.objects = objects ?? Empty<object>.Array;
        }

        public static TupleData Create(params object[] objects) => new TupleData(objects);
    }

    /// <summary>
    /// 元组扩展
    /// </summary>
    public static class TupleExtension
    {
        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1>(this Tuple<T1> tuple) => new TupleData(tuple.Item1);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2>(this Tuple<T1, T2> tuple) => new TupleData(tuple.Item1, tuple.Item2);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2, T3>(this Tuple<T1, T2, T3> tuple) => new TupleData(tuple.Item1, tuple.Item2, tuple.Item3);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2, T3, T4>(this Tuple<T1, T2, T3, T4> tuple) => new TupleData(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4, T5> tuple) => new TupleData(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2, T3, T4, T5, T6>(this Tuple<T1, T2, T3, T4, T5, T6> tuple) => new TupleData(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2, T3, T4, T5, T6, T7>(this Tuple<T1, T2, T3, T4, T5, T6, T7> tuple) => new TupleData(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);

        /// <summary>
        /// 转为元组数据接口
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="T5"></typeparam>
        /// <typeparam name="T6"></typeparam>
        /// <typeparam name="T7"></typeparam>
        /// <typeparam name="T8"></typeparam>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static TupleData ToTupleData<T1, T2, T3, T4, T5, T6, T7, T8>(this Tuple<T1, T2, T3, T4, T5, T6, T7, T8> tuple) => new TupleData(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Rest);
    }
}
