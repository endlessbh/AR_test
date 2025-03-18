using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Events
{
    /// <summary>
    /// 自定义命令交互器
    /// </summary>
    [Name("自定义命令交互器")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Tool(InteractionCategory.InteractCommon, rootType = typeof(ToolsManager))]
    public sealed class CustomCmdInteractor : Interactor
    {
        /// <summary>
        /// 命令列表
        /// </summary>
        public override List<string> cmds => _eventDatas.Cast(d => d._cmd).Distinct().ToList();

        /// <summary>
        /// 事件数据列表
        /// </summary>
        [Name("事件数据列表")]
        public List<InteractorUnityEventData> _eventDatas = new List<InteractorUnityEventData>();

        /// <summary>
        /// 交互回调
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="interactable"></param>
        /// <returns></returns>
        protected override bool OnInteractSingle(InteractData interactData, InteractObject interactable) => Invoke(interactData);

        /// <summary>
        /// 调用
        /// </summary>
        /// <param name="interactData"></param>
        private bool Invoke(InteractData interactData)
        {
            var result = false;
            foreach (var data in _eventDatas)
            {
                if (data._cmd == interactData.cmdName)
                {
                    data._interactorUnityEvent.Invoke(interactData);
                    result = true;
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 模式交互器Unity事件数据
    /// </summary>
    [Serializable]
    public class InteractorUnityEventData
    {
        /// <summary>
        /// 命令
        /// </summary>
        [Name("命令")]
        public string _cmd = "";

        /// <summary>
        /// 交互器事件
        /// </summary>
        [Name("交互器事件")]
        public InteractUnityEvent _interactorUnityEvent = new InteractUnityEvent();
    }
}
