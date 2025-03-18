using System;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Demos;
using XCSJ.PluginCommonUtils;

namespace XCSJ.EditorExtension.Demos
{
    /// <summary>
    /// 检查器案例检查器
    /// </summary>
    [Name("检查器案例检查器")]
    [CustomEditor(typeof(InspectorDemo))]
    [Serializable]
    public class InspectorDemoInspector : MBInspector<InspectorDemo> { }

    //[CustomPropertyDrawer(typeof(HideInSuperInspectorAttribute))]
    //public class HideInSuperInspectorAttributePropertyDrawer : PropertyDrawer<HideInSuperInspectorAttribute>
    //{
    //    bool hide = false;

    //    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //    {
    //        hide = HideInSuperInspectorAttribute.Hide(property.serializedObject.targetObject, fieldInfo);
    //        if (hide) return 0;
    //        return base.GetPropertyHeight(property, label);
    //    }

    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        if (hide) return;
    //        base.OnGUI(position, property, label);
    //    }
    //}

    //[CustomPropertyDrawer(typeof(FoldoutAttribute))]
    //public class FoldoutAttributePropertyDrawer : PropertyDrawer<FoldoutAttribute>
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        base.OnGUI(position, property, label);
    //    }
    //}

    public class DecoratorDrawer<T> : DecoratorDrawer where T : PropertyAttribute
    {
        public T propertyAttribute => attribute as T;
    }

    //[CustomPropertyDrawer(typeof(GroupAttribute))]
    //public class GroupAttributePropertyDrawer : DecoratorDrawer<GroupAttribute>
    //{
    //    public override void OnGUI(Rect position)
    //    {
    //        var propertyAttribute = this.propertyAttribute;
    //        position.yMin += EditorGUIUtility.singleLineHeight * 0.5f;
    //        position = EditorGUI.IndentedRect(position);
    //        GUI.Label(position, CommonFun.TempContent(MultiLanguageHelper.Tr(propertyAttribute.text), MultiLanguageHelper.Tr(propertyAttribute.tooltip)), EditorStyles.boldLabel);
    //    }

    //    public override float GetHeight() => EditorGUIUtility.singleLineHeight * 1.5f;
    //}

    //[CustomPropertyDrawer(typeof(EndGroupAttribute))]
    //public class EndGroupAttributePropertyDrawer : DecoratorDrawer<EndGroupAttribute>
    //{
    //    public override float GetHeight() => 0;
    //}

    //[CustomPropertyDrawer(typeof(ValidityCheckAttribute))]
    //public class ValidityCheckAttributePropertyDrawer : PropertyDrawer<ValidityCheckAttribute>
    //{
    //    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //    {
    //        base.OnGUI(position, property, label);
    //    }
    //}
}
