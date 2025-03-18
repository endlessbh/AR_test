using System;
using UnityEngine;
using XCSJ.Helper;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 对象弹出特性；用于修饰UnityEngine.Object类型的字段，可用于选择游戏对象、组件、组件模式中的组件集、组件集中组件（包括但不限于状态、跳转、状态组等）；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ObjectPopupAttribute : PropertyAttribute
    {
        /// <summary>
        ///  组件集弹出菜单宽度
        /// </summary>
        public float componentCollectionWidth = 80;

        /// <summary>
        /// 组件弹出菜单宽度
        /// </summary>
        public float componentWidth = 80;
    }

    /// <summary>
    /// 日期时间特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DateTimeAttribute : PropertyAttribute
    {
        /// <summary>
        /// 格式
        /// </summary>
        public string format = DateTimeHelper.DefaultFormat;

        /// <summary>
        /// 按钮宽度
        /// </summary>
        public float buttonWidth = 80;
    }

    /// <summary>
    /// 组件类型弹出特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ComponentTypePopupAttribute : PropertyAttribute
    {
        /// <summary>
        /// 按钮宽度
        /// </summary>
        public float buttonWidth = 80;
    }

    /// <summary>
    /// Guid生成器特性
    /// </summary>
    public class GuidCreaterAttribute : PropertyAttribute
    {
        /// <summary>
        /// 按钮宽度
        /// </summary>
        public float buttonWidth = 24;
    }
}
