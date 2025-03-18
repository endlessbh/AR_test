namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机核心控制器：用于被相机主控制器直接引用核心功能的子控制器
    /// </summary>
    public abstract class BaseCameraCoreController : BaseCameraSubController
    {
        /// <summary>
        /// 当将要切换为上一个相机控制器之前回调：将要切换为非当前相机前回调；空函数；
        /// </summary>
        public virtual void OnWillSwitchToLast() { }

        /// <summary>
        /// 当已切换为当前相机控制器之后回调：当前组件中的相机控制器即为当前相机控制器；空函数；
        /// </summary>
        public virtual void OnSwitchedToCurrent() { }
    }
}
