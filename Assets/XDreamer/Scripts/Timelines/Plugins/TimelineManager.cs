using System;
using System.Runtime.InteropServices;
using UnityEngine.Video;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.PluginTimelines
{
    /// <summary>
    /// 时间轴：用于管理维护Unity中可时间轴化对象的管理器插件；包括：视频、音频、序列帧、GIF、状态机时间轴等；
    /// </summary>
    [Name(TimelineManager.Title)]
    [Tip("用于管理维护Unity中可时间轴化对象的管理器插件；包括：视频、音频、序列帧、GIF、状态机时间轴等；", "A manager plug-in for managing and maintaining timeline objects in unity; Including: video, audio, sequence frame, GIF, state machine timeline, etc;")]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOption.Optional)]
    [Guid("4F9529C6-8355-4FB4-900D-04789A627E6C")]
    [Serializable]
    [Version("23.428")]
    public class TimelineManager : BaseManager<TimelineManager, EScriptID>
    {
        public const string Title = "时间轴";

        public const string ToolLib = "多媒体";

        public const string PlayableContent = "播放内容";

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(EScriptID id, ScriptParamList param)
        {
            switch (id)
            {
                case EScriptID.HandleVideo:
                    {
                        var videoPlayer = param[1] as VideoPlayer;
                        if (!videoPlayer) break;

                        switch (param[2] as string)
                        {
                            case "播放":
                                {
                                    videoPlayer.Play();
                                    return ReturnValue.Yes;
                                }
                            case "暂停":
                                {
                                    videoPlayer.Pause();
                                    return ReturnValue.Yes;
                                }
                            case "停止":
                                {
                                    videoPlayer.Stop();
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EScriptID.SetVideoSpeed:
                    {
                        var videoPlayer = param[1] as VideoPlayer;
                        if (!videoPlayer) break;

                        videoPlayer.playbackSpeed = (float)param[2];
                        return ReturnValue.Yes;
                    }
                case EScriptID.SetVideoProgress:
                    {
                        var videoPlayer = param[1] as VideoPlayer;
                        if (!videoPlayer) break;

                        videoPlayer.frame = (int)((float)param[2] * videoPlayer.frameCount);
                        return ReturnValue.Yes;
                    }
                case EScriptID.GetVideoProgress:
                    {
                        var videoPlayer = param[1] as VideoPlayer;
                        if (!videoPlayer) break;

                        return ReturnValue.True((videoPlayer.frame * 1D / videoPlayer.frameCount).ToString());
                    }
                case EScriptID.GetVideoLength:
                    {
                        var videoPlayer = param[1] as VideoPlayer;
                        if (!videoPlayer) break;

                        return ReturnValue.True((videoPlayer.length).ToString());
                    }
                case EScriptID.GetVideoPlayingTime:
                    {
                        var videoPlayer = param[1] as VideoPlayer;
                        if (!videoPlayer) break;

                        return ReturnValue.True(((videoPlayer.time)).ToString());
                    }
            }
            return ReturnValue.No;
        }
    }
}
