using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.Transitions.Base;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginSMS.Transitions.UGUI
{
    [ComponentMenu("跳过/UI事件触发器简版", typeof(SMSManager))]
    [Name("UI事件触发器简版")]
    public class UGUIEventTriggerLite : Trigger
    {
        [Name("UGUI对象")]
        [ComponentPopup()]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform _rectTransform;

        [Name("触发事件类型(简版)")]
        [EnumPopup]
        public EEventTriggerTypeLite eventTriggerTypeLite;

        private EventTrigger eventTriggerOnEntry;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public override void OnEntry(StateData data)
        {
            base.OnEntry(data);

            if (_rectTransform)
            {
                eventTriggerOnEntry = _rectTransform.gameObject.AddComponent<EventTrigger>();

                XGUIHelper.AddEventTrigger(eventTriggerOnEntry, eventTriggerTypeLite.ToEventTriggerType(), OnEventTrigger);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="data"></param>
        public override void OnExit(StateData data)
        {
            if (eventTriggerOnEntry) GameObject.Destroy(eventTriggerOnEntry);

            base.OnExit(data);
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _rectTransform;

        /// <summary>
        /// UI事件触发
        /// </summary>
        /// <param name="eventData"></param>
        protected virtual void OnEventTrigger(BaseEventData eventData) => SetTrigger();

        /// <summary>
        /// 友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _rectTransform ? _rectTransform.name : "";
    }
}
