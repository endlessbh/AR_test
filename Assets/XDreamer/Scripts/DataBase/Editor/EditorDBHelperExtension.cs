using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginDataBase.Tools.Kernel;

namespace XCSJ.EditorDataBase
{
    /// <summary>
    /// 编辑器DB助手扩展
    /// </summary>
    public static class EditorDBHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            DefaultDBHandler.Init();
        }
    }
}
