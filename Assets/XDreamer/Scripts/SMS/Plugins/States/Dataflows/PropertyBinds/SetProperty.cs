using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 设置属性:设置对象中指定成员的属性值(字段、属性)
    /// </summary>
    [ComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(SetProperty))]
    [Tip("设置对象中指定成员的属性值(字段、属性)", "Set the property value (field, property) of the specified member in the object")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class SetProperty : BasePropertySet<SetProperty>, IDropdownPopupAttribute, ITypeBinderGetter
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "设置属性";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.PropertyBinds, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(SetProperty))]
        [Tip("设置对象中指定成员的属性值(字段、属性)", "Set the property value (field, property) of the specified member in the object")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 绑定器:用于绑定对象的字段或属性信息的对象
        /// </summary>
        [Name("绑定器")]
        [Tip("用于绑定对象的字段或属性信息的对象", "An object used to bind field or property information of an object")]
        public FieldOrPropertyBinder binder = new FieldOrPropertyBinder();

        /// <summary>
        /// 属性参数:期望设置的属性参数值
        /// </summary>
        [Name("属性参数")]
        [Tip("期望设置的属性参数值", "Expected attribute parameter value")]
        public Argument argument = new Argument();

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            binder.SetMemberValue(argument.value);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return binder.ToFriendlyString() + "=" + argument.DefaultFriendlyString();
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && binder.DataValidity();
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetMemberNames(binder.targetType, binder.bindField, binder.bindingFlags, binder.includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = FieldOrPropertyBinder.GetTypeFullNames(binder.bindField, binder.bindingFlags, binder.includeBaseType);
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
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { binder, argument._fieldPropertyMethodBinderValue };

        #endregion
    }
}
