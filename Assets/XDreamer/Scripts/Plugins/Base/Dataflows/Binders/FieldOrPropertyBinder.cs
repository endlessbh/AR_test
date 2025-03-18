using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// 字段或属性绑定器
    /// </summary>
    [Name("字段或属性绑定器")]
    [Serializable]
    public sealed class FieldOrPropertyBinder : TypeMemberBinder
    {
        /// <summary>
        /// 绑定类型:标识类型中可绑定成员信息的类型
        /// </summary>
        [Name("绑定类型")]
        [Tip("标识类型中可绑定成员信息的类型", "Identifies the type of member information that can be bound in the type")]
        [EnumPopup]
        public EBindType _bindType = EBindType.PropertyDeclaredOnly;

        public override BindingFlags bindingFlags
        {
            get
            {
                switch (_bindType)
                {
                    case EBindType.FieldDeclaredOnly: 
                    case EBindType.PropertyDeclaredOnly:
                        {
                            return base.bindingFlags | BindingFlags.DeclaredOnly;
                        }
                }
                return base.bindingFlags;
            }
        }

        /// <summary>
        /// 绑定字段
        /// </summary>
        public bool bindField 
        { 
            get => _bindType == EBindType.Field || _bindType == EBindType.FieldDeclaredOnly; 
        }

        /// <summary>
        /// 成员信息对象
        /// </summary>
        public override MemberInfo memberInfo { get => bindField ? (MemberInfo)fieldInfo : propertyInfo; }

        /// <summary>
        /// 字段信息对象
        /// </summary>
        public FieldInfo fieldInfo => bindField ? FieldInfoCache.Get(targetType, memberName, bindingFlags, includeBaseType) : null;

        /// <summary>
        /// 属性信息对象
        /// </summary>
        public PropertyInfo propertyInfo => bindField ? null : PropertyInfoCache.Get(targetType, memberName, bindingFlags, includeBaseType);

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
            /// 仅声明字段:标识可绑定类型中仅声明字段信息
            /// </summary>
            [Name("仅声明字段")]
            [Tip("标识可绑定类型中仅声明字段信息（不包含基类）", "Identifies that only field information is declared in the bindable type (excluding the base class)")]
            FieldDeclaredOnly,


            /// <summary>
            /// 仅声明属性:标识可绑定类型中仅声明属性信息
            /// </summary>
            [Name("仅声明属性")]
            [Tip("标识可绑定类型中仅声明属性信息（不包含基类）", "Identifies that only attribute information is declared in the bindable type (excluding the base class)")]
            PropertyDeclaredOnly,
        }

        public FieldOrPropertyBinder()
        {
            
        }

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
        public static string[] GetTypeFullNames(bool fieldOrProperty, BindingFlags bindingFlags, bool includeBaseType) => TypeFullNameCache.GetTypeFullNames(fieldOrProperty, bindingFlags, includeBaseType);

        /// <summary>
        /// 获取字段或属性名称列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetMemberNames(Type type, bool fieldOrProperty, BindingFlags bindingFlags, bool includeBaseType) => MemberNameCache.GetMemberNames(type, fieldOrProperty, bindingFlags, includeBaseType);

        internal class TypeFullNameCache : TIVCache<TypeFullNameCache, bool, BindingFlags, bool, TypeFullNameCacheValue>
        {
            public static string[] GetTypeFullNames(bool fieldOrProperty, BindingFlags bindingFlags, bool includeBaseType)
            {
                return GetCacheValue(fieldOrProperty, bindingFlags, includeBaseType).typeFullNames;
            }
        }

        internal class TypeFullNameCacheValue : TIVCacheValue<TypeFullNameCacheValue, bool, BindingFlags, bool>
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

        internal class MemberNameCache : TIVCache<MemberNameCache, Type, bool, BindingFlags, bool, MemberNameCacheValue>
        {
            public static string[] GetMemberNames(Type type, bool fieldOrProperty, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return Empty<string>.Array;
                return GetCacheValue(type, fieldOrProperty, bindingFlags, includeBaseType)?.memberNames ?? Empty<string>.Array;
            }

            public static bool HasMembers(Type type, bool fieldOrProperty, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return false;
                return GetCacheValue(type, fieldOrProperty, bindingFlags, includeBaseType).memberNames.Length > 0;
            }
        }

        /// <summary>
        /// 成员名称缓存；键值依次为：类型、字段或属性、绑定标志、是否包含基类
        /// </summary>
        internal class MemberNameCacheValue : TIVCacheValue<MemberNameCacheValue, Type, bool, BindingFlags, bool>
        {
            public string[] memberNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                var names = key2 ? FieldInfosCache.Get(key1, key3, key4).Where(member => !ObsoleteAttributeCache.Exist(member) && CanHandled(member.FieldType)).Cast(member => member.Name) : PropertyInfosCache.Get(key1, key3, key4).Where(member => !ObsoleteAttributeCache.Exist(member) && CanHandled(member.PropertyType)).Cast(member => member.Name);

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
                        options = FieldOrPropertyBinder.GetMemberNames(targetType, bindField, bindingFlags, includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetTypeFullNames(bindField, bindingFlags, includeBaseType);
                        return true;
                    }
            }
            return base.TryGetOptions(purpose, propertyPath, out options);
        }

        #endregion
    }

    /// <summary>
    /// 字段绑定器
    /// </summary>
    [Name("字段绑定器")]
    [Serializable]
    public abstract class FieldBinder : TypeMemberBinder
    {
        /// <summary>
        /// 成员信息对象
        /// </summary>
        public override MemberInfo memberInfo { get => fieldInfo; }

        /// <summary>
        /// 字段信息对象
        /// </summary>
        public FieldInfo fieldInfo => FieldInfoCache.Get(targetType, memberName, bindingFlags, includeBaseType);
    }
}
