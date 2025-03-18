using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;

namespace XCSJ.CommonUtils.PluginCrossSectionShader
{
    /// <summary>
    /// 可剖切接口
    /// </summary>
    public interface ICuttable : IInteractable { }

    /// <summary>
    /// 可剖切对象
    /// </summary>
    [Name("可剖切对象")]
    [DisallowMultipleComponent]
    [Tool("模型", nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    [Owner(typeof(ToolsManager))]
    public class Cuttable : InteractableVirtual, ICuttable
    {
        /// <summary>
        /// 是否被切割
        /// </summary>
        public bool cutted { get; private set; } = false;

        /// <summary>
        /// 剖切命令
        /// </summary>
        [Name("剖切命令")]
        [OnlyMemberElements]
        public CutCmds _cutCmds = new CutCmds();

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _cutCmds.Reset();
        }

        /// <summary>
        /// 全部命令
        /// </summary>
        public override List<string> cmds => _cutCmds.cmdNames;

        /// <summary>
        /// 工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData) => _cutCmds.GetCmdNames(cutted ? ECutCmd.Recover : ECutCmd.Cut);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteractAsInteractable(InteractData interactData)
        {
            if (_cutCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case ECutCmd.Cut: cutted = true; return EInteractResult.Finished;
                    case ECutCmd.Recover: cutted = false; return EInteractResult.Finished;
                }
            }
            return EInteractResult.Aborted;
        }
    }
}

