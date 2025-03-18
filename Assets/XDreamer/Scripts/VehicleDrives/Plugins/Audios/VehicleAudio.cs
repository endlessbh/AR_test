using UnityEngine;
using UnityEngine.Audio;
using XCSJ.Attributes;
using XCSJ.Extension.Audios;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginVehicleDrive.Base;

namespace XCSJ.PluginVehicleDrive.Audios
{
    /// <summary>
    /// 车辆音频
    /// </summary>
    [Name("车辆音频")]
    [XCSJ.Attributes.Icon(EIcon.Audio)]
    [DisallowMultipleComponent]
    [RequireManager(typeof(VehicleDriveManger))]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(VehicleDriver))]
    public class VehicleAudio : VehicleDriverGetter
    {
        /// <summary>
        /// 车辆混合音频
        /// </summary>
        [Name("车辆混合音频")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public AudioMixerGroup audioMixer;

        /// <summary>
        /// 发动机启动
        /// </summary>
        [Group("发动机")]
		[Name("启动")]
	    public AudioClip engineStartClip;

        /// <summary>
        /// 发动机停止
        /// </summary>
        [Name("停止")]
        public AudioClip engineStopClip;

        /// <summary>
        /// 发动机空转
        /// </summary>
        [Name("空转")]
        public AudioClip engineClipIdle;
        private AudioSource engineSoundIdle;

        [Name("运转音频数量")]
        [EnumPopup]
        public EEngineRunningAudioType _engineRunningAudioType = EEngineRunningAudioType.One;

        /// <summary>
        /// 发动机高速运转
        /// </summary>
        [Name("高速运转")]
        public AudioClip engineClipHigh;
        internal AudioSource engineSoundHigh;

        /// <summary>
        /// 发动机高速低频
        /// </summary>
        [Name("高速运转低频")]
        public AudioClip engineClipHighOff;
        internal AudioSource engineSoundHighOff;

        /// <summary>
        /// 发动机中速运转
        /// </summary>
        [Name("中速运转")]
        [HideInSuperInspector(nameof(_engineRunningAudioType), EValidityCheckType.Equal | EValidityCheckType.Or, EEngineRunningAudioType.One, nameof(_engineRunningAudioType), EValidityCheckType.Equal, EEngineRunningAudioType.Two)]
        public AudioClip engineClipMed;
        private AudioSource engineSoundMed;

        /// <summary>
        /// 发动机中速低频
        /// </summary>
        [Name("中速运转低频")]
        [HideInSuperInspector(nameof(_engineRunningAudioType), EValidityCheckType.Equal | EValidityCheckType.Or, EEngineRunningAudioType.One, nameof(_engineRunningAudioType), EValidityCheckType.Equal, EEngineRunningAudioType.Two)]
        public AudioClip engineClipMedOff;
        internal AudioSource engineSoundMedOff;

        /// <summary>
        /// 发动机低速运转
        /// </summary>
        [Name("低速运转")]
        [HideInSuperInspector(nameof(_engineRunningAudioType), EValidityCheckType.Equal, EEngineRunningAudioType.One)]
        public AudioClip engineClipLow;
        private AudioSource engineSoundLow;

        /// <summary>
        /// 发动机低速低频
        /// </summary>
        [Name("低速运转低频")]
        [HideInSuperInspector(nameof(_engineRunningAudioType), EValidityCheckType.Equal, EEngineRunningAudioType.One)]
        public AudioClip engineClipLowOff;
        internal AudioSource engineSoundLowOff;

        /// <summary>
        /// 风
        /// </summary>
        [Name("风")]
        [Group("其他")]
        public AudioClip windClip;
        private AudioSource windSound;

        /// <summary>
        /// 刹车音频
        /// </summary>
        [Name("刹车音频")]
        public AudioClip brakeClip;
        private AudioSource brakeSound;

        /// <summary>
        /// 发动机音频震荡区间
        /// </summary>
        [Name("发动机音频震荡区间")]
		public Vector2 engineSoundPitchRange = new Vector2(.75f, 1.75f);

        /// <summary>
        /// 唤醒
        /// </summary>
        protected override void Awake()
	    {
            base.Awake();

	        CreateAudios();
	    }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
	    {
            base.OnEnable();
            VehicleDriver.onVehicleStateChanged += OnEngineStartOrStop;
	    }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
	    {
            base.OnDisable();
            VehicleDriver.onVehicleStateChanged -= OnEngineStartOrStop;
	    }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
	    {
			if (!vehicleDriver) return;

            if (windSound)
            {
                windSound.volume = Mathf.Lerp(0f, 1, vehicleDriver.speed / 300f);
                windSound.pitch = UnityEngine.Random.Range(.9f, 1f);
            }

            if (vehicleDriver.direction == 1)
            {
                brakeSound.volume = Mathf.Lerp(0f, 1f, GetBrakePower());
            }
            else
            {
                brakeSound.volume = 0f;
            }
        }

        private float GetBrakePower()
        {
            if (!vehicleDriver || vehicleDriver.wheelDrivers.Count == 0 || vehicleDriver.brake==null) return 0;

            float totalbrakeTorque = 0;
            foreach (var wc in vehicleDriver.wheelDrivers)
            {
                totalbrakeTorque += wc.wheelCollider.brakeTorque;
            }

            return Mathf.Clamp01(totalbrakeTorque / (vehicleDriver.brake.torque * vehicleDriver.wheelDrivers.Count)) * Mathf.Lerp(0f, 1f, vehicleDriver.wheelDrivers[0].wheelCollider.rpm / 50f);
        }


        /// <summary>
        /// 固定更新
        /// </summary>
        protected override void OnFixedUpdate()
	    {
	        EngineSounds();
	    }
	
	    private void CreateAudios()
	    {
			var range1 = new Vector2(5, 50);
			var range2 = new Vector2(5, 25);

			engineSoundHigh = AudioHelper.CreateAudioSource(audioMixer, transform, range1, 0, engineClipHigh, true, true);
	        engineSoundMed = AudioHelper.CreateAudioSource(audioMixer, transform, range1, 0, engineClipMed, true, true);
	        engineSoundLow = AudioHelper.CreateAudioSource(audioMixer, transform, range2, 0, engineClipLow, true, true);

            engineSoundHighOff = AudioHelper.CreateAudioSource(audioMixer, transform, range1, 0, engineClipHigh, true, true);
            engineSoundMedOff = AudioHelper.CreateAudioSource(audioMixer, transform, range1, 0, engineClipMed, true, true);
            engineSoundLowOff = AudioHelper.CreateAudioSource(audioMixer, transform, range2, 0, engineClipLow, true, true);

            AudioHelper.CreateLowPassFilter(engineSoundHighOff, 3000f);
            AudioHelper.CreateLowPassFilter(engineSoundMedOff, 3000f);
            AudioHelper.CreateLowPassFilter(engineSoundLowOff, 3000f);

            engineSoundIdle = AudioHelper.CreateAudioSource(audioMixer, transform, range2, 0, engineClipIdle, true, true);
            windSound = AudioHelper.CreateAudioSource(audioMixer, transform, new Vector2(1, 10), 0, windClip, true, false);

	        brakeSound = AudioHelper.CreateAudioSource(audioMixer, transform, new Vector2(1, 10), 0, brakeClip, true, true);

        }
		
	    /// <summary>
	    /// 发动机声音
	    /// </summary>
	    private void EngineSounds()
	    {
            if (vehicleDriver.engine == null) return;

            var percent = vehicleDriver.engine.RPM / vehicleDriver.engine.maxRPM;
            var t1 = percent * 2f;

			bool lessHalfMax = t1 < 1;
			var t2 = percent;

			var lowRPM = lessHalfMax ? Mathf.Lerp(0f, 1f, t1) : Mathf.Lerp(1f, .25f, t2);
			lowRPM = Mathf.Clamp01(lowRPM);

			var medRPM = lessHalfMax ? Mathf.Lerp(-.5f, 1f, t1) : Mathf.Lerp(1f, .5f, t2);
			medRPM = Mathf.Clamp01(medRPM);

			var highRPM = Mathf.Lerp(-1f, 1f, t2);
	        highRPM = Mathf.Clamp01(highRPM);
	
	        var volume = Mathf.Clamp(vehicleDriver.throttleValue, 0f, 1f);
	        var pitch = Mathf.Lerp(engineSoundPitchRange.x, engineSoundPitchRange.y, t2) * (vehicleDriver.engine.running ? 1f : 0f);

            var offVolume = (1f - volume);

            switch (_engineRunningAudioType)
            {
                case EEngineRunningAudioType.One:
                    {
                        Play(engineSoundHigh, highRPM * volume, pitch);
                        Play(engineSoundHighOff, highRPM * offVolume, pitch);
                        break;
                    }
                case EEngineRunningAudioType.Two:
                    {
                        Play(engineSoundHigh, highRPM * volume, pitch);
                        Play(engineSoundLow, lowRPM * volume, pitch);

                        Play(engineSoundHighOff, highRPM * offVolume, pitch);
                        Play(engineSoundLowOff, lowRPM * offVolume, pitch);
                        break;
                    }
                case EEngineRunningAudioType.Three:
                    {
                        Play(engineSoundHigh, highRPM * volume, pitch);
                        Play(engineSoundMed, medRPM * volume, pitch);
                        Play(engineSoundLow, lowRPM * volume, pitch);

                        Play(engineSoundHighOff, highRPM * offVolume, pitch);
                        Play(engineSoundMedOff, medRPM * offVolume, pitch);
                        Play(engineSoundLowOff, lowRPM * offVolume, pitch);
                        break;
                    }
                default:
                    break;
            }

            Play(engineSoundIdle, Mathf.Lerp(vehicleDriver.engine.running ? 1 : 0f, 0f, t2), pitch);
		}

        private void Play(AudioSource audioSource, float volume, float pitch)
        {
			if (audioSource)
            {
				audioSource.volume = volume;
				audioSource.pitch = pitch;
                if (!audioSource.isPlaying) audioSource.Play();
            }
        }
	
	    private void OnEngineStartOrStop(VehicleDriver vehicleController, VehicleEventArgs vehiclePartEventArgs)
	    {
            if (vehicleDriver != vehicleController) return;

            var engineArg = vehiclePartEventArgs as EngineEventArgs;
            if (engineArg != null)
            {
                AudioHelper.PlayAudioClip(engineArg.running ? engineStartClip : engineStopClip, transform.position, new Vector2(1, 50), 1, 1, audioMixer);
            }
		}

        [Name("发动机运转音频类型")]
        public enum EEngineRunningAudioType
        {
            [Name("单")]
            One,

            [Name("双")]
            Two,

            [Name("三")]
            Three,
        }
	}

    /// <summary>
    /// 车辆音频获取器
    /// </summary>
    /// <typeparam name="TComponent1"></typeparam>
    public abstract class VehicleAudioGetter : BaseVehicle
    {
        /// <summary>
        /// 车辆音频
        /// </summary>
        [Name("车辆音频")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleAudio _vehicleAudio = null;

        /// <summary>
        /// 车辆控制父对象 
        /// </summary>
        public VehicleAudio vehicleAudio => this.XGetComponentInParentOrGlobal(ref _vehicleAudio);

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            this.XGetComponentInParentOrGlobal(ref _vehicleAudio);
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            this.XGetComponentInParentOrGlobal(ref _vehicleAudio);
        }
    }
}
