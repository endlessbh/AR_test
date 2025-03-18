using System;
using UnityEngine.UI;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginXGUI.Views.Dropdowns;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 基础枚举型设置下拉框选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseSetDropdownOptionOfEnum<T> : BaseSetDropdownOption<T>, IDropdownPopupAttribute
        where T : BaseSetDropdownOptionOfEnum<T>
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
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            dropdown.SetTextValue(enumString);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            if (!dropdown) return "";
            return dropdown.name + ".文本=" + enumString;
        }

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public virtual bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(CustomEnumPopupAttribute):
                    {
                        options = EnumStringsCache.Get(enumType, enumStringType);
                        return true;
                    }
            }
            options = null;
            return false;
        }

        /// <summary>
        /// 尝试获取文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        public virtual bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(CustomEnumPopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = null;
            return false;
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public virtual bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(CustomEnumPopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = null;
            return false;
        }
    }

    /// <summary>
    /// 基础枚举型设置下拉框选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public class BaseSetDropdownOptionOfEnum<T, TEnum> : BaseSetDropdownOptionOfEnum<T>
        where T : BaseSetDropdownOptionOfEnum<T, TEnum>
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

        /// <summary>
        /// 尝试获取文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        public override bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(CustomEnumPopupAttribute):
                    {
                        option = EnumStringCache.Get(EnumValueCache.Get(enumType, propertyValue.ToString(), EEnumStringType.UnderlyingType), enumStringType);
                        return true;
                    }
            }
            return base.TryGetOption(purpose, propertyPath, options, propertyValue, out option);
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public override bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(CustomEnumPopupAttribute):
                    {
                        propertyValue = Converter.instance.ConvertTo<int>(EnumValueCache.Get(enumType, option, enumStringType));
                        return true;
                    }
            }
            return base.TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
        }
    }
}
