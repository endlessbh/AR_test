using XCSJ.Attributes;
using XCSJ.DataBase;
using XCSJ.Extension.Base.Net;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginDataBase.Tools
{
    /// <summary>
    /// 基础网络数据库组件
    /// </summary>
    public abstract class BaseNetDBMB : DBMB
    {
        /// <summary>
        /// 数据库对象
        /// </summary>
        public override BaseDB db => netDB;

        /// <summary>
        /// 网络数据库
        /// </summary>
        public abstract NetDB netDB { get; protected set; }

        /// <summary>
        /// 客户端网络包
        /// </summary>
        public abstract IClientNetPackage client { get; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public override string dbName
        {
            get
            {
                var name = base.dbName;
                if (string.IsNullOrEmpty(name))
                {
                    return _connectServerConfig.ToFriendlyString();
                }
                return name;
            }
        }

        /// <summary>
        /// 连接服务器配置
        /// </summary>
        [Name("连接服务器配置")]
        public ConnectServerConfigWithMode _connectServerConfig = new ConnectServerConfigWithMode();

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string serverAddress
        {
            get => _connectServerConfig.address;
            set
            {
                if (_connectServerConfig.address == value) return;
                this.XModifyProperty(ref _connectServerConfig._address, value);
            }
        }

        /// <summary>
        /// 服务器端口
        /// </summary>
        public int serverPort
        {
            get => _connectServerConfig.port;
            set
            {
                if (_connectServerConfig.port == value) return;
                this.XModifyProperty(ref _connectServerConfig._port, value);
            }
        }

        /// <summary>
        /// 连接模式
        /// </summary>
        public EConnectMode connectMode
        {
            get => _connectServerConfig._connectMode;
            set
            {
                if (_connectServerConfig._connectMode == value) return;
                this.XModifyProperty(ref _connectServerConfig._connectMode, value);
            }
        }

        /// <summary>
        /// 处理客户端配置
        /// </summary>
        [Name("处理客户端配置")]
        public HandleClientConfig _handleClientConfig = new HandleClientConfig();

        /// <summary>
        /// 用户信息
        /// </summary>
        [Name("用户信息")]
        public UserInfo _userInfo = new UserInfo();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            netDB = null;
            base.OnEnable();
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        protected override void CloseDB()
        {
            netDB?.Close();
        }
    }
}
