using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.Tools;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.DataViews;
using XCSJ.PluginXGUI.DataViews.Base;
using XCSJ.PluginXGUI.DataViews.GroupViews;
using XCSJ.PluginXGUI.Windows;

namespace XCSJ.EditorXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 数据视图集合检查器
    /// </summary>
    [CustomEditor(typeof(GroupView), true)]
    public class GroupViewInspector : GroupViewInspector<GroupView> { }

    /// <summary>
    /// 数据视图集合检查器
    /// </summary>
    public class GroupViewInspector<T> : MBInspector<T> where T : GroupView
    {
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(BaseModelView._fieldPropertyMethodBinder): return;
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        [Languages.LanguageTuple("Create", "创建")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(Tr("Create")))
            {
                var modelTarget = targetObject.modelTarget;
                if (modelTarget)
                {
                    var parent = targetObject._parent ? targetObject._parent : targetObject.transform;
                    var canvas = EditorXGUIHelper.CreateCanvas();
                    canvas.XSetParent(parent);
                    var window = XCSJ.EditorXGUI.ToolsMenu.LoadPrefab_DefaultXGUIPath("窗口模版.prefab");
                    window.XSetParent(canvas);
                    var content = window.GetComponent<UGUIWindow>().content;

                    DataViewHelper.CreateMemberDataViews(modelTarget, targetObject._viewBindDataTypeMemberMode, targetObject._includeBaseType, content.transform);
                }
            }
        }
    }


    /// <summary>
    /// 数据视图助手：通过传入一个类型找到对应的数据视图将其可视化
    /// </summary>
    [LanguageFileOutput]
    public static class EditorDataViewHelper
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
            Type tmpType = typeof(Enum);
            if (tmpType.FullName != dataType.FullName && tmpType.IsAssignableFrom(dataType))
            {
                newDataType = tmpType;
                return true;
            }

            tmpType = typeof(Component);
            if (tmpType.FullName != dataType.FullName && tmpType.IsAssignableFrom(dataType))
            {
                newDataType = tmpType;
                return true;
            }

            tmpType = typeof(MethodInfo);
            if (tmpType.FullName != dataType.FullName && tmpType.IsAssignableFrom(dataType))
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
#if UNITY_EDITOR
                    if (go) Undo.RegisterCreatedObjectUndo(go, go.name);
#endif
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
            catch (Exception ex)
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
                    var dataView = CreateDataView(fieldInfo.FieldType, target, bindType, fieldInfo.Name, transform);
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
                    var dataView = CreateDataView(propertyInfo.PropertyType, target, bindType, propertyInfo.Name, transform);
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
                    CreateDataView(methodInfo.GetType(), target, bindType, methodInfo.Name, transform);
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
    }

}