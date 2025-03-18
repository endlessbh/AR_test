using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginTools.Draggers;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginTools.Items;
using System.Net.Sockets;
using XCSJ.PluginMechanicalMotion.Tools;
using XCSJ.Extension.Interactions.Tools;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XCSJ.PluginTools.GameObjects
{
    /// <summary>
    /// 游戏对象插槽缓存
    /// </summary>
    [Name("游戏对象插槽缓存")]
    [Tool("游戏对象", disallowMultiple = true, rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Put)]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class GameObjectSocketCache : Interactor
    {
        /// <summary>
        /// 距离规则
        /// </summary>
        [Name("距离规则")]
        public enum EDistanceRule
        {
            [Name("世界坐标系")]
            World,

            [Name("屏幕坐标系")]
            Screen,
        }

        /// <summary>
        /// 距离规则
        /// </summary>
        [Name("距离规则")]
        [EnumPopup]
        public EDistanceRule _distanceRule = EDistanceRule.World;

        /// <summary>
        /// 匹配距离
        /// </summary>
        [Name("匹配距离")]
        [Min(0.01f)]
        public float _matchDistance = 0.1f;

        /// <summary>
        ///  材质规则
        /// </summary>
        [Name("材质规则")]
        public enum EMaterialRule
        {
            [Name("无")]
            None,

            [Name("在距离内显示材质对象")]
            ShowMaterialInDistance,
        }

        /// <summary>
        /// 材质规则
        /// </summary>
        [Name("材质规则")]
        [EnumPopup]
        public EMaterialRule _materialRule = EMaterialRule.ShowMaterialInDistance;

        /// <summary>
        /// 指示匹配对象的材质
        /// </summary>
        [Name("匹配材质")]
        public Material _matchMaterial = null;

        private GameObject _matchMaterialObject = null;

        /// <summary>
        /// 组插槽
        /// </summary>
        private List<GameObjectSocket> groupSockets = null;

        /// <summary>
        /// 当前插槽对象
        /// </summary>
        public GameObjectSocket currentSocket
        {
            get => _currentSocket;
            private set
            {
                _currentSocket = value;

                if (_currentSocket != null)
                {
                    currentGameObject = _currentSocket.grabbable.gameObject;
                    if (currentGameObject)
                    {
                        if (!_matchMaterialObject)
                        {
                            _matchMaterialObject = Instantiate(currentGameObject, _currentSocket.pose.position, _currentSocket.pose.rotation);
                            _matchMaterialObject.GetComponentsInChildren<Renderer>().Foreach(r => r.materials = new Material[] { _matchMaterial });
                            _matchMaterialObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (_matchMaterialObject)
                    {
                        _matchMaterialObject.SetActive(false);
                        DestroyImmediate(_matchMaterialObject);
                        _matchMaterialObject = null;
                    }
                }
            }
        }
        private GameObjectSocket _currentSocket = null;

        [Readonly]
        [Name("当前插槽对应游戏对象")]
        public GameObject currentGameObject = null;

        #region 插槽管理

        /// <summary>
        /// 槽对象
        /// </summary>
        protected static List<GameObjectSocket> socketList = new List<GameObjectSocket>();

        /// <summary>
        /// 插槽匹配
        /// </summary>
        public static event Action<GameObjectSocketCache, GameObjectSocket> onSocketMatch;

        /// <summary>
        /// 注册插槽对象
        /// </summary>
        /// <param name="sockets"></param>
        public static void RegisterSockets(params GameObjectSocket[] sockets)
        {
            foreach (var s in sockets)
            {
                if (s != null)
                {
                    socketList.AddWithDistinct(s);
                }
            }
        }

        /// <summary>
        /// 注销插槽对象
        /// </summary>
        /// <param name="socket"></param>
        public static void UnregisterSockets(params GameObjectSocket[] sockets)
        {
            foreach (var s in sockets)
            {
                if (s != null)
                {
                    socketList.Remove(s);
                }
            }
        }

        /// <summary>
        /// 查找同组插槽列表，如果没有组，则加入自身
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private static List<GameObjectSocket> FindSameGroupEmptySockets(Grabbable grabbable)
        {
            var list = new List<GameObjectSocket>();
            var socket = socketList.Find(s => s.grabbable == grabbable);
            if (socket != null)
            {
                if (socket.empty) list.Add(socket);
                if (!string.IsNullOrEmpty(socket.categoryName))
                {
                    list.AddRange(socketList.FindAll(s => s != socket && s.categoryName == socket.categoryName && s.empty));
                }
            }
            return list;
        }

        /// <summary>
        /// 移除所有插槽
        /// </summary>
        private static void ClearSocket() => socketList.Clear();

        #endregion

        public void Reset()
        {
#if UNITY_EDITOR
            // 添加指示匹配的材质
            this.XModifyProperty(()=> _matchMaterial = AssetDatabase.LoadAssetAtPath("Assets/XDreamer-Assets/基础/Materials/常用/TransparentRim.mat", typeof(Material)) as Material);
#endif
        }

        protected override void OnInputInteract(InteractObject sender, InteractData interactData)
        {
            base.OnInputInteract(sender, interactData);

            if (interactData.interactState != EInteractState.Finished) return;

            if (interactData.interactor is Grabbable grabbable && grabbable)
            {
                if (grabbable._grabResultCmds.TryGetECmd(interactData.cmdName, out var eCmd))
                {
                    switch (eCmd)
                    {
                        case EGrabResultCmd.Grab:
                            {
                                groupSockets = FindSameGroupEmptySockets(grabbable);
                                currentSocket = groupSockets.FirstOrDefault();
                                break;
                            }
                        case EGrabResultCmd.Release:
                            {
                                if (currentSocket != null)
                                {
                                    if (MatchCurrentSocket(grabbable))
                                    {
                                        currentSocket.empty = false;
                                        currentSocket.MoveTargetToSocket();
                                        onSocketMatch?.Invoke(this, currentSocket);
                                    }

                                    // 根据槽状态设定目标对象是否激活
                                    currentSocket.grabbable.gameObject.SetActive(!currentSocket.empty);
                                }
                                currentSocket = null;
                                break;
                            }
                        case EGrabResultCmd.Hold:
                            {
                                // 查找最近插槽
                                if (groupSockets.Count > 1)
                                {
                                    var socket = FindNearestSocket(grabbable.position);
                                    if (socket != null && currentSocket != socket)
                                    {
                                        currentSocket?.grabbable.gameObject.SetActive(false);// 非激活当前槽对象
                                        socket?.grabbable.gameObject.SetActive(true);// 激活当前槽对象
                                        currentSocket = socket;
                                    }
                                }

                                switch (_materialRule)
                                {
                                    case EMaterialRule.ShowMaterialInDistance:
                                        {
                                            if (_matchMaterialObject && currentSocket != null && grabbable.gameObject.activeInHierarchy)
                                            {
                                                _matchMaterialObject.SetActive(MatchCurrentSocket(grabbable));
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 查找离组内最近的插槽
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private GameObjectSocket FindNearestSocket(Vector3 position)
        {
            GameObjectSocket socket = null;

            float nearestDistance = Mathf.Infinity;
            foreach (var item in groupSockets)
            {
                if (item.empty)
                {
                    float distance = Vector3.Distance(position, item.pose.position);
                    if (distance < nearestDistance)
                    {
                        socket = item;
                        nearestDistance = distance;
                    }
                }
            }
            return socket;
        }

        /// <summary>
        /// 匹配当前插槽
        /// </summary>
        /// <returns></returns>
        private bool MatchCurrentSocket(Grabbable grabbable) => InMatchDistance(grabbable.transform.position, currentSocket.pose.position);

        /// <summary>
        /// 在匹配的距离范围内
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private bool InMatchDistance(Vector3 point1, Vector3 point2)
        {
            switch (_distanceRule)
            {
                case EDistanceRule.World: return Vector3.Distance(point1, point2) < _matchDistance;
                case EDistanceRule.Screen: return Vector3.Distance(Camera.main.WorldToScreenPoint(point1), Camera.main.WorldToScreenPoint(point2)) < _matchDistance;
            }
            return false;
        }
    }

    //public interface IGameObjectSocket
    //{
    //    string categoryName { get; }

    //    Vector3 position { get; }

    //    Quaternion rotation { get; }
    //}

    /// <summary>
    /// 游戏对象插槽
    /// </summary>
    [Name("游戏对象插槽")]
    [Serializable]
    public class GameObjectSocket
    {
        /// <summary>
        /// 目标
        /// </summary>
        public Grabbable grabbable { get; private set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string categoryName { get; private set; }

        /// <summary>
        /// 插槽状态
        /// </summary>
        public virtual bool empty
        {
            get => _empty;
            set
            {
                _empty = value;

                if (!_empty)
                {
                    if (grabbable)
                    {
                        grabbable.transform.position = pose.position;
                        grabbable.transform.rotation = pose.rotation;
                    }
                }
            }
        }
        private bool _empty = true;

        public Pose pose => _pose;

        private Pose _pose;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="target"></param>
        /// <param name="categoryName"></param>
        /// <param name="categoryName"></param>
        public GameObjectSocket(Grabbable grabbable, string categoryName, Pose pose)
        {
            this.grabbable = grabbable;
            this.categoryName = categoryName;
            _pose = pose;
        }

        /// <summary>
        /// 将目标移动至槽
        /// </summary>
        public void MoveTargetToSocket() => empty = false;
    }
}
