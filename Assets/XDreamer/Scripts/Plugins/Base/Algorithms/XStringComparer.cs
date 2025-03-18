using System;
using System.Text.RegularExpressions;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Algorithms
{
    /// <summary>
    /// 字符串比较规则
    /// </summary>
    [Name("字符串比较规则")]
    public enum EStringCompareRule
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        Any = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 包含
        /// </summary>
        [Name("包含")]
        Contain,

        /// <summary>
        /// 不包含
        /// </summary>
        [Name("不包含")]
        NotContain,

        /// <summary>
        /// 相等
        /// </summary>
        [Name("相等")]
        Equal,

        /// <summary>
        /// 不相等
        /// </summary>
        [Name("不相等")]
        NotEqual,

        /// <summary>
        /// 正则表达式匹配
        /// </summary>
        [Name("正则表达式匹配")]
        RegexMatch,

        /// <summary>
        /// 正则表达式不匹配
        /// </summary>
        [Name("正则表达式不匹配")]
        RegexNotMatch,
    }

    /// <summary>
    /// 扩展
    /// </summary>
    public static class StringCompareRuleExtension
    {
        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <param name="stringCompareRule"></param>
        /// <param name="value"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public static bool IsMatch(this EStringCompareRule stringCompareRule, string value, string compareValue)
        {
            switch (stringCompareRule)
            {
                case EStringCompareRule.Any: return true;
                case EStringCompareRule.Contain: return value.Contains(compareValue);
                case EStringCompareRule.NotContain: return !value.Contains(compareValue);
                case EStringCompareRule.Equal: return value == compareValue;
                case EStringCompareRule.NotEqual: return value != compareValue;
                case EStringCompareRule.RegexMatch: return (new Regex(compareValue)).IsMatch(value);
                case EStringCompareRule.RegexNotMatch: return !(new Regex(compareValue)).IsMatch(value);
                default: return false;
            }
        }
    }

    /// <summary>
    /// 可比较字符串
    /// </summary>
    [Serializable]
    [Name("可比较字符串")]
    public class ComparableString
    {
        /// <summary>
        /// 比较规则
        /// </summary>
        [Name("比较规则")]
        [EnumPopup]
        public EStringCompareRule _compareRule = EStringCompareRule.Any;

        /// <summary>
        /// 待比较值
        /// </summary>
        [Name("待比较值")]
        [HideInSuperInspector(nameof(_compareRule), EValidityCheckType.Equal | EValidityCheckType.Or, EStringCompareRule.Any, nameof(_compareRule), EValidityCheckType.Equal, EStringCompareRule.None)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [OnlyMemberElements]
        public StringPropertyValue _compareValue = new StringPropertyValue();

        /// <summary>
        /// 待比较值
        /// </summary>
        public string compareValue => _compareValue.GetValue();

        /// <summary>
        /// 传入值是否与本对象中待比较值匹配
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsMatch(string value) => _compareRule.IsMatch(value, compareValue);
    }

    /// <summary>
    /// 字符串比较
    /// </summary>
    [Serializable]
    [Name("字符串比较")]
    public class XStringComparer
    {
        /// <summary>
        /// 比较规则
        /// </summary>
        [Name("比较规则")]
        [EnumPopup]
        public EStringCompareRule _compareRule = EStringCompareRule.Any;

        /// <summary>
        /// 关键字
        /// </summary>
        [Name("关键字")]
        [HideInSuperInspector(nameof(_compareRule), EValidityCheckType.Equal| EValidityCheckType.Or, EStringCompareRule.None,
            nameof(_compareRule), EValidityCheckType.Equal, EStringCompareRule.Any)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _keyword = "";

        /// <summary>
        /// 关键字是否匹配对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsMatch(string value) => _compareRule.IsMatch(value, _keyword);
    }
}
