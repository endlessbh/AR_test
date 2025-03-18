using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Tools;
using static XCSJ.PluginTools.Events.InteractEventHandle;

namespace XCSJ.PluginTools.Events
{
    /// <summary>
    /// 交互事件处理器:监听交互事件，比较器匹配后执行UnityEvent
    /// </summary>
    [Name("交互事件处理器")]
    [Tip("监听交互事件，比较匹配后执行UnityEvent", "Listen for interactive events, and execute UnityEvent after comparing and matching")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    [Tool(InteractionCategory.InteractCommon, nameof(InteractableVirtual), rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class InteractEventHandle : Interactor<InteractUnityEventExecuter>
    {
        /// <summary>
        /// 交互Unity事件数据
        /// </summary>
        [Serializable]
        public class InteractUnityEventExecuter : InteractComparer, IInteractInput
        {
            /// <summary>
            /// 交互Unity事件
            /// </summary>
            [Name("交互Unity事件")]
            public InteractUnityEvent _interactUnityEvent = new InteractUnityEvent();

            /// <summary>
            /// 能否处理
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="sender"></param>
            /// <param name="interactData"></param>
            /// <returns></returns>
            public bool CanHandle(InteractObject owner, InteractObject sender, InteractData interactData)
            {
                return IsMatch(interactData);
            }

            /// <summary>
            /// 执行Unity定义事件
            /// </summary>
            /// <param name="interactor"></param>
            /// <param name="interactData"></param>
            /// <returns></returns>
            public InteractData Handle(InteractObject interactor, InteractData interactData)
            {
                _interactUnityEvent.Invoke(interactData);
                return interactData;
            }
        }
    }

    /// <summary>
    /// 交互Unity事件
    /// </summary>
    [Serializable]
    public class InteractUnityEvent : UnityEvent<InteractData> { }
}
