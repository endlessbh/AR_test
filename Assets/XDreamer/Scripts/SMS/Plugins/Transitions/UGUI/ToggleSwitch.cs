using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginSMS.States.UGUI;
using XCSJ.PluginSMS.Transitions.Base;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("UGUI/Toggle切换", typeof(SMSManager))]
    [Name("Toggle切换", "ToggleSwitch")]
    public class ToggleSwitch : TransitionComponent
    {
        [Name("Toggle控件")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(Toggle))]
        public Toggle toggle;

        [Name("进入时规则")]
        [EnumPopup]
        public EToggleEntryRule entryRule = EToggleEntryRule.None;

        [Name("退出时规则")]
        [EnumPopup]
        public EToggleEntryRule exitRule = EToggleEntryRule.None;

        [Name("触发类型")]
        [EnumPopup]
        public EToggleTriggerType triggerType = EToggleTriggerType.Switch;

        public override bool Finished()
        {
            if (!toggle) return false;

            switch (triggerType)
            {
                case EToggleTriggerType.None: return true;
                case EToggleTriggerType.On: return toggle.isOn;
                case EToggleTriggerType.Off: return !toggle.isOn;
                case EToggleTriggerType.Switch:
                case EToggleTriggerType.SwitchOn:
                case EToggleTriggerType.SwitchOff: return finished;
                default: return false;
            }
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            if (toggle)
            {
                HandleRule(entryRule);

                toggle.onValueChanged.AddListener(OnValueChanged);
            }
        }

        public override void OnExit(StateData data)
        {
            if (toggle)
            {
                toggle.onValueChanged.RemoveListener(OnValueChanged);

                HandleRule(exitRule);
            }
            base.OnExit(data);
        }

        protected void HandleRule(EToggleEntryRule rule)
        {
            switch (rule)
            {
                case EToggleEntryRule.On:
                    {
                        toggle.isOn = true;
                        break;
                    }
                case EToggleEntryRule.Off:
                    {
                        toggle.isOn = false;
                        break;
                    }
                case EToggleEntryRule.Switch:
                    {
                        toggle.isOn = !toggle.isOn;
                        break;
                    }
            }
        }

        private void OnValueChanged(bool value)
        {
            switch (triggerType)
            {
                case EToggleTriggerType.Switch:
                    {
                        finished = true;
                        break;
                    }
                case EToggleTriggerType.SwitchOn:
                    {
                        if (toggle.isOn) finished = true;
                        break;
                    }
                case EToggleTriggerType.SwitchOff:
                    {
                        if (!toggle.isOn) finished = true;
                        break;
                    }
            }
        }

        public override bool DataValidity()
        {
            return toggle;
        }

        public override string ToFriendlyString()
        {
            return (toggle ? toggle.name : "") + " " + CommonFun.Name(triggerType);
        }
    }
}
