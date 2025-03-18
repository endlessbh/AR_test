using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Characters;
using XCSJ.Interfaces;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.MiniMaps
{
    /// <summary>
    /// 导航图
    /// </summary>
    [Name("导航图")]
    public class MiniMap : ViewInteractor
    {
        /// <summary>
        /// UI根节点
        /// </summary>
        [Group("基础设置", textEN = "Base Settings")]
        [Name("UI根节点")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform uiRoot = null;

        private Canvas _uiRootCanvas = null;

        [Name("导航图相机控制器")]
        [Tip("导航图观察相机, 用于导航图像的生成", "The navigation map observation camera is used for the generation of navigation images")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public MiniMapCameraController miniMapCamera = null;

        [Group("玩家设置", textEN = "Player Settings", defaultIsExpanded = false)]
        [Name("使用当前相机作为玩家")]
        public bool useCurrentCamera = true;

        /// <summary>
        /// 玩家，导航图在自动旋转模式下会追随玩家方向
        /// </summary>
        [Name("玩家")]
        [HideInSuperInspector(nameof(useCurrentCamera), EValidityCheckType.Equal, true)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _player = null;

        [Name("禁用玩家子游戏对象作为导航项")]
        [Tip("勾选时，当玩家游戏对象下的子游戏对象也是导航项时将被禁用；如果当前玩家是相机，同时禁用相机观察对象作为导航项；", "When checked, when the sub game object under the player's game object is also a navigation item, it will be disabled; If the current player is a camera, disable the camera observation object as the navigation item at the same time;")]
        public bool disablePlayerChildrenItem = true;

        public Transform player
        {
            get => _realPlayer;
            set
            {
                if (_realPlayer == value) return;

                _realPlayer = value;
                if (miniMapCamera)
                {
                    miniMapCamera.player = _realPlayer;
                }

                if (player)
                {
                    playerChildren.Clear();
                    playerChildren.AddRangeWithDistinct(player.GetComponentsInChildren<Transform>());

                    // 如果玩家有目标对象组件，将目标对象的模板设置为排除追踪的对象
                    if (player.GetComponent<ITarget>() is ITarget target)
                    {
                        if (target.target is GameObject tmpGO && tmpGO)
                        {
                            playerChildren.Add(tmpGO.transform);
                        }
                        else if (target.target is Component tmpComponent && tmpComponent)
                        {
                            playerChildren.Add(tmpComponent.transform);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 导航图真正操作的对象，在新版本相机里面，角色相机控制的对象是角色
        /// </summary>
        private Transform _realPlayer = null;

        [Name("玩家UI")]
        public MiniMapItemData _playerItemData = new MiniMapItemData();

        /// <summary>
        /// 导航图项列表管理
        /// </summary>
        [EndGroup]
        [Name("非玩家项列表")]
        public List<UIItemList> items = new List<UIItemList>();

        /// <summary>
        /// 导航图追踪项与UI 1对1 图
        /// </summary>
        private Dictionary<Transform, MiniMapItemData> _trackItemMap = new Dictionary<Transform, MiniMapItemData>();

        private HashSet<Transform> playerChildren = new HashSet<Transform>();

        private CameraManagerProvider cameraManagerProvider;

        /// <summary>
        /// 玩家、UI、玩家项和相机都有效
        /// </summary>
        /// <returns></returns>
        private bool DataValid() => player && uiRoot && uiRoot.sizeDelta.x > 0 && uiRoot.sizeDelta.y > 0 && _playerItemData.valid && miniMapCamera && miniMapCamera.linkCamera;

        /// <summary>
        /// 启用
        /// </summary>
        private void Start()
        {
            InitData();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            UpdatePlayer();
            if (!DataValid()) return;

            // 更新玩家图标
            UpdateItem(_playerItemData, player, uiRoot.sizeDelta);

            // 更新非玩家图标
            foreach (var kv in _trackItemMap)
            {
                var item = kv.Key;
                var data = kv.Value;

                // 禁用玩家子游戏对象作为导航项
                if (disablePlayerChildrenItem && (player == item || playerChildren.Contains(item)))
                {
                    data.ui.gameObject.SetActive(false);
                    continue;
                }

                // 更新导航项
                UpdateItem(data, item, uiRoot.sizeDelta);
            }

            // 更新指南针
            UpdateCompass(uiRoot.sizeDelta);
        }

        private void InitData()
        {
            // 导航图相机组件, 先找子节点，找不到再全局查找
            if (!miniMapCamera)
            {
                miniMapCamera = GetComponentInChildren<MiniMapCameraController>();

                if (!miniMapCamera)
                {
                    miniMapCamera = FindObjectOfType<MiniMapCameraController>();
                }
            }

            UpdatePlayer();

            // 使用导航图存储项数据，创建运行时追踪对象
            items.ForEach(i => i.transforms.ForEach(t => CreateItem(i.itemData, t)));

            if (uiRoot) _uiRootCanvas = uiRoot.GetComponentInParent<Canvas>();

            if (CameraManager.instance)
            {
                cameraManagerProvider = CameraManager.instance.cameraManagerProvider as CameraManagerProvider;
            }
        }

        /// <summary>
        /// 最后新版相机控制器
        /// </summary>
        private Transform _lastNewCameraControllerTransform = null;

        private void UpdatePlayer()
        {
            if (useCurrentCamera)
            {
                var cam = CameraHelperExtension.currentCamera;
                if (cam && cam != miniMapCamera.linkCamera)
                {
                    if (cameraManagerProvider && cameraManagerProvider.currentCameraController)
                    {
                        if (_lastNewCameraControllerTransform != cameraManagerProvider.currentCameraController)
                        {
                            _lastNewCameraControllerTransform = cameraManagerProvider.currentCameraController.transform;
                            // 角色相机情况下，使用角色控制器转换
                            var xcc = _lastNewCameraControllerTransform.GetComponentInParent<XCharacterController>();
                            player = xcc ? xcc.transform : _lastNewCameraControllerTransform;
                        }
                    }
                    else // 旧版相机
                    {
                        _lastNewCameraControllerTransform = null;
                        player = cam.transform;
                    }
                }
            }
            else
            {
                _lastNewCameraControllerTransform = null;
                player = _player;
            }
        }

        /// <summary>
        /// 获取导航图点击点UI本地坐标
        /// </summary>
        /// <param name="clickPoint"></param>
        /// <param name="uiLocalPointNormalized"></param>
        /// <returns></returns>
        public bool TryGetClickPointInMap(Vector3 clickPoint, out Vector2 uiLocalPointNormalized)
        {
            var rs = RectTransformUtility.RectangleContainsScreenPoint(uiRoot, clickPoint, _uiRootCanvas.worldCamera);

            if (rs && RectTransformUtility.ScreenPointToLocalPointInRectangle(uiRoot, clickPoint, _uiRootCanvas.worldCamera, out uiLocalPointNormalized))
            {
                uiLocalPointNormalized = Rect.PointToNormalized(uiRoot.rect, uiLocalPointNormalized);

                switch (_minimapType)
                {
                    case CompassItemData.EMiniMapType.Circle:
                        {
                            // 超出导航图圆形范围
                            var halfPoint = uiLocalPointNormalized - Vector2.one / 2;
                            if (halfPoint.magnitude > 0.5)
                            {
                                rs = false;
                            }
                            break;
                        }
                }
            }
            else
            {
                uiLocalPointNormalized = Vector2.zero;
            }

            return rs;
        }

        #region 导航项操作

        /// <summary>
        /// 更新项的位置与旋转
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="obj"></param>
        private void UpdateItem(MiniMapItemData itemData, Transform obj, Vector2 uiSize)
        {
            if (!itemData.valid || !obj) return;

            // 同步UI的激活与追踪对象的激活
            itemData.ui.gameObject.SetActive(obj.gameObject.activeInHierarchy);
            if (itemData.ui.gameObject.activeSelf)
            {
                // 更新位置
                var point = (Vector2)miniMapCamera.linkCamera.WorldToViewportPoint(obj.position + itemData.positionOffset);
                point -= Vector2.one / 2;
                point.Scale(uiSize);
                itemData.ui.anchoredPosition = point;

                itemData.UpdateRotation(obj);
            }
        }

        /// <summary>
        /// 创建追踪项对应真实的UI
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public RectTransform CreateItem(RectTransform ui, Transform item) => CreateItem(ui, item, Vector3.zero);

        /// <summary>
        /// 创建追踪项对应真实的UI
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public RectTransform CreateItem(MiniMapItemData itemData, Transform item) => CreateItem(itemData.ui, item, itemData.positionOffset, itemData.followRotation, itemData.rotationYOffset);

        /// <summary>
        /// 创建追踪项对应真实的UI
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="item"></param>
        /// <param name="positionOffset"></param>
        /// <param name="followRotation"></param>
        /// <param name="rotationYOffset"></param>
        /// <returns></returns>
        public RectTransform CreateItem(RectTransform ui, Transform item, Vector3 positionOffset, bool followRotation = false, float rotationYOffset = 0)
        {
            if (!ui || !item) return null;

            if (_trackItemMap.ContainsKey(item)) return null;

            var cloneUI = CloneUI(ui);
            if (cloneUI)
            {
                _trackItemMap.Add(item, new MiniMapItemData(cloneUI, positionOffset, followRotation, rotationYOffset));
            }
            return cloneUI;
        }

        /// <summary>
        /// 销毁导航项
        /// </summary>
        public bool DestroyItem(Transform item)
        {
            var rs = _trackItemMap.TryGetValue(item, out MiniMapItemData itemData);
            if (rs)
            {
                _trackItemMap.Remove(item);
                if (itemData.ui)
                {
                    itemData.ui.gameObject.SetActive(false);
                    Destroy(itemData.ui);
                }
            }
            return rs;
        }

        /// <summary>
        /// 获取追踪项UI
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public RectTransform GetItemUI(Transform item)
        {
            RectTransform ui = null;
            if (item)
            {
                _trackItemMap.TryGetValue(item, out MiniMapItemData itemData);
                ui = itemData.ui;
            }
            return ui;
        }

        /// <summary>
        /// 克隆UI
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        private RectTransform CloneUI(RectTransform ui)
        {
            var cloneGameObject = ui.gameObject.XCloneObject();
            if (cloneGameObject)
            {
                cloneGameObject.gameObject.XSetParent(ui.transform.parent);
                cloneGameObject.gameObject.XSetUniqueName(ui.name);
                cloneGameObject.transform.localScale = ui.localScale;
            }

            return cloneGameObject.GetComponent<RectTransform>();
        }

        /// <summary>
        /// 导航图项列表
        /// </summary>
        [Serializable]
        [Name("导航图项列表")]
        public class UIItemList
        {
            /// <summary>
            /// 项数据
            /// </summary>
            [Name("导航数据")]
            public MiniMapItemData itemData;

            [Name("追踪项列表")]
            [OnlyMemberElements]
            public List<Transform> transforms = new List<Transform>();
        }

        #endregion

        #region 指南针

        /// <summary>
        /// 方位参考对象
        /// </summary>
        [Group("方位", textEN = "Direction", defaultIsExpanded = false)]
        [Name("方位参考对象")]
        [Tip("采用参考对象的Z轴朝向作为参考方向", "The z-axis orientation of the reference object is used as the reference direction")]
        public Transform _directionReference = null;

        /// <summary>
        /// 方位
        /// </summary>
        [Name("方位")]
        [HideInSuperInspector(nameof(_directionReference), EValidityCheckType.NotNull)]
        public Vector3PropertyValue _directionVector3;

        /// <summary>
        /// 获取方位
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDirection()
        {
            return _directionReference? _directionReference.forward: _directionVector3.GetValue();
        }

        /// <summary>
        /// 获取方位
        /// </summary>
        /// <returns></returns>
        public float GetDirectionYAngle()
        {
            var dir = GetDirection();
            dir.y = 0;
            return Vector3.SignedAngle(dir, Vector3.forward, Vector3.up);
        }

        [Name("导航图类型")]
        [EnumPopup]
        [HideInSuperInspector()]
        public CompassItemData.EMiniMapType _minimapType = CompassItemData.EMiniMapType.Circle;

        [Name("方位参考对象")]
        [HideInSuperInspector()]
        public Transform _compass = null;

        [Name("方位")]
        [HideInSuperInspector()]
        public Vector3 _compassDirection = Vector3.forward;

        [Name("UI项")]
        [HideInSuperInspector()]
        public MiniMapItemData _compassItemData = new MiniMapItemData();

        /// <summary>
        /// 指南针
        /// </summary>
        /// <param name="uiSize"></param>
        private void UpdateCompass(Vector2 uiSize)
        {
            if (!_compassItemData.valid) return;

            // 更新位置
            var dir = _compass ? _compass.forward : _compassDirection;
            dir = Quaternion.Euler(0, -miniMapCamera.linkCamera.transform.eulerAngles.y, 0) * dir;
            var point = new Vector2(dir.x, dir.z);
            point.Normalize();

            switch (_minimapType)
            {
                case CompassItemData.EMiniMapType.Circle:
                    {
                        point.Scale(uiSize);
                        _compassItemData.ui.anchoredPosition = point / 2;
                        break;
                    }
                case CompassItemData.EMiniMapType.Rect:
                    {
                        if (point.x == 0 || point.y == 0)
                        {
                            point.Scale(uiSize);
                        }
                        else
                        {
                            // 比较斜率, 当前向量斜率大于地图矩形斜率
                            var k = point.y / point.x;
                            if (Mathf.Abs(k) > Mathf.Abs(uiSize.y / uiSize.x))
                            {
                                point.y = point.y > 0 ? uiSize.y : -uiSize.y;
                                point.x = point.y / k;
                            }
                            else
                            {
                                point.x = point.x > 0 ? uiSize.x : -uiSize.x;
                                point.y = point.x * k;
                            }
                        }
                        _compassItemData.ui.anchoredPosition = point / 2;
                        break;
                    }
            }

            _compassItemData.UpdateRotation(_compass);
        }

        #endregion

    }
}
