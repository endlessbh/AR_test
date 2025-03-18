using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Extension.Audios;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.Audios
{
    [Name("转向灯音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireComponent(typeof(VehicleLightController))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class IndicatorLightAudio : VehicleAudioGetter
    {
        /// <summary>
        /// 指示灯音频片段
        /// </summary>
        [Name("指示灯音频")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AudioClip _indicatorClip;

        private AudioSource _indicatorSound;

        [Name("指示灯间隔音频")]
        public float _delayTime = 1.5f;

        private float timeCounter = 0;

        private VehicleLightController vehicleLightController = null;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            vehicleLightController = GetComponent<VehicleLightController>();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (vehicleLightController) vehicleLightController.onIndicatorChanged += PlayOrStopIndicatorSound;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (vehicleLightController) vehicleLightController.onIndicatorChanged -= PlayOrStopIndicatorSound;
        }

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (vehicleAudio)
            {
                _indicatorSound = AudioHelper.CreateAudioSource(vehicleAudio.audioMixer, transform, new Vector2(1f, 30f), 1, _indicatorClip, false, false);
            }
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected void FixedUpdate()
        {
            if (play)
            {
                timeCounter += Time.deltaTime;

                if (timeCounter > _delayTime)
                {
                    timeCounter = 0;
                    if (!_indicatorSound.isPlaying) _indicatorSound.Play();
                }
            }
        }

        private bool play = false;

        /// <summary>
        /// 播放或停止音频
        /// </summary>
        /// <param name="play"></param>
        private void PlayOrStopIndicatorSound(bool play)
        {
            if (!_indicatorSound) return;

            this.play = play;
            if (play)
            {
                if (!_indicatorSound.isPlaying) _indicatorSound.Play();
            }
            else
            {
                timeCounter = 0;
                if (_indicatorSound.isPlaying) _indicatorSound.Stop();
            }
        }
    }
}
