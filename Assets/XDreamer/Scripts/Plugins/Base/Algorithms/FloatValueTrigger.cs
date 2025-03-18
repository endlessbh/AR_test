using System;
using XCSJ.Attributes;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Algorithms
{
    [Name("数值比较类型")]
    public enum ENumberValueCompareRule
    {
        [Name("等于")]
        Equal = 0,

        [Name("不等于")]
        NotEqual,

        [Name("小于")]
        Less,

        [Name("小于等于")]
        LessEqual,

        [Name("大于")]
        Greater,

        [Name("大于等于")]
        GreaterEqual,

        [Name("变化时")]
        Changed,
    }

    /// <summary>
    /// 数值比较触发器
    /// </summary>
    [Name("数值比较触发器")]
    [Serializable]
    public class FloatValueTrigger
    {
        [Name("比较规则")]
        [EnumPopup]
        public ENumberValueCompareRule compareRule = ENumberValueCompareRule.Changed;

        [Name("触发值")]
        [HideInSuperInspector(nameof(compareRule), EValidityCheckType.Equal, ENumberValueCompareRule.Changed)]
        public float triggerValue = 1;

        public bool IsTrigger(float value)
        {
            switch (compareRule)
            {
                case ENumberValueCompareRule.Equal: return MathX.Approximately(value, triggerValue);
                case ENumberValueCompareRule.Greater: return value > triggerValue;
                case ENumberValueCompareRule.GreaterEqual: return value >= triggerValue;
                case ENumberValueCompareRule.Less: return value < triggerValue;
                case ENumberValueCompareRule.LessEqual: return value <= triggerValue;
                case ENumberValueCompareRule.NotEqual: return !MathX.Approximately(value, triggerValue);
                case ENumberValueCompareRule.Changed: return true;
            }

            return false;
        }
    }

}
