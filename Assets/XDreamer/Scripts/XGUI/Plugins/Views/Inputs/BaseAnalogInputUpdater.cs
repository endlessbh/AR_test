using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 基础模拟输入更新器
    /// </summary>
    [Name("基础模拟输入更新器")]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    public abstract class BaseAnalogInputUpdater : InteractProvider, IAnalogInputUpdater
    {
        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void UpdateAxis(IInput input, string name, float value);

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public abstract void UpdateButton(IInput input, string name, bool downOrUp);
    }
}
