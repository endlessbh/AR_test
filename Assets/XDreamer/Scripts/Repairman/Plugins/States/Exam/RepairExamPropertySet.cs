using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginRepairman.States.Exam
{
    [ComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
    [Name(Title, nameof(RepairExamPropertySet))]
    [Tip("拆装修理答题组件是用于执行答题命令的执行体。对设定的考试状态组件执行答题命令，组件激活后即刻切换为完成态。", "The disassembly and repair answer component is an actuator used to execute the answer command. Execute the answer command for the set examination status component, and switch to the completed state immediately after the component is activated.")]
    [Owner(typeof(RepairmanManager))]
    public class RepairExamPropertySet : BasePropertySet<RepairExamPropertySet>
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Title = "拆装考试属性设置";

        [Name(Title, nameof(RepairExamPropertySet))]
        [StateLib(RepairmanHelperExtension.StepStateLibName, typeof(RepairmanManager))]
        [StateComponentMenu(RepairmanHelperExtension.StepPath + "/" + Title, typeof(RepairmanManager))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("拆装修理答题组件是用于执行答题命令的执行体。对设定的考试状态组件执行答题命令，组件激活后即刻切换为完成态。", "The disassembly and repair answer component is an actuator used to execute the answer command. Execute the answer command for the set examination status component, and switch to the completed state immediately after the component is activated.")]
        public static State CreateAnswerQuestion(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 拆装考试
        /// </summary>
        [Name("拆装考试")]
        [StateComponentPopup]
        public RepairExam repairExam = null;

        /// <summary>
        /// 操作
        /// </summary>
        [Name("操作")]
        [EnumPopup]
        public EPropertyName _propertyName = EPropertyName.AnswerQuestion;

        /// <summary>
        /// 属性名称
        /// </summary>
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 回答考题
            /// </summary>
            [Name("回答考题")]
            AnswerQuestion,

            /// <summary>
            /// 跳过考题
            /// </summary>
            [Name("跳过考题")]
            Skip,
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            if (!repairExam) return;

            switch (_propertyName)
            {
                case EPropertyName.AnswerQuestion:
                    {
                        repairExam.Answer();
                        break;
                    }
                case EPropertyName.Skip:
                    {
                        repairExam.Skip();
                        if (repairExam.repairTaskWork)
                        {
                            repairExam.repairTaskWork.GotoNextStep();
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return repairExam;
        }

        /// <summary>
        /// 友好提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_propertyName);
        }
    }
}
