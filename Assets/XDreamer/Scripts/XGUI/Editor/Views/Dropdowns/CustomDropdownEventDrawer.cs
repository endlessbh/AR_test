using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.EditorXGUI.Views.Dropdowns
{
    /// <summary>
    /// 自定义下拉框事件绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomDropdownEvent), true)]
    public class CustomDropdownEventDrawer : UnityEventDrawer
    {
        static XGUIContent _valueTypeLabel { get; } = new XGUIContent(typeof(CustomDropdownEvent), nameof(CustomDropdownEvent._valueType));

        static XGUIContent _valueLabel { get; } = new XGUIContent(typeof(CustomDropdownEvent), nameof(CustomDropdownEvent._value));

        static XGUIContent _textLabel { get; } = new XGUIContent(typeof(CustomDropdownEvent), nameof(CustomDropdownEvent._text));

        class Data : ArrayElementData { }

        ArrayElementDataCache<Data> cache = new ArrayElementDataCache<Data>();

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 2 + PropertyDrawerHelper.SpaceWidth * 2;
        }

        /// <summary>
        /// 绘制GUIpublic class 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var data = cache.GetData(property);

            var _valueType = property.FindPropertyRelative(nameof(CustomDropdownEvent._valueType));
            var valueType = (EDropdownValueType)_valueType.intValue;

            EditorGUI.BeginChangeCheck();
            valueType = (EDropdownValueType)UICommonFun.EnumPopup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), _valueTypeLabel, valueType);
            if (EditorGUI.EndChangeCheck())
            {
                _valueType.intValue = (int)valueType;
            }

            var rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + PropertyDrawerHelper.SpaceWidth, position.width, EditorGUIUtility.singleLineHeight);

            var trLabel = label;
            switch (valueType)
            {
                case EDropdownValueType.Any: break;
                case EDropdownValueType.Value:
                    {
                        var _value = property.FindPropertyRelative(nameof(CustomDropdownEvent._value));
                        EditorGUI.PropertyField(rect, _value, _valueLabel);

                        if (data.isArrayElement)
                        {
                            trLabel = CommonFun.TempContent(data.indexString + ".值为[" + _value.intValue.ToString() + "]时触发", label.text + ":" + label.tooltip);
                        }
                        else
                        {
                            trLabel = CommonFun.TempContent("值为[" + _value.intValue.ToString() + "]时触发", label.text + ":" + label.tooltip);
                        }
                        break;
                    }
                case EDropdownValueType.Text:
                    {
                        var _text = property.FindPropertyRelative(nameof(CustomDropdownEvent._text));
                        EditorGUI.PropertyField(rect, _text, _textLabel);

                        if (data.isArrayElement)
                        {
                            trLabel = CommonFun.TempContent(data.indexString + ".文本为[" + _text.stringValue + "]时触发", label.text + ":" + label.tooltip);
                        }
                        else
                        {
                            trLabel = CommonFun.TempContent("文本为[" + _text.stringValue + "]时触发", label.text + ":" + label.tooltip);
                        }
                        break;
                    }
            }

            //调用默认绘制
            position.yMin += (EditorGUIUtility.singleLineHeight * 2 + PropertyDrawerHelper.SpaceWidth * 2);

            base.OnGUI(position, property, trLabel);
        }
    }
}
