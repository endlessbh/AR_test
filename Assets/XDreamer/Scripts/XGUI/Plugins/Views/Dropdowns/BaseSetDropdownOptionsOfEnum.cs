using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 基础设置枚举型下拉框选项列表
    /// </summary>
    public abstract class BaseSetDropdownOptionsOfEnum : BaseSetDropdownOptions, ISetDropdownOptionsOfEnum
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
    /// <typeparam name="TEnum">枚举类型</typeparam>
    public abstract class BaseSetDropdownOptionsOfEnum<TEnum> : BaseSetDropdownOptionsOfEnum
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

    /// <summary>
    /// 设置枚举型下拉框选项列表接口
    /// </summary>
    public interface ISetDropdownOptionsOfEnum : ISetDropdownOptions
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        Type enumType { get; set; }

        /// <summary>
        /// 枚举字符串类型
        /// </summary>
        EEnumStringType enumStringType { get; set; }
    }
}
