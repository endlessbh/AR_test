using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Kernel;
using XCSJ.PluginSMS.States;
using XCSJ.PluginSMS.States.Base;

namespace XCSJ.PluginMMO.States
{
    [XCSJ.Attributes.Icon(EIcon.State)]
    [ComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
    [Name(Title, nameof(NetIdentityEvent))]
    [Tip("用于捕获网络标识组件的各种网络事件", "Used to capture various network events of the network identification component")]
    [RequireManager(typeof(MMOManager))]
    [Owner(typeof(MMOManager))]
    public class NetIdentityEvent : Trigger<NetIdentityEvent>, ISerializationCallbackReceiver
    {
        public const string Title = "网络标识事件";

        [StateLib(MMOHelper.CategoryName, typeof(MMOManager))]
        [StateComponentMenu(MMOHelper.CategoryName + "/" + Title, typeof(MMOManager))]
        [Name(Title, nameof(NetIdentityEvent))]
        [XCSJ.Attributes.Icon(EMemberRule.ReflectedType)]
        [Tip("用于捕获网络标识组件的各种网络事件", "Used to capture various network events of the network identification component")]
        public static State Create(IGetStateCollection obj) => CreateNormalState(obj);

        [Name("网络标识")]
        [ComponentPopup]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public NetIdentity netIdentity;

        [Name("网络事件类型")]
        [Tip("期望捕获的网络事件类型", "Type of network event expected to be captured")]
        [EnumPopup]
        public ENetEventType netEventType = ENetEventType.None;

        [Name("变量名")]
        [Tip("将网络事件回调的额外参数信息存储到当前变量中", "Store the additional parameter information of the network event callback into the current variable")]
        [VarString(EVarStringHierarchyKeyMode.Set)]
        public string variableName;

        #region ISerializationCallbackReceiver

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CommonFun.VarNameToVarString(ref variableName);
        }

        #endregion

        /// <summary>
        /// 将值设置到变量
        /// </summary>
        /// <param name="value">值</param>
        protected void SetToVariable(string value)
        {
            variableName.TrySetOrAddSetHierarchyVarValue(value);
        }

        public override void OnEntry(StateData stateData)
        {
            base.OnEntry(stateData);
            if (!netIdentity) return;
            switch (netEventType)
            {
                case ENetEventType.OnStart:
                    {
                        NetIdentity.onStart += OnStart;
                        break;
                    }
                case ENetEventType.OnStop:
                    {
                        NetIdentity.onStop += OnStop;
                        break;
                    }
                case ENetEventType.OnCloned:
                    {
                        NetIdentity.onCloned += OnCloned;
                        break;
                    }
                case ENetEventType.OnWillDestroy:
                    {
                        NetIdentity.onWillDestroy += OnWillDestroy;
                        break;
                    }
                case ENetEventType.OnStartPlayerLink:
                    {
                        NetIdentity.onStartPlayerLink += OnStartPlayerLink;
                        break;
                    }
                case ENetEventType.OnStopPlayerLink:
                    {
                        NetIdentity.onStopPlayerLink += OnStopPlayerLink;
                        break;
                    }
                case ENetEventType.OnSyncEnable:
                    {
                        NetIdentity.onSyncEnable += OnSyncEnable;
                        break;
                    }
                case ENetEventType.OnSyncDisable:
                    {
                        NetIdentity.onSyncDisable += OnSyncDisable;
                        break;
                    }
                case ENetEventType.OnStartAccess:
                case ENetEventType.OnStartLocalAccess:
                case ENetEventType.OnStartNormalAccess:
                    {
                        NetIdentity.onStartAccess += OnStartAccess;
                        break;
                    }
                case ENetEventType.OnStopAccess:
                case ENetEventType.OnStopLocalAccess:
                case ENetEventType.OnStopNormalAccess:
                    {
                        NetIdentity.onStopAccess += OnStopAccess;
                        break;
                    }
                case ENetEventType.OnPlayerEnter:
                    {
                        NetIdentity.onPlayerEnter += OnPlayerEnter;
                        break;
                    }
                case ENetEventType.OnPlayerExit:
                    {
                        NetIdentity.onPlayerExit += OnPlayerExit;
                        break;
                    }
                case ENetEventType.OnUserEnter:
                    {
                        NetIdentity.onUserEnter += OnUserEnter;
                        break;
                    }
                case ENetEventType.OnUserExit:
                    {
                        NetIdentity.onUserExit += OnUserExit;
                        break;
                    }
                case ENetEventType.OnClonedAsTemplate:
                    {
                        NetIdentity.onClonedAsTemplate += OnClonedAsTemplate;
                        break;
                    }
            }
        }

        public override void OnExit(StateData stateData)
        {
            base.OnExit(stateData);
            if (!netIdentity) return;

            switch (netEventType)
            {
                case ENetEventType.OnStart:
                    {
                        NetIdentity.onStart -= OnStart;
                        break;
                    }
                case ENetEventType.OnStop:
                    {
                        NetIdentity.onStop -= OnStop;
                        break;
                    }
                case ENetEventType.OnCloned:
                    {
                        NetIdentity.onCloned -= OnCloned;
                        break;
                    }
                case ENetEventType.OnWillDestroy:
                    {
                        NetIdentity.onWillDestroy -= OnWillDestroy;
                        break;
                    }
                case ENetEventType.OnStartPlayerLink:
                    {
                        NetIdentity.onStartPlayerLink -= OnStartPlayerLink;
                        break;
                    }
                case ENetEventType.OnStopPlayerLink:
                    {
                        NetIdentity.onStopPlayerLink -= OnStopPlayerLink;
                        break;
                    }
                case ENetEventType.OnSyncEnable:
                    {
                        NetIdentity.onSyncEnable -= OnSyncEnable;
                        break;
                    }
                case ENetEventType.OnSyncDisable:
                    {
                        NetIdentity.onSyncDisable -= OnSyncDisable;
                        break;
                    }
                case ENetEventType.OnStartAccess:
                case ENetEventType.OnStartLocalAccess:
                case ENetEventType.OnStartNormalAccess:
                    {
                        NetIdentity.onStartAccess -= OnStartAccess;
                        break;
                    }
                case ENetEventType.OnStopAccess:
                case ENetEventType.OnStopLocalAccess:
                case ENetEventType.OnStopNormalAccess:
                    {
                        NetIdentity.onStopAccess -= OnStopAccess;
                        break;
                    }
                case ENetEventType.OnPlayerEnter:
                    {
                        NetIdentity.onPlayerEnter -= OnPlayerEnter;
                        break;
                    }
                case ENetEventType.OnPlayerExit:
                    {
                        NetIdentity.onPlayerExit -= OnPlayerExit;
                        break;
                    }
                case ENetEventType.OnUserEnter:
                    {
                        NetIdentity.onUserEnter -= OnUserEnter;
                        break;
                    }
                case ENetEventType.OnUserExit:
                    {
                        NetIdentity.onUserExit -= OnUserExit;
                        break;
                    }
                case ENetEventType.OnClonedAsTemplate:
                    {
                        NetIdentity.onClonedAsTemplate -= OnClonedAsTemplate;
                        break;
                    }
            }
        }

        public override string ToFriendlyString()
        {
            return string.Format("{0}.{1}", netIdentity ? netIdentity.name : "", CommonFun.Name(netEventType));
        }

        public override bool DataValidity()
        {
            return base.DataValidity() && netIdentity;
        }

        #region 网络标识事件回调

        private void OnStart(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnStop(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnCloned(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnWillDestroy(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnStartPlayerLink(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnStopPlayerLink(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnSyncEnable(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnSyncDisable(NetIdentity netIdentity)
        {
            if (netIdentity == this.netIdentity) finished = true;
        }

        private void OnStartAccess(NetIdentity netIdentity, bool local)
        {
            if (netIdentity == this.netIdentity)
            {
                switch (netEventType)
                {
                    case ENetEventType.OnStartAccess:
                        {
                            finished = true;
                            SetToVariable(local.ToString());
                            break;
                        }
                    case ENetEventType.OnStartLocalAccess:
                        {
                            if (local)
                            {
                                finished = true;
                                SetToVariable(local.ToString());
                            }
                            break;
                        }
                    case ENetEventType.OnStartNormalAccess:
                        {
                            if (!local)
                            {
                                finished = true;
                                SetToVariable(local.ToString());
                            }
                            break;
                        }
                }
            }
        }

        private void OnStopAccess(NetIdentity netIdentity, bool local)
        {
            if (netIdentity == this.netIdentity)
            {
                switch (netEventType)
                {
                    case ENetEventType.OnStopAccess:
                        {
                            finished = true;
                            SetToVariable(local.ToString());
                            break;
                        }
                    case ENetEventType.OnStopLocalAccess:
                        {
                            if (local)
                            {
                                finished = true;
                                SetToVariable(local.ToString());
                            }
                            break;
                        }
                    case ENetEventType.OnStopNormalAccess:
                        {
                            if (!local)
                            {
                                finished = true;
                                SetToVariable(local.ToString());
                            }
                            break;
                        }
                }
            }
        }

        private void OnPlayerEnter(NetIdentity netIdentity, string guid)
        {
            if (netIdentity == this.netIdentity)
            {
                finished = true;
                SetToVariable(guid);
            }
        }

        private void OnPlayerExit(NetIdentity netIdentity, string guid)
        {
            if (netIdentity == this.netIdentity)
            {
                finished = true;
                SetToVariable(guid);
            }
        }

        private void OnUserEnter(NetIdentity netIdentity, string guid)
        {
            if (netIdentity == this.netIdentity)
            {
                finished = true;
                SetToVariable(guid);
            }
        }

        private void OnUserExit(NetIdentity netIdentity, string guid)
        {
            if (netIdentity == this.netIdentity)
            {
                finished = true;
                SetToVariable(guid);
            }
        }

        private void OnClonedAsTemplate(NetIdentity netIdentity, NetIdentity newNetIdentity)
        {
            if (netIdentity == this.netIdentity)
            {
                finished = true;
                SetToVariable(CommonFun.GameObjectComponentToString(newNetIdentity));
            }
        }

        #endregion
    }

    /// <summary>
    /// 网络事件类型
    /// </summary>
    [Name("网络事件类型")]
    public enum ENetEventType
    {
        [Name("无")]
        None,

        [Name("当启动时")]
        [Tip("当对象在网络环境中生效(包括网络连接成功、MMO对象克隆等情况)时回调", "Callback when the object takes effect in the network environment (including successful network connection, MMO object cloning, etc.)")]
        OnStart,

        [Name("当停止时")]
        [Tip("当对象在网络环境中失效(包括网络连接关闭、MMO对象销毁等情况)时回调", "Callback when the object fails in the network environment (including network connection closing, MMO object destruction, etc.)")]
        OnStop,

        [Name("当克隆时")]
        [Tip("当对象在网络环境中MMO对象克隆时回调；如果对象调用本函数，说明当前对象是运行时被动态克隆的；", "Callback when the object is cloned from the MMO object in the network environment; If the object calls this function, it indicates that the current object is dynamically cloned at runtime;")]
        OnCloned,

        [Name("当将要销毁时")]
        [Tip("当对象在网络环境中MMO对象将销毁时回调；如果对象调用本函数，说明当前对象是运行时被动态克隆的；", "Callback when the MMO object will be destroyed in the network environment; If the object calls this function, it indicates that the current object is dynamically cloned at runtime;")]
        OnWillDestroy,

        [Name("当启动玩家关联时")]
        [Tip("当前对象与网络环境中的玩家产生关联时回调", "Callback when the current object is associated with a player in the network environment")]
        OnStartPlayerLink,

        [Name("当停止玩家关联时")]
        [Tip("当前对象与网络环境中的玩家解除关联时回调", "Callback when the current object is disassociated from the player in the network environment")]
        OnStopPlayerLink,

        [Name("当同步启用时")]
        [Tip("当前对象同步状态启用时回调，即当前对象变为可同步数据的状态时（即进入房间后）回调；", "Callback when the synchronization status of the current object is enabled, that is, callback when the current object changes to the status of synchronizable data (i.e. after entering the room);")]
        OnSyncEnable,

        [Name("当同步禁用时")]
        [Tip("当前对象同步状态禁用时回调，即当前对象变为不可可同步数据的状态时（即退出房间后）回调；", "Callback when the synchronization status of the current object is disabled, that is, callback when the current object changes to the status of non synchronized data (i.e. after exiting the room);")]
        OnSyncDisable,

        [Name("当启动权限时")]
        [Tip("当前用户(玩家)对当前对象开始具有权限时回调", "Callback when the current user (player) starts to have permission on the current object")]
        OnStartAccess,

        [Name("当停止权限时")]
        [Tip("当前用户(玩家)对当前对象停止具有权限时回调", "Callback when the current user (player) stops having permission on the current object")]
        OnStopAccess,

        [Name("当玩家进入时")]
        [Tip("当有玩家进入房间时回调", "Callback when a player enters the room")]
        OnPlayerEnter,

        [Name("当玩家退出时")]
        [Tip("当有玩家退出房间时回调", "Callback when a player exits the room")]
        OnPlayerExit,

        [Name("当启动本地权限时")]
        [Tip("当前用户(玩家)对当前对象开始具有本地权限时回调", "Callback when the current user (player) starts to have local permissions on the current object")]
        OnStartLocalAccess,

        [Name("当停止本地权限时")]
        [Tip("当前用户(玩家)对当前对象停止具有本地权限时回调", "Callback when the current user (player) stops having local permissions on the current object")]
        OnStopLocalAccess,

        [Name("当启动普通权限时")]
        [Tip("当前用户(玩家)对当前对象开始具有普通权限时回调", "Callback when the current user (player) starts to have normal permissions on the current object")]
        OnStartNormalAccess,

        [Name("当停止普通权限时")]
        [Tip("当前用户(玩家)对当前对象停止具有普通权限时回调", "Callback when the current user (player) stops having normal permissions on the current object")]
        OnStopNormalAccess,

        [Name("当用户进入时")]
        [Tip("当有用户进入房间时回调", "Callback when a user enters the room")]
        OnUserEnter,

        [Name("当用户退出时")]
        [Tip("当有用户退出房间时回调", "Callback when a user exits the room")]
        OnUserExit,

        [Name("当做为模版被克隆时")]
        [Tip("当对象在网络环境中MMO对象被作为模版克隆时回调；如果对象调用本函数，说明当前对象是运行时被作为模版被动态克隆了新的对象；", "Callback when the MMO object is cloned as a template in the network environment; If the object calls this function, it means that the current object is dynamically cloned as a template at runtime;")]
        OnClonedAsTemplate,
    }
}
