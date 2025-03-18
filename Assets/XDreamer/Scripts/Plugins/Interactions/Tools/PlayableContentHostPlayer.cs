using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Interfaces;
using static XCSJ.PluginTimelines.Tools.PlayableContentSet;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 可播放内容宿主播放器：可播放存放在其他组件上的内容, 也同时可被其他播放器所控制
    /// </summary>
    public abstract class PlayableContentHostPlayer : PlayableContent, IPlayableContentHost
    {

    }
}
