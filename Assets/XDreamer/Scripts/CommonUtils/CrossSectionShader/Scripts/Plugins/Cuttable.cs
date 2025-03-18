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
    /// �����нӿ�
    /// </summary>
    public interface ICuttable : IInteractable { }

    /// <summary>
    /// �����ж���
    /// </summary>
    [Name("�����ж���")]
    [DisallowMultipleComponent]
    [Tool("ģ��", nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    [RequireManager(typeof(ToolsManager), typeof(ToolsExtensionManager))]
    [Owner(typeof(ToolsManager))]
    public class Cuttable : InteractableVirtual, ICuttable
    {
        /// <summary>
        /// �Ƿ��и�
        /// </summary>
        public bool cutted { get; private set; } = false;

        /// <summary>
        /// ��������
        /// </summary>
        [Name("��������")]
        [OnlyMemberElements]
        public CutCmds _cutCmds = new CutCmds();

        /// <summary>
        /// ����
        /// </summary>
        public virtual void Reset()
        {
            _cutCmds.Reset();
        }

        /// <summary>
        /// ȫ������
        /// </summary>
        public override List<string> cmds => _cutCmds.cmdNames;

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData) => _cutCmds.GetCmdNames(cutted ? ECutCmd.Recover : ECutCmd.Cut);

        /// <summary>
        /// ���Խ���
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

