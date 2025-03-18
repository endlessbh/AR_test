using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Interactions.Tools
{
    #region 交互器实体

    /// <summary>
    /// 交互器实体
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class Interactor<TInteractorInput> : InteractUsageObject, IInteractInputer where TInteractorInput : class, IInteractInput, new()
    {
        /// <summary>
        /// 交互输入器
        /// </summary>
        public override IInteractInputer interactInputer => this;

        /// <summary>
        /// 交互器输入列表
        /// </summary>
        [Name("交互器输入列表")]
        [FormerlySerializedAs("_interactorInputs")]
        public List<TInteractorInput> _interactInputs = new List<TInteractorInput>();

        /// <summary>
        /// 交互器输入列表
        /// </summary>
        public IEnumerable<IInteractInput> interactInputs => _interactInputs;
    }

    /// <summary>
    /// 交互器实体
    /// </summary>
    public abstract class Interactor : Interactor<InteractorInput> { }

    #endregion

    #region 交互输入

    /// <summary>
    /// 交互输入
    /// </summary>
    [Serializable]
    public class InteractorInput : InteractComparer, IInteractInput
    {
        /// <summary>
        /// 匹配处理规则
        /// </summary>
        public enum EMatchHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 替换命令
            /// </summary>
            [Name("替换命令")]
            ReplaceCmd,
        }

        /// <summary>
        /// 匹配后处理
        /// </summary>
        [Name("匹配后处理")]
        [EnumPopup]
        public EMatchHandleRule _matchHandleRule = EMatchHandleRule.ReplaceCmd;

        /// <summary>
        /// 替换命令名称
        /// </summary>
        [Name("替换命令名称")]
        public StringPropertyValue _repalceCmdName = new StringPropertyValue();

        /// <summary>
        /// 能否处理
        /// </summary>
        /// <param name="owner">当前对象所属的交互器</param>
        /// <param name="sender">产生交互数据的交互器</param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanHandle(InteractObject owner, InteractObject sender, InteractData interactData)
        {
            return IsMatch(sender, interactData);
        }

        /// <summary>
        /// 尝试处理
        /// </summary>
        /// <param name="interactor">产生交互数据的交互器</param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public InteractData Handle(InteractObject interactor, InteractData interactData)
        {
            if (interactData == null) return null;

            switch (_matchHandleRule)
            {
                case EMatchHandleRule.ReplaceCmd:
                    {
                        if (_repalceCmdName.TryGetValue(out var value))
                        {
                            interactData.SetCmd(value);
                        }
                        break;
                    }
            }

            return interactData;
        }

    }

    #endregion
}
