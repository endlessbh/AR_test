using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;

namespace XCSJ.Extension.CNScripts.Inputs
{
    /// <summary>
    /// 触摸脚本事件类型 
    /// </summary>
    [Name("触摸脚本事件类型")]
    public enum ETouchScriptEventType
    {
        /// <summary>
        /// 左滑时执行
        /// </summary>
        [Name("左滑时执行")]
        SwipeLeft = 0,

        [Name("右滑时执行")]
        SwipeRight,

        [Name("上滑时执行")]
        SwipeUp,

        [Name("下滑时执行")]
        SwipeDown,

        [Name("水平拖动时执行")]
        DragHorizon,

        [Name("垂直拖动时执行")]
        DragVertical,

        [Name("两指延伸时执行")]
        Spread,

        [Name("两指收缩时执行")]
        Pinch,

        [Name("启动时执行")]
        Start,

        [Name("双指水平拖动时执行")]
        DoubleDragHorizon,

        [Name("双指垂直拖动时执行")]
        DoubleDragVertical,
    }

    /// <summary>
    /// 触摸脚本事件函数
    /// </summary>
    [Name("触摸脚本事件函数")]
    [Serializable]
    public class TouchScriptEventFunction : EnumFunction<ETouchScriptEventType> { }

    /// <summary>
    /// 触摸脚本事件函数集合
    /// </summary>
    [Name("触摸脚本事件函数集合")]
    [Serializable]
    public class TouchScriptEventFunctionCollection : EnumFunctionCollection<ETouchScriptEventType, TouchScriptEventFunction> { }

    /// <summary>
    /// 触摸脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.InputMenu + Title)]
    public class TouchScriptEvent : BaseScriptEvent<ETouchScriptEventType, TouchScriptEventFunction, TouchScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "触摸脚本事件";

        /// <summary>
        /// 单指移动触发阈值
        /// </summary>
        [Name("单指移动触发阈值")]
        [Range(0.01f, 960f)]
        public float oneFingerMoveTriggerThreshold = 10;

        /// <summary>
        /// 双指移动触发阈值
        /// </summary>
        [Name("双指移动触发阈值")]
        [Range(0.01f, 960f)]
        public float twoFingerMoveTriggerThreshold = 2;

        float x_dis = 0;

        float y_dis = 0;

        float m_currentTouchDistance = 0;

        float m_lastTouchDistance = 0;

        /// <summary>
        /// 启动
        /// </summary>
        protected override void Start()
        {
            base.Start();
            ExecuteScriptEvent(ETouchScriptEventType.Start);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            switch (Input.touchCount)
            {
                case 1://判断触摸数量为单点触摸  
                    {
                        if (Input.GetTouch(0).phase == TouchPhase.Moved)
                        {
                            x_dis += Input.GetAxis("Mouse X");
                            y_dis += Input.GetAxis("Mouse Y");
                            float xDelta = Input.GetAxis("Mouse X");
                            float yDelta = Input.GetAxis("Mouse Y");
                            OnDragHorizon(xDelta);
                            OnDragVertical(yDelta);
                        }
                        else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                        {
                            if (Mathf.Abs(x_dis) > Mathf.Abs(y_dis))
                            {
                                if (x_dis > oneFingerMoveTriggerThreshold)
                                    OnSwipeRight(x_dis);
                                else if (x_dis < -oneFingerMoveTriggerThreshold)
                                    OnSwipeLeft(x_dis);
                            }
                            else
                            {
                                if (y_dis > oneFingerMoveTriggerThreshold)
                                    OnSwipeUp(y_dis);
                                else if (y_dis < -oneFingerMoveTriggerThreshold)
                                    OnSwipeDown(y_dis);
                            }
                            x_dis = 0;
                            y_dis = 0;
                        }
                        break;
                    }
                case 2:
                    {
                        Touch touch1 = Input.GetTouch(0);
                        Touch touch2 = Input.GetTouch(1);
                        //前两只手指触摸类型都为移动触摸   
                        if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                        {
                            m_currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                            m_lastTouchDistance = Vector2.Distance((touch1.position - touch1.deltaPosition), (touch2.position - touch2.deltaPosition));
                            float touchDeltaDistance = m_currentTouchDistance - m_lastTouchDistance;
                            if (touchDeltaDistance > twoFingerMoveTriggerThreshold)
                            {
                                OnSpread(touchDeltaDistance);
                            }
                            else if (touchDeltaDistance < -twoFingerMoveTriggerThreshold)
                            {
                                OnPinch(touchDeltaDistance);
                            }
                            else
                            {
                                float xDelta = Input.GetAxis("Mouse X");
                                float yDelta = Input.GetAxis("Mouse Y");
                                OnDoubleDragHorizon(xDelta);
                                OnDoubleDragVertical(yDelta);
                            }
                        }
                        break;
                    }
            }//end switch 触摸点数目
        }

        /// <summary>
        /// 当右滑
        /// </summary>
        /// <param name="distance"></param>
        public void OnSwipeRight(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.SwipeRight, distance.ToString());
        }

        /// <summary>
        /// 当左滑
        /// </summary>
        /// <param name="distance"></param>
        public void OnSwipeLeft(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.SwipeLeft, distance.ToString());
        }

        /// <summary>
        /// 当上滑
        /// </summary>
        /// <param name="distance"></param>
        public void OnSwipeUp(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.SwipeUp, distance.ToString());
        }

        /// <summary>
        /// 当下滑
        /// </summary>
        /// <param name="distance"></param>
        public void OnSwipeDown(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.SwipeDown, distance.ToString());
        }

        /// <summary>
        /// 当水平拖拽
        /// </summary>
        /// <param name="distance"></param>
        public void OnDragHorizon(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.DragHorizon, distance.ToString());
        }

        /// <summary>
        /// 当垂直拖拽
        /// </summary>
        /// <param name="distance"></param>
        public void OnDragVertical(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.DragVertical, distance.ToString());
        }

        /// <summary>
        /// 当展开
        /// </summary>
        /// <param name="distance"></param>
        public void OnSpread(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.Spread, distance.ToString());
        }

        /// <summary>
        /// 当捏住
        /// </summary>
        /// <param name="distance"></param>
        public void OnPinch(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.Pinch, distance.ToString());
        }

        /// <summary>
        /// 当双指水平拖拽
        /// </summary>
        /// <param name="distance"></param>
        public void OnDoubleDragHorizon(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.DoubleDragHorizon, distance.ToString());
        }

        /// <summary>
        /// 当双指垂直拖拽
        /// </summary>
        /// <param name="distance"></param>
        public void OnDoubleDragVertical(float distance)
        {
            ExecuteScriptEvent(ETouchScriptEventType.DoubleDragVertical, distance.ToString());
        }
    }
}
