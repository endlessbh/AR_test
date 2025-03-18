using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Caches;
using XCSJ.Extension.GenericStandards;

namespace XCSJ.Extension.Base.Extensions
{
    /// <summary>
    /// 类型缓存扩展类：用于处理运行时因Unity代码剥离导致无法做类型反射查找的问题；
    /// </summary>
    public static class TypeCacheExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.IPhonePlayer:
                    {
                        TypesCache.customGetTypes += GetComponentTypes;
                        TypesCache.customGetTypes += CustomGetTypes;
                        break;
                    }
            }
        }

        private static KeyValuePair<bool, IEnumerable<Type>> GetComponentTypes() => new KeyValuePair<bool, IEnumerable<Type>>(true, ComponentManagerExtension.SubclassOfComponent);

        private static KeyValuePair<bool, IEnumerable<Type>> CustomGetTypes() => new KeyValuePair<bool, IEnumerable<Type>>(true, Types);

        /// <summary>
        /// 期望运行时生效的类型
        /// </summary>
        private static readonly HashSet<Type> Types = new HashSet<Type>()
        {
            typeof(UnityEngine.RenderSettings)
        };
    }
}
