using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Items
{
    /// <summary>
    /// 切换命令枚举值
    /// </summary>
    public enum ESwitchCmd
    {
        [Name("开")]
        On,

        [Name("关")]
        Off,

        [Name("切换")]
        Switch
    }

    /// <summary>
    /// 切换命令
    /// </summary>
    [Serializable]
    public class SwitchCmd : Cmd<ESwitchCmd> { }

    /// <summary>
    /// 切换命令列表
    /// </summary>
    [Serializable]
    public class SwitchCmds : Cmds<ESwitchCmd, SwitchCmd> { }

    /// <summary>
    /// 可开关交互对象
    /// </summary>
    public abstract class Switchable : InteractableVirtual
    {
        /// <summary>
        /// 开关态
        /// </summary>
        public virtual bool isOn
        {
            get => _isOn;
            set
            {
                if (_isOn != value)
                {
                    _isOn = value;

                    onValueChanged?.Invoke(this);
                }
            }
        }
        [Readonly]
        public bool _isOn;

        public virtual void Reset() => _switchCmds.Reset();

        /// <summary>
        /// 尝试切换
        /// </summary>
        /// <param name="context"></param>
        /// <param name="interactor"></param>
        /// <param name="switchFinished"></param>
        public virtual void Switch()
        {
            isOn = !isOn;
        }

        /// <summary>
        /// 值变化回调
        /// </summary>
        public static event Action<Switchable> onValueChanged;

        [Name("开关命令")]
        [OnlyMemberElements]
        public SwitchCmds _switchCmds = new SwitchCmds();

        /// <summary>
        /// 全部命令
        /// </summary>
        public override List<string> cmds => _switchCmds.cmdNames;

        /// <summary>
        /// 可工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData)
        {
            return _switchCmds.GetCmdNames(isOn ? ESwitchCmd.Off : ESwitchCmd.On, ESwitchCmd.Switch);
        }

        /// <summary>
        /// 执行交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteractAsInteractable(InteractData interactData)
        {
            if (_switchCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case ESwitchCmd.On: if(!isOn) Switch(); return EInteractResult.Finished;
                    case ESwitchCmd.Off: if (isOn) Switch(); return EInteractResult.Finished;
                    case ESwitchCmd.Switch: Switch(); return EInteractResult.Finished;
                }
            }
            return EInteractResult.Aborted;
        }
    }

}
