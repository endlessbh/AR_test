using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginHTCVive.Base;
using XCSJ.PluginXXR.Interaction.Toolkit;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Tools.Controllers;

namespace XCSJ.PluginHTCVive.Tools
{
    /// <summary>
    /// 交互IO通过Vive手柄
    /// </summary>
    [Name("交互IO通过Vive手柄")]
    [Tip("通过Vive手柄手柄模拟控制器交互的输入输出", "Simulate the input and output of controller interaction through vive handle")]
    [RequireManager(typeof(HTCViveManager), typeof(XXRInteractionToolkitManager))]
    [Tool(XRITHelper.IO, nameof(AnalogController))]
    [Tool(HTCViveManager.Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [DisallowMultipleComponent]
    [Owner(typeof(HTCViveManager))]
    public class InteractIOByVive : BaseAnalogControllerIO, IInteractIO
    {
        /// <summary>
        /// Vive交互手柄轴与按钮
        /// </summary>
        [Name("Vive交互手柄轴与按钮")]
        public ViveControllerInteractAxisAndButton _viveControllerInteractAxisAndButton = new ViveControllerInteractAxisAndButton();

        /// <summary>
        /// 是否按下激活键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfActivate(AnalogController analogController) => _viveControllerInteractAxisAndButton.IsPressedOfActivate();

        /// <summary>
        /// 是否按下选择键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfSelect(AnalogController analogController) => _viveControllerInteractAxisAndButton.IsPressedOfSelect();

        /// <summary>
        /// 是否按下UI键
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfUI(AnalogController analogController) => _viveControllerInteractAxisAndButton.IsPressedOfUI();
    }

    /// <summary>
    /// Vive交互手柄轴与按钮
    /// </summary>
    [Serializable]
    [Name("Vive交互手柄轴与按钮")]
    public class ViveControllerInteractAxisAndButton : BaseViveController
    {
        #region Vive Focus交互按钮

        /// <summary>
        /// Vive Focus 激活按钮
        /// </summary>
        [Name("激活按钮")]
        [Tip("Vive Focus 激活按钮", "Vive Focus activation button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _ViveFocusButtonOfActivate = EViveFocusAxisAndButton.Trigger;

        /// <summary>
        /// Vive Focus 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [Tip("Vive Focus 选择按钮", "Vive Focus selection button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _ViveFocusButtonOfSelect = EViveFocusAxisAndButton.GripButton;

        /// <summary>
        /// Vive Focus UI按钮
        /// </summary>
        [Name("UI按钮")]
        [Tip("Vive Focus UI按钮", "Vive Focus UI button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _ViveFocusButtonOfUI = EViveFocusAxisAndButton.Trigger;

        #endregion

        #region Vive Pro交互按钮

        /// <summary>
        /// Vive Pro 激活按钮
        /// </summary>
        [Name("激活按钮")]
        [Tip("Vive Pro 激活按钮", "Vive Pro activation button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _ViveProButtonOfActivate = EViveProAxisAndButton.Trigger;

        /// <summary>
        /// Vive Pro 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [Tip("Vive Pro 选择按钮", "Vive Pro select button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _ViveProButtonOfSelect = EViveProAxisAndButton.GripButton;

        /// <summary>
        /// Vive Pro UI按钮
        /// </summary>
        [Name("UI按钮")]
        [Tip("Vive Pro UI按钮", "Vive Pro UI button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _ViveProButtonOfUI = EViveProAxisAndButton.Trigger;

        #endregion

        #region Vive Cosmos交互按钮

        /// <summary>
        /// Vive Cosmos 激活按钮
        /// </summary>
        [Name("激活按钮")]
        [Tip("Vive Cosmos 激活按钮", "Vive Cosmos activation button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _ViveCosmosButtonOfActivate = EViveCosmosAxisAndButton.Trigger;

        /// <summary>
        /// Vive Cosmos 选择按钮
        /// </summary>
        [Name("选择按钮")]
        [Tip("Vive Cosmos 选择按钮", "Vive Cosmos select button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _ViveCosmosButtonOfSelect = EViveCosmosAxisAndButton.GripButton;

        /// <summary>
        /// Vive Cosmos UI按钮
        /// </summary>
        [Name("UI按钮")]
        [Tip("Vive Cosmos UI按钮", "Vive Cosmos UI button")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _ViveCosmosButtonOfUI = EViveCosmosAxisAndButton.Trigger;

        #endregion

        /// <summary>
        /// 死区：对于轴的值超过本死区值时，认为事件成立；
        /// </summary>
        [Name("死区")]
        [Tip("对于轴的值超过本死区值时，认为事件成立；", "When the value of the axis exceeds the dead band value, the event is considered to be true;")]
        [Range(-1, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// 激活按键按下
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfActivate()
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusButtonOfActivate.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.VivePro: return _ViveProButtonOfActivate.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosButtonOfActivate.IsPressed(_handleType, _deadZone);
            }
            return false;
        }

        /// <summary>
        /// 选择按键按下
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfSelect()
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusButtonOfSelect.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.VivePro: return _ViveProButtonOfSelect.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosButtonOfSelect.IsPressed(_handleType, _deadZone);
            }
            return false;
        }

        /// <summary>
        /// UI按键按下
        /// </summary>
        /// <returns></returns>
        public bool IsPressedOfUI()
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusButtonOfUI.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.VivePro: return _ViveProButtonOfUI.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosButtonOfUI.IsPressed(_handleType, _deadZone);
            }
            return false;
        }
    }
}
