using System;
using System.ComponentModel;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.Caches;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.XGUI.Dropdowns;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Algorithms;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.PluginSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 基础枚举型下拉框切换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseDropdownSwitchOfEnum<T> : BaseDropdownSwitch<T>, IDropdownPopupAttribute
        where T : BaseDropdownSwitchOfEnum<T>
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
        /// 枚举字符串：如下拉框的选择文本与本参数值相等时，会标记当前状态组件为完成态；通过<see cref="enumValue"/>与<see cref="enumStringType"/>设置或获取对应的枚举字符串
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
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            if (dropdown)
            {
                dropdown.onValueChanged.AddListener(OnDropdownSwitch);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            if (dropdown)
            {
                dropdown.onValueChanged.RemoveListener(OnDropdownSwitch);
            }
            base.OnExit(stateData);
        }

        /// <summary>
        /// 当下拉框切换时：如下拉框的选择文本与<see cref="enumString"/>值相等时，会标记当前状态组件为完成态
        /// </summary>
        /// <param name="val"></param>
        protected virtual void OnDropdownSwitch(int val)
        {
            if (dropdown.TryGetTextValue(out string text) && text == enumString)
            {
                finished = true;
            }
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
                        propertyValue = option ?? "";
                        return true;
                    }
            }
            propertyValue = null;
            return false;
        }
    }

    /// <summary>
    /// 基础枚举型下拉框切换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    public abstract class BaseDropdownSwitchOfEnum<T, TEnum> : BaseDropdownSwitchOfEnum<T>
        where T : BaseDropdownSwitchOfEnum<T, TEnum>
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
