using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{
    /// <summary>
    /// Unity到OS消息命令脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class UnityToOSMsgCmd_ScriptParam : EnumScriptParam<EUnityToOSMsgCmd>
    {
        /// <summary>
        /// 脚本参数类型
        /// </summary>
        public const int ScriptParamType = SceneHandleRuleWhenFail_ScriptParam.ScriptParamType + 1;
    }

    /// <summary>
    /// Uinty向OS消息命令:Uinty向OS发送的各种消息命令
    /// </summary>
    [Name("Uinty向OS消息命令")]
    public enum EUnityToOSMsgCmd
    {
        [Name("无")]
        None = 0,

        [Name("用户自定义")]
        UserDefine,

        [Name("返回OS")]
        BackOS,

        [Name("Unity引擎加载完成")]
        UnityEngineLoadedFinish,

        [Name("导入场景完成")]
        ImportSceneFinish,

        [Name("加载场景完成")]
        [Tip("由加载成功后的场景在启动时发送；加载场景、导入并加载场景执行成功后均回调的是本消息；")]
        LoadSceneFinish,

        [Name("定时消息")]
        [Tip("当无导入或加载场景任务时，定时回调的消息；")]
        TimedMessage,

        [Name("定时消息(导入并加载中)")]
        [Tip("当有导入并加载场景任务时，定时回调的消息；")]
        TimedMessageInImportAndLoad,

        [Name("定时消息(导入中)")]
        [Tip("当有导入场景任务时，定时回调的消息；")]
        TimedMessageInImport,

        [Name("定时消息(加载中)")]
        [Tip("当有加载场景任务时，定时回调的消息；")]
        TimedMessageInLoad,

        [Name("导入并加载场景失败")]
        ImportAndLoadSceneFailed,

        [Name("导入场景失败")]
        ImportSceneFailed,

        [Name("加载场景失败")]
        LoadSceneFailed,

        [Name("场景名称列表")]
        SceneNameList,

        [Name("通过OS切换场景")]
        SwitchSceneByOS,

        [Name("二维码扫描结果")]
        QRCodeScanResult,

        [Name("单句XCSJ脚本执行结果")]
        SingleXCSJScriptRunResult,

        [Name("卸载子场景完成")]
        UnloadSubSceneFinish,

        [Name("卸载全部子场景完成")]
        UnloadAllSubSceneFinish,
    }
}
