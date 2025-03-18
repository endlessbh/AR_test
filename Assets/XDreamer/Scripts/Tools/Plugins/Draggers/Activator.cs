using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 激活器
    /// </summary>
    [Name("激活器")]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager), index = InteractionCategory.InteractorIndex)]
    public class Activator : Interactor
    {
        /// <summary>
        /// 全部交互命令
        /// </summary>
        public override List<string> cmds => _activeCmds.cmdNames;

        /// <summary>
        /// 当前工作交互命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData)
        {
            return cmds;
        }

        /// <summary>
        /// 激活命令列表
        /// </summary>
        [Name("激活命令列表")]
        [OnlyMemberElements]
        public ActiveCmds _activeCmds = new ActiveCmds();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _activeCmds.Reset();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            return _activeCmds.Exists(interactData.cmdName) && base.CanInteract(interactData);
        }

        /// <summary>
        /// 处理交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected override bool OnInteractSingle(InteractData interactData, InteractObject interactable)
        {
            if (_activeCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EActiveCmd.Active:
                        {
                            if (interactable is IActivatable activeInteractable)
                            {
                                return !activeInteractable.isActived;
                            }
                            break;
                        }
                    case EActiveCmd.Deactive:
                        {
                            if (interactable is IActivatable activeInteractable)
                            {
                                return activeInteractable.isActived;
                            }
                            break;
                        }
                }
            }

            return false;
        }
    }
}
