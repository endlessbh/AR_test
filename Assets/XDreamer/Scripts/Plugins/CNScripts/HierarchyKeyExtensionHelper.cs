using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Helper;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 层级键扩展组手：层级变量中的层级键扩展机制
    /// </summary>
    public static class HierarchyKeyExtensionHelper
    {
#if UNITY_EDITOR

        /// <summary>
        /// 扩展数据：仅适用于编辑器内界面绘制,运行时不可使用
        /// </summary>
        private static Dictionary<EHierarchyKeyMode, HierarchyKeyExtensionDataList> extensionDatas { get; } = new Dictionary<EHierarchyKeyMode, HierarchyKeyExtensionDataList>();

        /// <summary>
        /// 尝试获取层级键扩展数据列表
        /// </summary>
        /// <param name="hierarchyKeyMode"></param>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public static bool TryGetHierarchyKeyExtensionDataList(EHierarchyKeyMode hierarchyKeyMode, out HierarchyKeyExtensionDataList dataList) => extensionDatas.TryGetValue(hierarchyKeyMode, out dataList);

#endif

        private static bool CheckExtensionHierarchyKeys(HierarchyKeyAttribute hierarchyKeyAttribute)
        {
            if (hierarchyKeyAttribute == null) return false;
#if UNITY_EDITOR//在编辑器环境中执行严格的校验
            if (!HierarchyVarHelper.ValidExtensionHierarchyKey(hierarchyKeyAttribute.hierarchyKey))
            {
                Debug.LogError("期望的扩展层级键名称[" + hierarchyKeyAttribute.hierarchyKey + "]不是有效的扩展层级键名称！");
                return false;
            }
            foreach (var key in hierarchyKeyAttribute.hierarchyKeys)
            {
                if (!HierarchyVarHelper.ValidExtensionHierarchyKey(key))
                {
                    Debug.LogError("期望的扩展层级键名称[" + hierarchyKeyAttribute.hierarchyKey + "]不是有效的扩展层级键名称！");
                    return false;
                }
            }
#endif
            return true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
#if UNITY_EDITOR
            extensionDatas.Clear();
#endif

            var ms = MethodHelper.GetStaticMethodsAndAttribute<HierarchyKeyAttribute>();
            foreach (var info in ms)
            {
                var attribute = info.attribute;
                if (!CheckExtensionHierarchyKeys(attribute)) continue;

                switch (attribute.hierarchyKeyMode)
                {
                    case EHierarchyKeyMode.Get:
                        {
                            var func = ToGetFunc(info.methodInfo);
                            if (func == null) break;
                            AddGetFunc(info, func);
                            HierarchyVarHelper.RegisterGetHierarchyVarValueFunc(attribute.hierarchyKey, func);
                            foreach (var key in attribute.hierarchyKeys)
                            {
                                HierarchyVarHelper.RegisterGetHierarchyVarValueFunc(key, func);
                            }
                            break;
                        }
                    case EHierarchyKeyMode.Set:
                        {
                            var func = ToSetFunc(info.methodInfo);
                            if (func == null) break;
                            AddSetFunc(info, func);
                            HierarchyVarHelper.RegisterSetHierarchyVarValueFunc(attribute.hierarchyKey, func);
                            foreach (var key in attribute.hierarchyKeys)
                            {
                                HierarchyVarHelper.RegisterSetHierarchyVarValueFunc(key, func);
                            }
                            break;
                        }
                }
            }

#if UNITY_EDITOR

            //处理获取与设置均支持的情况
            if (!extensionDatas.TryGetValue(EHierarchyKeyMode.Get, out var getList)) return;
            if (!extensionDatas.TryGetValue(EHierarchyKeyMode.Set, out var setList)) return;
            if (!extensionDatas.TryGetValue(EHierarchyKeyMode.Both, out var bothList))
            {
                extensionDatas[EHierarchyKeyMode.Both] = bothList = new HierarchyKeyExtensionDataList(EHierarchyKeyMode.Both);
            }

            foreach (var getData in getList.hierarchyKeyExtensionDatas)
            {
                foreach (var setData in setList.hierarchyKeyExtensionDatas)
                {
                    if (getData.hierarchyKey == setData.hierarchyKey)//需要确保主层级键相同
                    {
                        bothList.hierarchyKeyExtensionDatas.Add(new HierarchyKeyExtensionData(getData.getInfo, getData.getFunc, setData.setInfo, setData.setFunc));
                    }
                }
            }
#endif
        }


        private static Dictionary<string, string> displayKeyCache = new Dictionary<string, string>();

        /// <summary>
        /// 获取显示键：将键字符串中_替换为/
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDisplayKey(string key)
        {
            if (displayKeyCache.TryGetValue(key, out var value)) return value;
            displayKeyCache[key] = value = key.Replace("_", "/");
            return value;
        }

        #region 获取扩展机制

        private static void AddGetFunc(MethodHelper.Info<HierarchyKeyAttribute> info, Func<IVarContext, IHierarchyVar, string, object> getFunc)
        {
#if UNITY_EDITOR
            var mode = info.attribute.hierarchyKeyMode;
            if (!extensionDatas.TryGetValue(mode, out var list))
            {
                extensionDatas[mode] = list = new HierarchyKeyExtensionDataList(mode);
            }
            list.hierarchyKeyExtensionDatas.Add(new HierarchyKeyExtensionData(info, getFunc));
#endif
        }

        private static Func<IVarContext, IHierarchyVar, string, object> ToGetFunc(MethodInfo methodInfo)
        {
            try
            {
                return Delegate.CreateDelegate(typeof(Func<IVarContext, IHierarchyVar, string, object>), methodInfo) as Func<IVarContext, IHierarchyVar, string, object>;
            }
            catch { return default; }
        }

        #endregion

        #region 设置扩展机制

        private static void AddSetFunc(MethodHelper.Info<HierarchyKeyAttribute> info, Func<IVarContext, IHierarchyVar, string, object, bool> setFunc)
        {
#if UNITY_EDITOR
            var mode = info.attribute.hierarchyKeyMode;
            if (!extensionDatas.TryGetValue(mode, out var list))
            {
                extensionDatas[mode] = list = new HierarchyKeyExtensionDataList(mode);
            }
            list.hierarchyKeyExtensionDatas.Add(new HierarchyKeyExtensionData(info, setFunc));
#endif
        }

        private static Func<IVarContext, IHierarchyVar, string, object, bool> ToSetFunc(MethodInfo methodInfo)
        {
            try
            {
                return Delegate.CreateDelegate(typeof(Func<IVarContext, IHierarchyVar, string, object, bool>), methodInfo) as Func<IVarContext, IHierarchyVar, string, object, bool>;
            }
            catch { return default; }
        }

        #endregion
    }

    /// <summary>
    /// 层级键扩展数据列表
    /// </summary>
    public class HierarchyKeyExtensionDataList
    {
        /// <summary>
        /// 层级键模式
        /// </summary>
        public EHierarchyKeyMode hierarchyKeyMode { get; private set; }

        /// <summary>
        /// 层级键扩展数据列表
        /// </summary>
        public List<HierarchyKeyExtensionData> hierarchyKeyExtensionDatas = new List<HierarchyKeyExtensionData>();

        private Dictionary<string, (string, HierarchyKeyExtensionData)> _displayDictioanry = default;

        /// <summary>
        /// 显示字典：经过键名优化与排序；
        /// </summary>
        public Dictionary<string, (string, HierarchyKeyExtensionData)> displayDictioanry
        {
            get
            {
                if (_displayDictioanry == null)
                {
                    _displayDictioanry = new Dictionary<string, (string, HierarchyKeyExtensionData)>();
                    foreach (var data in hierarchyKeyExtensionDatas)
                    {
                        var key = data.hierarchyKey;
                        try
                        {
                            _displayDictioanry.Add(HierarchyKeyExtensionHelper.GetDisplayKey(key), (key, data));

                            foreach (var sameFuncKey in data.hierarchyKeys)
                            {
                                _displayDictioanry.Add(HierarchyKeyExtensionHelper.GetDisplayKey(sameFuncKey), (sameFuncKey, data));
                            }
                        }
                        catch
                        {
                            Debug.LogError("在对层级键进行扩展时出现重名的扩展层级键名称: " + key);
                        }
                    }
                    _displayDictioanry = _displayDictioanry.Sort();
                }
                return _displayDictioanry;
            }
        }

        private string[] _displayArray = default;

        /// <summary>
        /// 显示数组
        /// </summary>
        public string[] displayArray
        {
            get
            {
                if (_displayArray == null)
                {
                    _displayArray = displayDictioanry.Keys.ToArray();
                }
                return _displayArray;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="hierarchyKeyMode"></param>
        public HierarchyKeyExtensionDataList(EHierarchyKeyMode hierarchyKeyMode)
        {
            this.hierarchyKeyMode = hierarchyKeyMode;
        }
    }

    /// <summary>
    /// 层级键扩展数据
    /// </summary>
    public class HierarchyKeyExtensionData
    {
        /// <summary>
        /// 层级键模式
        /// </summary>
        public EHierarchyKeyMode hierarchyKeyMode { get; private set; }

        #region 获取

        /// <summary>
        /// 获取信息
        /// </summary>
        public MethodHelper.Info<HierarchyKeyAttribute> getInfo { get; private set; }

        /// <summary>
        /// 获取函数
        /// </summary>
        public Func<IVarContext, IHierarchyVar, string, object> getFunc { get; private set; }

        #endregion

        #region 设置

        /// <summary>
        /// 获取信息
        /// </summary>
        public MethodHelper.Info<HierarchyKeyAttribute> setInfo { get; private set; }

        /// <summary>
        /// 设置函数
        /// </summary>
        public Func<IVarContext, IHierarchyVar, string, object, bool> setFunc { get; private set; }

        #endregion

        /// <summary>
        /// 层级键
        /// </summary>
        public string hierarchyKey
        {
            get
            {
                switch (hierarchyKeyMode)
                {
                    case EHierarchyKeyMode.Get: return getInfo.attribute.hierarchyKey;
                    case EHierarchyKeyMode.Set: return setInfo.attribute.hierarchyKey;
                    case EHierarchyKeyMode.Both: return getInfo.attribute.hierarchyKey;
                    default: return default;
                }
            }
        }
        private string[] _hierarchyKeys = null;

        /// <summary>
        /// 层级键数组
        /// </summary>
        public string[] hierarchyKeys
        {
            get
            {
                switch (hierarchyKeyMode)
                {
                    case EHierarchyKeyMode.Get: return getInfo.attribute.hierarchyKeys;
                    case EHierarchyKeyMode.Set: return setInfo.attribute.hierarchyKeys;
                    case EHierarchyKeyMode.Both:
                        {
                            if (_hierarchyKeys == null)
                            {
                                _hierarchyKeys = getInfo.attribute.hierarchyKeys.Intersect(setInfo.attribute.hierarchyKeys).ToArray();
                            }
                            return _hierarchyKeys;
                        }
                    default: return default;
                }
            }
        }

        /// <summary>
        /// 显示层级键数组
        /// </summary>
        public bool displayHierarchyKeys { get; set; } = false;

        private string _hierarchyKeysString = null;

        /// <summary>
        /// 层级键数组字符串
        /// </summary>
        public string hierarchyKeysString => _hierarchyKeysString ?? (_hierarchyKeysString = hierarchyKeys.ToStringDirect(","));

        private string _tip = null;

        /// <summary>
        /// 提示
        /// </summary>
        public string tip
        {
            get
            {
                if (_tip == null)
                {
                    switch (hierarchyKeyMode)
                    {
                        case EHierarchyKeyMode.Get: _tip = CommonFun.Tip(getInfo.methodInfo); break;
                        case EHierarchyKeyMode.Set: _tip = CommonFun.Tip(setInfo.methodInfo); break;
                        case EHierarchyKeyMode.Both:
                            {
                                _tip = "<color=#0000FFFF>获取：</color>" + CommonFun.Tip(getInfo.methodInfo) + "\t<color=#0000FFFF>设置：</color>" + CommonFun.Tip(setInfo.methodInfo);
                                break;
                            }
                        default: _tip = ""; break;
                    }
                }
                return _tip;
            }
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="getInfo"></param>
        /// <param name="getFunc"></param>
        public HierarchyKeyExtensionData(MethodHelper.Info<HierarchyKeyAttribute> getInfo, Func<IVarContext, IHierarchyVar, string, object> getFunc, MethodHelper.Info<HierarchyKeyAttribute> setInfo, Func<IVarContext, IHierarchyVar, string, object, bool> setFunc)
        {
            this.hierarchyKeyMode = EHierarchyKeyMode.Both;
            this.getInfo = getInfo;
            this.getFunc = getFunc;
            this.setInfo = setInfo;
            this.setFunc = setFunc;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="getInfo"></param>
        /// <param name="getFunc"></param>
        public HierarchyKeyExtensionData(MethodHelper.Info<HierarchyKeyAttribute> getInfo, Func<IVarContext, IHierarchyVar, string, object> getFunc)
        {
            this.hierarchyKeyMode = EHierarchyKeyMode.Get;
            this.getInfo = getInfo;
            this.getFunc = getFunc;
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="setInfo"></param>
        /// <param name="setFunc"></param>
        public HierarchyKeyExtensionData(MethodHelper.Info<HierarchyKeyAttribute> setInfo, Func<IVarContext, IHierarchyVar, string, object, bool> setFunc)
        {
            this.hierarchyKeyMode = EHierarchyKeyMode.Set;
            this.setInfo = setInfo;
            this.setFunc = setFunc;
        }
    }

    /// <summary>
    /// 层级键特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HierarchyKeyAttribute : Attribute
    {
        /// <summary>
        /// 层级键模式
        /// </summary>
        public EHierarchyKeyMode hierarchyKeyMode { get; private set; }

        /// <summary>
        /// 层级键
        /// </summary>
        public string hierarchyKey { get; private set; }

        /// <summary>
        /// 层级键数组
        /// </summary>
        public string[] hierarchyKeys { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="hierarchyKey"></param>
        /// <param name="hierarchyKeys"></param>
        public HierarchyKeyAttribute(EHierarchyKeyMode hierarchyKeyMode, string hierarchyKey, params string[] hierarchyKeys)
        {
            this.hierarchyKeyMode = hierarchyKeyMode;
            this.hierarchyKey = !string.IsNullOrEmpty(hierarchyKey) ? hierarchyKey : throw new ArgumentException(nameof(hierarchyKey));
            this.hierarchyKeys = hierarchyKeys ?? Empty<string>.Array;
        }
    }

    /// <summary>
    /// 层级键模式：用于扩展层级键机制时的具体模式
    /// </summary>
    [Name("层级键模式")]
    public enum EHierarchyKeyMode
    {
        /// <summary>
        /// 获取
        /// </summary>
        [Name("主页")]
        [XCSJ.Attributes.Icon(EIcon.Home)]
        Main,

        /// <summary>
        /// 获取
        /// </summary>
        [Name("获取")]
        [XCSJ.Attributes.Icon(EIcon.Property)]
        Get,

        /// <summary>
        /// 设置
        /// </summary>
        [Name("设置")]
        [XCSJ.Attributes.Icon(EIcon.Property)]
        Set,

        /// <summary>
        /// 二者
        /// </summary>
        [Name("二者")]
        [XCSJ.Attributes.Icon(EIcon.Property)]
        Both,
    }
}
