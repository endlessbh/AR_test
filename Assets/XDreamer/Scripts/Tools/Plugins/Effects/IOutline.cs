using UnityEngine;

namespace XCSJ.PluginTools.Effects
{
    /// <summary>
    /// 轮廓线接口:可轮廓的接口
    /// </summary>
    public interface IOutline
    {
        /// <summary>
        /// 能否显示轮廓线
        /// </summary>
        bool canDisplay { get; set; }

        /// <summary>
        /// 是否显示轮廓线
        /// </summary>
        bool isDisplay { get; set; }

        /// <summary>
        /// 开始显示
        /// </summary>
        /// <param name="outlineData">轮廓线数据</param>
        void StartDisplay(IOutlineData outlineData);

        /// <summary>
        /// 停止显示
        /// </summary>
        void StopDisplay();
    }

    /// <summary>
    /// 轮廓器的接口
    /// </summary>
    public interface IOutlineData
    {
        /// <summary>
        /// 颜色
        /// </summary>
        Color color { get; }

        /// <summary>
        /// 宽度
        /// </summary>
        float width { get; }

        /// <summary>
        /// 无遮挡
        /// </summary>
        bool overlay { get; }
    }
}
