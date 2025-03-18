using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorExtension.Base;
using XCSJ.EditorSMS.States.Base;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginXGUI;
using XCSJ.PluginXGUI.States;
using XCSJ.PluginXGUI.Widgets;

namespace XCSJ.EditorXGUI.States
{
    /// <summary>
    /// 显示对话框检查器
    /// </summary>
    [Name("显示对话框检查器")]
    [CustomEditor(typeof(ShowDialogBox), true)]
    public class ShowDialogBoxInspector : TriggerInspector<ShowDialogBox>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [Languages.LanguageTuple("Missing dialog box object in scene!", "场景中缺少对话框对象!")]
        [Languages.LanguageTuple("Create Dialog Box", "创建对话框")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ShowDialogBox._useGlobalDialog):
                    {
                        if (stateComponent._useGlobalDialog && !XGUIHelper.dialogBox)
                        {
                            base.OnDrawMember(serializedProperty, propertyData);
                            EditorGUILayout.HelpBox(Tr("Missing dialog box object in scene!"), MessageType.Error);
                            if (GUILayout.Button(Tr("Create Dialog Box")))
                            {
                                XCSJ.EditorXGUI.ToolsMenu.CreateCanvasAndLoadPrefabByXGUIPath("对话框");
                            }
                            return;
                        }
                        break;
                    }
                case nameof(ShowDialogBox._result):
                    {
                        if (serializedProperty.FindPropertyRelative(nameof(StringPropertyValue._propertyValueType)).intValue != (int)EPropertyValueType.Value) break;

                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);

                        var text = stateComponent._result.GetValue();
                        if (GUILayout.Button(text, EditorObjectHelper.MiniPopup, UICommonOption.Width60))
                        {
                            var sp = serializedProperty.Copy();
                            EditorHelper.DrawMenu(text, DialogBox.results, newSelectText =>
                            {
                                sp.FindPropertyRelative(nameof(StringPropertyValue._value)).stringValue = newSelectText;
                                sp.serializedObject.ApplyModifiedProperties();
                            });
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
