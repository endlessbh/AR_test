using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginPhysicses.Tools.Grounds;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.DriveAssists
{
    /// <summary>
    /// 车轮划痕管理器 ：在运行态动态生成划痕，并实时调整划痕模型网格
    /// </summary>
    [Name("车轮划痕管理器")]
    [XCSJ.Attributes.Icon(EIcon.Wheel)]
    [Tool(VehicleDriveHelper.CategoryComponentName, rootType = typeof(VehicleDriveManger))]
    [RequireComponent(typeof(GroundMaterialConfig))]
    [RequireManager(typeof(VehicleDriveManger))]
    public class VehicleSkidmark : VehicleControllerGetter
	{
        /// <summary>
        /// 划痕产生速度值
        /// </summary>
        [Name("划痕产生速度值")]
        public float _velocityValue = 1; 

		/// <summary>
		/// 划痕列表
		/// </summary>
	    public Skidmark[] _skidmarks;

        private int _lastGroundIndex = 0;
	    private int[] _lastSkidMarkIndex;

        /// <summary>
        /// 启用
        /// </summary>
        protected void Start()
        {
            if (vehicleController && vehicleController.vehicleDriver)
            {
                _lastSkidMarkIndex = new int[vehicleController.vehicleDriver.wheelDrivers.Count];
                for (int i = 0; i < _lastSkidMarkIndex.Length; i++)
                {
                    _lastSkidMarkIndex[i] = -1;
                }
            }
        }

        /// <summary>
        /// 响应更新
        /// </summary>
        protected void FixedUpdate()
	    {
            var vd = vehicleController.vehicleDriver;
            if (!vehicleController || !vd) return;

            for (int i = 0; i < vd.wheelDrivers.Count; i++)
            {
                var wc = vd.wheelDrivers[i];
                if (wc.currentFrictions != null)
                {
                    // 滑动力大于0
                    var offsetSlip = wc.totalSlip - wc.currentFrictions.slip;
                    var v = vd.velocity;
                    if (offsetSlip > 0 && v.magnitude > _velocityValue)
                    {
                        _lastSkidMarkIndex[i] = AddSkidMark(wc.wheelHit.point + 2f * v * Time.deltaTime, wc.wheelHit.normal, offsetSlip, _lastSkidMarkIndex[i], wc.groundIndex);
                    }
                    else
                    {
                        _lastSkidMarkIndex[i] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// 由打滑的车轮调用的函数。收集所需的所有信息
        // 稍后创建网格。设置滑动标记b部分的强度设置alpha
        // 顶点颜色的。
        /// </summary>
        /// <param name="pos">痕迹位置</param>
        /// <param name="normal">碰撞接触点法向量</param>
        /// <param name="intensity">强度</param>
        /// <param name="lastSkidmark">最后划痕索引</param>
        /// <param name="groundIndex">地面索引</param>
        private int AddSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastSkidmark, int groundIndex)
	    {
	        if (_lastGroundIndex != groundIndex)
	        {
	            _lastGroundIndex = groundIndex;
	            return -1;
	        }

            return _skidmarks[groundIndex].CreateSkidMark(pos, normal, intensity, lastSkidmark);
        }
	}
}
