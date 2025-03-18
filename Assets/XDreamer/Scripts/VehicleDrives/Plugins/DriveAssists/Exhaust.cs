using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
	/// <summary>
	/// 排放系统 ：使用驱动力计算并控制排气的粒子系统
	/// /// </summary>
	[RequireComponent(typeof(ParticleSystem))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
	[Name("排气系统")]
	[Tip("使用驱动力计算并控制排气的粒子系统", "A particle system that uses the driving force to calculate and control the exhaust")]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    public class Exhaust : VehicleDriverGetter
    {
        /// <summary>
        /// 发射粒子范围
        /// </summary>
        [Group("粒子配置")]
        [Name("发射粒子范围")]
        public Vector2 _emissionRange = new Vector2(5f, 50f);

        /// <summary>
        /// 发射粒子尺寸
        /// </summary>
        [Name("发射粒子尺寸")]
        public Vector2 _sizeRange = new Vector2(2.5f, 5f);

        /// <summary>
        /// 发射速度范围
        /// </summary>
        [Name("发射速度范围")]
        public Vector2 _speedRange = new Vector2(.5f, 5f);

        private ParticleSystem particle;
	    private ParticleSystem.EmissionModule emission;

        [Name("火焰粒子")]
        public ParticleSystem flame;
	    private ParticleSystem.EmissionModule subEmission;

        [Group("光晕")]
        [Name("光晕")]
        public LensFlare lensFlare;
        private Light flameLight;

        /// <summary>
        /// 明亮度
        /// </summary>
        [Name("光晕明亮度")]
        public float flareBrightness = 1f;
	    private float finalFlareBrightness;

        /// <summary>
        /// 火焰颜色
        /// </summary>
        [Name("正常颜色")]
        public Color flameColor = Color.red;

        /// <summary>
        /// 增加动力后火焰颜色
        /// </summary>
        [Name("动力增强颜色")]
        public Color boostFlameColor = Color.blue;
	
	    private float flameTime = 0f;

		private IVehicleBoost[] _vehicleBoosts = null;

		/// <summary>
		/// 开始
		/// </summary>
		protected override void Awake()
	    {
			base.Awake();

	        particle = GetComponent<ParticleSystem>();
	        emission = particle.emission;
	
	        if (flame)
	        {
	            subEmission = flame.emission;
	            flameLight = flame.GetComponentInChildren<Light>();
	        }
	
	        if (flameLight)
	        {
	            flameLight.flare = null;
	        }

			_vehicleBoosts = GetComponentsInParent<IVehicleBoost>();
            if (_vehicleBoosts == null|| _vehicleBoosts.Length==0)
            {
				_vehicleBoosts = GetComponentsInChildren<IVehicleBoost>();
			}
		}
	
		/// <summary>
		/// 更新
		/// </summary>
	    protected void Update()
	    {
            if (!vehicleDriver || vehicleDriver.engine == null || !particle) return;

            // 烟雾
            Smoke();

			// 火焰
	        Flame();
	
			// 光晕
	        LensFlare();
	    }
	
	    private void Smoke()
	    {
	        if (vehicleDriver.engine.running)
	        {
	            var main = particle.main;
	
	            if (vehicleDriver.speed < 50)
	            {
	                emission.enabled = true;
	
	                if (vehicleDriver.throttleValue > .35f)
	                {
	                    emission.rateOverTime = _emissionRange.y;
	                    main.startSpeed = _speedRange.y;
	                    main.startSize = _sizeRange.y;
	                }
	                else
	                {
	                    emission.rateOverTime = _emissionRange.x;
	                    main.startSpeed = _speedRange.x;
	                    main.startSize = _sizeRange.x;
	                }
	                return;
	            }
	        }
			emission.enabled = false;
		}

		private bool IsVehicleBoost()
        {
            foreach (var item in _vehicleBoosts)
            {
                if (item.boost)
                {
					return true;
                }
            }
			return false;
        }

		/// <summary>
		/// 火焰工作回调函数
		/// </summary>
		public event Action<bool> onFlameWork = null;

		private void Flame()
	    {
	        if (vehicleDriver.engine.running)
	        {
	            if (vehicleDriver.throttleValue >= .25f) flameTime = 0f;

				bool boost = IsVehicleBoost();
	            if ((vehicleDriver.engine.RPM >= 5000 && vehicleDriver.engine.RPM <= 5500  
					&& vehicleDriver.throttleValue <= .25f && flameTime <= .5f)  || boost)
	            {
	                flameTime += Time.deltaTime;
					FlameOn(boost);
				}
	            else
	            {
                    FlameOff();
                }
	        }
	        else
	        {
				if (emission.enabled)
				{
					emission.enabled = false;
				}
				FlameOff();
			}
	    }

		private void FlameOn(bool boost)
        {
            subEmission.enabled = true;

            if (flameLight) flameLight.intensity = 3f * UnityEngine.Random.Range(.25f, 1f);

            var main = flame.main;
            main.startColor = boost && flame ? boostFlameColor : flameColor;
            flameLight.color = main.startColor.color;

			onFlameWork?.Invoke(true);
        }

		private void FlameOff()
        {
            subEmission.enabled = false;

            if (flameLight) flameLight.intensity = 0f;

            onFlameWork?.Invoke(false);
        }
	
	    private void LensFlare()
	    {
	        if (!lensFlare) return;
	        var cam = CameraHelperExtension.currentCamera;
	        if (!cam) return;
	
	        float distanceTocam = Vector3.Distance(transform.position, cam.transform.position);
	        float angle = Vector3.Angle(transform.forward, cam.transform.position - transform.position);
	
	        if (angle != 0) finalFlareBrightness = flareBrightness * (4 / distanceTocam) * ((100f - 1.11f * angle) / 100f) / 2f;
	
	        lensFlare.brightness = finalFlareBrightness * flameLight.intensity;
	        lensFlare.color = flameLight.color;
	    }
	
	}
}
