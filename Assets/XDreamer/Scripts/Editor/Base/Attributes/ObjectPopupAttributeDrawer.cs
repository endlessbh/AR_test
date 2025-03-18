using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base.XUnityEditor;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 组件集弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ObjectPopupAttribute))]
    public class ObjectPopupAttributeDrawer : PropertyDrawer<ObjectPopupAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var attr = propertyAttribute;

            var componentCollectionWidth = attr.componentCollectionWidth;
            var componentWidth = attr.componentWidth;
            var rect = new Rect(position.x, position.y, position.width - componentCollectionWidth - componentWidth - PropertyDrawerHelper.SpaceWidth * 2, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = componentCollectionWidth;
            EditorObjectHelper.ComponentCollectionPopup(rect, property);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = componentWidth;
            EditorObjectHelper.ObjectComponentPopup(rect, property);
        }
    }

    /// <summary>
    /// 日期时间特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(DateTimeAttribute))]
    [LanguageTuple("Format", "格式化")]
    [LanguageFileOutput]
    public class DateTimeAttributeDrawer : PropertyDrawer<DateTimeAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var buttonWidth = propertyAttribute.buttonWidth;

            var rect = new Rect(position.x, position.y, position.width - buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            if (GUI.Button(rect, "Format".Tr(typeof(DateTimeAttributeDrawer))))
            {
                if (DateTime.TryParse(property.stringValue, out var dateTime))
                {
                    property.stringValue = dateTime.ToString(propertyAttribute.format);
                }
                else
                {
                    property.stringValue = DateTime.Now.ToString(propertyAttribute.format);
                }
                CommonFun.FocusControl();
            }
        }
    }

    /// <summary>
    /// 组件弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentPopupAttribute))]
    public class ComponentPopupAttributeDrawer : PropertyDrawer<ComponentPopupAttribute>
    {
        private Type _componentType;

        /// <summary>
        /// 组件类型
        /// </summary>
        public Type componentType
        {
            get
            {
                if (_componentType == null)
                {
                    _componentType = propertyAttribute.componentType;
                    if (_componentType == null)
                    {
                        _componentType = TypeHelper.TryGetElementType(fieldInfo.FieldType, out var elementType) ? elementType : fieldInfo.FieldType;
                    }
                    if (_componentType != null && !_componentType.IsInterface && !typeof(Component).IsAssignableFrom(_componentType))
                    {
                        _componentType = null;
                    }
                    if (_componentType == null)
                    {
                        _componentType = typeof(MB);
                    }
                }
                return _componentType;
            }
        }

        private const float AddButtonWidth = 24;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    {
                        var propertyAttribute = this.propertyAttribute;
                        if (Application.isPlaying && !propertyAttribute.displayOnRuntime) break;
                        var componentType = this.componentType;
                        if (propertyAttribute.overrideLabel)
                        {
                            label = fieldInfo.TrLabel();// CommonFun.NameTip(fieldInfo);
                        }

                        var component = property.objectReferenceValue as Component;
                        //if (component)
                        //{
                        //    label.tooltip += string.Format("\n名称路径:" + CommonFun.GameObjectComponentToString(component));
                        //}
                        var popupWidth = propertyAttribute.width;
                        var components = component ? component.GetComponents(componentType) : Empty<Component>.Array;
                        Rect rect = Rect.zero;
                        if (components.Length > 1)
                        {
                            rect = new Rect(position.x, position.y, position.width - 2 * popupWidth - AddButtonWidth - 3 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorObjectHelper.GameObjectComponentPopup(rect, componentType, propertyAttribute.searchFlags, property);

                            //组件选择
                            rect.x = rect.x + rect.width + SpaceWidth;
                            //rect.width = popupWidth;

                            var componentNew = property.objectReferenceValue as Component;
                            if (componentNew != component) components = componentNew ? componentNew.GetComponents(componentType) : Empty<Component>.Array;
                            var index = components.IndexOf(componentNew);
                            var names = components.Cast((i, c) => (i + 1).ToString() + "." + c.GetType().Name).ToArray();
                            EditorGUI.BeginChangeCheck();
                            var newIndex = EditorGUI.Popup(rect, index, names);
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.objectReferenceValue = newIndex >= 0 ? components[newIndex] : default;
                            }
                        }
                        else
                        {
                            rect = new Rect(position.x, position.y, position.width - popupWidth - AddButtonWidth - 2 * SpaceWidth, EditorGUIUtility.singleLineHeight);
                            EditorGUI.ObjectField(rect, property, label);

                            rect.x = rect.x + rect.width + SpaceWidth;
                            rect.width = popupWidth;

                            EditorObjectHelper.GameObjectComponentPopup(rect, componentType, propertyAttribute.searchFlags, property);
                        }

                        rect.x += rect.width;
                        rect.width = AddButtonWidth;

                        // 给宿主游戏对象加组件，并设置当前属性
                        if (GUI.Button(rect, new GUIContent("", EditorIconHelper.GetIconInLib(EIcon.Add))))
                        {
                            var go = CommonFun.GetHostGameObject(property.serializedObject.targetObject);
                            if (go)
                            {
                                var component1 = go.XAddComponent(componentType);
                                if (component1) 
                                {
                                    property.objectReferenceValue = component1;
                                }
                            }
                        }

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 游戏对象弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GameObjectPopupAttribute))]
    public class GameObjectPopupAttributeDrawer : PropertyDrawer<GameObjectPopupAttribute>
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    {
                        //var gameObject = property.objectReferenceValue as GameObject;
                        //if (gameObject)
                        //{
                        //    label.tooltip += string.Format("\n名称路径:" + CommonFun.GameObjectToString(gameObject));
                        //}

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - SpaceWidth, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ObjectField(rect, property, label);
                        //gameObject = property.objectReferenceValue as GameObject;

                        rect.x = rect.x + rect.width + SpaceWidth;
                        rect.width = popupWidth;
                        EditorObjectHelper.GameObjectPopup(rect, propertyAttribute.componentType, propertyAttribute.includeInactive, property);
                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }

    /// <summary>
    /// 组件类型弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentTypePopupAttribute))]
    public class ComponentTypePopupAttributeDrawer : PropertyDrawer<ComponentTypePopupAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }
            var attr = propertyAttribute;

            var buttonWidth = attr.buttonWidth;
            var rect = new Rect(position.x, position.y, position.width - buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            EditorObjectHelper.GameObjectComponentTypePopup(rect, property);
        }
    }

    /// <summary>
    /// 数组元素数据
    /// </summary>
    public class ArrayElementData
    {
        /// <summary>
        /// 属性数据
        /// </summary>
        public PropertyData propertyData;

        /// <summary>
        /// 是数组元素
        /// </summary>
        public bool isArrayElement = false;

        /// <summary>
        /// 以0开始的程序索引
        /// </summary>
        public int index = -1;

        /// <summary>
        /// 以1开始的索引字符串，比<see cref="index"/>显示值大1；
        /// </summary>
        public string indexString => indexContent.text;

        /// <summary>
        /// 索引内容：索引字符串内容
        /// </summary>
        public GUIContent indexContent = new GUIContent();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="property"></param>
        public virtual void Init(SerializedProperty property)
        {
            propertyData = PropertyData.GetPropertyData(property);
            index = propertyData.arrayElementIndex;
            if (index >= 0)
            {
                indexContent.text = (index + 1).ToString();
            }

            isArrayElement = propertyData.isArrayElement;
        }

        /// <summary>
        /// 翻译标签
        /// </summary>
        public GUIContent trLabel = new GUIContent();

        /// <summary>
        /// 更新翻译标签
        /// </summary>
        public virtual void UpdateTrLabel()
        {
            trLabel.text = propertyData.trLabel.text;
            trLabel.tooltip = propertyData.trLabel.tooltip;
        }
    }

    /// <summary>
    /// 数组元素数据缓存
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ArrayElementDataCache<TData> where TData : ArrayElementData, new()
    {
        Dictionary<string, TData> dictionary = new Dictionary<string, TData>();

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="property"></param>
        /// <param name="init"></param>
        /// <returns></returns>
        public TData GetData(SerializedProperty property)
        {
            if (!dictionary.TryGetValue(property.propertyPath, out var data))
            {
                dictionary[property.propertyPath] = data = new TData();
                data.Init(property);
            }
            data.UpdateTrLabel();
            return data;
        }
    }

    /// <summary>
    /// 作为数组元素的属性绘制器
    /// </summary>
    /// <typeparam name="TArrayElementData"></typeparam>
    public abstract class PropertyDrawerAsArrayElement<TArrayElementData> : PropertyDrawer where TArrayElementData : ArrayElementData, new()
    {
        /// <summary>
        /// 缓存
        /// </summary>
        public ArrayElementDataCache<TArrayElementData> cache { get; } = new ArrayElementDataCache<TArrayElementData>();
    }

    /// <summary>
    /// 激活游戏对象信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ActiveGameObjectInfo))]
    public class ActiveGameObjectInfoDrawer : PropertyDrawerAsArrayElement<ActiveGameObjectInfoDrawer.Data>
    {
        public class Data : ArrayElementData
        {
            public SerializedProperty gameObjectSP;
            public SerializedProperty activeSP;

            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                gameObjectSP = property.FindPropertyRelative(nameof(ActiveGameObjectInfo._gameObject));
                activeSP = property.FindPropertyRelative(nameof(ActiveGameObjectInfo._active));
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            label = data.isArrayElement ? data.indexContent : label;

            var labelRect = new Rect(position.x, position.y, 48, position.height);
            label = EditorGUI.BeginProperty(labelRect, label, property);
            EditorGUI.PrefixLabel(labelRect, label);

            var enumValue = (EBool)data.activeSP.intValue;
            var enumValueNew = (EBool)UICommonFun.EnumPopup(new Rect(position.xMax - 72, position.y, 72, position.height), enumValue);
            if (enumValue != enumValueNew)
            {
                data.activeSP.intValue = (int)enumValueNew;
            }
            var obj = data.gameObjectSP.objectReferenceValue;
            var objNew = EditorGUI.ObjectField(new Rect(position.x + 48, position.y, position.width - 120, position.height), data.gameObjectSP.objectReferenceValue, typeof(GameObject), true);
            if (obj != objNew)
            {
                data.gameObjectSP.objectReferenceValue = objNew;
            }

            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 基础激活游戏对象信息列表绘制器
    /// </summary>
    //[CustomPropertyDrawer(typeof(BaseActiveGameObjectInfoList), true)]
    public class BaseActiveGameObjectInfoListDrawer : PropertyDrawerAsArrayElement<BaseActiveGameObjectInfoListDrawer.Data>
    {
        public class Data : ArrayElementData
        {
            public SerializedProperty enumEventSP;
            public SerializedProperty infosSP;

            public FieldInfo enumEventFI;
            public bool isValidEnumType = false;
            public Type validEnumType;

            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                //drawInsertDelete = false;
                enumEventSP = property.FindPropertyRelative("_" + nameof(BaseActiveGameObjectInfoList.enumEvent));
                infosSP = property.FindPropertyRelative("_" + nameof(BaseActiveGameObjectInfoList.infos));

                enumEventFI = ScriptAttributeUtility.GetFieldInfoFromProperty(enumEventSP, out _);

                var enumType = enumEventFI.FieldType;
                if (enumType.IsEnum)
                {
                    isValidEnumType = true;
                    validEnumType = enumType;
                }
            }

            /// <summary>
            /// 更新翻译标签
            /// </summary>
            public override void UpdateTrLabel()
            {
                base.UpdateTrLabel();
                if (isArrayElement && isValidEnumType)
                {
                    var text = enumEventSP.enumDisplayNames[enumEventSP.enumValueIndex].Tr(validEnumType);
                    trLabel.text = string.Format("{0}.{1}", indexString, text);
                    trLabel.tooltip = trLabel.text;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => BaseInspector.DrawMember(property, cache.GetData(property).propertyData);
    }

    /// <summary>
    /// 启用组件信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(EnableComponentInfo))]
    public class EnableComponentInfoDrawer : PropertyDrawerAsArrayElement<EnableComponentInfoDrawer.Data>
    {
        public class Data : ArrayElementData
        {
            public SerializedProperty componentSP;
            public SerializedProperty enableSP;
            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                componentSP = property.FindPropertyRelative(nameof(EnableComponentInfo._component));
                enableSP = property.FindPropertyRelative(nameof(EnableComponentInfo._enable));
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            label = data.isArrayElement ? data.indexContent : label;

            var labelRect = new Rect(position.x, position.y, 48, position.height);
            label = EditorGUI.BeginProperty(labelRect, label, property);
            EditorGUI.PrefixLabel(labelRect, label);

            var enumValue = (EBool)data.enableSP.intValue;
            var enumValueNew = (EBool)UICommonFun.EnumPopup(new Rect(position.xMax - 72, position.y, 72, position.height), enumValue);
            if (enumValue != enumValueNew)
            {
                data.enableSP.intValue = (int)enumValueNew;
            }

            var rect = new Rect(position.x + 48, position.y, position.width - 120, position.height);
            EditorGUI.PropertyField(rect, data.componentSP, GUIContent.none);

            //var obj = data.componentSP.objectReferenceValue;
            //var objNew = EditorGUI.ObjectField(new Rect(position.x + 48, position.y, position.width - 120, position.height), data.componentSP.objectReferenceValue, typeof(Component), true);
            //if (obj != objNew)
            //{
            //    data.componentSP.objectReferenceValue = objNew;
            //}

            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 基础激活游戏对象信息列表绘制器
    /// </summary>
    //[CustomPropertyDrawer(typeof(BaseEnableComponentInfoList), true)]
    public class BaseEnableComponentInfoListDrawer : PropertyDrawerAsArrayElement<BaseEnableComponentInfoListDrawer.Data>
    {
        public class Data : ArrayElementData
        {
            public SerializedProperty enumEventSP;
            public SerializedProperty infosSP;

            public FieldInfo enumEventFI;
            public bool isValidEnumType = false;
            public Type validEnumType;

            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                //drawInsertDelete = false;
                enumEventSP = property.FindPropertyRelative("_" + nameof(BaseActiveGameObjectInfoList.enumEvent));
                infosSP = property.FindPropertyRelative("_" + nameof(BaseActiveGameObjectInfoList.infos));

                enumEventFI = ScriptAttributeUtility.GetFieldInfoFromProperty(enumEventSP, out _);

                var enumType = enumEventFI.FieldType;
                if (enumType.IsEnum)
                {
                    isValidEnumType = true;
                    validEnumType = enumType;
                }
            }

            public override void UpdateTrLabel()
            {
                base.UpdateTrLabel();
                if (isArrayElement && isValidEnumType)
                {
                    var text = enumEventSP.enumDisplayNames[enumEventSP.enumValueIndex].Tr(validEnumType);
                    trLabel.text = string.Format("{0}.{1}", indexString, text);
                    trLabel.tooltip = trLabel.text;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) => BaseInspector.DrawMember(property, cache.GetData(property).propertyData);
    }

    /// <summary>
    /// 启用组件信息绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseRuntimePlatformDetailInfo), true)]
    public class BaseRuntimePlatformDetailInfoDrawer : PropertyDrawerAsArrayElement<BaseRuntimePlatformDetailInfoDrawer.Data>
    {
        public class Data : ArrayElementData
        {
            public SerializedProperty runtimePlatformSP;
            public SerializedProperty valueSP;
            public override void Init(SerializedProperty property)
            {
                base.Init(property);
                runtimePlatformSP = property.FindPropertyRelative(nameof(BaseRuntimePlatformDetailInfo._runtimePlatform));
                valueSP = property.FindPropertyRelative("_value");
            }
        }

        private const float LabelWidth = 48;
        private const float RPWidth = 72 + 90;
        private const float ValueX = LabelWidth + RPWidth;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);
            label = data.isArrayElement ? data.indexContent : label;

            var labelRect = new Rect(position.x, position.y, LabelWidth, position.height);
            label = EditorGUI.BeginProperty(labelRect, label, property);
            EditorGUI.PrefixLabel(labelRect, label);

            EditorGUI.PropertyField(new Rect(position.x + LabelWidth, position.y, RPWidth, position.height), data.runtimePlatformSP, GUIContent.none);
            EditorGUI.PropertyField(new Rect(position.x + ValueX, position.y, position.width - ValueX, position.height), data.valueSP, GUIContent.none);

            EditorGUI.EndProperty();
        }
    }


    /// <summary>
    /// Guid生成器特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(GuidCreaterAttribute))]
    public class GuidCreaterAttributeDrawer : PropertyDrawer<GuidCreaterAttribute>
    {
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var buttonWidth = propertyAttribute.buttonWidth;

            var rect = new Rect(position.x, position.y, position.width - 2 * buttonWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = buttonWidth;
            if (GUI.Button(rect, UICommonOption.Copy, EditorStyles.miniButtonLeft))
            {
                CommonFun.CopyTextToClipboardForPC(property.stringValue);
            }
            rect.x = rect.x + rect.width;
            //rect.width = buttonWidth;
            if (GUI.Button(rect, UICommonOption.Reset, EditorStyles.miniButtonRight))
            {
                property.stringValue = GuidHelper.GetNewGuid();
            }
        }
    }
}
