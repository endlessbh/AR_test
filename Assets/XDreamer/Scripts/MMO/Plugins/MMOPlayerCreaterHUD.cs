using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Helper;
using XCSJ.Languages;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginMMO
{
    [ExecuteInEditMode]
    [RequireManager(typeof(MMOManager))]
    [RequireComponent(typeof(MMOPlayerCreater))]
    [Name("多人在线MMO-玩家生成器HUD", "MMO-Player Creater HUD")]
    [DisallowMultipleComponent]
    public sealed class MMOPlayerCreaterHUD : InteractProvider, IReset
    {
        public MMOPlayerCreater creater { get; private set; }

        public BaseGUIWindow hudWindow => window;

        [Name("HUD窗口")]
        public HUDWindow window = new HUDWindow();

        private void Awake()
        {
            window.HUD = this;
            window.creater = creater = GetComponent<MMOPlayerCreater>();
        }

        private void Start()
        {
            window.Start();
        }

        private void OnGUI()
        {
            window.OnGUI();
        }

        public void Reset()
        {
            window.visable = true;
            window.rect = new Rect(0, 0, 420, 240);
            window._alignMode = ERectAnchor.Right;
        }

        [Serializable]
        [Name("HUD窗口")]
        public class HUDWindow : BaseGUIWindow, IStart
        {
            internal MMOPlayerCreaterHUD HUD;

            internal MMOPlayerCreater creater;

            [Name("启动时对齐")]
            public bool alignOnStart = true;

            [Name("每行数目")]
            [Range(1, 10)]
            public int countPerRow = 5;

            public void Start()
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.English);
#else
                _title = CommonFun.Name(HUD.GetType(), ELanguageType.Chinese);
#endif
                if (alignOnStart) SetWindowPositionInScreen(_alignMode);
            }

            public override bool autoLayout => true;

            protected override void OnDrawContentLayout()
            {
                if (!HUD || !creater) return;

                switch (LocalCache.state)
                {
                    case ENetState.SyncRoomed:
                        {
                            bool hasPlayer = creater.player;

                            {
                                GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                                GUILayout.Label("Player", GUILayout.Width(60));
#else
                                GUILayout.Label("玩家", GUILayout.Width(60));
#endif

                                var list = creater.playerPrefabs.Where(p => p).ToList();
                                var array = list.ToList(p => p.name).ToArray();

                                var oldIndex = list.IndexOf(creater.playerPrefab);
                                var index = GUILayout.SelectionGrid(oldIndex, array, countPerRow);
                                if (oldIndex != index && !hasPlayer)
                                {
                                    creater.playerPrefab = list[index];
                                }
                                GUILayout.EndHorizontal();
                            }

                            {
                                GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                                GUILayout.Label("Position", GUILayout.Width(60));
#else
                                GUILayout.Label("位置", GUILayout.Width(60));
#endif

                                var list = creater.startPositions.Where(p => p).ToList();
                                var array = list.ToList(p => p.name).ToArray();

                                var oldIndex = list.IndexOf(creater.startPosition);
                                var index = GUILayout.SelectionGrid(oldIndex, array, countPerRow);
                                if (oldIndex != index && !hasPlayer)
                                {
                                    creater.startPosition = list[index];
                                }
                                GUILayout.EndHorizontal();
                            }

                            {
                                GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                                GUILayout.Label("Nickname", GUILayout.Width(60));
#else
                                GUILayout.Label("昵称", GUILayout.Width(60));
#endif
                                var oldIndex = creater.nickNames.IndexOf(creater.playerNickName);
                                var index = GUILayout.SelectionGrid(oldIndex, creater.nickNames.ToArray(), countPerRow);
                                if (oldIndex != index && !hasPlayer)
                                {
                                    creater.playerNickName = creater.nickNames[index];
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                                GUILayout.Label("Nickname", GUILayout.Width(60));
#else
                                GUILayout.Label("昵称", GUILayout.Width(60));
#endif
                                creater.playerNickName = GUILayout.TextField(creater.playerNickName);
                                GUILayout.EndHorizontal();
                            }

                            if (hasPlayer)
                            {
#if UNITY_WEBGL && !UNITY_EDITOR
                                if (GUILayout.Button("Delete Player"))
#else
                                if (GUILayout.Button("删除玩家"))
#endif
                                {
                                    creater.DeletePlayer();
                                }
                            }
                            else
                            {
                                GUILayout.BeginHorizontal();
#if UNITY_WEBGL && !UNITY_EDITOR
                                if (GUILayout.Button("Create Player" + creater.setting.waitedTimeString))
#else
                                if (GUILayout.Button("创建玩家" + creater.setting.waitedTimeString))
#endif
                                {
                                    creater.CreatePlayer();
                                }
                                if (GUILayout.Button("X", GUILayout.Width(30)))
                                {
                                    creater.setting.SwitchRule();
                                }
                                GUILayout.EndHorizontal();
                            }

#if UNITY_WEBGL && !UNITY_EDITOR
                            if (GUILayout.Button("Recreate Player (Random)"))
#else
                            if (GUILayout.Button("重新创建玩家(随机)"))
#endif
                            {
                                creater.RerandomCeatePlayer();
                            }

                            break;
                        }
                    default:
                        {
#if UNITY_WEBGL && !UNITY_EDITOR
                            GUILayout.Label("Can't create player without joining any valid rooms!", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
#else
                            GUILayout.Label("未进入任何有效房间,无法创建玩家！", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
#endif
                            break;
                        }
                }
            }
        }
    }
}
