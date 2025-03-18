using System;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginTools.Items;
using static XCSJ.PluginTools.Draggers.Dragger;

namespace XCSJ.PluginTools.States
{
    /// <summary>
    /// 拖拽比较器
    /// </summary>
    [Serializable]
    public class DraggerComparer : InteractComparer<EGrabState, Dragger, Grabbable> { }

    /// <summary>
    /// 拖拽器事件
    /// </summary>
    [ComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
    [Name(Title, nameof(DraggerEvent))]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    public class DraggerEvent : Trigger<DraggerEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "拖拽器事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(InteractionCategory.Interact, typeof(ToolsManager))]
        [StateComponentMenu(InteractionCategory.Interact + "/" + Title, typeof(ToolsManager))]
        [Name(Title, nameof(DraggerEvent))]
        [XCSJ.Attributes.Icon(EIcon.Put)]
        public static State CreateCollisionTriggerEvent(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 拖拽比较器
        /// </summary>
        [Name("拖拽比较器")]
        public DraggerComparer _draggerComparer = new DraggerComparer();

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            InteractObject.onInteractFinished += OnInteractFinished;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            InteractObject.onInteractFinished -= OnInteractFinished;
        }

        private void OnInteractFinished(InteractObject interactor, InteractData interactData) => CheckFinish(interactData);

        private void CheckFinish(InteractData interactData)
        {
            if (!finished && interactData.cmdEnum is EGrabState grabState)
            {
                if (_draggerComparer.IsMatch(interactData))
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _draggerComparer.DataValidity();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_draggerComparer._cmd);
    }
}

