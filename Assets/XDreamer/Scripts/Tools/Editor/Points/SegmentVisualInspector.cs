using UnityEditor;
using UnityEngine;
using XCSJ.EditorCommonUtils;
using XCSJ.Languages;
using XCSJ.PluginTools.Effects;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.Points;

namespace XCSJ.EditorTools.Points
{
    /// <summary>
    /// 线段可视化检查器
    /// </summary>
    [CustomEditor(typeof(SegmentVisual), true)]
    [CanEditMultipleObjects]
    public class SegmentVisualInspector : MBInspector<SegmentVisual>
    {
        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        [LanguageTuple("When the line render type is equal to the line renderer, the line style width must be greater than 0", "线渲染类型等于线渲染器时线样式宽度必须大于0")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(SegmentVisual._lineRenderMode):
                    {
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (targetObject._lineRenderMode == ERenderMode.LineRenderer)
                        {
                            if (targetObject._lineStyle && targetObject._lineStyle.width == 0)
                            {
                                UICommonFun.RichHelpBox("When the line render type is equal to the line renderer, the line style width must be greater than 0".Tr(this.GetType()), MessageType.Error);
                            }
                        }
                        return;
                    }
                case nameof(SegmentVisual._lineStyle):
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            base.OnDrawMember(serializedProperty, propertyData);
                            EditorGUI.BeginDisabledGroup(targetObject._lineStyle);
                            if (GUILayout.Button(UICommonOption.Insert, UICommonOption.WH24x16))
                            {
                                serializedProperty.objectReferenceValue = targetObject.gameObject.AddComponent<LineStyle>();
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
