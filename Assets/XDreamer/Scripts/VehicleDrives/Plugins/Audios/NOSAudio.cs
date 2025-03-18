using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Audios;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.DriveAssists;

namespace XCSJ.PluginVehicleDrive.Audios
{
    /// <summary>
    /// 氮氧增压系统音频
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NOS))]
    [Name("氮氧增压系统音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class NOSAudio : VehicleAudioGetter
    {
        /// <summary>
        /// 燃烧音频
        /// </summary>
        [Name("燃烧音频")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AudioClip _NOSClip;
        private AudioSource _NOSSound;

        /// <summary>
        /// 停止注入音频
        /// </summary>
        [Name("停止注入音频")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AudioClip _blowoutClip;
        private AudioSource _blowSound;

        private NOS nos = null;

        /// <summary>
        /// 开始
        /// </summary>
        protected override void Start()
        {
            base.Start();

            nos = GetComponent<NOS>();

            if (!vehicleAudio) return;

            if (_NOSClip )
            {
                _NOSSound = AudioHelper.CreateAudioSource(vehicleAudio.audioMixer, transform, new Vector2(5, 10), 1f, _NOSClip, true, false);
            }

            if (_blowoutClip)
            {
                _blowSound = AudioHelper.CreateAudioSource(vehicleAudio.audioMixer, transform, new Vector2(1, 10), 1, _blowoutClip, false, false);
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if(nos) nos.onNOSWork += OnNOSWorking;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (nos) nos.onNOSWork -= OnNOSWorking;
        }

        /// <summary>
        /// NOS工作状态回调
        /// </summary>
        /// <param name="working"></param>
        protected void OnNOSWorking(bool enable)
        {           
            if (enable)
            {
                if (_NOSSound && !_NOSSound.isPlaying) _NOSSound.Play();
            }
            else
            {
                if (_NOSSound && _NOSSound.isPlaying)
                {
                    _NOSSound.Stop();

                    if (_blowSound && !_blowSound.isPlaying)
                    {
                        _blowSound.Play();
                    }
                }
            }
        }
    }
}
