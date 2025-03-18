using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Caches;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.EditorExtension.Base.Dataflows.Base
{
    /// <summary>
    /// 基础属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BasePropertyValue), true)]
    public class PropertyValueDrawer : PropertyDrawer
    {
        internal const float PropertyValueTypeWidth = 80;

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;

            //绘制标签
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //绘制属性值类型选择
            var position1 = new Rect(position.x, position.y, PropertyValueTypeWidth, position.height);
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            //绘制属性值
            var position2 = new Rect(position.x + PropertyValueTypeWidth, position.y, position.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));
                        EditorGUI.PropertyField(position2, valueSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.VarString:
                    {
                        var varStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._varString));
                        EditorGUI.PropertyField(position2, varStringSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.ExpressionString:
                    {
                        var expressionStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._expressionString));
                        EditorGUI.PropertyField(position2, expressionStringSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 字符串属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(StringPropertyValue_TextArea), true)]
    public class StringPropertyValue_TextAreaDrawer : PropertyValueDrawer
    {
        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));
            switch ((EPropertyValueType)propertyValueTypeSP.intValue)
            {
                case EPropertyValueType.Value:
                    {
                        return base.GetPropertyHeight(property, label) * 3;
                    }
            }
            return base.GetPropertyHeight(property, label);
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position.x, position.y, PropertyValueTypeWidth, position.height);
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position.x + PropertyValueTypeWidth, position.y, position.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));
                        valueSP.stringValue = EditorGUI.TextArea(position2, valueSP.stringValue);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.VarString:
                    {
                        var varStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._varString));
                        EditorGUI.PropertyField(position2, varStringSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.ExpressionString:
                    {
                        var expressionStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._expressionString));
                        EditorGUI.PropertyField(position2, expressionStringSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 自定义函数属性值绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomFunctionPropertyValue), true)]
    public class CustomFunctionPropertyValueDrawer : PropertyValueDrawer
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);
            EditorGUI.BeginProperty(position, label, property);

            var propertyValueTypeSP = property.FindPropertyRelative(nameof(BasePropertyValue._propertyValueType));

            // Draw label
            var position0 = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var position1 = new Rect(position0.x, position0.y, PropertyValueTypeWidth, position.height);
            var propertyValueType = (EPropertyValueType)propertyValueTypeSP.intValue;
            var newPropertyValueType = (EPropertyValueType)UICommonFun.EnumPopup(position1, propertyValueType);
            if (propertyValueType != newPropertyValueType)
            {
                propertyValueTypeSP.intValue = (int)newPropertyValueType;
            }

            var position2 = new Rect(position0.x + PropertyValueTypeWidth, position0.y, position0.width - PropertyValueTypeWidth, position.height);
            switch (newPropertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        var valueSP = property.FindPropertyRelative(PropertyValueFieldNameAttribute.GetFieldName(fieldInfo.FieldType));
                        EditorGUILayout.PropertyField(valueSP, GUIContent.none, true);
                        break;
                    }
                case EPropertyValueType.Variable:
                    {
                        var variableNameSP = property.FindPropertyRelative(nameof(BasePropertyValue._variableName));
                        EditorGUI.PropertyField(position2, variableNameSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.VarString:
                    {
                        var varStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._varString));
                        EditorGUI.PropertyField(position2, varStringSP, GUIContent.none);
                        break;
                    }
                case EPropertyValueType.ExpressionString:
                    {
                        var expressionStringSP = property.FindPropertyRelative(nameof(BasePropertyValue._expressionString));
                        EditorGUI.PropertyField(position2, expressionStringSP, GUIContent.none);
                        break;
                    }
            }
            EditorGUI.EndProperty();
        }
    }

    /// <summary>
    /// 属性路径列表绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(PropertyPathList))]
    [LanguageFileOutput]
    public class PropertyPathListDrawer : PropertyDrawer
    {
        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        Dictionary<int, (GUIContent, Type)> dictionary = new Dictionary<int, (GUIContent, Type)>();

        private (GUIContent, Type) GetDisplayType(IPropertyPathList propertyPathList, int index)
        {
            if (dictionary.TryGetValue(index, out var displayType)) return displayType;

            displayType = propertyPathList != null && propertyPathList.TryGetPropertyPathValueType(index, out var type) ? (type.TrLabel(), type) : (new GUIContent(), default);
            dictionary[index] = displayType;
            return displayType;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        [LanguageTuple("Member Name", "成员名称")]
        [LanguageTuple("Value Type", "值类型")]
        [LanguageTuple("Operation", "操作")]
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyPathsSP = property.FindPropertyRelative(nameof(PropertyPathList._propertyPaths));
            propertyPathsSP.isExpanded = UICommonFun.Foldout(propertyPathsSP.isExpanded, label, () =>
            {
                if (GUILayout.Button(UICommonOption.Run, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                {
                    CommonFun.FocusControl();
                    var propertyPathList0 = property.ConvertValueTo<IPropertyPathList>();
                    if (propertyPathList0.TryGetPropertyValue(out var value))
                    {
                        propertyPathList0.TryGetPropertyValueType(out var type);
                        Debug.LogFormat("属性路径列表的实例值为[{0}],类型[{1}]",
                           value.ToScriptParamString(),
                           type?.FullName ?? "");
                    }
                    else
                    {
                        Debug.LogFormat("属性路径列表的实例值计算失败");
                    }
                }
                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                {
                    CommonFun.FocusControl();
                    propertyPathsSP.arraySize++;
                }
                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    CommonFun.FocusControl();
                    if (propertyPathsSP.arraySize > 0) propertyPathsSP.arraySize--;
                }
            });
            if (!propertyPathsSP.isExpanded) return;

            var propertyPathList = property.ConvertValueTo<IPropertyPathList>();
            CommonFun.BeginLayout();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("NO.", UICommonOption.Width32);
            EditorGUILayout.LabelField("Type".Tr(), UICommonOption.Width120);
            EditorGUILayout.LabelField("Member Name".Tr(typeof(PropertyPathListDrawer)));
            EditorGUILayout.LabelField("Value Type".Tr(typeof(PropertyPathListDrawer)), UICommonOption.Width120);
            EditorGUILayout.LabelField("Operation".Tr(typeof(PropertyPathListDrawer)), UICommonOption.WH72x16);
            EditorGUILayout.EndHorizontal();

            dictionary.Clear();
            var bg = GUI.backgroundColor;

            for (int i = 0; i < propertyPathsSP.arraySize; i++)
            {
                var typeMemberSP = propertyPathsSP.GetArrayElementAtIndex(i);
                var memberNameSP = typeMemberSP.FindPropertyRelative(nameof(TypeMember._memberName));

                var mainType = GetDisplayType(propertyPathList, i - 1);
                var mainTypeLabel = mainType.Item1;
                var valueType = GetDisplayType(propertyPathList, i);
                var valueTypeLabel = valueType.Item1;

                GUI.backgroundColor = (string.IsNullOrEmpty(mainTypeLabel.text) || string.IsNullOrEmpty(valueTypeLabel.text)) ? Color.red : bg;

                UICommonFun.BeginHorizontal(i);
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                EditorGUILayout.LabelField(mainTypeLabel, UICommonOption.Width120);

                EditorGUILayout.PropertyField(memberNameSP, GUIContent.none);

                EditorGUILayout.LabelField(valueTypeLabel, UICommonOption.Width120);

                if (GUILayout.Button(UICommonOption.Run, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                {
                    CommonFun.FocusControl();
                    if (propertyPathList.TryGetPropertyPathValue(i, out var value))
                    {
                        Debug.LogFormat("类型[{0}]中成员[{1}]（类型: {2}）值为[{3}]",
                            mainTypeLabel.text,
                            memberNameSP.stringValue,
                            valueTypeLabel.text,
                            value.ToScriptParamString());
                    }
                    else
                    {
                        var error = "异常错误！";
                        if (mainType.Item2 == typeof(void))
                        {
                            error = "空返回值类型无法计算成员信息！";
                        }
                        Debug.LogWarningFormat("类型[{0}]中成员[{1}]（类型: {2}）值计算失败: {3}",
                            mainTypeLabel.text,
                            memberNameSP.stringValue,
                            valueTypeLabel.text,
                            error);
                    }
                }

                if (GUILayout.Button(UICommonOption.Insert, EditorStyles.miniButtonMid, UICommonOption.WH24x16))
                {
                    CommonFun.FocusControl();
                    typeMemberSP.InsertArrayElementAtThisIndex();
                }

                if (GUILayout.Button(UICommonOption.Delete, EditorStyles.miniButtonRight, UICommonOption.WH24x16))
                {
                    CommonFun.FocusControl();
                    typeMemberSP.DeleteArrayElementCommand();
                }

                UICommonFun.EndHorizontal();
            }
            GUI.backgroundColor = bg;
            CommonFun.EndLayout();
        }
    }
}
