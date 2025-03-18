using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{
    /// <summary>
    /// 变量名脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class VariableName_ScriptParam : EnumScriptParam<EVariableName>
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = SceneHandleRuleWhenFail_ScriptParam.ScriptParamType + 3;
    }

    /// <summary>
    /// 变量名称枚举，与各运行时平台（OS）做数据通信时的变量名，为兼容以前版本，部分部分枚举变量名以小写字母开头；
    /// </summary>
    [Name("变量名")]
    public enum EVariableName
    {
        // OS与Unity通信数据时使用 --- 作为JSON字符串的Key使用
        MsgCmd,
        Msg,

        // OS --> Unity ：为符合部分平台的变量命名规范将变量首字母小写；当前场景可能生效的全局变量；
        scenePath,
        sceneName,
        sceneIndex,//场景索引由1开始递增
        userDefine,
        userDefineFunName,
        param,
        xcsjScript,
        imagePath,
        otherInfo,

        // Unity --> OS ：为符合部分平台的变量命名规范将变量首字母小写；不同消息命令时各参数代表不同意思；
        activeSceneName,//当前激活场景名称
        paramCount,//用于标识当前返回的有效参数的数目
        // 其他参数形式为： paramN 模式，例如 param1,param2,param3...数字N根据具体参数数目递增

        //静态变量 -- 使用首字母大写，用于切换场景
        NextScenePath,
        NextSceneName,
        NextSceneRuleWhenFail,
        PreviousScenePath,
        PreviousSceneName,
    }
}
