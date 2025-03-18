using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.ComponentModel;
using XCSJ.Extension.ExtensionExample;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension
{
    /// <summary>
    /// 扩展开发：用于对<see cref="XDreamer"/>进行扩展开发的案例代码
    /// </summary>
    [Serializable]
    [Index(index = int.MaxValue)]
    [DisallowMultipleComponent]
    [ComponentKit(EKit.Advanced)]
    [ComponentOption(EComponentOption.Optional)]
    [Name("扩展开发")]
    [Tip("用于对" + Product.Name + "进行扩展开发的案例代码", "For " + Product.Name + " case code for extension development")]
    [Guid("D9768580-A377-488C-8F60-D4F9D23D410E")]
    [Version("23.428")]
    public sealed class ExtensionExampleManager : BaseManager<ExtensionExampleManager>
    {
        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <returns></returns>
        public override List<Script> GetScripts()
        {
            var list = Script.GetScriptsOfEnum<EExampleScriptID>(this);
            try
            {
                //处理扩展脚本
                list.AddRange(Script.GetScriptsOfStaticMethod(TypeHelper.FindTypeInAppWithInterface(typeof(IExtensionScript)).ToArray()));
            }
            catch { }
            return list;
        }

        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override ReturnValue ExecuteScript(int id, ScriptParamList param)
        {
            switch ((EExampleScriptID)id)
            {
                case EExampleScriptID.OutputHelloWorld:
                    {
                        Debug.Log("Hello World！");
                        break;
                    }
                default:
                    {
                        if (ScriptManager.instance.IDScripts.TryGetValue(id, out var script) && script != null)
                        {
                            EExampleScriptID sid = (EExampleScriptID)id;
                            if (param.Count > 0)
                            {
                                Debug.Log(string.Format("脚本命令: [{0}], ID: [{1}] 参数信息-->", script.name.ToString(), sid.ToString()));
                                foreach (var kv in param)
                                {
                                    Debug.Log(string.Format("参数key: [{0}], 参数value: [{1}], 参数value类型: [{2}]", kv.Key.ToString(), kv.Value, kv.Value?.GetType()?.ToString() ?? ""));
                                }
                                Debug.Log(string.Format("<--脚本命令: [{0}], ID: [{1}] 参数信息!", script.name.ToString(), sid.ToString()));
                            }
                            else
                            {
                                Debug.Log("脚本命令: [" + sid.ToString() + "] 无参数");
                            }
                            return ReturnValue.Yes;
                        }
                        break;
                    }
            }
            return ReturnValue.No;
        }
    }
}


