using UnityEditor;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorXGUI;
using XCSJ.PluginSMS.States.UGUI;

namespace XCSJ.EditorSMS.States.UGUI
{
    /// <summary>
    /// 滑块控制检查器
    /// </summary>
    [Name("滑块控制检查器")]
    [CustomEditor(typeof(SliderController))]
    public class SliderControllerInspetor : StateComponentInspector<SliderController>
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
                case nameof(stateComponent.slider):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        EditorXGUIHelper.DrawCreateButton(stateComponent.slider, () =>
                        {
                            ToolsMenu.CreateUIInCanvas(() =>
                            {
                                var slider = ToolsMenu.CreateUIWithStyle<Slider>();
                                serializedProperty.objectReferenceValue = slider;
                                return slider.gameObject;
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
