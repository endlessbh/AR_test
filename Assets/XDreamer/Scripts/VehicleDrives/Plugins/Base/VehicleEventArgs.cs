using System;
using System.Collections.Generic;
using System.Text;
using XCSJ.PluginVehicleDrive.Controllers;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 车辆事件
    /// </summary>
    public abstract class VehicleEventArgs : EventArgs
    {

    }

    /// <summary>
    /// 车辆状态
    /// </summary>
    public class VehicleStateEventArgs : VehicleEventArgs
    {
        /// <summary>
        /// 发动机运行
        /// </summary>
        public EVehicleState vehicleState { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="running"></param>
        public VehicleStateEventArgs(EVehicleState vehicleState)
        {
            this.vehicleState = vehicleState;
        }
    }

    /// <summary>
    /// 发动机事件
    /// </summary>
    public class EngineEventArgs : VehicleEventArgs
    {
        /// <summary>
        /// 发动机运行
        /// </summary>
        public bool running { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="running"></param>
        public EngineEventArgs(bool running)
        {
            this.running = running;
        }
    }

    /// <summary>
    /// 刹车事件
    /// </summary>
    public class BrakeEventArgs : VehicleEventArgs
    {
        /// <summary>
        /// 手刹
        /// </summary>
        public bool handBrakeOn { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="handBrakeOn"></param>
        public BrakeEventArgs(bool handBrakeOn)
        {
            this.handBrakeOn = handBrakeOn;
        }
    }

    /// <summary>
    /// 变速箱事件
    /// </summary>
    public class GearBoxEventArgs : VehicleEventArgs
    {
        /// <summary>
        /// 空挡
        /// </summary>
        public bool NGear { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="NGear"></param>
        public GearBoxEventArgs(bool NGear)
        {
            this.NGear = NGear;
        }
    }

    /// <summary>
    /// 车灯事件
    /// </summary>
    public class LightEventArgs : VehicleEventArgs
    {
        /// <summary>
        /// 灯光类型
        /// </summary>
        public ELightType lightType { get; }

        /// <summary>
        /// 开关
        /// </summary>
        public bool isOn { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lightType"></param>
        /// <param name="isOn"></param>
        public LightEventArgs(ELightType lightType, bool isOn)
        {
            this.lightType = lightType;

            this.isOn = isOn;
        }
    }
}
