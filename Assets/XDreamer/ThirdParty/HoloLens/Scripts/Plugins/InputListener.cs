#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginHoloLens
{
    /// <summary>
    /// 输入监听器
    /// </summary>
    [Name("输入监听器")]
    [RequireManager(typeof(HoloLensManager))]
    public class InputListener : InteractProvider
#if XDREAMER_HOLOLENS
        , IFocusable, IInputHandler, IInputClickHandler, IManipulationHandler, ISpeechHandler
#endif
    {
        private const string GlobalInputListenerName = "全局输入监听";

        public static InputListener global
        {
            get
            {
                if (!_global && HoloLensManager.instance && HoloLensManager.instance.hasAccess)
                {
                    if (!(_global = HoloLensManager.instance.transform.GetComponentInChildren<InputListener>()))
                    {
                        _global = (new GameObject(GlobalInputListenerName)).AddComponent<InputListener>();
                        _global.transform.SetParent(HoloLensManager.instance.transform);
                    }
                }
                return _global;
            }
        }
        private static InputListener _global;

        #region 聚焦

        public event Action onFocusEnterCallback;
        public event Action onFocusExitCallback;

        public void OnFocusEnter()
        {
            onFocusEnterCallback?.Invoke();
        }

        public void OnFocusExit()
        {
            onFocusExitCallback?.Invoke();
        }

        #endregion

        #region 点击

#if XDREAMER_HOLOLENS
        public event Action<InputEventData> onDownCallback;
#else
        public event Action<BaseEventData> onDownCallback;
#endif

#if XDREAMER_HOLOLENS
        public event Action<InputEventData> onUpCallback;
#else
        public event Action<BaseEventData> onUpCallback;
#endif

#if XDREAMER_HOLOLENS
        public event Action<InputClickedEventData> onClickCallback;
#else
        public event Action<BaseEventData> onClickCallback;
#endif

#if XDREAMER_HOLOLENS
        void IInputHandler.OnInputDown(InputEventData eventData)
#else
        void OnInputDown(BaseEventData eventData)
#endif
        {
#if XDREAMER_HOLOLENS
            InteractionInputSource inputSource = eventData.InputSource as InteractionInputSource;
            if (inputSource != null)
            {
                switch (eventData.PressType)
                {
                    case InteractionSourcePressInfo.Grasp:
                        inputSource.StartHaptics(eventData.SourceId, 1.0f);
                        return;
                    case InteractionSourcePressInfo.Menu:
                        inputSource.StartHaptics(eventData.SourceId, 1.0f, 1.0f);
                        return;
                }
            }
#endif
            onDownCallback?.Invoke(eventData);
        }

#if XDREAMER_HOLOLENS
        void IInputHandler.OnInputUp(InputEventData eventData)
#else
        void OnInputUp(BaseEventData eventData)
#endif
        {
#if XDREAMER_HOLOLENS
                InteractionInputSource inputSource = eventData.InputSource as InteractionInputSource;
            if (inputSource != null)
            {
                if (eventData.PressType == InteractionSourcePressInfo.Grasp)
                {
                    inputSource.StopHaptics(eventData.SourceId);
                }
            }
#endif
            onUpCallback?.Invoke(eventData);
        }

#if XDREAMER_HOLOLENS
        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
#else
        void OnInputClicked(BaseEventData eventData)
#endif
        {
            onClickCallback?.Invoke(eventData);
        }

        #endregion

        #region 拖拽

        private static GameObject dragGameObject = null;

        // 对象移动前的位置
        private Vector3 originPosition;

        [Name("启用拖拽")]
        public bool dragEnable = false;

#if XDREAMER_HOLOLENS
        public void OnManipulationCanceled(ManipulationEventData eventData) => EndDrag(eventData);
#else
        public void OnManipulationCanceled(BaseEventData eventData) => EndDrag(eventData);
#endif

#if XDREAMER_HOLOLENS
        public void OnManipulationCompleted(ManipulationEventData eventData) => EndDrag(eventData);
#else
        public void OnManipulationCompleted(BaseEventData eventData) => EndDrag(eventData);
#endif

#if XDREAMER_HOLOLENS
        public void OnManipulationStarted(ManipulationEventData eventData) => StartDrag(eventData);
#else
        public void OnManipulationStarted(BaseEventData eventData) => StartDrag(eventData);
#endif

#if XDREAMER_HOLOLENS
        public void OnManipulationUpdated(ManipulationEventData eventData) => Draging(eventData);
#else
        public void OnManipulationUpdated(BaseEventData eventData) => Draging(eventData);
#endif

#if XDREAMER_HOLOLENS
        private void StartDrag(ManipulationEventData eventData)
#else
        private void StartDrag(BaseEventData eventData)
#endif
        {
            if (dragEnable && !dragGameObject)
            {
#if XDREAMER_HOLOLENS
                InputManager.Instance.PushModalInputHandler(gameObject);
#endif
                // 开始移动前，保存原始位置
                originPosition = transform.position;

                dragGameObject = gameObject;

                eventData.Use();
            }
        }

#if XDREAMER_HOLOLENS
        private void Draging(ManipulationEventData eventData)
#else
        private void Draging(BaseEventData eventData)
#endif
        {
            if (dragEnable && gameObject == dragGameObject)
            {
#if XDREAMER_HOLOLENS
                transform.position = originPosition + eventData.CumulativeDelta;
#endif
                eventData.Use();
            }
        }

#if XDREAMER_HOLOLENS
        private void EndDrag(ManipulationEventData eventData)
#else
        private void EndDrag(BaseEventData eventData)
#endif
        {
            if (dragEnable && dragGameObject)
            {
#if XDREAMER_HOLOLENS
                InputManager.Instance.PopModalInputHandler();
#endif
                dragGameObject = null;

                eventData.Use();
            }
        }

        #endregion

        #region 语音识别

        [Name("关键字列表")]
        public List<string> keyWords = new List<string>();

#if XDREAMER_HOLOLENS
        public event Action<SpeechEventData> onSpeechCallback;
#else
        public event Action<BaseEventData> onSpeechCallback;
#endif

        public void AddKeyWords(IEnumerable<string> keyWords, bool isGlobal)
        {
#if XDREAMER_HOLOLENS
            if (InputManager.Instance && isGlobal)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }

            foreach (var kw in keyWords)
            {
                if (!string.IsNullOrEmpty(kw))
                {
                    this.keyWords.Add(kw);
                }
            }
            CommonOperation.AddKeyWords(CommonOperation.FindOrCreateSpeechInputSource(), keyWords);
#endif
        }

        public void RemoveKeyWords(IEnumerable<string> keyWords, bool isGlobal)
        {
#if XDREAMER_HOLOLENS
            if (InputManager.Instance && isGlobal)
            {
                InputManager.Instance.RemoveGlobalListener(gameObject);
            }

            foreach (var kw in keyWords)
            {
                this.keyWords.Remove(kw);
            }

            CommonOperation.RemoveKeyWord(CommonOperation.FindOrCreateSpeechInputSource(), keyWords);
#endif
        }

#if XDREAMER_HOLOLENS
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
#else
        public void OnSpeechKeywordRecognized(BaseEventData eventData)
#endif
        {
#if XDREAMER_HOLOLENS
            if (keyWords.Contains(eventData.RecognizedText))
#endif
            {
                onValidSpeechRecognized?.Invoke(this, eventData);
                onSpeechCallback?.Invoke(eventData);
            }
#if XDREAMER_HOLOLENS
            else
#endif
            {
                onInvalidSpeechRecognized?.Invoke(this, eventData);
            }

        }

#if XDREAMER_HOLOLENS
        public static event Action<InputListener, SpeechEventData> onValidSpeechRecognized;
#else
        public static event Action<InputListener, BaseEventData> onValidSpeechRecognized;
#endif

#if XDREAMER_HOLOLENS
        public static event Action<InputListener, SpeechEventData> onInvalidSpeechRecognized;
#else
        public static event Action<InputListener, BaseEventData> onInvalidSpeechRecognized;
#endif
#endregion

    }
}
