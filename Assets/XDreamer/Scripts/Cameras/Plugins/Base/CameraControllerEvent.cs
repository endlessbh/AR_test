using System;
using System.Collections.Generic;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 相机控制器事件
    /// </summary>
    public class CameraControllerEvent
    {
        #region 事件接口管理

        private static HashSet<ICameraControllerEvent> cameraEvents = new HashSet<ICameraControllerEvent>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="cameraEvent"></param>
        public static void Register(ICameraControllerEvent cameraEvent)
        {
            if (cameraEvent == null || cameraEvents.Contains(cameraEvent)) return;
            cameraEvents.Add(cameraEvent);
            //Log.Debug("Register 0:" + cameraEvent);
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cameraEvent"></param>
        public static void Register<T>(T cameraEvent) where T : UnityEngine.Object, ICameraControllerEvent
        {
            if (!cameraEvent || cameraEvents.Contains(cameraEvent)) return;
            cameraEvents.Add(cameraEvent);
            //Log.Debug("Register 1:" + cameraEvent);
        }

        /// <summary>
        /// 取消注册事件
        /// </summary>
        /// <param name="cameraEvent"></param>
        public static void Unregister(ICameraControllerEvent cameraEvent)
        {
            if (cameraEvent == null) return;
            cameraEvents.Remove(cameraEvent);
        }

        /// <summary>
        /// 清空事件
        /// </summary>
        public static void Clear() => cameraEvents.Clear();

        #endregion

        #region 切换事件：开始切换、将要结束切换、将要切换为上一个、已切换为当前、结束切换

        #region 开始切换

        /// <summary>
        /// 开始切换相机控制器事件：当将要开始切换相机控制器之前回调的事件；即旧相机控制器(即当前相机控制器)将要开始切换到新相机控制器的位置与旋转（如果需要补间）之前回调；；由<see cref="CallOnBeginSwitch"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController, BaseCameraMainController> onBeginSwitch;

        /// <summary>
        /// 调用开始切换相机控制器事件：当将要开始切换相机控制器之前回调，即将由旧相机控制器切换到新相机控制器；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public static void CallOnBeginSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            onBeginSwitch?.Invoke(from, to);
            foreach (var e in cameraEvents) e?.OnBeginSwitch(from, to);
        }

        #endregion

        #region 将要结束切换

        /// <summary>
        /// 将要结束切换相机控制器事件：当将要结束切换之前回调的事件；即当前相机控制已经切换到目标相机控制器的位置与旋转（如果需要补间）之后回调；由<see cref="CallOnBeginSwitch"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController, BaseCameraMainController> onWillEndSwitch;

        /// <summary>
        /// 调用将要结束切换相机控制器事件：当将要结束切换之前回调的事件；即旧相机控制器(即当前相机控制器)已经切换到新相机控制器的位置与旋转（如果需要补间）之后回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        public static void CallOnWillEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            onWillEndSwitch?.Invoke(from, to);
            foreach (var e in cameraEvents) e?.OnWillEndSwitch(from, to);
        }

        #endregion

        #region 将要切换为上一个

        /// <summary>
        /// 将要切换为上一个相机控制器事件：当将要切换为上一个相机控制器之前回调的事件；将要切换为非当前相机前回调；由<see cref="CallOnWillSwitchToLast"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController> onWillSwitchToLast;

        /// <summary>
        /// 调用将要切换为上一个相机控制器事件：当将要切换为上一个相机控制器之前回调的事件；将要切换为非当前相机前回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        public static void CallOnWillSwitchToLast(BaseCameraMainController from)
        {
            onWillSwitchToLast?.Invoke(from);
            foreach (var e in cameraEvents) e?.OnWillSwitchToLast(from);
        }

        #endregion

        #region 已切换为当前

        /// <summary>
        /// 已切换为当前相机控制器事件：当已切换为当前相机控制器之后回调的事件；新相机控制器已经被设置为当前相机控制器之后的回调；由<see cref="CallOnSwitchedToCurrent"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController> onSwitchedToCurrent;

        /// <summary>
        /// 调用已切换为当前相机控制器事件：当已切换为当前相机控制器之后回调的事件；新相机控制器已经被设置为当前相机控制器之后的回调；
        /// </summary>
        /// <param name="from">新相机控制器(即当前相机控制器)</param>
        public static void CallOnSwitchedToCurrent(BaseCameraMainController to)
        {
            foreach (var e in cameraEvents) e?.OnSwitchedToCurrent(to);
            onSwitchedToCurrent?.Invoke(to);
        }

        #endregion

        #region 结束切换

        /// <summary>
        /// 结束切换相机控制器事件：当将要已经切换相机控制器之后回调的事件；由<see cref="CallOnEndSwitch"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController, BaseCameraMainController> onEndSwitch;

        /// <summary>
        /// 调用结束切换相机控制器事件：当将要已经切换相机控制器之后回调的事件；
        /// </summary>
        /// <param name="from">旧相机控制器</param>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        public static void CallOnEndSwitch(BaseCameraMainController from, BaseCameraMainController to)
        {
            foreach (var e in cameraEvents) e?.OnEndSwitch(from, to);
            onEndSwitch?.Invoke(from, to);
        }

        #endregion

        #endregion

        #region 相机主控制器组件启用与禁用事件

        /// <summary>
        /// 相机主控制器组件启用事件：当相机主控制器组件启用时回调的事件；由<see cref="CallOnEnabled"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController> onEnabled;

        /// <summary>
        /// 调用相机主控制器组件启用事件：当相机主控制器组件启用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        public static void CallOnEnabled(BaseCameraMainController cameraController)
        {
            onEnabled?.Invoke(cameraController);
            foreach (var e in cameraEvents) e?.OnEnabled(cameraController);
        }

        /// <summary>
        /// 相机主控制器组件禁用事件：当相机主控制器组件禁用时回调的事件；由<see cref="CallOnDisabled"/>函数调用；
        /// </summary>
        public static event Action<BaseCameraMainController> onDisabled;

        /// <summary>
        /// 调用相机主控制器组件禁用事件：当相机主控制器组件禁用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        public static void CallOnDisabled(BaseCameraMainController cameraController)
        {
            foreach (var e in cameraEvents) e?.OnDisabled(cameraController);
            onDisabled?.Invoke(cameraController);
        }

        #endregion
    }

    /// <summary>
    /// 相机控制器事件接口
    /// </summary>
    public interface ICameraControllerEvent
    {
        /// <summary>
        /// 当将要开始切换相机控制器之前回调，即将由旧相机控制器切换到新相机控制器；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        void OnBeginSwitch(BaseCameraMainController from, BaseCameraMainController to);

        /// <summary>
        /// 当将要结束切换之前回调的事件；即旧相机控制器(即当前相机控制器)已经切换到新相机控制器的位置与旋转（如果需要补间）之后回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        /// <param name="to">新相机控制器</param>
        void OnWillEndSwitch(BaseCameraMainController from, BaseCameraMainController to);

        /// <summary>
        /// 当将要切换为上一个相机控制器之前回调的事件；将要切换为非当前相机前回调；
        /// </summary>
        /// <param name="from">旧相机控制器(即当前相机控制器)</param>
        void OnWillSwitchToLast(BaseCameraMainController from);

        /// <summary>
        /// 当已切换为当前相机控制器之后回调的事件；新相机控制器已经被设置为当前相机控制器之后的回调；
        /// </summary>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        void OnSwitchedToCurrent(BaseCameraMainController to);

        /// <summary>
        /// 当将要已经切换相机控制器之后回调的事件；
        /// </summary>
        /// <param name="from">旧相机控制器</param>
        /// <param name="to">新相机控制器(即当前相机控制器)</param>
        void OnEndSwitch(BaseCameraMainController from, BaseCameraMainController to);

        /// <summary>
        /// 当相机主控制器组件启用时回调的事件；
        /// </summary>
        /// <param name="cameraController"></param>
        void OnEnabled(BaseCameraMainController cameraController);

        /// <summary>
        /// 当相机主控制器组件禁用时回调的事件
        /// </summary>
        /// <param name="cameraController"></param>
        void OnDisabled(BaseCameraMainController cameraController);
    }
}
