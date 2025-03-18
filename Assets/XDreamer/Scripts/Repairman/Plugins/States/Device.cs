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
    /// 设备：模块或零件集合，不能再被包含
    /// 1、记录机械对象的零件位置和旋转量，对机械零件移动轴进行记录
    /// 2、记录机械零件之间的拆装约束关系，分析当前零件自由度（是否自由拆或装）
    /// </summary>
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]    
    [Name(Title, nameof(Device))]
    [XCSJ.Attributes.Icon(index = 34481)]
    [DisallowMultipleComponent]
    [RequireState(typeof(SubStateMachine))]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("设备组件是包含零件组件与模块组件的容器。是一个数据组织对象、其中数据提供给其他状态组件使用。", "Equipment components are containers that contain part components and module components. Is a data organization object in which data is provided to other state components for use.")]
    public sealed class Device : Module
    {
        /// <summary>
        /// 名称
        /// </summary>
        public new const string Title = "设备";

        [Name(Title, nameof(Device))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager), stateType = EStateType.SubStateMachine)]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("设备组件是包含零件组件与模块组件的容器。是一个数据组织对象、其中数据提供给其他状态组件使用。", "Equipment components are containers that contain part components and module components. Is a data organization object in which data is provided to other state components for use.")]
        public static State CreateDevice(IGetStateCollection obj)
        {
            return obj?.CreateSubStateMachine(CommonFun.Name(typeof(Device)), null, typeof(Device));
        }

        #region 属性

        /// <summary>
        /// 关联交互零件
        /// </summary>
        public override Tools.Part interactPart
        {
            get
            {
                if (!gameObject) return null;

                var d = gameObject.XGetOrAddComponent<Tools.Device>();
                d._moduleSC = this;
                return d;
            }
        }

        public override ETreeNodeType nodeType => ETreeNodeType.Root;

        #endregion

        #region 状态组件方法

        public override bool DataValidity() => go && !GetParentItem();

        public override string ToFriendlyString() => GetParentItem() ? "父容器不能是物品" : "";

        #endregion
    }
}
