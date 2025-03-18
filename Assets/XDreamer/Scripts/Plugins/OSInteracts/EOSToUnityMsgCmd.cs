using XCSJ.Attributes;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{
    /// <summary>
    /// OS到Unity消息命令脚本参数
    /// </summary>
    [ScriptParamType(ScriptParamType)]
    public class OSToUnityMsgCmd_ScriptParam : EnumScriptParam<EOSToUnityMsgCmd>
    {
        /// <summary>
        /// 脚本参数
        /// </summary>
        public const int ScriptParamType = SceneHandleRuleWhenFail_ScriptParam.ScriptParamType + 2;
    }

    /// <summary>
    /// OS向Uinty消息命令:OS向Uinty发送的各种消息命令
    /// </summary>
    [Name("OS向Uinty消息命令")]
    public enum EOSToUnityMsgCmd
    {
        [Name("无")]
        None = 0,

        [Name("导入并加载场景")]
        ImportAndLoadScene,

        [Name("导入场景")]
        ImportScene,

        [Name("加载场景")]
        LoadScene,

        [Name("加载或导入并加载场景")]
        LoadOrImportAndLoadScene,

        [Name("卸载子场景")]
        UnloadSubScene,

        [Name("卸载子场景(通过索引)")]
        UnloadSubSceneByIndex,

        [Name("卸载全部子场景")]
        UnloadAllSubScene,

        [Name("请求场景名称列表")]
        RequestSceneNameList,

        [Name("用户自定义")]
        UserDefine,

        [Name("调用自定义函数")]
        CallUserDefineFun,

        [Name("执行XCSJ脚本")]
        RunXCSJScript,

        [Name("执行单句XCSJ脚本并返回结果")]
        RunSingleXCSJScriptAndReturnResult,

        [Name("请求图片二维码扫描")]
        RequestImageQRCodeScan,
    }
}
