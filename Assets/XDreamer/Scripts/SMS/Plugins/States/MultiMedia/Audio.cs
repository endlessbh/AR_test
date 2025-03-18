using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.MultiMedia
{
    /// <summary>
    /// 音频:音频组件是播放声音的对象。播放完音频之后，组件切换为完成态。
    /// </summary>
    [ComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(Audio))]
    [Tip("音频组件是播放声音的对象。播放完音频之后，组件切换为完成态。", "The audio component is the object of playing sound. After playing the audio, the component switches to the finished state.")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    public class Audio : WorkClip<Audio>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "音频";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(SMSHelperExtension.MultiMediaCategoryName, typeof(SMSManager))]
        [StateComponentMenu(SMSHelperExtension.MultiMediaCategoryName + "/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(Audio))]
        [Tip("音频组件是播放声音的对象。播放完音频之后，组件切换为完成态。", "The audio component is the object of playing sound. After playing the audio, the component switches to the finished state.")]
        [XCSJ.Attributes.Icon(EIcon.Audio)]
        public static State CreateAudio(IGetStateCollection obj) => CreateNormalState(obj);

        [Group("音频属性")]
        [Name("音频源")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup(typeof(AudioSource))]
        public AudioSource audioSource;

        [Name("时间容差")]
        [Tip("当音频当前播放时间与期望时间在时间容差内时，不更新音频的播放时间;", "When the current playing time of the audio is within the time tolerance of the expected time, the playing time of the audio is not updated;")]
        [Range(0.001f, 1)]
        public float timeTolerance = 0.1f;

        public bool invalid => !audioSource || !audioSource.clip || MathX.ApproximatelyZero(timeLength) || MathX.ApproximatelyZero(audioSource.clip.length);

        // 设置百分比时，如果声音没有播放，则设定播放
        public bool triggerPlayWhenSetPercent { get; set; } = true;

        public void Play()
        {
            if (invalid) return;
            audioSource.Play();
        }

        public void Stop()
        {
            if (invalid) return;
            audioSource.time = 0;
            audioSource.Stop();
        }

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            if (stateData == null || stateData.workMode == EWorkMode.Simulate) return;
            Play();
        }

        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (stateData == null || stateData.workMode == EWorkMode.Simulate) return;
            if (invalid) return;
            var p = percent.percent01OfWorkCurve;
            if (MathX.Approximately(p, 1) || MathX.Approximately(p, 0))
            {
                Stop();
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    var time = p * audioSource.clip.length;
                    if (!MathX.Approximately(time, audioSource.time, timeTolerance))
                    {
                        //Log.DebugFormat("--->percent:{0}, time:{1}, audioSource.time: {2}", p, time, audioSource.time);
                        audioSource.time = (float)time;
                    }
                }
                else if (triggerPlayWhenSetPercent
                      && stateData != null
                      && stateData.workMode != EWorkMode.Simulate)
                {
                    //Log.Debug(stateData.workMode);
                    //Log.Debug(stateData.workState.GetNamePath());
                    Play();
                }
            }
        }

        public override void OnExit(StateData stateData)
        {
            
            if (stateData == null || stateData.workMode == EWorkMode.Simulate) return;
            Stop();
            base.OnExit(stateData);
        }

        public override void Reset(ResetData stateData)
        {
            base.Reset(stateData);
            switch (stateData.dataRule)
            {
                case EDataRule.Init:
                case EDataRule.Entry:
                    {
                        Stop();
                        break;
                    }
            }
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && !invalid;
        }

        public override string ToFriendlyString()
        {
            return audioSource ? audioSource.name : "";
        }

        public override void OnPlayerStateChanged(IWorkClipPlayer player, EPlayerState lastPlayerState)
        {
            base.OnPlayerStateChanged(player, lastPlayerState);
            switch(player.playerState)
            {
                case EPlayerState.Play:
                    {
                        if (parent.isActive || parent.busy)
                        {
                            Play();
                        }
                        break;
                    }
                case EPlayerState.Pause:
                    {
                        if (invalid) return;
                        audioSource.Pause();
                        break;
                    }
                case EPlayerState.Resume:
                    {
                        if (invalid) return;
                        audioSource.UnPause();
                        break;
                    }
                case EPlayerState.Stop:
                    {
                        Stop();
                        break;
                    }
            }
        }
    }
}
