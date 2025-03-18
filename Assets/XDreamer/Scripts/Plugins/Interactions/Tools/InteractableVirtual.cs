using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 可交互虚体
    /// </summary>
    [RequireComponent(typeof(InteractableEntity))]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class InteractableVirtual : InteractUsageObject
    {
        /// <summary>
        /// 依赖可交互实体
        /// </summary>
        public InteractableEntity interactableEntity
        {
            get
            {
                if (!_interactableEntity)
                {
                    _interactableEntity = this.XGetOrAddComponent<InteractableEntity>();
                }
                return _interactableEntity;
            }
        }

        private InteractableEntity _interactableEntity;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake() { }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            interactableEntity.AddWork(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            interactableEntity.RemoveWork(this);
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteractAsInteractable(InteractData interactData) => interactData != null && GetWorkCmds(interactData).Contains(interactData.cmdName);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public sealed override bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            interactResult = OnInteractAsInteractable(interactData);
            return interactResult != EInteractResult.Aborted;
        }

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected virtual EInteractResult OnInteractAsInteractable(InteractData interactData) => EInteractResult.Finished;
    }
}
