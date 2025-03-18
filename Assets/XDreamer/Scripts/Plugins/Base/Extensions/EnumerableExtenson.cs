using System;
using System.Collections.Generic;

namespace XCSJ.Extension.Base.Extensions
{
    /// <summary>
    /// 可迭代扩展：针对<see cref="IEnumerable{T}"/>的扩展
    /// </summary>
    public static class EnumerableExtenson
    {
        /// <summary>
        /// 条件转换迭代器
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="whereCast"></param>
        /// <returns></returns>
        private static IEnumerable<TResult> WhereCastIterator<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, (bool, TResult)> whereCast)
        {
            foreach (TSource obj in source)
            {
                var tuple = whereCast(obj);
                if (tuple.Item1) yield return tuple.Item2;
            }
        }

        /// <summary>
        /// 条件转换为列表
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="whereCast"></param>
        /// <returns></returns>
        public static List<TResult> WhereToList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, (bool, TResult)> whereCast)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (whereCast == null) throw new ArgumentNullException(nameof(whereCast));
            return new List<TResult>(WhereCastIterator(source, whereCast));
        }

        /// <summary>
        /// 条件转换
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="whereCast"></param>
        /// <returns></returns>
        public static IEnumerable<TResult> WhereCast<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, (bool, TResult)> whereCast)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (whereCast == null) throw new ArgumentNullException(nameof(whereCast));
            return WhereCastIterator(source, whereCast);
        }
    }
}
