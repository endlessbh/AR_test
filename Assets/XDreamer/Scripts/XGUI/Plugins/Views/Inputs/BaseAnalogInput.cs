using System;
using System.Collections.Generic;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;

namespace XCSJ.PluginXGUI.Views.Inputs
{
    /// <summary>
    /// 基础模拟输入:模拟输入按钮的状态或轴的值
    /// </summary>
    [Name("基础模拟输入")]
    [Tip("模拟输入按钮的状态或轴的值", "Analog input button status or axis value")]
    [RequireManager(typeof(XGUIManager), typeof(ToolsManager))]
    [Owner(typeof(XGUIManager))]
    public abstract class BaseAnalogInput: InteractProvider
    {
        /// <summary>
        /// 更新规则
        /// </summary>
        [Name("更新规则")]
        [Flags]
        public enum EUpdateRule
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Name("默认")]
            [EnumFieldName("默认")]
            Default = 1 << 0,

            /// <summary>
            /// 更新器
            /// </summary>
            [Name("更新器")]
            [EnumFieldName("更新器")]
            Updater = 1 << 1,

            /// <summary>
            /// 更新器列表
            /// </summary>
            [Name("更新器列表")]
            [EnumFieldName("更新器列表")]
            UpdaterList = 1 << 2,
        }

        /// <summary>
        /// 更新规则
        /// </summary>
        [Name("更新规则")]
        [EnumPopup]
        public EUpdateRule _updateRule = EUpdateRule.Default;

        /// <summary>
        /// 模拟输入更新器
        /// </summary>
        [Name("模拟输入更新器")]
        public MB _analogInputUpdater;

        /// <summary>
        /// 模拟输入更新器
        /// </summary>
        public MB analogInputUpdater
        {
            get
            {
                if (_analogInputUpdater)
                {
                    if (_analogInputUpdater is IAnalogInputUpdater)
                    {
                        return _analogInputUpdater;
                    }
                    _analogInputUpdater = _analogInputUpdater.GetComponent<IAnalogInputUpdater>() as MB;
                }
                if (!_analogInputUpdater)
                {
                    _analogInputUpdater = GetComponentInParent<IAnalogInputUpdater>() as MB;
                }
                return _analogInputUpdater;
            }
            set
            {
                if (value)
                {
                    _analogInputUpdater = (value is IAnalogInputUpdater ? value : value.GetComponent<IAnalogInputUpdater>() as MB);
                }
            }
        }

        /// <summary>
        /// 模拟输入更新器列表
        /// </summary>
        [Name("模拟输入更新器列表")]
        public List<MB> _analogInputUpdaters = new List<MB>();

        /// <summary>
        /// 输入
        /// </summary>
        [Name("输入")]
        [EnumPopup]
        public EInput _input = EInput.VirtualInput;

        /// <summary>
        /// 输入
        /// </summary>
        public IInput input => _input.GetInput();

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void UpdateAxis(string name, float value) => UpdateAxis(input, name, value);

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void UpdateAxis(IInput input, string name, float value)
        {
            if (input == null) return;
            if ((_updateRule & EUpdateRule.Default) != 0)
            {
                input.UpdateAxis(name, value);
            }
            if ((_updateRule & EUpdateRule.Updater) != 0 && analogInputUpdater && analogInputUpdater is IAnalogInputUpdater updater)
            {
                updater.UpdateAxis(input, name, value);
            }
            if ((_updateRule & EUpdateRule.UpdaterList) != 0)
            {
                foreach (var u in _analogInputUpdaters)
                {
                    if (u && u is IAnalogInputUpdater updater1) updater1.UpdateAxis(input, name, value);
                }
            }
        }

        public void UpdateButton(string name, bool downOrUp) => UpdateButton(input, name, downOrUp);

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public void UpdateButton(IInput input, string name, bool downOrUp)
        {
            if (input == null) return;
            if ((_updateRule & EUpdateRule.Default) != 0)
            {
                input.UpdateButton(name, downOrUp);
            }
            if ((_updateRule & EUpdateRule.Updater) != 0 && analogInputUpdater && analogInputUpdater is IAnalogInputUpdater updater)
            {
                updater.UpdateButton(input, name, downOrUp);
            }
            if ((_updateRule & EUpdateRule.UpdaterList) != 0)
            {
                foreach (var u in _analogInputUpdaters)
                {
                    if (u && u is IAnalogInputUpdater updater1) updater1.UpdateButton(input, name, downOrUp);
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            if (analogInputUpdater)
            {
                _analogInputUpdaters.Add(analogInputUpdater);
            }
        }
    }
}
