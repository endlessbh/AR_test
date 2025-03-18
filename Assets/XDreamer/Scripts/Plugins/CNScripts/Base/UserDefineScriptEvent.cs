using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.CNScripts;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts.Base
{
    /// <summary>
    /// 用户定义脚本事件类型
    /// </summary>
    [Name("用户定义脚本事件类型")]
    public enum EUserDefineScriptEventType
    {
        /// <summary>
        /// 用户定义事件1
        /// </summary>
        [Name("用户定义事件1")]
        UserDefineEvent1,

        /// <summary>
        /// 用户定义事件2
        /// </summary>
        [Name("用户定义事件2")]
        UserDefineEvent2,

        /// <summary>
        /// 用户定义事件3
        /// </summary>
        [Name("用户定义事件3")]
        UserDefineEvent3,

        /// <summary>
        /// 用户定义事件4
        /// </summary>
        [Name("用户定义事件4")]
        UserDefineEvent4,

        /// <summary>
        /// 用户定义事件5
        /// </summary>
        [Name("用户定义事件5")]
        UserDefineEvent5,

        /// <summary>
        /// 用户定义事件6
        /// </summary>
        [Name("用户定义事件6")]
        UserDefineEvent6,

        /// <summary>
        /// 用户定义事件7
        /// </summary>
        [Name("用户定义事件7")]
        UserDefineEvent7,

        /// <summary>
        /// 用户定义事件8
        /// </summary>
        [Name("用户定义事件8")]
        UserDefineEvent8,
    }

    /// <summary>
    /// 用户定义脚本事件函数
    /// </summary>
    [Name("用户定义脚本事件函数")]
    [Serializable]
    public class UserDefineScriptEventFunction : EnumFunction<EUserDefineScriptEventType> { }

    /// <summary>
    /// 用户自定义脚本事件函数集合
    /// </summary>
    [Name("用户自定义脚本事件函数集合")]
    [Serializable]
    public class UserDefineScriptEventFunctionCollection : EnumFunctionCollection<EUserDefineScriptEventType, UserDefineScriptEventFunction> { }

    /// <summary>
    /// 用户自定义脚本事件
    /// </summary>
    [Serializable]
    [Name(Title)]
    [DisallowMultipleComponent]
    [AddComponentMenu(CNScriptHelper.CNScriptMenu + Title)]
    public class UserDefineScriptEvent : BaseScriptEvent<EUserDefineScriptEventType, UserDefineScriptEventFunction, UserDefineScriptEventFunctionCollection>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "用户自定义脚本事件";
    }
}
