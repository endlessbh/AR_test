using XCSJ.Attributes;
using XCSJ.PluginStereoView.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginStereoView.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        _Begin = IDRange.Begin,

        #region 立体显示-目录
        /// <summary>
        /// 立体显示
        /// </summary>
        [ScriptName("立体显示", nameof(StereoView), EGrammarType.Category)]
        [ScriptDescription("立体显示的相关脚本目录；")]
        #endregion
        StereoView,

        #region 开启
        [ScriptName("开启立体显示", nameof(StartStereoView))]
        [ScriptDescription("开启立体显示模块")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        #endregion
        StartStereoView,

        #region 关闭
        [ScriptName("关闭立体显示", nameof(CloseStereoView))]
        [ScriptDescription("关闭立体显示模块")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        #endregion
        CloseStereoView,

        #region 获取屏幕锚点关联信息
        [ScriptName("获取屏幕锚点关联信息", nameof(GetScreenAnchorLinkInfo))]
        [ScriptDescription("获取屏幕锚点关联信息")]
        [ScriptReturn("成功返回 信息值 ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "屏幕锚点关联", typeof(ScreenAnchorLink))]
        [ScriptParams(2, EParamType.Combo, "信息类型", "关联旋转")]
        #endregion
        GetScreenAnchorLinkInfo,

        #region 设置屏幕锚点关联信息
        [ScriptName("设置屏幕锚点关联信息", nameof(SetScreenAnchorLinkInfo))]
        [ScriptDescription("设置屏幕锚点关联信息")]
        [ScriptReturn("成功返回 #True ;失败返回 #False;")]
        [ScriptParams(1, EParamType.GameObjectComponent, "屏幕锚点关联", typeof(ScreenAnchorLink))]
        [ScriptParams(2, EParamType.Combo, "信息类型", "关联旋转")]
        [ScriptParams(3, EParamType.String, "信息")]
        #endregion
        SetScreenAnchorLinkInfo,

        /// <summary>
        /// 当前已使用的脚本最大ID
        /// </summary>
        MaxCurrent
    }
}
