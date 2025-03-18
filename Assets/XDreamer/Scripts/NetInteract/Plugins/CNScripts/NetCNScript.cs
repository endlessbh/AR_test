using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginNetInteract.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.CNScripts
{
    /// <summary>
    /// 网络中文脚本
    /// </summary>
    [Serializable]
    [Name("网络中文脚本")]
    public class NetCNScript : IDataValidity
    {
        /// <summary>
        /// 变量替换规则
        /// </summary>
        [Name("变量替换规则")]
        [OnlyMemberElements]
        public EVariableReplaceRulePropertyValue _variableReplaceRule = new EVariableReplaceRulePropertyValue();

        /// <summary>
        /// 脚本集
        /// </summary>
        [Name("脚本集")]
        [OnlyMemberElements]
        public CustomFunctionPropertyValue _scriptSet = new CustomFunctionPropertyValue();

        /// <summary>
        /// 转网络中文脚本问题
        /// </summary>
        /// <returns></returns>
        public NetCNScriptQuestion ToNetQuestion()
        {
            var question = new NetCNScriptQuestion() { questionCode = EQuestionCode.Valid };
            question.scriptPackage.packageType = ENetCNScriptPackageType.Execute;
            question.scriptPackage.scriptStrings.AddRange(GetScriptStrings(_scriptSet.GetScriptStrings(), _variableReplaceRule.GetValue()));
            return question;
        }

        /// <summary>
        /// 隐式转换为网络中文脚本问题
        /// </summary>
        /// <param name="netCNScript"></param>
        public static implicit operator NetCNScriptQuestion(NetCNScript netCNScript) => netCNScript?.ToNetQuestion();

        /// <summary>
        /// 转网络中文脚本答案
        /// </summary>
        /// <returns></returns>
        public NetCNScriptAnswer ToNetAnswer()
        {
            var answer = new NetCNScriptAnswer() { answerCode = EAnswerCode.Valid };
            answer.netCNScriptPackage.packageType = ENetCNScriptPackageType.Execute;
            answer.netCNScriptPackage.scriptStrings.AddRange(GetScriptStrings(_scriptSet.GetScriptStrings(), _variableReplaceRule.GetValue()));
            return answer;
        }

        /// <summary>
        /// 隐式转换为网络中文脚本答案
        /// </summary>
        /// <param name="netCNScript"></param>
        public static implicit operator NetCNScriptAnswer(NetCNScript netCNScript) => netCNScript?.ToNetAnswer();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity() => true;

        private static IEnumerable<string> GetScriptStrings(Function function, EVariableReplaceRule variableReplaceRule)
        {
            return GetScriptStrings(function.GetScriptStrings().Cast(ss => ss.scriptString), variableReplaceRule);
        }

        private static IEnumerable<string> GetScriptStrings(IEnumerable<string> function, EVariableReplaceRule variableReplaceRule)
        {
            switch (variableReplaceRule)
            {
                case EVariableReplaceRule.LocalReplace:
                    {
                        var scriptManager = ScriptManager.instance;
                        if (!scriptManager) break;

                        var list = new List<string>();
                        foreach (var ss in function)
                        {
                            if (ss.IndexOf(ScriptOption.VarFlag) >= 0 && ScriptString.TryParse(ss, scriptManager, out var rt))
                            {
                                var sb = new StringBuilder();
                                if (!string.IsNullOrEmpty(rt.returnValue)) sb.Append(rt.returnValue + ScriptOption.Equal);
                                sb.Append(rt.cmd);
                                foreach (var kv in rt.paramRT)
                                {
                                    var pRT = kv.Value;

                                    sb.Append(ScriptOption.ScriptParamDelimiter);

                                    var result = pRT.scriptParam.varStringAnalysisResult;
                                    if (result == null)
                                    {
                                        sb.Append(pRT.paramString);
                                    }
                                    else
                                    {
                                        switch (result.varScope)
                                        {
                                            case EVarScope.Global:
                                                {
                                                    if (scriptManager.TryGetHierarchyVarValue(result, out var varValue))
                                                    {
                                                        sb.Append(varValue);
                                                    }
                                                    else
                                                    {
                                                        sb.Append(pRT.paramString);
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    sb.Append(pRT.paramString);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                list.Add(sb.ToString());
                            }
                            else
                            {
                                list.Add(ss);
                            }
                        }
                        return list;
                    }
                case EVariableReplaceRule.LocalReaplce_NonSystemVariable:
                    {
                        var scriptManager = ScriptManager.instance;
                        if (!scriptManager) break;

                        var list = new List<string>();
                        foreach (var ss in function)
                        {
                            if (ss.IndexOf(ScriptOption.VarFlag) >= 0 && ScriptString.TryParse(ss, scriptManager, out var rt))
                            {
                                var sb = new StringBuilder();
                                if (!string.IsNullOrEmpty(rt.returnValue)) sb.Append(rt.returnValue + ScriptOption.Equal);
                                sb.Append(rt.cmd);
                                foreach (var kv in rt.paramRT)
                                {
                                    var pRT = kv.Value;

                                    sb.Append(ScriptOption.ScriptParamDelimiter);

                                    var result = pRT.scriptParam.varStringAnalysisResult;
                                    if (result == null || result.isSystemVariable)
                                    {
                                        sb.Append(pRT.paramString);
                                    }
                                    else
                                    {
                                        switch (result.varScope)
                                        {
                                            case EVarScope.Global:
                                                {
                                                    if (scriptManager.TryGetHierarchyVarValue(result, out var varValue))
                                                    {
                                                        sb.Append(varValue);
                                                    }
                                                    else
                                                    {
                                                        sb.Append(pRT.paramString);
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    sb.Append(pRT.paramString);
                                                    break;
                                                }
                                        }
                                    }
                                }

                                list.Add(sb.ToString());
                            }
                            else
                            {
                                list.Add(ss);
                            }
                        }
                        return list;
                    }
                case EVariableReplaceRule.RemoteReplace: break;
            }
            return function;
        }
    }

    /// <summary>
    /// 变量替换规则属性值
    /// </summary>
    [Name("变量替换规则属性值")]
    [Serializable]
    [PropertyType(typeof(EVariableReplaceRule))]
    public class EVariableReplaceRulePropertyValue : EnumPropertyValue<EVariableReplaceRule> { }

    /// <summary>
    /// 变量替换规则
    /// </summary>
    [Name("变量替换规则")]
    public enum EVariableReplaceRule
    {
        /// <summary>
        /// 远程替换
        /// </summary>
        [Name("远程替换")]
        [Tip("本地不做任何修改，将脚本语句全部发送到远程接收端后做处理", "Do not make any changes locally. Send all script statements to the remote receiver for processing")]
        RemoteReplace = 0,

        /// <summary>
        /// 本地替换：在本地将所有出现的全局变量，全部替换为对应的值
        /// </summary>
        [Name("本地替换")]
        [Tip("在本地将所有出现的全局变量，全部替换为对应的值", "Replace all global variables with corresponding values locally")]
        LocalReplace,

        /// <summary>
        /// 本地替换非系统变量：在本地将所有出现的非系统型的全局变量，全部替换为对应的值
        /// </summary>
        [Name("本地替换非系统变量")]
        [Tip("在本地将所有出现的非系统型的全局变量，全部替换为对应的值", "Replace all non systematic global variables with corresponding values locally")]
        LocalReaplce_NonSystemVariable,
    }

    /// <summary>
    /// 网络中文脚本问题
    /// </summary>
    public class NetCNScriptQuestion : NetQuestion
    {
        /// <summary>
        /// 网络中文脚本包
        /// </summary>
        public NetCNScriptPackage scriptPackage { get; set; } = new NetCNScriptPackage();
    }

    /// <summary>
    /// 网络中文脚本答案
    /// </summary>
    public class NetCNScriptAnswer : NetAnswer
    {
        /// <summary>
        /// 网络中文脚本包
        /// </summary>
        public NetCNScriptPackage netCNScriptPackage { get; set; } = new NetCNScriptPackage();
    }

    /// <summary>
    /// 网络中文脚本包
    /// </summary>
    public class NetCNScriptPackage
    {
        /// <summary>
        /// 脚本字符串列表
        /// </summary>
        public List<string> scriptStrings { get; set; } = new List<string>();

        /// <summary>
        /// 返回值列表
        /// </summary>
        public List<string> returnValues { get; set; } = new List<string>();

        /// <summary>
        /// 包类型
        /// </summary>
        public ENetCNScriptPackageType packageType { get; set; } = ENetCNScriptPackageType.None;

        /// <summary>
        /// 隐式转换为网络中文脚本问题
        /// </summary>
        /// <param name="netCNScriptPackage"></param>
        public static implicit operator NetCNScriptQuestion(NetCNScriptPackage netCNScriptPackage) => new NetCNScriptQuestion() { scriptPackage = netCNScriptPackage };

        /// <summary>
        /// 隐式转换为网络中文脚本答案
        /// </summary>
        /// <param name="netCNScriptPackage"></param>
        public static implicit operator NetCNScriptAnswer(NetCNScriptPackage netCNScriptPackage) => new NetCNScriptAnswer() { netCNScriptPackage = netCNScriptPackage };

        /// <summary>
        /// 处理
        /// </summary>
        public NetCNScriptPackage Handle()
        {
            var package = new NetCNScriptPackage();
            switch (packageType)
            {
                case ENetCNScriptPackageType.Execute:
                    {
                        var manager = ScriptManager.instance;
                        if(manager)
                        {
                            switch(scriptStrings.Count)
                            {
                                case 0:
                                    {
                                        package.packageType = ENetCNScriptPackageType.ReturnFail;
                                        break;
                                    }
                                case 1:
                                    {
                                        var cmd = scriptStrings[0];
                                        package.scriptStrings.Add(cmd);
                                        package.returnValues.Add(manager.ExecuteScript(cmd).ToStringWithPrefix());
                                        package.packageType = ENetCNScriptPackageType.ReturnSuccess;
                                        break;
                                    }
                                default:
                                    {
                                        package.scriptStrings.AddRange(scriptStrings);
                                        manager.ExecuteScripts(scriptStrings);
                                        package.returnValues.Add(scriptStrings.Count.ToString());//多行脚本语句时返回脚本数量
                                        package.packageType = ENetCNScriptPackageType.ReturnSuccess;
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            package.packageType = ENetCNScriptPackageType.ReturnFail;
                        }
                        break;
                    }
                case ENetCNScriptPackageType.ReturnSuccess:
                    {
                        //Debug.Log("返回成功:" + returnValues[0]);
                        break;
                    }
                case ENetCNScriptPackageType.ReturnFail:
                    {
                        //Debug.Log("返回失败:");
                        break;
                    }
            }
            return package;
        }
    }

    /// <summary>
    /// 网络中文脚本包类型
    /// </summary>
    [Name("网络中文脚本包类型")]
    public enum ENetCNScriptPackageType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 执行
        /// </summary>
        Execute,

        /// <summary>
        /// 返回成功
        /// </summary>
        ReturnSuccess,

        /// <summary>
        /// 返回失败
        /// </summary>
        ReturnFail,
    }
}
