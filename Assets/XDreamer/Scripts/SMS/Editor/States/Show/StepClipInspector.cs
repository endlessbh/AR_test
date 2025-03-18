using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.PluginSMS.States.Show;

namespace XCSJ.EditorSMS.States.Show
{
    /// <summary>
    /// 步骤剪辑检查器
    /// </summary>
    [Name("步骤剪辑检查器")]
    [CustomEditor(typeof(StepClip), true)]
    public class StepClipInspector : StateComponentInspector<StepClip>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {        
            switch (serializedProperty.name)
            {
                case nameof(StepClip.step):
                    {
                        if (stateComponent.step)
                        {
                            propertyData.trLabel.text = stateComponent.step.parent.name;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
