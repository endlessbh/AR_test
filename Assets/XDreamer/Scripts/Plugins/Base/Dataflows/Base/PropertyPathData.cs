using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Base
{
    /// <summary>
    /// 属性路径数据
    /// </summary>
    public class PropertyPathData
    {
        /// <summary>
        /// 属性路径
        /// </summary>
        public string propertyPath = "";

        /// <summary>
        /// 索引
        /// </summary>
        public int index = -1;

        /// <summary>
        /// 索引字符串：比索引值大1
        /// </summary>
        public string indexString = "";

        /// <summary>
        /// 标签
        /// </summary>
        public GUIContent label = new GUIContent();

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init() { }
    }

    /// <summary>
    /// 属性路径缓存
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PropertyPathCache<TData> where TData : PropertyPathData, new()
    {
        Dictionary<string, TData> datas = new Dictionary<string, TData>();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        public TData GetData(string propertyPath)
        {
            if (datas.TryGetValue(propertyPath, out var data)) return data;
            datas[propertyPath] = data = new TData() { propertyPath = propertyPath };
            if (PropertyPathHelper.TryGetLastArrayElementIndex(propertyPath, out var index))
            {
                data.index = index;
                data.indexString = (data.index + 1).ToString();
            }
            data.Init();
            return data;
        }

        /// <summary>
        /// 尝试获取数组元素数据
        /// </summary>
        /// <param name="propertyPath"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGetArrayElementData(string propertyPath, out TData data)
        {
            data = GetData(propertyPath);
            return data.index >= 0;
        }
    }

    /// <summary>
    /// 属性路径缓存
    /// </summary>
    public class PropertyPathCache : PropertyPathCache<PropertyPathData> { }

    /// <summary>
    /// 属性路径列表
    /// </summary>
    public interface IPropertyPathList
    {
        /// <summary>
        /// 尝试获取属性值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetPropertyValueType(out Type type);

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetPropertyPathValueType(out Type type);

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetPropertyPathValueType(int index, out Type type);

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetPropertyValue(out object value);

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetPropertyPathValue(out object value);

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetPropertyPathValue(int index, out object value);
    }

    /// <summary>
    /// 属性路径列表:通过反射方式处理，仅支持字段、属性、空形参的方法,使用时需要配合<see cref="IPropertyPathList"/>使用
    /// </summary>
    [Serializable]
    [Name("属性路径列表")]
    public class PropertyPathList : IDropdownPopupAttribute, IPropertyPathList
    {
        /// <summary>
        /// 属性路径列表
        /// </summary>
        [Name("属性路径列表")]
        public List<TypeMember> _propertyPaths = new List<TypeMember>();

        /// <summary>
        /// 反射标记量:用于获取成员时使用
        /// </summary>
        public BindingFlags bindingFlags => TypeHelper.DefaultLookup;

        /// <summary>
        /// 包含基础类型
        /// </summary>
        public bool includeBaseType => true;

        /// <summary>
        /// 转属性路径
        /// </summary>
        /// <returns></returns>
        public string ToPropertyPath() => _propertyPaths.ToString(i => i._memberName, ".");

        /// <summary>
        /// 转属性路径
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ToPropertyPath(int index)
        {
            var result = "";
            for (int i = 0; i <= index && i < _propertyPaths.Count; i++)
            {
                result += "." + _propertyPaths[i]._memberName;
            }
            return result;
        }

        #region 设置

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetPropertyPathValue(object instance, int index, object value)
        {
            if (index < 0 || index >= _propertyPaths.Count) return false;
            try
            {
                var result = instance;
                for (int i = 0; i < index; i++)
                {
                    if (result != null && DataBinderHelper.TryGetValue(result.GetType(), result, _propertyPaths[i]._memberName, out var tmpValue, default))
                    {
                        result = tmpValue;
                    }
                    else
                    {
                        return false;
                    }
                }
                return result != null && DataBinderHelper.TrySetValue(result, _propertyPaths[index]._memberName, value);
            }
            catch (Exception ex) { Debug.LogException(ex); }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetPropertyPathValue(object instance, object value) => TrySetPropertyPathValue(instance, _propertyPaths.Count - 1, value);

        #endregion

        #region 获取

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(object instance, out object value) => TryGetPropertyPathValue(instance, _propertyPaths.Count - 1, out value);

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="propertyInstanceType"></param>
        /// <param name="index"></param>
        /// <param name="typeMemberCacheValue"></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(object instance, int index, out object value)
        {
            if (index == -1)
            {
                value = instance;
                return true;
            }
            if (index < 0 || index >= _propertyPaths.Count)
            {
                value = default;
                return false;
            }
            try
            {
                var result = instance;
                for (int i = 0; i <= index; i++)
                {
                    if (result != null && DataBinderHelper.TryGetValue(result.GetType(), result, _propertyPaths[i]._memberName, out var tmpValue, default))
                    {
                        result = tmpValue;
                    }
                    else
                    {
                        value = default;
                        return false;
                    }
                }
                value = result;
                return true;
            }
            catch (Exception ex) { Debug.LogException(ex); }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取类型成员集合缓存值
        /// </summary>
        /// <param name="propertyInstanceType"></param>
        /// <param name="typeMemberCacheValue"></param>
        /// <returns></returns>
        public bool TryGetTypeMemberCacheValue(Type propertyInstanceType, out TypeMemberCacheValue typeMemberCacheValue) => TryGetTypeMemberCacheValue(propertyInstanceType, _propertyPaths.Count - 1, out typeMemberCacheValue);

        /// <summary>
        /// 尝试获取类型成员集合缓存值
        /// </summary>
        /// <param name="propertyInstanceType"></param>
        /// <param name="index"></param>
        /// <param name="typeMemberCacheValue"></param>
        /// <returns></returns>
        public bool TryGetTypeMemberCacheValue(Type propertyInstanceType, int index, out TypeMemberCacheValue typeMemberCacheValue)
        {
            if (index == -1)
            {
                typeMemberCacheValue = TypeMemberCache.Get(propertyInstanceType, "");
                return typeMemberCacheValue != null && typeMemberCacheValue.memberValueType != null;
            }
            if (index < 0 || index >= _propertyPaths.Count)
            {
                typeMemberCacheValue = default;
                return false;
            }
            try
            {
                var type = propertyInstanceType;
                TypeMemberCacheValue result = default;
                for (int i = 0; i <= index; i++)
                {
                    if (type == null)
                    {
                        typeMemberCacheValue = default;
                        return false;
                    }

                    var member = _propertyPaths[i];
                    result = TypeMemberCache.GetCacheValue(type, member._memberName, member.bindingFlags, member.includeBaseType);
                    type = result.memberValueType;
                }

                typeMemberCacheValue = result ?? TypeMemberCache.GetCacheValue(type, "", bindingFlags, includeBaseType);
                return typeMemberCacheValue != null;
            }
            catch (Exception ex) { Debug.LogException(ex); }
            typeMemberCacheValue = default;
            return false;
        }

        /// <summary>
        /// 尝试获取类型成员集合缓存值
        /// </summary>
        /// <param name="propertyInstanceType"></param>
        /// <param name="index"></param>
        /// <param name="typeMembersCacheValue"></param>
        /// <returns></returns>
        public bool TryGetTypeMembersCacheValue(Type propertyInstanceType, int index, out TypeMembersCacheValue typeMembersCacheValue)
        {
            try
            {
                var type = propertyInstanceType;
                for (int i = 0; i <= index && i < _propertyPaths.Count; i++)
                {
                    if (type == null)
                    {
                        typeMembersCacheValue = default;
                        return false;
                    }

                    var member = _propertyPaths[i];
                    type = TypeMemberCache.GetCacheValue(type, member._memberName, member.bindingFlags, member.includeBaseType).memberValueType;
                }
                typeMembersCacheValue = TypeMembersCache.GetCacheValue(type, bindingFlags, includeBaseType);
                return typeMembersCacheValue != null;
            }
            catch (Exception ex) { Debug.LogException(ex); }
            typeMembersCacheValue = default;
            return false;
        }

        #endregion

        /// <summary>
        /// 属性实例类型
        /// </summary>
        public Type propertyInstanceType { get; private set; }

        /// <summary>
        /// 属性实例
        /// </summary>
        public object propertyInstance { get; private set; }

        /// <summary>
        /// 宿主实例
        /// </summary>
        public object hostInstance { get; private set; }

        /// <summary>
        /// 设置实例
        /// </summary>
        /// <param name="propertyInstanceType"></param>
        /// <param name="hostInstance"></param>
        /// <param name="propertyInstance"></param>
        public void SetInstance(Type propertyInstanceType, object hostInstance, object propertyInstance = default)
        {
            this.propertyInstanceType = propertyInstanceType;
            this.propertyInstance = propertyInstance;
            this.hostInstance = hostInstance;
        }

        #region IDropdownPopupAttribute

        /// <summary>
        /// 尝试获取选项数组
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="propertyPath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            if (PropertyPathHelper.TryGetLastArrayElementIndex(propertyPath, out var index) && propertyInstanceType != null && TryGetTypeMembersCacheValue(propertyInstanceType, index - 1, out var cacheValue))
            {
                options = cacheValue.memberNames;
                return true;
            }
            options = default;
            return false;
        }

        /// <summary>
        /// 尝试获取选项
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="propertyPath"></param>
        /// <param name="options"></param>
        /// <param name="propertyValue"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            option = (propertyValue as string) ?? "";
            return true;
        }

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="propertyPath"></param>
        /// <param name="options"></param>
        /// <param name="option"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            propertyValue = option;
            return true;
        }

        #endregion

        #region IPropertyPathList

        bool IPropertyPathList.TryGetPropertyValueType(out Type type)
        {
            type = propertyInstanceType;
            return type != null;
        }

        bool IPropertyPathList.TryGetPropertyPathValueType(out Type type) => ((IPropertyPathList)this).TryGetPropertyPathValueType(_propertyPaths.Count - 1, out type);

        bool IPropertyPathList.TryGetPropertyPathValueType(int index, out Type type)
        {
            if (index < 0)
            {
                return ((IPropertyPathList)this).TryGetPropertyValueType(out type);
            }
            if (index >= _propertyPaths.Count)
            {
                type = default;
                return false;
            }

            if (((IPropertyPathList)this).TryGetPropertyValueType(out var instance) && TryGetTypeMemberCacheValue(instance, index, out var value))
            {
                type = value.memberValueType;
                return true;
            }
            type = default;
            return false;
        }

        bool IPropertyPathList.TryGetPropertyValue(out object value)
        {
            try
            {
                value = propertyInstance;
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        bool IPropertyPathList.TryGetPropertyPathValue(out object value) => TryGetPropertyPathValue(propertyInstance, out value);

        bool IPropertyPathList.TryGetPropertyPathValue(int index, out object value) => TryGetPropertyPathValue(propertyInstance, index, out value);

        #endregion
    }

    /// <summary>
    /// 类型成员
    /// </summary>
    [Serializable]
    public class TypeMember
    {
        /// <summary>
        /// 成员名称
        /// </summary>
        [Name("成员名称")]
        [Tip("期望绑定的成员名称", "Name of member expected to bind")]
        [MemberNamePopup]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _memberName = "";

        /// <summary>
        /// 反射标记量:用于获取成员时使用
        /// </summary>
        public BindingFlags bindingFlags => TypeHelper.DefaultLookup;

        /// <summary>
        /// 包含基础类型
        /// </summary>
        public bool includeBaseType => true;
    }
}
