using System;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Binders;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// 事件监听器
    /// </summary>
    public abstract class EventListener : FieldBinder
    {
        #region 回调事件

        /// <summary>
        /// 当事件被调用时回调；携带事件回调时的完整参数信息；
        /// </summary>
        public event Action<EventListener, ITupleData> onEventInvoked;

        /// <summary>
        /// 当事件被调用时回调；不携带任何参数，仅用于触发事件；
        /// </summary>
        public event Action onEventInvokedEmpty;

        /// <summary>
        /// 事件被调用时回调
        /// </summary>
        /// <param name="tuple">事件回调时的所有参数转化的元组对象</param>
        protected void OnEventInvoked(ITupleData tuple)
        {
            onEventInvoked?.Invoke(this, tuple);
            onEventInvokedEmpty?.Invoke();
        }

        /// <summary>
        /// 事件被调用时回调
        /// </summary>
        protected void OnEventInvoked() => OnEventInvoked(EmptyTupleData.Default);

        #endregion

        #region 添加监听

        /// <summary>
        /// 内部添加监听
        /// </summary>
        protected abstract void AddListenerInternal();

        /// <summary>
        /// 添加监听
        /// </summary>
        public void AddListener() => AddListenerInternal();

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="action"></param>
        public void AddListener(Action action)
        {
            if (action == null) return;
            onEventInvokedEmpty += action;
            AddListener();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="action"></param>
        public void AddListener(Action<EventListener, ITupleData> action)
        {
            if (action == null) return;
            onEventInvoked += action;
            AddListener();
        }

        #endregion

        #region 移除监听

        protected abstract void RemoveListenerInternal();

        /// <summary>
        /// 移除监听
        /// </summary>
        public void RemoveListener() => RemoveListenerInternal();

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="action"></param>
        public void RemoveListener(Action action)
        {
            if (action == null) return;
            onEventInvokedEmpty -= action;
            RemoveListener();
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="action"></param>
        public void RemoveListener(Action<EventListener, ITupleData> action)
        {
            if (action == null) return;
            onEventInvoked -= action;
            RemoveListener();
        }

        #endregion
    }
}
