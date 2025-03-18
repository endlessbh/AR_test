using UnityEditor;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.EditorCommonUtils;
using XCSJ.EditorCommonUtils.Base.CategoryViews;
using XCSJ.Languages;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO;

namespace XCSJ.EditorMMO
{
    /// <summary>
    /// 多人在线MMO检查器
    /// </summary>
    [Name("多人在线MMO检查器")]
    [CustomEditor(typeof(MMOManager))]
    public class MMOManagerInspector : BaseManagerInspector<MMOManager>
    {
        /// <summary>
        /// 当绘制检查器GUI
        /// </summary>
        [LanguageTuple("Connect", "连接")]
        [LanguageTuple("Close", "关闭")]
        [LanguageTuple("Login", "登录")]
        [LanguageTuple("Logout", "登出")]
        [LanguageTuple("JoinRoom", "加入房间")]
        [LanguageTuple("QuitRoom", "退出房间")]
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var manager = this.manager;
            if (Application.isPlaying)
            {
                var isConnected = manager.IsConnected();

                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(isConnected);
                if (GUILayout.Button(Tr("Connect")))
                {
                    manager.Connect();
                }
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button(Tr("Close")))
                {
                    manager.Close();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginDisabledGroup(!isConnected);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Tr("Login")))
                {
                    manager.Login();
                }
                if (GUILayout.Button(Tr("Logout")))
                {
                    manager.Logout();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Tr("JoinRoom")))
                {
                    manager.JoinRoom();
                }
                if (GUILayout.Button(Tr("QuitRoom")))
                {
                    manager.QuitRoom();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.BeginDisabledGroup(!manager.IsConnected());
                return;
            }

            if (!manager.GetComponent<MMOManagerHUD>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(MMOManagerHUD))))
                {
                    Undo.AddComponent<MMOManagerHUD>(manager.gameObject);
                }
            }

            if (!manager.GetComponent<MMOPlayerCreater>())
            {
                if (GUILayout.Button(CommonFun.NameTip(typeof(MMOPlayerCreater))))
                {
                    Undo.AddComponent<MMOPlayerCreater>(manager.gameObject);
                }
            }

            DrawDetailInfos();

            MMOMBInspector.categoryList.DrawVertical();
        }

        /// <summary>
        /// 网络标识列表
        /// </summary>
        [Name("网络标识列表")]
        [Tip("当前场景中所有的网络标识对象", "All network identification objects in the current scene")]
        public bool netIdentities = true;

        [Name("网络标识")]
        [Tip("网络标识所在的游戏对象；本项只读；", "The game object where the network logo is located; This item is read-only;")]
        public bool netIdentity;

        [Name("本地权限")]
        [Tip("详细解释请查看网络标识对象上对应属性的具体解释；本项只读；", "For detailed explanation, please check the specific explanation of the corresponding attribute on the network identification object; This item is read-only;")]
        public bool localAccess;

        [Name("权限")]
        [Tip("详细解释请查看网络标识对象上对应属性的具体解释；本项只读；", "For detailed explanation, please check the specific explanation of the corresponding attribute on the network identification object; This item is read-only;")]
        public bool access;

        private void DrawDetailInfos()
        {
            netIdentities = UICommonFun.Foldout(netIdentities, TrLabel(nameof(netIdentities)));
            if (!netIdentities) return;

            CommonFun.BeginLayout();

            #region 标题            

            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            GUILayout.Label("NO.", UICommonOption.Width32);
            GUILayout.Label(TrLabel(nameof(netIdentity)));
            GUILayout.Label(TrLabel(nameof(localAccess)), UICommonOption.Width120);
            GUILayout.Label(TrLabel(nameof(access)), UICommonOption.Width120);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            #endregion

            var cache = ComponentCache.Get(typeof(NetIdentity), true);
            for (int i = 0; i < cache.components.Length; i++)
            {
                var component = cache.components[i] as NetIdentity;

                UICommonFun.BeginHorizontal(i);

                //编号
                EditorGUILayout.LabelField((i + 1).ToString(), UICommonOption.Width32);

                //网络标识
                var gameObject = component.gameObject;
                EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

                //本地权限
                EditorGUILayout.Toggle(component.localAccess, UICommonOption.Width120);

                //权限
                EditorGUILayout.Toggle(component.access, UICommonOption.Width120);

                UICommonFun.EndHorizontal();
            }

            CommonFun.EndLayout();
        }
    }
}
