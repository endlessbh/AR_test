using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginXAR.Foundation.Faces.Tools;
using XCSJ.Scripts;

namespace XCSJ.Extension.Base.Dataflows.Binders
{
    /// <summary>
    /// 类型绑定器获取器接口
    /// </summary>
    public interface ITypeBinderGetter
    {
        /// <summary>
        /// 获取器所有者
        /// </summary>
        UnityEngine.Object owner { get; }

        /// <summary>
        /// 获取类型绑定器列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITypeBinder> GetTypeBinders();
    }

    /// <summary>
    /// 类型成员绑定器别名缓存
    /// </summary>
    public sealed class AliasCache : InstanceClass<AliasCache>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AliasCache()
        {
            XDreamerEvents.onAnyAssetsChanged -= MarkDirty;
            XDreamerEvents.onAnyAssetsChanged += MarkDirty;
        }

        /// <summary>
        /// 类型绑定信息
        /// </summary>
        private class TypeBinderInfo
        {
            public UnityEngine.Object owner { get; private set; }

            public ITypeBinder typeBinder { get; private set; }

            public TypeBinderInfo(UnityEngine.Object owner, ITypeBinder typeBinder)
            {
                this.owner = owner;
                this.typeBinder = typeBinder;
            }
        }

        /// <summary>
        /// 别名缓存
        /// </summary>
        private Dictionary<string, TypeBinderInfo> aliasMap = new Dictionary<string, TypeBinderInfo>();

        /// <summary>
        /// 别名已添加事件 ：当别名已添加到缓存之后的回调事件
        /// </summary>
        public event Action<string, ITypeBinder> onAddedAlias = null;

        /// <summary>
        /// 别名已移除事件 ：当别名从缓存已移除之后的回调事件
        /// </summary>
        public event Action<string, ITypeBinder> onRemovedAlias = null;

        /// <summary>
        /// 别名清除事件 ：当别名缓存全部清理之后的回调事件
        /// </summary>
        public event Action onClearedAlias = null;

        /// <summary>
        /// 包含别名
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public bool Contains(string alias) => !string.IsNullOrEmpty(alias) && aliasMap.ContainsKey(alias);

        /// <summary>
        /// 通过别名获取绑定器
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public ITypeBinder GetTypeBinder(string alias)
        {
            if (string.IsNullOrEmpty(alias)) return default;

            Update();

            if(aliasMap.TryGetValue(alias, out var value))
            {
                return value.typeBinder;
            }
            return default;
        }

        private string[] keys = null;

        /// <summary>
        /// 获取缓存别名
        /// </summary>
        /// <returns></returns>
        public string[] GetAliases()
        {
            Update();

            if (keys == null)
            {
                var list = aliasMap.Keys.ToList();
                list.Sort();
                keys = list.ToArray();
            }
            return keys;
        }

        /// <summary>
        /// 尝试获取别名对应的值
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetOwner(string alias, out UnityEngine.Object owner)
        {
            if (aliasMap.TryGetValue(alias, out var info))
            {
                owner = info.owner;
                return true;
            }
            owner = null;
            return false;
        }

        /// <summary>
        /// 尝试创建唯一别名
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="owner"></param>
        /// <param name="uniqueAlias"></param>
        /// <returns></returns>
        public bool TryCreateUniqueAlias(string alias, UnityEngine.Object owner, out string uniqueAlias)
        {
            if (string.IsNullOrEmpty(alias) || Contains(alias))
            {
                uniqueAlias = CommonFun.GetUniqueName(owner.name, (name) => !Contains(name));
                return true;
            }

            uniqueAlias = default;
            return false;
        }

        /// <summary>
        /// 刷新别名缓存：全场景查找类型绑定器的别名对象
        /// </summary>
        public void Refresh()
        {
            Clear();

            keys = null;
            var getters = FindTypeBinderGetters();

            foreach (var getter in getters)
            {
                AddAlias(getter);
            }
        }

        private bool dirty = true;

        private static void MarkDirty()
        {
            instance.dirty = true;
        }

        private void Update()
        {
            if (dirty)
            {
                dirty = false;
                Refresh();
            }
        }

        /// <summary>
        /// 使用别名类型绑定器拥有者来刷新缓存
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool UpdateWithOwner(UnityEngine.Object owner)
        {
            if (owner is ITypeBinderGetter getter)
            {
                // 移除与owner相关关键字
                var removeAliases = new List<(string, ITypeBinder)>();
                foreach (var kv in aliasMap)
                {
                    if (kv.Value.owner == owner)
                    {
                        removeAliases.Add((kv.Key, kv.Value.typeBinder));
                    }
                }
                foreach (var item in removeAliases)
                {
                    RemoveAlias(item.Item1, item.Item2);
                }

                // 重新添加别名
                AddAlias(getter);

                keys = null;
                return true;
            }
            return false;
        }

        private void AddAlias(ITypeBinderGetter getter)
        {
            var binders = getter.GetTypeBinders();
            foreach (var item in binders)
            {
                AddAlias(item.alias, getter.owner, item);
            }
        }

        private bool AddAlias(string alias, UnityEngine.Object owner, ITypeBinder binder)
        {
            if (string.IsNullOrEmpty(alias) || binder == null) return false;

            if (aliasMap.TryGetValue(alias, out var info))
            {
                if (info.owner != owner)
                {
                    Debug.LogWarning(string.Format("【{0}】别名注册重名！当前注册对象【{1}】，已存在对象【{2}】", alias, owner.ToScriptParamString(), info.owner.ToScriptParamString()));
                }
                return false;
            }
            else
            {
                //Debug.Log(string.Format("别名{0}添加！", alias));
                aliasMap.Add(alias, new TypeBinderInfo(owner, binder));
                onAddedAlias?.Invoke(alias, binder);
                return true;
            }
        }

        private void RemoveAlias(string alias, ITypeBinder typeBinder)
        {
            aliasMap.Remove(alias);
            onRemovedAlias?.Invoke(alias, typeBinder);
        }

        /// <summary>
        /// 清空
        /// </summary>
        private void Clear()
        {
            aliasMap.Clear();

            onClearedAlias?.Invoke();
        }

        private IEnumerable<ITypeBinderGetter> FindTypeBinderGetters()
        {
            var binders = new List<ITypeBinderGetter>();

            // 添加状态机上的别名绑定器
            var rootSM = RootStateMachine.instance;
            if (rootSM)
            {
                binders.AddRange(rootSM.GetComponentsInChildren<ITypeBinderGetter>(true));
            }
            // 添加场景中Mono组件上的别名绑定器
            binders.AddRange(ComponentCache.GetComponents<ITypeBinderGetter>(true));
            return binders;
        }

        /// <summary>
        /// unity对象转路径
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToPathString(UnityEngine.Object obj)
        {
            obj.ToScriptParamString();
            return CommonFun.ObjectToString(obj);
        }
    }

    /// <summary>
    /// 别名数据特性：主要用于修饰存储别名的字符串
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class AliasAttribute : PropertyAttribute
    {
        /// <summary>
        /// 别名数据类型
        /// </summary>
        public EAliasDataType aliasDataType = EAliasDataType.Get;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aliasDataType"></param>
        public AliasAttribute(EAliasDataType aliasDataType = EAliasDataType.Get)
        {
            this.aliasDataType = aliasDataType;
        }
    }

    /// <summary>
    /// 别名数据类型
    /// </summary>
    public enum EAliasDataType
    {
        Get,

        Set,
    }
}
