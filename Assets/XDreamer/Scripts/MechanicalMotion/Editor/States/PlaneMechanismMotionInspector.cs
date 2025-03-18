using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Base;
using XCSJ.PluginMechanicalMotion.States;
using XCSJ.PluginSMS.States.Others;

namespace XCSJ.EditorMechanicalMotion.States
{
    /// <summary>
    /// 平面机构运动控制检查器
    /// </summary>
    [Name("平面机构运动控制检查器")]
    [CustomEditor(typeof(PlaneMechanismMotion))]
    public class PlaneMechanismMotionInspector: WorkClipInspector<PlaneMechanismMotion>
    {
        private static bool _synLimitRange = true;

        /// <summary>
        /// 绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        [Languages.LanguageTuple("Synchronization limit range", "同步限定范围")]
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(PlaneMechanismMotion._range):
                    {
                        var pm = stateComponent._planeMechanism;
                        if (pm && pm._isLimit)
                        {
                            EditorGUILayout.BeginHorizontal();

                            base.OnDrawMember(serializedProperty, propertyData);

                            if (_synLimitRange = UICommonFun.ButtonToggle(new GUIContent(Tr("Synchronization limit range")), _synLimitRange, EditorStyles.miniButtonRight, UICommonOption.Width80))
                            {
                                serializedProperty.vector2Value = pm._range;
                            }
                            EditorGUILayout.EndHorizontal();
                            return;
                        }
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
