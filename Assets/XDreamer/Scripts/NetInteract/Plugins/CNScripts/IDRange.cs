using XCSJ.Attributes;
using XCSJ.Extension;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.CNScripts
{
    /// <summary>
    /// ID区间
    /// </summary>
    public class IDRange
    {
        /// <summary>
        /// 开始值，38784
        /// </summary>
        public const int Begin = (int)EExtensionID._0x2f;

        /// <summary>
        /// 结束值，38912-1=38911
        /// </summary>
        public const int End = (int)EExtensionID._0x30 - 1;
    }

    /// <summary>
    /// 脚本ID
    /// </summary>
    [Name("脚本ID")]
    public enum EScriptsID
    {
        /// <summary>
        /// 开始
        /// </summary>
        _Begin = IDRange.Begin,

        #region 网络交互-目录
        /// <summary>
        /// 网络交互
        /// </summary>
        [ScriptName(NetInteractHelper.Title, nameof(NetInteract), EGrammarType.Category)]
        [ScriptDescription(NetInteractHelper.Title + "操作的目录")]
        #endregion
        NetInteract,

        #region 设置客户端属性

        /// <summary>
        /// 设置客户端属性:设置客户端的IP或端口
        /// </summary>
        [ScriptName("设置客户端属性", nameof(ClientPropertySet))]
        [ScriptDescription("设置客户端的IP或端口")]
        [ScriptReturn("返回值为空", "Return null")]
        [ScriptParams(1, EParamType.GameObjectComponent, "客户端:", typeof(Client))]
        [ScriptParams(2, EParamType.Combo, "属性类型:", "IP", "端口")]
        [ScriptParams(3, EParamType.String, "IP:")]
        [ScriptParams(4, EParamType.Int, "端口:")]
        #endregion
        ClientPropertySet,
    }
}
