using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSamsungWMR.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginSamsungWMR.Tools
{
    /// <summary>
    /// 交互IO通过三星玄龙手柄
    /// </summary>
    [Name("交互IO通过三星玄龙手柄")]
    [Tip("通过三星玄龙手柄手柄模拟控制器交互的输入输出", "The input and output of the controller interaction is simulated through the Samsung Xuanlong handle")]
    [RequireManager(typeof(SamsungWMRManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [Tool(SamsungWMRManager.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    [Owner(typeof(SamsungWMRManager))]
    public class InteractIOBySamsungWMR : BaseAnalogControllerIO, IInteractIO
    {
        /// <summary>
        /// 手柄类型
        /// </summary>
        [Name("手柄类型")]
        [EnumPopup]
        public EHandRule _handleType = EHandRule.Left;

        /// <summary>
        /// 激活按钮
        /// </summary>
        [Name("激活按钮")]
        [EnumPopup]
        public ESamsungWMRAxisAndButton _buttonOfActivate = ESamsungWMRAxisAndButton.Trigger;

        /// <summary>
        /// 激活按钮
        /// </summary>
        public ESamsungWMRAxisAndButton buttonOfActivate
        {
            get => _buttonOfActivate;
            set => this.XModifyProperty(ref _buttonOfActivate, value);
        }

        /// <summary>
        /// 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [EnumPopup]
        public ESamsungWMRAxisAndButton _buttonOfSelect = ESamsungWMRAxisAndButton.Grip;

        /// <summary>
        /// 选择按钮
        /// </summary>
        public ESamsungWMRAxisAndButton buttonOfSelect
        {
            get => _buttonOfSelect;
            set => this.XModifyProperty(ref _buttonOfSelect, value);
        }

        /// <summary>
        /// UI按钮
        /// </summary>
        [Name("UI按钮")]
        [EnumPopup]
        public ESamsungWMRAxisAndButton _buttonOfUI = ESamsungWMRAxisAndButton.Trigger;

        /// <summary>
        /// UI按钮
        /// </summary>
        public ESamsungWMRAxisAndButton buttonOfUI
        {
            get => _buttonOfUI;
            set => this.XModifyProperty(ref _buttonOfUI, value);
        }

        /// <summary>
        /// 死区
        /// </summary>
        [Name("死区")]
        [Range(0, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// 是否按下激活键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _buttonOfActivate.IsPressed(_handleType, _deadZone);

        /// <summary>
        /// 是否按下选择键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _buttonOfSelect.IsPressed(_handleType, _deadZone); 

        /// <summary>
        /// 是否按下UI键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _buttonOfUI.IsPressed(_handleType, _deadZone); 
    }
}
