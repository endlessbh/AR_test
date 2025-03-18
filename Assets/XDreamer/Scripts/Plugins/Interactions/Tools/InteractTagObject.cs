using System;
using System.Collections.Generic;
using System.Linq;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 交互标签对象
    /// </summary>
    public abstract class InteractTagObject : InteractObject
    {
        /// <summary>
        /// 标签属性
        /// </summary>
        [Name("标签属性")]
        public TagProperty _tagProperty = new TagProperty();
    }

    /// <summary>
    /// 标签属性数据
    /// </summary>
    [Serializable]
    public class TagPropertyData : BaseInteractPropertyData
    {
        /// <summary>
        /// 空构造函数
        /// </summary>
        public TagPropertyData() { }


        /// <summary>
        /// 空构造函数
        /// </summary>
        public TagPropertyData(string key) : base(key) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        public TagPropertyData(string key, string value) : base(key, value) { }

        /// <summary>
        /// 比较规则
        /// </summary>
        [Name("比较规则")]
        [EnumPopup]
        public EStringCompareRule _compareRule = EStringCompareRule.Equal;

        /// <summary>
        /// 与输入标签字符串数组中的其中之一匹配
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsMatch(params string[] tags)
        {
            var tagValue = value;

            // 输入标签无效且标签表达式值为空时认为匹配
            if (tags == null || tags.Length == 0)
            {
                return string.IsNullOrEmpty(tagValue);
            }

            foreach (var item in tags)
            {
                if (_compareRule.IsMatch(tagValue, item)) return true;
            }
            return false;
        }

        /// <summary>
        /// 与输入标签匹配:首先关键字需要相等
        /// </summary>
        /// <param name="tagPropertyData"></param>
        /// <returns></returns>
        public bool IsMatch(TagPropertyData tagPropertyData)
        {
            if (tagPropertyData == null) return false;

            return key == tagPropertyData.key && IsMatch(tagPropertyData.value);
        }
    }

    /// <summary>
    /// 标签属性
    /// </summary>
    [Serializable]
    public class TagProperty
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TagProperty() { }

        /// <summary>
        /// 标签属性数据列表
        /// </summary>
        [Name("标签属性数据列表")]
        public List<TagPropertyData> _tagPropertyDatas = new List<TagPropertyData>();

        /// <summary>
        /// 第一个键
        /// </summary>
        public string firstKey => _tagPropertyDatas.FirstOrDefault()?.key??"";

        /// <summary>
        /// 第一个值
        /// </summary>
        public string firstValue => _tagPropertyDatas.FirstOrDefault()?.value ?? "";

        /// <summary>
        /// 获取第一个符合键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetFirstValue(string key)
        {
            var data = _tagPropertyDatas.Find(d => d.key == key);
            return data != null ? data.value : "";
        }

        /// <summary>
        /// 设置第一个符合键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool SetFirstValue(string key, string value)
        {
            var data = _tagPropertyDatas.Find(d => d.key == key);
            if (data != null)
            {
                data.value = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取所有符合键的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] GetValues(string key) => _tagPropertyDatas.Where(t => t.key == key).Cast(d => d.value).ToArray();

        /// <summary>
        /// 与输入标签字符串数组中的其中之一匹配
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsMatch(params string[] tags) => _tagPropertyDatas.Exists(d => d.IsMatch(tags));

        //public bool Contian(TagProperty tagProperty) => true;

        //public bool Cross(TagProperty tagProperty) => true;

        //public bool Equals(TagProperty tagProperty) => true;
    }
}
