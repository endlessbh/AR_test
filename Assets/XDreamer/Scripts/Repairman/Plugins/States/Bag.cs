using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States
{
    /// <summary>
    /// 背包
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Bag))]
    [XCSJ.Attributes.Icon(index = 34480)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("背包组件是可放置零件或工具的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。", "A backpack assembly is a container in which parts or tools can be placed. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use.")]
    public class Bag : Tool
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "背包";

        [Name(Title, nameof(Bag))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [RequireManager(typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("背包组件是可放置零件或工具的容器。用子状态机实现。是一个数据组织对象、其中数据提供给其他状态组件使用。", "A backpack assembly is a container in which parts or tools can be placed. It is realized by sub state machine. Is a data organization object in which data is provided to other state components for use.")]
        public static State CreateBag(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(Bag)),null, typeof(Bag));
        }

        public override ETreeNodeType nodeType => ETreeNodeType.Root;
    }
}
