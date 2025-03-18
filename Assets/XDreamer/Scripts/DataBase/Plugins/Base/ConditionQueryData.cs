using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Interfaces;
using XCSJ.Scripts;

namespace XCSJ.PluginDataBase.Base
{
    /// <summary>
    /// 条件查询数据
    /// </summary>
    [Name("条件查询数据")]
    [Serializable]
    public class ConditionQueryData : IToFriendlyString
    {
        /// <summary>
        /// 表名:表名
        /// </summary>
        [Name("表名")]
        [Tip("表名", "Table name")]
        [OnlyMemberElements]
        public StringPropertyValue _tableName = new StringPropertyValue();

        /// <summary>
        /// 字段名:多个字段名使用英文逗号‘,’间隔
        /// </summary>
        [Name("字段名")]
        [Tip("多个字段名使用英文逗号‘,’间隔", "Multiple field names are separated by English commas', '")]
        [OnlyMemberElements]
        public StringPropertyValue _fieldNames = new StringPropertyValue();

        /// <summary>
        /// 条件字段:条件字段
        /// </summary>
        [Name("条件字段")]
        [Tip("条件字段")]
        [OnlyMemberElements]
        public StringPropertyValue _conditionFieldName = new StringPropertyValue();

        /// <summary>
        /// 匹配条件:匹配条件
        /// </summary>
        [Name("匹配条件")]
        [Tip("匹配条件")]
        [OnlyMemberElements]
        public EMatchConditionPropertyValue _matchCondition = new EMatchConditionPropertyValue();

        /// <summary>
        /// 条件值
        /// </summary>
        [Name("条件值")]
        [Tip("条件值")]
        [OnlyMemberElements]
        public StringPropertyValue _conditionValue = new StringPropertyValue();

        /// <summary>
        /// 条件值是文本
        /// </summary>
        [Name("条件值是文本")]
        [Tip("条件值是文本")]
        [OnlyMemberElements]
        public EBool2PropertyValue _conditionValueIsText = new EBool2PropertyValue();

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <returns></returns>
        public string GetSql() => GetSql(_conditionValue.GetValue());

        /// <summary>
        /// 获取SQL语句
        /// </summary>
        /// <param name="conditionValue">条件值</param>
        /// <returns></returns>
        public string GetSql(string conditionValue)
        {
            var selectFields = _fieldNames.GetValue();
            if (string.IsNullOrEmpty(selectFields)) selectFields = "*";

            var condition = conditionValue ?? _conditionValue.GetValue();
            if (EBool2.Yes == _conditionValueIsText.GetValue()) condition = string.Format("'{0}'", condition);

            var sql = string.Format("select {0} from {1} where {2} {3} {4}", selectFields, _tableName.GetValue(), _conditionFieldName.GetValue(), _matchCondition.GetCondition(), condition);

            return sql;
        }

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <returns></returns>
        public string ToFriendlyString() => ToFriendlyString(_conditionValue.ToFriendlyString());

        /// <summary>
        /// 转友好字符串
        /// </summary>
        /// <param name="conditionValue"></param>
        /// <returns></returns>
        public string ToFriendlyString(string conditionValue)
        {
            var selectFields = _fieldNames.ToFriendlyString();
            if (string.IsNullOrEmpty(selectFields)) selectFields = "*";

            var condition = conditionValue ?? _conditionValue.ToFriendlyString();
            if (EBool2.Yes == _conditionValueIsText.GetValue()) condition = string.Format("'{0}'", condition);

            return string.Format("select {0} from {1} where {2} {3} {4}", selectFields, _tableName.ToFriendlyString(), _conditionFieldName.ToFriendlyString(), _matchCondition.ToFriendlyString(), condition);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public virtual bool DataValidity()
        {
            return _tableName.DataValidity() && _conditionFieldName.DataValidity();
        }
    }

    /// <summary>
    /// 匹配条件
    /// </summary>
    [Name("匹配条件")]
    public enum EMatchCondition
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [EnumFieldName("")]
        [Abbreviation("")]
        None,

        /// <summary>
        /// 相等
        /// </summary>
        [Name("相等")]
        [EnumFieldName("=")]
        [Abbreviation("=")]
        Equal,

        /// <summary>
        /// 不等
        /// </summary>
        [Name("不等")]
        [EnumFieldName("<>")]
        [Abbreviation("<>")]
        NotEqual,

        /// <summary>
        /// 大于
        /// </summary>
        [Name("大于")]
        [EnumFieldName(">")]
        [Abbreviation(">")]
        Greater,

        /// <summary>
        /// 大于等于
        /// </summary>
        [Name("大于等于")]
        [EnumFieldName(">=")]
        [Abbreviation(">=")]
        GreaterEqual,

        /// <summary>
        /// 小于
        /// </summary>
        [Name("小于")]
        [EnumFieldName("<")]
        [Abbreviation("<")]
        Less,

        /// <summary>
        /// 小于等于
        /// </summary>
        [Name("小于等于")]
        [EnumFieldName("<=")]
        [Abbreviation("<=")]
        LessEqual,

        /// <summary>
        /// 像
        /// </summary>
        [Name("像")]
        [EnumFieldName("like")]
        [Abbreviation("like")]
        Like,

        /// <summary>
        /// 之间
        /// </summary>
        [Name("之间")]
        [EnumFieldName("between")]
        [Abbreviation("between")]
        Between,

        /// <summary>
        /// 在
        /// </summary>
        [Name("在")]
        [EnumFieldName("in")]
        [Abbreviation("in")]
        In,

        /// <summary>
        /// 不在
        /// </summary>
        [Name("不在")]
        [EnumFieldName("not in")]
        [Abbreviation("not in")]
        NotIn,
    }

    /// <summary>
    /// 匹配条件属性值
    /// </summary>
    [Name("匹配条件属性值")]
    [Serializable]
    [PropertyType(typeof(EMatchCondition))]
    public class EMatchConditionPropertyValue : EnumPropertyValue<EMatchCondition>
    {
        /// <summary>
        /// 获取条件
        /// </summary>
        /// <returns></returns>
        public string GetCondition() => AbbreviationAttribute.GetAbbreviation(GetValue());

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_propertyValueType)
            {
                case EPropertyValueType.Value:
                    {
                        return AbbreviationAttribute.GetAbbreviation(_enumValue);
                    }
            }
            return base.ToFriendlyString();
        }
    }
}
