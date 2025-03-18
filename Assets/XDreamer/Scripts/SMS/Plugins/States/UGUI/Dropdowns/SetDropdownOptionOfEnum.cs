using System;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 设置枚举型下拉框选项：设置枚举型下拉框选项可用于设置下拉框的当前选择内容
    /// </summary>
    [ComponentMenu("UGUI/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SetDropdownOptionOfEnum))]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    public class SetDropdownOptionOfEnum : BaseSetDropdownOptionOfEnum<SetDropdownOptionOfEnum>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "设置枚举型下拉框选项";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SetDropdownOptionOfEnum))]
        [Tip("设置枚举型下拉框选项可用于设置下拉框的当前选择内容", "The set enumeration type drop-down box option can be used to set the current selection of the drop-down box")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

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
}
