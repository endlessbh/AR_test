using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;

namespace XCSJ.PluginSMS.States.Show.UI
{
    /// <summary>
    /// 步骤数量界面
    /// </summary>
    [Name("步骤数量界面")]
    [RequireComponent(typeof(Text))]
    public class GUIStepCount : GUIStepGroupInfo
    {
        private Text text = null;

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        protected void Awake()
        {
            text = GetComponent<Text>();
        }

        private StepGroup lastGroup = null;

        /// <summary>
        /// 当步骤变化时回调
        /// </summary>
        /// <param name="group"></param>
        protected override void OnStepChanged(StepGroup group)
        {
            if (!group) return;

            if (lastGroup!= group)
            {
                text.text = group.count.ToString();
            }
            lastGroup = group;
        }
    }
}
