using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 获取并设置属性:获取对象中指定成员的属性值(字段、属性),同时将获取到的值设置到对象中指定成员的属性值(字段、属性)
    /// </summary>
    [ComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(GetAndSetProperty))]
    [Tip("获取对象中指定成员的属性值(字段、属性),同时将获取到的值设置到对象中指定成员的属性值(字段、属性)", "Get the property value (field, property) of the specified member in the object, and set the obtained value to the property value (field, property) of the specified member in the object")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class GetAndSetProperty : BaseGetProperty<GetAndSetProperty>, IDropdownPopupAttribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "获取并设置属性";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.PropertyBinds, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(GetAndSetProperty))]
        [Tip("获取对象中指定成员的属性值(字段、属性),同时将获取到的值设置到对象中指定成员的属性值(字段、属性)", "Get the property value (field, property) of the specified member in the object, and set the obtained value to the property value (field, property) of the specified member in the object")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 设置绑定器:用于设置属性时使用的绑定对象的字段或属性信息的对象列表
        /// </summary>
        [Name("设置绑定器")]
        [Tip("用于设置属性时使用的绑定对象的字段或属性信息的对象列表", "The object list used to set the field or property information of the bound object used when setting the property")]
        public List<FieldOrPropertyBinder> _setBinders = new List<FieldOrPropertyBinder>();


        #region ITypeBinderGetter

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ITypeBinder> GetTypeBinders()
        {
            var list = new List<ITypeBinder>();
            list.AddRange(base.GetTypeBinders());
            list.AddRange(_setBinders);
            return list;
        }

        #endregion


        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var binderValue = _binder.memberValue;

            //base.Execute(stateData, executeMode);
            SetToVariable(binderValue);
            foreach (var setBinder in _setBinders)
            {
                setBinder.SetMemberValue(binderValue);
            }
        }

        private static readonly StringBuilder friendlyString = new StringBuilder();

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            friendlyString.Clear();
            var getString = _binder.ToFriendlyString();
            if (!string.IsNullOrEmpty(_resultVarString))
            {
                friendlyString.AppendLine(_resultVarString + "=" + getString);
            }
            foreach (var setBinder in _setBinders)
            {
                friendlyString.AppendLine(setBinder.ToFriendlyString() + "=" + getString);
            }
            return friendlyString.ToString();
        }

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _setBinders.All(binder => binder.DataValidity());
        }

        #region IDropdownPopupAttribute实现

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        if (propertyPath.Contains(nameof(_setBinders)))
                        {
                            var match = Regex.Match(propertyPath, @"\d+");
                            if (match.Success && _setBinders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldOrPropertyBinder setBinder)
                            {
                                options = FieldOrPropertyBinder.GetMemberNames(setBinder.targetType, setBinder.bindField, setBinder.bindingFlags, setBinder.includeBaseType);
                                return true;
                            }
                        }
                        break;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        if (propertyPath.Contains(nameof(_setBinders)))
                        {
                            var match = Regex.Match(propertyPath, @"\d+");
                            if (match.Success && _setBinders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldOrPropertyBinder setBinder)
                            {
                                options = FieldOrPropertyBinder.GetTypeFullNames(setBinder.bindField, setBinder.bindingFlags, setBinder.includeBaseType);
                                return true;
                            }
                        }
                        break;
                    }
            }
            return _binder.TryGetOptions(purpose, propertyPath, out options);
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            return _binder.TryGetOption(purpose, propertyPath, options, propertyValue, out option);
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            return _binder.TryGetPropertyValue(purpose, propertyPath, options, option, out propertyValue);
        }

        #endregion
    }
}
