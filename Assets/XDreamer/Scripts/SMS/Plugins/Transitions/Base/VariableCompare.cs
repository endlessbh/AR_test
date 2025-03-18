using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.Scripts;

namespace XCSJ.PluginSMS.Transitions.Base
{
    [ComponentMenu("跳过/变量比较", typeof(SMSManager))]
    [Name("变量比较")]
    public class VariableCompare : Trigger, ISerializationCallbackReceiver
    {
        [Name("变量")]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        public string variable;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variable);
        }

        #endregion

        /// <summary>
        /// 比较运算符
        /// </summary>
        [Name("比较运算符")]
        [FormerlySerializedAs("compareType")]
        [EnumPopup]
        public ECompareOperator _compareOperator = ECompareOperator.Equal;

        /// <summary>
        /// 待比较值
        /// </summary>
        [Name("待比较值")]
        public Argument _compareValue = new Argument();

        [Name("比较规则")]
        [EnumPopup]
        public ECompareRule compareRule = ECompareRule.String;

        private bool match = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            match = false;
        }

        /// <summary>
        /// 更新回调
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (!match && CheckVariable())
            {
                match = true;
                SetTrigger();
            }
        }

        /// <summary>
        /// 比较变量
        /// </summary>
        /// <returns></returns>
        protected bool CheckVariable()
        {
            if (variable.TryGetHierarchyVarValue(out var variableValue) && _compareValue.GetValueToString() is string tmpCompareValue)
            {
                return VariableCompareHelper.ValueCompareValue(variableValue, _compareOperator, tmpCompareValue, compareRule);
            }
            return false;
        }

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return variable + VariableCompareHelper.ToAbbreviations(_compareOperator) + _compareValue.ToFriendlyString();
        }
    }
}
