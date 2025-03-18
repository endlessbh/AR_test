using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginHTCVive.Base
{
    /// <summary>
    /// Vive设备助手
    /// </summary>
    public static class ViveDeviceHelper
    {
    }

    /// <summary>
    /// Vive设备类型
    /// </summary>
    [Name("Vive设备类型")]
    public enum EViveDeviceType
    {
        /// <summary>
        /// Vive Focus
        /// </summary>
        [Name("Vive Focus")]
        ViveFocus,

        /// <summary>
        /// Vive Pro
        /// </summary>
        [Name("Vive Pro")]
        VivePro,

        /// <summary>
        /// Vive Cosmos
        /// </summary>
        [Name("Vive Cosmos")]
        ViveCosmos,
    }

    public abstract class BaseViveController
    {
        /// <summary>
        /// Vive设备类型
        /// </summary>
        [Name("Vive设备类型")]
        [EnumPopup]
        public EViveDeviceType _ViveDeviceType = EViveDeviceType.VivePro;

        /// <summary>
        /// 手柄类型
        /// </summary>
        [Name("手柄类型")]
        [EnumPopup]
        public EHandRule _handleType = EHandRule.Left;

        /// <summary>
        /// 设备友好字符串
        /// </summary>
        public string deviceFriendlyString
        {
            get
            {
                return CommonFun.Name(_ViveDeviceType);
            }
        }
    }

    /// <summary>
    /// Vive手柄轴与按钮
    /// </summary>
    [Serializable]
    [Name("Vive手柄轴与按钮")]
    public class ViveControllerAxisAndButton: BaseViveController
    {
        /// <summary>
        /// Vive Focus轴与按钮
        /// </summary>
        [Name("Vive Focus轴与按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _ViveFocusAxisAndButton = EViveFocusAxisAndButton.Trigger;

        /// <summary>
        /// Vive Pro轴与按钮
        /// </summary>
        [Name("Vive Pro轴与按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _ViveProAxisAndButton = EViveProAxisAndButton.Trigger;

        /// <summary>
        /// Vive Cosmos轴与按钮
        /// </summary>
        [Name("Vive Cosmos轴与按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _ViveCosmosAxisAndButton = EViveCosmosAxisAndButton.Trigger;

        /// <summary>
        /// 死区：对于轴的值超过本死区值时，认为事件成立；
        /// </summary>
        [Name("死区")]
        [Tip("对于轴的值超过本死区值时，认为事件成立；", "When the value of the axis exceeds the dead band value, the event is considered to be true;")]
        [Range(-1, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// 按键按下
        /// </summary>
        /// <returns></returns>
        public bool IsPressed()
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusAxisAndButton.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.VivePro: return _ViveProAxisAndButton.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosAxisAndButton.IsPressed(_handleType, _deadZone);
            }
            return false;
        }

        /// <summary>
        /// 轴与按钮友好字符串
        /// </summary>
        public string axisAndButtonFriendlyString
        {
            get
            {
                string buttonName = "";
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            buttonName = CommonFun.Name(_ViveFocusAxisAndButton);
                            break;
                        }
                    case EViveDeviceType.VivePro:
                        {
                            buttonName = CommonFun.Name(_ViveProAxisAndButton);
                            break;
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            buttonName = CommonFun.Name(_ViveCosmosAxisAndButton);
                            break;
                        }
                }
                return CommonFun.Name(_ViveDeviceType) + ":" + buttonName;
            }
        }
    }


    /// <summary>
    /// Vive手柄2D轴
    /// </summary>
    [Name("Vive手柄2D轴")]
    [Serializable]
    public class ViveControllerAxis2D : BaseViveController
    {
        /// <summary>
        /// Vive Focus 2D轴
        /// </summary>
        [Name("Vive Focus 2D轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxis2D _ViveFocusAxis2D = EViveFocusAxis2D.ThumbStick;

        /// <summary>
        /// 摇杆或触控板
        /// </summary>
        [Name("Vive Pro 2D轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxis2D _ViveProAxis2D = EViveProAxis2D.ThumbStick;

        /// <summary>
        /// Vive Cosmos 2D轴
        /// </summary>
        [Name("Vive Cosmos 2D轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxis2D _ViveCosmosAxis2D = EViveCosmosAxis2D.JoyStick;

        /// <summary>
        /// 获取2D轴输入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetAxis2DValue(out Vector2 value)
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusAxis2D.TryGetAxis2DValue(_handleType, out value);
                case EViveDeviceType.VivePro: return _ViveProAxis2D.TryGetAxis2DValue(_handleType, out value);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosAxis2D.TryGetAxis2DValue(_handleType, out value);
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 2D轴友好字符串
        /// </summary>
        public string axis2DFriendlyString
        {
            get
            {
                string buttonName = "";
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            buttonName = CommonFun.Name(_ViveFocusAxis2D);
                            break;
                        }
                    case EViveDeviceType.VivePro:
                        {
                            buttonName = CommonFun.Name(_ViveProAxis2D);
                            break;
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            buttonName = CommonFun.Name(_ViveCosmosAxis2D);
                            break;
                        }
                }
                return CommonFun.Name(_ViveDeviceType) + ":" + buttonName;
            }
        }
    }

    /// <summary>
    /// Vive手柄
    /// </summary>
    [Serializable]
    [Name("Vive手柄")]
    public class ViveController : BaseViveController
    {
        /// <summary>
        /// Vive Focus轴与按钮
        /// </summary>
        [Name("Vive Focus轴与按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _ViveFocusAxisAndButton = EViveFocusAxisAndButton.Trigger;

        /// <summary>
        /// Vive Focus 2D轴
        /// </summary>
        [Name("Vive Focus 2D轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxis2D _ViveFocusAxis2D = EViveFocusAxis2D.ThumbStick;

        /// <summary>
        /// Vive Pro轴与按钮
        /// </summary>
        [Name("Vive Pro轴与按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _ViveProAxisAndButton = EViveProAxisAndButton.Trigger;

        /// <summary>
        /// 摇杆或触控板
        /// </summary>
        [Name("Vive Pro 2D轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxis2D _ViveProAxis2D = EViveProAxis2D.ThumbStick;

        /// <summary>
        /// Vive Cosmos轴与按钮
        /// </summary>
        [Name("Vive Cosmos轴与按钮")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _ViveCosmosAxisAndButton = EViveCosmosAxisAndButton.Trigger;

        /// <summary>
        /// Vive Cosmos 2D轴
        /// </summary>
        [Name("Vive Cosmos 2D轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxis2D _ViveCosmosAxis2D = EViveCosmosAxis2D.JoyStick;

        /// <summary>
        /// 死区：对于轴的值超过本死区值时，认为事件成立；
        /// </summary>
        [Name("死区")]
        [Tip("对于轴的值超过本死区值时，认为事件成立；", "When the value of the axis exceeds the dead band value, the event is considered to be true;")]
        [Range(-1, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// 按键按下
        /// </summary>
        /// <returns></returns>
        public bool IsPressed()
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusAxisAndButton.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.VivePro: return _ViveProAxisAndButton.IsPressed(_handleType, _deadZone);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosAxisAndButton.IsPressed(_handleType, _deadZone);
            }
            return false;
        }

        /// <summary>
        /// 获取2D轴输入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetAxis2DValue(out Vector2 value)
        {
            switch (_ViveDeviceType)
            {
                case EViveDeviceType.ViveFocus: return _ViveFocusAxis2D.TryGetAxis2DValue(_handleType, out value);
                case EViveDeviceType.VivePro: return _ViveProAxis2D.TryGetAxis2DValue(_handleType, out value);
                case EViveDeviceType.ViveCosmos: return _ViveCosmosAxis2D.TryGetAxis2DValue(_handleType, out value);
            }
            value = default;
            return false;
        }
    }
}
