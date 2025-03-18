using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 枚举类型弹出菜单特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class EnumTypePopupAttribute : PropertyAttribute
    {
        /// <summary>
        /// 弹出菜单宽度
        /// </summary>
        public float width = 80;

        /// <summary>
        /// 枚举类型字符串数组缓存
        /// </summary>
        private static XObject<string[]> _EnumTypeStrings { get; } = new XObject<string[]>(x =>
        {
            var list = new List<string>();
            EnumHelper.ForeachTypes(type => list.Add(type.FullNameToHierarchyString()));
            list.Sort();
            return list.ToArray();
        });

        /// <summary>
        /// 枚举类型字符串数组
        /// </summary>
        public static string[] EnumTypeStrings => _EnumTypeStrings;
    }

    /// <summary>
    /// 自定义枚举弹出式菜单特性,需要遵守基类特性<see cref="DropdownPopupAttribute"/>的使用规则；
    /// </summary>
    public class CustomEnumPopupAttribute: DropdownPopupAttribute
    {
        public CustomEnumPopupAttribute() : base(nameof(CustomEnumPopupAttribute)) { }
    }
}
