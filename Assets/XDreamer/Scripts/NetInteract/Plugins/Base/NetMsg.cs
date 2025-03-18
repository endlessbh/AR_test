using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;

namespace XCSJ.PluginNetInteract.Base
{
    /// <summary>
    /// 网络消息类型
    /// </summary>
    [Name("网络消息类型")]
    public enum ENetMsgType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 消息<see cref="NetMsg"/>
        /// </summary>
        [Name("消息")]
        Msg,

        /// <summary>
        /// 中文脚本<see cref="CNScripts.NetCNScript"/>
        /// </summary>
        [Name("中文脚本")]
        CNScript,
    }

    /// <summary>
    /// 网络消息
    /// </summary>
    [Serializable]
    [Name("网络消息")]
    public class NetMsg : IDataValidity
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        [Name("客户端标识")]
        [OnlyMemberElements]
        public StringPropertyValue _clientID = new StringPropertyValue();

        /// <summary>
        /// 客户端标识
        /// </summary>
        public string clientID => _clientID.GetValue("");

        /// <summary>
        /// 命令
        /// </summary>
        [Name("命令")]
        [OnlyMemberElements]
        public StringPropertyValue _cmd = new StringPropertyValue();

        /// <summary>
        /// 命令
        /// </summary>
        public string cmd => _cmd.GetValue("");

        /// <summary>
        /// 数据
        /// </summary>
        [Name("数据")]
        [OnlyMemberElements]
        public StringPropertyValue _data = new StringPropertyValue();

        /// <summary>
        /// 数据
        /// </summary>
        public string data => _data.GetValue("");

        /// <summary>
        /// 转网络问题
        /// </summary>
        /// <returns></returns>
        public NetMsgQuestion ToNetQuestion() => new NetMsgQuestion() { clientID = clientID, cmd = cmd, data = data, questionCode = EQuestionCode.Valid };       

        /// <summary>
        /// 隐式转换为网络问题
        /// </summary>
        /// <param name="netMsg"></param>
        public static implicit operator NetMsgQuestion(NetMsg netMsg) => netMsg?.ToNetQuestion();

        /// <summary>
        /// 转网络答案
        /// </summary>
        /// <returns></returns>
        public NetMsgAnswer ToNetAnswer() => new NetMsgAnswer() { clientID = clientID, cmd = cmd, data = data, answerCode = EAnswerCode.Valid };

        /// <summary>
        /// 隐式转换为网络答案
        /// </summary>
        /// <param name="netMsg"></param>
        public static implicit operator NetMsgAnswer(NetMsg netMsg) => netMsg?.ToNetAnswer();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity() => _clientID.DataValidity() && _cmd.DataValidity() && _data.DataValidity();

        /// <summary>
        /// 提示字符串
        /// </summary>
        /// <param name="netMsg"></param>
        /// <returns></returns>
        public string GetTip()
        {
            string tip = "";
            if (!string.IsNullOrEmpty(cmd))
            {
                tip = CommonFun.Name(typeof(NetMsg), nameof(_cmd)) + ":" + cmd;
            }
            if (!string.IsNullOrEmpty(data))
            {
                if (!string.IsNullOrEmpty(tip))
                {
                    tip += " ";// 空白分割两个字符串
                }
                tip += CommonFun.Name(typeof(NetMsg), nameof(_data)) + ":" + data;
            }
            return tip;
        }
    }

    /// <summary>
    /// 消息检测规则
    /// </summary>
    [Name("消息检测规则")]
    public enum EMsgCheckRule
    {
        /// <summary>
        /// 任意
        /// </summary>
        [Name("任意")]
        [Tip("不论收到的消息为何种类型，均标识组件完成", "Regardless of the type of message received, it identifies the completion of the component")]
        Any,

        /// <summary>
        /// 无:不做任何操作
        /// </summary>
        [Name("无")]
        [Tip("不做任何操作", "Do nothing")]
        None,

        /// <summary>
        /// 检查命令
        /// </summary>
        [Name("检查命令")]
        CheckCmd,

        /// <summary>
        /// 检查数据
        /// </summary>
        [Name("检查数据")]
        CheckData,

        /// <summary>
        /// 检查命令与数据
        /// </summary>
        [Name("检查命令与数据")]
        CheckCmdAndData,

        /// <summary>
        /// 检查命令或数据
        /// </summary>
        [Name("检查命令或数据")]
        CheckCmdOrData,
    }

    /// <summary>
    /// 日志输出规则
    /// </summary>
    [Name("日志输出规则")]
    public enum ELogOutputRule
    {
        [Name("无")]
        [Tip("不做任何操作", "Do nothing")]
        None,

        [Name("总是输出")]
        [Tip("无论收到何种消息，均将消息输出；不论组件是否已经处于完成态，均会输出；", "No matter what message is received, the message is output; Whether the component is in the completed state or not, it will be output;")]
        OutputAlways,

        [Name("输出")]
        [Tip("无论收到何种消息，均将消息输出；如果组件已经处于完成态，则不输出；", "No matter what message is received, the message is output; If the component is already in the completed state, it will not be output;")]
        Output,

        /// <summary>
        /// 如果检查有效则输出:根据检查规则检查后消息有效，则将消息输出；如果组件已经处于完成态，则不输出；
        /// </summary>
        [Name("如果检查有效则输出")]
        [Tip("根据检查规则检查后消息有效，则将消息输出；如果组件已经处于完成态，则不输出；", "If the message is valid after checking according to the inspection rules, the message will be output; If the component is already in the completed state, it will not be output;")]
        OutputIfCheckValid,

        /// <summary>
        /// 如果检查无效则输出:根据检查规则检查后消息输出，则将消息输出；如果组件已经处于完成态，则不输出；
        /// </summary>
        [Name("如果检查无效则输出")]
        [Tip("根据检查规则检查后消息无效，则将消息输出；如果组件已经处于完成态，则不输出；", "If the message is invalid after checking according to the inspection rules, the message will be output; If the component is already in the completed state, it will not be output;")]
        OutputIfCheckInvalid,
    }

    /// <summary>
    /// 网络消息问题
    /// </summary>
    public class NetMsgQuestion : NetQuestion
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        public string clientID { get; set; } = "";

        /// <summary>
        /// 命令
        /// </summary>
        public string cmd { get; set; } = "";

        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; } = "";

        /// <summary>
        /// 转网络答案
        /// </summary>
        /// <returns></returns>
        public override NetAnswer ToNetAnswer() => new NetMsgAnswer() { clientID = clientID, cmd = cmd, data = data, answerCode = EAnswerCode.Valid };
    }

    /// <summary>
    /// 网络消息答案
    /// </summary>
    public class NetMsgAnswer : NetAnswer
    {
        /// <summary>
        /// 客户端标识
        /// </summary>
        public string clientID { get; set; } = "";

        /// <summary>
        /// 命令
        /// </summary>
        public string cmd { get; set; } = "";

        /// <summary>
        /// 数据
        /// </summary>
        public string data { get; set; } = "";

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="question"></param>
        public void Set(NetMsgQuestion question)
        {
            base.Set(question);
            this.clientID = question.clientID;
            this.cmd = question.cmd;
            this.data = question.data;
        }
    }
}
