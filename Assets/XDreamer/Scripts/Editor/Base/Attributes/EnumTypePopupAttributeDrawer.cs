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
    /// 枚举类型弹出菜单特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumTypePopupAttribute))]
    public class EnumTypePopupAttributeDrawer : PropertyDrawer<EnumTypePopupAttribute>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    {
                        if (PropertyDrawerHelper.DrawStringPopup(position, property, label, EnumTypePopupAttribute.EnumTypeStrings, true, propertyAttribute.width)) return;
                        break;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }
}
