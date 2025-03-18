using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.Net;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Tools;

namespace XCSJ.EditorNetInteract.Tools
{
    /// <summary>
    /// 服务器检查器
    /// </summary>
    [Name("服务器检查器")]
    [CustomEditor(typeof(Server))]
    public class ServerInspector : MBInspector<Server>
    {
        /// <summary>
        /// 已连接客户端列表:已连接到当前服务器的所有客户端
        /// </summary>
        [Name("已连接客户端列表")]
        [Tip("已连接到当前服务器的所有客户端", "All clients connected to the current server")]
        public bool _displayClients = true;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetObject = this.targetObject;
            var server = targetObject._server;
            EditorGUILayout.LabelField("服务器监听状态", server.IsRunning() ? "运行中" : "已停止");
            EditorGUILayout.LabelField("监听端口", targetObject.validListenPort.ToString());

            if (Application.isPlaying && targetObject.gameObject.activeInHierarchy)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("操作");
                if (GUILayout.Button("启动", EditorStyles.miniButtonLeft))
                {
                    targetObject.StartServerAndTrySyncObject();
                }
                if (GUILayout.Button("停止", EditorStyles.miniButtonMid))
                {
                    targetObject.StopServerAndSyncObject();
                }
                if (GUILayout.Button("重启", EditorStyles.miniButtonRight))
                {
                    targetObject.RestartServerAndTrySyncObject();
                }
                EditorGUILayout.EndHorizontal();
            }

            _displayClients = UICommonFun.Foldout(_displayClients, CommonFun.NameTip(GetType(), nameof(_displayClients)));
            if (!_displayClients) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("客户端唯一标识", "客户端唯一标识，本值由服务器分配，在连接过程中保持不变；"));
            GUILayout.Label(CommonFun.TempContent("客户端IP", "客户端IP；"), UICommonOption.Width100);
            GUILayout.Label(CommonFun.TempContent("客户端端口", "客户端端口；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("本地IP", "与客户端连接的本地IP；"), UICommonOption.Width100);
            GUILayout.Label(CommonFun.TempContent("本地端口", "与客户端连接的本地端口；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("模式", "连接模式；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("远程", "检测远程连接是否有效"), UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("操作", "操作；"), UICommonOption.Width32);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var clients = server.clients.CopyList();
            var count = clients.Count;
            for (int i = 0; i < count; i++)
            {
                var client = clients[i];
                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //客户端唯一标识
                EditorGUILayout.LabelField(client.guid);

                //客户端IP
                EditorGUILayout.LabelField(client.remoteIP, UICommonOption.Width100);

                //客户端端口
                EditorGUILayout.LabelField(client.remotePort.ToString(), UICommonOption.Width60);

                //本地IP
                EditorGUILayout.LabelField(client.localIP, UICommonOption.Width100);

                //本地端口
                EditorGUILayout.LabelField(client.localPort.ToString(), UICommonOption.Width60);

                //连接模式
                EditorGUILayout.LabelField(CommonFun.Name(client.netQAMode), UICommonOption.Width60);

                //操作
                if (GUILayout.Button("检测", UICommonOption.Width32))
                {
                    if (!client.isConnectToRemote) client.Close();
                }

                //操作
                if (GUILayout.Button("关闭", UICommonOption.Width32))
                {
                    client.Close();
                }

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
