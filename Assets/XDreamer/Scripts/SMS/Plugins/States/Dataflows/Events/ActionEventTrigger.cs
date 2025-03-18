using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Events;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.Events
{
    /// <summary>
    /// Action事件触发器:Action事件触发器可用于监听并捕获基于Action类型委托实现的事件
    /// </summary>
    [ComponentMenu(DataflowHelper.Events + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ActionEventTrigger))]
    [Tip("Action事件触发器可用于监听并捕获基于Action类型委托实现的事件", "The action event trigger can be used to listen to and capture events based on action type delegate implementation")]
    [XCSJ.Attributes.Icon(EIcon.Function)]
    public class ActionEventTrigger : Trigger<ActionEventTrigger>, IDropdownPopupAttribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Action事件触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DataflowHelper.Events, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.Events + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(ActionEventTrigger))]
        [Tip("Action事件触发器可用于监听并捕获基于Action类型委托实现的事件", "The action event trigger can be used to listen to and capture events based on action type delegate implementation")]
        [XCSJ.Attributes.Icon(EIcon.Function)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// Action事件监听器
        /// </summary>
        [Name("Action事件监听器")]
        public ActionEventListener actionEventListener = new ActionEventListener();

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            actionEventListener.AddListener(OnEventInvoked);
        }

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            actionEventListener.RemoveListener(OnEventInvoked);
        }

        private void OnEventInvoked(EventListener eventListener, ITupleData tuple)
        {
            finished = true;
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return actionEventListener.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return actionEventListener.DataValidity();
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = ActionEventListener.GetEventFieldNames(actionEventListener.targetType, actionEventListener.bindingFlags, actionEventListener.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = ActionEventListener.GetTypeFullNames(actionEventListener.bindingFlags, actionEventListener.includeBaseType);
                        return true;
                    }
            }
            options = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            option = default;
            return false;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            propertyValue = default;
            return false;
        }
    }
}
