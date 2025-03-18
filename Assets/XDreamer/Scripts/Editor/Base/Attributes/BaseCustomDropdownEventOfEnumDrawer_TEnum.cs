using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Views.Dropdowns;

namespace XCSJ.EditorExtension.XGUI.Dropdowns
{
    /// <summary>
    /// 自定义枚举型下拉框事件绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(BaseCustomDropdownEventOfEnum), true)]
    public class BaseCustomDropdownEventOfEnumDrawer_TEnum : UnityEventDrawer
    {
        XGUIContent _enumStringTypeLabel { get; } = new XGUIContent(typeof(BaseCustomDropdownEventOfEnum<>), nameof(BaseCustomDropdownEventOfEnum<EEnumStringType>._enumStringType));
        XGUIContent _enumValueLabel { get; } = new XGUIContent(typeof(BaseCustomDropdownEventOfEnum<>), nameof(BaseCustomDropdownEventOfEnum<EEnumStringType>._enumValue));

        /// <summary>
        /// 获取属性高度
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        /// <summary>
        /// 绘制GUIpublic class 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var _enumStringType = property.FindPropertyRelative(nameof(BaseCustomDropdownEventOfEnum<EEnumStringType>._enumStringType));
            var _enumValue = property.FindPropertyRelative(nameof(BaseCustomDropdownEventOfEnum<EEnumStringType>._enumValue));

            var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rect, _enumStringType, _enumStringTypeLabel);

            rect.y = rect.y + EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.PropertyField(rect, _enumValue, _enumValueLabel);

            //调用默认绘制
            position.yMin += (EditorGUIUtility.singleLineHeight * 2 + 4);
            base.OnGUI(position, property, label);
        }
    }
}
