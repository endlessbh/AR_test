using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginXGUI.DataViews.GroupViews;

namespace XCSJ.EditorXGUI.DataViews.GroupViews
{
    /// <summary>
    /// 变换数据视图检查器
    /// </summary>
    [Name("变换数据视图检查器")]
    [CustomEditor(typeof(TransformView), true)]
    public class TransformViewInspector : ComponentViewInspector
    {
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(ComponentView._includeBaseType):
                case nameof(ComponentView._viewBindDataTypeMemberMode):
                    return;
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
