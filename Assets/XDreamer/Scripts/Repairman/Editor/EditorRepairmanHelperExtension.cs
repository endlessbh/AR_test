using UnityEditor;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginRepairman.Kernel;

namespace XCSJ.EditorRepairman
{
    /// <summary>
    /// 编辑器MMO助手扩展
    /// </summary>
    public class EditorRepairmanHelperExtension
    {
        /// <summary>
        /// 初始化
        /// </summary>
        [InitializeOnLoadMethod]
        public static void Init()
        {
            DefaultRepairmanHandler.Init();
        }
    }
}
