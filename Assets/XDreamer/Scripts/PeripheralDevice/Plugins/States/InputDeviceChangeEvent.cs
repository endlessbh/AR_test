using XCSJ.Attributes;
using XCSJ.Extension.Base.InputSystems;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.Extension.Base.Attributes;

#if XDREAMER_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace XCSJ.PluginPeripheralDevice.States
{
    /// <summary>
    /// 输入设备变更事件
    /// </summary>
    [Name(Title)]
    [Owner(typeof(PeripheralDeviceInputManager))]
    public class InputDeviceChangeEvent : Trigger<InputDeviceChangeEvent>, IDropdownPopupAttribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入设备变更事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(InputDeviceChangeEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(PeripheralDeviceInputHelper.Title, typeof(PeripheralDeviceInputManager))]
        [StateComponentMenu(PeripheralDeviceInputHelper.Title + "/" + Title, typeof(PeripheralDeviceInputManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 输入设备
        /// </summary>
        [Name("输入设备")]
        [InputDeviceTypePopup]
        public string _inputDevice = InputSystemHelper.DisplayInputDeviceTypeFullName;

#if XDREAMER_INPUT_SYSTEM

        /// <summary>
        /// 输入设备变更
        /// </summary>
        [Name("输入设备变更")]
        public InputDeviceChange _inputDeviceChange = InputDeviceChange.Added;

#else
        /// <summary>
        /// 输入设备变更
        /// </summary>
        [Name("输入设备变更")]
        public int _inputDeviceChange = 0;

#endif

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);

#if XDREAMER_INPUT_SYSTEM
            InputSystem.onDeviceChange += OnDeviceChange;
#endif
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
#if XDREAMER_INPUT_SYSTEM
            InputSystem.onDeviceChange -= OnDeviceChange;
#endif
        }


#if XDREAMER_INPUT_SYSTEM
        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange inputDeviceChange)
        {
            if (finished) return;
            if (inputDeviceChange != _inputDeviceChange) return;
            var type = InputSystemHelper.GetInputDeviceType(_inputDevice);
            if (type == null) return;
            finished = type.IsAssignableFrom(inputDevice.GetType());
        }
#endif

        /// <summary>
        /// 输出友好字符串
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
#if XDREAMER_INPUT_SYSTEM
            return (InputSystemHelper.GetInputDeviceType(_inputDevice)?.Name ?? "") + "." + _inputDeviceChange;
#else
            return "未启用输入系统!";
#endif
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
#if XDREAMER_INPUT_SYSTEM
            return base.DataValidity();
#else
            return false;
#endif
        }

        /// <summary>
        /// 尝试获取选项数组
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="propertyPath"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            options = InputSystemHelper.displayInputDeviceTypeFullNames;
            return true;
        }

        /// <summary>
        /// 尝试获取选项
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="propertyPath"></param>
        /// <param name="options"></param>
        /// <param name="propertyValue"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            option = propertyValue as string;
            return !string.IsNullOrEmpty(option);
        }

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="propertyPath"></param>
        /// <param name="options"></param>
        /// <param name="option"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            propertyValue = option;
            return !string.IsNullOrEmpty(option);
        }
    }
}
