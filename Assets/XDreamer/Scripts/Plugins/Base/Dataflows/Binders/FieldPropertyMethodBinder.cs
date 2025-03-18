using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 绑定类型:标识类型中可绑定成员信息的类型
    /// </summary>
    [Name("绑定类型")]
    [Tip("标识类型中可绑定成员信息的类型", "Identifies the type of member information that can be bound in the type")]
    public enum EBindType
    {
        /// <summary>
        /// 字段:标识可绑定类型中字段信息
        /// </summary>
        [Name("字段")]
        [Tip("标识可绑定类型中字段信息", "Identifies the field information in the bindable type")]
        Field,

        /// <summary>
        /// 属性:标识可绑定类型中属性信息
        /// </summary>
        [Name("属性")]
        [Tip("标识可绑定类型中属性信息", "Identifies the attribute information in the bindable type")]
        Property,

        /// <summary>
        /// 方法:标识可绑定类型中方法信息（仅使用空参数方法）
        /// </summary>
        [Name("方法")]
        [Tip("标识可绑定类型中方法信息（仅使用空参数方法）", "Identifies the method information in the bindable type (only empty parameter methods are used)")]
        Method,

        /// <summary>
        /// 仅声明字段:标识可绑定类型中仅声明字段信息（不包含基类）
        /// </summary>
        [Name("仅声明字段")]
        [Tip("标识可绑定类型中仅声明字段信息（不包含基类）", "Identifies that only field information is declared in the bindable type (excluding the base class)")]
        FieldDeclaredOnly,

        /// <summary>
        /// 仅声明属性:标识可绑定类型中仅声明属性信息（不包含基类）
        /// </summary>
        [Name("仅声明属性")]
        [Tip("标识可绑定类型中仅声明属性信息（不包含基类）", "Identifies that only attribute information is declared in the bindable type (excluding the base class)")]
        PropertyDeclaredOnly,

        /// <summary>
        /// 仅声明方法:标识可绑定类型中仅声明方法信息（不包含基类，仅使用空参数方法）
        /// </summary>
        [Name("仅声明方法")]
        [Tip("标识可绑定类型中仅声明方法信息（不包含基类，仅使用空参数方法）", "Identify that only method information is declared in the bindable type (no base class, only empty parameter methods are used)")]
        MethodDeclaredOnly,
    }

    /// <summary>
    /// 绑定成员信息类型
    /// </summary>
    [Name("绑定成员信息类型")]
    public enum EBindMemberInfoType
    {
        /// <summary>
        /// 字段
        /// </summary>
        [Name("字段")]
        Field,

        /// <summary>
        /// 属性
        /// </summary>
        [Name("属性")]
        Property,

        /// <summary>
        /// 方法
        /// </summary>
        [Name("方法")]
        Method,
    }

    /// <summary>
    /// 字段属性方法绑定器
    /// </summary>
    [Name("字段属性方法绑定器")]
    [Serializable]
    public class FieldPropertyMethodBinder : TypeMemberBinder
    {
        /// <summary>
        /// 绑定类型:标识类型中可绑定成员信息的类型
        /// </summary>
        [Name("绑定类型")]
        [Tip("标识类型中可绑定成员信息的类型", "Identifies the type of member information that can be bound in the type")]
        [EnumPopup]
        public EBindType _bindType = EBindType.PropertyDeclaredOnly;

        /// <summary>
        /// 绑定标志
        /// </summary>
        public override BindingFlags bindingFlags
        {
            get
            {
                switch (_bindType)
                {
                    case EBindType.FieldDeclaredOnly:
                    case EBindType.PropertyDeclaredOnly:
                    case EBindType.MethodDeclaredOnly:
                        {
                            return base.bindingFlags | BindingFlags.DeclaredOnly;
                        }
                }
                return base.bindingFlags;
            }
        }

        /// <summary>
        /// 绑定成员信息类型
        /// </summary>
        public EBindMemberInfoType bindMemberInfoType
        {
            get
            {
                switch (_bindType)
                {
                    case EBindType.Field: 
                    case EBindType.FieldDeclaredOnly: return EBindMemberInfoType.Field;
                    case EBindType.Property: 
                    case EBindType.PropertyDeclaredOnly: return EBindMemberInfoType.Property;
                    case EBindType.Method:
                    case EBindType.MethodDeclaredOnly: return EBindMemberInfoType.Method;
                    default: throw new ArgumentException();
                }
            }
        }

        /// <summary>
        /// 成员信息对象
        /// </summary>
        public override MemberInfo memberInfo 
        { get
            {
                switch (bindMemberInfoType)
                {
                    case EBindMemberInfoType.Field: return fieldInfo;
                    case EBindMemberInfoType.Property: return propertyInfo;
                    case EBindMemberInfoType.Method: return methodInfo;
                    default: throw new ArgumentException();
                }
            } 
        }

        /// <summary>
        /// 成员值，当成员信息类型为字段或属性时本参数有意义；
        /// </summary>
        public override object memberValue 
        {
            get
            {
                switch (bindMemberInfoType)
                {
                    case EBindMemberInfoType.Field: 
                    case EBindMemberInfoType.Property: return GetMemberValue();
                    case EBindMemberInfoType.Method:
                    default: return null;
                }
            }
            set
            {
                switch (bindMemberInfoType)
                {
                    case EBindMemberInfoType.Field:
                    case EBindMemberInfoType.Property: SetMemberValue(value); break;
                    case EBindMemberInfoType.Method: methodInfo?.Invoke(mainObject, null); break;
                    default: break;
                }
            }
        }

        /// <summary>
        /// 字段信息对象
        /// </summary>
        public FieldInfo fieldInfo => bindMemberInfoType == EBindMemberInfoType.Field ? FieldInfoCache.Get(mainType, memberName, bindingFlags, includeBaseType) : null;

        /// <summary>
        /// 属性信息对象
        /// </summary>
        public PropertyInfo propertyInfo => bindMemberInfoType == EBindMemberInfoType.Property ? PropertyInfoCache.Get(mainType, memberName, bindingFlags, includeBaseType) : null;

        /// <summary>
        /// 属性信息对象
        /// </summary>
        public MethodInfo methodInfo => bindMemberInfoType == EBindMemberInfoType.Method ? MethodInfoCache.Get(mainType, memberName, bindingFlags, includeBaseType) : null;

        #region 针对字段与属性的缓存机制

        /// <summary>
        /// 判断类型是否时可被处理的
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CanHandled(Type type) => Argument.CanHandle(type);

        /// <summary>
        /// 获取所有具有字段或属性的类型全名称字符串；类型全名称的命名空间层级以'/'间隔；
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static string[] GetTypeFullNames(EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType) => TypeFullNameCache.GetTypeFullNames(bindMemberInfoType, bindingFlags, includeBaseType);

        /// <summary>
        /// 获取字段或属性名称列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetMemberNames(Type type, EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType) => MemberNameCache.GetMemberNames(type, bindMemberInfoType, bindingFlags, includeBaseType);

        internal class TypeFullNameCache : TIVCache<TypeFullNameCache, EBindMemberInfoType, BindingFlags, bool, TypeFullNameCacheValue>
        {
            public static string[] GetTypeFullNames(EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType)
            {
                return GetCacheValue(bindMemberInfoType, bindingFlags, includeBaseType).typeFullNames;
            }
        }

        internal class TypeFullNameCacheValue : TIVCacheValue<TypeFullNameCacheValue, EBindMemberInfoType, BindingFlags, bool>
        {
            public string[] typeFullNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                var names = new List<string>();
                TypeHelper.GetTypes(type =>
                {
                    if (!type.IsClass || !type.IsPublic) return false;
                    if (type.IsGenericType || type.IsAbstract) return false;
                    if (!MemberNameCache.HasMembers(type, key1, key2, key3)) return false;

                    names.Add(type.FullNameToHierarchyString());
                    return true;
                });
#if UNITY_EDITOR
                //排序
                names.Sort();
#endif
                //转数组
                typeFullNames = names.ToArray();
                return true;
            }
        }

        internal class MemberNameCache : TIVCache<MemberNameCache, Type, EBindMemberInfoType, BindingFlags, bool, MemberNameCacheValue>
        {
            public static string[] GetMemberNames(Type type, EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return Empty<string>.Array;
                return GetCacheValue(type, bindMemberInfoType, bindingFlags, includeBaseType)?.memberNames ?? Empty<string>.Array;
            }

            public static bool HasMembers(Type type, EBindMemberInfoType bindMemberInfoType, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return false;
                return GetCacheValue(type, bindMemberInfoType, bindingFlags, includeBaseType).memberNames.Length > 0;
            }
        }

        /// <summary>
        /// 成员名称缓存；键值依次为：类型、属性或字段或事件或方法、绑定标志、是否包含基类
        /// </summary>
        internal class MemberNameCacheValue : TIVCacheValue<MemberNameCacheValue, Type, EBindMemberInfoType, BindingFlags, bool>
        {
            public string[] memberNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                IEnumerable<string> names = null;
                switch (key2)
                {
                    case EBindMemberInfoType.Field:                        
                        names = FieldInfosCache.Get(key1, key3, key4).Where(member => !ObsoleteAttributeCache.Exist(member) && CanHandled(member.FieldType)).Cast(member => member.Name);
                        break;
                    case EBindMemberInfoType.Property:
                        names = PropertyInfosCache.Get(key1, key3, key4).Where(member => !ObsoleteAttributeCache.Exist(member) && CanHandled(member.PropertyType)).Cast(member => member.Name);
                        break;
                    case EBindMemberInfoType.Method:
                        names = MethodInfosCache.Get(key1, key3, key4).Where(member => !ObsoleteAttributeCache.Exist(member) && member.GetParameters().Length == 0).Cast(member => member.Name);
                        break;
                    default:
                        break;
                }

                //去重
                names = names.Distinct();

#if UNITY_EDITOR
                //排序
                names = names.OrderBy(n => n);
#endif

                //转数组
                memberNames = names.ToArray();

                return true;
            }
        }

        #endregion

        #region IDropdownPopupAttribute

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public override bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = FieldPropertyMethodBinder.GetMemberNames(targetType, bindMemberInfoType, bindingFlags, includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = FieldPropertyMethodBinder.GetTypeFullNames(bindMemberInfoType, bindingFlags, includeBaseType);
                        return true;
                    }
            }
            return base.TryGetOptions(purpose, propertyPath, out options);
        }

        #endregion
    }

    /// <summary>
    /// 字段属性方法成员绑定器
    /// </summary>
    [Name("字段属性方法成员绑定器")]
    [Serializable]
    public class FieldPropertyMethodMemberBinder : FieldPropertyMethodBinder, IPropertyPathList
    {
        /// <summary>
        /// 属性路径列表
        /// </summary>
        [Name("属性路径列表")]
        public PropertyPathList _propertyPathList = new PropertyPathList();

        #region IDropdownPopupAttribute

        public override bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            if (PropertyPathHelper.TryGetLastArrayElementIndex(propertyPath, out _))
            {
                _propertyPathList.SetInstance(memberType, this);
                return _propertyPathList.TryGetOptions(purpose, propertyPath, out options);
            }
            return base.TryGetOptions(purpose, propertyPath, out options);
        }

        #endregion

        #region 设置

        /// <summary>
        /// 尝试设置属性值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetPropertyValue(object value)
        {
            try
            {
                memberValue = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试设置属性路径值
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TrySetPropertyPathValue(int index, object value)
        {
            if (index < 0)
            {
                return TrySetPropertyValue(value);
            }
            if (index >= _propertyPathList._propertyPaths.Count)
            {
                return false;
            }

            return TryGetPropertyValue(out var instance) && _propertyPathList.TrySetPropertyPathValue(instance, index, value);
        }

        /// <summary>
        /// 尝试设置属性路径值
        /// </summary>
        /// <param name="value" ></param>
        /// <returns></returns>
        public bool TrySetPropertyPathValue(object value) => TrySetPropertyPathValue(_propertyPathList._propertyPaths.Count - 1, value);

        #endregion

        #region 获取

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="index" ></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(int index, out object value)
        {
            if (index < 0)
            {
                return TryGetPropertyValue(out value);
            }
            if (index >= _propertyPathList._propertyPaths.Count)
            {
                value = default;
                return false;
            }

            if (TryGetPropertyValue(out var instance) && _propertyPathList.TryGetPropertyPathValue(instance, index, out value))
            {
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值
        /// </summary>
        /// <param name="value" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValue(out object value) => TryGetPropertyPathValue(_propertyPathList._propertyPaths.Count - 1, out value);

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="index" ></param>
        /// <param name="type" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValueType(int index, out Type type)
        {
            if (index < 0)
            {
                return TryGetPropertyValueType(out type);
            }
            if (index >= _propertyPathList._propertyPaths.Count)
            {
                type = default;
                return false;
            }

            if (TryGetPropertyValueType(out var instance) && _propertyPathList.TryGetTypeMemberCacheValue(instance, index, out var value))
            {
                type = value.memberValueType;
                return true;
            }
            type = default;
            return false;
        }

        /// <summary>
        /// 尝试获取属性路径值类型
        /// </summary>
        /// <param name="type" ></param>
        /// <returns></returns>
        public bool TryGetPropertyPathValueType(out Type type) => TryGetPropertyPathValueType(_propertyPathList._propertyPaths.Count - 1, out type);

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="value">值</param>
        public bool TryGetPropertyValue(out object value)
        {
            try
            {
                value = memberValue;
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// 尝试获取属性值类型
        /// </summary>
        /// <param name="type">类型</param>
        public bool TryGetPropertyValueType(out Type type)
        {
            type = memberType;
            return type != null;
        }

        #endregion
    }
}
