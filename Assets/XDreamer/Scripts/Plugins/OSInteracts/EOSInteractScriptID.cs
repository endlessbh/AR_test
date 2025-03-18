using XCSJ.Attributes;
using XCSJ.Scripts;

namespace XCSJ.Extension.OSInteracts
{

    public static class OSInteractIDRange
    {
        public const int Begin = (int)EExtensionID._0x1;//32896
        public const int End = (int)EExtensionID._0x2 - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//32896
        public const int MonoBehaviour = Begin + Fragment * 1;//32920
        public const int StateLib = Begin + Fragment * 2;//32944
        public const int Tools = Begin + Fragment * 3;//32968
        public const int Editor = Begin + Fragment * 4;//32992
    }

    /// <summary>
    /// OS交互脚本ID
    /// </summary>
    [Name("OS交互脚本ID")]
    public enum EOSInteractScriptID
    {
        _Begin = OSInteractIDRange.Begin,

        #region OS交互-目录
        /// <summary>
        /// OS交互<br />
        /// </summary>
        [ScriptName("OS交互", nameof(OSInteract), EGrammarType.Category)]
        #endregion
        OSInteract,

        #region 向OS发送消息
        [ScriptName("向OS发送消息", nameof(SendMessageToOS))]
        [ScriptDescription("发送消息到OS；不同的消息命令时,不同输入参数需要与遵守特定的约束规范，以体现不同参数的所代表的意义；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(0, UnityToOSMsgCmd_ScriptParam.ScriptParamType, "消息命令：")]
        [ScriptParams(1, EParamType.String, "参数1：")]
        [ScriptParams(2, EParamType.String, "参数2：")]
        [ScriptParams(3, EParamType.String, "参数3：")]
        [ScriptParams(4, EParamType.String, "参数4：")]
        #endregion
        SendMessageToOS,

        #region 返回OS
        /// <summary>
        /// 返回OS
        /// </summary>
        [ScriptName("返回OS", nameof(BackOS))]
        [ScriptDescription("返回OS;")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.Combo, "类型：", "返回并加载主场景", "直接返回")]
        [ScriptParams(2, EParamType.String, "信息：")]
        [ScriptParams(3, EParamType.String, "额外信息：")]
        #endregion
        BackOS,

        #region 通过OS切换场景
        /// <summary>
        /// 通过OS切换场景
        /// </summary>
        [ScriptName("通过OS切换场景", nameof(SwitchSceneByOS))]
        [ScriptDescription("Unity向OS发送切换场景命令")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "场景路径：")]
        [ScriptParams(2, EParamType.String, "场景名称：")]
        [ScriptParams(3, EParamType.String, "其他消息：")]
        [ScriptParams(4, SceneHandleRuleWhenFail_ScriptParam.ScriptParamType, "失败时场景处理规则：", defaultObject = ESceneHandleRuleWhenFail.LoadPreviousScene)]
        #endregion
        SwitchSceneByOS,

        #region 通过主场景切换场景
        [ScriptName("通过主场景切换场景", nameof(SwitchSceneByMainScene))]
        [ScriptDescription("Unity先加载主场景,然后主场景再执行切换场景的命令；**特别说明:本脚本的执行需要特制的主场景的配合！普通主场景无法完成对应动作！**处理规则在某些平台会失效；如果场景已被导入则执行加载场景操作，未导入则执行导入并加载场景操作；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.File, "场景路径(如果场景已被导入,本项可为空)：")]
        [ScriptParams(2, EParamType.String, "场景名称：")]
        [ScriptParams(3, SceneHandleRuleWhenFail_ScriptParam.ScriptParamType, "失败时场景处理规则：", defaultObject = ESceneHandleRuleWhenFail.LoadPreviousScene)]
        #endregion
        SwitchSceneByMainScene,

        #region 处理切换场景失败
        [ScriptName("处理切换场景失败", nameof(HandleSwitchSceneFail))]
        [ScriptDescription("目前主要用于‘通过主场景切换场景’脚本、'通过OS切换场景'脚本、所有通过OS发送的加载（切换）场景类型命令在发生场景处理失败时的后续处理；如果未明确指定处理规则，则使用主场景设置中的‘失败时场景处理规则’进行处理；")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        #endregion
        HandleSwitchSceneFail,

        #region 获取之前场景信息
        [ScriptName("获取之前场景信息", nameof(GetPreviousSceneInfo))]
        [ScriptDescription("本信息仅在调用执行‘返回OS’、‘通过OS切换场景’、‘通过主场景切换场景’中文脚本命令时自动设置!!!")]
        [ScriptParams(1, EParamType.Combo, "信息：", "场景全路径", "场景名称")]
        #endregion
        GetPreviousSceneInfo,

        #region 模拟OS向Unity发送消息
        [ScriptName("模拟OS向Unity发送消息", nameof(AnalogOSSendMessageToUnity))]
        [ScriptDescription("**非专业人士,本脚本谨慎使用**")]
        [ScriptParams(1, OSToUnityMsgCmd_ScriptParam.ScriptParamType, "消息命令:")]
        [ScriptParams(2, VariableName_ScriptParam.ScriptParamType, "Key1", defaultObject = "scenePath")]
        [ScriptParams(3, EParamType.File, "Value1")]
        [ScriptParams(4, VariableName_ScriptParam.ScriptParamType, "Key2", defaultObject = "sceneName")]
        [ScriptParams(5, EParamType.String, "Value2")]
        #endregion
        AnalogOSSendMessageToUnity,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent,
    }

}

