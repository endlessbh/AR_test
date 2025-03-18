using System;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.Dataflows.DataBinders
{
    /// <summary>
    /// 数据绑定器
    /// </summary>
    public abstract class DataBinder : IDataBinder
    {
        /// <summary>
        /// 主类型
        /// </summary>
        public abstract Type mainType { get; }

        /// <summary>
        /// 成员名
        /// </summary>
        public abstract string memberName { get; set; }

        /// <summary>
        /// 反射标记量:用于获取成员时使用
        /// </summary>
        public virtual BindingFlags bindingFlags => TypeHelper.DefaultLookupNonPublic;

        /// <summary>
        /// 包含基础类型
        /// </summary>
        public virtual bool includeBaseType => true;

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool TryGetMemberValue(Type type, object obj, string memberName, out object value, object[] index = null)
        {
            return TypeHelper.TryGetValue(out value, TypeMemberCache.GetCacheValue(type, memberName, bindingFlags, includeBaseType)?.memberInfo, obj, index);
        }

        /// <summary>
        /// 尝试设置值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool TrySetMemberValue(Type type, object obj, string memberName, object value, object[] index = null)
        {
            var memberInfo = FieldOrPropertyInfoCache.Get(type, memberName, bindingFlags, includeBaseType);
            var memberType = memberInfo.GetMemberType();
            if (memberInfo != null && TryConvertTo(value, memberType, out object outValue))
            {
                return TypeHelper.TrySetValue(outValue, memberInfo, obj, index);
            }
            else
            {
                Debug.LogErrorFormat("数据绑定器[{0}]在执行{1}时: [{2}] -> [{3}]类型转换失败!",
                    this.GetType(),
                    nameof(TrySetMemberValue),
                    value?.GetType(),
                    memberType);
            }
            return false;
        }

        /// <summary>
        /// 尝试转化
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outputType"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected virtual bool TryConvertTo(object input, Type outputType, out object output)
        {
            return Converter.instance.TryConvertTo(input, outputType, out output);
        }

        /// <summary>
        /// 尝试转化
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outputType"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        protected virtual bool TryConvertTo<TOutput>(object input, out TOutput output)
        {
            return Converter.instance.TryConvertTo<TOutput>(input, out output);
        }
    }

    /// <summary>
    /// 数据绑定器泛型类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DataBinder<T> : DataBinder
    {
        /// <summary>
        /// 主类型
        /// </summary>
        public override Type mainType => typeof(T);
    }
}
