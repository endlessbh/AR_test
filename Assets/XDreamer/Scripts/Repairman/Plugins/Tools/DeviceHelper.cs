using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginRepairman.Base;
using XCSJ.PluginRepairman.States;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginTools;
using XCSJ.PluginTools.GameObjects;

namespace XCSJ.PluginRepairman.Tools
{
    /// <summary>
    /// 设备助手：用于指导逐步的拆卸或装配操作
    /// </summary>
    [Name("设备助手")]
    [Tip("用于指导逐步的拆卸或装配操作", "Used to guide step-by-step disassembly or assembly operations")]
    [XCSJ.Attributes.Icon(EIcon.Click)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(RepairmanManager))]
    [Tool(category = RepairmanHelper.PluginName, purposes = new string[] { nameof(RepairmanManager) }, groupRule = EToolGroupRule.None)]
    [Owner(typeof(RepairmanManager))]
    public sealed class DeviceHelper : InteractProvider
    {
        /// <summary>
        /// 设备
        /// </summary>
        [Name("设备")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Device _device;

        /// <summary>
        /// 设备
        /// </summary>
        public Device device { get => _device; set => _device = value; }

        /// <summary>
        /// 指导装配或拆卸
        /// </summary>
        public bool isGuideAssembly { get; set; } = true;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            if (!_device) _device = UnityObjectExtension.GetComponentInGlobal<Device>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            //PartStateComponent.onPartAssembleStateChanged += OnPartAssembleStateChanged;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            //PartStateComponent.onPartAssembleStateChanged -= OnPartAssembleStateChanged;
        }

        /// <summary>
        /// 开始
        /// </summary>
        private void Start()
        {
            Guide();
        }

        private void OnPartAssembleStateChanged(Part part, EAssembleState assembleState)
        {
            Guide();
        }

        //private Dictionary<PartStateComponent, PartSorket> currentCanAssemblyPartMap = new Dictionary<PartStateComponent, PartSorket>();

        private void Guide()
        {
            if (!device) return;

            if (isGuideAssembly)
            {
                HandleAssemblyVisual();
            }
            else
            {
                HandleDisassemblyVisual();
            }
        }

        private void HandleAssemblyVisual()
        {
            //var parts = device.GetFreeAssemblyParts();
            //if (parts.Count(p => p) == 0)
            //{
            //    GameObjectSocketCache.UnregisterSockets(currentCanAssemblyPartMap.Values);
            //    currentCanAssemblyPartMap.Clear();
            //}
            //else
            //{
            //    foreach (var part in parts)
            //    {
            //        if (!currentCanAssemblyPartMap.ContainsKey(part))
            //        {
            //            var ps = new PartSorket(part, part.gameObject.transform, ESocketMatchRule.DisplaySocketSelfGameObject);
            //            currentCanAssemblyPartMap.Add(part, ps);
            //            GameObjectSocketCache.RegisterSocket(ps);
            //        }
            //    }
            //}
        }

        private void HandleDisassemblyVisual()
        {

        }
    }
}

