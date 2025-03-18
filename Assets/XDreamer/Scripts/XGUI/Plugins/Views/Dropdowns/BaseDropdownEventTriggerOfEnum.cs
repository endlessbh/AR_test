using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Caches;
using System.Text.RegularExpressions;
using XCSJ.Algorithms;
using XCSJ.Helper;
using XCSJ.Extension.XGUI.Dropdowns;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 基础枚举型下拉框事件触发器
    /// </summary>
    public abstract class BaseDropdownEventTriggerOfEnum : BaseDropdownEventTrigger, IDropdownPopupAttribute
    {
        /// <summary>
        /// 自定义枚举型下拉框事件列表
        /// </summary>
        public abstract IEnumerable<BaseCustomDropdownEventOfEnum> customDropdownEventOfEnums { get; }

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
                        var match = Regex.Match(propertyPath, @"\d+");
                        if (match.Success && customDropdownEventOfEnums.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is BaseCustomDropdownEventOfEnum e)
                        {
                            options = EnumStringsCache.Get(e.enumType, e.enumStringType);
                            return true;
                        }
                        break;
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
    /// 基础枚举型下拉框事件触发器
    /// </summary>
    public abstract class BaseDropdownEventTriggerOfEnum<TEnum, TEvent> : BaseDropdownEventTriggerOfEnum
        where TEvent : BaseCustomDropdownEventOfEnum<TEnum>
    {
        /// <summary>
        /// 自定义枚举型下拉框事件列表
        /// </summary>
        [Name("自定义枚举型下拉框事件列表")]
        [SerializeField]
        public List<TEvent> _events = new List<TEvent>();

        /// <summary>
        /// 自定义枚举型下拉框事件列表
        /// </summary>
        public virtual IEnumerable<TEvent> events => _events;

        /// <summary>
        /// 自定义枚举型下拉框事件列表
        /// </summary>
        public override IEnumerable<BaseCustomDropdownEventOfEnum> customDropdownEventOfEnums => _events.Cast(e => (BaseCustomDropdownEventOfEnum)e);

        /// <summary>
        /// 自定义基础下拉框事件列表
        /// </summary>
        public override IEnumerable<BaseCustomDropdownEvent> customDropdownEvents => _events.Cast(e => (BaseCustomDropdownEvent)e);

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
                        var match = Regex.Match(propertyPath, @"\d+");
                        if (match.Success && customDropdownEventOfEnums.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is BaseCustomDropdownEventOfEnum e)
                        {
                            option = EnumStringCache.Get(EnumValueCache.Get(e.enumType, propertyValue.ToString(), EEnumStringType.UnderlyingType), e.enumStringType);
                            return true;
                        }
                        option = default;
                        return false;
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
                        var match = Regex.Match(propertyPath, @"\d+");
                        if (match.Success && customDropdownEventOfEnums.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is BaseCustomDropdownEventOfEnum e)
                        {
                            propertyValue = Converter.instance.ConvertTo<int>(EnumValueCache.Get(e.enumType, option, e.enumStringType));
                            return true;
                        }
                        propertyValue = default;
                        return false;
                    }
            }
            return base.TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
        }
    }
}
