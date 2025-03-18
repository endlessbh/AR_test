using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.States;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.EditorSMS.States.Show;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装步骤检查器
    /// </summary>
    [Name("拆装步骤检查器")]
    [CustomEditor(typeof(RepairStep), true)]
    public class RepairStepInspector : StepInspector
    {
        /// <summary>
        /// 拆装步骤
        /// </summary>
        public RepairStep repairStep => target as RepairStep;

        private string scenePath = "";

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            scenePath = SMSHelper.GetScenePath(repairStep);
        }

        /// <summary>
        /// 零件列表操作
        /// </summary>
        [Name("零件列表操作")]
        public bool selectedPartsOperation;

        /// <summary>
        /// 工具列表操作
        /// </summary>
        [Name("工具列表操作")]
        public bool selectedToolsOperation;

        /// <summary>
        /// 当绘制成员
        /// </summary>
        /// <param name="serializedProperty"></param>
        /// <param name="propertyData"></param>
        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(RepairStep.selectedParts):
                    {                        
                        DrawSelectComponent<Part>(TrLabel(nameof(selectedPartsOperation)), serializedProperty.Copy());
                        break;
                    }
                case nameof(RepairStep.selectedTools):
                    {
                        DrawSelectComponent<XCSJ.PluginRepairman.States.Tool>(TrLabel(nameof(selectedToolsOperation)), serializedProperty.Copy());
                        break;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }

        private void DrawSelectComponent<T>(GUIContent label, SerializedProperty memberProperty) where T : StateComponent
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            SMTreeEditorWindow.SelectStateComponentWithButton<T>((win, sc) => SetMemberPropertyList(memberProperty, sc), null, EditorStyles.miniButtonLeft, scenePath, UICommonOption.WH24x16);

            if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Add), "添加选中对象到列表中"), EditorStyles.miniButtonMid, UICommonOption.WH24x16))
            {
                int addObjectCount = 0;
                foreach (var node in NodeSelection.selections)
                {
                    if (node is StateNode sn)
                    {
                        var targetObj = sn.state.GetComponent<T>();
                        if (targetObj)
                        {
                            ++addObjectCount;
                            SetMemberPropertyList(memberProperty, targetObj);
                        }
                    }
                }
                if (addObjectCount == 0)
                {
                    Debug.LogWarningFormat("添加失败，请选择状态机中的[{0}]对象!", CommonFun.Name(typeof(T)));
                }
            }

            if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Delete), "清除列表中所有对象"), EditorStyles.miniButtonRight, UICommonOption.WH24x16))
            {
                memberProperty.ClearArray();
                memberProperty.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
