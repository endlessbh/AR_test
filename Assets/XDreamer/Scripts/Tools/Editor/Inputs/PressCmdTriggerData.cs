using UnityEditor;
using UnityEngine;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorExtension.Base.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Inputs;

namespace XCSJ.EditorTools.Inputs
{
    /// <summary>
    /// 数据项
    /// </summary>
    public class PressCmdTriggerData : ArrayElementData
    {
        private SerializedProperty cmdNameSP;
        protected SerializedProperty useRuleSP;
        protected SerializedProperty uiRuleSP;
        private SerializedProperty triggerStateSP;
        private SerializedProperty pressMaxDistanceSP;
        private SerializedProperty fixedTimeSP;
        protected bool display = true;

        /// <summary>
        /// 初始化属性
        /// </summary>
        /// <param name="property"></param>
        public override void Init(SerializedProperty property)
        {
            base.Init(property);

            cmdNameSP = property.FindPropertyRelative(nameof(RayCmd._cmdName));
            useRuleSP = property.FindPropertyRelative(nameof(RayCmd._inputMode));
            uiRuleSP = property.FindPropertyRelative(nameof(RayCmd._uiRule));
            triggerStateSP = property.FindPropertyRelative(nameof(RayCmd._triggerState));
            pressMaxDistanceSP = property.FindPropertyRelative(nameof(RayCmd._pressMaxDistance));
            fixedTimeSP = property.FindPropertyRelative(nameof(RayCmd._fixedTime));
        }

        /// <summary>
        /// 获取行数
        /// </summary>
        /// <returns></returns>
        public virtual int GetRowCount()
        {
            var pressState = (EPressState)triggerStateSP.intValue;

            return display ? ((pressState == EPressState.PressedAndReleased || pressState == EPressState.KeepingAndDelayTimeAndHasInteractable || pressState == EPressState.NoKeepingAndDelayTimeAndHasInteractable) ? 6 : 5) : 1;
        }

        // 绘制
        public virtual Rect OnGUI(Rect rect, GUIContent label)
        {
            label = isArrayElement ? indexContent : label;

            // 标题
            rect = new Rect(rect.x, rect.y, rect.width, 18);
            GUI.Label(rect, "", XGUIStyleLib.Get(EGUIStyle.Box));
            display = GUI.Toggle(rect, display, label, EditorStyles.foldout);

            if (display)
            {
                // 匹配规则
                rect.xMin += 18;

                rect = PropertyDrawerHelper.DrawProperty(rect, useRuleSP);

                EditorGUI.BeginDisabledGroup(useRuleSP.intValue == 0);
                {
                    rect = PropertyDrawerHelper.DrawProperty(rect, cmdNameSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, uiRuleSP);
                    rect = PropertyDrawerHelper.DrawProperty(rect, triggerStateSP);

                    var pressState = (EPressState)triggerStateSP.intValue;

                    switch (pressState)
                    {
                        case EPressState.PressedAndReleased:
                            {
                                rect = PropertyDrawerHelper.DrawProperty(rect, pressMaxDistanceSP);
                                break;
                            }
                        case EPressState.KeepingAndDelayTimeAndHasInteractable:
                        case EPressState.NoKeepingAndDelayTimeAndHasInteractable:
                            {
                                rect = PropertyDrawerHelper.DrawProperty(rect, fixedTimeSP);
                                break;
                            }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            return rect;
        }
    }
}
