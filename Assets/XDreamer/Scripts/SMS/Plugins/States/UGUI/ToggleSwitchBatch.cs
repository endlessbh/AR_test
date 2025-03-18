using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.UGUI
{
    /// <summary>
    /// Toggle切换批量:Toggle批量切换组件是多个Toggle中任意一个开关状态符合设定状态的触发器。当条件满足时，符合条件的Toggle将以对象完整路径（Unity层级树路径）存储于指定的全局变量中，组件切换为完成态。
    /// </summary>
    [ComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(ToggleSwitchBatch))]
    [Tip("Toggle批量切换组件是多个Toggle中任意一个开关状态符合设定状态的触发器。当条件满足时，符合条件的Toggle将以对象完整路径（Unity层级树路径）存储于指定的全局变量中，组件切换为完成态。", "Toggle batch switching component is a trigger whose switching state of any one of multiple toggle conforms to the set state. When the conditions are met, the qualified toggle will be stored in the specified global variable as the object full path (unity hierarchical tree path), and the component will be switched to the completed state.")]
    [XCSJ.Attributes.Icon(index = 33610)]
    public class ToggleSwitchBatch : ToggleTrigger<ToggleSwitchBatch>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Toggle切换批量";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("UGUI", typeof(SMSManager))]
        [StateComponentMenu("UGUI/"+ Title, typeof(SMSManager))]
        [Name(Title)]
        [Tip("Toggle批量切换组件是多个Toggle中任意一个开关状态符合设定状态的触发器。当条件满足时，符合条件的Toggle将以对象完整路径（Unity层级树路径）存储于指定的全局变量中，组件切换为完成态。", "Toggle batch switching component is a trigger whose switching state of any one of multiple toggle conforms to the set state. When the conditions are met, the qualified toggle will be stored in the specified global variable as the object full path (unity hierarchical tree path), and the component will be switched to the completed state.")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State CreateToggleSwitchBatch(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("Toggle控件列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [Readonly(EEditorMode.Runtime)]
        [Array]
        public List<Toggle> toggles = new List<Toggle>();

        [Name("Toggle变量")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string toggleVariable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref toggleVariable);
        }

        #endregion

        public override bool Finished()
        {
            switch (triggerType)
            {
                case EToggleTriggerType.None: return true;
                case EToggleTriggerType.On:
                    {
                        return toggles.Any(toggle =>
                        {
                            if (toggle.isOn)
                            {
                                toggleVariable.TrySetOrAddSetHierarchyVarValue(CommonFun.GameObjectComponentToString(toggle));
                                return true;
                            }
                            return false;
                        });
                    }
                case EToggleTriggerType.Off:
                    {
                        return toggles.Any(toggle =>
                        {
                            if (!toggle.isOn)
                            {
                                toggleVariable.TrySetOrAddSetHierarchyVarValue(CommonFun.GameObjectComponentToString(toggle));
                                return true;
                            }
                            return false;
                        });
                    }
                case EToggleTriggerType.Switch:
                case EToggleTriggerType.SwitchOn:
                case EToggleTriggerType.SwitchOff: return finished;

                default: return false;
            }
        }

        private bool isModifying = false;

        public override bool Init(StateData data)
        {
            HandleRule(initRule);
            toggles.ForEach(toggle => toggle.onValueChanged.AddListener(b => OnValueChanged(toggle, b)));
            return base.Init(data);
        }

        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);
            isModifying = true;
            HandleRule(entryRule);
            isModifying = false;

        }

        public override void OnExit(StateData data)
        {
            //toggles.ForEach(toggle => toggle.onValueChanged.RemoveListener(OnValueChanged));
            isModifying = true;
            HandleRule(exitRule);
            isModifying = false;
            base.OnExit(data);
        }

        protected override void HandleRule(EToggleEntryRule rule)
        {
            switch (rule)
            {
                case EToggleEntryRule.On:
                    {
                        toggles.ForEach(toggle => toggle.isOn = true);
                        break;
                    }
                case EToggleEntryRule.Off:
                    {
                        toggles.ForEach(toggle => toggle.isOn = false);
                        break;
                    }
                case EToggleEntryRule.Switch:
                    {
                        toggles.ForEach(toggle => toggle.isOn = !toggle.isOn);
                        break;
                    }
            }
        }

        private void OnValueChanged(Toggle toggle,bool value)
        {
            if (finished || isModifying || !parent.isActive) return;

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
                default:return;
            }

            if(finished)
            {
                toggleVariable.TrySetOrAddSetHierarchyVarValue(CommonFun.GameObjectComponentToString(toggle));
            }

        }

        public override bool DataValidity() => toggles.Count > 0;

        public override string ToFriendlyString()
        {
            return toggles.Count + " " + CommonFun.Name(triggerType);
        }
    }    
}
