using System;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.DataBinders;
using XCSJ.Extension.Base.Dataflows.Models;

namespace XCSJ.Extension.Base.Dataflows.Binders.XUnityEngine.XUI.XButton
{
    /// <summary>
    /// 按钮点击绑定器 ：在当前绑定模式下，源属性为命令，点击按钮会向源对象发送命令
    /// </summary>
    [Name("按钮点击")]
    [DataBinder(typeof(Button), nameof(Button.onClick))]
    public class Button_onClick_Binder : TypeMemberDataBinder<Button>
    {
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="onReceiveEvent"></param>
        public override void Bind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Bind(onReceiveEvent);

            target.onClick.AddListener(OnClick);
        }

        /// <summary>
        /// 解绑
        /// </summary>
        public override void Unbind(Action<XValueEventArg> onReceiveEvent)
        {
            base.Unbind(onReceiveEvent);

            target.onClick.RemoveListener(OnClick);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="eventArg"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGet(XValueEventArg eventArg, out object value)
        {
            value = default;
            return false;
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="linkedBindData"></param>
        /// <param name="eventArg"></param>
        public override void Set(ITypeMemberDataBinder linkedBindData, XValueEventArg eventArg) { }

        /// <summary>
        /// 点击触发
        /// </summary>
        private void OnClick() => SendEvent(new XValueEventArg(this));

        /// <summary>
        /// 初始化值：无
        /// </summary>
        public override void InitValue() { }
    }
}
