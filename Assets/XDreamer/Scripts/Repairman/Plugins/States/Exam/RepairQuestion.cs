using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginRepairman.States.RepairTask;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States.Exam
{
    [RequireComponent(typeof(RepairStep))]
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairQuestion))]
    [XCSJ.Attributes.Icon(EIcon.Question)]
    [Tip("考题组件是拆装修理考试的基础元素，通过状态实现。每个考题可设置分数，分数的不同占整体考试权重值不同。", "The test component is the basic element of the disassembly and repair test, which is realized through the state. Scores can be set for each test question. Different scores account for different weight values of the overall test.")]
    [Owner(typeof(RepairmanManager))]
    public class RepairQuestion : StateComponent<RepairQuestion>,IQuestion
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "拆装考题";

        [Name(Title, nameof(RepairQuestion))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("考题组件是拆装修理考试的基础元素，通过状态实现。每个考题可设置分数，分数的不同占整体考试权重值不同。", "The test component is the basic element of the disassembly and repair test, which is realized through the state. Scores can be set for each test question. Different scores account for different weight values of the overall test.")]
        public static State CreateRepairQuestion(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("分数权值")]
        [Range(0, 100)]
        [Tip("所有题目的分数加在一起是总分，然后按百分制分别占权重", "The scores of all questions add up to the total score, and then they are weighted according to the percentage system")]
        public float scoreWeightValue = 1f;

        public EQuestionState state { get; set; } = EQuestionState.Unfinish;

        public float score => scoreWeightValue;

        public string description => repairStep.description;

        protected RepairStep repairStep ;

        public override bool Init(StateData data)
        {
            repairStep = GetComponent<RepairStep>();
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            state = EQuestionState.Current;
        }

        public override bool Finished()
        {
            return (state == EQuestionState.Right || state == EQuestionState.Wrong);
        }

        public virtual bool Answer()
        {
            if (repairStep.Finished())
            {
                state = EQuestionState.Right;
                return true;
            }        
            return false;
        }

        public virtual void Skip()
        {
            state = EQuestionState.Wrong;
        }

        public void SkipRepairStep()
        {
            repairStep.Skip();
            Skip();
        }
    }

    /// <summary>
    /// 考题状态
    /// </summary>
    [Name("考题状态")]
    public enum EQuestionState
    {
        /// <summary>
        /// 当前
        /// </summary>
        Current,

        /// <summary>
        /// 未完成
        /// </summary>
        Unfinish,

        /// <summary>
        /// 对
        /// </summary>
        Right,

        /// <summary>
        /// 错误
        /// </summary>
        Wrong
    }

    public interface IQuestion
    {
        EQuestionState state { get; }

        string description { get; }

        float score { get; }
    }
}
