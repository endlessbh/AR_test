using UnityEngine;
using UnityEngine.Audio;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 播放音频
    /// </summary>
    [Name("播放音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    public class PlayAudio : BaseEffect
    {
        /// <summary>
        /// 音频剪辑
        /// </summary>
        [Name("音频剪辑")]
        public AudioClip _audioClip;

        /// <summary>
        /// 混合音频
        /// </summary>
        [Name("混合音频")]
        public AudioMixerGroup _audioMixer;

        /// <summary>
        /// 音量
        /// </summary>
        [Name("音量")]
        [Range(0, 1)]
        public float _volume = 0.5f;

        /// <summary>
        /// 播放最小最大距离
        /// </summary>
        [Name("播放最小最大距离")]
        [LimitRange(0, 10000)]
        [HideInInspector]
        public Vector2 _distanceRange = new Vector2(0, 10);

        [Name("音高变化量范围")]
        [HideInInspector]
        public Vector2 _patchRandomRange = new Vector2(0.9f, 1.1f);

        /// <summary>
        /// 启用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void EnableEffect(InteractData interactData, GameObject gameObject)
        {
            if (!gameObject) return;
            if (interactData.TryGetPosition(out var position, true))
            {
                Play(gameObject, position, _volume);
            }
        }

        /// <summary>
        /// 禁用特效
        /// </summary>
        /// <param name="interactData"></param>
        /// <param name="gameObject"></param>
        public override void DisableEffect(InteractData interactData, GameObject gameObject)
        {
        }

        /// <summary>
        /// 播放音频剪辑
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="volume">音量</param>
        private void Play(GameObject gameObject, Vector3 position, float volume)
        {
            var audioClip = GetInteractPropertyValueByRule(gameObject)?.audioClip;
            if (!audioClip) audioClip = _audioClip;

            if (audioClip)
            {
                AudioSource.PlayClipAtPoint(audioClip, position, _volume);
            }
        }
    }
}
