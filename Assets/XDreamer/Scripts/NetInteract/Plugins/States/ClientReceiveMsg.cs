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
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 客户端收到消息:客户端收到服务器发送的消息
    /// </summary>
    [Name(Title)]
    [Tip("客户端收到服务器发送的消息", "The client receives the message sent by the server")]
    [ComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Import)]
    [Owner(typeof(NetInteractManager))]
    public class ClientReceiveMsg : Trigger<ClientReceiveMsg>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "客户端收到消息";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Client, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Client + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ClientReceiveMsg))]
        [Tip("客户端收到服务器发送的消息", "The client receives the message sent by the server")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 客户端
        /// </summary>
        [Name("客户端")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Client _client;

        /// <summary>
        /// 客户端
        /// </summary>
        public Client client => this.XGetComponentInGlobal(ref _client);

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
        [Tip("用于将消息中数据项存储的变量", "Use variables in message items to store data")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _dataVariable = "";

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
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
            Client.onReceived += OnReceived;
        }

        /// <summary>
        /// 当退出
        /// </summary>
        /// <param name="stateData"></param>
        public override void OnExit(StateData stateData)
        {
            Client.onReceived -= OnReceived;
            base.OnExit(stateData);
        }

        private void Output(NetMsgAnswer netAnswer)
        {
            var log = string.Format(@"客户端收到答案消息:{0}
客户端对象:{1}
客户端标识:{2}
命令:{3}
数据:{4}",
                this.GetNamePath(),
                CommonFun.GameObjectToString(_client.gameObject),
                netAnswer.clientID,
                netAnswer.cmd,
                netAnswer.data);

            //Debug.Log(log);
            Log.Debug(log);
        }

        /// <summary>
        /// 当接收到网络答案时回调
        /// </summary>
        /// <param name="client"></param>
        /// <param name="netAnswer"></param>
        private void OnReceived(Client client, NetAnswer netAnswer)
        {
            if (client != this.client) return;
            var netMsgAnswer = netAnswer as NetMsgAnswer;
            if (netMsgAnswer == null) return;
            if (_logOutputRule == ELogOutputRule.OutputAlways)
            {
                Output(netMsgAnswer);
            }
            if (finished)
            {
                return;
            }
            if (_logOutputRule == ELogOutputRule.Output)
            {
                Output(netMsgAnswer);
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
                        finished = _CheckCmd.IsMatch(netMsgAnswer.cmd);
                        break;
                    }
                case EMsgCheckRule.CheckData:
                    {
                        finished = _CheckData.IsMatch(netMsgAnswer.data);
                        break;
                    }
                case EMsgCheckRule.CheckCmdAndData:
                    {
                        finished = _CheckCmdAndData._compareCmdValue.IsMatch(netMsgAnswer.cmd) && _CheckCmdAndData._compareDataValue.IsMatch(netMsgAnswer.data);
                        break;
                    }
                case EMsgCheckRule.CheckCmdOrData:
                    {
                        finished = _CheckCmdOrData._compareCmdValue.IsMatch(netMsgAnswer.cmd) || _CheckCmdOrData._compareDataValue.IsMatch(netMsgAnswer.data);
                        break;
                    }
            }
            switch (_logOutputRule)
            {
                case ELogOutputRule.OutputIfCheckValid:
                    {
                        if (finished) Output(netMsgAnswer);
                        break;
                    }
                case ELogOutputRule.OutputIfCheckInvalid:
                    {
                        if (!finished) Output(netMsgAnswer);
                        break;
                    }
            }
            if (finished)
            {
                _clientIDVariable.TrySetOrAddSetHierarchyVarValue(netMsgAnswer.clientID);
                _cmdVariable.TrySetOrAddSetHierarchyVarValue(netMsgAnswer.cmd);
                _dataVariable.TrySetOrAddSetHierarchyVarValue(netMsgAnswer.data);
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_client) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (!client) { }
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
                        return string.Format("命令:{0}和数据:{1}" , _CheckCmdAndData._compareCmdValue.compareValue, _CheckCmdAndData._compareDataValue.compareValue);
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
