using System.Collections.Generic;
using UnityEngine.Events;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Events;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.Events
{
    /// <summary>
    /// Unity事件触发器：Unity事件触发器可用于监听并捕获基于<see cref="UnityEventBase"/>实现的事件
    /// </summary>
    [ComponentMenu(DataflowHelper.Events + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(UnityEventTrigger))]
    [Tip("Unity事件触发器可用于监听并捕获基于UnityEventBase实现的事件", "Unity event triggers can be used to listen and capture events based on the implementation of UnityEventBase")]
    [XCSJ.Attributes.Icon(EIcon.Function)]
    public class UnityEventTrigger : Trigger<UnityEventTrigger>, IDropdownPopupAttribute, ITypeBinderGetter
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Unity事件触发器";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(DataflowHelper.Events, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.Events + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(UnityEventTrigger))]
        [XCSJ.Attributes.Icon(EIcon.Function)]
        [Tip("Unity事件触发器可用于监听并捕获基于UnityEventBase实现的事件", "Unity event triggers can be used to listen and capture events based on the implementation of UnityEventBase")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// Unity事件监听器
        /// </summary>
        [Name("Unity事件监听器")]
        public UnityEventListener unityEventListener = new UnityEventListener();

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { unityEventListener };

        #endregion

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            unityEventListener.AddListener(OnEventInvoked);
        }

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            unityEventListener.RemoveListener(OnEventInvoked);
        }

        private void OnEventInvoked()
        {
            finished = true;
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return unityEventListener.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return unityEventListener.DataValidity();
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = UnityEventListener.GetEventFieldNames(unityEventListener.targetType, unityEventListener.bindingFlags, unityEventListener.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = Empty<string>.Array;
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
                        option = "";
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
