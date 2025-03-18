using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginSMS.States.Inputs
{
    /// <summary>
    /// 输入事件:用于捕获输入对应期望类型的事件
    /// </summary>
    [ComponentMenu("输入/" + Title, typeof(SMSManager))]
    [Name(Title, nameof(InputEvent))]
    [Tip("用于捕获输入对应期望类型的事件", "Used to capture events of the expected type corresponding to the input")]
    [XCSJ.Attributes.Icon(EIcon.Event)]
    public class InputEvent : Trigger<InputEvent>
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "输入事件";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("输入", typeof(SMSManager))]
        [StateComponentMenu("输入/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(InputEvent))]
        [Tip("用于捕获输入对应期望类型的事件", "Used to capture events of the expected type corresponding to the input")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 输入
        /// </summary>
        [Name("输入")]
        [EnumPopup]
        public EInput _input = EInput.StandaloneInput;

        /// <summary>
        /// 事件类型
        /// </summary>
        public enum EEventType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 无输入
            /// </summary>
            [Name("无输入")]
            NoInput,

            /// <summary>
            /// 任意按键
            /// </summary>
            [Name("任意按键")]
            AnyKey,

            /// <summary>
            /// 任意按键按下
            /// </summary>
            [Name("任意按键按下")]
            AnyKeyDown,
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        [Name("事件类型")]
        [EnumPopup]
        public EEventType _eventType = EEventType.NoInput;

        /// <summary>
        /// 无输入数据
        /// </summary>
        [Serializable]
        [Name("无输入数据")]
        public class NoInputData
        {
            /// <summary>
            /// 等待时间:无输入操作成立等待的时长；单位：秒；
            /// </summary>
            [Name("等待时间")]
            [Tip("无输入操作成立等待的时长；单位：秒；", "The waiting time for the establishment of no input operation; Unit: second;")]
            [Range(0, 600)]
            public float _waitTime = 3;

            /// <summary>
            /// 鼠标按键
            /// </summary>
            [Name("鼠标按键")]
            [EnumPopup]
            public EMouseButton _mouseButton = EMouseButton.Left;

            internal float waitedTime = 0;
        }

        /// <summary>
        /// 无输入数据
        /// </summary>
        [Name("无输入数据")]
        [HideInSuperInspector(nameof(_eventType), EValidityCheckType.NotEqual, EEventType.NoInput)]
        [OnlyMemberElements]
        public NoInputData _noInputData = new NoInputData();

        /// <summary>
        /// 进度
        /// </summary>
        public override double progress
        {
            get
            {
                switch (_eventType)
                {
                    case EEventType.NoInput: return _noInputData.waitedTime / _noInputData._waitTime;
                    default: return base.progress;
                }
            }
            set => base.progress = value;
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            _noInputData.waitedTime = 0;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnUpdate(StateData stateData)
        {
            base.OnUpdate(stateData);
            if (finished) return;
            switch (_eventType)
            {
                case EEventType.NoInput:
                    {
                        var input = _input.GetInput();
                        if (_noInputData._mouseButton.GetMouseButtonDown(input) || _noInputData._mouseButton.GetMouseButton(input))
                        {
                            _noInputData.waitedTime = 0;
                        }
                        else if (_noInputData.waitedTime >= _noInputData._waitTime)
                        {
                            finished = true;
                        }
                        else
                        {
                            _noInputData.waitedTime += Time.deltaTime;
                        }
                        break;
                    }
                case EEventType.AnyKey:
                    {
                        finished = _input.GetInput().anyKey;
                        break;
                    }
                case EEventType.AnyKeyDown:
                    {
                        finished = _input.GetInput().anyKeyDown;
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
            switch (_eventType)
            {
                case EEventType.NoInput: return CommonFun.Name(_eventType) + _noInputData._waitTime + "秒";
                case EEventType.AnyKey: return CommonFun.Name(_eventType);
                case EEventType.AnyKeyDown: return CommonFun.Name(_eventType);
                default: return base.ToFriendlyString();
            }
        }
    }
}
