using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 事件基类
    /// </summary>
    public abstract class XGUIEventArgs : EventArgs
    {

    }

    /// <summary>
    /// 窗口事件
    /// </summary>
    public class WindowEventArgs : XGUIEventArgs
    {
        /// <summary>
        /// 全屏
        /// </summary>
        public bool fullScreen { get; }

        /// <summary>
        /// 扩展
        /// </summary>
        public bool expand { get; }

        /// <summary>
        /// 可拖拽
        /// </summary>
        public bool canDrag { get; }

        /// <summary>
        /// 窗口最大化
        /// </summary>
        public bool maxSize { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fullScreen"></param>
        /// <param name="expand"></param>
        /// <param name="canDrag"></param>
        public WindowEventArgs(bool fullScreen, bool expand, bool canDrag, bool maxSize)
        {
            this.fullScreen = fullScreen;
            this.expand = expand;
            this.canDrag = canDrag;
            this.maxSize = maxSize;
        }
    }
}
