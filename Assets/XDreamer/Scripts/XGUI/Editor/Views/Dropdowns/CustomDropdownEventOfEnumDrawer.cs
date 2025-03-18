using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.EditorXGUI.Views.Dropdowns
{
    /// <summary>
    /// 自定义枚举型下拉框事件绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(CustomDropdownEventOfEnum), true)]
    public class CustomDropdownEventOfEnumDrawer : UnityEventDrawer
    {
        XGUIContent _enumTypeLabel { get; } = new XGUIContent(typeof(CustomDropdownEventOfEnum), nameof(CustomDropdownEventOfEnum._enumType));
        XGUIContent _enumStringTypeLabel { get; } = new XGUIContent(typeof(CustomDropdownEventOfEnum), nameof(CustomDropdownEventOfEnum._enumStringType));
        protected static XGUIContent _enumStringLabel { get; } = new XGUIContent(typeof(CustomDropdownEventOfEnum), nameof(CustomDropdownEventOfEnum._enumString));

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 3 + 6;
        }

        /// <summary>
        /// 绘制GUIpublic class 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _enumType = property.FindPropertyRelative(nameof(CustomDropdownEventOfEnum._enumType));
            var _enumStringType = property.FindPropertyRelative(nameof(CustomDropdownEventOfEnum._enumStringType));
            var _enumString = property.FindPropertyRelative(nameof(CustomDropdownEventOfEnum._enumString));

            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rect, _enumType, _enumTypeLabel);

            rect.y = rect.y + EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, _enumStringType, _enumStringTypeLabel);

            rect.y = rect.y + EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, _enumString, _enumStringLabel);

            //调用默认绘制
            position.yMin += (EditorGUIUtility.singleLineHeight * 3 + 6);
            base.OnGUI(position, property, label);
        }
    }
}
