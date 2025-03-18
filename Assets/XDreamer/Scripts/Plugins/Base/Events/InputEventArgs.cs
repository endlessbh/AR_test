using System;
using UnityEngine;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// 对应Unity中InputManager定义的事件
    /// </summary>
    public class InputEventArgs : XEventArgs
    {
        /// <summary>
        /// 期望控制的变换对象
        /// </summary>
        public Transform transform;

        /// <summary>
        /// 输入的名称
        /// </summary>
        public string name;

        /// <summary>
        /// 输入的值
        /// </summary>
        public float value;
    }
}
