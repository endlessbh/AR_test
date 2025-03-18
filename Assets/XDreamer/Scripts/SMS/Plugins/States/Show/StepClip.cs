using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Show
{
    /// <summary>
    /// 步骤剪辑：步骤剪辑组件是将动画组件关联到步骤上的对象。多个步骤剪辑构成一个步骤，组件激活后随即切换为完成态。
    /// </summary>
    [ComponentMenu("展示/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(StepClip))]
    [Tip("步骤剪辑组件是将动画组件关联到步骤上的对象。多个步骤剪辑构成一个步骤，组件激活后随即切换为完成态。", "A step clip component is an object that associates an animation component to a step. Multiple step clips constitute one step, and the component will switch to the completed state immediately after activation.")]
    [XCSJ.Attributes.Icon(index = 33645)]
    [DisallowMultipleComponent]
    public class StepClip : StateComponent<StepClip>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "步骤剪辑";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("展示", typeof(SMSManager))]
        [StateComponentMenu("展示/"+ Title, typeof(SMSManager))]
        [Name(Title, nameof(StepClip))]
        [Tip("步骤剪辑组件是将动画组件关联到步骤上的对象。多个步骤剪辑构成一个步骤，组件激活后随即切换为完成态。", "A step clip component is an object that associates an animation component to a step. Multiple step clips constitute one step, and the component will switch to the completed state immediately after activation.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateStepClip(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("步骤", "Step")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [StateComponentPopup(typeof(Step), stateCollectionType = EStateCollectionType.Current, searchFlags = ESearchFlags.DefaultChildrenOptimize)]
        public Step step;

        public override bool Init(StateData data)
        {            
            if (step)
            {
                step.AddClip(this);
            }
            clipState = EStepState.Unfinished;

            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            clipState = EStepState.Active;
        }

        public override void OnExit(StateData data)
        {
            clipState = EStepState.Finished;

            if (step)
            {
                step.OnStepClipStateChanged(this, clipState);
            }

            base.OnExit(data);
        }

        public override bool DataValidity() => step;

        public override bool Finished() => true;

        public EStepState clipState { get; set; } = EStepState.None;

        public void ResetClipState() { clipState = EStepState.Unfinished; }
    }
}
