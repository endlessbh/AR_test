using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.CNScripts;
using UnityEngine;
using XCSJ.Scripts;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Dataflows.DataModel
{
    /// <summary>
    /// 数据恢复；用于恢复数据记录器中记录的数据信息；
    /// </summary>
    [ComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(DataRecovery))]
    [Tip("用于恢复数据记录器中记录的数据信息", "Used to recover the data information recorded in the data logger")]
    [XCSJ.Attributes.Icon(EIcon.State)]
    public class DataRecovery : LifecycleExecutor<DataRecovery>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "数据恢复";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(DataflowHelper.DataModel, typeof(SMSManager))]
        [StateComponentMenu(DataflowHelper.DataModel + "/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(DataRecovery))]
        [Tip("用于恢复数据记录器中记录的数据信息", "Used to recover the data information recorded in the data logger")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 数据记录器
        /// </summary>
        [Name("数据记录器")]
        [StateComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public DataRecorder dataRecorder;

        /// <summary>
        /// 数据恢复模式:数据恢复到数据记录器中记录的哪个时刻的数据；仅可选择单一值，不支持多值操作；
        /// </summary>
        [Name("数据恢复模式")]
        [Tip("数据恢复到数据记录器中记录的哪个时刻的数据；仅可选择单一值，不支持多值操作；", "When the data is recovered to the data recorded in the data recorder; Only a single value can be selected, and multi value operation is not supported;")]
        [EnumPopup]
        public EDataRecordMode dataRecoveryMode = EDataRecordMode.Init;

        /// <summary>
        /// 数据记录作用域
        /// </summary>
        [Name("数据记录作用域")]
        [Tip("将指定作用域的数据记录恢复", "Recover the data records of the specified scope")]
        [EnumPopup]
        public EDataRecordScope _dataRecordScope = EDataRecordScope.Scene;

        /// <summary>
        /// 数据恢复规则
        /// </summary>
        [Name("数据恢复规则")]
        [EnumPopup]
        public EDataRecoveryRule dataRecoveryRule = EDataRecoveryRule.All;

        /// <summary>
        /// 数据恢复规则值类型
        /// </summary>
        [Name("数据恢复规则值类型")]
        [EnumPopup]
        public EDataRecoveryRuleValueType dataRecoveryRuleValueType = EDataRecoveryRuleValueType.None;

        /// <summary>
        /// 数据恢复规则值类型
        /// </summary>
        [Name("数据恢复规则值类型")]
        public enum EDataRecoveryRuleValueType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 值
            /// </summary>
            [Name("值")]
            Value,

            /// <summary>
            /// 变量
            /// </summary>
            [Name("变量")]
            Variable,
        }

        /// <summary>
        /// 数据恢复规则值
        /// </summary>
        [Name("数据恢复规则值")]
        [HideInSuperInspector(nameof(dataRecoveryRuleValueType), EValidityCheckType.NotEqual, EDataRecoveryRuleValueType.Value)]
        public string dataRecoveryRuleValue = "";

        /// <summary>
        /// 数据恢复规则变量名
        /// </summary>
        [Name("数据恢复规则变量名")]
        [VarString]
        [HideInSuperInspector(nameof(dataRecoveryRuleValueType), EValidityCheckType.NotEqual, EDataRecoveryRuleValueType.Variable)]
        public string dataRecoveryRuleVariable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref dataRecoveryRuleVariable);
        }

        #endregion

        private string GetDataRecoveryRuleValue()
        {
            switch (dataRecoveryRuleValueType)
            {
                case EDataRecoveryRuleValueType.Value: return dataRecoveryRuleValue;
                case EDataRecoveryRuleValueType.Variable:
                    {
                        if (dataRecoveryRuleVariable.TryGetHierarchyVarValue(out var value))
                        {
                            return value.ToScriptParamString();
                        }
                        return "";
                    }
                case EDataRecoveryRuleValueType.None:
                default: return "";
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (dataRecorder)
            {
                dataRecorder.Recovery(dataRecoveryMode, dataRecoveryRule, GetDataRecoveryRuleValue(), _dataRecordScope);
            }
        }

        /// <summary>
        /// 检测当前对象是否处于完成态
        /// </summary>
        /// <returns></returns>
        public override bool Finished() => true;

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return (dataRecorder ? dataRecorder.parent.name : "") + "->" + CommonFun.Name(dataRecoveryMode);
        }

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return base.DataValidity() && dataRecorder && (dataRecorder.dataRecordMode & dataRecoveryMode) != 0;
        }
    }

    /// <summary>
    /// 数据恢复规则
    /// </summary>
    [Name("数据恢复规则")]
    public enum EDataRecoveryRule
    {
        /// <summary>
        /// 所有:将所有数据记录信息的全部恢复
        /// </summary>
        [Name("所有")]
        [Tip("将所有数据记录信息的全部恢复", "Restore all data records and information")]
        All = -1,

        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None = 0,

        /// <summary>
        /// 名称相等
        /// </summary>
        [Name("名称相等")]
        NameEquals,

        /// <summary>
        /// 名称不相等
        /// </summary>
        [Name("名称不相等")]
        NameNotEquals,

        /// <summary>
        /// 名称包含
        /// </summary>
        [Name("名称包含")]
        NameContains,

        /// <summary>
        /// 名称不包含
        /// </summary>
        [Name("名称不包含")]
        NameNotContains,

        /// <summary>
        /// 名称正则匹配
        /// </summary>
        [Name("名称正则匹配")]
        NameRegexMatch,

        /// <summary>
        /// 名称正则不匹配
        /// </summary>
        [Name("名称正则不匹配")]
        NameRegexNotMatch,

        /// <summary>
        /// 是游戏对象的子级(通过名称路径)
        /// </summary>
        [Name("是游戏对象的子级(通过名称路径)")]
        IsChildOfGameObjectByNamePath,

        /// <summary>
        /// 不是游戏对象的子级(通过名称路径)
        /// </summary>
        [Name("不是游戏对象的子级(通过名称路径)")]
        NotIsChildOfGameObjectByNamePath,
    }
}
