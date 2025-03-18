using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils;
using static XCSJ.Extension.Base.Dataflows.Binders.TypeBinder;

namespace XCSJ.EditorExtension.Base.Dataflows.Binders
{
    /// <summary>
    /// 类型绑定器绘制器
    /// </summary>
    //[CustomPropertyDrawer(typeof(TypeBinder), true)]
    public class TypeBinderDrawer : PropertyDrawer
    {
    }

    /// <summary>
    /// 类型成员绑定器绘制器
    /// </summary>
    //[CustomPropertyDrawer(typeof(TypeMemberBinder), true)]
    public class TypeMemberBinderDrawer : PropertyDrawer
    {
    }


    /// <summary>
    /// 字段属性方法绑定器绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(FieldPropertyMethodBinder), true)]
    public class FieldPropertyMethodBinderDrawer : TypeMemberBinderDrawer
    {
        int i = 0;

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (i > 1)
            {
                return (base.GetPropertyHeight(property, label) + 4) * i + 6;
            }
            else
            {
                return base.GetPropertyHeight(property, label) + 6;
            }
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var propertyData = PropertyData.GetPropertyData(property);

            var height = EditorGUIUtility.singleLineHeight + 2;

            var rect = new Rect(position.x, position.y, position.width, height + 2);
            EditorGUI.LabelField(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            rect.xMin += 16;
            EditorGUI.PropertyField(rect, property, propertyData.GetTrLabel(), false);
            i = 1;
            if (!property.isExpanded) return;

            rect.y += 2;
            rect.height = 2;
            EditorGUI.LabelField(rect, "");

            rect.xMin += 2;
            rect.height = height;
            try
            {
                //CommonFun.BeginLayout(propertyData.needIndent, propertyData.needBoundBox);

                var serializedPropertyCopy = property.Copy();
                SerializedProperty endProperty = serializedPropertyCopy.GetEndProperty();
                var drawMembers = true;
                var propertyCache = propertyData.propertyCache;

                var typeBindRule = EBinderRule.Instance;

                while (serializedPropertyCopy.NextVisible(drawMembers)
                    && !SerializedProperty.EqualContents(serializedPropertyCopy, endProperty))
                {
                    drawMembers = false;

                    switch (serializedPropertyCopy.name)
                    {
                        case nameof(TypeBinder._typeBindRule):
                            {
                                typeBindRule = (EBinderRule)serializedPropertyCopy.intValue;
                                break;
                            }
                        case nameof(TypeBinder._includeBaseType):
                            {
                                if (typeBindRule == EBinderRule.Alias) continue;
                                break;
                            }
                        case nameof(TypeBinder._targetType):
                            {
                                if (typeBindRule != EBinderRule.Static) continue;
                                break;
                            }
                        case nameof(TypeBinder._target):
                            {
                                if (typeBindRule != EBinderRule.Instance) continue;
                                break;
                            }
                        case nameof(TypeBinder._instanceTypeFullName):
                            {
                                if (typeBindRule != EBinderRule.InstanceType) continue;
                                break;
                            }
                        case nameof(TypeBinder._targetAlias):
                            {
                                if (typeBindRule != EBinderRule.Alias) continue;
                                break;
                            }
                    }
                    var propertyDataCopy = propertyCache.GetPropertyData(serializedPropertyCopy);
                    if (propertyCache.drawMember && !propertyData.hide)
                    {
                        rect.y += height + 2;
                        i++;
                        EditorGUI.PropertyField(rect, serializedPropertyCopy, propertyDataCopy.GetTrLabel(), false);
                    }
                }
            }
            finally
            {
                //CommonFun.EndLayout();
            }
        }
    }
}
