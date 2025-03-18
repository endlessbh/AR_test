using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.UGUI
{
    /// <summary>
    /// UGUI触发脚本事件类型 
    /// </summary>
    [Name("UGUI触发脚本事件类型")]
    public enum EUGUITriggerScriptEventType
    {
        [Name("指针按下时执行")]
        OnPointerDown,

        [Name("指针进入时执行")]
        OnPointerEnter,

        [Name("指针离开时执行")]
        OnPointerExit,

        [Name("指针抬起时执行")]
        OnPointerUp,

        [Name("指针点击时执行")]
        OnPointerClick,

        [Name("拖拽时执行")]
        OnDrag,

        [Name("指针点击时执行")]
        OnDrop,

        [Name("滚动时执行")]
        OnScroll,

        [Name("更新选择时执行")]
        OnUpdateSelected,

        [Name("选择时执行")]
        OnSelect,

        [Name("取消选择时执行")]
        OnDeselect,

        [Name("移动时执行")]
        OnMove,

        [Name("初始化潜在的拖拽时执行")]
        OnInitializePotentialDrag,

        [Name("开始拖拽时执行")]
        OnBeginDrag,

        [Name("结束拖拽时执行")]
        OnEndDrag,

        [Name("提交时执行")]
        OnSubmit,

        [Name("取消时执行")]
        OnCancel,

        [Name("启动时执行")]
        Start,
    }

    /// <summary>
    /// UGUI触发脚本事件函数
    /// </summary>
    [Serializable]
    [Name("UGUI触发脚本事件函数")]
    public class UGUITriggerScriptEventFunction : EnumFunction<EUGUITriggerScriptEventType> { }

    /// <summary>
    /// UGUI触发脚本事件函数集合
    /// </summary>
    [Name("UGUI触发脚本事件函数集合")]
    [Serializable]
    public class UGUITriggerScriptEventFunctionCollection : EnumFunctionCollection<EUGUITriggerScriptEventType, UGUITriggerScriptEventFunction> { }

    /// <summary>
    /// UGUI触发脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.UGUIMenu + Title)]
    [ExecuteInEditMode]
    public class UGUITriggerScriptEvent : BaseScriptEvent<EUGUITriggerScriptEventType, UGUITriggerScriptEventFunction, UGUITriggerScriptEventFunctionCollection>, 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler, 
        IBeginDragHandler,
        IInitializePotentialDragHandler, 
        IDragHandler,
        IEndDragHandler, 
        IDropHandler, 
        IScrollHandler, 
        IUpdateSelectedHandler, 
        ISelectHandler, 
        IDeselectHandler, 
        IMoveHandler, 
        ISubmitHandler, 
        ICancelHandler
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "UGUI触发脚本事件";

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            ExecuteScriptEvent(EUGUITriggerScriptEventType.Start);
        }

        /// <summary>
        /// 当开始拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            //Log.Debug("OnBeginDrag");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnBeginDrag, eventData.ToString());
        }

        /// <summary>
        /// 当取消
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnCancel(BaseEventData eventData)
        {
            //Log.Debug("OnCancel");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnCancel, eventData.ToString());
        }

        /// <summary>
        /// 当取消选择
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDeselect(BaseEventData eventData)
        {
            //Log.Debug("OnDeselect");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnDeselect, eventData.ToString());
        }

        /// <summary>
        /// 当拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            //Log.Debug("OnDrag");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnDrag, eventData.ToString());
        }

        /// <summary>
        /// 当下降
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrop(PointerEventData eventData)
        {
            //Log.Debug("OnDrop");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnDrop, eventData.ToString());
        }

        /// <summary>
        /// 当结束拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            //Log.Debug("OnEndDrag");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnEndDrag, eventData.ToString());
        }

        /// <summary>
        /// 当初始化指针拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            //Log.Debug("OnInitializePotentialDrag");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnInitializePotentialDrag, eventData.ToString());
        }

        /// <summary>
        /// 当移动
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnMove(AxisEventData eventData)
        {
            //Log.Debug("OnMove");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnMove, eventData.ToString());
        }

        /// <summary>
        /// 当指针点击
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            //Log.Debug("OnPointerClick");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnPointerClick, eventData.ToString());
        }

        /// <summary>
        /// 当指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            //Log.Debug("OnPointerDown");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnPointerDown, eventData.ToString());
        }

        /// <summary>
        /// 当指针进入
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            //Log.Debug("OnPointerEnter");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnPointerEnter, eventData.ToString());
        }

        /// <summary>
        /// 当指针退出
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            //Log.Debug("OnPointerExit");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnPointerExit, eventData.ToString());
        }

        /// <summary>
        /// 当指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            //Log.Debug("OnPointerUp");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnPointerUp, eventData.ToString());
        }

        /// <summary>
        /// 当滚动
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnScroll(PointerEventData eventData)
        {
            //Log.Debug("OnScroll");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnScroll, eventData.ToString());
        }

        /// <summary>
        /// 当选择
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSelect(BaseEventData eventData)
        {
            //Log.Debug("OnSelect");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnSelect, eventData.ToString());
        }

        /// <summary>
        /// 当提交
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnSubmit(BaseEventData eventData)
        {
            //Log.Debug("OnSubmit");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnSubmit, eventData.ToString());
        }

        /// <summary>
        /// 当更新已选择的
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            //Log.Debug("OnUpdateSelected");
            ExecuteScriptEvent(EUGUITriggerScriptEventType.OnUpdateSelected, eventData.ToString());
        }
    }
}
