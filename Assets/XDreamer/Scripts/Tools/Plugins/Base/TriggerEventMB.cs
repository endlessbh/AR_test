using System;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Base
{
    /// <summary>
    /// 触发事件组件
    /// </summary>
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public abstract class TriggerEventMB : InteractProvider
    {
        /// <summary>
        /// 目标对象：触发某些特殊事件时的目标对象
        /// </summary>
        [Group("触发设置")]
        [Name("目标对象")]
        [Tip("触发某些特殊事件时的目标对象", "The target object when certain special events are triggered")]
        [ObjectPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public UnityEngine.Object _targetObject;

        /// <summary>
        /// 目标对象
        /// </summary>
        public UnityEngine.Object targetObject
        {
            get
            {
                if (!_targetObject)
                {
                    this.XModifyProperty(ref _targetObject, gameObject);
                }
                return _targetObject;
            }
        }

        /// <summary>
        /// 触发事件模式
        /// </summary>
        [Name("触发事件模式")]
        [Tip("触发事件模式", "Trigger event mode")]
        [EnumPopup]
        public ETriggerEventMode _triggerEventMode = ETriggerEventMode.TargetMouseUpAsButton;

        /// <summary>
        /// 当回调时触发事件模式
        /// </summary>
        public ETriggerEventMode triggerEventModeOnCallback { get; private set; } = ETriggerEventMode.None;

        /// <summary>
        /// 开始
        /// </summary>
        public virtual void Start()
        {
            CheckTrigger(ETriggerEventMode.Start, true);
        }        

        /// <summary>
        /// 当启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            CheckTrigger(ETriggerEventMode.Enable, true);
            StartCheckTrigger();
        }

        /// <summary>
        /// 当禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            StopCheckTrigger();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (targetObject) { }
        }

        private Transform GetTargetTransform()
        {
            var targetObject = this.targetObject;
            if (!targetObject) return default;

            if (targetObject is GameObject go && go)
            {
                return go.transform;
            }
            else if (targetObject is Component c && c)
            {
                return c.transform;
            }

            return default;
        }

        /// <summary>
        /// 分析触发事件模式
        /// </summary>
        /// <returns></returns>
        public ETriggerEventMode AnalyseTriggerEventMode()
        {
            var targetObject = this.targetObject;
            if (!targetObject) return ETriggerEventMode.None;

            var targetTransform = GetTargetTransform();
            if (targetTransform)
            {
                if (targetTransform.GetComponent<MouseUpAsButtonEvent>() || targetTransform.GetComponent<Collider>())
                {
                    return ETriggerEventMode.TargetMouseUpAsButton;
                }
                if (targetTransform.GetComponent<Button>())
                {
                    return ETriggerEventMode.TargetButtonClick;
                }
            }

            return ETriggerEventMode.Enable;
        }

        private void StartCheckTrigger()
        {
            switch (_triggerEventMode)
            {
                case ETriggerEventMode.TargetMouseUpAsButton:
                    {
                        CheckTrigger(_triggerEventMode, false);

                        var targetTransform = GetTargetTransform();
                        if (targetTransform && targetTransform.XGetOrAddComponent<MouseUpAsButtonEvent>() is MouseUpAsButtonEvent button && button)
                        {
                            this.mouseUpAsButtonEvent = button;
                            MouseUpAsButtonEvent.onMouseUpAsButton += InternalOnMouseUpAsButton;
                        }
                        break;
                    }
                case ETriggerEventMode.TargetButtonClick:
                    {
                        CheckTrigger(_triggerEventMode, false);

                        var targetTransform = GetTargetTransform();
                        if (targetTransform && targetTransform.XGetOrAddComponent<Button>() is Button button && button)
                        {
                            this.button = button;
                            button.onClick.AddListener(OnButtonClick);
                        }
                        break;
                    }
            }
        }

        private void StopCheckTrigger()
        {
            triggerEventModeOnCallback = ETriggerEventMode.None;
            if (button)
            {
                button.onClick.RemoveListener(OnButtonClick);
                button = null;
            }
            if(mouseUpAsButtonEvent)
            {
                MouseUpAsButtonEvent.onMouseUpAsButton -= InternalOnMouseUpAsButton;
            }
        }

        private MouseUpAsButtonEvent mouseUpAsButtonEvent;

        private void InternalOnMouseUpAsButton(MouseUpAsButtonEvent mouseUpAsButtonEvent)
        {
            if (!this.mouseUpAsButtonEvent) return;
            if (mouseUpAsButtonEvent != this.mouseUpAsButtonEvent) return;
            InternalCallTriggerEvent();
        }

        private Button button;

        private void OnButtonClick()
        {
            if (triggerEventModeOnCallback != ETriggerEventMode.TargetButtonClick) return;
            InternalCallTriggerEvent();
        }

        /// <summary>
        /// 当触发事件发生时
        /// </summary>
        public static event Action<TriggerEventMB> onTriggerEvent;

        /// <summary>
        /// 检查触发
        /// </summary>
        /// <param name="triggerEventMode"></param>
        /// <param name="callback"></param>
        private bool CheckTrigger(ETriggerEventMode triggerEventMode, bool callback)
        {
            //触发事件模式不可用
            if (triggerEventMode == ETriggerEventMode.None) return false;

            //要求触发事件模式一致
            if (triggerEventMode != _triggerEventMode) return false;

            if (triggerEventModeOnCallback == ETriggerEventMode.None || triggerEventModeOnCallback == triggerEventMode)
            {
                //置过触发事件模式
                triggerEventModeOnCallback = triggerEventMode;

                //回调处理
                if (callback)
                {
                    InternalCallTriggerEvent();
                }

                return true;
            }

            return false;
        }

        private void InternalCallTriggerEvent()
        {
            OnTriggerEvent();
            onTriggerEvent?.Invoke(this);
        }

        /// <summary>
        /// 调用事件触发：要求<see cref="_triggerEventMode"/>值必须为<see cref="ETriggerEventMode.CustomTrigger"/>时方能触发
        /// </summary>
        public void CallTrigerEvent() => CheckTrigger(ETriggerEventMode.CustomTrigger, true);

        /// <summary>
        /// 当触发事件时
        /// </summary>
        protected abstract void OnTriggerEvent();
    }

    /// <summary>
    /// 触发事件模式
    /// </summary>
    [Name("触发事件模式")]
    public enum ETriggerEventMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 开始:当前组件对象开始时触发
        /// </summary>
        [Name("开始")]
        [Tip("当前组件对象开始时触发", "Triggered when the current component object starts")]
        Start,

        /// <summary>
        /// 启用:当前组件对象启用时触发
        /// </summary>
        [Name("启用")]
        [Tip("当前组件对象启用时触发", "Triggered when the current component object is enabled")]
        Enable,

        /// <summary>
        /// 自定义触发:由用户自行调用触发
        /// </summary>
        [Name("自定义触发")]
        [Tip("由用户自行调用触发", "Triggered by the user's own call")]
        CustomTrigger,

        /// <summary>
        /// 目标鼠标弹起作为按钮
        /// </summary>
        [Name("目标鼠标弹起作为按钮")]
        TargetMouseUpAsButton,

        /// <summary>
        /// 目标按钮点击
        /// </summary>
        [Name("目标按钮点击")]
        TargetButtonClick,
    }
}
