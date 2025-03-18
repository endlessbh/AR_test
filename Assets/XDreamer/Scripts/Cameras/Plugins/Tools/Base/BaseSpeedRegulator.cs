using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础速度调节器
    /// </summary>
    [Name("基础速度调节器")]
    public abstract class BaseSpeedRegulator : BaseCameraToolController
    {
        /// <summary>
        /// 速度控制器列表
        /// </summary>
        [Name("速度控制器列表")]
        [ComponentPopup(typeof(BaseSpeedController), searchFlags = ESearchFlags.Default)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<BaseSpeedController> _speedControllers = new List<BaseSpeedController>();

        /// <summary>
        /// 调节模式
        /// </summary>
        [Name("调节模式")]
        public enum ERegulateMode
        {
            /// <summary>
            /// 加法
            /// </summary>
            [Name("加法")]
            Add,

            /// <summary>
            /// 乘法
            /// </summary>
            [Name("乘法")]
            Mul,
        }

        /// <summary>
        /// 调节模式
        /// </summary>
        [Name("调节模式")]
        [EnumPopup]
        public ERegulateMode _regulateMode = ERegulateMode.Mul;

        /// <summary>
        /// 增加值
        /// </summary>
        [Name("增加值")]
        public Vector3 _increaseValue = new Vector3(1.01f, 1.01f, 1.01f);

        /// <summary>
        /// 减少值
        /// </summary>
        [Name("减少值")]
        public Vector3 _decreaseValue = new Vector3(0.99f, 0.99f, 0.99f);

        /// <summary>
        /// 获取速度
        /// </summary>
        /// <param name="speed"></param>
        /// <param name="increaseOrDecrease"></param>
        /// <returns></returns>
        public Vector3 GetSpeed(Vector3 speed, bool increaseOrDecrease)
        {
            switch (_regulateMode)
            {
                case ERegulateMode.Add: return speed + (increaseOrDecrease ? _increaseValue : _decreaseValue);
                case ERegulateMode.Mul: return Vector3.Scale(speed, increaseOrDecrease ? _increaseValue : _decreaseValue);
                default: return speed;
            }
        }

        /// <summary>
        /// 更新速度
        /// </summary>
        /// <param name="speedController"></param>
        /// <param name="increaseOrDecrease"></param>
        public void UpdateSpeed(BaseSpeedController speedController, bool increaseOrDecrease)
        {
            if (!speedController) return;
            speedController.speedInfo.value = GetSpeed(speedController.speedInfo.value, increaseOrDecrease);
        }

        /// <summary>
        /// 更新速度
        /// </summary>
        /// <param name="increaseOrDecrease"></param>
        public void UpdateSpeed(bool increaseOrDecrease)
        {
            foreach (var speedController in _speedControllers)
            {
                UpdateSpeed(speedController, increaseOrDecrease);
            }
        }
    }
}
