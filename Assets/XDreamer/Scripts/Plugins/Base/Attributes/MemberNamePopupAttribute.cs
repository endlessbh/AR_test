using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 成员名称弹出式菜单特性,需要遵守基类特性<see cref="DropdownPopupAttribute"/>的使用规则；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class MemberNamePopupAttribute : DropdownPopupAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        public MemberNamePopupAttribute() : base(nameof(MemberNamePopupAttribute)) { }
    }

    /// <summary>
    /// 类型全名称弹出式菜单特性,需要遵守基类特性<see cref="DropdownPopupAttribute"/>的使用规则；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class TypeFullNamePopupAttribute : DropdownPopupAttribute
    {
        /// <summary>
        /// 构造
        /// </summary>
        public TypeFullNamePopupAttribute() : base(nameof(TypeFullNamePopupAttribute)) { }
    }
}
