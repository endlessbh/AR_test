using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 燃料接口
    /// </summary>
    public interface IFuel
    {
        /// <summary>
        /// 当前量
        /// </summary>
        float amount { get; }

        /// <summary>
        /// 容量
        /// </summary>
        float capacity { get; }

        /// <summary>
        /// 百分比
        /// </summary>
        float percent { get; }

        /// <summary>
        /// 加油
        /// </summary>
        /// <param name="amount"></param>
        void Add(float amount);

        /// <summary>
        /// 获取燃油动力
        /// </summary>
        /// <returns></returns>
        float GetPower();
    }
}
