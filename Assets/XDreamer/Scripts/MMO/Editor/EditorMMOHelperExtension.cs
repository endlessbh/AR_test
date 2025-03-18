using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.Kernel;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 编辑器MMO助手扩展
    /// </summary>
    public static class EditorMMOHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            DefaultMMOHandler.Init();
        }
    }
}
