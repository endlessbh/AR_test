using System;
using System.Collections.Generic;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.Scripts;

namespace XCSJ.Extension.CNScripts
{
    /// <summary>
    /// 脚本组手
    /// </summary>
    public static class CNScriptHelper
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "中文脚本";

        /// <summary>
        /// 中文脚本菜单
        /// </summary>
        public const string CNScriptMenu = Product.Name + "/中文脚本/";

        /// <summary>
        /// 输入菜单
        /// </summary>
        public const string InputMenu = CNScriptMenu + "输入/";

        /// <summary>
        /// UGUI菜单
        /// </summary>
        public const string UGUIMenu = CNScriptMenu + "UGUI/";

        /// <summary>
        /// NGUI菜单
        /// </summary>
        public const string NGUIMenu = CNScriptMenu + "NGUI/";

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Init()
        {
            HierarchyKeyExtensionHelper.Init();
        }
    }

    /// <summary>
    /// 设置变量字符串值
    /// </summary>
    [Serializable]
    public class SetVarStringValue
    {
        /// <summary>
        /// 结果变量字符串
        /// </summary>
        [Name("结果变量字符串")]
        [Tip("将成功执行的结果信息存储在结果变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to the result variable string")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public string _resultVarString;

        /// <summary>
        /// 结果变量字符串列表
        /// </summary>
        [Name("结果变量字符串列表")]
        [Tip("将成功执行的结果信息存储在结果变量字符串列表内每个变量字符串对应的变量中", "Store the successful execution result information in the variable corresponding to each variable string in the result variable string list")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public List<string> _resultVarStrings = new List<string>();

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="varValue"></param>
        /// <returns></returns>
        public bool SetVarValue(object varValue) => _resultVarString.TrySetOrAddSetHierarchyVarValue(varValue, _resultVarStrings);
    }
}
