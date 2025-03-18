using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;
using XCSJ.Tools;
using XCSJ.PluginXGUI.Base;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    [Name(Title, nameof(SetDropdownOptionsOfEnum))]
    [XCSJ.Attributes.Icon(EIcon.Dropdown)]
    [Tip("设置枚举型下拉框选项列表可用于设置BaseSetDropdownOptionsOfEnum下拉框的下拉选项列表内容", "Set enumeration type drop-down box option list can be used to set the contents of the drop-down option list of BaseSetDropdownOptionsOfEnum drop-down box")]
    [Tool(XGUICategory.Component, nameof(XGUIManager), rootType = typeof(ToolsManager))]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public class SetDropdownOptionsOfEnum: BaseSetDropdownOptionsOfEnum
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "设置枚举型下拉框选项列表";

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
