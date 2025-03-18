using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Helpers;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机目标控制器
    /// </summary>
    public abstract class BaseCameraTargetController : BaseCameraCoreController, ICameraTargetController
    {
        #region 目标

        /// <summary>
        /// 主目标
        /// </summary>
        [Name("主目标")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _mainTarget;

        /// <summary>
        /// 主目标:设置操作支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        public Transform mainTarget
        {
            get
            {
                if (!_mainTarget)
                {
                    //第一个有效项
                    var value = _targets.FirstOrDefault(t => t);
                    if (value) mainTarget = value;
                }
                return _mainTarget;
            }
            set
            {
                this.XModifyProperty(ref _mainTarget, value, nameof(mainTarget));
            }
        }

        /// <summary>
        /// 目标列表
        /// </summary>
        [Name("目标列表")]
        public List<Transform> _targets = new List<Transform>();

        /// <summary>
        /// 目标列表
        /// </summary>
        public Transform[] targets
        {
            get => _targets.ToArray();
            set
            {
                this.XModifyProperty(() =>
                {
                    _targets.Clear();
                    if (value != null)
                    {
                        _targets.AddRange(value);
                    }
                }, nameof(targets));
            }
        }

        /// <summary>
        /// 第一目标
        /// </summary>
        public Transform firstTarget => _targets.FirstOrDefault();

        /// <summary>
        /// 末一目标
        /// </summary>
        public Transform lastTarget => _targets.LastOrDefault();

        /// <summary>
        /// 移除无效对象
        /// </summary>
        public void RemoveInvalidObject()
        {
            this.XModifyProperty(() => _targets.RemoveAll(ObjectHelper.ObjectIsNull));
        }

        #endregion

        #region 位置

        /// <summary>
        /// 目标位置
        /// </summary>
        public abstract Vector3 targetPosition { get; }

        /// <summary>
        /// 获取目标位置
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract Vector3 GetTargetPosition(int mode);

        /// <summary>
        /// 更新目标位置：会实时计算目标位置并同时更新缓存值，本值基于世界坐标；
        /// </summary>
        /// <returns></returns>
        public abstract Vector3 UpdateTargetPosition();

        /// <summary>
        /// 尝试设置目标位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public abstract bool TrySetTargetPosition(Vector3 position);

        #endregion

        #region 旋转

        /// <summary>
        /// 目标旋转角度
        /// </summary>
        public abstract Vector3 targetRotationAngle { get; }

        /// <summary>
        /// 目标旋转量
        /// </summary>
        public abstract Quaternion targetRotation { get; }

        /// <summary>
        /// 尝试设置目标旋转
        /// </summary>
        /// <param name="boundsCenter"></param>
        /// <returns></returns>
        public abstract bool TrySetTargetRotation(Quaternion rotation);

        #endregion

        #region 包围盒

        /// <summary>
        /// 目标包围盒
        /// </summary>
        public abstract Bounds targetBounds { get; }

        /// <summary>
        /// 获取目标包围盒
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public abstract Bounds GetTargetBounds(int mode);

        /// <summary>
        /// 获取目标包围盒
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public abstract Bounds GetTargetBounds(int mode, Func<Renderer, bool> func);

        /// <summary>
        /// 尝试设置目标包围盒中心
        /// </summary>
        /// <param name="boundsCenter"></param>
        /// <returns></returns>
        public abstract bool TrySetTargetBoundsCenter(Vector3 boundsCenter);

        #endregion

        /// <summary>
        /// 需要主目标
        /// </summary>
        protected virtual bool needMainTarget => true; 

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            var mainController = this.mainController;
            if (mainController)
            {
                mainController.cameraTargetController = this;
            }
            if (!mainTarget && needMainTarget)
            {
                Debug.LogWarningFormat("游戏对象[{0}]上的相机目标控制器组件，未找到任何有效的主目标！",
                   CommonFun.GameObjectToString(gameObject));
            }
        }
    }
}
