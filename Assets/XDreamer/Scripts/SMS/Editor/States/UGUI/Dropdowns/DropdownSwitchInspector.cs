using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.XGUI;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.NodeKit;
using XCSJ.EditorXGUI;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States.UGUI;
using XCSJ.PluginSMS.States.UGUI.Dropdowns;

namespace XCSJ.EditorSMS.States.UGUI.Dropdowns
{
    /// <summary>
    /// 下拉框切换检查器
    /// </summary>
    [Name("下拉框切换检查器")]
    [CustomEditor(typeof(DropdownSwitch))]
    public class DropdownSwitchInspcetor : StateComponentInspector<DropdownSwitch>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(stateComponent._dropdown):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.dropdown, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var dropdown = ToolsMenu.CreateUIWithStyle<Dropdown>();
                                serializedProperty.objectReferenceValue = dropdown;
                                return dropdown.gameObject;
                            });
                        });
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(stateComponent.triggerValue):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (stateComponent.dropdown)
                        {
                            List<Dropdown.OptionData> options = stateComponent.dropdown.options;
                            string[] displayOptions = new string[options.Count];
                            int[] optionValues = new int[options.Count];
                            for (int i = 0; i < options.Count; i++)
                            {
                                displayOptions[i] = options[i].text;
                                optionValues[i] = i;
                            }
                            serializedProperty.intValue = EditorGUILayout.IntPopup(serializedProperty.intValue, displayOptions, optionValues, GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
                case nameof(stateComponent.triggerText):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (stateComponent.dropdown)
                        {
                            List<Dropdown.OptionData> options = stateComponent.dropdown.options;
                            string[] displayOptions = new string[options.Count];
                            int[] optionValues = new int[options.Count];
                            for (int i = 0; i < options.Count; i++)
                            {
                                displayOptions[i] = options[i].text;
                                optionValues[i] = i;
                            }
                            serializedProperty.stringValue = UICommonFun.Popup(serializedProperty.stringValue, displayOptions, false, GUILayout.Width(100));
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
