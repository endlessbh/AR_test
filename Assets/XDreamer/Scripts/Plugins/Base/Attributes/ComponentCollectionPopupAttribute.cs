using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XCSJ.Extension.Base.Attributes
{
    /// <summary>
    /// 组件集弹出菜单特性；用于修饰UnityEngine.Object类型的字段，可用于选择游戏对象，组件模式中的组件集（包括但不限于状态、跳转、状态组等）；
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ComponentCollectionPopupAttribute : PropertyAttribute
    {
        /// <summary>
        /// 弹出菜单宽度
        /// </summary>
        public float width = 80;
    }
}
