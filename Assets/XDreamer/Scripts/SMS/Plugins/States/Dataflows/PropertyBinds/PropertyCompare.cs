using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Interfaces;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 属性比较:将对象中指定成员的属性值(字段、属性)与期望值做比较；当比较条件成立后，状态组件切换为完成态；
    /// </summary>
    [ComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(PropertyCompare))]
    [Tip("将对象中指定成员的属性值(字段、属性)与期望值做比较；当比较条件成立后，状态组件切换为完成态；", "Compare the attribute value (field, attribute) of the specified member in the object with the expected value; When the comparison condition is established, the state component switches to the completed state;")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class PropertyCompare : BasePropertyCompare<PropertyCompare>, IDropdownPopupAttribute, ITypeBinderGetter
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "属性比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.PropertyBinds , typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(PropertyCompare))]
        [Tip("将对象中指定成员的属性值(字段、属性)与期望值做比较；当比较条件成立后，状态组件切换为完成态；", "Compare the attribute value (field, attribute) of the specified member in the object with the expected value; When the comparison condition is established, the state component switches to the completed state;")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 属性检测
        /// </summary>
        [Name("属性检测")]
        public PropertyDetection _propertyDetection = new PropertyDetection();

        /// <summary>
        /// 当更新时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (!finished && _propertyDetection.Check())
            {
                finished = true;
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return _propertyDetection._binder.memberInfo != null;
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _propertyDetection.ToFriendlyString();

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetMemberNames(_propertyDetection._binder.targetType, _propertyDetection._binder.bindField, _propertyDetection._binder.bindingFlags, _propertyDetection._binder.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetTypeFullNames(_propertyDetection._binder.bindField, _propertyDetection._binder.bindingFlags, _propertyDetection._binder.includeBaseType);
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

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { _propertyDetection._binder, _propertyDetection._argumentDetection._argument._fieldPropertyMethodBinderValue };

        #endregion
    }

    /// <summary>
    /// 属性检测
    /// </summary>
    [Name("属性检测")]
    [Serializable]
    public class PropertyDetection : IToFriendlyString
    {
        /// <summary>
        /// 绑定器:用于绑定对象的字段或属性信息的对象
        /// </summary>
        [Name("绑定器")]
        [Tip("用于绑定对象的字段或属性信息的对象", "An object used to bind field or property information of an object")]
        public FieldOrPropertyBinder _binder = new FieldOrPropertyBinder();

        /// <summary>
        /// 待检参数:等待与绑定器中成员信息值的参数对象
        /// </summary>
        [Name("待检参数")]
        [Tip("等待与绑定器中成员信息值的参数对象", "Wait for the parameter object with the member information value in the binder")]
        public ArgumentDetection _argumentDetection = new ArgumentDetection();

        /// <summary>
        /// 检查属性是否符合检测规则
        /// </summary>
        /// <returns></returns>
        public bool Check() => _argumentDetection.Check(_binder.memberValue);

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString()
        {
            switch (_argumentDetection._detectionOrder)
            {
                case EDetectionOrder.Parameter_Argument: return _binder.ToFriendlyString() + _argumentDetection.ToFriendlyString();
                case EDetectionOrder.Argument_Parameter: return _argumentDetection.ToFriendlyString() + _binder.ToFriendlyString();
                default: return "";
            }
        }
    }
}
