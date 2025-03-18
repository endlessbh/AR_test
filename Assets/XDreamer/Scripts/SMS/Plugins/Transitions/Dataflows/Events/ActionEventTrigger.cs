using System;
using System.Collections.Generic;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Events;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Dataflows;
using XCSJ.PluginSMS.Transitions.Base;
using static XCSJ.Extension.Base.Dataflows.Binders.TypeBinder;

namespace XCSJ.PluginSMS.Transitions.Dataflows.Events
{
    /// <summary>
    /// Action事件触发器:可用于监听并捕获基于Action类型委托实现的事件；仅可用于捕获入状态、入状态上组件的Action事件；
    /// </summary>
    [ComponentMenu(DataflowHelper.Events + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(ActionEventTrigger))]
    [Tip("可用于监听并捕获基于Action类型委托实现的事件；仅可用于捕获入状态、入状态上组件的Action事件；", "It can be used to listen to and capture events based on action type delegate implementation; It can only be used to capture the action events of components in and out of the state;")]
    [XCSJ.Attributes.Icon(EIcon.Function)]
    public class ActionEventTrigger : Trigger, IDropdownPopupAttribute, ITypeBinderGetter
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title= "Action事件触发器";

        /// <summary>
        /// Action事件监听器
        /// </summary>
        [Name("Action事件监听器")]
        public ActionEventListener _actionEventListener = new ActionEventListener();

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { _actionEventListener };

        #endregion

        #region 针对Action事件监听器属性的访问优化

        /// <summary>
        /// 类型绑定规则
        /// </summary>
        public EBinderRule typeBindRule
        {
            get => _actionEventListener.typeBindRule;
            set => this.XModifyProperty(ref _actionEventListener._typeBindRule, value);
        }

        /// <summary>
        /// 目标类型全名称
        /// </summary>
        public string targetTypeFullName
        {
            get => _actionEventListener.targetTypeFullName;
            set => this.XModifyProperty(ref _actionEventListener._targetType, value);
        }


        /// <summary>
        /// 目标类型
        /// </summary>
        public Type targetType
        {
            get => _actionEventListener.targetType;
            set => this.XModifyProperty(() => _actionEventListener.targetType = value);
        }

        /// <summary>
        /// 目标
        /// </summary>
        public UnityEngine.Object target
        {
            get => _actionEventListener.target;
            set => this.XModifyProperty(() => _actionEventListener.target = value);
        }

        /// <summary>
        /// 检查参数
        /// </summary>
        public bool checkArguments
        {
            get => _actionEventListener.checkArguments;
            set => this.XModifyProperty(ref _actionEventListener.checkArguments, value);
        }

        #endregion

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            _actionEventListener.AddListener(OnEventInvoked);
        }

        /// <summary>
        /// 进入激活态回调
        /// </summary>
        /// <param name="stateData">状态数据对象</param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            _actionEventListener.RemoveListener(OnEventInvoked);
        }

        private void OnEventInvoked(EventListener eventListener, ITupleData tuple)
        {
            finished = true;
            canTrigger = true;
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return _actionEventListener.ToFriendlyString();
        }

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _actionEventListener.DataValidity();
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute)://成员名称
                    {
                        //nameof(ActionEventListener._memberName)
                        options = ActionEventListener.GetEventFieldNames(_actionEventListener.targetType, _actionEventListener.bindingFlags, _actionEventListener.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = ActionEventListener.GetTypeFullNames(_actionEventListener.bindingFlags, _actionEventListener.includeBaseType);
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
