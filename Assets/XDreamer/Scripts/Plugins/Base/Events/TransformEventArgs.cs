using System;
using UnityEngine;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// 变换事件
    /// </summary>
    public abstract class TransformEventArgs : XEventArgs
    {
        /// <summary>
        /// 期望控制的变换对象
        /// </summary>
        public Transform transform;

        /// <summary>
        /// 矢量速度
        /// </summary>
        public Vector3 velocity;

        /// <summary>
        /// 空间
        /// </summary>
        public Space space;
    }

    /// <summary>
    /// 移动事件
    /// </summary>
    public class MoveEventArgs : TransformEventArgs { }

    /// <summary>
    /// 旋转事件
    /// </summary>
    public class RotateEventArgs : TransformEventArgs { }

    /// <summary>
    /// 缩放事件
    /// </summary>
    public class ScaleEventArgs : TransformEventArgs { }
}
