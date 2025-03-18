using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginTools.PropertyDatas
{
    /// <summary>
    /// 交互属性
    /// </summary>
    [Name("交互属性")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    public class InteractProperty : InteractProvider
    {
        /// <summary>
        /// 属性数据列表：关键字和值不能同时出现重复
        /// </summary>
        [Name("属性数据列表")]
        [OnlyMemberElements]
        public InteractPropertyDataList _dataList = new InteractPropertyDataList();
    }

    /// <summary>
    /// 交互属性数据列表
    /// </summary>
    [Serializable]
    public class InteractPropertyDataList
    {
        [Name("属性数据列表")]
        public List<InteractPropertyData> _datas = new List<InteractPropertyData>();

        /// <summary>
        /// 获取所有键
        /// </summary>
        /// <returns></returns>
        public string[] GetKeys() => _datas.Where(d => d._key.TryGetValue(out _)).Cast(d => d._key.TryGetValue(out var keyValue) ? keyValue : "").ToArray();

        /// <summary>
        /// 获取所有匹配关键字的文本值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] GetValues(string key) => _datas.Where(item => item.IsMatch(key)).Cast(d => d.value).ToArray();

        /// <summary>
        /// 获取属性全部的文本值
        /// </summary>
        /// <returns></returns>
        public string[] GetAllValues() => _datas.Cast(d => d.value).ToArray();

        /// <summary>
        /// 包含关键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return _datas.Exists(d => d._key.propertyValue == key);
        }

        /// <summary>
        /// 包含
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(string key, string value)
        {
            return _datas.Exists(d => d.IsMatch(key, value));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(string key, string value)
        {
            _datas.Add(new InteractPropertyData(key, value));
            return true;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key) => Remove(d => d.IsMatch(key));

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(string key, string value) => Remove(d => d.IsMatch(key, value));

        private bool Remove(Func<InteractPropertyData, bool> isMatch)
        {
            bool result = false;
            for (int i = _datas.Count - 1; i >= 0; --i)
            {
                var data = _datas[i];
                if (isMatch.Invoke(data))
                {
                    _datas.RemoveAt(i);
                    result = true;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 交互属性助手
    /// </summary>
    public static class InteractPropertyHelper
    {
        /// <summary>
        /// 获取游戏对象上获取与关键字匹配的第一个交互属性值
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="keyExpression"></param>
        /// <returns></returns>
        public static InteractPropertyData GetInteractPropertyValue(GameObject gameObject, string keyOrExpression, bool includeChildren = true)
        {
            if (gameObject)
            {
                var key = GetKey(keyOrExpression);

                InteractProperty[] array = includeChildren ? gameObject.GetComponentsInChildren<InteractProperty>() : gameObject.GetComponents<InteractProperty>();

                // 查找第一个符合关键字的属性值
                foreach (var property in array)
                {
                    var pd = property._dataList._datas.Find(d => d.key == key);
                    if (pd != null)
                    {
                        return pd;
                    }
                }
            }
            return default;
        }

        /// <summary>
        /// 从游戏对象上获取与关键字匹配的交互属性值
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="keyOrExpression"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static InteractPropertyData[] GetInteractPropertyValues(GameObject gameObject, string keyOrExpression, bool includeChildren = true)
        {
            if (gameObject)
            {
                var key = GetKey(keyOrExpression);

                if (includeChildren)
                {
                    return GetInteractPropertyValues(gameObject.GetComponentsInChildren<InteractProperty>(), keyOrExpression);
                }
                else
                {
                    return GetInteractPropertyValues(gameObject.GetComponents<InteractProperty>(), keyOrExpression);
                }
            }
            return default;
        }

        /// <summary>
        /// 从全局中获取交互属性值
        /// </summary>
        /// <param name="keyOrExpression"></param>
        /// <returns></returns>
        public static InteractPropertyData[] GetInteractPropertyValues(string keyOrExpression)
        {
            return GetInteractPropertyValues(ComponentCache.GetComponents<InteractProperty>(), keyOrExpression);
        }

        private static InteractPropertyData[] GetInteractPropertyValues(IEnumerable<InteractProperty> interactProperties, string keyOrExpression)
        {
            var key = GetKey(keyOrExpression);

            var list = new List<InteractPropertyData>();
            foreach (var property in interactProperties)
            {
                list.AddRange(property._dataList._datas.Where(d => d.key == key));
            }
            return list.ToArray();
        }

        /// <summary>
        /// 从表达式中获取关键字
        /// </summary>
        /// <param name="keyOrExpression"></param>
        /// <returns></returns>
        public static string GetKey(string keyOrExpression)
        {
            var key = keyOrExpression;
            if (ExpressionStringAnalysisResult.TryParse(keyOrExpression, out var result) && !string.IsNullOrEmpty(result.key))
            {
                key = result.key;
            }
            return key;
        }

        /// <summary>
        /// 获取场景中所有T类型的关键字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetAllKeysInScene<T>() where T : InteractProperty => GetPropertyDatas<T>(ds => ds._dataList.GetKeys());

        /// <summary>
        /// 获取场景中所有T类型的属性文本值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string[] GetAllTextValuesInScene<T>() where T : InteractProperty => GetPropertyDatas<T>(ds => ds._dataList.GetAllValues());

        /// <summary>
        /// 获取场景中与关键字相匹配的所有T类型属性文本值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static string[] GetAllTextValuesInScene<T>(string key) where T : InteractProperty => GetPropertyDatas<T>(ds => ds._dataList.GetValues(key));

        private static string[] GetPropertyDatas<T>(Func<T, IEnumerable<string>> findValues) where T : InteractProperty
        {
            var list = new List<string>();
            foreach (var item in ComponentCache.GetComponents<T>())
            {
                list.AddRange(findValues.Invoke(item));
            }
            list.Distinct();
            list.Sort();
            return list.ToArray();
        }

        /// <summary>
        /// 获取当前组件上可交互属性上所有与关键字匹配的属性值对象
        /// </summary>
        /// <param name="component"></param>
        /// <param name="key"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static InteractPropertyData[] GetInteractPropertyValues(this Component component, string key, bool includeChildren = true)
        {
            var propertys = includeChildren ? component.GetComponentsInChildren<InteractProperty>() : component.GetComponents<InteractProperty>();
            if (propertys.Length > 0)
            {
                var list = new List<InteractPropertyData>();
                foreach (var item in propertys)
                {
                    list.AddRange(item._dataList._datas.Where(d => d.key == key));
                }
                return list.ToArray();
            }
            return Empty<InteractPropertyData>.Array;
        }

        /// <summary>
        /// 获取当前组件上可交互属性上所有与关键字匹配的文本值
        /// </summary>
        /// <param name="component"></param>
        /// <param name="key"></param>
        /// <param name="includeChildren"></param>
        /// <returns></returns>
        public static string[] GetAllTextValues(this Component component, string key, bool includeChildren = true)
        {
            var propertys = includeChildren ? component.GetComponentsInChildren<InteractProperty>() : component.GetComponents<InteractProperty>();
            if (propertys.Length > 0)
            {
                var list = new List<string>();
                foreach (var item in propertys)
                {
                    list.AddRange(item._dataList.GetValues(key));
                }
                return list.ToArray();
            }
            return Empty<string>.Array;
        }
    }


}
