using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginXBox.Base;

namespace XCSJ.PluginXBox.States
{
    /// <summary>
    /// XBox轴与按钮事件：用于监听并捕获输入设备XBox的轴与按钮事件
    /// </summary>
    [ComponentMenu(XBoxHelper.Title + "/" + Title, typeof(XBoxManager))]
    [Name(Title)]
    [Tip("用于监听并捕获输入设备XBox的轴与按钮事件", "It is used to monitor and capture the axis and button events of the input device Xbox")]
    [Owner(typeof(XBoxManager))]
    public class XBoxAxisAndButtonEvent : Trigger<XBoxAxisAndButtonEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XBox轴与按钮事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(XBoxAxisAndButtonEvent))]
#if UNITY_EDITOR
        [StateLib(XBoxHelper.Title, typeof(XBoxManager))]
        [StateComponentMenu(XBoxHelper.Title + "/" + Title, typeof(XBoxManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 轴与按钮
        /// </summary>
        [Name("轴与按钮")]
        [EnumPopup]
        public EXBoxAxisAndButton _axisAndButton = EXBoxAxisAndButton.None;

        /// <summary>
        /// 死区：对于轴的值超过本死区值时，认为事件成立；
        /// </summary>
        [Name("死区")]
        [Tip("对于轴的值超过本死区值时，认为事件成立；", "When the value of the axis exceeds the dead band value, the event is considered to be true;")]
        [Range(-1, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            if (finished) return;
            finished = _axisAndButton.IsPressed(_deadZone);
        }

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            //return base.ToFriendlyString();
            return CommonFun.Name(_axisAndButton);
        }
    }
}
