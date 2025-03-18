using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Events
{
    /// <summary>
    /// Action事件监听器
    /// </summary>
    [Name("Action事件监听器")]
    [Serializable]
    public class ActionEventListener : EventListener
    {
        /// <summary>
        /// 事件方法信息；事件字段信息对应执行方法（Invoke）的方法信息
        /// </summary>
        public MethodInfo eventMethodInfo => ActionMethodHelper.GetActionMethodInfo(fieldInfo?.FieldType);

        /// <summary>
        /// 委托事件对象
        /// </summary>
        public Delegate delegateEvent => fieldInfo?.GetValue(target) as Delegate;

        private Delegate invokedDelegate = null;

        /// <summary>
        /// 检查参数:标识是否对Action事件回调的参数做检查;为True时，会启用参数检测列表的检测机制；为False时，只要事件回调就会执行后续的触发；
        /// </summary>
        [Name("检查参数")]
        [Tip("标识是否对Action事件回调的参数做检查;为True时，会启用参数检测列表的检测机制；为False时，只要事件回调就会执行后续的触发；", "Identify whether to check the parameters of the action event callback; When true, the detection mechanism of parameter detection list will be enabled; When it is false, the subsequent trigger will be executed as long as the event callback;")]
        public bool checkArguments = false;

        /// <summary>
        /// 参数检测列表:将Action事件的参数与本列表中参数根据规则做检测,全部符合规则才会执行后续的触发；
        /// </summary>
        [Name("参数检测列表")]
        [Tip("将Action事件的参数与本列表中参数根据规则做检测,全部符合规则才会执行后续的触发；", "Detect the parameters of the action event and the parameters in this list according to the rules, and the subsequent triggering will be executed only if they all meet the rules;")]
        [HideInSuperInspector(nameof(checkArguments), EValidityCheckType.False)]
        public List<ArgumentDetection> argumentDetections = new List<ArgumentDetection>();

        private bool CheckArguments(ActionMethodBase actionMethodBase, ITupleData tuple)
        {
            if (!checkArguments) return true;
            if (argumentDetections.Count == 0) return true;
            if (tuple.Length == 0) return false;
            try
            {
                if (argumentDetections.Any(arg => !arg.Check(tuple[arg._index]))) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 当事件被调用时
        /// </summary>
        /// <param name="actionMethodBase"></param>
        /// <param name="tuple"></param>
        protected void OnEventInvoked(ActionMethodBase actionMethodBase, ITupleData tuple)
        {
            if (invokedDelegate != null
                && invokedDelegate.Target == actionMethodBase
                && CheckArguments(actionMethodBase, tuple))
            {
                OnEventInvoked(tuple);
            }
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        protected override void AddListenerInternal()
        {
            if (invokedDelegate == null && fieldInfo is FieldInfo fi)
            {
                var delegateEvent = fi.GetValue(fi.IsStatic ? null : target) as Delegate;

                invokedDelegate = ActionMethodHelper.GetActionMethodDelegate(fi.FieldType);

                delegateEvent = Delegate.Combine(delegateEvent, invokedDelegate);

                fi.SetValue(fi.IsStatic ? null : target, delegateEvent);

                ActionMethodBase.onEventInvoked += OnEventInvoked;
            }
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        protected override void RemoveListenerInternal()
        {
            if (invokedDelegate != null && fieldInfo is FieldInfo fi)
            {
                var delegateEvent = fi.GetValue(fi.IsStatic ? null : target) as Delegate;

                delegateEvent = Delegate.Remove(delegateEvent, invokedDelegate);

                fi.SetValue(fi.IsStatic ? null : target, delegateEvent);

                invokedDelegate = null;

                ActionMethodBase.onEventInvoked -= OnEventInvoked;
            }
        }

        #region 针对Action事件的缓存机制

        /// <summary>
        /// 获取所有具有Action事件的类型全名称字符串；类型全名称的命名空间层级以'/'间隔；
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static string[] GetTypeFullNames(BindingFlags bindingFlags, bool includeBaseType) => TypeFullNameCache.GetTypeFullNames(bindingFlags, includeBaseType);

        /// <summary>
        /// 获取事件字段名称列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bindingFlags"></param>
        /// <returns></returns>
        public static string[] GetEventFieldNames(Type type, BindingFlags bindingFlags, bool includeBaseType) => FieldNameCache.GetEventFieldNames(type, bindingFlags, includeBaseType);

        internal class TypeFullNameCache : TIVCache<TypeFullNameCache, BindingFlags, bool, TypeFullNameCacheValue>
        {
            public static string[] GetTypeFullNames(BindingFlags bindingFlags, bool includeBaseType)
            {
                return GetCacheValue(bindingFlags, includeBaseType).fullNames;
            }
        }

        internal class TypeFullNameCacheValue : TIVCacheValue<TypeFullNameCacheValue, BindingFlags, bool>
        {
            /// <summary>
            /// 类型全名称的命名空间层级以'/'间隔；
            /// </summary>
            public string[] fullNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                var names = new List<string>();
                TypeHelper.GetTypes(type =>
                {
                    if (!type.IsClass || !type.IsPublic) return false;
                    if (type.IsGenericType || type.IsAbstract) return false;
                    if (!FieldNameCache.HasEvents(type, key1, key2)) return false;

                    names.Add(type.FullNameToHierarchyString());
                    return true;
                });
#if UNITY_EDITOR
                //排序
                names.Sort();
#endif
                //转数组
                fullNames = names.ToArray();
                return true;
            }
        }

        internal class FieldNameCache : TIVCache<FieldNameCache, Type, BindingFlags, bool, FieldNameCacheValue>
        {
            public static string[] GetEventFieldNames(Type type, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return Empty<string>.Array;
                return GetCacheValue(type, bindingFlags, includeBaseType)?.fieldNames ?? Empty<string>.Array;
            }

            public static bool HasEvents(Type type, BindingFlags bindingFlags, bool includeBaseType)
            {
                if (type == null) return false;
                return GetCacheValue(type, bindingFlags, includeBaseType).fieldNames.Length > 0;
            }
        }

        internal class FieldNameCacheValue : TIVCacheValue<FieldNameCacheValue, Type, BindingFlags, bool>
        {
            public string[] fieldNames { get; private set; } = Empty<string>.Array;

            public override bool Init()
            {
                var names = FieldInfosCache.Get(key1, key2, key3).Where(fi => ActionMethodHelper.IsActionMethodType(fi)).Cast(fi => fi.Name);

                //去重
                names = names.Distinct();
#if UNITY_EDITOR
                //排序
                names = names.OrderBy(n => n);
#endif
                //转数组
                fieldNames = names.ToArray();
                return true;
            }
        }

        #endregion

        #region IDropdownPopupAttribute

        /// <summary>
        /// 尝试获取选项文本列表；
        /// </summary>
        /// <param name="purpose">目标用途</param>
        /// <param name="propertyPath">属性路径</param>
        /// <param name="options">选项文本列表；如果期望下拉式弹出菜单出现层级，需要数组元素中有'/'</param>
        /// <returns></returns>
        public override bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            switch (purpose)
            {
                case nameof(MemberNamePopupAttribute):
                    {
                        options = ActionEventListener.GetEventFieldNames(targetType, bindingFlags, includeBaseType);
                        return true;
                    }
                case nameof(TypeFullNamePopupAttribute):
                    {
                        options = ActionEventListener.GetTypeFullNames(bindingFlags, includeBaseType);
                        return true;
                    }
            }
            return base.TryGetOptions(purpose, propertyPath, out options);
        }

        #endregion
    }
}
