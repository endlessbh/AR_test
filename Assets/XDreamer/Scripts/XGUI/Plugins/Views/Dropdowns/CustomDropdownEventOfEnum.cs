using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 自定义枚举型下拉框事件
    /// </summary>
    [Serializable]
    [Name("自定义枚举型下拉框事件")]
    public class CustomDropdownEventOfEnum : BaseCustomDropdownEventOfEnum
    {
        /// <summary>
        /// 枚举类型
        /// </summary>
        [Name("枚举类型")]
        [EnumTypePopup]
        public string _enumType = "";

        /// <summary>
        /// 枚举类型全名称
        /// </summary>
        public string enumTypeFullName { get => _enumType; set => _enumType = value; }

        /// <summary>
        /// 枚举类型
        /// </summary>
        public override Type enumType
        {
            get => TypeCache.Get(enumTypeFullName);
            set => enumTypeFullName = TypeToString(value);
        }

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

        /// <summary>
        /// 枚举字符串
        /// </summary>
        [Name("枚举字符串")]
        [CustomEnumPopup]
        public string _enumString = "";

        /// <summary>
        /// 枚举字符串
        /// </summary>
        public override string enumString { get => _enumString; set => _enumString = value; }

        /// <summary>
        /// 枚举值：通过<see cref="enumType"/>与<see cref="enumStringType"/>转换<see cref="enumString"/>来设置或获取枚举值
        /// </summary>
        public override Enum enumValue
        {
            get => EnumValueCache.Get(enumType, enumString, enumStringType);
            set => enumString = EnumStringCache.Get(value, enumStringType);
        }

        /// <summary>
        /// 类型转为字符串；用于<see cref="enumType"/>类型与<see cref="_enumType"/>字符串的转化；
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string TypeToString(Type type) => type.FullNameToHierarchyString() ?? "";
    }

    /// <summary>
    /// 基础自定义枚举型下拉框事件
    /// </summary>
    public abstract class BaseCustomDropdownEventOfEnum : BaseCustomDropdownEvent
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
        /// 枚举字符串
        /// </summary>
        public virtual string enumString
        {
            get => EnumStringCache.Get(enumValue, enumStringType);
            set => enumValue = EnumValueCache.Get(enumType, value, enumStringType);
        }

        /// <summary>
        /// 枚举值
        /// </summary>
        public abstract Enum enumValue { get; set; }

        /// <summary>
        /// 显示文本
        /// </summary>
        public override string displayText => enumString;
    }

    /// <summary>
    /// 基础自定义枚举型下拉框事件
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public abstract class BaseCustomDropdownEventOfEnum<TEnum> : BaseCustomDropdownEventOfEnum
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

        /// <summary>
        /// 枚举值
        /// </summary>
        [Name("枚举值")]
        [CustomEnumPopup]
        public TEnum _enumValue = default;

        /// <summary>
        /// 枚举值
        /// </summary>
        public override Enum enumValue
        {
            get => (Enum)(object)_enumValue;
            set => _enumValue = (TEnum)(object)value;
        }
    }
}
