using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginRepairman.Machines;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States
{
    [ComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(Part))]
    [XCSJ.Attributes.Icon(EIcon.Part)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tip("零件组件用于关联一个三维模型和图片的容器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。零件不能放在设备和模块之外。零件不能包含零件。", "The part component is a container used to associate a 3D model with a picture. It is realized by state. Is a data organization object in which data is provided to other state components for use. Parts cannot be placed outside equipment and modules. Parts cannot contain parts.")]
    public class Part : Item
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "零件";

        [Name(Title, nameof(Part))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [StateLib(RepairmanHelperExtension.DataModelStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.DataModelStateLibName + "/" + Title, typeof(RepairmanManager))]
        [Tip("零件组件用于关联一个三维模型和图片的容器。用状态来实现。是一个数据组织对象、其中数据提供给其他状态组件使用。零件不能放在设备和模块之外。零件不能包含零件。", "The part component is a container used to associate a 3D model with a picture. It is realized by state. Is a data organization object in which data is provided to other state components for use. Parts cannot be placed outside equipment and modules. Parts cannot contain parts.")]
        public static State CreatePart(IGetStateCollection obj) => obj?.CreateNormalState(CommonFun.Name(typeof(Part)), null, typeof(Part));

        #region 属性

        /// <summary>
        /// 所属的模块
        /// </summary>
        public Module module => parent.parent.GetComponent<Module>();

        /// <summary>
        /// 所属的设备
        /// </summary>
        public Device device
        {
            get
            {
                var m = module;
                if (m)
                {
                    return (m is Device d) ? d : m.device;
                }

                return null;
            }
        }

        /// <summary>
        /// 关联交互零件
        /// </summary>
        public virtual Tools.Part interactPart
        {
            get
            {
#if UNITY_EDITOR
                _interactPart = go ? go.XGetOrAddComponent<Tools.Part>() : null;
#else
                    if (!_interactPart)
                    {
                        _interactPart = go ? go.XGetOrAddComponent<Tools.Part>() : null;
                    }
#endif
                return _interactPart;
            }
        }
        private Tools.Part _interactPart;

        /// <summary>
        /// 替换零件种类标签
        /// </summary>
        public string replacePartTypeTag => interactPart ? interactPart.replacePartTypeTag : "";

        /// <summary>
        /// 零件装配状态
        /// </summary>
        public virtual EAssembleState assembleState
        {
            get => interactPart ? interactPart.assembleState : EAssembleState.None;
            set
            {
                if (interactPart)
                {
                    interactPart.assembleState = value;
                }
            }
        }

#endregion

#region 状态组件方法

        public override void Reset()
        {
            base.Reset();

            // 添加零件组件
            if (interactPart) { }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stateData"></param>
        /// <returns></returns>
        public override bool Init(StateData stateData)
        {
            // 添加零件组件
            if (interactPart) { }

            return base.Init(stateData);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => base.DataValidity() && GetParentItem();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => GetParentItem() ? name : "需放在模块和设备中!";

#endregion
    }
}
