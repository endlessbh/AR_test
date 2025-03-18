using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 仅修饰String时有效;修饰可以下拉选择网络属性名称；需要遵守基类特性<see cref="DropdownPopupAttribute"/>的使用规则；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class NetPropertyNameAttribute : DropdownPopupAttribute
    {
        public NetPropertyNameAttribute() : base(nameof(NetPropertyNameAttribute)) { }
    }
}
