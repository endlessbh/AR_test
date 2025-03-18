using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Helper;

namespace XCSJ.PluginXGUI.Styles.Base
{
    /// <summary>
    /// 组件样式绑定器特性：用于修饰<see cref="BaseStyleElement"/>类型    
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class StyleLinkAttribute : LinkedTypeAttribute
    {
        /// <summary>
        /// 参数设置枚举类型
        /// </summary>
        public Type paramsSettingEnumType { get; } = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type"></param>
        public StyleLinkAttribute(Type type, Type paramsSettingEnumType, bool forChildClasses = true) : base(type, forChildClasses, ToPurpose(type))
        {
            this.paramsSettingEnumType = paramsSettingEnumType;
        }

        /// <summary>
        /// 获取目标字符串
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static string ToPurpose(Type type) => nameof(StyleLinkAttribute) + "." + type.Name;

        /// <summary>
        /// 获取样式元素类型
        /// </summary>
        /// <param name="type">被应用样式的类</param>
        /// <returns></returns>
        public static Type GetStyleElementType(Type typeAppliedStyle) => LinkedTypeCache.Get(typeAppliedStyle, ToPurpose(typeAppliedStyle));

    }
}