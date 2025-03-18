using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.ComponentModel;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXBox.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginXBox
{
    /// <summary>
    /// XBox：用于管理XBox手柄并处理对应事件
    /// </summary>
    [Serializable]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Peripheral)]
    [ComponentOption(EComponentOption.Optional)]
    [Name(XBoxHelper.Title)]
    [Tip("用于管理XBox手柄并处理对应事件；", "Used to manage Xbox handle and handle corresponding events;")]
    [Guid("CB78A802-FE7B-4862-8694-8A6A055996A8")]
    [Version("23.428")]
    public class XBoxManager : BaseManager<XBoxManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns>脚本列表</returns>
        public override List<Script> GetScripts() => Script.GetScriptsOfEnum<EScriptID>(this);

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <param name="id">脚本ID</param>
        /// <param name="param">脚本参数</param>
        /// <returns>ReturnValue</returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            //switch ((EScriptID)id)
            //{
            //}
            return ReturnValue.No;
        }
    }
}
