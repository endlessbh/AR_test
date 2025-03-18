using System;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Extension.Logs;
using XCSJ.Helper;
using XCSJ.Kernel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;

namespace XCSJ.Extension.Base.Kernel
{
    /// <summary>
    /// 插件处理器扩展
    /// </summary>
    public static class PluginsHandlerExtension
    {
        private static bool initialized = false;

        /// <summary>
        /// 初始化，编辑态与运行时均本函数均会被调用
        /// </summary>
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            PluginsHandler.pluginsHandler = DefaultPluginsHandler.instance;
            DataHandler.dataHandler = DefaultDataHandler.instance;
            PathHandler.pathHandler = DefaultPathHandler.instance;
            TweenHandler.tweenHandler = DefaultTweenHandler.instance;

            LogHandler.logHandler = DefaultLogManagerHandler.instance;

#if !UNITY_EDITOR
            UnityObjectHandler.unityObjectHandler = DefaultUnityObjectHandler.instance;
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
            AsyncActionHelper.asyncActionHandler = AsyncActionHandler.instance;
#endif
        }
    }

    /// <summary>
    /// 异步动作处理器
    /// </summary>
    public class AsyncActionHandler : InstanceClass<AsyncActionHandler>, IAsyncActionHandler
    {
        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="action"></param>
        public void AsyncExecute(Action action) => CommonFun.DelayCall(action);
    }
}
