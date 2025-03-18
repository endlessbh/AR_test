using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.LocomotionProviders;

#if XDREAMER_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace XCSJ.EditorXXR.Interaction.Toolkit.Tools.LocomotionProviders
{
    /// <summary>
    /// 模拟运动提供者检查器
    /// </summary>
    [Name("模拟运动提供者检查器")]
    [CustomEditor(typeof(AnalogLocomotionProvider))]
    class AnalogLocomotionProviderInspector
#if XDREAMER_XR_INTERACTION_TOOLKIT
        : Editor
#else
        : MBInspector<AnalogLocomotionProvider>        
#endif
    {

#if XDREAMER_XR_INTERACTION_TOOLKIT

        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        public void OnEnable()
        {
            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(AnalogLocomotionProvider));
        }

        /// <summary>
        /// 检查器绘制
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            categoryList.DrawVertical();

            DrawSetPreset();
        }

        private bool setMoveResult = true;
        private bool setTurnResult = true;

        [LanguageTuple("Set [ActionBasedContinuousMoveProvider] Preset", "设置[基于动作的连续移动提供者]预设动作")]
        [LanguageTuple("Set [ActionBasedContinuousTurnProvider] Preset", "设置[基于动作的连续转动提供者]预设动作")]
        [LanguageTuple("No preset action resources were found in the project for the [ActionBasedContinuousMoveProvider] type, Standard assets can be imported from the [XR Interaction Toolkit]", "项目中未找到适用于[基于动作的连续移动提供者]类型的预设动作资源,可从[XR Interaction Toolkit]中导入标准资产")]
        [LanguageTuple("No preset action resources were found in the project for the [ActionBasedContinuousTurnProvider] type, Standard assets can be imported from the [XR Interaction Toolkit]", "项目中未找到适用于[基于动作的连续转动提供者]类型的预设动作资源,可从[XR Interaction Toolkit]中导入标准资产")]
        private void DrawSetPreset()
        {
            var go = (target as Component).gameObject;

            // 绘制设置移动预设
            {
                var moveProvider = go.GetComponent<ActionBasedContinuousMoveProvider>();
                EditorGUI.BeginDisabledGroup(!moveProvider);
                {
                    if (GUILayout.Button("Set [ActionBasedContinuousMoveProvider] Preset".Tr(typeof(AnalogLocomotionProviderInspector))))
                    {
                        setMoveResult = EditorXRITHelper.SetPresetTo(moveProvider);
                    }
                    if (moveProvider && !setMoveResult)
                    {
                        UICommonFun.RichHelpBox(@"<size=12>" + "No preset action resources were found in the project for the [ActionBasedContinuousMoveProvider] type, Standard assets can be imported from the [XR Interaction Toolkit]".Tr(typeof(AnalogLocomotionProviderInspector)) + "</size>", MessageType.Warning);
                    }
                }
                EditorGUI.EndDisabledGroup();
            }

            // 绘制设置转动预设
            {
                var turnProvider = go.GetComponent<ActionBasedContinuousTurnProvider>();
                EditorGUI.BeginDisabledGroup(!turnProvider);
                if (GUILayout.Button("Set [ActionBasedContinuousTurnProvider] Preset".Tr(typeof(AnalogLocomotionProviderInspector))))
                {
                    setTurnResult = EditorXRITHelper.SetPresetTo(turnProvider);
                }
                if (turnProvider && !setTurnResult)
                {
                    UICommonFun.RichHelpBox(@"<size=12>" + "No preset action resources were found in the project for the [ActionBasedContinuousTurnProvider] type, Standard assets can be imported from the [XR Interaction Toolkit]".Tr(typeof(AnalogLocomotionProviderInspector)) + "</size>", MessageType.Warning);
                }
                EditorGUI.EndDisabledGroup();
            }
        }

#endif
    }
}
