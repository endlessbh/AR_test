using System;
using XCSJ.Attributes;
using XCSJ.PluginsCameras.Base;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础相机切换补间器
    /// </summary>
    [Name("基础相机切换补间器")]
    public abstract class BaseCameraSwitchTweener : BaseCameraToolController
    {
        /// <summary>
        /// 当将要开始强制切换时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="directSwitch">标识是直接切换还是补间切换</param>
        /// <returns>支持强制切换返回True，不支持强制切换返回False;</returns>
        public abstract bool OnWillBeginMustSwitch(BaseCameraMainController targetCameraController, bool directSwitch);

        /// <summary>
        /// 当开始切换补间时回调
        /// </summary>
        /// <param name="targetCameraController">期望切换到的目标相机控制器</param>
        /// <param name="duration">补间的持续时间</param>
        /// <param name="onCompleted">补间完成后的回调</param>
        public abstract void OnBeginSwitchTween(BaseCameraMainController targetCameraController, float duration, Action onCompleted);

        /// <summary>
        /// 当结束切换补间后回调
        /// </summary>
        /// <param name="newCurrentCameraController">新的当前相机控制器</param>
        public abstract void OnEndSwitchTween(BaseCameraMainController newCurrentCameraController);        
    }
}
