using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Draggers.TRSHandles;
using XCSJ.PluginTools.Items;

namespace XCSJ.PluginTools.SelectionUtils
{
    #region 选择命令

    /// <summary>
    /// 选择命令
    /// </summary>
    public enum ESelectorCmd
    {
        [Name("选择")]
        Select,

        [Name("取消选择")]
        Unselect,

        [Name("清空选择")]
        Clear,
    }

    /// <summary>
    /// 选择命令
    /// </summary>
    [Serializable]
    public class SelectorCmd : Cmd<ESelectorCmd> { }

    /// <summary>
    /// 选择命令列表
    /// </summary>
    [Serializable]
    public class SelectorCmds : Cmds<ESelectorCmd, SelectorCmd> { }

    #endregion

    /// <summary>
    /// 选择集修改
    /// </summary>
    [Name("选择集修改器")]
    [Tip("通过鼠标点击、触摸点击实现基于游戏对象选择集的修改", "The modification based on game object selection set is realized by mouse click and touch click")]
    [XCSJ.Attributes.Icon(EIcon.Select)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    [Tool("选择集", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager), index = InteractionCategory.InteractorIndex)]
    public class SelectionModify : SelectionListener
    {
        #region 交互输入

        /// <summary>
        /// 选择命令列表
        /// </summary>
        [Name("选择命令列表")]
        [OnlyMemberElements]
        public SelectorCmds _selectorCmds = new SelectorCmds();

        /// <summary>
        /// 全部交互命令
        /// </summary>
        public override List<string> cmds => _selectorCmds.cmdNames;

        #endregion

        #region Unity 消息

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            // 禁用时尝试取消当前选择
            UnselectCurrent();
        }

        #endregion

        #region 交互处理

        /// <summary>
        /// 能否交互
        /// </summary>
        public bool canInteract { get; set; } = true;

        /// <summary>
        /// 当前选中游戏对象名称：用于界面绑定的属性对象
        /// </summary>
        public string selectedGameObjectName => _currentSelection ? _currentSelection.name : "";

        /// <summary>
        /// 当前选择对象
        /// </summary>
        public GameObject currentSelection => _currentSelection;

        /// <summary>
        /// 当前选中游戏对象
        /// </summary>
        [Readonly]
        public GameObject _currentSelection;

        /// <summary>
        /// 选择
        /// </summary>
        public void Select(GameObject gameObject) => TryInteractInternal(ESelectorCmd.Select, gameObject);

        /// <summary>
        /// 取消选择
        /// </summary>
        /// <param name="gameObject"></param>
        public void UnSelect(GameObject gameObject) => TryInteractInternal(ESelectorCmd.Unselect, gameObject);

        /// <summary>
        /// 取消选择当前记录的对象
        /// </summary>
        public void UnselectCurrent() => UnSelect(_currentSelection);

        /// <summary>
        /// 能否交互：用于将选择交互器指令转为真正可交互对象的指令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData)
        {
            if (!canInteract) return false;

            // 如果为清除选择集或交互对象为空,执行取消选择操作
            if (_selectorCmds.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case ESelectorCmd.Select:
                        {
                            // 输入可交互对象为不等于当前对象或输入可交互对象为空时, 对当前对象执行【取消选择】命令
                            if (_currentSelection)
                            {
                                var component = interactData.interactable;
                                if (component)
                                {
                                    if (component.gameObject != _currentSelection)
                                    {
                                        UnselectCurrent();
                                    }
                                }
                                else
                                {
                                    UnselectCurrent();
                                }
                            }
                            break;
                        }
                    case ESelectorCmd.Clear: return _currentSelection;
                }
            }

            // 使用基类的过滤规则
            return base.CanInteract(interactData);
        }

        /// <summary>
        /// 单一交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected override bool OnInteractSingle(InteractData interactData, InteractObject interactable)
        {
            if (interactData.cmdEnum is ESelectorCmd eCmd || _selectorCmds.TryGetECmd(interactData.cmdName, out eCmd))
            {
                switch (eCmd)
                {
                    case ESelectorCmd.Select:
                        {
                            var component = interactData.interactable;
                            if (component)
                            {
                                _currentSelection = component.gameObject;
                            }
                            break;
                        }
                    case ESelectorCmd.Unselect:
                        {
                            var component = interactData.interactable;
                            if (component && component.gameObject == _currentSelection)
                            {
                                _currentSelection = null;
                            }
                            break;
                        }
                    case ESelectorCmd.Clear:
                        {
                            // 当前记录的选择对象不为空时，对当前对象执行【取消选择】命令
                            UnselectCurrent();
                            break;
                        }
                }
            }

            return base.OnInteractSingle(interactData, interactable);
        }

        private bool TryInteractInternal(ESelectorCmd selectorCmd, GameObject gameObject)
        {
            if (!gameObject) return false;

            var s = gameObject.GetComponent<ISelectable>() as InteractObject;
            return s ? TryInteract(_selectorCmds.GetCmdName(selectorCmd), out _, s) : false;
        } 

        #endregion
    }
}