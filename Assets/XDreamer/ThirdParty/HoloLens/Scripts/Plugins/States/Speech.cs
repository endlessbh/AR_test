#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Components;

namespace XCSJ.PluginHoloLens
{
    [Serializable]
    [ComponentMenu("HoloLens/HoloLens语音识别", typeof(HoloLensManager))]
    [Name("HoloLens语音识别", nameof(Speech))]
    [XCSJ.Attributes.Icon(EIcon.Chat)]
    [KeyNode(nameof(ITrigger), "触发器")]
    [Tip("HoloLens语音识别组件是用于检测用户是否说出指定话语的触发器。当用户说出指定话语后，组件切换为完成态。", "Hololens speech recognition component is a trigger used to detect whether the user speaks a specified utterance. When the user utters the specified words, the component switches to the completed state.")]
    public class Speech : HoloLensStateComponent<Speech>, ITrigger
    {
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib("HoloLens", typeof(HoloLensManager))]
        [StateComponentMenu("HoloLens/HoloLens语音识别", typeof(HoloLensManager))]
#endif
        [Name("HoloLens语音识别", nameof(Speech))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("HoloLens语音识别组件是用于检测用户是否说出指定话语的触发器。当用户说出指定话语后，组件切换为完成态。", "Hololens speech recognition component is a trigger used to detect whether the user speaks a specified utterance. When the user utters the specified words, the component switches to the completed state.")]
        public static State CreateSpeech(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("关键词")]
        public List<string> keyWords = new List<string>();

        [Name("全局监听")]
        [Tip("勾选:HoloLens视点不需要聚焦游戏对象;不勾选:HoloLens视点必须聚焦到游戏对象上说话，才能识别;", "Check: the hololens viewpoint does not need to focus on the game object; Uncheck: the viewpoint of hololens must focus on the game object to speak before it can be recognized;")]
        public bool isGlobal = true;

        [Name("游戏对象")]
        [HideInSuperInspector(nameof(isGlobal), EValidityCheckType.Equal, true)]
        public GameObject gameObject = null;

        private InputListener speechListener;

        public override void OnBeforeEntry(StateData data)
        {
            base.OnBeforeEntry(data);

            if (HoloLensManager.instance && HoloLensManager.instance.hasAccess && DataValidity())
            {
                GameObject go = null;
                if (isGlobal)
                {
                    if (InputListener.global)
                    {
                        go = InputListener.global.gameObject;
                    }
                }
                else
                {
                    go = gameObject;
                }

                if (go)
                {
                    speechListener = CommonFun.GetOrAddComponent<InputListener>(go);
                    speechListener.AddKeyWords(this.keyWords, isGlobal);
                }
            }

            if (speechListener)
            {
                speechListener.onSpeechCallback += OnSpeechKeywordRecognized;
            }
        }

        public override void OnAfterExit(StateData data)
        {
            if (speechListener)
            {
                speechListener.onSpeechCallback -= OnSpeechKeywordRecognized;
                speechListener.RemoveKeyWords(this.keyWords, isGlobal);
            }

            base.OnAfterExit(data);
        }       

#if XDREAMER_HOLOLENS
        protected void OnSpeechKeywordRecognized(SpeechEventData eventData)
#else
        protected void OnSpeechKeywordRecognized(BaseEventData eventData)
#endif
        {
#if XDREAMER_HOLOLENS
            if (keyWords.Contains(eventData.RecognizedText))
#endif
            {
                finished = true;
            }
        }

        public override bool DataValidity()
        {
            return keyWords.Count > 0 && keyWords.Exists(kw=>!string.IsNullOrEmpty(kw));
        }

        public override string ToFriendlyString()
        {
            return keyWords.Count>0? keyWords[0]:"";
        }
    }
}
