using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Logs
{
    /// <summary>
    /// 日志查看器绘制器
    /// </summary>
    [Name("日志查看器绘制器")]
    [RequireManager(typeof(LogManager))]
    public class LogViewerDrawer : InteractProvider
    {
        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            LogViewerWindow.instance.Update();
        }

        /// <summary>
        /// 绘制GUI
        /// </summary>
        protected void OnGUI()
        {
            LogViewerWindow.instance.OnGUI();
        }
    }
}
