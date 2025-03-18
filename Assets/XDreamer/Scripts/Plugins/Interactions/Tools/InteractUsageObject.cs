using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools;

namespace XCSJ.Extension.Interactions.Tools
{
    /// <summary>
    /// 交互用途对象
    /// </summary>
    public abstract class InteractUsageObject : InteractTagObject, IUsageHost
    {
        private Usage _usage;

        /// <summary>
        /// 用途
        /// </summary>
        public Usage usage => _usage ?? (_usage = new Usage(this));

        /// <summary>
        /// 获取用途关键字
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetUsageKey(string key = null) => key ?? GetType().Name;

        /// <summary>
        /// 添加用途
        /// </summary>
        /// <param name="usageData"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool OnAddUsage(UsageData usageData, InteractObject user) => true;

        /// <summary>
        /// 移除用途
        /// </summary>
        /// <param name="usageData"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual bool OnRemoveUsage(UsageData usageData, InteractObject user) => true;
    }

    /// <summary>
    /// 用途：
    /// 1、用于记录当前用途的集合
    /// 2、通过关键字将用途集合进行分组归类
    /// </summary>
    public class Usage
    {
        /// <summary>
        /// 用途宿主
        /// </summary>
        public IUsageHost usageHost { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="usageHost"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Usage(IUsageHost usageHost)
        {
            this.usageHost = usageHost ?? throw new ArgumentNullException(nameof(usageHost));
        }

        /// <summary>
        /// 用途字典：键=分类名称（分组），值=用途集合对象
        /// </summary>
        public Dictionary<string, UsageData> usageMap { get; } = new Dictionary<string, UsageData>();

        /// <summary>
        /// 能否添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="canAdd"></param>
        /// <returns></returns>
        public bool CanAdd(string key, Func<UsageData, InteractObject> canAdd) => canAdd != null && GetOrCreate(key) is UsageData usageData ? canAdd(usageData) : false;

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getUser"></param>
        /// <returns></returns>
        public bool Add(string key, Func<UsageData, InteractObject> getUser)
        {
            if (getUser == null) return false;
            var usageData = GetOrCreate(key);
            if (usageData == null) return false;
            var interactObject = getUser(usageData);
            if (interactObject && usageData.users.AddWithDistinct(interactObject))
            {
                if (usageHost.OnAddUsage(usageData, interactObject))
                {
                    return true;
                }
                else
                {
                    usageData.users.Remove(interactObject);
                }
            }
            return false;
        }

        public bool Add(string key, InteractObject user) => Add(key, u => user);

        /// <summary>
        /// 能否移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="canRemove"></param>
        /// <returns></returns>
        public bool CanRemove(string key, Func<UsageData, InteractObject> canRemove) => canRemove != null && Get(key) is UsageData usageData ? canRemove(usageData) : false;

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="matchRemove"></param>
        /// <returns></returns>
        public bool Remove(string key, Func<UsageData, InteractObject> matchRemove)
        {
            if (matchRemove == null) return false;
            var usageData = Get(key);
            if (usageData == null) return false;
            var interactObject = matchRemove(usageData);
            if (interactObject && usageData.users.Remove(interactObject))
            {
                if (usageHost.OnRemoveUsage(usageData, interactObject))
                {
                    return true;
                }
                else
                {
                    usageData.users.Add(interactObject);
                }
                return true;
            }
            return false;
        }
        public bool Remove(string key, InteractObject user) => Remove(key, u => user);

        /// <summary>
        /// 是否包含关键字
        /// </summary>
        /// <param name="key"></param>
        /// <param name="contains"></param>
        /// <returns></returns>
        public bool Contains(string key, Func<UsageData, bool> contains) => contains != null && Get(key) is UsageData usageData ? contains(usageData) : false;

        /// <summary>
        /// 是否包含匹配的关键字和使用者
        /// </summary>
        /// <param name="key"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Contains(string key, InteractObject user) => Contains(key, usageData => usageData.Contains(user));

        /// <summary>
        /// 获取匹配关键字和条件的数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public int GetCount(string key, Func<UsageData, int> match) => match != null && Get(key) is UsageData usageData ? match(usageData) : 0;

        /// <summary>
        /// 获取匹配关键字的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetCount(string key) => Get(key) is UsageData usageData ? usageData.userCount : 0;

        /// <summary>
        /// 获取匹配关键字的用途数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UsageData Get(string key) => !string.IsNullOrEmpty(key) && usageMap.TryGetValue(key, out var usageData) ? usageData : default;

        /// <summary>
        /// 获取或创建匹配关键字的用途数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UsageData GetOrCreate(string key) => string.IsNullOrEmpty(key) ? default : (Get(key) ?? (usageMap[key] = new UsageData(key)));
    }

    /// <summary>
    /// 用途数据
    /// </summary>
    public class UsageData
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key"></param>
        public UsageData(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string key { get; private set; }

        /// <summary>
        /// 使用者列表
        /// </summary>
        public List<InteractObject> users { get; private set; } = new List<InteractObject>();

        /// <summary>
        /// 使用者数量
        /// </summary>
        public int userCount => users.Count;

        /// <summary>
        /// 是否包含使用者
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Contains(InteractObject user) => users.Contains(user);
    }

    /// <summary>
    /// 用途宿主接口
    /// </summary>
    public interface IUsageHost
    {
        bool OnAddUsage(UsageData usageData, InteractObject user);

        bool OnRemoveUsage(UsageData usageData, InteractObject user);
    }
}
