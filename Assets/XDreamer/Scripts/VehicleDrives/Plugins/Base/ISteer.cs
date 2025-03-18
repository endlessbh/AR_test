using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 转向器
    /// </summary>
    public interface ISteer
    {
        /// <summary>
        /// 车辆转角
        /// </summary>
        float steerAngle { get; }

        /// <summary>
        /// 方向盘转角
        /// </summary>
        float steerWheelAngle { get; }
    }
}
