using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 车轮粒子
    /// </summary>
    [Name("车轮粒子")]
    [XCSJ.Attributes.Icon(EIcon.Wheel)]
    [Tool(VehicleDriveHelper.CategoryComponentName, nameof(WheelDriver))]
    [RequireComponent(typeof(WheelDriver))]
    [RequireManager(typeof(VehicleDriveManger))]
    [DisallowMultipleComponent]
    public class WheelParticle : InteractProvider
    {    
	    // 粒子系统
	    private List<ParticleSystem> allWheelParticles = new List<ParticleSystem>();
	
	    private WheelDriver _wheelDriver = null;

		/// <summary>
		/// 唤醒
		/// </summary>
		protected void Awake()
	    {
	        _wheelDriver = GetComponent<WheelDriver>();
	    }

        /// <summary>
        /// 启动
        /// </summary>
        protected void Start()
        {
            if (_wheelDriver.groundMaterialConfig)
            {
                // 创建地面粒子系统
                var frictions = _wheelDriver.groundMaterialConfig._groundMaterialFrictions;
                foreach (var f in frictions)
                {
                    if (f.groundParticles)
                    {
                        var ps = Instantiate(f.groundParticles, transform.position, transform.rotation) as GameObject;
                        var emission = ps.GetComponent<ParticleSystem>().emission;
                        emission.enabled = false;
                        ps.transform.SetParent(transform, false);
                        ps.transform.localPosition = Vector3.zero;
                        ps.transform.localRotation = Quaternion.identity;
                        allWheelParticles.Add(ps.GetComponent<ParticleSystem>());
                    }
                }
            }
		}

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
	    {
	        if (_wheelDriver.currentFrictions == null) return;
	
	        bool emEnable = _wheelDriver.totalSlip > _wheelDriver.currentFrictions.slip;
	
	        // 如果车轮滑动值比地面材质的滑动值大，则启用粒子，否则禁用
	        for (int i = 0; i < allWheelParticles.Count; i++)
	        {
	            var em = allWheelParticles[i].emission;
	            em.enabled = (emEnable && i == _wheelDriver.groundIndex) ? true : false;
	        }
	    }
	}
}
