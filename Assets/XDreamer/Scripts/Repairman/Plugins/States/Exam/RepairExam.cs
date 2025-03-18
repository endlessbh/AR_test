using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States.Exam
{
    /// <summary>
    /// 考试接口
    /// </summary>
    public interface IExam
    {
        event Action<List<IQuestion>> onStarted;
        event Action<ExamResult> onFinished;
    }

    /// <summary>
    /// 考试结果
    /// </summary>
    public class ExamResult
    {
        public float getScore;
        public float totalScore;
        public bool result;
        public string detailInfos;

        public ExamResult(float getScore, float totalScore, bool result, string detailInfos)
        {
            this.getScore = getScore;
            this.totalScore = totalScore;
            this.result = result;
            this.detailInfos = detailInfos;
        }
    }

    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairExam))]
    [XCSJ.Attributes.Icon(EIcon.Exam)]
    [Tip("拆装考试组件是由多个考题构成，在修理任务流程的基础上，通过选择零件和工具进行步骤答题，每题回答完毕，结果立即显示在答题卡上。考试完毕会弹出答题结果，结果中除了显示分数，还显示具体的回答错误的步骤信息。", "The disassembly and assembly test assembly is composed of multiple test questions. On the basis of the repair task process, answer the questions step by step by selecting parts and tools. After each question is answered, the results are immediately displayed on the answer card. After the exam, the answer result will pop up. In the result, not only the score, but also the specific step information of wrong answer will be displayed.")]
    public class RepairExam : RepairGuide<RepairExam>, IExam
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "拆装考试";

        [Name(Title, nameof(RepairExam))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装考试组件是由多个考题构成，在拆装任务流程的基础上，通过选择零件和工具进行步骤答题，每题回答完毕，结果立即显示在答题卡上。考试完毕会弹出答题结果，结果中除了显示分数，还显示具体的回答错误的步骤信息。", "The disassembly and assembly test assembly is composed of multiple test questions. On the basis of the repair task process, answer the questions step by step by selecting parts and tools. After each question is answered, the results are immediately displayed on the answer card. After the exam, the answer result will pop up. In the result, not only the score, but also the specific step information of wrong answer will be displayed.")]
        public static State CreateRepairExam(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 总分数
        /// </summary>
        [Name("总分数")]
        public float totalScore = 100;

        /// <summary>
        /// 答题次数
        /// </summary>
        [Name("答题次数")]
        [Range(1, 6)]
        public int answerCount = 3;

        /// <summary>
        /// 考试通过标准线
        /// </summary>
        [Name("考试通过标准线")]
        [Range(0, 1)]
        public float passLevel = 0.6f;

        private RepairQuestion currentQuestion => questions.Find(q => q.state == EQuestionState.Current);

        public List<RepairQuestion> questions { get; private set; } = new List<RepairQuestion>();

        public int QuestionCount => questions != null ? questions.Count : 0;

        #region 考试事件

        /// <summary>
        /// 问题接口
        /// </summary>
        public event Action<List<IQuestion>> onStarted;

        /// <summary>
        /// 考试信息改变
        /// </summary>
        public event Action<string> onExamInfoChanged;

        /// <summary>
        /// 考试完成
        /// </summary>
        public event Action<ExamResult> onFinished;

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool Init(StateData data)
        {
            base.Init(data);

            CreateQuestions();

            SetQuestionsEnable(false);

            return true;
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            ResetData();

            SetQuestionsEnable(true);

            onStarted?.Invoke(questions.ConvertAll<IQuestion>(q => q as IQuestion));

            // 清空UI提示信息
            onExamInfoChanged?.Invoke("");
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            onFinished?.Invoke(GetExamResult());
            ScriptManager.CallUDF(finishCallbackFun);

            SetQuestionsEnable(false);

            base.OnExit(data);
        }

        /// <summary>
        /// 完成
        /// </summary>
        /// <returns></returns>
        public override bool Finished()
        {
            return questions.All(q => q.Finished());
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);
            ResetData();
        }

        /// <summary>
        /// 创建考试
        /// </summary>
        private void CreateQuestions()
        {
            if (repairTaskWork && repairTaskWork.parent is SubStateMachine subSM && subSM)
            {
                // 依据步骤的顺序添加考题
                repairTaskWork.steps.ForEach(s =>
                {
                    var q = s.GetComponent<RepairQuestion>();
                    if (q)
                    {
                        questions.Add(q);
                    }
                });
            }
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        private void ResetData()
        {
            questions.ForEach(q => q.state = EQuestionState.Unfinish);
            rightQuestions.Clear();
            errorQuestionMap.Clear();
            skipQuestions.Clear();
        }

        /// <summary>
        /// 考题有效
        /// </summary>
        /// <param name="enable"></param>
        protected void SetQuestionsEnable(bool enable)
        {
            questions.ForEach(q => q.enable = enable);
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return totalScore + CommonFun.Name(this.GetType(), nameof(totalScore));
        }

        #region 答题交互

        /// <summary>
        /// 回答题目
        /// </summary>
        public void Answer()
        {
            if (Selection.selections.Length == 0)
            {
                onExamInfoChanged?.Invoke("请选择一个对象！");
                return;
            }

            if (AnswerQuestionEnable())
            {
                var question = currentQuestion;
                if (question.Answer())
                {
                    RecordRight(question);

                    onExamInfoChanged?.Invoke("回答正确！");
                }
                else
                {
                    int errorCount = RecordError(question);

                    onExamInfoChanged?.Invoke("回答错误，还有[" + (answerCount - errorCount) + "]次答题机会！");

                    if (errorCount == answerCount)
                    {
                        RecordSkip(question);

                        question.SkipRepairStep();

                        onExamInfoChanged?.Invoke("跳过考题！");
                    }
                }
            }
        }

        /// <summary>
        /// 跳过题目
        /// </summary>
        public override bool Skip()
        {
            if (AnswerQuestionEnable())
            {
                var question = currentQuestion;

                // 没有回答正确
                if (!question.Answer())
                {
                    RecordSkip(question);
                }

                question.Skip();

                onExamInfoChanged?.Invoke("跳过考题！");

                return true;
            }
            return false;
        }

        /// <summary>
        /// 考试结果
        /// </summary>
        /// <returns></returns>
        public ExamResult GetExamResult()
        {
            float totalScoreWeightValue = CalculateTotalScoreWeightValue();
            if (totalScoreWeightValue > 0)
            {
                float getScore = totalScore * (totalScoreWeightValue - CalculateSubtractScoreWeightValue()) / totalScoreWeightValue;
                bool pass = IsPass(getScore);
                string errorDatails = "";
                foreach (var question in questions)
                {
                    if (skipQuestions.Contains(question))
                    {
                        errorDatails += question.description + "(错误)\n";
                    }
                    else if (errorQuestionMap.ContainsKey(question))
                    {
                        errorDatails += question.description + "(错误" + errorQuestionMap[question].ToString() + "次)\n";
                    }
                }

                return new ExamResult(getScore, totalScore, pass, errorDatails);
            }
            else
            {
                return new ExamResult(0, 0, false, "");
            }
        }

        private bool AnswerQuestionEnable()
        {
            if (currentQuestion == null)
            {
                return false;
            }
            return currentQuestion.state == EQuestionState.Current;
        }

        #endregion

        #region 分数计算 和 错误记录

        /// <summary>
        /// 答对题目索引
        /// </summary>
        protected List<RepairQuestion> rightQuestions = new List<RepairQuestion>();

        /// <summary>
        /// 正确问题数量
        /// </summary>
        public int rightQuestionCount => rightQuestions.Count;

        /// <summary>
        /// key=答错题目索引, value=次数
        /// </summary>
        protected Dictionary<RepairQuestion, int> errorQuestionMap = new Dictionary<RepairQuestion, int>();

        /// <summary>
        /// 跳过步骤
        /// </summary>
        protected List<RepairQuestion> skipQuestions = new List<RepairQuestion>();

        private float CalculateTotalScoreWeightValue()
        {
            float questionTotalScore = 0;
            questions.ForEach(q =>
            {
                questionTotalScore += q.score;
            });
            return questionTotalScore;
        }

        /// <summary>
        /// 返回获得分数 100分制
        /// </summary>
        /// <returns></returns>
        private float CalculateSubtractScoreWeightValue()
        {
            float totalSubtractScore = 0;

            foreach (var question in questions)
            {
                // 跳过
                if (skipQuestions.Contains(question))
                {
                    totalSubtractScore += question.score;
                }
                // 答错
                else if (errorQuestionMap.ContainsKey(question))
                {
                    // 得分 = 当前步骤分数*（错误次数）/ 答题总次数
                    totalSubtractScore += question.score * errorQuestionMap[question] / answerCount;
                }
            }
            return totalSubtractScore;
        }

        /// <summary>
        /// 是否通过考试
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        private bool IsPass(float score)
        {
            return totalScore > 0 ? (score / totalScore) >= passLevel : false;
        }

        /// <summary>
        /// 记录错误，并返回错误次数
        /// </summary>
        private int RecordError(RepairQuestion rq)
        {
            if (errorQuestionMap.ContainsKey(rq))
            {
                if (errorQuestionMap[rq] <= answerCount)
                {
                    errorQuestionMap[rq] += 1;
                }
            }
            else
            {
                errorQuestionMap.Add(rq, 1);
            }
            return errorQuestionMap[rq];
        }

        /// <summary>
        /// 记录正确回答
        /// </summary>
        private void RecordRight(RepairQuestion rq)
        {
            if (rightQuestions.Contains(rq) == false)
            {
                rightQuestions.Add(rq);
            }
        }

        /// <summary>
        /// 记录跳过回答
        /// </summary>
        private void RecordSkip(RepairQuestion rq)
        {
            if (skipQuestions.Contains(rq) == false)
            {
                skipQuestions.Add(rq);
            }
        }

        #endregion
    }
}
