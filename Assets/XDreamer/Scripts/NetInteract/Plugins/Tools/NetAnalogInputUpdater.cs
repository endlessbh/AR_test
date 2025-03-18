using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Base.Inputs;
using XCSJ.LitJson;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginXGUI.Views.Inputs;

namespace XCSJ.PluginNetInteract.Tools
{
    /// <summary>
    /// 网络模拟输入更新器
    /// </summary>
    [Name("网络模拟输入更新器")]
    [RequireManager(typeof(NetInteractManager))]
    [Owner(typeof(NetInteractManager))]
    [Tool(NetInteractHelper.Title, nameof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.JoyStick)]
    public class NetAnalogInputUpdater : BaseAnalogInputUpdater
    {
        /// <summary>
        /// 网络端
        /// </summary>
        [Name("网络端")]
        public NetEnd _netEnd = new NetEnd();

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _netEnd.FindNetEnds();
        }

        /// <summary>
        /// 仅用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            _netEnd.FindNetEnds();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        /// <summary>
        /// 更新轴
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public override void UpdateAxis(IInput input, string name, float value)
        {
            NetAnalogInput netAnalogInput = new NetAnalogInput() { input = input.input, name = name, value = value, inputUpdateType = NetAnalogInput.EInputUpdateType.Axis };
            _netEnd.Send(netAnalogInput);
        }

        /// <summary>
        /// 更新按钮
        /// </summary>
        /// <param name="input"></param>
        /// <param name="name"></param>
        /// <param name="downOrUp"></param>
        public override void UpdateButton(IInput input, string name, bool downOrUp)
        {
            NetAnalogInput netAnalogInput = new NetAnalogInput() { input = input.input, name = name, downOrUp = downOrUp, inputUpdateType = NetAnalogInput.EInputUpdateType.Button };
            _netEnd.Send(netAnalogInput);
        }
    }

    /// <summary>
    /// 网络模拟输入
    /// </summary>
    [Serializable]
    [Import]
    [Name("网络模拟输入")]
    public class NetAnalogInput : ISyncData
    {
        /// <summary>
        /// 输入
        /// </summary>
        public EInput input { get; set; } = default;

        /// <summary>
        /// 输入更新类型
        /// </summary>
        [Name("输入更新类型")]
        public enum EInputUpdateType
        {
            /// <summary>
            /// 无
            /// </summary>
            None,

            /// <summary>
            /// 轴
            /// </summary>
            Axis,

            /// <summary>
            /// 按钮
            /// </summary>
            Button,
        }

        /// <summary>
        /// 输入更新类型
        /// </summary>
        public EInputUpdateType inputUpdateType { get; set; } = EInputUpdateType.None;

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// 值
        /// </summary>
        public float value { get; set; } = 0;

        /// <summary>
        /// 按下或弹起
        /// </summary>
        public bool downOrUp { get; set; } = false;

        /// <summary>
        /// 处理
        /// </summary>
        public void Handle()
        {
            //Debug.LogFormat("{0}: {1}: {2}: {3}: {4}", input, inputUpdateType, name, value, downOrUp);
            switch (inputUpdateType)
            {
                case EInputUpdateType.Axis:
                    {
                        input.UpdateAxis(name, value);
                        break;
                    }
                case EInputUpdateType.Button:
                    {
                        input.UpdateButton(name, downOrUp);
                        break;
                    }
            }
        }

        /// <summary>
        /// 转网络模拟输入问题
        /// </summary>
        /// <returns></returns>
        public NetAnalogInputQuestion ToNetQuestion()
        {
            var question = new NetAnalogInputQuestion() { questionCode = EQuestionCode.Valid };
            question.netAnalogInput = this;
            return question;
        }

        /// <summary>
        /// 转网络模拟输入答案
        /// </summary>
        /// <returns></returns>
        public NetAnalogInputAnswer ToNetAnswer()
        {
            var answer = new NetAnalogInputAnswer() { answerCode = EAnswerCode.Valid };
            answer.netAnalogInput = this;
            return answer;
        }

        NetQuestion ISyncData.ToNetQuestion() => ToNetQuestion();

        NetAnswer ISyncData.ToNetAnswer() => ToNetAnswer();
    }

    /// <summary>
    /// 网络模拟输入问题
    /// </summary>
    [Import]
    public class NetAnalogInputQuestion : NetQuestion
    {
        /// <summary>
        /// 网络模拟输入
        /// </summary>
        public NetAnalogInput netAnalogInput { get; set; } = new NetAnalogInput();
    }

    /// <summary>
    /// 网路模拟输入答案
    /// </summary>
    [Import]
    public class NetAnalogInputAnswer : NetAnswer
    {
        /// <summary>
        /// 网络模拟输入
        /// </summary>
        public NetAnalogInput netAnalogInput { get; set; } = new NetAnalogInput();
    }
}
