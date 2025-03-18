using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSamsungWMR.Base;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginSamsungWMR.States
{
    /// <summary>
    /// 三星手柄轴与按钮事件：用于监听并捕获三星手柄的轴与按钮事件
    /// </summary>
    [ComponentMenu(SamsungWMRManager.Title + "/" + Title, typeof(SamsungWMRManager))]
    [Name(Title)]
    [Tip("用于监听并捕获三星手柄的轴与按钮事件")]
    [Owner(typeof(SamsungWMRManager))]
    public class ESamsungWMRAxisAndButtonEvent : Trigger<ESamsungWMRAxisAndButtonEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "三星手柄轴与按钮事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(ESamsungWMRAxisAndButtonEvent))]
#if UNITY_EDITOR
        [StateLib(SamsungWMRManager.Title, typeof(SamsungWMRManager))]
        [StateComponentMenu(SamsungWMRManager.Title + "/" + Title, typeof(SamsungWMRManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 手柄类型
        /// </summary>
        [Name("手柄类型")]
        [EnumPopup]
        public EHandRule _handleType = EHandRule.Left;

        /// <summary>
        /// 轴与按钮
        /// </summary>
        [Name("轴与按钮")]
        [EnumPopup]
        public ESamsungWMRAxisAndButton _axisAndButton = ESamsungWMRAxisAndButton.Trigger;

        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        [EnumPopup]
        public EClickType _clickType = EClickType.DownAndUp;

        /// <summary>
        /// 死区：对于轴的值超过本死区值时，认为事件成立；
        /// </summary>
        [Name("死区")]
        [Tip("对于轴的值超过本死区值时，认为事件成立；")]
        [Range(-1, 1)]
        public float _deadZone = 0.5f;

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

            var isPressed = _axisAndButton.IsPressed(_handleType, _deadZone);

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
        public override string ToFriendlyString()
        {
            return CommonFun.Name(_handleType) + ":" + CommonFun.Name(_axisAndButton);
        }
    }
}
