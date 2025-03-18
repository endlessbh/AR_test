using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.Collections;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.PluginCommonUtils;


namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 下拉式弹出菜单特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(DropdownPopupAttribute), true)]
    public class DropdownPopupAttributeDrawer : PropertyDrawer<DropdownPopupAttribute>
    {
        /// <summary>
        /// 绘制GUI时调用
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var popup = property.ConvertValueTo<IDropdownPopupAttribute>();
            if (popup != null)
            {
                var attr = propertyAttribute;

                var purpose = attr.purpose;
                var propertyPath = property.propertyPath;

                if (popup.TryGetOptions(purpose, propertyPath, out string[] options))
                {
                    EditorGUI.BeginChangeCheck();

                    var newOption = "";
                    if (attr.hasField)
                    {
                        var popupWidth = attr.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - PropertyDrawerHelper.SpaceWidth, position.height);
                        base.OnGUI(rect, property, label);

                        if (!popup.TryGetOption(purpose, propertyPath, options, property.GetSerializedPropertyValue(), out string option))
                        {
                            option = "";
                        }

                        rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
                        rect.width = popupWidth;
                        newOption = UICommonFun.Popup(rect, option, options);
                    }
                    else
                    {
                        if (!popup.TryGetOption(purpose, propertyPath, options, property.GetSerializedPropertyValue(), out string option))
                        {
                            option = "";
                        }

                        label = EditorGUI.BeginProperty(position, label, property);
                        newOption = UICommonFun.Popup(position, label, option, options);
                        EditorGUI.EndProperty();
                    }

                    if (EditorGUI.EndChangeCheck() && popup.TryGetPropertyValue(purpose, propertyPath, options, newOption, out object propertyValue))
                    {
                        property.SetSerializedPropertyValue(propertyValue);
                    }
                    return;
                }
            }
            else
            {
                var type = property.serializedObject.targetObject.GetType();
                Debug.LogErrorFormat("类[{0}]({1})未实现接口[{2}],属性[{3}]的修饰特性[{4}]无法生效!",
                    CommonFun.Name(type),
                    type.FullName,
                    nameof(IDropdownPopupAttribute),
                    property.propertyPath,
                    attribute.GetType()
                    );
            }
            base.OnGUI(position, property, label);
        }
    }
}
