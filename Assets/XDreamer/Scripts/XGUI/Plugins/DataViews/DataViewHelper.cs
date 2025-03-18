using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.Algorithms;

namespace XCSJ.PluginXGUI.DataViews
{
    /// <summary>
    /// 数据视图助手：通过传入一个类型找到对应的数据视图将其可视化
    /// </summary>
    [LanguageFileOutput]
    public static class DataViewHelper
    {
        #region 查找数据视图类型(通过数据)

        /// <summary>
        /// 获取数据视图类型
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="dataViewType">数据视图类型</param>
        /// <returns></returns>
        public static bool TryGetDataViewType<T>(out Type dataViewType) => TryGetDataViewType(typeof(T), out dataViewType);

        /// <summary>
        /// 获取数据视图类型
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="dataViewType">数据视图类型</param>
        /// <returns></returns>
        public static bool TryGetDataViewType(object data, out Type dataViewType) => TryGetDataViewType(data.GetType(), out dataViewType);

        /// <summary>
        /// 获取数据视图类型
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="dataViewType">数据视图类型</param>
        /// <returns></returns>
        public static bool TryGetDataViewType(Type dataType, out Type dataViewType)
        {
            dataViewType = DataViewAttribute.GetDataViewType(dataType);
            // 当设定对象为空时，尝试使用其基类进行初始化操作
            if (dataViewType == null)
            {
                if (TryGetIsAssignableFromDataType(dataType, out var newDataType))
                {
                    TryGetDataViewType(newDataType, out dataViewType);
                }
            }
            return dataViewType != null;
        }

        private static bool TryGetIsAssignableFromDataType(Type dataType, out Type newDataType)
        {
            if (dataType.IsEnum)
            {
                newDataType = typeof(Enum);
                return true;
            }

            var tmpType = typeof(Component);
            if (tmpType.IsAssignableFrom(dataType))
            {
                newDataType = tmpType;
                return true;
            }

            tmpType = typeof(MethodInfo);
            if (tmpType.IsAssignableFrom(dataType))
            {
                newDataType = tmpType;
                return true;
            }

            newDataType = default;
            return false; 
        }

        #endregion

        #region 查找数据类型(通过数据视图)

        /// <summary>
        /// 查找数据类型
        /// </summary>
        /// <typeparam name="T">数据视图类型</typeparam>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public static bool TryGetDataType<T>(out Type dataType) => TryGetDataType(typeof(T), out dataType);

        /// <summary>
        /// 查找数据类型
        /// </summary>
        /// <param name="dataView">数据视图</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public static bool TryGetDataType(BaseModelView dataView, out Type dataType)
        {
            if (dataView)
            {
                return TryGetDataType(dataView.GetType(), out dataType);
            }
            else
            {
                dataType = default;
                return false;
            }
        }

        /// <summary>
        /// 查找数据类型
        /// </summary>
        /// <param name="dataViewType">数据视图类型</param>
        /// <param name="dataType">数据视图</param>
        /// <returns></returns>
        public static bool TryGetDataType(Type dataViewType, out Type dataType)
        {
            if (TypeHelper.TryGetAttributes<DataViewAttribute>(dataViewType, out var attrs))
            {
                dataType = attrs[0].type;
                return true;
            }
            dataType = default;
            return false;
        }

        #endregion

        #region 数据视图模版

        /// <summary>
        /// 数据视图模版:参数1=数据类型，参数2=数据视图对象
        /// </summary>
        private static Dictionary<Type, BaseModelView> dataViewTemplates = new Dictionary<Type, BaseModelView>();

        public static void AddDataViewTemplate(BaseModelView dataView)
        {
            if (TryGetDataType(dataView, out var dataType))
            {
                if (!dataViewTemplates.ContainsKey(dataType))
                {
                    dataViewTemplates.Add(dataType, dataView);
                }
            }
        }

        /// <summary>
        /// 添加视图模版
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="dataView"></param>
        public static void AddDataViewTemplate(Type dataType, BaseModelView dataView)
        {
            if (dataType == null || !dataView) return;

            dataViewTemplates.Add(dataType, dataView);
        }

        public static void RemoveDataViewTemplate(BaseModelView dataView)
        {
            if (TryGetDataType(dataView, out var dataType))
            {
                RemoveDataViewTemplate(dataType);
            }
        }

        public static void RemoveDataViewTemplate(Type dataViewLinkType)
        {
            dataViewTemplates.Remove(dataViewLinkType);
        }

        /// <summary>
        /// 使用原始数据类型在模版中查找关联的游戏对象，如果查出失败，则使用原始数据类型的可赋值类型进行查找
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private static GameObject CreateDataViewFromTemplates(Type dataType)
        {
            if (!dataViewTemplates.TryGetValue(dataType, out var tmplate))
            {
                if (TryGetIsAssignableFromDataType(dataType, out var newDataType))
                {
                    return CreateDataViewFromTemplates(newDataType);
                }
            }

            if (tmplate)
            {
                return tmplate.gameObject.XCloneObject();
            }
            return null;
        }

        #endregion

        #region 创建对象数据视图

        /// <summary>
        /// 创建数据视图（通过数据）
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static BaseModelView CreateDataView(object data) => CreateDataView(data.GetType());

        /// <summary>
        /// 创建数据视图（通过数据类型）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static BaseModelView CreateDataView<T>() => CreateDataView(typeof(T));

        /// <summary>
        /// 创建数据视图（通过数据类型）:有模版则从模板克隆，无的话则创建
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        public static BaseModelView CreateDataView(Type dataType)
        {
            BaseModelView dataView = null;

            if (dataType != null)
            {
                GameObject go = CreateDataViewFromTemplates(dataType);

                // 使用模版资源创建失败则尝试使用默认Unity组件方式创建游戏对象
                if (!go && TryGetDataViewType(dataType, out var dataViewType))
                {
                    go = DefaultControls.factory.CreateGameObject(dataViewType.Name, typeof(RectTransform), typeof(LayoutElement), dataViewType);
                    if (go) 
                    {
                        UnityObjectHelper.RegisterCreatedObjectUndo(go);
                    }
                }

                if (go)
                {
                    go.SetActive(true);
                    go.TryGetComponent<BaseModelView>(out dataView);
                }
            }

            return dataView;
        }

        /// <summary>
        /// 创建数据视图
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="target">数据主对象</param>
        /// <param name="bindType">绑定类型：字段、属性和方法</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="parent">父级</param>
        /// <param name="title">标题(别名)</param>
        /// <returns></returns>
        public static BaseModelView CreateDataView<T>(UnityEngine.Object target, EBindType bindType, string memberName, Transform parent = null, string title = "")
        {
            return CreateDataView(typeof(T), target, bindType, memberName, parent, title);
        }

        /// <summary>
        /// 创建数据视图
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="target">数据主对象</param>
        /// <param name="bindType">绑定类型：字段、属性和方法</param>
        /// <param name="memberName">成员名称</param>
        /// <param name="parent">父级</param>
        /// <param name="title">标题(别名)</param>
        /// <returns></returns>
        public static BaseModelView CreateDataView(Type dataType, UnityEngine.Object target, EBindType bindType, string memberName, Transform parent = null, string title = "")
        {
            try
            {
                var dataView = CreateDataView(dataType);
                if (dataView)
                {
                    dataView.XSetName(memberName);
                    dataView.BindModel(target, bindType, memberName);
                    if (!string.IsNullOrEmpty(title)) dataView.SetLabelText(title);
                    dataView.transform.XSetTransformParent(parent);
                }
                return dataView;
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }

        #endregion

        #region 创建对象成员的数据视图

        /// <summary>
        /// 创建对象成员的数据视图
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="viewBindDataTypeMemberMode">绑定类型</param>
        /// <param name="includeBaseType">是否包含基类</param>
        /// <param name="parent">父级</param>
        /// <param name="createInsideMember">进入成员内部创建数据视图</param>
        public static void CreateMemberDataViews(UnityEngine.Object target, EViewBindDataTypeMemberMode viewBindDataTypeMemberMode, bool includeBaseType, Transform parent, bool createInsideMember = false)
        {


            // 创建字段视图
            if ((viewBindDataTypeMemberMode & EViewBindDataTypeMemberMode.Field) == EViewBindDataTypeMemberMode.Field)
            {
                CreateFieldDataViews(target, viewBindDataTypeMemberMode, includeBaseType, parent, createInsideMember);
            }

            // 创建属性视图
            if ((viewBindDataTypeMemberMode & EViewBindDataTypeMemberMode.Property) == EViewBindDataTypeMemberMode.Property)
            {
                CreatePropertyDataViews(target, viewBindDataTypeMemberMode, includeBaseType, parent, createInsideMember);
            }

            // 创建方法视图
            if ((viewBindDataTypeMemberMode & EViewBindDataTypeMemberMode.Method) == EViewBindDataTypeMemberMode.Method)
            {
                CreateMethodDataViews(target, includeBaseType, parent);
            }
        }

        /// <summary>
        /// 创建字段对应的数据视图
        /// </summary>
        /// <param name="target"></param>
        /// <param name="viewBindDataTypeMemberMode"></param>
        /// <param name="includeBaseType"></param>
        /// <param name="transform"></param>
        /// <param name="createInsideMember"></param>
        private static void CreateFieldDataViews(UnityEngine.Object target, EViewBindDataTypeMemberMode viewBindDataTypeMemberMode, bool includeBaseType, Transform transform, bool createInsideMember = false)
        {
            var targetType = target.GetType();
            var bindType = includeBaseType ? EBindType.Field : EBindType.FieldDeclaredOnly;
            foreach (FieldInfo fieldInfo in TypeHelper.GetFieldInfos(targetType, GetBindingFlags(includeBaseType), includeBaseType))
            {
                if (CanCreateDataView(fieldInfo))
                {
                    var dataView = DataViewHelper.CreateDataView(fieldInfo.FieldType, target, bindType, fieldInfo.Name, transform);
                    if (dataView && createInsideMember)
                    {
                        var unityObject = fieldInfo.GetValue(target) as UnityEngine.Object;
                        if (unityObject)
                        {
                            CreateMemberDataViews(unityObject, viewBindDataTypeMemberMode, includeBaseType, dataView.transform, createInsideMember);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建属性对应的数据视图
        /// </summary>
        /// <param name="target"></param>
        /// <param name="viewBindDataTypeMemberMode"></param>
        /// <param name="includeBaseType"></param>
        /// <param name="transform"></param>
        /// <param name="createInsideMember"></param>
        private static void CreatePropertyDataViews(UnityEngine.Object target, EViewBindDataTypeMemberMode viewBindDataTypeMemberMode, bool includeBaseType, Transform transform, bool createInsideMember = false)
        {
            var targetType = target.GetType();
            var bindType = includeBaseType ? EBindType.Property : EBindType.PropertyDeclaredOnly;
            foreach (PropertyInfo propertyInfo in TypeHelper.GetPropertyInfos(targetType, GetBindingFlags(includeBaseType), includeBaseType))
            {
                if (CanCreateDataView(propertyInfo))
                {
                    var dataView = DataViewHelper.CreateDataView(propertyInfo.PropertyType, target, bindType, propertyInfo.Name, transform);
                    if (dataView && createInsideMember)
                    {
                        var unityObject = propertyInfo.GetValue(target) as UnityEngine.Object;
                        if (unityObject)
                        {
                            CreateMemberDataViews(unityObject, viewBindDataTypeMemberMode, includeBaseType, dataView.transform, createInsideMember);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建方法数据视图
        /// </summary>
        /// <param name="target"></param>
        /// <param name="includeBaseType"></param>
        /// <param name="transform"></param>
        private static void CreateMethodDataViews(UnityEngine.Object target, bool includeBaseType, Transform transform)
        {
            var targetType = target.GetType();
            var bindType = includeBaseType ? EBindType.Method : EBindType.MethodDeclaredOnly;
            foreach (MethodInfo methodInfo in targetType.GetMethods(GetBindingFlags(includeBaseType) | BindingFlags.InvokeMethod))
            {
                // 排除属性的get和set方法
                if (methodInfo.IsSpecialName) continue;

                // 排除有参数的方法
                var parameters = methodInfo.GetParameters();
                if (parameters.Length > 0) continue;

                if (CanCreateDataView(methodInfo))
                {
                    DataViewHelper.CreateDataView(methodInfo.GetType(), target, bindType, methodInfo.Name, transform);
                }
            }
        }

        /// <summary>
        /// 能否创建数据视图
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="defaultEnable"></param>
        /// <returns></returns>
        private static bool CanCreateDataView(MemberInfo memberInfo, bool defaultEnable = true)
        {
            var att = memberInfo.GetCustomAttribute<DataViewEnableAttribute>(false);
            return att != null ? att.enable : defaultEnable;
        }

        /// <summary>
        /// 获取绑定标志量:默认为实例、公有对象
        /// </summary>
        /// <param name="includeBaseType"></param>
        /// <returns></returns>
        private static BindingFlags GetBindingFlags(bool includeBaseType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public;
            if (!includeBaseType)
            {
                flags = flags | BindingFlags.DeclaredOnly;
            }
            return flags;
        }

        #endregion

        public class ConverterCacheValue : TIVCacheValue<ConverterCacheValue, Type, Type, Type>
        {
            public bool canInputToOutput { get; private set; } = false;

            public bool canOutputToInput { get; private set; } = false;

            public bool canInputToOutputOrOutputToInput { get; private set; } = false;

            public bool canInputToOutputAndOutputToInput { get; private set; } = false;

            public override bool Init()
            {
                canInputToOutput = Match(key1, key2, key3);
                canOutputToInput = Match(key2, key1, key3);
                canInputToOutputOrOutputToInput = canInputToOutput || canOutputToInput;
                canInputToOutputAndOutputToInput = canInputToOutput && canOutputToInput;

                return true;
            }
        }

        public class ConverterCache : TIVCache<ConverterCache, Type, Type, Type, ConverterCacheValue> 
        { 
            private static ConverterCacheValue defaultValue = new ConverterCacheValue();

            public static ConverterCacheValue Get(Type inputType, Type outputType, Type converterType)
            {
                return GetCacheValue(inputType, outputType, converterType, defaultValue);
            }
        }

        public static bool Match(Type inputType, Type outputType, Type converterType)
        {
            var tmpOutputType = outputType;
            while (inputType != null)
            {
                outputType = tmpOutputType;

                while (outputType != null)
                {
                    var converterInterfaceType = typeof(IConverter<,>).MakeGenericType(inputType, outputType);

                    if (converterInterfaceType.IsAssignableFrom(converterType))
                    {
                        return true;
                    }
                    outputType = outputType.BaseType;
                }

                inputType = inputType.BaseType;
            }
            return false;
        }

    }

    /// <summary>
    /// 视图绑定数据类型成员模式:标识数据对象的类型中可绑定成员信息的类型
    /// </summary>
    [Name("视图绑定数据类型成员模式")]
    [Tip("标识类型中可绑定成员信息的类型", "Identifies the type of member information that can be bound in the type")]
    [Flags]
    public enum EViewBindDataTypeMemberMode
    {
        /// <summary>
        /// 字段:标识可绑定类型中字段信息
        /// </summary>
        [Name("字段")]
        [Tip("标识可绑定类型中字段信息", "Identifies the field information in the bindable type")]
        Field = 1 << 0,

        /// <summary>
        /// 属性:标识可绑定类型中属性信息
        /// </summary>
        [Name("属性")]
        [Tip("标识可绑定类型中属性信息", "Identifies the attribute information in the bindable type")]
        Property = 1 << 1,

        /// <summary>
        /// 方法:标识可绑定类型中方法信息（仅使用空参数方法）
        /// </summary>
        [Name("方法")]
        [Tip("标识可绑定类型中方法信息（仅使用空参数方法）", "Identifies the method information in the bindable type (only empty parameter methods are used)")]
        Method = 1 << 2,
    }
}
