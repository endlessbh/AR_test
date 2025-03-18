using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXGUI.Windows.TreeViews;

namespace XCSJ.PluginRepairman.States.RepairTask
{
    /// <summary>
    /// 树视图任务工作数据
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UITreeView))]
    [Name("树视图任务工作数据")]
    [RequireManager(typeof(RepairmanManager))]
    public class UITreeViewTaskWorkData : InteractProvider
    {
        /// <summary>
        /// 拆装任务
        /// </summary>
        [Name("拆装任务")]
        [Tip("状态机系统中的计划状态组件", "Planning state component in state machine system")]
        [StateComponentPopup(typeof(RepairTaskWork), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairTaskWork repairTaskWork;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            try
            {
                if (!repairTaskWork)
                {
                    repairTaskWork = RepairmanHelperExtension.GetStateComponent<RepairTaskWork>();
                }
                if (repairTaskWork)
                {
                    var tree = GetComponent<UITreeView>();
                    tree.data = repairTaskWork;
                    tree.Create();
                }
            }
            catch { }
        }
    }
}