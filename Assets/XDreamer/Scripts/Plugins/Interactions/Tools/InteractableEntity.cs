using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.Tools;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 可交互实体：交互器以其为主入口与可交互组件对象进行交互
    /// </summary>
    [Name("可交互实体")]
    [Tip("交互器以其为主入口与可交互组件对象进行交互", "The interactor interacts with the interactable component object through its main entrance")]
    [DisallowMultipleComponent]
    [Tool("常用", rootType = typeof(ToolsManager))]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager), index = InteractionCategory.InteractableIndex)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public sealed class InteractableEntity : InteractUsageObject, IHoverable, ISelectable, IActivatable, ITool
    {
        #region 悬停

        /// <summary>
        /// 悬停
        /// </summary>
        [Name("悬停")]
        public Hoverable _hoverable = new Hoverable();

        /// <summary>
        /// 能否悬停
        /// </summary>
        public bool canHover => _hoverable.canHover;

        /// <summary>
        /// 是否悬停
        /// </summary>
        public bool isHovered { get => _hoverable.isHovered; set => _hoverable.isHovered = value; }

        #endregion

        #region 选择

        /// <summary>
        /// 选择
        /// </summary>
        [Name("选择")]
        public Selectable _selectable = new Selectable();

        /// <summary>
        /// 能否选择
        /// </summary>
        public bool canSelect => _selectable.canSelect;

        /// <summary>
        /// 是否选择
        /// </summary>
        public bool isSelected { get => _selectable.isSelected; set => _selectable.isSelected = value; }

        #endregion

        #region 激活

        /// <summary>
        /// 激活
        /// </summary>
        [Name("激活")]
        public Activatable _activatable = new Activatable();

        /// <summary>
        /// 能否激活
        /// </summary>
        public bool canActive => _activatable.canActive;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool isActived { get => _activatable.isActived; set => _activatable.isActived = value; }

        #endregion

        #region Unity 消息

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ResetCmd();
        }

        /// <summary>
        /// 重置指令
        /// </summary>
        public void ResetCmd()
        {
            _hoverable._hoverCmds.Reset();
            _selectable._selectCmds.Reset();
            _activatable._activeCmds.Reset();

            RemoveDefaultWorkInteractable();
            AddDefaultWorkInteractable();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            AddDefaultWorkInteractable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            RemoveDefaultWorkInteractable();
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 可交互对象列表
        /// </summary>
        [Readonly]
        public List<InteractableVirtual> _interactables = new List<InteractableVirtual>();

        /// <summary>
        /// 添加缺省工作可交互对象
        /// </summary>
        private void AddDefaultWorkInteractable()
        {
            AddWork(_hoverable);
            AddWork(_selectable);
            AddWork(_activatable);

            _interactables.ForEach(item => AddWork(item));
        }

        /// <summary>
        /// 移除缺省工作可交互对象
        /// </summary>
        private void RemoveDefaultWorkInteractable()
        {
            RemoveWork(_hoverable);
            RemoveWork(_selectable);
            RemoveWork(_activatable);

            _interactables.ForEach(item => RemoveWork(item));
        }

        /// <summary>
        /// 工作工作可交互对象
        /// </summary>
        public List<IInteractable> _workInteractables = new List<IInteractable>();

        /// <summary>
        /// 包含可交互组件
        /// </summary>
        /// <param name="interactable"></param>
        /// <returns></returns>
        public bool ContainInteractable(IInteractable interactable) => interactable != null && (interactable as InteractableEntity == this || _workInteractables.Contains(interactable));

        /// <summary>
        /// 添加工作可交互对象
        /// </summary>
        /// <param name="interactable"></param>
        public void AddWork(IInteractable interactable)
        {
            if (interactable == null || _interactables.Contains(interactable)) return;

            _workInteractables.Add(interactable);
        }

        /// <summary>
        /// 移除工作可交互对象
        /// </summary>
        /// <param name="interactable"></param>
        public void RemoveWork(IInteractable interactable)
        {
            _workInteractables.Remove(interactable);
        }

        /// <summary>
        /// 工作交互器中是否存在某类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsExist<T>() where T : IInteractable
        {
            return _workInteractables.Exists(W => W is T);
        }

        /// <summary>
        /// 获取交互器类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInteractable<T>() where T : IInteractable
        {
            return (T)_workInteractables.Find(W => W is T);
        }

        /// <summary>
        /// 全部命令
        /// </summary>
        public override List<string> cmds
        {
            get
            {
                _cmds.Clear();
                foreach (var item in _workInteractables)
                {
                    _cmds.AddRange(item.cmds);
                }
                return _cmds;
            }
        }
        private List<string> _cmds = new List<string>();

        /// <summary>
        /// 工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData)
        {
            _workCmds.Clear();
            foreach (var item in _workInteractables)
            {
                _workCmds.AddRange(item.GetWorkCmds(interactData));
            }
            return _workCmds;
        }
        private List<string> _workCmds = new List<string>();

        /// <summary>
        /// 经过can校验后的可交互对象
        /// </summary>
        private List<IInteractable> canInteractables = new List<IInteractable>();

        /// <summary>
        /// 作为可交互对象能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteractAsInteractable(InteractData interactData)
        {
            canInteractables.Clear();
            foreach (var interactable in _workInteractables)
            {
                if (interactable == null) continue;

                if (interactable.CanInteractAsInteractable(interactData))
                {
                    canInteractables.Add(interactable);
                }
            }

            return canInteractables.Count > 0;
        }

        /// <summary>
        /// 作为可交互对象尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public override bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            var result = false;
            foreach (var interactable in canInteractables)
            {
                if (interactable.TryInteractAsInteractable(interactData, out _))
                {
                    result = true;
                }
            }

            canInteractables.Clear();
            interactResult = EInteractResult.Finished;
            return result;
        }

        private void OnDrawGizmos()
        {
            if (CommonFun.GetBounds(out var bounds, transform, true, false, false))
            {
                var size = bounds.size / 2;
                var min = Mathf.Min(new float[] { size.x, size.y, size.z });
                Gizmos.DrawWireSphere(transform.position, min);
            }
        }

        #endregion
    }

    #region 可悬停

    /// <summary>
    /// 可悬停接口
    /// </summary>
    public interface IHoverable : IInteractable
    {
        /// <summary>
        /// 能否悬停
        /// </summary>
        bool canHover { get; }

        /// <summary>
        /// 是否悬停
        /// </summary>
        bool isHovered { get; set; }
    }

    /// <summary>
    /// 悬停枚举
    /// </summary>
    public enum EHoverCmd
    {
        /// <summary>
        /// 悬停进入
        /// </summary>
        [Name("悬停进入")]
        Entry,

        /// <summary>
        /// 悬停停留
        /// </summary>
        [Name("悬停停留")]
        Stay,

        /// <summary>
        /// 悬停退出
        /// </summary>
        [Name("悬停退出")]
        Exit,
    }

    /// <summary>
    /// 悬停命令
    /// </summary>
    [Serializable]
    public class HoverCmd : Cmd<EHoverCmd> { }

    /// <summary>
    /// 悬停命令列表
    /// </summary>
    [Serializable]
    public class HoverCmds : Cmds<EHoverCmd, HoverCmd> { }

    /// <summary>
    /// 可悬停
    /// </summary>
    [Serializable]
    public class Hoverable : IHoverable
    {
        /// <summary>
        /// 可悬停
        /// </summary>
        [Name("可悬停")]
        public bool _hoverable = true;

        /// <summary>
        /// 悬停命令
        /// </summary>
        [Name("悬停命令")]
        [OnlyMemberElements]
        [HideInSuperInspector(nameof(_hoverable), EValidityCheckType.False)]
        public HoverCmds _hoverCmds = new HoverCmds();

        /// <summary>
        /// 是否悬停
        /// </summary>
        public bool isHovered
        {
            get => _isHovered;
            set => _isHovered = value;
        }

        /// <summary>
        /// 是否悬停
        /// </summary>
        [Readonly]
        [Name("已悬停")]
        [HideInSuperInspector(nameof(_hoverable), EValidityCheckType.False)]
        public bool _isHovered = false;

        /// <summary>
        /// 能否悬停
        /// </summary>
        public bool canHover => _hoverable;

        /// <summary>
        /// 命令列表
        /// </summary>
        public List<string> cmds => _hoverCmds.cmdNames;

        /// <summary>
        /// 获取工作命令列表
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public List<string> GetWorkCmds(InteractData interactData)
        {
            return isHovered ? _hoverCmds.GetCmdNames(EHoverCmd.Stay, EHoverCmd.Exit) : _hoverCmds.GetCmdNames(EHoverCmd.Entry);
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => nameof(Hoverable); set => throw new NotImplementedException(); }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanInteractAsInteractable(InteractData interactData) => interactData != null && canHover && GetWorkCmds(interactData).Contains(interactData.cmdName);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            interactResult = EInteractResult.Finished;

            if (_hoverCmds.TryGetECmd(interactData.cmdName, out var ecmd))
            {
                if (ecmd == EHoverCmd.Entry)
                {
                    isHovered = true;
                }
                else if (ecmd == EHoverCmd.Exit)
                {
                    isHovered = false;
                }

                return true;
            }

            return false;
        }
    }

    #endregion

    #region 可选择

    /// <summary>
    /// 可选择
    /// </summary>
    public interface ISelectable : IInteractable
    {
        /// <summary>
        /// 能否选择
        /// </summary>
        bool canSelect { get; }

        /// <summary>
        /// 是否选择
        /// </summary>
        bool isSelected { get; set; }
    }

    /// <summary>
    /// 选择命令枚举
    /// </summary>
    public enum ESelectCmd
    {
        /// <summary>
        /// 选择
        /// </summary>
        [Name("选择")]
        Select,

        /// <summary>
        /// 取消选择
        /// </summary>
        [Name("取消选择")]
        Unselect,
    }

    /// <summary>
    /// 选择命令
    /// </summary>
    [Serializable]
    public class SelectCmd : Cmd<ESelectCmd> { }

    /// <summary>
    /// 选择命令列表
    /// </summary>
    [Serializable]
    public class SelectCmds : Cmds<ESelectCmd, SelectCmd> { }

    /// <summary>
    /// 可选
    /// </summary>
    [Serializable]
    public class Selectable : ISelectable
    {
        /// <summary>
        /// 可选择
        /// </summary>
        [Name("可选择")]
        public bool _selectable = true;

        /// <summary>
        /// 选择命令
        /// </summary>
        [Name("选择命令")]
        [OnlyMemberElements]
        [HideInSuperInspector(nameof(_selectable), EValidityCheckType.False)]
        public SelectCmds _selectCmds = new SelectCmds();

        /// <summary>
        /// 是否选择
        /// </summary>
        [Readonly]
        [Name("已选择")]
        [HideInSuperInspector(nameof(_selectable), EValidityCheckType.False)]
        public bool _isSelected;

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool isSelected
        {
            get => _isSelected;
            set => _isSelected = value;
        }

        /// <summary>
        /// 能否选择
        /// </summary>
        public bool canSelect => _selectable;

        /// <summary>
        /// 命令列表
        /// </summary>
        public List<string> cmds => _selectCmds.cmdNames;

        /// <summary>
        /// 工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public List<string> GetWorkCmds(InteractData interactData) => _selectCmds.GetCmdNames(isSelected ? ESelectCmd.Unselect : ESelectCmd.Select);

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => nameof(Selectable); set => throw new NotImplementedException(); }

        /// <summary>
        /// 可交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanInteractAsInteractable(InteractData interactData) => interactData != null && canSelect && GetWorkCmds(interactData).Contains(interactData.cmdName);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            interactResult = EInteractResult.Finished;

            if (_selectCmds.TryGetECmd(interactData.cmdName, out var ecmd))
            {
                switch (ecmd)
                {
                    case ESelectCmd.Select:
                        {
                            isSelected = true;
                            Selection.Add(interactData.interactable.gameObject);
                            return true;
                        }
                    case ESelectCmd.Unselect:
                        {
                            isSelected = false;
                            Selection.Remove(interactData.interactable.gameObject);
                            return true;
                        }
                }
            }

            return false;
        }
    }

    #endregion

    #region 可激活

    /// <summary>
    /// 可交互状态接口：用于只有激活和非激活两种状态
    /// </summary>
    public interface IActivatable : IInteractable
    {
        /// <summary>
        /// 能否激活
        /// </summary>
        bool canActive { get; }

        /// <summary>
        /// 可交互对象的状态
        /// </summary>
        bool isActived { get; set; }
    }

    /// <summary>
    /// 激活命令枚举
    /// </summary>
    public enum EActiveCmd
    {
        /// <summary>
        /// 激活
        /// </summary>
        [Name("激活")]
        Active,

        /// <summary>
        /// 非激活
        /// </summary>
        [Name("非激活")]
        Deactive,
    }

    /// <summary>
    /// 激活命令
    /// </summary>
    [Serializable]
    public class ActiveCmd : Cmd<EActiveCmd> { }

    /// <summary>
    /// 激活命令列表
    /// </summary>
    [Serializable]
    public class ActiveCmds : Cmds<EActiveCmd, ActiveCmd> { }

    /// <summary>
    /// 可激活
    /// </summary>
    [Serializable]
    public class Activatable : IActivatable
    {
        /// <summary>
        /// 可激活
        /// </summary>
        [Name("可激活")]
        public bool _activatable = false;

        /// <summary>
        /// 选择替代对象
        /// </summary>
        [Name("选择替代对象")]
        public InteractableEntity _activatableInteractableOverride;

        /// <summary>
        /// 激活命令
        /// </summary>
        [Name("激活命令")]
        [OnlyMemberElements]
        [HideInSuperInspector(nameof(_activatable), EValidityCheckType.False)]
        public ActiveCmds _activeCmds = new ActiveCmds();

        /// <summary>
        /// 已激活
        /// </summary>
        [Readonly]
        [Name("已激活")]
        [HideInSuperInspector(nameof(_activatable), EValidityCheckType.False)]
        public bool _isActived;

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool isActived
        {
            get => _isActived;
            set => _isActived = value;
        }

        /// <summary>
        /// 能否激活
        /// </summary>
        public bool canActive => _activatable;

        /// <summary>
        /// 命令列表
        /// </summary>
        public List<string> cmds => _activeCmds.cmdNames;

        /// <summary>
        /// 工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public List<string> GetWorkCmds(InteractData interactData) => _activeCmds.GetCmdNames(isActived ? EActiveCmd.Deactive : EActiveCmd.Active);

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get => nameof(Activatable); set => throw new NotImplementedException(); }

        /// <summary>
        /// 可交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public bool CanInteractAsInteractable(InteractData interactData) => interactData != null && canActive && GetWorkCmds(interactData).Contains(interactData.cmdName);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactResult"></param>
        /// <returns></returns>
        public bool TryInteractAsInteractable(InteractData interactData, out EInteractResult interactResult)
        {
            interactResult = EInteractResult.Finished;

            if (_activeCmds.TryGetECmd(interactData.cmdName, out var ecmd))
            {
                switch (ecmd)
                {
                    case EActiveCmd.Active:
                        {
                            isActived = true;
                            return true;
                        }
                    case EActiveCmd.Deactive:
                        {
                            isActived = false;
                            return true;
                        }
                }
            }
            return false;
        }
    }

    #endregion
}
