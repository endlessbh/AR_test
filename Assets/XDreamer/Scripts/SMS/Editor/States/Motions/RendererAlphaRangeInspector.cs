using System;
using System.Reflection;
using UnityEditor;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginSMS.States.Motions;

namespace XCSJ.EditorSMS.States.Motions
{
    /// <summary>
    /// 渲染器透明度区间检查器
    /// </summary>
    [Name("渲染器透明度区间检查器")]
    [CustomEditor(typeof(RendererAlphaRange))]
    public class RendererAlphaRangeInspector : RendererRangeHandleInspector<RendererAlphaRange>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(stateComponent.leftValue):
                case nameof(stateComponent.inValue):
                case nameof(stateComponent.rightValue):
                    {
                        serializedProperty.floatValue = EditorGUILayout.Slider(propertyData.trLabel, serializedProperty.floatValue, 0, 1);
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
