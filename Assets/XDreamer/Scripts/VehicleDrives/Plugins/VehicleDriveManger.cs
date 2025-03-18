using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginVehicleDrive.Base;
using XCSJ.PluginVehicleDrive.Controllers;
using XCSJ.PluginVehicleDrive.DriveAssists;
using XCSJ.Scripts;

namespace XCSJ.PluginVehicleDrive
{
    /// <summary>
    /// 车辆驾驶：模拟摩托车、三轮车、汽车、卡车或更多轮的车辆的驾驶控制
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Professional)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(VehicleDriveHelper.CategoryName)]
    [Tip("模拟摩托车、三轮车、汽车、卡车或更多轮的车辆的驾驶控制", "Simulate the driving control of motorcycles, tricycles, cars, trucks or vehicles with more wheels")]
    [Guid("33F35F83-190B-409F-B7FF-C0227CD3E2F9")]
    [Version("23.428")]
    public class VehicleDriveManger : BaseManager<VehicleDriveManger>
    {
        /// <summary>
        /// 获取脚本列表
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts()
        {
            return Script.GetScriptsOfEnum<EVehicleDriveScriptID>(this);
        }

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            switch ((EVehicleDriveScriptID)id)
            {
                case EVehicleDriveScriptID.EngineControl:
                    {
                        var vehicle = param[1] as VehicleDriver;
                        if (vehicle)
                        {
                            switch (param[2] as string)
                            {
                                case "启动": vehicle.StartEngine(); return ReturnValue.Yes;
                                case "停止": vehicle.StopEngine(); return ReturnValue.Yes;
                                case "切换": vehicle.StartOrStopEngine(); return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EVehicleDriveScriptID.EnginePowerControl:
                    {
                        var vehicle = param[1] as VehicleDriver;
                        if (vehicle)
                        {
                            var value = (float)param[3];
                            switch (param[2] as string)
                            {
                                case "动力": vehicle.throttleInput = value; return ReturnValue.Yes;
                                case "刹车": vehicle.throttleInput = -value; return ReturnValue.Yes;
                                case "转向": vehicle.steerInput = value; return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EVehicleDriveScriptID.HandbrakeControl:
                    {
                        var vehicle = param[1] as VehicleDriver;
                        if (vehicle)
                        {
                            switch (param[2] as string)
                            {
                                case "拉手刹": vehicle.brake.handbrakeOn = true; return ReturnValue.Yes;
                                case "松手刹": vehicle.brake.handbrakeOn = false; return ReturnValue.Yes;
                                case "切换": vehicle.brake.handbrakeOn = !vehicle.brake.handbrakeOn; return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EVehicleDriveScriptID.GearControl:
                    {
                        var vehicle = param[1] as VehicleDriver;
                        if (vehicle)
                        {
                            switch (param[2] as string)
                            {
                                case "空挡": vehicle.gearBox.NGear = true; return ReturnValue.Yes;
                                case "非空挡": vehicle.gearBox.NGear = false; return ReturnValue.Yes;
                                case "升档": vehicle.gearBox.ShiftUp(); return ReturnValue.Yes;
                                case "降档": vehicle.gearBox.ShiftDown(); return ReturnValue.Yes;
                            }
                        }
                        break;
                    }
                case EVehicleDriveScriptID.NOSControl:
                    {
                        var nos = param[1] as NOS;
                        if (nos)
                        {
                            nos.boost = (param[2] as string) == "启动" ? true : false;
                            return ReturnValue.Yes;
                        }
                        break;
                    }
                case EVehicleDriveScriptID.VehicleLightControl:
                    {
                        var vlc = param[1] as VehicleLightController;
                        if (!vlc) break;

                        var operation = param[3] as string;
                        switch (param[2] as string)
                        {
                            case "近光":
                                {
                                    SetLightState(vlc, ELightType.LowBeamHead, operation);
                                    return ReturnValue.Yes;
                                }
                            case "远光":
                                {
                                    SetLightState(vlc, ELightType.HighBeamHead, operation);
                                    return ReturnValue.Yes;
                                }
                            case "雾灯":
                                {
                                    SetLightState(vlc, ELightType.Fog, operation);
                                    return ReturnValue.Yes;
                                }
                            case "左转":
                                {
                                    SetLightState(vlc, ELightType.LeftIndicator, operation);
                                    return ReturnValue.Yes;
                                }
                            case "右转":
                                {
                                    SetLightState(vlc, ELightType.RightIndicator, operation);
                                    return ReturnValue.Yes;
                                }
                            case "双闪":
                                {
                                    SetLightState(vlc, ELightType.AllIndicator, operation);
                                    return ReturnValue.Yes;
                                }
                        }
                        break;
                    }
                case EVehicleDriveScriptID.GetVehicleControlProperty:
                    {
                        var controller = param[1] as VehicleDriver;
                        if (!controller) break;
                        switch (param[2] as string)
                        {
                            case "方向":
                                {
                                    return ReturnValue.True(controller.direction.ToString());
                                }
                            case "速度":
                                {
                                    return ReturnValue.True(controller.speed.ToString());
                                }
                            case "转速":
                                {
                                    if (controller.engine!=null)
                                    {
                                        return ReturnValue.True(controller.engine.RPM);
                                    }
                                    break;
                                }
                            case "油量":
                                {
                                    if (controller.fuel != null)
                                    {
                                        return ReturnValue.True(controller.fuel.percent);
                                    }
                                    break;
                                }
                            case "车轮转角":
                                {
                                    if (controller.steer != null)
                                    {
                                        return ReturnValue.True(controller.steer.steerAngle);
                                    }
                                    break;
                                }
                            case "方向盘转角":
                                {
                                    if (controller.steer != null)
                                    {
                                        return ReturnValue.True(controller.steer.steerWheelAngle);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }

        private void SetLightState(VehicleLightController vehicleLightController, ELightType lightType, string state)
        {
            if (vehicleLightController == null) return;

            switch (state)
            {
                case "开": vehicleLightController.SetLightState(lightType, true); break;
                case "关": vehicleLightController.SetLightState(lightType, false); break;
                case "切换": vehicleLightController.SwitchLightState(lightType); break;
            }
        }
    }
}
