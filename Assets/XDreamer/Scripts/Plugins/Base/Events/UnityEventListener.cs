using System;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.XUnityEngine.XEvents;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// Unity事件监听器
    /// </summary>
    [Name("Unity事件监听器")]
    [Serializable]
    public class UnityEventListener : EventListener
    {
        /// <summary>
        /// Unity事件对象
        /// </summary>
        public UnityEventBase unityEvent => fieldInfo?.GetValue(target) as UnityEventBase;

        private bool hasAdd = false;

        /// <summary>
        /// 内部添加监听
        /// </summary>
        protected override void AddListenerInternal()
        {
            if (!hasAdd && this.unityEvent is UnityEventBase unityEvent)
            {
                unityEvent.AddCall(OnEventInvoked);
                hasAdd = true;
            }
        }

        /// <summary>
        /// 内部移除监听
        /// </summary>
        protected override void RemoveListenerInternal()
        {
            if (hasAdd && this.unityEvent is UnityEventBase unityEvent)
            {
                unityEvent.RemoveCall(OnEventInvoked);
                hasAdd = false;
            }
        }

        #region 针对UnityEvent的缓存机制

        /// <summary>
        /// 获取Unity事件字段名称列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string[] GetEventFieldNames(Type type, BindingFlags bindingFlags, bool includeBaseType) => FieldNameCache.GetEventFieldNames(type, bindingFlags,includeBaseType);

        internal class FieldNameCache : TIVCache<FieldNameCache, Type, BindingFlags, bool, FieldNameCacheValue>
        {
            public static string[] GetEventFieldNames(Type type, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return Empty<string>.Array;
                return GetCacheValue(type, bindingFlags, includeBaseType)?.eventFieldNames ?? Empty<string>.Array;
            }
        }

        internal class FieldNameCacheValue : TIVCacheValue<FieldNameCacheValue, Type, BindingFlags, bool>
        {
            public string[] eventFieldNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                var names = FieldInfosCache.Get(key1, key2, key3).Where(fi => typeof(UnityEventBase).IsAssignableFrom(fi.FieldType)).Cast(fi => fi.Name);

                //去重
                names = names.Distinct();

#if UNITY_EDITOR
                //排序
                names = names.OrderBy(n => n);
#endif
                //转数组
                eventFieldNames = names.ToArray();
                return true;
            }
        }

        #endregion
    }
}
