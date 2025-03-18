using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Helper;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 基础交互属性数据
    /// </summary>
    public class BaseInteractPropertyData
    {
        /// <summary>
        /// 属性关键字
        /// </summary>
        public string key
        {
            get
            {
                if (_key.TryGetValue(out var result))
                {
                    return result;
                }
                return "";
            }
        }

        /// <summary>
        /// 属性值
        /// </summary>
        public string value
        {
            get
            {
                if (_value.TryGetValue(out var result))
                {
                    return result;
                }
                return "";
            }
            set
            {
                _value._value = value;
            }
        }

        /// <summary>
        /// 属性键
        /// </summary>
        [Name("属性键")]
        public StringPropertyValue _key = new StringPropertyValue();

        /// <summary>
        /// 属性值
        /// </summary>
        [Name("属性值")]
        public StringPropertyValue _value = new StringPropertyValue();

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseInteractPropertyData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseInteractPropertyData(string key) => _key = new StringPropertyValue(key);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public BaseInteractPropertyData(string key, string value) : this(key) => _value = new StringPropertyValue(value);
    }

    #region 属性关键字特性与缓存

    /// <summary>
    /// 属性关键字缓存
    /// </summary>
    public static class PropertyKeyCache
    {
        private static string[] _propertyKeys = null;

        /// <summary>
        /// 属性关键字数组
        /// </summary>
        public static string[] propertyKeys
        {
            get
            {
                if (_propertyKeys == null)
                {
                    try
                    {
                        var list = new List<(int, string)>();
                        foreach (var type in TypeHelper.FindTypeInAppWithClass(typeof(object), true, true))
                        {
                            foreach (var fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public))
                            {
                                var att = AttributeHelper.GetAttribute<PropertyKeyAttribute>(fieldInfo);
                                if (att != null && Converter.instance.TryConvertTo<string>(fieldInfo.GetValue(null), out var key))
                                {
                                    list.Add((att.index, key));
                                }
                            }
                        }
                        // 优先按序号排，然后按名称排
                        list.Sort((x, y) => (x.Item1 == y.Item1) ? (x.Item2.CompareTo(y.Item2)) : (x.Item1 < y.Item1 ? -1 : 1));
                        _propertyKeys = list.Cast(item => item.Item2).ToArray();
                    }
                    catch
                    {
                        _propertyKeys = new string[0];
                    }
                }
                return _propertyKeys;
            }
        }
    }

    /// <summary>
    /// 属性关键字特性：主要修饰字符串常量类型的关键字
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class PropertyKeyAttribute : IndexAttribute { }

    #endregion
}
