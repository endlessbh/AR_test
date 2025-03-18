using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{
#if UNITY_5_6_OR_NEWER
    [Obsolete("本类不再使用", true)]
#endif
    [Serializable]
    [Name("运行时信息")]
    public class RuntimeInfo
    {
        [Name("默认发送消息")]
        public bool defaultSendMessage = true;

        [Name("信息")]
        [OnlyMemberElements]
        public List<Info> infos = new List<Info>();

        public void Reset()
        {
            infos.Add(new Info());
        }

        public bool NeedSendMessage()
        {
            foreach (var info in infos)
            {
                if (info.runtimePlatform == Application.platform) return info.sendMessage;
            }
            return defaultSendMessage;
        }

        [Name("信息")]
        [Serializable]
        public class Info
        {
            [Name("运行时平台")]
            public RuntimePlatform runtimePlatform = RuntimePlatform.WebGLPlayer;

            [Name("发送消息")]
            public bool sendMessage = false;
        }
    }
}
