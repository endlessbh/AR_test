using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginTools.PropertyDatas;
using XCSJ.Scripts;

namespace XCSJ.EditorTools.PropertyDatas
{
    /// <summary>
    /// 交互属性数据特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(InteractPropertyDataAttribute))]
    public class InteractPropertyDataAttributeDrawer : PropertyDrawer<InteractPropertyDataAttribute>
    {
        private const int PopupWidth = 100;

        /// <summary>
        /// 绘制属性
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    {
                        label = EditorGUI.BeginProperty(rect, label, property);
                        {
                            rect.width -= PopupWidth * (propertyAttribute.interactPropertyDataType == EInteractPropertyDataType.Both ? 2 : 1);
                            property.stringValue = EditorGUI.TextField(rect, label, property.stringValue);

                            rect.x += rect.width;
                            rect.width = PopupWidth;

                            var key = "";
                            var value = "";
                            if(ExpressionStringAnalysisResult.TryParse(property.stringValue, out var result))
                            {
                                key = result.key;
                                value = result.value;
                            }

                            EditorGUI.BeginChangeCheck();
                            switch (propertyAttribute.interactPropertyDataType)
                            {
                                case EInteractPropertyDataType.Key:
                                    {
                                        key = UICommonFun.Popup(rect, key, InteractPropertyHelper.GetAllKeysInScene<InteractProperty>());
                                        break;
                                    }
                                case EInteractPropertyDataType.Value:
                                    {
                                        value = UICommonFun.Popup(rect, value, InteractPropertyHelper.GetAllTextValuesInScene<InteractProperty>());
                                        break;
                                    }
                                case EInteractPropertyDataType.Both:
                                    {
                                        key = UICommonFun.Popup(rect, key, InteractPropertyHelper.GetAllKeysInScene<InteractProperty>());
                                        rect.x += PopupWidth;
                                        value = UICommonFun.Popup(rect, value, InteractPropertyHelper.GetAllTextValuesInScene<InteractProperty>(key));
                                        break;
                                    }
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                property.stringValue = ExpressionHelper.FormatExpressionString(key, value, false);
                            }
   
                        }
                        EditorGUI.EndProperty();
                        return;
                    }
            }
            base.OnGUI(rect, property, label);
        }
    }
}
