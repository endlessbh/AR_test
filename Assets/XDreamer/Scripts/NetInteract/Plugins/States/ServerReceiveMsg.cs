using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginNetInteract.Tools;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 服务器收到消息:服务器收到客户端发送的消息
    /// </summary>
    [Name(Title)]
    [Tip("服务器收到客户端发送的消息", "The server received a message from the client")]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Import)]
    [Owner(typeof(NetInteractManager))]
    public class ServerReceiveMsg : Trigger<ServerReceiveMsg>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "服务器收到消息";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Server, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ServerReceiveMsg))]
        [Tip("服务器收到客户端发送的消息", "The server received a message from the client")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 服务器
        /// </summary>
        [Name("服务器")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Server _server;

        /// <summary>
        /// 服务器
        /// </summary>
        public Server server => this.XGetComponentInGlobal(ref _server);

        /// <summary>
        /// 消息检测规则
        /// </summary>
        [Name("消息检测规则")]
        [EnumPopup]
        public EMsgCheckRule _msgCheckRule = EMsgCheckRule.Any;

        /// <summary>
        /// 日志输出规则
        /// </summary>
        [Name("日志输出规则")]
        [EnumPopup]
        public ELogOutputRule _logOutputRule = ELogOutputRule.None;

        /// <summary>
        /// 消息处理规则
        /// </summary>
        [Name("消息处理规则")]
        [EnumPopup]
        public EMsgHandleRule _msgHandleRule = EMsgHandleRule.None;

        /// <summary>
        /// 消息处理规则
        /// </summary>
        [Name("消息处理规则")]
        public enum EMsgHandleRule
        {
            /// <summary>
            /// 无:不做任何操作
            /// </summary>
            [Name("无")]
            [Tip("不做任何操作", "Do nothing")]
            None,

            /// <summary>
            /// 总是广播:无论收到何种消息，均将消息广播；不论组件是否已经处于完成态，均会广播；
            /// </summary>
            [Name("总是广播")]
            [Tip("无论收到何种消息，均将消息广播；不论组件是否已经处于完成态，均会广播；", "Broadcast the message no matter what message is received; Broadcast whether the component is in the completed state or not;")]
            BroadcastAlways,

            /// <summary>
            /// 广播:无论收到何种消息，均将消息广播；如果组件已经处于完成态，则不广播；
            /// </summary>
            [Name("广播")]
            [Tip("无论收到何种消息，均将消息广播；如果组件已经处于完成态，则不广播；", "Broadcast the message no matter what message is received; If the component is already in the completion state, it will not be broadcast;")]
            Broadcast,

            /// <summary>
            /// 如果检查有效则广播:根据检查规则检查后消息有效，则将消息广播；如果组件已经处于完成态，则不广播；
            /// </summary>
            [Name("如果检查有效则广播")]
            [Tip("根据检查规则检查后消息有效，则将消息广播；如果组件已经处于完成态，则不广播；", "Broadcast the message if the message is valid after checking according to the inspection rules; If the component is already in the completion state, it will not be broadcast;")]
            BroadcastIfCheckValid,

            /// <summary>
            /// 如果检查无效则广播:根据检查规则检查后消息无效，则将消息广播；如果组件已经处于完成态，则不广播；
            /// </summary>
            [Name("如果检查无效则广播")]
            [Tip("根据检查规则检查后消息无效，则将消息广播；如果组件已经处于完成态，则不广播；", "Broadcast the message if the message is invalid after checking according to the inspection rules; If the component is already in the completion state, it will not be broadcast;")]
            BroadcastIfCheckInvalid,
        }

        /// <summary>
        /// 待比较命令值
        /// </summary>
        [Name("待比较命令值")]
        [HideInSuperInspector(nameof(_msgCheckRule), EValidityCheckType.NotEqual, EMsgCheckRule.CheckCmd)]
        public ComparableString _CheckCmd = new ComparableString();

        /// <summary>
        /// 待比较数据值
        /// </summary>
        [Name("待比较数据值")]
        [HideInSuperInspector(nameof(_msgCheckRule), EValidityCheckType.NotEqual, EMsgCheckRule.CheckData)]
        public ComparableString _CheckData = new ComparableString();

        /// <summary>
        /// 待比较值
        /// </summary>
        [Serializable]
        [Name("待比较值")]
        public class CompareValue
        {
            /// <summary>
            /// 待比较命令值
            /// </summary>
            [Name("待比较命令值")]
            public ComparableString _compareCmdValue = new ComparableString();

            /// <summary>
            /// 待比较数据值
            /// </summary>
            [Name("待比较数据值")]
            public ComparableString _compareDataValue = new ComparableString();
        }

        /// <summary>
        /// 检查命令与数据的值对象
        /// </summary>
        [HideInSuperInspector(nameof(_msgCheckRule), EValidityCheckType.NotEqual, EMsgCheckRule.CheckCmdAndData)]
        [OnlyMemberElements]
        public CompareValue _CheckCmdAndData = new CompareValue();

        /// <summary>
        /// 检查命令与数据的值对象
        /// </summary>
        [HideInSuperInspector(nameof(_msgCheckRule), EValidityCheckType.NotEqual, EMsgCheckRule.CheckCmdOrData)]
        [OnlyMemberElements]
        public CompareValue _CheckCmdOrData = new CompareValue();

        /// <summary>
        /// 客户端唯一标识变量:用于将消息中客户端唯一标识项存储的变量;客户端唯一标识由服务器端产生;
        /// </summary>
        [Name("客户端唯一标识变量")]
        [Tip("用于将消息中客户端唯一标识项存储的变量;客户端唯一标识由服务器端产生;", "A variable used to store the unique identification item of the client in the message; The unique identification of the client is generated by the server;")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _clientGuidVariable = "";

        /// <summary>
        /// 客户端标识变量:用于将消息中客户端标识项存储的变量
        /// </summary>
        [Name("客户端标识变量")]
        [Tip("用于将消息中客户端标识项存储的变量", "The variable used to store the client identification item in the message")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _clientIDVariable = "";

        /// <summary>
        /// 命令变量:用于将消息中命令项存储的变量
        /// </summary>
        [Name("命令变量")]
        [Tip("用于将消息中命令项存储的变量", "The variable used to store the command item in the message")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _cmdVariable = "";

        /// <summary>
        /// 数据变量:用于将消息中数据项存储的变量
        /// </summary>
        [Name("数据变量")]
        [Tip("用于将消息中数据项存储的变量", "Variables used to store data items in messages")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _dataVariable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _clientGuidVariable);
            CommonFun.VarNameToVarString(ref _clientIDVariable);
            CommonFun.VarNameToVarString(ref _cmdVariable);
            CommonFun.VarNameToVarString(ref _dataVariable);
        }

        #endregion

        /// <summary>
        /// 当进入
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            Server.onReceived += OnReceived;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            Server.onReceived -= OnReceived;
            base.OnExit(stateData);
        }

        private void Output(NetMsgQuestion netQuestion)
        {
            var log = string.Format(@"服务器收到问题消息:{0}
客户端唯一标识:{1}
客户端标识:{2}
命令:{3}
数据:{4}",
                this.GetNamePath(),
                netQuestion.client.guid,
                netQuestion.clientID,
                netQuestion.cmd,
                netQuestion.data);

            //Debug.Log(log);
            Log.Debug(log);
        }

        /// <summary>
        /// 当接收到网络答案时回调
        /// </summary>
        /// <param name="server"></param>
        /// <param name="netQuestion"></param>
        private void OnReceived(Server server, NetQuestion netQuestion)
        {
            if (server != this.server) return;
            var netMsgQuestion = netQuestion as NetMsgQuestion;
            if (netMsgQuestion == null) return;
            if (_logOutputRule == ELogOutputRule.OutputAlways)
            {
                Output(netMsgQuestion);
            }
            if (_msgHandleRule == EMsgHandleRule.BroadcastAlways)
            {
                server.Broadcast(netQuestion);
            }
            if (finished) return;
            if (_logOutputRule == ELogOutputRule.Output)
            {
                Output(netMsgQuestion);
            }
            if (_msgHandleRule == EMsgHandleRule.Broadcast)
            {
                server.Broadcast(netQuestion);
            }

            switch (_msgCheckRule)
            {
                case EMsgCheckRule.Any:
                    {
                        finished = true;
                        break;
                    }
                case EMsgCheckRule.CheckCmd:
                    {
                        finished = _CheckCmd.IsMatch(netMsgQuestion.cmd);
                        break;
                    }
                case EMsgCheckRule.CheckData:
                    {
                        finished = _CheckData.IsMatch(netMsgQuestion.data);
                        break;
                    }
                case EMsgCheckRule.CheckCmdAndData:
                    {
                        finished = _CheckCmdAndData._compareCmdValue.IsMatch(netMsgQuestion.cmd) && _CheckCmdAndData._compareDataValue.IsMatch(netMsgQuestion.data);
                        break;
                    }
                case EMsgCheckRule.CheckCmdOrData:
                    {
                        finished = _CheckCmdOrData._compareCmdValue.IsMatch(netMsgQuestion.cmd) || _CheckCmdOrData._compareDataValue.IsMatch(netMsgQuestion.data);
                        break;
                    }
            }
            switch (_logOutputRule)
            {
                case ELogOutputRule.OutputIfCheckValid:
                    {
                        if (finished) Output(netMsgQuestion);
                        break;
                    }
                case ELogOutputRule.OutputIfCheckInvalid:
                    {
                        if (!finished) Output(netMsgQuestion);
                        break;
                    }
            }
            switch (_msgHandleRule)
            {
                case EMsgHandleRule.BroadcastIfCheckValid:
                    {
                        if (finished) server.Broadcast(netMsgQuestion);
                        break;
                    }
                case EMsgHandleRule.BroadcastIfCheckInvalid:
                    {
                        if (!finished) server.Broadcast(netMsgQuestion);
                        break;
                    }
            }

            if (finished)
            {
                _clientGuidVariable.TrySetOrAddSetHierarchyVarValue(netMsgQuestion.client.guid);
                _clientIDVariable.TrySetOrAddSetHierarchyVarValue(netMsgQuestion.clientID);
                _cmdVariable.TrySetOrAddSetHierarchyVarValue(netMsgQuestion.cmd);
                _dataVariable.TrySetOrAddSetHierarchyVarValue(netMsgQuestion.data);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_server) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!server) { }
        }

        /// <summary>
        /// 提示
        /// </summary>
        /// <returns></returns>
        public override string ToFriendlyString()
        {
            switch (_msgCheckRule)
            {
                case EMsgCheckRule.Any:
                    {
                        return string.Format("命令变量:{0},数据变量:{1}", _cmdVariable, _dataVariable);
                    }
                case EMsgCheckRule.CheckCmd:
                    {
                        return "命令:" + _CheckCmd.compareValue;
                    }
                case EMsgCheckRule.CheckData:
                    {
                        return "数据:" + _CheckData.compareValue;
                    }
                case EMsgCheckRule.CheckCmdAndData:
                    {
                        return string.Format("命令:{0}和数据:{1}", _CheckCmdAndData._compareCmdValue.compareValue, _CheckCmdAndData._compareDataValue.compareValue);
                    }
                case EMsgCheckRule.CheckCmdOrData:
                    {
                        return string.Format("命令:{0}或数据:{1}", _CheckCmdOrData._compareCmdValue.compareValue, _CheckCmdOrData._compareDataValue.compareValue);
                    }
            }
            return "";
        }
    }
}
