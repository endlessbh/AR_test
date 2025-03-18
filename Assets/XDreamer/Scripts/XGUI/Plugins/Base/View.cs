using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.PluginXGUI;

namespace XCSJ.PluginXGUI.Base
{
    /// <summary>
    /// 视图接口
    /// </summary>
    public interface IView 
    {
        /// <summary>
        /// 矩形变换
        /// </summary>
        RectTransform rectTransform { get; }

        /// <summary>
        /// 父级画布
        /// </summary>
        Canvas parentCanvas { get; }
    }

    /// <summary>
    /// 视图：XGUI基类
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public abstract class View : InteractProvider, IView
    {
        /// <summary>
        /// 视图变换
        /// </summary>
        public RectTransform rectTransform => this.XGetComponentInChildren(ref _rectTransform);
        private RectTransform _rectTransform;

        /// <summary>
        /// 父级画布
        /// </summary>
        public Canvas parentCanvas => this.XGetComponentInParent(ref _parentCanvas);
        private Canvas _parentCanvas;
    }

    /// <summary>
    /// 可拖拽视图接口
    /// </summary>
    public interface IDraggableView : IView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// 可拖拽
        /// </summary>
        bool canDrag { get; set; }
    }

    /// <summary>
    /// 可拖拽视图
    /// </summary>
    public abstract class DraggableView : View, IDraggableView, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 同级索引规则
        /// </summary>
        [Group("基础设置", textEN = "Base Settings")]
        [Name("同级索引规则")]
        [EnumPopup]
        public ESiblingIndexRule _siblingIndexRule = ESiblingIndexRule.Last_CurrentInclude_RootCanvasNotInclude;

        /// <summary>
        /// 可拖拽
        /// </summary>
        public virtual bool canDrag { get; set; } = false;

        /// <summary>
        /// 指针按下
        /// </summary>
        private bool isPointerDown = false;

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            CommonFun.BeginOnUI();

            if (canDrag)
            {
                isPointerDown = true;// 保证了指针是在当前对象上按下
            }
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (canDrag && isPointerDown)
            {
                transform.position += (Vector3)eventData.delta;
            }
        }

        /// <summary>
        /// 弹起
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            CommonFun.EndOnUI();

            isPointerDown = false;
        }

        /// <summary>
        /// 指针按下:设置窗口为同层级最前端，开始拖拽只有移动时才会触发，因此在这里进行设置才正确
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData) => transform.SetSiblingIndex(_siblingIndexRule);

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData) { }
    }

    /// <summary>
    /// 视图交互器
    /// </summary>
    [RequireManager(typeof(XGUIManager))]
    [Owner(typeof(XGUIManager))]
    public abstract class ViewInteractor : Interactor, IView
    {
        /// <summary>
        /// 视图变换
        /// </summary>
        public RectTransform rectTransform => this.XGetComponentInChildrenOrGlobal(ref _rectTransform);
        private RectTransform _rectTransform;

        /// <summary>
        /// 父级画布
        /// </summary>
        public Canvas parentCanvas => this.XGetComponentInParentOrGlobal(ref _parentCanvas);
        private Canvas _parentCanvas;
    }

    /// <summary>
    /// 可拖拽视图交互器
    /// </summary>
    public abstract class DraggableViewInteractor : ViewInteractor, IDraggableView, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// 同级索引规则
        /// </summary>
        [Name("同级索引规则")]
        [EnumPopup]
        public ESiblingIndexRule _siblingIndexRule = ESiblingIndexRule.Last_CurrentInclude_RootCanvasNotInclude;

        /// <summary>
        /// 可拖拽
        /// </summary>
        public virtual bool canDrag { get; set; } = false;

        /// <summary>
        /// 指针按下
        /// </summary>
        private bool isPointerDown = false;

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            CommonFun.BeginOnUI();

            if (canDrag)
            {
                isPointerDown = true;// 保证了指针是在当前对象上按下
            }
        }

        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (canDrag && isPointerDown)
            {
                transform.position += (Vector3)eventData.delta;
            }
        }

        /// <summary>
        /// 弹起
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            CommonFun.EndOnUI();

            isPointerDown = false;
        }

        /// <summary>
        /// 指针按下
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData) => transform.SetSiblingIndex(_siblingIndexRule);

        /// <summary>
        /// 指针弹起
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData) { }
    }
}
