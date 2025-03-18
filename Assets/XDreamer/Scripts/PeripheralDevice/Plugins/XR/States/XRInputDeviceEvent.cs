using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Caches;
using XCSJ.Collections;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Helper;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginPeripheralDevice.XR.States
{
    /// <summary>
    /// XR输入设备事件
    /// </summary>
    [Name(Title)]
    [Owner(typeof(PeripheralDeviceInputManager))]
    public class XRInputDeviceEvent : Trigger<XRInputDeviceEvent>, IDropdownPopupAttribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "XR输入设备事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Name(Title, nameof(XRInputDeviceEvent))]
#if UNITY_EDITOR && XDREAMER_EDITION_DEVELOPER
        [StateLib(PeripheralDeviceInputHelper.TitleXR, typeof(PeripheralDeviceInputManager))]
        [StateComponentMenu(PeripheralDeviceInputHelper.TitleXR + "/" + Title, typeof(PeripheralDeviceInputManager))]
#endif
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EEventType
        {
            [Name("无")]
            None,

            /// <summary>
            /// 设备
            /// </summary>
            [Name("设备")]
            [Tip("捕获输入设备的连接、断开、配置修改等事件", "Capture events such as connection, disconnection and configuration modification of input devices")]
            Device,

            /// <summary>
            /// 常用功能
            /// </summary>
            [Name("常用功能")]
            [Tip("捕获输入设备上各种物理轴与按钮等常用功能的事件", "Capture events of common functions such as various physical axes and buttons on the input device")]
            CommonUsages,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.CommonUsages;

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            InputDevices.deviceConnected += OnConnected;
            InputDevices.deviceDisconnected += OnDisconnected;
            InputDevices.deviceConfigChanged += OnConfigChanged;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            InputDevices.deviceConnected -= OnConnected;
            InputDevices.deviceDisconnected -= OnDisconnected;
            InputDevices.deviceConfigChanged -= OnConfigChanged;
        }

        /// <summary>
        /// 当更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            Handle();
        }

        #region 设备事件

        /// <summary>
        /// 输入设备特性
        /// </summary>
        [Name("输入设备特性")]
        [Tip("要求所有选择的特性同时满足时才能触发事件", "The event can only be triggered when all selected features are met at the same time")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.Device)]
        public InputDeviceCharacteristics _inputDeviceCharacteristics = InputDeviceCharacteristics.None;

        /// <summary>
        /// 设备事件类型
        /// </summary>
        [Name("设备事件类型")]
        public enum EDeviceEventType
        {
            /// <summary>
            /// 连接
            /// </summary>
            [Name("连接")]
            Connected,

            /// <summary>
            /// 断开
            /// </summary>
            [Name("断开")]
            Disconnected,

            /// <summary>
            /// 配置修改
            /// </summary>
            [Name("配置修改")]
            ConfigChanged,
        }

        /// <summary>
        /// 设备事件类型
        /// </summary>
        [Name("设备事件类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.Device)]
        public EDeviceEventType _deviceEventType = EDeviceEventType.Connected;

        private void OnConnected(InputDevice inputDevice)
        {
            if (finished || _eventType != EEventType.Device) return;
            if ((inputDevice.characteristics & _inputDeviceCharacteristics) != _inputDeviceCharacteristics) return;
            finished = _deviceEventType == EDeviceEventType.Connected;
        }

        private void OnDisconnected(InputDevice inputDevice)
        {
            if (finished || _eventType != EEventType.Device) return;
            if ((inputDevice.characteristics & _inputDeviceCharacteristics) != _inputDeviceCharacteristics) return;
            finished = _deviceEventType == EDeviceEventType.Disconnected;
        }

        private void OnConfigChanged(InputDevice inputDevice)
        {
            if (finished || _eventType != EEventType.Device) return;
            if ((inputDevice.characteristics & _inputDeviceCharacteristics) != _inputDeviceCharacteristics) return;
            finished = _deviceEventType == EDeviceEventType.ConfigChanged;
        }

        #endregion

        #region 常用功能

        /// <summary>
        /// XR节点
        /// </summary>
        [Name("XR节点")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.CommonUsages)]
        public XRNode _xrNode = XRNode.RightHand;

        /// <summary>
        /// 常用功能
        /// </summary>
        [Name("常用功能")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.CommonUsages)]
        [CustomEnumPopup]
        public string _commonUsages = nameof(CommonUsages.menuButton);

        private string _lastCommonUsages = "";

        private object _usage = null;

        /// <summary>
        /// 输入特征功能
        /// </summary>
        public object inputFeatureUsage
        {
            get
            {
                if (_lastCommonUsages != _commonUsages || _usage == null)
                {
                    _lastCommonUsages = _commonUsages;
                    _usage = GetInputFeatureUsage();
                }
                return _usage;
            }
        }

        private object GetInputFeatureUsage() => FieldInfoCache.Get(typeof(CommonUsages), _commonUsages)?.GetValue(null);

        /// <summary>
        /// 常用功能事件类型
        /// </summary>
        [Name("常用功能事件类型")]
        public enum ECommonUsagesEventType
        {
            /// <summary>
            /// 按钮
            /// </summary>
            [Name("按钮")]
            [Tip("对应常用功能的按钮或轴的值不属于【死区】指定范围内的值时，可认为时按钮点击；会将之值存储在【功能值变量字符串】对应的变量中；")]
            Button,

            /// <summary>
            /// 值
            /// </summary>
            [Name("值")]
            [Tip("仅获取对应常用功能的按钮或轴的值；会将之值存储在【功能值变量字符串】对应的变量中；")]
            Value,
        }

        /// <summary>
        /// 常用功能事件类型
        /// </summary>
        [Name("常用功能事件类型")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.CommonUsages)]
        public ECommonUsagesEventType _commonUsagesEventType = ECommonUsagesEventType.Button;

        /// <summary>
        /// 死区
        /// </summary>
        [Name("死区")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual | EValidityCheckType.Or, EEventType.CommonUsages, nameof(_commonUsagesEventType), EValidityCheckType.NotEqual, ECommonUsagesEventType.Button)]
        public DeadZone _deadZone = new DeadZone();

        /// <summary>
        /// 功能值变量字符串
        /// </summary>
        [Name("功能值变量字符串")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.CommonUsages)]
        public string _usageValueVarString = "";

        private void SetValue(object varValue) => _usageValueVarString.TrySetOrAddSetHierarchyVarValue(varValue);

        private void Handle()
        {
            if (finished || _eventType != EEventType.CommonUsages) return;
            var inputDevice = InputDevices.GetDeviceAtXRNode(_xrNode);
            if (!inputDevice.isValid || string.IsNullOrEmpty(_commonUsages)) return;

            var usage = this.inputFeatureUsage;
            if (usage == null) return;

            if (usage is InputFeatureUsage<bool> usageBool)
            {
                if (inputDevice.TryGetFeatureValue(usageBool, out var value))
                {                    
                    finished = _commonUsagesEventType == ECommonUsagesEventType.Value || !_deadZone.InDeadZone(value);
                    if (finished) SetValue(value);
                }
                return;
            }
            if (usage is InputFeatureUsage<float> usageFloat)
            {
                if (inputDevice.TryGetFeatureValue(usageFloat, out var value))
                {
                    finished = _commonUsagesEventType == ECommonUsagesEventType.Value || !_deadZone.InDeadZone(value);
                    if (finished) SetValue(value);
                }
                return;
            }
            if (usage is InputFeatureUsage<uint> usageUint)
            {
                if (inputDevice.TryGetFeatureValue(usageUint, out var value))
                {
                    finished = _commonUsagesEventType == ECommonUsagesEventType.Value || !_deadZone.InDeadZone(value);
                    if (finished) SetValue(value);
                }
                return;
            }
            if (usage is InputFeatureUsage<Vector2> usageVector2)
            {
                if (inputDevice.TryGetFeatureValue(usageVector2, out var value))
                {
                    finished = _commonUsagesEventType == ECommonUsagesEventType.Value || !_deadZone.InDeadZone(value);
                    if (finished) SetValue(value);
                }
                return;
            }
            if (usage is InputFeatureUsage<Vector3> usageVector3)
            {
                if (inputDevice.TryGetFeatureValue(usageVector3, out var value))
                {
                    finished = _commonUsagesEventType == ECommonUsagesEventType.Value || !_deadZone.InDeadZone(value);
                    if (finished) SetValue(value);
                }
                return;
            }
            if (usage is InputFeatureUsage<InputTrackingState> usageInputTrackingState)
            {
                if (inputDevice.TryGetFeatureValue(usageInputTrackingState, out var value))
                {
                    finished = _commonUsagesEventType == ECommonUsagesEventType.Value || !_deadZone.InDeadZone(value);
                    if (finished) SetValue(value);
                }
                return;
            }
        }

        bool IDropdownPopupAttribute.TryGetOptions(string purpose, string propertyPath, out string[] options)
        {
            var fieldInfos = FieldInfosCache.Get(typeof(CommonUsages), TypeHelper.StaticPublic);
            options = fieldInfos.Cast(fi => fi.Name).ToArray();
            return true;
        }

        bool IDropdownPopupAttribute.TryGetOption(string purpose, string propertyPath, string[] options, object propertyValue, out string option)
        {
            option = propertyValue as string;
            return true;
        }

        bool IDropdownPopupAttribute.TryGetPropertyValue(string purpose, string propertyPath, string[] options, string option, out object propertyValue)
        {
            propertyValue = option;
            return true;
        }

        #endregion

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _deadZone.Reset();
        }
    }

    /// <summary>
    /// 死区
    /// </summary>
    [Serializable]
    [Name("死区")]
    public class DeadZone
    {
        /// <summary>
        /// 布尔区间值
        /// </summary>
        [Name("布尔区间值")]
        public bool _boolRangeValue = false; 

        /// <summary>
        /// 在死区内
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InDeadZone(bool value) => value == _boolRangeValue;

        /// <summary>
        /// 浮点区间值
        /// </summary>
        [Name("浮点区间值")]
        public Vector2 _floatRangeValue = new Vector2(-0.5f, 0.5f);

        /// <summary>
        /// 在死区内
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InDeadZone(float value) => value >= _floatRangeValue.x && value <= _floatRangeValue.y;

        /// <summary>
        /// 无符号整数最小区间值
        /// </summary>
        [Name("无符号整数最小区间值")]
        public uint _uintMinRangeValue = 0;

        /// <summary>
        /// 无符号整数最大区间值
        /// </summary>
        [Name("无符号整数最大区间值")]
        public uint _uintMaxRangeValue = 0;

        /// <summary>
        /// 在死区内
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InDeadZone(uint value) => value >= _uintMinRangeValue && value <= _uintMaxRangeValue;

        /// <summary>
        /// 二维向量X区间值
        /// </summary>
        [Name("二维向量X区间值")]
        public Vector2 _v2xRangeValue = new Vector2(-0.5f, 0.5f);

        /// <summary>
        /// 二维向量Y区间值
        /// </summary>
        [Name("二维向量Y区间值")]
        public Vector2 _v2yRangeValue = new Vector2(-0.5f, 0.5f);

        /// <summary>
        /// 在死区内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allOrAny"></param>
        /// <returns></returns>
        public bool InDeadZone(Vector2 value, bool allOrAny = true)
        {
            if(allOrAny)
            {
                return value.x >= _v2xRangeValue.x && value.x <= _v2xRangeValue.y
                    && value.y >= _v2yRangeValue.x && value.y <= _v2yRangeValue.y;
            }
            else
            {
                return (value.x >= _v2xRangeValue.x && value.x <= _v2xRangeValue.y)
                    || (value.y >= _v2yRangeValue.x && value.y <= _v2yRangeValue.y);
            }
        }

        /// <summary>
        /// 三维向量X区间值
        /// </summary>
        [Name("三维向量X区间值")]
        public Vector2 _v3xRangeValue = new Vector2(-0.5f, 0.5f);

        /// <summary>
        /// 三维向量Y区间值
        /// </summary>
        [Name("三维向量Y区间值")]
        public Vector2 _v3yRangeValue = new Vector2(-0.5f, 0.5f);

        /// <summary>
        /// 三维向量Z区间值
        /// </summary>
        [Name("三维向量Z区间值")]
        public Vector2 _v3zRangeValue = new Vector2(-0.5f, 0.5f);

        /// <summary>
        /// 在死区内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="allOrAny"></param>
        /// <returns></returns>
        public bool InDeadZone(Vector3 value, bool allOrAny = true)
        {
            if (allOrAny)
            {
                return value.x >= _v3xRangeValue.x && value.x <= _v3xRangeValue.y
                    && value.y >= _v3yRangeValue.x && value.y <= _v3yRangeValue.y
                    && value.z >= _v3zRangeValue.x && value.z <= _v3zRangeValue.y;
            }
            else
            {
                return (value.x >= _v3xRangeValue.x && value.x <= _v3xRangeValue.y)
                    || (value.y >= _v3yRangeValue.x && value.y <= _v3yRangeValue.y)
                    || (value.z >= _v3zRangeValue.x && value.z <= _v3zRangeValue.y);
            }
        }

        /// <summary>
        /// 输入跟踪状态区间值列表
        /// </summary>
        [Name("输入跟踪状态区间值列表")]
        public List<InputTrackingState> _inputTrackingStateRangeValues = new List<InputTrackingState>();

        /// <summary>
        /// 在死区内
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool InDeadZone(InputTrackingState value) => _inputTrackingStateRangeValues.Contains(value);

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _boolRangeValue = false;

            _floatRangeValue = new Vector2(-0.5f, 0.5f);

            _uintMinRangeValue = 0;
            _uintMaxRangeValue = 0;

            _v2xRangeValue = new Vector2(-0.5f, 0.5f);
            _v2yRangeValue = new Vector2(-0.5f, 0.5f);

            _v3xRangeValue = new Vector2(-0.5f, 0.5f);
            _v3yRangeValue = new Vector2(-0.5f, 0.5f);
            _v3zRangeValue = new Vector2(-0.5f, 0.5f);

            _inputTrackingStateRangeValues.Clear();
            _inputTrackingStateRangeValues.Add(InputTrackingState.None);
        }
    }
}
