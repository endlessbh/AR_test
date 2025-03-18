using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginHTCVive.Base;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginHTCVive.States
{
    /// <summary>
    /// Vive手柄轴与按钮事件：用于监听并捕获Vive手柄的轴与按钮事件
    /// </summary>
    [ComponentMenu(HTCViveManager.Title + "/" + Title, typeof(HTCViveManager))]
    [Name(Title)]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    [Tip("用于监听并捕获HTC Vive手柄的轴与按钮事件")]
    [Owner(typeof(HTCViveManager))]
    public class EViveAxisAndButtonEvent : Trigger<EViveAxisAndButtonEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "HTC Vive手柄轴与按钮事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(EViveAxisAndButtonEvent))]
#if UNITY_EDITOR
        [StateLib(HTCViveManager.Title, typeof(HTCViveManager))]
        [StateComponentMenu(HTCViveManager.Title + "/" + Title, typeof(HTCViveManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// Vive手柄轴与按钮
        /// </summary>
        [Name("Vive手柄轴与按钮")]
        public ViveControllerAxisAndButton _viveControllerAxisAndButton = new ViveControllerAxisAndButton();

        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        [EnumPopup]
        public EClickType _clickType = EClickType.DownAndUp;

        private bool lastPressed = false;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            lastPressed = false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            if (finished) return;

            var isPressed = _viveControllerAxisAndButton.IsPressed();

            switch (_clickType)
            {
                case EClickType.DownAndUp:
                    {
                        finished = lastPressed && !isPressed;
                        lastPressed = isPressed;
                        break;
                    }
                case EClickType.Down:
                    {
                        finished = isPressed;
                        break;
                    }
                case EClickType.Up:
                    {
                        finished = !isPressed;
                        break;
                    }
            }
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => _viveControllerAxisAndButton.axisAndButtonFriendlyString;
    }
}
