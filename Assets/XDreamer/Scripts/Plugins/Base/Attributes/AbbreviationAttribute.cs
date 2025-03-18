using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 缩写特性:用于将长字符串用一个缩写替代
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class AbbreviationAttribute : Attribute
    {
        /// <summary>
        /// 缩写
        /// </summary>
        public string abbreviation { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="abbreviation"></param>
        public AbbreviationAttribute(string abbreviation)
        {
            this.abbreviation = abbreviation ?? throw new ArgumentNullException(nameof(abbreviation));
        }

        /// <summary>
        /// 获取缩写
        /// </summary>
        /// <param name="e"></param>
        /// <param name="defaultAbbreviation"></param>
        /// <returns></returns>
        public static string GetAbbreviation(Enum e, string defaultAbbreviation) => EnumAbbreviationCache.GetCacheValue(e, defaultAbbreviation) ?? defaultAbbreviation;

        /// <summary>
        /// 获取缩写
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetAbbreviation(Enum e) => GetAbbreviation(e, CommonFun.Name(e));

        /// <summary>
        /// 枚举缩写缓存
        /// </summary>
        internal class EnumAbbreviationCache : TICache<EnumAbbreviationCache, Enum, string>
        {
            protected override KeyValuePair<bool, string> CreateValue(Enum key1)
            {
                return new KeyValuePair<bool, string>(true, AttributeCache<AbbreviationAttribute>.GetOfField(key1)?.abbreviation);
            }
        }
    }

    /// <summary>
    /// 枚举字段名称特性：针对枚举被<see cref="FlagsAttribute"/>修饰情况下，其字段名称的显示；
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class EnumFieldNameAttribute
#if UNITY_2019_3_OR_NEWER
        : InspectorNameAttribute
#else
        : DescriptionAttribute
#endif
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="name"></param>
        public EnumFieldNameAttribute(string name) : base(name) { }
    }

    /// <summary>
    /// 特性值接口
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IAttributeValue<TValue>
    {
        TValue value { get; }
    }

    /// <summary>
    /// 特性值扩展
    /// </summary>
    public static class AttributeValueExtension
    {
        /// <summary>
        /// 获取特性值
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this TAttribute attribute, TValue defaultValue = default)
            where TAttribute : Attribute, IAttributeValue<TValue>
        {
            return attribute != null ? attribute.value : defaultValue;
        }

        /// <summary>
        /// 获取特性值
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="e"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TValue GetAttributeValue<TAttribute, TValue>(this Enum e, TValue defaultValue = default)
            where TAttribute : Attribute, IAttributeValue<TValue>
        {
            return GetAttributeValue(AttributeCache<TAttribute>.GetOfField(e), defaultValue);
        }
    }
}
