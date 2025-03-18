using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCSJ.PluginVehicleDrive.Base
{
    /// <summary>
    /// 变速箱接口
    /// </summary>
    public interface IGearBox
    {
        /// <summary>
        /// 自动挡
        /// </summary>
        bool auto { get; }

        /// <summary>
        /// 当前速率
        /// </summary>
        float currentRadio { get; }

        /// <summary>
        /// 空挡
        /// </summary>
        bool NGear { get; set; }

        /// <summary>
        /// 换挡中
        /// </summary>
        bool changingGear { get; }

        /// <summary>
        /// 当前齿轮索引
        /// </summary>
        int currentGearIndex { get; }

        /// <summary>
        /// 总齿轮数
        /// </summary>
        int totalGearCount { get; }

        /// <summary>
        /// 离合输入
        /// </summary>
        float clutchInput { get; set; }

        /// <summary>
        /// 换挡
        /// </summary>
        void ChangeGear();

        /// <summary>
        /// 升档
        /// </summary>
        void ShiftUp();

        /// <summary>
        /// 降档
        /// </summary>
        void ShiftDown();

        /// <summary>
        /// 切换至指定档位
        /// </summary>
        /// <param name="index">档位</param>
        void ShiftTo(int index);
    }
}
