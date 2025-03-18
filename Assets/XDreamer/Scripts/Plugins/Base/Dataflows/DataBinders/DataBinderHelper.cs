using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Helpers;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.Dataflows.DataBinders
{
    /// <summary>
    /// 数据链接助手
    /// </summary>
    public static class DataBinderHelper
    {
        ///// <summary>
        ///// 获取绑定器对象
        ///// </summary>
        ///// <param name="alias"></param>
        ///// <param name="getBinders"></param>
        ///// <returns></returns>        
        //public static ITypeMemberDataBinder CreateBindData(string alias, IBinderGetter[] getBinders)
        //{
        //    if (string.IsNullOrEmpty(alias)) return null;
        //    if (getBinders == null || getBinders.Length == 0) return null;

        //    var binder = GetBinder(alias, getBinders);
        //    if (binder == null) return null;
        //    return CreateTypeMemberDataBinder(binder);
        //}

        ///// <summary>
        ///// 从绑定获取器中查找匹配对象，找到即返回
        ///// </summary>
        ///// <param name="alias"></param>
        ///// <param name="getBinders"></param>
        ///// <returns></returns>
        //private static ITypeMemberBinder GetBinder(string alias, params IBinderGetter[] getBinders)
        //{
        //    ITypeMemberBinder binder = null;
        //    foreach (var getter in getBinders)
        //    {
        //        binder = getter.Get(alias);
        //        if (binder != null)
        //        {
        //            break;
        //        }
        //    }
        //    return binder;
        //}

        /// <summary>
        /// 创建类型成员数据绑定器：如果对应成员名称项的数据绑定器不存在，会返回指定类型但无具体成员名的数据绑定器；
        /// </summary>
        /// <param name="typeMemberBinder">类型成员绑定器</param>
        /// <returns></returns>
        public static ITypeMemberDataBinder CreateTypeMemberDataBinder(ITypeMemberBinder typeMemberBinder)
        {
            try
            {
                if (typeMemberBinder == null) return null;

                //获取成员名
                var memberName = (typeMemberBinder.hasMember && typeMemberBinder.memberInfo != null) ? typeMemberBinder.memberInfo.Name : "";

                //创建数据绑定器
                var dataBinder = CreateDataBinder(typeMemberBinder.mainType, typeMemberBinder.memberInfo.Name) as ITypeMemberDataBinder;
                if (dataBinder == null && !string.IsNullOrEmpty(memberName))
                {
                    dataBinder = CreateDataBinder(typeMemberBinder.mainType, "") as ITypeMemberDataBinder;
                }

                //初始化数据绑定器
                if (dataBinder != null) dataBinder.Init(typeMemberBinder);
                return dataBinder;
            }
            catch (Exception ex)
            {
                Debug.LogError(nameof(CreateTypeMemberDataBinder) + "执行异常:" + ex);
            }
            return null;
        }

        /// <summary>
        /// 构建数据绑定器:从全局所有程序集中查找指定类型与成员名的数据绑定器类型并创建实例后返回；要求参数类型与成员名与数据绑定器特性修饰的对应关系必须一致且参数均不可为null；
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static IDataBinder CreateDataBinder(Type type, string memberName)
        {
            try
            {
                var dataBinderType = DataBinderAttribute.GetBinderType(type, memberName);
                if (dataBinderType == null) return default;

                return TypeHelper.CreateInstance(dataBinderType) as IDataBinder;
            }
            catch (Exception ex)
            {
                Debug.LogError(nameof(CreateDataBinder) + "执行异常:" + ex);
            }
            return null;
        }

        /// <summary>
        /// 数据绑定器缓存：相同类型不同成员名键值时，可共用指定类型但无具体成员名的数据绑定器
        /// </summary>
        public class DataBinderCache : TICache<DataBinderCache, Type, string, IDataBinder>
        {
            /// <summary>
            /// 构建值
            /// </summary>
            /// <param name="key1"></param>
            /// <param name="key2"></param>
            /// <returns></returns>
            protected override KeyValuePair<bool, IDataBinder> CreateValue(Type key1, string key2)
            {
                return new KeyValuePair<bool, IDataBinder>(true, GetOrCreateDataBinder(key1, key2));
            }

            private IDataBinder GetOrCreateDataBinder(Type type, string memberName)
            {
                var dataBinder = CreateDataBinder(type, memberName);
                if (dataBinder == null && !string.IsNullOrEmpty(memberName))
                {
                    //相同类型不同成员名键值时，共用指定类型但无具体成员名的数据绑定器
                    return GetDataBinder(type);
                }
                return dataBinder;
            }

            /// <summary>
            /// 获取数据绑定器
            /// </summary>
            /// <param name="type"></param>
            /// <param name="memberName"></param>
            /// <returns></returns>
            public static IDataBinder GetDataBinder(Type type, string memberName = "")
            {
                if (type == null || memberName == null) return default;
                return GetCacheValue(type, memberName);
            }
        }

        /// <summary>
        /// 获取有效类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Type GetValidType(this Type type, object obj)
        {
            if (type == null && !obj.ObjectIsNull())
            {
                type = obj.GetType();
            }
            return type;
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryGetValue(Type type, object obj, string memberName, out object value, object[] index = null)
        {
            type = type.GetValidType(obj);
            if (DataBinderCache.GetDataBinder(type, memberName) is IDataBinder dataBinder)
            {
                return dataBinder.TryGetMemberValue(type, obj, memberName, out value, index);
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryGetValue(this object obj, string memberName, out object value, object[] index = null) => TryGetValue(null, obj, memberName, out value, index);

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TryGetValue(this Type type, string memberName, out object value, object[] index = null) => TryGetValue(type, null, memberName, out value, index);

        /// <summary>
        /// 尝试设置值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TrySetValue(Type type, object obj, string memberName, object value, object[] index = null)
        {
            type = type.GetValidType(obj);
            if (DataBinderCache.GetDataBinder(type, memberName) is IDataBinder dataBinder)
            {
                return dataBinder.TrySetMemberValue(type, obj, memberName, value, index);
            }
            return false;
        }

        /// <summary>
        /// 尝试设置值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TrySetValue(this object obj, string memberName, object value, object[] index = null) => TrySetValue(null, obj, memberName, value, index);

        /// <summary>
        /// 尝试设置值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberName"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TrySetValue(this Type type, string memberName, object value, object[] index = null) => TrySetValue(type, null, memberName, value, index);
    }
}
