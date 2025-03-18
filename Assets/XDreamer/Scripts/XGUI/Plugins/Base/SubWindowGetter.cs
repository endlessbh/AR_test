using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 子窗口获取器
    /// </summary>
    [Name("子窗口获取器")]
    public abstract class SubWindowGetter : View
    {
        /// <summary>
        /// 子窗口
        /// </summary>
        [Name("子窗口")]
        public SubWindow _subWindow;

        /// <summary>
        /// 子窗口
        /// </summary>
        public SubWindow subWindow => this.XGetComponentInParentOrGlobal(ref _subWindow);

        /// <summary>
        /// 重置
        /// </summary>
        protected virtual void Reset()
        {
            if (subWindow) { }
        }

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            if (subWindow) { }
        }
    }

    /// <summary>
    /// 子窗口事件监听
    /// </summary>
    public abstract class SubWindowEventListener : SubWindowGetter
    {
        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            SubWindow.onSubWindowEventCallback += OnSubWindowEventCallback;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            SubWindow.onSubWindowEventCallback -= OnSubWindowEventCallback;
        }

        /// <summary>
        /// 子窗口事件回调函数
        /// </summary>
        /// <param name="subWindow"></param>
        /// <param name="windowEventArgs"></param>
        protected virtual void OnSubWindowEventCallback(SubWindow subWindow, WindowEventArgs windowEventArgs)
        {
            if (this.subWindow == subWindow)
            {
                OnSubWindowEvent(windowEventArgs);
            }
        }

        /// <summary>
        /// 响应子窗口事件函数
        /// </summary>
        /// <param name="windowEventArgs"></param>
        protected abstract void OnSubWindowEvent(WindowEventArgs windowEventArgs);
    }
}
