using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 匹配规则
    /// </summary>
    public enum EInteractMatchRule
    {
        /// <summary>
        /// 总是成立
        /// </summary>
        [Name("总是成立")]
        Always,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        #region 交互器

        [Name("交互器相等")]
        InteractorEqual,

        [Name("交互器相等/且命令相等")]
        InteractorEqual_And_CmdEqual,

        [Name("交互器相等/或命令相等")]
        InteractorEqual_Or_CmdEqual,

        [Name("交互器相等/且可交互对象相等")]
        InteractorEqual_And_InteractableEqual,

        [Name("交互器相等/或可交互对象相等")]
        InteractorEqual_Or_InteractableEqual,

        [Name("交互器相等/且命令相等/且可交互对象相等")]
        InteractorEqual_And_CmdEqual_And_InteractableEqual,

        [Name("交互器相等/或命令相等/或可交互对象相等")]
        InteractorEqual_Or_CmdEqual_Or_InteractableEqual,

        #endregion

        #region 命令

        [Name("命令相等")]
        CmdEqual = 10000,

        [Name("命令相等/且可交互对象相等")]
        CmdEqual_And_InteractableEqual,

        [Name("命令相等/或可交互对象相等")]
        CmdEqual_Or_InteractableEqual,

        [Name("命令相等/且交互器不相等")]
        Cmd_And_InteractorNotEqual,

        #endregion

        #region 可交互对象

        [Name("可交互对象相等")]
        InteractableEqual = 20000,

        #endregion
    }

    /// <summary>
    /// 基础交互比较器
    /// </summary>
    [Serializable]
    public abstract class BaseInteractComparer 
    {
        /// <summary>
        /// 交互状态
        /// </summary>
        [Name("交互状态")]
        [EnumPopup]
        public EInteractState _interactState = EInteractState.Finished;

        /// <summary>
        /// 匹配规则
        /// </summary>
        [Name("匹配规则")]
        [EnumPopup]
        public EInteractMatchRule _matchRule = EInteractMatchRule.InteractorEqual_And_CmdEqual;

        /// <summary>
        /// 交互器
        /// </summary>
        protected abstract IInteractor interactor { get; }

        /// <summary>
        /// 可交互对象
        /// </summary>
        protected abstract IInteractable interactable { get; }

        /// <summary>
        /// 命令相等
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected abstract bool CmdEquals(InteractData interactData);

        private bool InteractorEquals(IInteractor interactor) => interactor != null && interactor == this.interactor;

        private bool InteractableEquals(IInteractable interactable) => interactable != null && interactable == this.interactable;

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool IsMatch(InteractData interactData) => IsMatch(interactData.interactor, interactData);

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="interactor"></param>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool IsMatch(InteractObject interactor, InteractData interactData)
        {
            if (interactData == null || _interactState != interactData.interactState) return false;

            switch (_matchRule)
            {
                case EInteractMatchRule.Always: return true;
                case EInteractMatchRule.None: return false;
                case EInteractMatchRule.InteractorEqual:
                    {
                        return InteractorEquals(interactor);
                    }
                case EInteractMatchRule.InteractorEqual_And_CmdEqual:
                    {
                        return InteractorEquals(interactor) && CmdEquals(interactData);
                    }
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual:
                    {
                        return InteractorEquals(interactor) || CmdEquals(interactData);
                    }
                case EInteractMatchRule.InteractorEqual_And_InteractableEqual:
                    {
                        return InteractorEquals(interactor) && InteractableEquals(interactData.interactable);
                    }
                case EInteractMatchRule.InteractorEqual_Or_InteractableEqual:
                    {
                        return InteractorEquals(interactor) || InteractableEquals(interactData.interactable);
                    }
                case EInteractMatchRule.InteractorEqual_And_CmdEqual_And_InteractableEqual:
                    {
                        return InteractorEquals(interactor) && CmdEquals(interactData) && InteractableEquals(interactData.interactable);
                    }
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual_Or_InteractableEqual:
                    {
                        return InteractorEquals(interactor) || CmdEquals(interactData) || InteractableEquals(interactData.interactable);
                    }
                case EInteractMatchRule.CmdEqual:
                    {
                        return CmdEquals(interactData);
                    }
                case EInteractMatchRule.CmdEqual_And_InteractableEqual:
                    {
                        return CmdEquals(interactData) && InteractableEquals(interactData.interactable);
                    }
                case EInteractMatchRule.CmdEqual_Or_InteractableEqual:
                    {
                        return CmdEquals(interactData) || InteractableEquals(interactData.interactable);
                    }
                case EInteractMatchRule.Cmd_And_InteractorNotEqual:
                    {
                        return CmdEquals(interactData) && !InteractorEquals(interactor);
                    }
                case EInteractMatchRule.InteractableEqual:
                    {
                        return InteractableEquals(interactData.interactable);
                    }
                default: return false;
            }
        }
    }

    /// <summary>
    /// 交互比较器：比较命令字符串、交互器和可交互对象是否匹配
    /// </summary>
    [Serializable]
    public class InteractComparer : BaseInteractComparer
    {
        /// <summary>
        /// 交互器
        /// </summary>
        [Name("交互器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InteractObject _interactor;

        /// <summary>
        /// 输入交互器
        /// </summary>
        public InteractObject inputInteractor => _interactor;

        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("命令名称")]
        public StringPropertyValue _cmdName = new StringPropertyValue();

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("可交互对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InteractableEntity _interactableEntity = null;

        /// <summary>
        /// 交互器
        /// </summary>
        protected override IInteractor interactor => _interactor;

        /// <summary>
        /// 可交互对象
        /// </summary>
        protected override IInteractable interactable => _interactableEntity;

        /// <summary>
        /// 命令相等
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override bool CmdEquals(InteractData interactData) => !string.IsNullOrEmpty(interactData.cmdName) && interactData.cmdName == _cmdName.GetValue();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            switch (_matchRule)
            {
                case EInteractMatchRule.Always:
                case EInteractMatchRule.None: return true;
                case EInteractMatchRule.InteractorEqual: return _interactor;
                case EInteractMatchRule.InteractorEqual_And_CmdEqual: return _interactor && cmdNameValid;
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual: return _interactor || cmdNameValid;
                case EInteractMatchRule.InteractorEqual_And_InteractableEqual: return _interactor && _interactableEntity;
                case EInteractMatchRule.InteractorEqual_Or_InteractableEqual: return _interactor || _interactableEntity;
                case EInteractMatchRule.InteractorEqual_And_CmdEqual_And_InteractableEqual: return _interactor && cmdNameValid && _interactableEntity;
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual_Or_InteractableEqual: return _interactor || cmdNameValid || _interactableEntity;
                case EInteractMatchRule.CmdEqual: return cmdNameValid;
                case EInteractMatchRule.CmdEqual_And_InteractableEqual: return cmdNameValid && _interactableEntity;
                case EInteractMatchRule.CmdEqual_Or_InteractableEqual: return cmdNameValid || _interactableEntity;
                case EInteractMatchRule.Cmd_And_InteractorNotEqual: return cmdNameValid && !_interactor;
                case EInteractMatchRule.InteractableEqual: return _interactableEntity;
                default: return false;
            }
        }

        private bool cmdNameValid => !string.IsNullOrEmpty(_cmdName.GetValue());
    }

    /// <summary>
    /// 交互比较器模版：比较命令枚举值（指定类型）、交互器（指定类型）和可交互对象（指定类型）是否匹配
    /// </summary>
    /// <typeparam name="TECmd"></typeparam>
    /// <typeparam name="TInteractor"></typeparam>
    /// <typeparam name="TInteractable"></typeparam>
    public class InteractComparer<TECmd, TInteractor, TInteractable> : BaseInteractComparer
        where TECmd : Enum
        where TInteractor : InteractObject
        where TInteractable : InteractObject
    {
        /// <summary>
        /// 命令
        /// </summary>
        [Name("命令")]
        [EnumPopup]
        public TECmd _cmd;

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("可交互对象")]
        [ComponentPopup]
        public TInteractable _interactable;

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("交互器")]
        [ComponentPopup]
        public TInteractor _interactor;

        /// <summary>
        /// 交互器
        /// </summary>
        protected override IInteractor interactor => _interactor;

        /// <summary>
        /// 可交互组件
        /// </summary>
        protected override IInteractable interactable => _interactable;

        /// <summary>
        /// 命令相等
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override bool CmdEquals(InteractData interactData)
        {
            return _cmd.Equals(interactData.cmdEnum);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity()
        {
            switch (_matchRule)
            {
                case EInteractMatchRule.Always:
                case EInteractMatchRule.None: return true;
                case EInteractMatchRule.InteractorEqual: 
                case EInteractMatchRule.InteractorEqual_And_CmdEqual: 
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual: return _interactor;
                case EInteractMatchRule.InteractorEqual_And_InteractableEqual: 
                case EInteractMatchRule.InteractorEqual_And_CmdEqual_And_InteractableEqual: return _interactor && _interactable;
                case EInteractMatchRule.InteractorEqual_Or_InteractableEqual: 
                case EInteractMatchRule.InteractorEqual_Or_CmdEqual_Or_InteractableEqual: return _interactor || _interactable;
                case EInteractMatchRule.CmdEqual: return true;
                case EInteractMatchRule.CmdEqual_And_InteractableEqual: 
                case EInteractMatchRule.CmdEqual_Or_InteractableEqual: 
                case EInteractMatchRule.InteractableEqual: return _interactable;
                case EInteractMatchRule.Cmd_And_InteractorNotEqual: return !_interactor;
                default: return false;
            }
        }
    }

    /// <summary>
    /// 交互信息类
    /// </summary>
    [Serializable]
    public class InteractInfo
    {
        /// <summary>
        /// 交互器
        /// </summary>
        [Name("交互器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InteractObject _interactor;

        /// <summary>
        /// 输入交互器
        /// </summary>
        public InteractObject inputInteractor => _interactor;

        /// <summary>
        /// 命令名称
        /// </summary>
        [Name("命令名称")]
        public StringPropertyValue _cmdName = new StringPropertyValue();

        /// <summary>
        /// 可交互对象
        /// </summary>
        [Name("可交互对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public InteractableEntity _interactableEntity = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public InteractInfo() { }

        /// <summary>
        /// 执行交互
        /// </summary>
        /// <returns></returns>
        public bool TryInteract(out EInteractResult interactResult)
        {
            if (_interactor && _cmdName.TryGetValue(out var cmd))
            {
                return _interactor.TryInteract(cmd, out interactResult, _interactableEntity);
            }
            interactResult = default;
            return false;
        }
    }
}
