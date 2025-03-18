using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers
{
    /// <summary>
    /// 交互IO通过键码:通过键码模拟控制器交互输入输出
    /// </summary>
    [Name("交互IO通过键码")]
    [Tip("通过键码模拟控制器交互的输入输出", "Simulate the input and output of controller interaction through key code")]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [XCSJ.Attributes.Icon(EIcon.Keyboard)]
    [DisallowMultipleComponent]
    public class InteractIOByKeyCode : BaseAnalogControllerIO, IInteractIO
    {
        /// <summary>
        /// 激活设置
        /// </summary>
        [Name("激活设置")]
        public KeyCodesSetting _settingOfActivate = new KeyCodesSetting();

        /// <summary>
        /// 选择设置
        /// </summary>
        [Name("选择设置")]
        public KeyCodesSetting _settingOfSelect = new KeyCodesSetting();

        /// <summary>
        /// UI设置
        /// </summary>
        [Name("UI设置")]
        public KeyCodesSetting _settingOfUI = new KeyCodesSetting();

        /// <inheritdoc/>
        public bool IsPressedOfActivate(AnalogController analogController) => _settingOfActivate.IsPressed();

        /// <inheritdoc/>
        public bool IsPressedOfSelect(AnalogController analogController) => _settingOfSelect.IsPressed();

        /// <inheritdoc/>
        public bool IsPressedOfUI(AnalogController analogController) => _settingOfUI.IsPressed();

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.XModifyProperty(() =>
            {
                _settingOfSelect._keyCode = KeyCode.Space;
                _settingOfActivate._keyCode = KeyCode.Alpha1;
                _settingOfUI._keyCode = KeyCode.Space;
            });
        }
    }
}
