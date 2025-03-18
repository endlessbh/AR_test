using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤组操作:步骤组操作组件是对步骤组进行控制的执行体。控制包括【跳转上一步】、【跳转下一步】、【重置当前步骤】、【跳转到开始步骤】和【跳转到结束步骤】等操作，组件执行完毕后切换为完成态。
    /// </summary>
    [ComponentMenu("展示/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StepGroupOperation))]
    [XCSJ.Attributes.Icon(index = 33647)]
    [DisallowMultipleComponent]
    [Tip("步骤组操作组件是对步骤组进行控制的执行体。控制包括【跳转上一步】、【跳转下一步】、【重置当前步骤】、【跳转到开始步骤】和【跳转到结束步骤】等操作，组件执行完毕后切换为完成态。", "The step group operation component is the actuator that controls the step group. Control includes [jump to previous step], [jump to next step], [reset current step], [jump to start step] and [jump to end step], etc. after the component is executed, it switches to the completed state.")]
    public class StepGroupOperation : LifecycleExecutor<StepGroupOperation>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "步骤组操作";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("展示", typeof(SMSManager))]
        [StateComponentMenu("展示/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(StepGroupOperation))]
        [Tip("步骤组操作组件是对步骤组进行控制的执行体。控制包括【跳转上一步】、【跳转下一步】、【重置当前步骤】、【跳转到开始步骤】和【跳转到结束步骤】等操作，组件执行完毕后切换为完成态。", "The step group operation component is the actuator that controls the step group. Control includes [jump to previous step], [jump to next step], [reset current step], [jump to start step] and [jump to end step], etc. after the component is executed, it switches to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateStepGroupController(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("步骤组")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup()]
        public StepGroup stepGroup = null;

        [Name("步骤组操作")]
        [EnumPopup]
        public EStepGroupOperation stepGroupOperation = EStepGroupOperation.None;

        public override void Execute(StateData data, EExecuteMode executeMode)
        {
            OperateStepGroup(stepGroupOperation);
        }

        private void OperateStepGroup(EStepGroupOperation stepGroupOperation)
        {
            if (!stepGroup) return;

            switch (stepGroupOperation)
            {
                case EStepGroupOperation.None:
                    break;
                case EStepGroupOperation.GotoPreviousStep:
                    {
                        stepGroup.GotoPreviousStep();
                        break;
                    }
                case EStepGroupOperation.GotoNextStep:
                    {
                        stepGroup.GotoNextStep();
                        break;
                    }
                case EStepGroupOperation.ResetCurrentStep:
                    {
                        stepGroup.ResetCurrentStep();
                        break;
                    }
                case EStepGroupOperation.GotoFirstStep:
                    {
                        stepGroup.GotoFirstStep();
                        break;
                    }                    
                case EStepGroupOperation.GotoLastStep:
                    {
                        stepGroup.GotoLastStep();
                        break;
                    }
                default:
                    break;
            }
        }

        public override bool DataValidity() => stepGroup;

        public override string ToFriendlyString() => CommonFun.Name(stepGroupOperation);

        [Name("步骤组操作")]
        public enum EStepGroupOperation
        {
            [Name("无")]
            None,

            [Name("跳转到上一个步骤")]
            GotoPreviousStep,

            [Name("跳转到下一个步骤")]
            GotoNextStep,

            [Name("重置当前步骤")]
            ResetCurrentStep,

            [Name("跳转到开始步骤")]
            GotoFirstStep,

            [Name("跳转到结束步骤")]
            GotoLastStep,
        }
    }
}
