using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Operations
{
    /// <summary>
    /// 状态进度:状态进度组件是设置状态进度的执行体。组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StateProgress))]
    [XCSJ.Attributes.Icon(index = 33663)]
    [Tip("状态进度组件是设置状态进度的执行体。组件执行完毕后切换为完成态。", "Status progress component is the executor of setting status progress. After the component is executed, it is switched to the completed state.")]
    public class StateProgress : LifecycleExecutor<StateProgress>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态进度";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/"+ Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateProgress))]
        [Tip("状态进度组件是设置状态进度的执行体。组件执行完毕后切换为完成态。", "Status progress component is the executor of setting status progress. After the component is executed, it is switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("状态")]
        [StatePopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public State state;

        [Name("使用变量值")]
        public bool useVariable = false;

        [Name("状态进度")]
        [Range(0,1)]
        [HideInSuperInspector(nameof(useVariable), EValidityCheckType.Equal, true)]
        public float stateProgress = 0;

        [Name("变量")]
        [VarString(EVarStringHierarchyKeyMode.Get)]
        [HideInSuperInspector(nameof(useVariable), EValidityCheckType.NotEqual, true)]
        public string variable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variable);
        }

        #endregion

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            if (state)
            {
                if (useVariable)
                {
                    if (variable.TryGetHierarchyVarValue(out var varStringValue) &&
                        Converter.instance.TryConvertTo(varStringValue, out float varFloatValue))
                    {
                        state.SetProgress(varFloatValue);
                    }
                }
                else
                {
                    state.SetProgress(stateProgress);
                }
            }
        }

        public override string ToFriendlyString()
        {
            return state ? state.name : "";
        }
    }
}
