using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorSMS.States.Show;
using XCSJ.EditorTools;
using XCSJ.PluginRepairman.States.RepairTask;

namespace XCSJ.EditorRepairman.Inspectors
{
    /// <summary>
    /// 拆装修理任务检查器
    /// </summary>
    [Name("拆装修理任务检查器")]
    [CustomEditor(typeof(RepairTaskWork), true)]
    public class RepairTaskWorkInspector : StepGroupRootInspector
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [Languages.LanguageTuple("Create [Repair Step List] UI", "创建[拆装步骤列表]界面")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent(Tr("Create [Repair Step List] UI"), EditorIconHelper.GetIconInLib(EIcon.Add)), UICommonOption.Height18))
            {
                XCSJ.EditorRepairman.Tools.ToolsMenu.CreateUITaskWorkTreeView(ToolContext.Get(typeof(XCSJ.EditorRepairman.Tools.ToolsMenu), nameof(XCSJ.EditorRepairman.Tools.ToolsMenu.CreateUITaskWorkTreeView)));
            }
        }
    }
}
