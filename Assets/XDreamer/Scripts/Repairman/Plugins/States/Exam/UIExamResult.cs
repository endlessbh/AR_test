using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginRepairman.States.Exam
{
    /// <summary>
    /// 考试结果界面
    /// </summary>
    [Name("考试结果界面")]
    [RequireManager(typeof(RepairmanManager))]
    public class UIExamResult : View
    {
        [Name("拆装修理考试")]
        [StateComponentPopup(typeof(RepairExam), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairExam exam;

        [Name("得分")]
        public Text getScore;

        [Name("总分")]
        public Text totalScore;

        [Name("考试结果")]
        public Text result;

        [Name("错误详细记录")]
        public Text errorsDetailDescription;

        private string orgGetScoreText;

        private string orgTotalScoreText;

        private string orgResultText;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            if (!exam)
            {
                exam = RepairmanHelperExtension.GetStateComponent<RepairExam>();
            }

            if (getScore) orgGetScoreText = getScore.text;
            if (totalScore) orgTotalScoreText = totalScore.text;
            if (result) orgResultText = result.text;
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (exam)
            {
                SetExamResult(exam.GetExamResult());
            }
        }

        public void SetExamResult(ExamResult examResult)
        {
            if (getScore)
            {
                getScore.text = orgGetScoreText + ((int)examResult.getScore).ToString();
            }

            if (totalScore)
            {
                totalScore.text = orgTotalScoreText + ((int)examResult.totalScore).ToString();
            }

            if (result)
            {
                result.text = orgResultText + (((bool)examResult.result) ? "通过" : "未通过");
            }

            if (errorsDetailDescription)
            {
                errorsDetailDescription.text = examResult.detailInfos;
            }
        }
    }
}
