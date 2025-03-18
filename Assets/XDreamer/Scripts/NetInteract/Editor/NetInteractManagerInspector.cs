using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.EditorTools;
using XCSJ.EditorTools.Windows;
using XCSJ.Extension.Base.Net;
using XCSJ.Languages;
using XCSJ.Net;
using XCSJ.Net.Tcp.Threading;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract;
using XCSJ.PluginNetInteract.Tools;

namespace XCSJ.EditorNetInteract
{
    /// <summary>
    /// 网络交互管理器检查器
    /// </summary>
    [CustomEditor(typeof(NetInteractManager))]
    [Name("网络交互管理器检查器")]
    public class NetInteractManagerInspector : BaseManagerInspector<NetInteractManager>
    {
        private static CategoryList categoryList = null;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (categoryList == null) categoryList = EditorToolsHelper.GetWithPurposes(nameof(NetInteractManager));
        }

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();
            DrawServersInternal();
            DrawClientsInternal();

            EditorGUILayout.Separator();
            categoryList.DrawVertical();
        }

        /// <summary>
        /// 服务器列表
        /// </summary>
        [Name("服务器列表")]
        [Tip("当前场景中所有的服务器对象", "All server objects in the current scene")]
        private static bool servers = true;

        /// <summary>
        /// 客户端列表
        /// </summary>
        [Name("客户端列表")]
        [Tip("当前场景中所有的客户端对象", "All client objects in the current scene")]
        private static bool clients = true;

        private void DrawServersInternal()
        {
            servers = UICommonFun.Foldout(servers, TrLabel(nameof(servers)));
            if (!servers) return;
            DrawServers();
        }

        [Name("服务器")]
        [Tip("服务器所在的游戏对象；本项只读；", "The game object where the server is located; This item is read-only;")]
        public bool server;

        [Name("状态")]
        [Tip("服务器状态", "Server status")]
        public bool serverState;

        [Name("监听端口")]
        [Tip("服务器的监听端口", "Listening port of the server")]
        public bool listenPort;

        [Name("客户端数")]
        [Tip("已连入服务器的客户端数量", "已连入服务器的客户端数量")]
        public bool clientCount;

        [Name("操作")]
        [Tip("对服务器进行控制", "Control the server")]
        public bool serverOperation;

        [LanguageTuple("Running", "运行中")]
        [LanguageTuple("Stopped", "已停止")]
        internal static void DrawServers()
        {
            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(server)));
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(serverState)), UICommonOption.Width48);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(listenPort)), UICommonOption.Width48);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(clientCount)), UICommonOption.Width48);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(serverOperation)), UICommonOption.Width32x3);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            var components = ComponentCache.Get(typeof(Server), true).components;
            var count = components.Length;
            for (int i = 0; i < count; i++)
            {
                var component = components[i] as Server;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //服务器
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //服务器监听状态
                EditorGUILayout.LabelField(component.isRunning ? "Running".Tr(typeof(NetInteractManagerInspector)) : "Stopped".Tr(typeof(NetInteractManagerInspector)), UICommonOption.Width48);

                //监听端口
                if (component.isRunning)
                {
                    EditorGUILayout.LabelField(component.validListenPort.ToString(), UICommonOption.Width48);
                }
                else
                {
                    component.port = EditorGUILayout.IntField(component.port, UICommonOption.Width48);
                }

                //客户端数
                EditorGUILayout.LabelField(component._server.clients.count.ToString(), UICommonOption.Width48);

                //操作
                EditorGUI.BeginDisabledGroup(!isPlaying || !component.gameObject.activeInHierarchy);
                {
                    if (GUILayout.Button("启动", EditorStyles.miniButtonLeft, UICommonOption.Width32))
                    {
                        component.StartServerAndTrySyncObject();
                    }
                    if (GUILayout.Button("停止", EditorStyles.miniButtonMid, UICommonOption.Width32))
                    {
                        component.StopServerAndSyncObject();
                    }
                    if (GUILayout.Button("重启", EditorStyles.miniButtonRight, UICommonOption.Width32))
                    {
                        component.RestartServerAndTrySyncObject();
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }

        private void DrawClientsInternal()
        {
            clients = UICommonFun.Foldout(clients, TrLabel(nameof(clients)));
            if (!clients) return;
            DrawClients();
        }

        [Name("客户端")]
        [Tip("客户端所在的游戏对象；本项只读；", "The client of the game object; This item is read-only;")]
        public bool client;

        [Name("状态")]
        [Tip("客户端状态", "Client status")]
        public bool clientState;

        [Name("地址")]
        [Tip("服务器地址", "server address")]
        public bool addresss;

        [Name("端口")]
        [Tip("服务器端口", "Server port")]
        public bool port;

        [Name("模式")]
        [Tip("连接模式", "Connection mode")]
        public bool mode;

        [Name("操作")]
        [Tip("对客户端进行控制", "Control the client")]
        public bool clientOperation;

        internal static void DrawClients()
        {           
            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(client)));
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(clientState)), UICommonOption.Width48);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(addresss)), UICommonOption.Width100);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(port)), UICommonOption.Width48);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(mode)), UICommonOption.Width60);
            GUILayout.Label(typeof(NetInteractManagerInspector).TrLabel(nameof(clientOperation)), UICommonOption.Width32x3);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var isPlaying = Application.isPlaying;
            var components = ComponentCache.Get(typeof(Client), true).components;
            var count = components.Length;
            for (int i = 0; i < count; i++)
            {
                var component = components[i] as Client;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //客户端
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //状态
                EditorGUILayout.LabelField(CommonFun.Name(component._client.clientState), UICommonOption.Width48);

                //地址-端口-模式
                if (component.isConnected)
                {
                    EditorGUILayout.LabelField(component.serverAddress, UICommonOption.Width100);
                    EditorGUILayout.LabelField(component.serverPort.ToString(), UICommonOption.Width48);
                    EditorGUILayout.LabelField(CommonFun.Name(component.connectMode), UICommonOption.Width60);
                    EditorGUILayout.LabelField(CommonFun.Name(component.netQAMode), UICommonOption.Width48);
                }
                else
                {
                    component.serverAddress = EditorGUILayout.TextField(component.serverAddress, UICommonOption.Width100);
                    component.serverPort = EditorGUILayout.IntField(component.serverPort, UICommonOption.Width48);
                    component.connectMode = (EConnectMode)UICommonFun.EnumPopup(component.connectMode, UICommonOption.Width60);
                    component.netQAMode = (ENetQAMode)UICommonFun.EnumPopup(component.netQAMode, UICommonOption.Width48);
                }

                //操作
                EditorGUI.BeginDisabledGroup(!isPlaying || !component.gameObject.activeInHierarchy);
                {
                    if (GUILayout.Button("启动", EditorStyles.miniButtonLeft, UICommonOption.Width32))
                    {
                        component.ConnectAndTrySyncObject();
                    }
                    if (GUILayout.Button("停止", EditorStyles.miniButtonMid, UICommonOption.Width32))
                    {
                        component.CloseAndSyncObject();
                    }
                    if (GUILayout.Button("重启", EditorStyles.miniButtonRight, UICommonOption.Width32))
                    {
                        component.ReconnectAndTrySyncObject();
                    }
                }
                EditorGUI.EndDisabledGroup();

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }

    /// <summary>
    /// 服务器列表查看器
    /// </summary>
    [ToolObjectViewerEditor(typeof(Server), true)]
    public class ServerListViewer : ToolObjectViewerEditor
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            //base.OnGUI();
            NetInteractManagerInspector.DrawServers();
        }
    }

    /// <summary>
    /// 客户端列表查看器
    /// </summary>
    [ToolObjectViewerEditor(typeof(Client), true)]
    public class ClientListViewer : ToolObjectViewerEditor
    {
        /// <summary>
        /// 绘制GUI
        /// </summary>
        public override void OnGUI()
        {
            //base.OnGUI();
            NetInteractManagerInspector.DrawClients();
        }
    }
}
