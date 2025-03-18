using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 制动器接口
    /// </summary>
    public interface IBrake
    {
        /// <summary>
        /// 刹车扭矩
        /// </summary>
        float torque { get; }

        /// <summary>
        /// ABS启用
        /// </summary>
        bool ABSEnable { get; }

        /// <summary>
        /// 手刹启用
        /// </summary>
        bool handbrakeOn { get; set; }

        /// <summary>
        /// 刹车启用
        /// </summary>
        bool brakeEnable { get; set; }
    }
}
