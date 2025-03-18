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
    /// 设置枚举型下拉框选项列表：设置枚举型下拉框选项列表可用于设置下拉框的下拉选项列表内容
    /// </summary>
    [ComponentMenu("UGUI/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SetDropdownOptionsOfEnum))]
    [Tip("设置枚举型下拉框选项列表可用于设置下拉框的下拉选项列表内容", "Set enumeration type drop-down box option list can be used to set the contents of the drop-down option list of the drop-down box")]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    public class SetDropdownOptionsOfEnum : BaseSetDropdownOptionsOfEnum<SetDropdownOptionsOfEnum>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "设置枚举型下拉框选项列表";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(SetDropdownOptionsOfEnum))]
        [Tip("设置枚举型下拉框选项列表可用于设置下拉框的下拉选项列表内容", "Set enumeration type drop-down box option list can be used to set the contents of the drop-down option list of the drop-down box")]
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
        /// 类型转为字符串；用于<see cref="enumType"/>类型与<see cref="_enumType"/>字符串的转化；
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string TypeToString(Type type) => type.FullNameToHierarchyString() ?? "";
    }
}
