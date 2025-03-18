using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.Interfaces;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.PluginMMO.NetSyncs
{
    /// <summary>
    /// 网络玩家
    /// </summary>
    [XCSJ.Attributes.Icon(EIcon.Authentication)]
    [DisallowMultipleComponent]
    [Name("网络玩家")]
    [Tool(MMOHelper.CategoryName, MMOHelper.ToolPurpose, rootType = typeof(MMOManager))]
    public sealed class NetPlayer : NetProperty, INetPlayer
    {
        #region INetPlayer

        /// <summary>
        /// 昵称
        /// </summary>
        [Name("昵称")]
        public string nickName
        {
            get => GetProperty(nameof(nickName))?.value;
            set => SetProperty(nameof(nickName), value);
        }

        /// <summary>
        /// 本地玩家
        /// </summary>
        public NetPlayer localPlayer => LocalCache.GetLocalPlayer()?.netPlayer as NetPlayer;

        INetPlayer INetPlayer.localPlayer => LocalCache.GetLocalPlayer()?.netPlayer;

        #endregion

        #region 相机处理

        /// <summary>
        /// 查找相机
        /// </summary>
        private void FindCamera()
        {
            if (!_playerCameraController)
            {
                _playerCameraController = GetComponentInChildren<BaseCameraMainController>(true);
            }
        }

        private void CameraStartLink()
        {
            NewCameraStartLink();
        }

        private void CameraStopLink()
        {
            NewCameraStopLink();
        }

        #region 相机设置

        /// <summary>
        /// 玩家相机控制器
        /// </summary>
        [Group("相机设置", defaultIsExpanded = false)]
        [Name("玩家相机控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public BaseCameraMainController _playerCameraController;

        /// <summary>
        /// 相机目标处理规则
        /// </summary>
        [Name("相机目标处理规则")]
        [EnumPopup]
        public ECameraTargetHandleRule _cameraTargetHandleRule = ECameraTargetHandleRule.None;

        /// <summary>
        /// 相机目标处理规则
        /// </summary>
        [Name("相机目标处理规则")]
        public enum ECameraTargetHandleRule
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 无
            /// </summary>
            [Name("主目标")]
            [Tip("仅将主目标设置到玩家相机控制器对应的主目标", "Only set the main target to the main target corresponding to the player's camera controller")]
            MainTarget,

            /// <summary>
            /// 目标列表
            /// </summary>
            [Name("目标列表")]
            [Tip("仅将目标列表设置到玩家相机控制器对应的目标列表", "Only set the target list to the target list corresponding to the player's camera controller")]
            Targets,

            /// <summary>
            /// 二者
            /// </summary>
            [Name("二者")]
            [Tip("将主目标与目标列表设置到玩家相机控制器对应的主目标与目标列表", "Set the main target and target list to the main target and target list corresponding to the player's camera controller")]
            Both,
        }

        /// <summary>
        /// 相机主目标
        /// </summary>
        [Name("相机主目标")]
        [HideInSuperInspector(nameof(_cameraTargetHandleRule), EValidityCheckType.Equal, ECameraTargetHandleRule.None)]
        public Transform _cameraMainTarget;

        [Name("相机目标列表")]
        [HideInSuperInspector(nameof(_cameraTargetHandleRule), EValidityCheckType.Equal, ECameraTargetHandleRule.None)]
        public List<Transform> _cameraTargets = new List<Transform>();

        private BaseCameraMainController _prveCameraController;
        private Dictionary<ICameraTargetController, Transform> _tmpCameraMainTarget = new Dictionary<ICameraTargetController, Transform>();
        private Dictionary<ICameraTargetController, Transform[]> _tmpCameraTargets = new Dictionary<ICameraTargetController, Transform[]>();

        private void NewCameraStartLink()
        {
            _prveCameraController = null;
            _tmpCameraMainTarget.Clear();
            _tmpCameraTargets.Clear();

            if (isLocalPlayer)
            {
                //启用玩家相机控制器
                if (_playerCameraController)
                {
                    var cameraManager = CameraManager.instance;
                    if (cameraManager)
                    {
                        //缓存当前相机控制器
                        _prveCameraController = cameraManager.GetCurrentCameraController();

                        //切换到当前玩家相机控制器
                        cameraManager.SwitchCameraController(_playerCameraController, 0, null, true);
                    }
                    if (!_prveCameraController)
                    {
                        var camera = Camera.main;
                        if (camera)
                        {
                            _prveCameraController = camera.GetComponentInParent<BaseCameraMainController>();
                        }
                    }
                    //将之前的相机控制器游戏对象强制禁用
                    if (_prveCameraController)
                    {
                        _prveCameraController.gameObject.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning("前置相机控制器无效！");
                    }

                    //保证玩家相机控制器的可用性
                    _playerCameraController.enabled = true;
                    _playerCameraController.gameObject.SetActive(true);

                    //设置玩家相机控制器相关信息
                    foreach (var t in _playerCameraController.GetComponentsInChildren<ICameraTargetController>())
                    {
                        _tmpCameraMainTarget[t] = t.mainTarget;
                        _tmpCameraTargets[t] = t.targets;

                        switch (_cameraTargetHandleRule)
                        {
                            case ECameraTargetHandleRule.MainTarget:
                                {
                                    t.mainTarget = _cameraMainTarget;
                                    break;
                                }
                            case ECameraTargetHandleRule.Targets:
                                {
                                    t.targets = _cameraTargets.ToArray();
                                    break;
                                }
                            case ECameraTargetHandleRule.Both:
                                {
                                    t.mainTarget = _cameraMainTarget;
                                    t.targets = _cameraTargets.ToArray();
                                    break;
                                }
                        }
                    }
                }
            }
            else
            {
                //禁用玩家相机控制器
                if (_playerCameraController)
                {
                    var localPlayer = this.localPlayer;
                    if (localPlayer && localPlayer._playerCameraController == _playerCameraController)
                    {
                        //多个玩家共用一个相机控制器，不做处理
                    }
                    if (_playerCameraController.cameraEntityController.mainCamera == Camera.main)
                    {
                        //如果玩家控制器的主相机是当前正在使用相机，不做处理
                    }
                    else
                    {
                        _playerCameraController.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void NewCameraStopLink()
        {
            //还原玩家相机控制器相关信息
            if (_playerCameraController)
            {
                foreach (var kv in _tmpCameraMainTarget)
                {
                    kv.Key.mainTarget = kv.Value;
                }
                _tmpCameraMainTarget.Clear();
                foreach (var kv in _tmpCameraTargets)
                {
                    kv.Key.targets = kv.Value;
                }
                _tmpCameraTargets.Clear();
            }

            //还原之前相机控制器
            if (_prveCameraController)
            {
                var manager = CameraManager.instance;
                if (manager && manager.SwitchCameraController(_prveCameraController, 0, null, true))
                {
                    //切换成功
                }

                //保证之前相机控制器的可用性
                _prveCameraController.gameObject.SetActive(true);
                _prveCameraController.enabled = true;
            }
            else
            {
                //切换为初始相机控制器
                var manager = CameraManager.instance;
                if (manager)
                {
                    manager.SwitchCameraController(manager.GetInitCameraController(), 0, null, true);
                }
            }
        }

        #endregion

        #endregion

        #region MB方法

        /// <summary>
        /// 唤醒初始化
        /// </summary>
        public void Awake()
        {
            FindCamera();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            FindCamera();
            nickName = Product.Name;
        }

        #endregion

        #region INetEvent

        /// <summary>
        /// 当前对象与网络环境中的玩家产生关联时回调
        /// </summary>
        public override void OnStartPlayerLink()
        {
            base.OnStartPlayerLink();
            CameraStartLink();
        }

        /// <summary>
        /// 当前对象与网络环境中的玩家解除关联时回调
        /// </summary>
        public override void OnStopPlayerLink()
        {
            base.OnStopPlayerLink();
            CameraStopLink();
        }

        #endregion
    }
}
