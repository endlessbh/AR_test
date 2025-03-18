using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginNetInteract.Tools;

namespace XCSJ.EditorNetInteract.Tools
{
    /// <summary>
    /// 客户端检查器
    /// </summary>
    [Name("客户端检查器")]
    [CustomEditor(typeof(Client))]
    public class ClientInspector : MBInspector<Client>
    {
        /// <summary>
        /// 客户端连接信息:显示当前客户端与服务器的连接信息
        /// </summary>
        [Name("客户端连接信息")]
        [Tip("显示当前客户端与服务器的连接信息", "Displays the connection information between the current client and the server")]
        public bool _displayClient = true;

        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetObject = this.targetObject;
            var client = targetObject._client;
            EditorGUILayout.LabelField("客户端连接状态", CommonFun.Name(client.clientState));

            if (Application.isPlaying && targetObject.gameObject.activeInHierarchy)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("操作");
                if (GUILayout.Button("连接", EditorStyles.miniButtonLeft))
                {
                    targetObject.ConnectAndTrySyncObject();
                }
                if (GUILayout.Button("断开", EditorStyles.miniButtonMid))
                {
                    targetObject.CloseAndSyncObject();
                }
                if (GUILayout.Button("重连", EditorStyles.miniButtonRight))
                {
                    targetObject.ReconnectAndTrySyncObject();
                }
                EditorGUILayout.EndHorizontal();
            }

            _displayClient = UICommonFun.Foldout(_displayClient, CommonFun.NameTip(GetType(), nameof(_displayClient)));
            if (!_displayClient || !client.IsConnected()) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(CommonFun.TempContent("服务器IP", "服务器IP；")/*, UICommonOption.Width100*/);
            GUILayout.Label(CommonFun.TempContent("服务器端口", "服务器端口；"), UICommonOption.Width60);
            GUILayout.Label(CommonFun.TempContent("本地IP", "与服务器连接的本地IP；"), UICommonOption.Width100);
            GUILayout.Label(CommonFun.TempContent("本地端口", "与服务器连接的本地端口；"), UICommonOption.Width60);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            UICommonFun.BeginHorizontal(0);

            //编号
            EditorGUILayout.LabelField((1).ToString(), UICommonOption.Width32);

            //服务器IP
            EditorGUILayout.LabelField(client.remoteIP/*, UICommonOption.Width100*/);

            //服务器端口
            EditorGUILayout.LabelField(client.remotePort.ToString(), UICommonOption.Width60);

            //本地IP
            EditorGUILayout.LabelField(client.localIP, UICommonOption.Width100);

            //本地端口
            EditorGUILayout.LabelField(client.localPort.ToString(), UICommonOption.Width60);

            UICommonFun.EndHorizontal();

            CommonFun.EndLayout();
        }
    }
}
