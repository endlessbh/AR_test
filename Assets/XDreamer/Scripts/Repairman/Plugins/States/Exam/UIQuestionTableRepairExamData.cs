using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.States;

namespace XCSJ.PluginRepairman.States.Exam
{
    /// <summary>
    /// 问题表格拆装考试数据
    /// </summary>
    [Name("问题表格拆装考试数据")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIQuestionTable))]
    [RequireManager(typeof(RepairmanManager))]
    public class UIQuestionTableRepairExamData : InteractProvider
    {
        [Name("修理考试")]
        [StateComponentPopup(typeof(RepairExam), stateCollectionType = EStateCollectionType.Root)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RepairExam exam;

        /// <summary>
        /// 等待UIQuestionTable做初始化之后在开始
        /// </summary>
        protected void Start()
        {
            if(!exam)
            {
                exam = RepairmanHelperExtension.GetStateComponent<RepairExam>();
            }
            var table = GetComponent<UIQuestionTable>();
            if (exam && table)
            {
                table.data = exam;
                table.OnCreateQuestionTable(exam.questions.Cast<IQuestion>().ToList());
            }
        }
    }
}

