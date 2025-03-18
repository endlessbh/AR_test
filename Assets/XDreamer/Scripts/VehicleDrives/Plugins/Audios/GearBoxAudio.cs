using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Audios;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.Audios
{
    /// <summary>
    /// 变速箱音频
    /// </summary>
    [Name("变速箱音频")]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(GearBox))]
    [RequireManager(typeof(VehicleDriveManger))]
    [RequireComponent(typeof(GearBox))]
    public class GearBoxAudio : VehicleAudioGetter
    {
        private GearBox gearBox = null;

        /// <summary>
        /// 变速箱
        /// </summary>
        [Name("变速箱")]
        public AudioClip[] gearShiftingClips;
        private AudioSource gearShiftingSound;

        /// <summary>
        /// 倒车
        /// </summary>
        [Name("倒车")]
        public AudioClip reversingClip;
        private AudioSource reversingSound;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected void Awake()
        {
            gearBox = GetComponent<GearBox>();

            reversingSound = AudioHelper.CreateAudioSource(_vehicleAudio.audioMixer, transform, new Vector2(1, 10), 0, reversingClip, true, false);
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            gearBox.onChangeGear += OnChangeGearBox;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            gearBox.onChangeGear -= OnChangeGearBox;
        }

        /// <summary>
        /// 固定更新
        /// </summary>
        protected void FixedUpdate()
        {
            if (!reversingSound) return;

            if (gearBox.vehicleDriver.direction == -1)
            {
                if (!reversingSound.isPlaying) reversingSound.Play();

                reversingSound.volume = Mathf.Lerp(0f, 1f, gearBox.vehicleDriver.speed / gearBox.currentGear.targetSpeedForNextGear);
                reversingSound.pitch = reversingSound.volume;
            }
            else
            {
                if (reversingSound.isPlaying) reversingSound.Stop();
            }
        }

        private void OnChangeGearBox()
        {
            if (gearShiftingClips.Length > 0)
            {
                AudioHelper.PlayAudioClip(gearShiftingClips[UnityEngine.Random.Range(0, gearShiftingClips.Length)], transform.position, new Vector2(1, 5), 1, 1, _vehicleAudio.audioMixer);
            }
        }
    }
}
