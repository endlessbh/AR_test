using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginsCameras;

namespace XCSJ.PluginVehicleDrive.Controllers
{
    /// <summary>
    /// 车灯 ：控制车灯的亮灭与闪烁
    /// </summary>
    [RequireComponent(typeof(Light))]
    [Name("车灯")]
    [Tip("控制车灯的亮灭与闪烁", "Control the light on, off and flashing")]
    [XCSJ.Attributes.Icon(EIcon.VehicleLight)]
    [Tool("车辆驾驶组件", rootType = typeof(VehicleDriveManger))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class VehicleLight : BaseVehicle
    {
        /// <summary>
        /// 车灯控制器
        /// </summary>
        [Name("车灯控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public VehicleLightController _vehicleLightController = null;

        /// <summary>
        /// 灯类型
        /// </summary>
        [Name("灯类型")]
        [EnumPopup]
        public ELightType lightType = ELightType.LowBeamHead;

        /// <summary>
        /// 灯光强度
        /// </summary>
        [Name("灯光强度")]
	    public float _intensity = 1f;

        /// <summary>
        /// 灯光亮灭速度
        /// </summary>
        [Name("灯光亮灭速度")]
        public float _onOffSpeed = 1f;

        /// <summary>
        /// 光晕
        /// </summary>
        [Name("光晕")]
        public Flare _flare;

        /// <summary>
        /// 闪耀明亮度
        /// </summary>
        [Name("闪耀明亮度")]
        [HideInSuperInspector(nameof(_flare), EValidityCheckType.Null)]
        public float flareBrightness = 1.5f;
        private float finalFlareBrightness;

        private Light _light;
        private LensFlare lensFlare;
        private TrailRenderer trail;

        /// <summary>
        /// 闪烁频率
        /// </summary>
        [Name("闪烁频率")]
        [HideInSuperInspector(nameof(_flare), EValidityCheckType.Null)]
        public int refreshRate = 30;
	    private float refreshTimer = 0f;

	    private VehicleLightController vehicleLightController => this.XGetComponentInParentOrGlobal(ref _vehicleLightController);

        /// <summary>
        /// 重置
        /// </summary>
        protected void Reset()
        {
            this.XGetComponentInParentOrGlobal(ref _vehicleLightController);

            CheckLensFlare();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
	    {
            this.XGetComponentInParentOrGlobal(ref _vehicleLightController);

            _light = GetComponent<Light>();
	        lensFlare = GetComponent<LensFlare>();
	        trail = GetComponentInChildren<TrailRenderer>();
	
	        if (lensFlare)
	        {
	            if (_light.flare != null) _light.flare = null;
	        }
		
	        CheckLensFlare();
	    }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
	    {
            base.OnEnable();
	        if (!_light) _light = GetComponent<Light>();
	
	        _light.intensity = 0f;

            vehicleLightController.onIndicatorChanged += OnIndicator;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            vehicleLightController.onIndicatorChanged -= OnIndicator;
        }

        private void OnIndicator(bool isOn)
        {
            indicatorTimer = 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
	    {
            // 更新灯光
			Light();

            // 更新光晕
			LensFlare();

            // 更新拖尾效果
            Trail();
        }

		private void Light()
        {
            switch (lightType)
            {
                case ELightType.LowBeamHead: Lighting(vehicleLightController.lowBeamHeadLightsOn ? _intensity : 0f); break;
                case ELightType.HighBeamHead: Lighting(vehicleLightController.highBeamHeadLightsOn ? _intensity : 0f); break;
                case ELightType.Brake: Lighting(vehicleLightController.brakeLightOn ? _intensity : 0); break;
                case ELightType.Reverse: Lighting(vehicleLightController.reverseLightOn ? _intensity : 0f); break;
                case ELightType.LeftIndicator: UpdateIndicatorLight(true); break;
                case ELightType.RightIndicator: UpdateIndicatorLight(false); break;
                case ELightType.Fog: Lighting(vehicleLightController.fogLightOn ? _intensity : 0f); break;
            }
        }

        /// <summary>
        /// 灯光强度
        /// </summary>
        /// <param name="input"></param>
        public void Lighting(float input)
	    {
	        _light.intensity = Mathf.Lerp(_light.intensity, input, Time.deltaTime * _onOffSpeed * 20f);
	    }

        /// <summary>
        /// 闪烁时间
        /// </summary>
        public float indicatorTimer { get; private set; } = 0f;

		private void UpdateIndicatorLight(bool isLeftLight)
        {
            switch (vehicleLightController.indicatorLight)
            {
                case EIndicatorLight.Left:
                    {
                        if (isLeftLight)
                        {
                            Flash();
                        }
                        else
                        {
                            StopFlash();
                        }
                        break;
                    }
                case EIndicatorLight.Right:
                    {
                        if (!isLeftLight)
                        {
                            Flash();
                        }
                        else
                        {
                            StopFlash();
                        }
                        break;
                    }
                case EIndicatorLight.All:
                    {
                        Flash();
                        break;
                    }
                case EIndicatorLight.Off:
                    {
                        StopFlash();
                        break;
                    }
            }
        }

        private void Flash()
        {
            indicatorTimer += Time.deltaTime;
            if (indicatorTimer >= 1f) indicatorTimer = 0f;
            Lighting(indicatorTimer >= .5f ? 0 : 1);
        }

        private void StopFlash()
        {
            indicatorTimer = 0;
            Lighting(0);
        }

        /// <summary>
        /// 控制光晕亮起过程
        /// </summary>
        private void LensFlare()
        {
			if (!lensFlare) return;
            var cam = CameraHelperExtension.currentCamera;
            if (!cam) return;

            if (refreshTimer > (1f / refreshRate))
	        {
	            refreshTimer = 0f;

				var camPos = cam.transform.position;
				float angle = Vector3.Angle(transform.forward, camPos - transform.position);

				if (angle != 0)
				{
					var distanceTocam = 4f / Vector3.Distance(transform.position, camPos);
					finalFlareBrightness = flareBrightness * distanceTocam * (100f - angle) / 300f;
				}
	
	            lensFlare.brightness = finalFlareBrightness * _light.intensity;
	            lensFlare.color = _light.color;
	        }
	        refreshTimer += Time.deltaTime;
	    }
	
	    private void Trail()
	    {
            if (trail)
            {
                trail.emitting = _light.intensity > .1f ? true : false;
                trail.startColor = _light.color;
            }
	    }
		
	    private void CheckLensFlare()
	    {
			var lensFlare = transform.GetComponent<LensFlare>();

			if (!lensFlare)
	        {
				lensFlare = gameObject.XAddComponent<LensFlare>();
				lensFlare.brightness = 0f;
	        }
	
	        if (lensFlare.flare == null) lensFlare.flare = _flare;
	
	        if(_light) _light.flare = null;
	    }
	}
}
