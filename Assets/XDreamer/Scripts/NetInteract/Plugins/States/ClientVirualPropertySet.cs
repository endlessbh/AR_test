using System;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Net;
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
    /// 虚拟客户端属性设置: 虚拟客户端属性设置
    /// </summary>
    [Tip("虚拟客户端属性设置")]
    [Name(Title)]
    [ComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
    [XCSJ.Attributes.Icon(EIcon.Property)]
    [Owner(typeof(NetInteractManager))]
    public class ClientVirualPropertySet : BasePropertySet<ClientVirualPropertySet>
    {
        /// <summary>
        /// 虚拟客户端属性设置
        /// </summary>
        public const string Title = "虚拟客户端属性设置";

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib(NetInteractHelper.Server, typeof(NetInteractManager))]
        [StateComponentMenu(NetInteractHelper.Server + "/" + Title, typeof(NetInteractManager))]
        [Name(Title, nameof(ClientSendMsg))]
        [Tip("虚拟客户端属性设置")]
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
            /// 读取超时:读取超时时间;单位：毫秒;
            /// </summary>
            [Name("读取超时")]
            [Tip("读取超时时间;单位：毫秒;")]
            [EnumFieldName("读取超时")]
            ReadTimeout,

            /// <summary>
            /// 写入超时:写入超时时间;单位：毫秒;
            /// </summary>
            [Name("写入超时")]
            [Tip("写入超时时间;单位：毫秒;")]
            [EnumFieldName("写入超时")]
            WriteTimeout,
        }

        /// <summary>
        /// 无延时:无延时
        /// </summary>
        [Name("无延时")]
        [Tip("无延时")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.NoDelay)]
        [OnlyMemberElements]
        public EBoolPropertyValue _NoDelay = new EBoolPropertyValue();

        /// <summary>
        /// 读取超时:读取超时时间;单位：毫秒;本值不允许低于3000；
        /// </summary>
        [Name("读取超时")]
        [Tip("读取超时时间;单位：毫秒;本值不允许低于3000；")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.ReadTimeout)]
        [OnlyMemberElements]
        public IntPropertyValue _ReadTimeout = new IntPropertyValue();

        /// <summary>
        /// 写入超时:写入超时时间;单位：毫秒;本值不允许低于3000；
        /// </summary>
        [Name("写入超时")]
        [Tip("写入超时时间;单位：毫秒;本值不允许低于3000；")]
        [HideInSuperInspector(nameof(_propertyName), EValidityCheckType.NotEqual, EPropertyName.WriteTimeout)]
        [OnlyMemberElements]
        public IntPropertyValue _WriteTimeout = new IntPropertyValue();

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
                        client.noDelay = _NoDelay.GetValue(client.noDelay, EBool.Yes);
                        break;
                    }
                case EPropertyName.ReadTimeout:
                    {
                        client.readTimeout = Math.Max(NetHelper.DefaultTimeoutOfClient, _ReadTimeout.GetValue(NetHelper.DefaultTimeoutOfServer));
                        break;
                    }
                case EPropertyName.WriteTimeout:
                    {
                        client.writeTimeout = Math.Max(NetHelper.DefaultTimeoutOfClient, _WriteTimeout.GetValue(NetHelper.DefaultTimeoutOfServer));
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
            _ReadTimeout._value = NetHelper.DefaultTimeoutOfClient;
            _WriteTimeout._value = NetHelper.DefaultTimeoutOfClient;
        }
    }
}
