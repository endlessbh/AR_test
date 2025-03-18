using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Operations
{
    /// <summary>
    /// 状态检测:检测状态处于期望值，组件切换为完成态。
    /// </summary>
    [ComponentMenu("状态操作/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(StateDetection))]
    [XCSJ.Attributes.Icon(index = 33661)]
    [Tip("检测状态处于期望值，组件切换为完成态。", "The detection state is at the expected value, and the component is switched to the completed state.")]
    public class StateDetection : Trigger<StateDetection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "状态检测";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("状态操作", typeof(SMSManager))]
        [StateComponentMenu("状态操作/" + Title, typeof(SMSManager))]
#endif
        [Name(Title, nameof(StateDetection))]
        [Tip("检测状态处于期望值，组件切换为完成态。", "The detection state is at the expected value, and the component is switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateStateDetection(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 检测类型
        /// </summary>
        [Name("检测类型")]
        [EnumPopup]
        public EStateDetectionType detectionType = EStateDetectionType.Finish;

        /// <summary>
        /// 状态
        /// </summary>
        [StatePopup]
        [Name("状态")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public State state = null;

        private bool stateActive = false;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);

            if (!state || finished) return;

            switch (detectionType)
            {
                case EStateDetectionType.Active:
                    {
                        finished = state.active;
                        break;
                    }
                case EStateDetectionType.Unactive:
                    {
                        finished = !state.active;
                        break;
                    }
                case EStateDetectionType.FromActiveToUnactive:
                    {
                        if (stateActive)
                        {
                            finished = !state.active;
                        }
                        else
                        {
                            stateActive = state.active;
                        }
                        break;
                    }
                case EStateDetectionType.Finish:
                    {
                        finished = state.finished;
                        break;
                    }
                default: break;
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            stateActive = false;
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return state ? state.name + "->" + CommonFun.Name(detectionType) : "";
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return state;
        }
    }

    [Name("状态检测类型")]
    public enum EStateDetectionType
    {
        [Name("激活")]
        Active,

        [Name("非激活")]
        Unactive,

        [Name("激活至非激活")]
        FromActiveToUnactive,

        [Name("完成")]
        Finish
    }

}
