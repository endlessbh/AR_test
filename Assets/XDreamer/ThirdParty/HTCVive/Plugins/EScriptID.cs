using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.Scripts;

namespace XCSJ.PluginHTCVive
{
    public static class HTCViveIDRange
    {
        public const int Begin = (int)EExtensionID._0x17;//35712
        public const int End = (int)EExtensionID._0xa - 1;

        public const int Fragment = 0x18;//24

        public const int Common = Begin + Fragment * 0;//35712
        public const int MonoBehaviour = Begin + Fragment * 1;//35736
        public const int StateLib = Begin + Fragment * 2;//35760
        public const int Tools = Begin + Fragment * 3;//35784
        public const int Editor = Begin + Fragment * 4;//35808
    }

    /// <summary>
    /// HTC Vive脚本ID
    /// </summary>
    [Name("HTC Vive脚本ID")]
    public enum EHTCViveScriptID
    {
        _Begin = HTCViveIDRange.Begin,

        #region HTC Vive-目录
        /// <summary>
        /// HTC Vive
        /// </summary>
        [ScriptName(nameof(HTCVive), nameof(HTCVive), EGrammarType.Category)]
        [ScriptDescription("HTC Vive的相关脚本目录；")]
        #endregion
        HTCVive,

        #region 激活SteamVR输入动作集
        /// <summary>
        /// 激活SteamVR输入动作集
        /// </summary>
        [ScriptName("激活SteamVR输入动作集", nameof(ActiveSteamVRInputActionSet))]
        [ScriptDescription("SteamVR动作集激活后才能执行对应的动作", "")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.String, "动作集名称:")]
        [ScriptParams(2, EParamType.Bool, "激活:", defaultObject = EBool.Yes)]
        #endregion
        ActiveSteamVRInputActionSet,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
