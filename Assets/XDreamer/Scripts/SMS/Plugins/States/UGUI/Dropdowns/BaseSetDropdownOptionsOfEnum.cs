using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.XGUI.Dropdowns;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 基础设置枚举型下拉框选项列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSetDropdownOptionsOfEnum<T> : BaseSetDropdownOptions<T>, ISetDropdownOptionsOfEnum
        where T : BaseSetDropdownOptionsOfEnum<T>
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        public abstract Type enumType { get; set; }

        /// <summary>
        /// 枚举字符串类型
        /// </summary>
        public abstract EEnumStringType enumStringType { get; set; }

        /// <summary>
        /// 选项数据列表：用于设置下拉框选项的选项数据列表
        /// </summary>
        public override List<Dropdown.OptionData> optionDatas => EnumStringsCache.Get(enumType, enumStringType).ToList(enumString => new Dropdown.OptionData(enumString));
    }

    /// <summary>
    /// 基础设置枚举型下拉框选项列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public abstract class BaseSetDropdownOptionsOfEnum<T, TEnum> : BaseSetDropdownOptionsOfEnum<T>
        where T : BaseSetDropdownOptionsOfEnum<T, TEnum>
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        public override Type enumType { get => typeof(TEnum); set { } }

        /// <summary>
        /// 枚举字符串类型
        /// </summary>
        [Name("枚举字符串类型")]
        [EnumPopup]
        public EEnumStringType _enumStringType = EEnumStringType.NameAttributeCN;

        /// <summary>
        /// 枚举字符串类型
        /// </summary>
        public override EEnumStringType enumStringType { get => _enumStringType; set => _enumStringType = value; }
    }
}
