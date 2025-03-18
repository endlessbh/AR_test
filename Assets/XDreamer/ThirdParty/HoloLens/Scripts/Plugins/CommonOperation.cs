using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if XDREAMER_HOLOLENS
using HoloToolkit.Unity.InputModule;
#endif
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginHoloLens
{
    public class CommonOperation
    {
#if XDREAMER_HOLOLENS
        public static SpeechInputSource FindOrCreateSpeechInputSource()
        {
            var hololens = GetHoloLensManager();
            if(!hololens)
            {
                return null;
            }

            var sis = GameObject.FindObjectOfType<SpeechInputSource>();
            if (!sis)
            {
                var go = CommonFun.CreateGameObjectWithUniqueName(hololens.gameObject, "语音输入源");
                if(go) sis = go.AddComponent<SpeechInputSource>();
            }
            return sis;
        }

        public static void AddKeyWords(SpeechInputSource speechInputSource, IEnumerable<string> keyWords)
        {
            if (!speechInputSource) return;

            List<KeywordAndKeyCode> inputSourceKeywordList = new List<KeywordAndKeyCode>();
            if (speechInputSource.Keywords != null)
            {
                inputSourceKeywordList = speechInputSource.Keywords.ToList();
            }

            foreach (var kw in keyWords)
            {
                if (!string.IsNullOrEmpty(kw) && !inputSourceKeywordList.Exists(k => k.Keyword == kw))
                {
                    inputSourceKeywordList.Add(new KeywordAndKeyCode() { Keyword = kw });
                }
            }

            speechInputSource.Keywords = inputSourceKeywordList.ToArray();
        }

        public static void RemoveKeyWord(SpeechInputSource speechInputSource, IEnumerable<string> keyWords)
        {
            if (!speechInputSource) return;

            List<KeywordAndKeyCode> inputSourceKeywordList = new List<KeywordAndKeyCode>();
            if (speechInputSource.Keywords != null)
            {
                inputSourceKeywordList = speechInputSource.Keywords.ToList();
            }

            foreach (var kw in keyWords)
            {
                int index = inputSourceKeywordList.FindIndex(sk => sk.Keyword == kw);
                if (index>=0)
                {
                    inputSourceKeywordList.RemoveAt(index);
                }
            }

            speechInputSource.Keywords = inputSourceKeywordList.ToArray();
        }

        public static HoloLensManager GetHoloLensManager()
        {
            var hololens = HoloLensManager.instance;
            if (hololens && hololens.hasAccess)
            {
                return hololens;
            }
            else
            {
                Debug.Log("HoloLensManager对象无效或无权限");
                return null;
            }
        }
#endif

    }
}
