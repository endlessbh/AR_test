using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Windows.TreeViews;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 树视图计划数据
    /// </summary>
    [Name("树视图计划数据")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UITreeView))]
    [RequireManager(typeof(SMSManager))]
    public class UITreeViewPlanData : InteractProvider
    {
        /// <summary>
        /// 计划:状态机系统中的计划状态组件
        /// </summary>
        [Name("计划")]
        [Tip("状态机系统中的计划状态组件", "Planning state component in state machine system")]
        [StateComponentPopup(typeof(Plan), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Plan plan;

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            try
            {
                if (plan)
                {
                    var tree = GetComponent<UITreeView>();
                    tree.data = plan;
                    tree.Create();
                }
            }
            catch { }
        }
    }
}
