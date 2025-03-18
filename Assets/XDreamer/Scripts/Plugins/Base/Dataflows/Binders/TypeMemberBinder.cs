using System;
using System.Linq;
using System.Reflection;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 类型成员绑定器
    /// </summary>
    [Name("类型成员绑定器")]
    [Serializable]
    public abstract class TypeMemberBinder : TypeBinder, ITypeMemberBinder
    {
        /// <summary>
        /// 成员名称
        /// </summary>
        [Name("成员名称")]
        [Tip("期望绑定的成员名称", "Name of member expected to bind")]
        [MemberNamePopup]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _memberName = "";

        /// <summary>
        /// 成员名称
        /// </summary>
        public override string memberName { get => _memberName; set => _memberName = value; }

        #region ITypeMemberBinder

        /// <summary>
        /// 有成员
        /// </summary>
        public override bool hasMember => true;

        /// <summary>
        /// 成员信息对象
        /// </summary>
        public override MemberInfo memberInfo
        {
            get
            {
                return mainType?.GetMember(memberName, bindingFlags).FirstOrDefault();
            }
        }

        /// <summary>
        /// 成员类型
        /// </summary>
        public override Type memberType => TypeHelper.GetMemberType(memberInfo);

        /// <summary>
        /// 成员值，当成员信息类型为字段或属性时本参数有意义；
        /// </summary>
        public override object memberValue { get => GetMemberValue(); set => SetMemberValue(value); }

        /// <summary>
        /// 实体类型
        /// </summary>
        public override Type entityType { get => memberType; }

        /// <summary>
        /// 实体对象
        /// </summary>
        public override object entityObject { get => memberValue; set => memberValue = value; }

        #endregion

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual object GetMemberValue(object[] index = null)
        {
            DataBinderHelper.TryGetValue(mainType, mainObject, memberName, out object value, index);
            return value;
        }

        /// <summary>
        /// 设置成员值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public virtual bool SetMemberValue(object value, object[] index = null)
        {
            return DataBinderHelper.TrySetValue(mainType, mainObject, memberName, value, index);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => base.ToFriendlyString() + "." + memberName;

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public virtual bool DataValidity() => memberInfo != null;

        #region IDropdownPopupAttribute

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public override bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        throw new InvalidProgramException("需实现类[" + GetType() + "]成员方法[" + nameof(TryGetOptions) + "]中目标用途为[" + nameof(MemberNamePopupAttribute) + "]时的具体的数据处理！");
                    }
            }
            return base.TryGetOptions(purpose, propertyPath, out options);
        }

        /// <summary>
        /// 尝试获取文本选项：通过属性值转为文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        public override bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        option = (propertyValue as string) ?? "";
                        return true;
                    }
            }
            return base.TryGetOption(purpose, propertyPath, options, propertyValue, out option);
        }

        /// <summary>
        /// 尝试获取属性值：通过文本选项转为属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public override bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        propertyValue = option;
                        return true;
                    }
            }
            return base.TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
        }

        #endregion
    }
}
