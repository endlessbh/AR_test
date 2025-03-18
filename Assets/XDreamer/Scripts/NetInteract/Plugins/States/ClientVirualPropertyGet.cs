using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginNetInteract.Base;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;
using XCSJ.Scripts;

namespace XCSJ.PluginNetInteract.States
{
    /// <summary>
    /// 虚拟客户端属性获取: 虚拟客户端属性获取
    /// </summary>
    [Tip("虚拟客户端属性获取", "Virtual client property acquisition")]
    [Name(Title)]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(NetInteractManager))]
    public class ClientVirualPropertyGet : BasePropertyGet<ClientVirualPropertyGet>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 虚拟客户端属性获取
        /// </summary>
        public const string Title = "虚拟客户端属性获取";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Server, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ClientSendMsg))]
        [Tip("虚拟客户端属性获取", "Virtual client property acquisition")]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 客户端唯一编号：虚拟客户端唯一编号
        /// </summary>
        [Name("虚拟客户端唯一编号")]
        [Tip("虚拟客户端唯一编号")]
        [OnlyMemberElements]
        public StringPropertyValue _clientGuid = new StringPropertyValue();

        /// <summary>
        /// 客户端唯一编号
        /// </summary>
        public string clientGuid => _clientGuid.GetValue("");

        /// <summary>
        /// 虚拟客户端
        /// </summary>
        public ClientVirtual client
        {
            get
            {
                var manager = NetInteractManager.instance;
                return manager ? manager.GetClientVirtual(clientGuid) : null;
            }
        }

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
#if UNITY_2019_3_OR_NEWER
        //[EnumPopup]
#else
        [EnumPopup]
#endif
        public EPropertyName _propertyName = EPropertyName.None;

        /// <summary>
        /// 属性名称
        /// </summary>
        [Name("属性名称")]
        public enum EPropertyName
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            [EnumFieldName("无")]
            None,

            /// <summary>
            /// 无延时
            /// </summary>
            [Name("无延时")]
            [EnumFieldName("无延时")]
            NoDelay,

            /// <summary>
            /// 读取超时
            /// </summary>
            [Name("读取超时")]
            [Tip("读取超时时间;单位：毫秒;")]
            [EnumFieldName("读取超时")]
            ReadTimeout,

            /// <summary>
            /// 写入超时
            /// </summary>
            [Name("写入超时")]
            [Tip("写入超时时间;单位：毫秒;")]
            [EnumFieldName("写入超时")]
            WriteTimeout,

            /// <summary>
            /// 是否连接
            /// </summary>
            [Name("是否连接")]
            [EnumFieldName("是否连接")]
            IsConnected,

            /// <summary>
            /// 是否连接到远程
            /// </summary>
            [Name("是否连接到远程")]
            [EnumFieldName("是否连接到远程")]
            IsConnectToRemote,

            /// <summary>
            /// 有效连接
            /// </summary>
            [Name("有效连接")]
            [EnumFieldName("有效连接")]
            ValidConnection,

            /// <summary>
            /// 本地地址
            /// </summary>
            [Name("本地地址")]
            [EnumFieldName("本地地址")]
            LocalAddress,

            /// <summary>
            /// 本地端口
            /// </summary>
            [Name("本地端口")]
            [EnumFieldName("本地端口")]
            LocalPort,

            /// <summary>
            /// 远程地址
            /// </summary>
            [Name("远程地址")]
            [EnumFieldName("远程地址")]
            RemoteAddress,

            /// <summary>
            /// 远程端口
            /// </summary>
            [Name("远程端口")]
            [EnumFieldName("远程端口")]
            RemotePort,
        }

        /// <summary>
        /// 变量名:将获取到的属性值存储在变量名对应的全局变量中
        /// </summary>
        [Name("变量名")]
        [Tip("将获取到的属性值存储在变量名对应的全局变量中")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string _variableName;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref _variableName);
        }

        #endregion

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(object value)
        {
            _variableName.TrySetOrAddSetHierarchyVarValue(value);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="stateData"></param>
        /// <param name="executeMode"></param>
        public override void Execute(StateData stateData, EExecuteMode executeMode)
        {
            var client = this.client;
            if (client == null) return;
            switch (_propertyName)
            {
                case EPropertyName.NoDelay:
                    {
                        SetToVariable(client.noDelay);
                        break;
                    }
                case EPropertyName.ReadTimeout:
                    {
                        SetToVariable(client.readTimeout);
                        break;
                    }
                case EPropertyName.WriteTimeout:
                    {
                        SetToVariable(client.writeTimeout);
                        break;
                    }
                case EPropertyName.IsConnected:
                    {
                        SetToVariable(client.isConnected);
                        break;
                    }
                case EPropertyName.IsConnectToRemote:
                    {
                        SetToVariable(client.isConnectToRemote);
                        break;
                    }
                case EPropertyName.ValidConnection:
                    {
                        SetToVariable(client.isConnected && client.isConnectToRemote);
                        break;
                    }
                case EPropertyName.LocalAddress:
                    {
                        SetToVariable(client.localIP);
                        break;
                    }
                case EPropertyName.LocalPort:
                    {
                        SetToVariable(client.localPort);
                        break;
                    }
                case EPropertyName.RemoteAddress:
                    {
                        SetToVariable(client.remoteIP);
                        break;
                    }
                case EPropertyName.RemotePort:
                    {
                        SetToVariable(client.remotePort);
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            if (!_clientGuid.DataValidity()) return false;
            return base.DataValidity();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            _clientGuid._propertyValueType = EPropertyValueType.Variable;
        }
    }
}
