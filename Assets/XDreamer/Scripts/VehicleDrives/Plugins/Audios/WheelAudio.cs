using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Audios;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.Audios
{
    /// <summary>
    /// 车轮音频
    /// </summary>
    [Name("车轮音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(WheelDriver))]
    [RequireComponent(typeof(WheelDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    public class WheelAudio : VehicleAudioGetter
    {
        /// <summary>
        /// 车轮碰撞
        /// </summary>
        [Name("车轮碰撞")]
        public AudioClip _bumpAudioClip = null;

        /// <summary>
        /// 播放碰撞音频的力
        /// </summary>
        [Name("播放碰撞音频的力")]
        [Tip("车轮当前受力与上一次受力的差值大于当前值时播放音频", "When the difference between the current force on the wheel and the last force is greater than the current value, the audio will be played")]
        [Range(10, 20000)]
        public float _playBumpAudioForce = 5000;

        /// <summary>
        /// 轮胎打滑音频
        /// </summary>
        [Name("轮胎打滑")]
        public AudioClip _tireSkidAudioClip = null;
        private AudioSource tireSkidAudioSource;

        // 碰撞力
        private float lastBumpForce = -1;

        private WheelDriver wheelDriver;

        protected override void Start()
        {
            base.Start();

            wheelDriver = GetComponent<WheelDriver>();

            // 创建划痕音频
            if (_vehicleAudio && _tireSkidAudioClip)
            {
                tireSkidAudioSource = AudioHelper.CreateAudioSource(vehicleAudio.audioMixer, wheelDriver.transform, new Vector2(5f, 50f), 0f, _tireSkidAudioClip, true, false);
                tireSkidAudioSource.transform.position = transform.position;
            }
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected void FixedUpdate()
        {
            if (wheelDriver.currentFrictions == null) return;

            if (tireSkidAudioSource)
            {
                // 获取地面音频信息
                _tireSkidAudioClip = wheelDriver.currentFrictions.groundSound;

                tireSkidAudioSource.volume = 0f;
                if (wheelDriver.totalSlip > wheelDriver.currentFrictions.slip)
                {
                    if (!tireSkidAudioSource.isPlaying) tireSkidAudioSource.Play();

                    if (wheelDriver.vehicleDriver.velocity.magnitude > 1f)
                    {
                        var audioVolume = wheelDriver.currentFrictions.volume;
                        tireSkidAudioSource.volume = Mathf.Lerp(0f, audioVolume, wheelDriver.totalSlip);
                        tireSkidAudioSource.pitch = Mathf.Lerp(1f, .8f, tireSkidAudioSource.volume);
                    }
                }
                else
                {
                    if (tireSkidAudioSource.volume <= .05f && tireSkidAudioSource.isPlaying) tireSkidAudioSource.Stop();
                }
            }

            // 碰撞力音频
            if (_bumpAudioClip)
            {
                // 计算碰撞力
                var bumpForce = wheelDriver.wheelHit.force - lastBumpForce - _playBumpAudioForce;

                //	碰撞力足够大，播放音频
                if (bumpForce >= 0 && lastBumpForce > 0)
                {
                    AudioHelper.PlayAudioClip(_bumpAudioClip, wheelDriver.vehicleDriver.transform.position, new Vector2(5, 50), UnityEngine.Random.Range(.9f, 1.1f), bumpForce / 3000f, vehicleAudio.audioMixer);
                }
                lastBumpForce = wheelDriver.wheelHit.force;
            }
        }      
    }
}