using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.EditorCommonUtils;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;

namespace XCSJ.EditorExtension.Base.Attributes
{
    /// <summary>
    /// 动画器参数特性绘制器
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimatorParameterPopupAttribute))]
    public class AnimatorParameterPopupAttributeDrawer : PropertyDrawer<AnimatorParameterPopupAttribute>
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        /// <param name="position"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    {
                        var array = Empty<string>.Array;
                        if (property.serializedObject.targetObject is IAnimatorParameter animatorParameter
                            && animatorParameter.TryGetAnimator(property.propertyPath, out Animator animator)
                            && animator 
                            && animator.runtimeAnimatorController is AnimatorController animatorController)
                        {
                            var parameterType = propertyAttribute.parameterType;
                            array = animatorController.parameters.WhereCast(p => (!parameterType.HasValue || parameterType.Value == p.type, p.name)).ToArray();
                        }

                        var popupWidth = propertyAttribute.width;
                        var rect = new Rect(position.x, position.y, position.width - popupWidth - PropertyDrawerHelper.SpaceWidth, EditorGUIUtility.singleLineHeight);
                        base.OnGUI(rect, property, label);

                        rect.x = rect.x + rect.width + PropertyDrawerHelper.SpaceWidth;
                        rect.width = popupWidth;
                        property.stringValue = UICommonFun.Popup(rect, property.stringValue, array);

                        return;
                    }
            }
            base.OnGUI(position, property, label);
        }
    }
}
