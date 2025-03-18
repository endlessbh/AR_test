using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 组件集弹出特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(ComponentCollectionPopupAttribute))]
    public class ComponentCollectionPopupAttributeDrawer : PropertyDrawer<ComponentCollectionPopupAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                base.OnGUI(position, property, label);
                return;
            }

            var popupWidth = propertyAttribute.width;
            var rect = new Rect(position.x, position.y, position.width - popupWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
            base.OnGUI(rect, property, label);

            rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
            rect.width = popupWidth;
            EditorObjectHelper.ComponentCollectionPopup(rect, property);
        }
    }
}
