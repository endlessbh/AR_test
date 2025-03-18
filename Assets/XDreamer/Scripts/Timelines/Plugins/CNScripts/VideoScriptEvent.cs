using System;
using UnityEngine;
using UnityEngine.Video;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.PluginTimelines.CNScripts
{
    /// <summary>
    /// 视频脚本事件
    /// </summary>
    [Name("视频脚本事件")]
    public enum EVideoScriptEventType
    {
        [Name("视频开始时执行")]
        VideoStarted = 0,

        [Name("播放结束时执行")]
        videoEnded,
    }

    /// <summary>
    /// 视频脚本事件函数
    /// </summary>
    [Serializable]
    [Name("视频脚本事件函数")]
    public class VideoScriptEventFunction : EnumFunction<EVideoScriptEventType> { }

    /// <summary>
    /// 视频脚本事件函数集合
    /// </summary>
    [Serializable]
    [Name("视频脚本事件函数集合")]
    public class VideoScriptEventFunctionCollection : EnumFunctionCollection<EVideoScriptEventType, VideoScriptEventFunction> { }

    /// <summary>
    /// 视频脚本事件
    /// </summary>
    [Name("视频脚本事件")]
    [Serializable]
    [RequireComponent(typeof(VideoPlayer))]
    [DisallowMultipleComponent]
    public class VideoScriptEvent : BaseScriptEvent<EVideoScriptEventType, VideoScriptEventFunction, VideoScriptEventFunctionCollection>
    {
        private VideoPlayer _videoPlayer;

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            if (_videoPlayer)
            {
                _videoPlayer.started += VideoStarted;
                _videoPlayer.loopPointReached += VideoEnded;
            }
            else
            {
                Log.Error(string.Format("{0}不包含VideoPlayer组件", gameObject.name));
            }
        }

        void VideoStarted(VideoPlayer source)
        {
            ExecuteScriptEvent(EVideoScriptEventType.VideoStarted);
        }

        void VideoEnded(VideoPlayer source)
        {
            ExecuteScriptEvent(EVideoScriptEventType.videoEnded);
        }
    }
}
