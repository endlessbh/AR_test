using XCSJ.Attributes;
using XCSJ.PluginXGUI.Windows.ImageBrowers;
using XCSJ.Scripts;

namespace XCSJ.PluginXGUI.CNScripts
{
    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptID
    {
        /// <summary>
        /// 结束
        /// </summary>
        _Begin = IDRange.Begin,

        #region XGUI-目录
        /// <summary>
        /// XGUI
        /// </summary>
        [ScriptName(nameof(XGUI), nameof(XGUI), EGrammarType.Category)]
        [ScriptDescription("XGUI的相关脚本目录；")]
        #endregion
        XGUI,

        #region 图片浏览器控制
        /// <summary>
        /// 图片浏览器控制
        /// </summary>
        [ScriptName("图片浏览器控制", nameof(ImageBrowerControl))]
        [ScriptDescription("图片浏览器相关的各种基础控制命令")]
        [ScriptReturn("成功返回 #True ; 失败返回 #False ;")]
        [ScriptParams(1, EParamType.GameObject, "图片浏览器:", typeof(ImageBrower))]
        [ScriptParams(2, EParamType.Combo, "控制命令:", "全屏", "取消全屏", "上一张", "下一张","跳转至指定页")]
        #endregion 
        ImageBrowerControl
    }
}
