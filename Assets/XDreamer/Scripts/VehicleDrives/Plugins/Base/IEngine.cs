using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 引擎接口
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// 运转
        /// </summary>
        bool running { get; set; }

        /// <summary>
        /// 转速（N转每分钟）
        /// </summary>
        float RPM { get; }

        /// <summary>
        /// 最小转速
        /// </summary>
        float minRPM { get; }

        /// <summary>
        /// 最大转速
        /// </summary>
        float maxRPM { get; }
    }
}
