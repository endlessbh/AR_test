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

namespace XCSJ.EditorExtension.Base.Dataflows.Base
{
    /// <summary>
    /// 实参绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(Argument), true)]
    public class ArgumentDrawer : PropertyDrawer
    {
        internal const float ArgumentTypeWidth = 80;

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            //绘制实参类型选择
            var argumentTypeSP = property.FindPropertyRelative(nameof(Argument._argumentType));
            var argumentType = (EArgumentType)argumentTypeSP.intValue;

            var relativePropertyPath = ArgumentValueFieldNameAttribute.GetArgumentValueFieldName(argumentType);
            if (!string.IsNullOrEmpty(relativePropertyPath))
            {
                var valueSP = property.FindPropertyRelative(relativePropertyPath);
                return EditorGUI.GetPropertyHeight(valueSP, GUIContent.none, true) + 2;
            }

            return base.GetPropertyHeight(property, label) + 2;
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            //绘制标签
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //绘制实参类型选择
            var argumentTypeSP = property.FindPropertyRelative(nameof(Argument._argumentType));
            var position1 = new Rect(position.x, position.y, ArgumentTypeWidth, position.height);
            EditorGUI.PropertyField(position1, argumentTypeSP, GUIContent.none);
            var argumentType = (EArgumentType)argumentTypeSP.intValue;

            //属性值信息
            var position2 = new Rect(position.x + ArgumentTypeWidth, position.y, position.width - ArgumentTypeWidth, position.height);
            var relativePropertyPath = ArgumentValueFieldNameAttribute.GetArgumentValueFieldName(argumentType);

            //Unity对象特殊处理
            if (argumentType == EArgumentType.UnityObject)
            {
                var position3 = new Rect(position2.x, position2.y, ArgumentTypeWidth, position.height);
                position2.xMin += ArgumentTypeWidth;
                var unityObjectTypeSP = property.FindPropertyRelative(nameof(Argument._unityObjectType));
                EditorGUI.PropertyField(position3, unityObjectTypeSP, GUIContent.none);
            }

            //绘制属性值
            if (!string.IsNullOrEmpty(relativePropertyPath))
            {
                var valueSP = property.FindPropertyRelative(relativePropertyPath);
                EditorGUI.PropertyField(position2, valueSP, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }
    }
}
