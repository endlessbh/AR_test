using System;
using UnityEditor;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorXGUI;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.UGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    /// <summary>
    /// 滚动条控制检查器
    /// </summary>
    [Name("滚动条控制检查器")]
    [CustomEditor(typeof(ScrollBarController))]
    public class ScrollBarControllerInspetor : StateComponentInspector<ScrollBarController>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(stateComponent.numberValueTrigger):
                    {
                        if (stateComponent.scrollbar)
                        {
                            stateComponent.numberValueTrigger.compareRule = (ENumberValueCompareRule)UICommonFun.EnumPopup(CommonFun.NameTip(typeof(FloatValueTrigger), nameof(FloatValueTrigger.compareRule)), stateComponent.numberValueTrigger.compareRule);

                            if (stateComponent.numberValueTrigger.compareRule != ENumberValueCompareRule.Changed)
                            {
                                stateComponent.numberValueTrigger.triggerValue = EditorGUILayout.Slider(CommonFun.NameTooltip(typeof(FloatValueTrigger), nameof(FloatValueTrigger.triggerValue)),
stateComponent.numberValueTrigger.triggerValue, 0, 1);
                            }

                        }
                        return;
                    }
                case nameof(stateComponent.scrollbar):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.scrollbar, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var scrollbar = EditorXGUIHelper.CreateUGUI<Scrollbar>();
                                serializedProperty.objectReferenceValue = scrollbar;
                                return scrollbar.gameObject;
                            });
                        });
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
