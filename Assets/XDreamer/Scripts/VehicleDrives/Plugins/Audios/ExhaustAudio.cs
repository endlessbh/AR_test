using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Audios;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.DriveAssists;

namespace XCSJ.PluginVehicleDrive.Audios
{
    [Name("排气音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireComponent(typeof(Exhaust))]
    [DisallowMultipleComponent]
    [RequireManager(typeof(VehicleDriveManger))]
    public class ExhaustAudio : VehicleAudioGetter
    {
        /// <summary>
        /// 排气音频
        /// </summary>
        [Name("排气音频")]
        public AudioClip[] exhaustFlameClips;
        private AudioSource _exhaustSource;

        private Exhaust _exhaust = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            _exhaust = GetComponent<Exhaust>();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (vehicleAudio)
            {
                _exhaustSource = AudioHelper.CreateAudioSource(vehicleAudio.audioMixer, transform, new Vector2(10f, 25f), 1f, exhaustFlameClips[0], false, false);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if(_exhaust) _exhaust.onFlameWork += OnFlameWork;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_exhaust) _exhaust.onFlameWork -= OnFlameWork;
        }

        private void OnFlameWork(bool work)
        {
            if (!_exhaust || !_exhaustSource) return;

            if (work)
            {
                if (!_exhaustSource.isPlaying)
                {
                    _exhaustSource.clip = exhaustFlameClips[UnityEngine.Random.Range(0, exhaustFlameClips.Length)];
                    _exhaustSource.Play();
                }
            }
            else
            {
                if (_exhaustSource.isPlaying) _exhaustSource.Stop();
            }
        }
    }
}
