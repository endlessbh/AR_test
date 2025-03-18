using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginRepairman.States.Exam
{
    /// <summary>
    /// 问题方格
    /// </summary>
    [Name("问题方格")]
    [RequireManager(typeof(RepairmanManager))]
    public class UIQuestionCell : InteractProvider
    {
        [Name("题目文字描述")]
        public Text questionText;

        [Name("题目图片")]
        public Image img;

        private UIQuestionTable questionTable;

        private IQuestion question;

        protected void Reset() => FindComponents();

        protected void Awake() => FindComponents();

        private void FindComponents()
        {
            if (!questionText)
            {
                questionText = GetComponent<Text>();
            }

            if (!img)
            {
                img = GetComponent<Image>();
            }
        }

        public void SetData(IQuestion question, int questionIndex, UIQuestionTable questionTable)
        {
            this.questionTable = questionTable;
            this.question = question;

            transform.SetSiblingIndex(questionIndex);
            if (questionText)
            {
                questionText.text = (questionIndex + 1).ToString();
            }
        }

        private EQuestionState oldState = EQuestionState.Unfinish;

        protected void Update()
        {
            if (question!=null && question.state!=oldState)
            {
                //Debug.Log("ResetState ==> oldState:" + oldState + ",question.state"+ question.state);
                oldState = question.state;
                SetState(oldState);
            }
        }

        public void ResetState()
        {
            //Debug.Log("ResetState ==> GUIQuestionBox:" + question.description+ ",state:"+ oldState);
            SetState(EQuestionState.Unfinish);
            //Debug.Log("ResetState ==> state:" + oldState);
        }

        private void SetState(EQuestionState questionState)
        {
            oldState = questionState;

            if (!img) return;

            switch (oldState)
            {
                case EQuestionState.Current:
                    {
                        img.color = questionTable.currentColor;
                        break;
                    }
                case EQuestionState.Unfinish:
                    {
                        img.color = questionTable.unfinishColor;
                        break;
                    }
                case EQuestionState.Right:
                    {
                        img.color = questionTable.rightColor;
                        break;
                    }
                case EQuestionState.Wrong:
                    {
                        img.color = questionTable.wrongColor;
                        break;
                    }
            }
        }
    }
}
