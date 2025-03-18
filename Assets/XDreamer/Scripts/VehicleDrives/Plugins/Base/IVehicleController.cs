using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 车辆输入
    /// </summary>
    public interface IVehicleInput
    {
        /// <summary>
        /// 动力输入
        /// </summary>
        float throttleInput { get; set; }

        /// <summary>
        /// 转向输入
        /// </summary>
        float steerInput { get; set; }
    }

    /// <summary>
    /// 车辆控制接口
    /// </summary>
    public interface IVehicleController : IVehicleInput
    {
        /// <summary>
        /// 方向
        /// </summary>
        int direction { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        float speed { get; }

        /// <summary>
        /// 引擎
        /// </summary>
        IEngine engine { get; set; } 

        /// <summary>
        /// 刹车控制器
        /// </summary>
        IBrake brake { get; set; }

        /// <summary>
        /// 变速箱
        /// </summary>
        IGearBox gearBox { get; set; }

        /// <summary>
        /// 方向盘
        /// </summary>
        ISteer steer { get; set; }

        /// <summary>
        /// 燃料对象
        /// </summary>
        IFuel fuel { get; set; }

        /// <summary>
        /// 车辆灯光控制器
        /// </summary>
        IVehicleLightController lightController { get; set; }
    }
}
