using System;
using XCSJ.Attributes;
using XCSJ.LitJson;
using XCSJ.Net;
using XCSJ.Net.Tcp;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;

namespace XCSJ.PluginNetInteract.Base
{
    /// <summary>
    /// 网络答案
    /// </summary>
    public class NetAnswer : AnswerNetPackage, IDataValidity
    {
        /// <summary>
        /// 答案码
        /// </summary>
        public EAnswerCode answerCode { get; set; } = EAnswerCode.Valid;   

        /// <summary>
        /// 心跳答案
        /// </summary>
        [Json(false)]
        public static NetAnswer HeatBeatAnswer { get; } = new NetAnswer() { answerCode = EAnswerCode.HeartBeat };

        /// <summary>
        /// 无效且再次答案
        /// </summary>
        [Json(false)]
        public static NetAnswer InvalidAgainAnswer { get; } = new NetAnswer() { answerCode = EAnswerCode.InvalidAgain };

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity() => true;
    }

    /// <summary>
    /// 答案代码
    /// </summary>
    [Name("答案代码")]
    public enum EAnswerCode
    {
        /// <summary>
        /// 错误
        /// </summary>
        Error = -1,

        /// <summary>
        /// 无，默认值，无效答案
        /// </summary>
        None = 0,

        /// <summary>
        /// 心跳响应，无效答案，且表明此时服务器上无有效答案
        /// </summary>
        HeartBeat,

        /// <summary>
        /// 有效答案
        /// </summary>
        Valid,

        /// <summary>
        /// 有效答案，且需要客户端再次提出问题(即请求数据)
        /// </summary>
        ValidAgain,

        /// <summary>
        /// 无效答案，但需要客户端再次提出问题(即请求数据)
        /// </summary>
        InvalidAgain,
    }
}
