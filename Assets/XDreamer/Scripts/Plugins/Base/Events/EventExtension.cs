using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Interfaces;

namespace XCSJ.Extension.Base.Events
{
    public static class EventExtension
    {
        /// <summary>
        /// 重新查找事件处理者
        /// </summary>
        /// <param name="component"></param>
        /// <param name="list"></param>
        public static void Refind(this Component component, List<IEventHandler> list)
        {
            list?.Clear();
            Find(component, list);
        }

        /// <summary>
        /// 查找事件处理者
        /// </summary>
        /// <param name="component"></param>
        /// <param name="list"></param>
        public static void Find(this Component component, List<IEventHandler> list)
        {
            if (!component || list == null) return;
            component.GetComponents(list);
        }
    }
}
