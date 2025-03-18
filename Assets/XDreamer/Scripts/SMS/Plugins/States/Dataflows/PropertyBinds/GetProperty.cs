using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 获取属性:获取对象中指定成员的属性值(字段、属性)
    /// </summary>
    [ComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GetProperty))]
    [Tip("获取对象中指定成员的属性值(字段、属性)", "Gets the property value (field, property) of the specified member in the object")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class GetProperty : BaseGetProperty<GetProperty>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "获取属性";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.PropertyBinds, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(GetProperty))]
        [Tip("获取对象中指定成员的属性值(字段、属性)", "Gets the property value (field, property) of the specified member in the object")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);
    }
}
