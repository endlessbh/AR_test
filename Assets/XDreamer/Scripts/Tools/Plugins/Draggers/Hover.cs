using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Draggers
{
    /// <summary>
    /// 悬停器
    /// </summary>
    [Name("悬停器")]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager), index = InteractionCategory.InteractorIndex)]
    [XCSJ.Attributes.Icon(EIcon.GameObjectActive)]
    public class Hover : Interactor
    {
        /// <summary>
        /// 全部交互命令
        /// </summary>
        public override List<string> cmds => _hoverCmds.cmdNames;

        /// <summary>
        /// 当前工作交互命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData)
        {
            return _hoverCmds.GetCmdNames(interactData.interactable == null ? EHoverCmd.Exit : (stayInteractable == interactData.interactable ? EHoverCmd.Stay : EHoverCmd.Entry));
        }

        /// <summary>
        /// 悬停命令列表
        /// </summary>
        [Name("悬停命令列表")]
        [OnlyMemberElements]
        public HoverCmds _hoverCmds = new HoverCmds();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _hoverCmds.Reset();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // 禁用时执行退出悬停命令
            TryExitHover();
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            // 传入命令等于悬停命令
            if (_hoverCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                return base.CanInteract(interactData);
            }
            else // 传入命令非悬停命令，则执行命令替换处理
            {
                var current = interactData.interactable;

                // 传入空对象，则退出停留对象
                if (current == null)
                {
                    if (stayInteractable != null)
                    {
                        TryExitHover(interactData);
                        stayInteractable = null;
                        return true;
                    }
                    return false;
                }
                else
                {
                    var enumCmd = stayInteractable == current ? EHoverCmd.Stay : EHoverCmd.Entry;
                    interactData.SetCmd(_hoverCmds.GetCmdName(enumCmd), this, enumCmd);

                    return base.CanInteract(interactData);
                }
            }
        }

        /// <summary>
        /// 处理交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected override bool OnInteractSingle(InteractData interactData, InteractObject interactable)
        {
            if (_hoverCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case EHoverCmd.Entry:
                        {
                            // 首先尝试退出停留对象
                            if (stayInteractable != null)
                            {
                                TryExitHover(interactData);
                                stayInteractable = null;
                            }

                            stayInteractable = interactable;
                            break;
                        }
                    case EHoverCmd.Stay:
                        {
                            //Debug.Log("悬停停留:" + interactable.name);
                            break;
                        }
                    case EHoverCmd.Exit:
                        {
                            if (interactData.interactable == stayInteractable)
                            {
                                stayInteractable = null;
                            }
                            break;
                        }
                }
            }

            return base.OnInteractSingle(interactData, interactable);
        }

        /// <summary>
        /// 只允许一个对象处于悬停状态
        /// </summary>
        private InteractObject stayInteractable
        {
            get => _stayInteractable;
            set => _stayInteractable = value;
        }
        private InteractObject _stayInteractable;

        /// <summary>
        /// 尝试退出悬停
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        private bool TryExitHover(InteractData interactData = null)
        {
            if (stayInteractable != null)
            {
                return TryInteract(new InteractData(EHoverCmd.Exit, this, _hoverCmds.GetCmdName(EHoverCmd.Exit), interactData, this, stayInteractable), out _);
            }
            return false;
        }
    }
}
