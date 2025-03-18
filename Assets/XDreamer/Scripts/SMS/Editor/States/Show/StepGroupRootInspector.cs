using System.Linq;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.EditorSMS.States.Show
{
    /// <summary>
    /// 步骤组根检查器
    /// </summary>
    [Name("步骤组根检查器")]
    [CustomEditor(typeof(StepGroupRoot), true)]
    public class StepGroupRootInspector : StepGroupInspector
    {
        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [Languages.LanguageTuple("If the playback content is empty, the step list cannot be automatically located to the current step in playback mode", "播放内容为空将导致步骤列表在播放模式下无法自动定位至当前步骤")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            base.OnDrawMember(serializedProperty, propertyData);

            switch (serializedProperty.name)
            {
                case nameof(StepGroupRoot.timeLinePlayContent):
                    {
                        if (!serializedProperty.objectReferenceValue)
                        {
                            EditorGUILayout.HelpBox(Tr("If the playback content is empty, the step list cannot be automatically located to the current step in playback mode"), MessageType.Warning);
                        }
                        break;
                    }
            }
        }
    }
}
