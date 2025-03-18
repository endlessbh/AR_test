using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础速度控制器
    /// </summary>
    [Name("基础速度控制器")]
    public abstract class BaseSpeedController : BaseCameraToolController, ICameraSpeed
    {
        #region 速度

        /// <summary>
        /// 速度信息
        /// </summary>
        [Name("速度信息")]
        public UpdateVector3RuntimePlatformInfo _speedInfo = new UpdateVector3RuntimePlatformInfo();

        /// <summary>
        /// 速度信息
        /// </summary>
        public UpdateVector3RuntimePlatformInfo speedInfo => _speedInfo;

        /// <summary>
        /// 本帧的实时速度；已经过时间缩放的速度值；
        /// </summary>
        public Vector3 speedRealtime => _speedInfo.valueRealtime;

        /// <summary>
        /// 速度系数
        /// </summary>
        public virtual Vector3 speedCoefficient => Vector3.one;

        /// <summary>
        /// 基础值
        /// </summary>
        Vector3 ICameraSpeed.speed => _speedInfo.valueRealtime;

        #endregion

        /// <summary>
        /// 唤醒
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            _speedInfo.Init();
        }

        /// <summary>
        /// 更新：使用间隔时间与基础速度更新本帧的实时速度信息
        /// </summary>
        protected virtual void Update()
        {
            //更新速度信息
            _speedInfo.Update(Time.deltaTime, speedCoefficient);
        }
    }
}
