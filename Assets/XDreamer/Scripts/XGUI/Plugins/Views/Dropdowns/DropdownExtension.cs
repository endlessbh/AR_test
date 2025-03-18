using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.PluginXGUI.Views.Dropdowns
{
    /// <summary>
    /// 下拉框扩展
    /// </summary>
    public static class DropdownExtension
    {
        /// <summary>
        /// 添加选项：将枚举中定义的字段信息添加为下拉框的选项；
        /// </summary>
        /// <typeparam name="T">枚举泛型类型</typeparam>
        /// <param name="dropdown"></param>
        /// <param name="enumStringType"></param>
        /// <param name="clearOptions"></param>
        /// <returns></returns>
        public static bool AddOptions<T>(this Dropdown dropdown, EEnumStringType enumStringType = EEnumStringType.NameAttributeCN, bool clearOptions = true)
        {
            return AddOptions(dropdown, typeof(T), enumStringType, clearOptions);
        }

        /// <summary>
        /// 添加选项：将枚举中定义的字段信息添加为下拉框的选项；
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="e"></param>
        /// <param name="enumStringType"></param>
        /// <param name="clearOptions"></param>
        /// <returns></returns>
        public static bool AddOptions(this Dropdown dropdown, Enum e, EEnumStringType enumStringType = EEnumStringType.NameAttributeCN, bool clearOptions = true)
        {
            return AddOptions(dropdown, e?.GetType(), enumStringType, clearOptions);
        }

        /// <summary>
        /// 添加选项：将枚举中定义的字段信息添加为下拉框的选项；
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="enumType"></param>
        /// <param name="enumStringType"></param>
        /// <param name="clearOptions"></param>
        /// <returns></returns>
        public static bool AddOptions(this Dropdown dropdown, Type enumType, EEnumStringType enumStringType = EEnumStringType.NameAttributeCN, bool clearOptions = true)
        {
            if (!dropdown) return false;

            var options = EnumStringsCache.Get(enumType, enumStringType)?.ToList();
            if (options == null || options.Count == 0) return false;

            if (clearOptions) dropdown.ClearOptions();
            dropdown.AddOptions(options);

            return true;
        }

        /// <summary>
        /// 设置枚举值，通过枚举值设置下拉框的值；设置值时会触发下拉框的值已变更的事件<see cref="Dropdown.onValueChanged"/>
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="e"></param>
        /// <param name="enumStringType"></param>
        /// <returns></returns>
        public static int SetEnumValue(this Dropdown dropdown, Enum e, EEnumStringType enumStringType = EEnumStringType.NameAttributeCN)
        {
            if (!dropdown || e == null) return -1;
            return dropdown.SetTextValue(EnumStringCache.Get(e, enumStringType));
        }

        /// <summary>
        /// 尝试获取枚举值
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="enumType"></param>
        /// <param name="enumValue"></param>
        /// <param name="enumStringType"></param>
        /// <returns></returns>
        public static bool TryGetEnumValue(this Dropdown dropdown, Type enumType, out Enum enumValue, EEnumStringType enumStringType = EEnumStringType.NameAttributeCN)
        {
            if (!TryGetTextValue(dropdown, out string text))
            {
                enumValue = default;
                return false;
            }
            return EnumValueCache.TryGet(enumType, text, out enumValue, enumStringType);
        }

        /// <summary>
        /// 尝试获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dropdown"></param>
        /// <param name="enumValue"></param>
        /// <param name="enumStringType"></param>
        /// <returns></returns>
        public static bool TryGetEnumValue<T>(this Dropdown dropdown, out T enumValue, EEnumStringType enumStringType = EEnumStringType.NameAttributeCN) where T : struct
        {
            if (!TryGetTextValue(dropdown, out string text))
            {
                enumValue = default;
                return false;
            }
            return EnumValueCache<T>.TryGet(text, out enumValue, enumStringType);
        }

        /// <summary>
        /// 设置文本值
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int SetTextValue(this Dropdown dropdown, string text)
        {
            if (!dropdown || string.IsNullOrEmpty(text)) return -1;

            var index = dropdown.options.IndexOf(data => data.text == text);

            if (index >= 0) dropdown.value = index;

            return dropdown.value;
        }

        /// <summary>
        /// 获取下拉框当前选择的文本值
        /// </summary>
        /// <param name="dropdown"></param>
        /// <returns>如果获取失败，返回null</returns>
        public static string GetTextValue(this Dropdown dropdown) => TryGetTextValue(dropdown, out string text) ? text : null;

        /// <summary>
        /// 尝试获取下拉框当前选择的文本值
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool TryGetTextValue(this Dropdown dropdown, out string text)
        {
            if (!dropdown)
            {
                text = default;
                return false;
            }

            if (dropdown.captionText)
            {
                text = dropdown.captionText.text;
                return true;
            }
            else
            {
                try
                {
                    text = dropdown.options[dropdown.value].text;
                    return true;
                }
                catch
                {
                    text = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// 尝试获取下拉框的文本值
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool TryGetTextValue(this Dropdown dropdown, int value, out string text)
        {
            if (!dropdown)
            {
                text = default;
                return false;
            }

            try
            {
                text = dropdown.options[value].text;
                return true;
            }
            catch
            {
                text = default;
                return false;
            }
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="value"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool TryGetValue(this Dropdown dropdown,out int value, out string text)
        {
            if (!dropdown)
            {
                value = default;
                text = default;
                return false;
            }

            value = dropdown.value;
            if (dropdown.captionText)
            {
                text = dropdown.captionText.text;
                return true;
            }
            else
            {
                try
                {
                    text = dropdown.options[dropdown.value].text;
                    return true;
                }
                catch
                {
                    text = default;
                    return false;
                }
            }
        }
    }
}
