using System.Linq;
using UnityEngine;
using XCSJ.Algorithms;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机实体控制器
    /// </summary>
    public abstract class BaseCameraEntityController : BaseCameraCoreController, ICameraEntityController
    {
        /// <summary>
        /// 主相机
        /// </summary>
        [Name("主相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Camera _mainCamera;

        /// <summary>
        /// 主相机:如果主相机无效，那么相机列表肯定为空；
        /// </summary>
        public Camera mainCamera
        {
            get
            {
                if (!_mainCamera)
                {
                    //第一个有效项
                    _mainCamera = cameras?.FirstOrDefault(c => c);
                }
                return _mainCamera;
            }
        }

        /// <summary>
        /// 相机列表
        /// </summary>
        [Name("相机列表")]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public Camera[] _cameras;

        /// <summary>
        /// 相机列表
        /// </summary>
        public Camera[] cameras
        {
            get
            {
                if (_cameras == null || _cameras.Length == 0)
                {
                    if (mainController)
                    {
                        this.XModifyProperty(ref _cameras, mainController.GetComponentsInChildren<Camera>(true));
                    }
                    if (_cameras == null || _cameras.Length == 0)
                    {
                        this.XModifyProperty(ref _cameras, GetComponentsInChildren<Camera>(true));
                    }
                    if (_cameras == null)
                    {
                        this.XModifyProperty(ref _cameras, Empty<Camera>.Array);
                    }
                }
                return _cameras;
            }
        }

        /// <summary>
        /// 更新相机列表
        /// </summary>
        public void UpdateCamears()
        {
            this.XModifyProperty(ref _cameras, null);
            if (cameras != null) { }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var mainController = this.mainController;
            if (mainController)
            {
                mainController.cameraEntityController = this;
            }
            if (!mainCamera)
            {
                Debug.LogWarningFormat("游戏对象[{0}]上的相机实体控制器组件，未找到任何有效的主相机！",
                   CommonFun.GameObjectToString(gameObject));
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!mainCamera) { }
        }
    }
}
