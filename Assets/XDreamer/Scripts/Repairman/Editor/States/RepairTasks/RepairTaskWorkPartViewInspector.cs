using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.Input;
using XCSJ.EditorSMS.Inspectors;
using XCSJ.EditorSMS.States.Nodes;
using XCSJ.EditorTools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.RepairTask;
using static XCSJ.PluginXGUI.Windows.Tables.Table;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装任务零件视图检查器
    /// </summary>
    [Name("拆装任务零件视图检查器")]
    [CustomEditor(typeof(RepairTaskWorkPartView), true)]
    public class RepairTaskWorkPartViewInspector : StateComponentInspector<RepairTaskWorkPartView>
    {
        private string createStepButtonName;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            createStepButtonName = string.Format("添加选中的零件为[{0}]", RepairStepByMatchPosition.Title);
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // 根据选择零件动态添加步骤
            if (GUILayout.Button(new GUIContent(createStepButtonName, EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                foreach (var node in NodeSelection.selections)
                {
                    if (node is StateNode sn)
                    {
                        var s = sn.state;
                        if (s)
                        {
                            var part = s.GetComponent<Part>();
                            if (part)
                            {
                                var stepState = RepairStepByMatchPosition.Create(stateComponent.parent);
                                var step = stepState.GetComponent<RepairStep>();
                                step.XModifyProperty(() => step.selectedParts.Add(part));
                            }
                        }
                    }
                }
            }
        }

        protected override void OnDrawMember(SerializedProperty serializedProperty, PropertyData propertyData)
        {
            switch (serializedProperty.name)
            {
                case nameof(RepairTaskWorkPartView._table):
                    {
                        EditorGUILayout.BeginHorizontal();
                        base.OnDrawMember(serializedProperty, propertyData);
                        if (GUILayout.Button(new GUIContent(EditorIconHelper.GetIconInLib(EIcon.Add), "创建可抓对象列表"), UICommonOption.WH32x16))
                        {
                            var go = EditorToolsHelperExtension.LoadPrefab_DefaultToolPath("可抓对象列表.prefab");
                            XCSJ.EditorXGUI.ToolsMenu.CreateUIInCanvas(() => go);
                            if (go)
                            {
                                stateComponent.XModifyProperty(() => { if (stateComponent.table) { } });

                                var table = stateComponent.table;
                                if (table)
                                {
                                    table.XModifyProperty(()=> table._modelHandleRule = EDataHandleRule.InactiveGameObjectOnAdd | EDataHandleRule.UnselectGameObjectOnAdd | EDataHandleRule.ActiveGameObjectOnRemove);
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        return;
                    }
            }
            base.OnDrawMember(serializedProperty, propertyData);
        }
    }
}
