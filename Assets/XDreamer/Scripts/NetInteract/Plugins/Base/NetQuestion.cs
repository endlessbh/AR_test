using XCSJ.Attributes;
using XCSJ.LitJson;
using XCSJ.Net;
using XCSJ.PluginCommonUtils.ComponentModel;

namespace XCSJ.PluginNetInteract.Base
{
    /// <summary>
    /// 网络问题
    /// </summary>
    public class NetQuestion : QuestionNetPackage, IDataValidity
    {
        /// <summary>
        /// 用于在服务器端标识当前问题的所属客户端
        /// </summary>
        [Json(false)]
        public ClientVirtual client { get; set; }

        /// <summary>
        /// 问题码
        /// </summary>
        public EQuestionCode questionCode { get; set; } = EQuestionCode.Valid;

        /// <summary>
        /// 心跳问题
        /// </summary>
        [Json(false)]
        public static NetQuestion HeartBeatQuestion { get; } = new NetQuestion() { questionCode = EQuestionCode.HeartBeat };

        /// <summary>
        /// 转网络答案
        /// </summary>
        /// <returns></returns>
        public virtual NetAnswer ToNetAnswer() => new NetAnswer() { answerCode = EAnswerCode.Valid };

        /// <summary>
        /// 隐式转换为网络答案
        /// </summary>
        /// <param name="netMsg"></param>
        public static implicit operator NetAnswer(NetQuestion netQuestion) => netQuestion?.ToNetAnswer();

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public bool DataValidity() => true;
    }

    /// <summary>
    /// 问题代码
    /// </summary>
    [Name("问题代码")]
    public enum EQuestionCode
    {
        /// <summary>
        /// 无，默认值，无效问题
        /// </summary>
        None = 0,

        /// <summary>
        /// 心跳
        /// </summary>
        HeartBeat,

        /// <summary>
        /// 有效问题
        /// </summary>
        Valid,
    }
}
