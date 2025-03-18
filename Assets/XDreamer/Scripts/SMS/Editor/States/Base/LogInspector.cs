using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.CNScripts;
using XCSJ.PluginSMS.States.CNScripts;
using XCSJ.PluginXGUI;

namespace XCSJ.EditorSMS.States.Base
{
    /// <summary>
    /// 日志检查器
    /// </summary>
    [Name("日志检查器")]
    [CustomEditor(typeof(XCSJ.PluginSMS.States.Base.Log), true)]
    public class LogInspector : StateComponentInspector<XCSJ.PluginSMS.States.Base.Log>
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [Languages.LanguageTuple("Missing log window object in scene!", "场景中缺少日志窗口对象!")]
        [Languages.LanguageTuple("Create Log Window", "创建日志窗口!")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(XCSJ.PluginSMS.States.Base.Log._outputLogWindow):
                    {
                        if (stateComponent._outputLogWindow && !XGUIHelper.logViewController)
                        {
                            base.OnDrawMember(serializedProperty, propertyData);
                            EditorGUILayout.HelpBox(Tr("Missing log window object in scene!"), MessageType.Error);
                            if (GUILayout.Button(Tr("Create Log Window")))
                            {
                                XCSJ.EditorXGUI.ToolsMenu.CreateCanvasAndLoadPrefabByXGUIPath("日志窗口");
                            }
                            return;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
