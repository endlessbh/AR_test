using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using XCSJ.PluginCommonUtils;
using System;
using XCSJ.Attributes;

namespace XCSJ.Extension.Audios
{
    public static class AudioHelper
    {
        /// <summary>
        /// 创建AudioSource
        /// </summary>
        /// <param name="audioName"></param>
        /// <param name="audioClip"></param>
        /// <param name="loop"></param>
        /// <param name="playNow"></param>
        /// <returns></returns>
        public static AudioSource CreateAudioSource(string audioName, AudioClip audioClip, bool loop, bool playNow)
        {
            var audioSourceObject = UnityObjectHelper.CreateGameObject(audioName);
            var source = audioSourceObject.XAddComponent<AudioSource>();

            source.clip = audioClip;
            source.loop = loop;
            return source;
        }

        /// <summary>
        /// 播放临时音频
        /// </summary>
        /// <param name="audioClip">音频片段</param>
        /// <param name="position">位置</param>
        /// <param name="distanceRange">距离</param>
        /// <param name="volume">音量</param>
        /// <param name="audioMixer">混合音频组</param>
        public static void PlayAudioClip(AudioClip audioClip, Vector3 position, Vector2 distanceRange, float pitch, float volume = 1, AudioMixerGroup audioMixer = null)
        {
            var source = CreateAudioSource(audioClip, audioMixer, position, Quaternion.identity, distanceRange, volume);
            if (source)
            {
                GameObject.Destroy(source.gameObject, audioClip.length);
            }
        }

        /// <summary>
        /// 创建音频源
        /// </summary>
        /// <param name="audioMixer">混音对象</param>
        /// <param name="parent">父节点</param>
        /// <param name="distanceRange">音频距离</param>
        /// <param name="volume">音量</param>
        /// <param name="audioClip">音频剪辑片段</param>
        /// <param name="loop">循环播放</param>
        /// <param name="playNow">立即播放</param>
        /// <param name="groupName">音频组名：为空则不创建组</param>
        /// <returns></returns>
        public static AudioSource CreateAudioSource(AudioMixerGroup audioMixer, Transform parent, Vector2 distanceRange, float volume, AudioClip audioClip, bool loop, bool playNow, string groupName = "")
        {
            var source = CreateAudioSource(audioClip, audioMixer, parent.position, parent.rotation, distanceRange, volume, playNow, loop);
            if (source)
            {
                // 在父节点下再创建一个音频组，用于管理
                if (!string.IsNullOrEmpty(groupName))
                {
                    var group = parent.Find("音频组");
                    if (!group)
                    {
                        group = UnityObjectHelper.CreateGameObject("音频组").transform;
                        group.XSetTransformParent(parent);
                    }
                    source.transform.XSetTransformParent(group);
                }
                else
                {
                    source.transform.XSetTransformParent(parent);
                }
            }
            return source;
        }

        /// <summary>
        /// 创建音频源
        /// </summary>
        /// <param name="audioMixer">混音对象</param>
        /// <param name="parent">父节点</param>
        /// <param name="audioName">音源名称</param>
        /// <param name="minDistance">最小距离</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="volume">音量</param>
        /// <param name="audioClip">音频剪辑片段</param>
        /// <param name="loop">循环播放</param>
        /// <param name="playNow">立即播放</param>
        /// <returns></returns>
        public static AudioSource CreateAudioSource(AudioClip audioClip, AudioMixerGroup audioMixer, Vector3 position, Quaternion rotation, Vector2 distanceRange, float volume, bool playNow = true, bool loop = false)
        {
            var source = CreateAudioSource(audioClip, audioMixer);
            if (source)
            {
                source.transform.position = position;
                source.transform.rotation = rotation;

                source.minDistance = distanceRange.x;
                source.maxDistance = distanceRange.y;
                source.volume = volume;
                source.clip = audioClip;
                source.loop = loop;
                source.dopplerLevel = .5f;

                source.spatialBlend = (distanceRange.x == 0 && distanceRange.y == 0) ? 0f : 1f;

                source.playOnAwake = playNow;
                if (playNow)
                {
                    source.Play();
                }
            }
            return source;
        }

        private static AudioSource CreateAudioSource(AudioClip audioClip, AudioMixerGroup audioMixer)
        {
            if (!audioClip) return null;

            var audioSourceObject = UnityObjectHelper.CreateGameObject(audioClip.name);
            var source = audioSourceObject.XAddComponent<AudioSource>();
            if (audioMixer) source.outputAudioMixerGroup = audioMixer;

            return source;
        }

        /// <summary>
        /// 增加高频过滤，用于涡轮
        /// </summary>
        public static void CreateHighPassFilter(AudioSource source, float freq, int level)
        {
            if (source == null) return;

            var highFilter = source.gameObject.AddComponent<AudioHighPassFilter>();
            highFilter.cutoffFrequency = freq;
            highFilter.highpassResonanceQ = level;
        }

        /// <summary>
        /// 增加低频过滤，用于发动机关闭
        /// </summary>
        public static void CreateLowPassFilter(AudioSource source, float freq)
        {
            if (source == null) return;

            var lowFilter = source.gameObject.AddComponent<AudioLowPassFilter>();
            lowFilter.cutoffFrequency = freq;
        }
    }

}