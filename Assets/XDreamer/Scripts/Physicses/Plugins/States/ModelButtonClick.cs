using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginPhysicses.Tools.Gadgets;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPhysicses.States
{
    /// <summary>
    /// 模型按钮点击：监听模型按钮按下或弹起状态
    /// </summary>
    [ComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
    [Name(Title, nameof(ModelButtonClick))]
    [Tip("监听模型按钮按下或弹起状态", "Monitor Model button pressed or popped")]
    [XCSJ.Attributes.Icon(EIcon.Click)]
    [DisallowMultipleComponent]
    [Owner(typeof(PhysicsManager))]
    public class ModelButtonClick : Trigger<ModelButtonClick>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "模型按钮点击";

        /// <summary>
        /// 创建模型按钮点击
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(PhysicsManager.Title, typeof(PhysicsManager))]
        [StateComponentMenu(PhysicsManager.Title + "/" + Title, typeof(PhysicsManager))]
        [Name(Title, nameof(ModelButtonClick))]
        [Tip("监听模型按钮按下或弹起状态", "Monitor Model button pressed or popped")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 模型按钮
        /// </summary>
        [Name("模型按钮")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public ModelButton _modelButton;

        /// <summary>
        /// 点击类型
        /// </summary>
        [Name("点击类型")]
        [EnumPopup]
        public EClickType _clickType = EClickType.DownAndUp;

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

            ModelButton.onPressed += OnModelButtonPressed;

            pressedDown = false;
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);

            ModelButton.onPressed -= OnModelButtonPressed;
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity() => _modelButton;

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString() => (_modelButton ? _modelButton.name : "") + CommonFun.Name(_clickType);

        private bool pressedDown = false;

        private void OnModelButtonPressed(ModelButton physicsButton, bool isPressed)
        {
            if (_modelButton != physicsButton || finished) return;

            switch (_clickType)
            {
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
                case EClickType.DownAndUp:
                    {
                        if (isPressed)
                        {
                            pressedDown = true;
                        }
                        else
                        {
                            if (pressedDown)
                            {
                                pressedDown = false;
                                finished = true;
                            }
                        }
                        break;
                    }
            }
        }
    }
}
