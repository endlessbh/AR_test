using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.Extension.Base.Helpers;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.PropertyBinds
{
    /// <summary>
    /// 多项属性比较：将多个对象中指定成员的属性值(字段、属性)相互做比较；当比较条件成立后，状态组件切换为完成态；
    /// </summary>
    [ComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(MultiPropertyCompare))]
    [Tip("将多个对象中指定成员的属性值(字段、属性)相互做比较；当比较条件成立后，状态组件切换为完成态；", "Compare the attribute values (fields and attributes) of the specified members in multiple objects; When the comparison condition is established, the state component switches to the completed state;")]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    public class MultiPropertyCompare : Trigger<MultiPropertyCompare>, IDropdownPopupAttribute, ITypeBinderGetter
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "多项属性比较";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.PropertyBinds, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.PropertyBinds + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(MultiPropertyCompare))]
        [Tip("将多个对象中指定成员的属性值(字段、属性)相互做比较；当比较条件成立后，状态组件切换为完成态；", "Compare the attribute values (fields and attributes) of the specified members in multiple objects; When the comparison condition is established, the state component switches to the completed state;")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 绑定器列表：用于比较使用的绑定对象的字段或属性信息的对象列表
        /// </summary>
        [Name("绑定器列表")]
        [Tip("用于比较使用的绑定对象的字段或属性信息的对象列表", "A list of objects used to compare the field or attribute information of the binding object used")]
        public List<FieldOrPropertyBinder> _binders = new List<FieldOrPropertyBinder>();

        #region ITypeBinderGetter

        /// <summary>
        /// 获取器所有者
        /// </summary>
        public UnityEngine.Object owner => this;

        /// <summary>
        /// 类型绑定器获取器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITypeBinder> GetTypeBinders() => _binders;

        #endregion

        /// <summary>
        /// 多项比较规则
        /// </summary>
        [Name("多项比较规则")]
        [EnumPopup]
        public EMultiCompareRule _multiCompareRule = EMultiCompareRule.AllEqual;

        /// <summary>
        /// 多项比较规则
        /// </summary>
        [Name("多项比较规则")]
        public enum EMultiCompareRule
        {
            /// <summary>
            /// 无：不做比较
            /// </summary>
            [Name("无")]
            [Tip("不做比较", "No comparison")]
            None,

            /// <summary>
            /// 全部相等：如果少于2个元素时，不做处理；
            /// </summary>
            [Name("全部相等")]
            [Tip("如果少于2个元素时，不做处理；", "If there are less than 2 elements, it will not be processed;")]
            AllEqual,

            /// <summary>
            /// 任意两个相等：如果少于2个元素时，不做处理；
            /// </summary>
            [Name("任意两个相等")]
            [Tip("如果少于2个元素时，不做处理；", "If there are less than 2 elements, it will not be processed;")]
            AnyTwoEqual,
        }

        /// <summary>
        /// 当更新时
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (!finished)
            {
                switch(_multiCompareRule)
                {
                    case EMultiCompareRule.AllEqual:
                        {
                            var count = _binders.Count;
                            if (count < 2) break;

                            var values = new List<object>();
                            for (int i = 0; i < count; i++)
                            {
                                var value = _binders[i].memberValue;
                                if (!values.All(v => ObjectHelper.ObjectEquals(value, v)))
                                {
                                    return;
                                }
                                values.Add(value);
                            }
                            finished = true;
                            break;
                        }
                    case EMultiCompareRule.AnyTwoEqual:
                        {
                            var count = _binders.Count;
                            if (count < 2) break;
                            var values = new List<object>();
                            for (int i = 0; i < count; i++)
                            {
                                var value = _binders[i].memberValue;
                                if (values.Any(v => ObjectHelper.ObjectEquals(value, v)))
                                {
                                    finished = true;
                                    return;
                                }
                                values.Add(value);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => CommonFun.Name(_multiCompareRule);

        /// <summary>
        /// 判断数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && _binders.All(binder => binder.DataValidity());
        }

        #region IDropdownPopupAttribute实现

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public virtual bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        var match = Regex.Match(propertyPath, @"\d+");
                        if (match.Success && _binders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldOrPropertyBinder binder)
                        {
                            options = FieldOrPropertyBinder.GetMemberNames(binder.targetType, binder.bindField, binder.bindingFlags, binder.includeBaseType);
                            return true;
                        }
                        break;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        var match = Regex.Match(propertyPath, @"\d+");
                        if (match.Success && _binders.ElementAtOrDefault(Converter.instance.ConvertTo<int>(match.Value)) is FieldOrPropertyBinder binder)
                        {
                            options = FieldOrPropertyBinder.GetTypeFullNames(binder.bindField, binder.bindingFlags, binder.includeBaseType);
                            return true;
                        }
                        break;
                    }
            }
            options = default;
            return false;
        }

        /// <summary>
        /// 尝试获取文本选项
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="propertyValue">属性值</param>
        /// <param name="option">选项文本</param>
        /// <returns></returns>
        public virtual bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
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

        /// <summary>
        /// 尝试获取属性值
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="option">选项文本</param>
        /// <param name="propertyValue">属性值</param>
        /// <returns></returns>
        public virtual bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
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

        #endregion
    }
}
