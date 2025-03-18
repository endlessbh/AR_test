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
using XCSJ.PluginCommonUtils;
using XCSJ.PluginMMO.NetSyncs;

namespace XCSJ.PluginMMO
{
    [RequireManager(typeof(MMOManager))]
    [RequireComponent(typeof(MMOManager))]
    [Name("多人在线MMO-玩家生成器")]
    [DisallowMultipleComponent]
    public sealed class MMOPlayerCreater : InteractProvider, IAwake, IUpdate
    {
        public static MMOPlayerCreater instance
        {
            get
            {
                var manager = MMOManager.instance;
                return manager ? manager.GetComponent<MMOPlayerCreater>() : null;
            }
        }

        [Group("玩家预制件信息")]
        [Name("玩家预制件列表")]
        [SerializeField]
        private List<GameObject> _playerPrefabs = new List<GameObject>();

        public List<GameObject> playerPrefabs => _playerPrefabs;

        [Name("排序玩家预制件列表")]
        public bool sortPlayerPrefabs = false;

        [Name("玩家预制件")]
        [Tip("无效时,会尝试从玩家预制件列表中随机选择一个", "When it is invalid, it will try to select one at random from the player's preform list")]
        public GameObject playerPrefab;

        [Name("玩家")]
        [Readonly]
        public GameObject player;

        [Name("玩家网络标识")]
        [Readonly]
        public NetIdentity playerNetIdentity;

        [Group("玩家生成时位置信息")]
        [Name("起始位置列表")]
        [SerializeField]
        private List<Transform> _startPositions = new List<Transform>();

        public List<Transform> startPositions => _startPositions;

        [Name("排序起始位置列表")]
        public bool sortStartPositions = true;

        [Name("起始位置")]
        [Tip("无效时,会尝试从起始位置列表中随机选择一个", "When it is invalid, it will try to select one randomly from the starting position list")]
        public Transform startPosition;

        [Group("玩家生成时昵称信息")]
        [Name("昵称列表")]
        public List<string> nickNames = new List<string>();

        [Name("玩家昵称")]
        [Tip("为空时,会尝试从昵称列表中随机选择一个", "When empty, an attempt is made to randomly select one from the list of nicknames")]
        [SerializeField]
        private string _playerNickName = "";
        public string playerNickName
        {
            get => _playerNickName;
            set
            {
                if (_playerNickName == value) return;
                _playerNickName = value;
                if (playerNetIdentity)
                {
                    var player = playerNetIdentity.GetComponent<INetPlayer>();
                    if (player != null)
                    {
                        player.nickName = value;
                    }
                }
            }
        }

        [EndGroup]
        [Name("设置")]
        public TimeSetting setting = new TimeSetting();

        #region 玩家预制件

        public GameObject GetPlayerPrefab()
        {
            if (playerPrefab) return playerPrefab;
            RemoveInvalidPlayerPrefab();
            if (_playerPrefabs.Count == 0) return null;
            return _playerPrefabs[UnityEngine.Random.Range(0, _playerPrefabs.Count)];
        }

        private void RemoveInvalidPlayerPrefab()
        {
            if (_playerPrefabs.Count > 0)
            {
                for (int num = _playerPrefabs.Count - 1; num >= 0; num--)
                {
                    if (!_playerPrefabs[num])
                    {
                        _playerPrefabs.RemoveAt(num);
                    }
                }
            }
        }

        private void SortPlayerPrefabsIfNeed()
        {
            if (sortPlayerPrefabs) SortPlayerPrefabs();
        }

        public void SortPlayerPrefabs()
        {
            RemoveInvalidPlayerPrefab();
            _playerPrefabs.Sort((x, y) => x.name.CompareTo(y.name));
        }

        public void AddPlayerPrefab(GameObject gameObject)
        {
            if (!gameObject) return;
            _playerPrefabs.AddWithDistinct(gameObject, (x, y) => x == y);
            SortPlayerPrefabsIfNeed();
        }

        public void RemovePlayerPrefab(GameObject gameObject)
        {
            if (!gameObject) return;
            _playerPrefabs.Remove(gameObject);
        }

        #endregion

        #region 玩家起始位置

        public Transform GetStartPosition()
        {
            if (startPosition) return startPosition;
            RemoveInvalidStartPosition();
            if (_startPositions.Count == 0) return null;
            return _startPositions[UnityEngine.Random.Range(0, _startPositions.Count)];
        }

        private void RemoveInvalidStartPosition()
        {
            if (_startPositions.Count > 0)
            {
                for (int num = _startPositions.Count - 1; num >= 0; num--)
                {
                    if (!_startPositions[num])
                    {
                        _startPositions.RemoveAt(num);
                    }
                }
            }
        }

        private void SortStartPositionsIfNeed()
        {
            if (sortStartPositions) SortStartPositions();
        }

        public void SortStartPositions()
        {
            RemoveInvalidStartPosition();
            _startPositions.Sort((x, y) => x.name.CompareTo(y.name));
        }

        public void AddStartPosition(Transform transform)
        {
            if (!transform) return;
            _startPositions.AddWithDistinct(transform, (x, y) => x == y);
            SortStartPositionsIfNeed();
        }

        public void RemoveStartPosition(Transform transform)
        {
            if (!transform) return;
            _startPositions.Remove(transform);
        }

        #endregion

        public string GetNickName()
        {
            if (!string.IsNullOrEmpty(_playerNickName)) return _playerNickName;
            if (nickNames.Count > 0)
            {
                for (int num = nickNames.Count - 1; num >= 0; num--)
                {
                    if (string.IsNullOrEmpty(nickNames[num]))
                    {
                        nickNames.RemoveAt(num);
                    }
                }
            }
            if (nickNames.Count == 0) return "[空昵称]";
            return nickNames[UnityEngine.Random.Range(0, nickNames.Count)];
        }

        public void Awake()
        {
            SortPlayerPrefabsIfNeed();
            SortStartPositionsIfNeed();
        }

        public void Update()
        {
            if (LocalCache.syncEnable)
            {
                if (player)
                {
                    return;
                }
                if (setting.NeedTry())
                {
                    CreatePlayer();
                }
            }
        }

        public void CreatePlayer(string playerName)
        {
            playerNickName = playerName;
            CreatePlayer();
        }

        public void CreatePlayer()
        {
            if (player || !LocalCache.state.CanSync())
            {
                return;
            }

            if (!(LocalCache.GetLocalPlayer() is LocalCache.PlayerInfo playerInfo))
            {
                Log.WarningFormat("在[{0}]({1})生成玩家时，未找到有效的本地玩家信息！",
                    CommonFun.Name(GetType()),
                    GetType().Name);
                return;
            }

            playerPrefab = GetPlayerPrefab();
            if (!playerPrefab)
            {
                Log.WarningFormat("在[{0}]({1})生成玩家时，未找到有效的玩家预制件！",
                    CommonFun.Name(GetType()),
                    GetType().Name);
                return;
            }

            var netIdentity = playerPrefab.GetComponent<NetIdentity>();
            if (!netIdentity)
            {
                Log.WarningFormat("在[{0}]({1})生成玩家时，玩家预制件[{2}]未找到组件[3]({4})！",
                    CommonFun.Name(GetType()),
                    GetType().Name,
                    playerPrefab.name,
                    CommonFun.Name(typeof(NetIdentity)),
                    typeof(NetIdentity).Name);
                return;
            }

            _playerNickName = GetNickName();
            var netPlayer = playerPrefab.GetComponent<NetPlayer>();
            if (!netPlayer)
            {
                Log.WarningFormat("在[{0}]({1})生成玩家时，玩家预制件[{2}]未找到组件[3]({4})，可能导致玩家部分信息无法网络同步！",
                      CommonFun.Name(GetType()),
                      GetType().Name,
                      playerPrefab.name,
                      CommonFun.Name(typeof(NetPlayer)),
                      typeof(NetPlayer).Name);
            }

            playerNetIdentity = LocalCache.AddPlayer(netIdentity, newNI =>
            {

                //玩家游戏对象
                player = newNI.gameObject;

                //更新玩家位置
                startPosition = GetStartPosition();
                if (startPosition)
                {
                    player.transform.position = startPosition.transform.position;
                    player.transform.rotation = startPosition.transform.rotation;
                }

                //更新玩家属性
                var newNetPlayer = newNI.GetComponent<NetPlayer>();
                if (newNetPlayer)
                {
                    newNetPlayer.nickName = _playerNickName;
                }

            });
        }

        public void RerandomCeatePlayer()
        {
            DeletePlayer();
            playerPrefab = null;
            _playerNickName = "";
            startPosition = null;
            CreatePlayer();
        }

        public void DeletePlayer()
        {
            LocalCache.DeletePlayer();
            player = null;
        }
    }
}
