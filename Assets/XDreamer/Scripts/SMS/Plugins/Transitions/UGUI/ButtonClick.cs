using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.UGUI;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("跳过/按钮点击", typeof(SMSManager))]
    [Name("按钮点击")]
    public class ButtonClick : Trigger
    {
        /// <summary>
        /// 按钮
        /// </summary>
        [Name("按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup()]
        public Button button;

        private Button _buttonOnEntry;

        /// <summary>
        /// 当重置
        /// </summary>
        /// <param name="data"></param>
        public override void Reset(ResetData data)
        {
            base.Reset(data);

            _checkTriggerRule = ECheckTriggerRule.Always;
        }

        /// <summary>
        /// 当创建时
        /// </summary>
        public override void OnCreated()
        {
            base.OnCreated();

            _checkTriggerRule = ECheckTriggerRule.Always;
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if (button)
            {
                _buttonOnEntry = button;
                _buttonOnEntry.onClick.AddListener(OnClicked);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            base.OnExit(data);

            if (_buttonOnEntry)
            {
                _buttonOnEntry.onClick.RemoveListener(OnClicked);
                _buttonOnEntry = null;
            }
        }

        private void OnClicked() => SetTrigger();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => button;

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => button ? button.name : "";
    }
}
