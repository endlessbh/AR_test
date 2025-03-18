using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Dataflows.Binders;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginMMO.NetSyncs;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginMMO.Transitions
{
    /// <summary>
    /// 将网络属性的值与待比较值进行比较判断；
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetPropertyCompare))]
    [Tip("将网络属性的值与待比较值进行比较判断", "Compare and judge the value of network attribute with the value to be compared")]
    [RequireManager(typeof(MMOManager))]
    public class NetPropertyCompare : TransitionComponent, IDropdownPopupAttribute, ITypeBinderGetter
    {
        public const string Title = "网络属性比较";

        [Name("网络属性")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [FormerlySerializedAs(nameof(netProperty))]
        public NetProperty _netProperty;

        /// <summary>
        /// 网络属性
        /// </summary>
        public NetProperty netProperty { get => _netProperty; set => _netProperty = value; }

        [Name("属性名")]
        [Tip("期望比较的网络属性的名称", "The name of the network attribute you want to compare")]
        [NetPropertyName]
        [FormerlySerializedAs(nameof(propertyName))]
        public string _propertyName;

        /// <summary>
        /// 属性名
        /// </summary>
        public string propertyName { get => _propertyName; set => _propertyName = value; }

        /// <summary>
        /// 比较运算符
        /// </summary>
        [Name("比较运算符")]
        [FormerlySerializedAs("compareType")]
        [FormerlySerializedAs("_compareType")]
        [EnumPopup]
        public ECompareOperator _compareOperator = ECompareOperator.Equal;

        /// <summary>
        /// 比较运算符
        /// </summary>
        public ECompareOperator compareOperator { get => _compareOperator; set => _compareOperator = value; }

        /// <summary>
        /// 待比较值
        /// </summary>
        [Name("待比较值")]
        public Argument _compareValue = new Argument();        

        [Name("比较规则")]
        [FormerlySerializedAs(nameof(compareRule))]
        [EnumPopup]
        public ECompareRule _compareRule = ECompareRule.String;

        public ECompareRule compareRule { get => _compareRule; set => _compareRule = value; }

        public override bool Finished()
        {
            return base.Finished() || Check();
        }

        private bool Check()
        {
            if (netProperty && netProperty.GetProperty(propertyName) is Property property && _compareValue.GetValueToString() is string tmpCompareValue)
            {
                return VariableCompareHelper.ValueCompareValue(property.value, compareOperator, tmpCompareValue, compareRule);
            }
            return false;
        }

        //public override void OnUpdate(StateData data)
        //{
        //    base.OnUpdate(data);

        //    if (canTrigger)
        //    {
        //        finished = Check();
        //    }
        //}

        public override string ToFriendlyString()
        {
            return propertyName + ".属性值" + VariableCompareHelper.ToAbbreviations(compareOperator) + _compareValue.ToFriendlyString();
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && netProperty;
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(NetPropertyNameAttribute):
                    {
                        options = netProperty ? netProperty.propertys.Cast(p => p.name).ToArray() : Empty<string>.Array;
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
                case nameof(NetPropertyNameAttribute):
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
                case nameof(NetPropertyNameAttribute):
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
        public IEnumerable<ITypeBinder> GetTypeBinders() => new ITypeBinder[] { _compareValue._fieldPropertyMethodBinderValue };

        #endregion

    }
}
