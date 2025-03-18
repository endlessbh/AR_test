using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Interactions;
using XCSJ.EditorExtension.Base.Interactions.Tools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Inputs;
using static XCSJ.PluginTools.Inputs.RayCmd;

namespace XCSJ.EditorTools.Inputs
{
    /// <summary>
    /// 鼠标输入器检查器
    /// </summary>
    [Name("鼠标输入器检查器")]
    [CustomEditor(typeof(MouseInput))]
    public class MouseInputInspector : InteractorInspector<MouseInput>
    {
        [Name("UI规则")]
        private static EUIRule uiRule = EUIRule.InvalidOnAnyUI;

        /// <summary>
        /// 绘制成员前
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [LanguageTuple("Batch Set Cmd Input Use Rule", "批量设置命令输入使用规则")]
        [LanguageTuple("Standard", "标准")]
        [LanguageTuple("Analog", "模拟")]
        [LanguageTuple("Batch Set Cmd UI Rule", "批量设置命令UI规则")]
        [LanguageTuple("Set", "设置")]
        protected override void OnBeforeDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.propertyPath)
            {
                case nameof(MouseInput._cmds):
                    {
                        EditorGUILayout.BeginVertical(XGUIStyleLib.Get(EGUIStyle.Box));
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(Tr("Batch Set Cmd Input Use Rule"), GUILayout.Width(170));
                                if (GUILayout.Button("Nothing"))
                                {
                                    SetInputMode(0);
                                }
                                if (GUILayout.Button("Everything"))
                                {
                                    SetInputMode(RayCmd.EInputMode.Standard | RayCmd.EInputMode.Analog);
                                }
                                if (GUILayout.Button(Tr("Standard")))
                                {
                                    SetInputMode(RayCmd.EInputMode.Standard);
                                }
                                if (GUILayout.Button(Tr("Analog")))
                                {
                                    SetInputMode(RayCmd.EInputMode.Analog);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                GUILayout.Label(Tr("Batch Set Cmd UI Rule"), GUILayout.Width(170));

                                uiRule = (EUIRule)UICommonFun.EnumPopup(uiRule);
                                if (GUILayout.Button(Tr("Set"), GUILayout.MinWidth(80)))
                                {
                                    SetUIRule(uiRule);
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                        break;
                    }
            }
            base.OnBeforeDrawMember(serializedProperty, propertyData);
        }

        private void SetInputMode(RayCmd.EInputMode inputMode) => targetObject.XModifyProperty(() => targetObject.SetInputMode(inputMode));

        private void SetUIRule(EUIRule uiRule) => targetObject.XModifyProperty(() => targetObject.SetUIRule(uiRule));
    }
}