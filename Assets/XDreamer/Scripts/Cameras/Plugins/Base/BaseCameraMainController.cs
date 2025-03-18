using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Components;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Interfaces;
using XCSJ.Maths;
using XCSJ.PluginCamera;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Base.Kernel;
using XCSJ.Tools;

namespace XCSJ.PluginsCameras.Base
{
    /// <summary>
    /// 基础相机主控制器
    /// </summary>
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [RequireManager(typeof(CameraManager))]
    [Owner(typeof(CameraManager))]
    public abstract class BaseCameraMainController : BaseMainController, ICameraMainController, ICameraOwner, IComponentHasOwner<ICameraOwner>, ITool, ITarget
    {
        /// <summary>
        /// 获取相机控制器枚举迭代器
        /// </summary>
        public static IEnumerable<BaseCameraMainController> cameraControllers => ComponentCache.GetComponents<BaseCameraMainController>(true);

        #region 相机拥有者

        /// <summary>
        /// 相机拥有者：如存在父级拥有者，则返回父级拥有者；否者返回当前组件（即当前相机控制器的拥有者为自身）
        /// </summary>
        public ICameraOwner cameraOwner => this.GetParentOrDirectOwner<ICameraOwner>();

        /// <summary>
        /// 拥有者游戏对象
        /// </summary>
        public GameObject ownerGameObject => gameObject;

        /// <summary>
        /// 拥有者
        /// </summary>
        public ICameraOwner owner => cameraOwner;

        IComponentOwner IHasOwner<IComponentOwner>.owner => cameraOwner;

        IOwner IHasOwner.owner => cameraOwner;

        #endregion

        #region 相机实体

        /// <summary>
        /// 相机实体控制器
        /// </summary>
        [Group("相机实体", defaultIsExpanded = false)]
        [Name("相机实体控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseCameraEntityController _cameraEntityController;

        /// <summary>
        /// 相机实体控制器
        /// </summary>
        public BaseCameraEntityController cameraEntityController
        {
            get => this.XGetComponentInChildren(ref _cameraEntityController);
            set => this.XModifyProperty(ref _cameraEntityController, value);
        }

        ICameraEntityController ICameraMainControllerMembers.cameraEntityController => _cameraEntityController;

        #endregion

        #region 变换

        /// <summary>
        /// 相机变换器
        /// </summary>
        [Group("变换(移动旋转)", defaultIsExpanded = false, textEN = "Transformer (Move Rotate)")]
        [Name("相机变换器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseCameraTransformer _cameraTransformer;

        /// <summary>
        /// 相机变换器
        /// </summary>
        public BaseCameraTransformer cameraTransformer
        {
            get => this.XGetComponentInChildren(ref _cameraTransformer);
            set => this.XModifyProperty(ref _cameraTransformer, value);
        }

        ITransformer ITransformMainControllerMembers.transformer => cameraTransformer;

        ICameraTransformer ICameraMainControllerMembers.cameraTransformer => cameraTransformer;

        #endregion

        #region 移动

        /// <summary>
        /// 移动速度系数
        /// </summary>
        public Vector3 moveSpeedCoefficient
        {
            get => cameraTransformer.moveSpeedCoefficient;
            set => cameraTransformer.moveSpeedCoefficient = value;
        }

        /// <summary>
        /// 移动阻尼系数
        /// </summary>
        public float moveDampingCoefficient
        {
            get => cameraTransformer.moveDampingCoefficient;
            set => cameraTransformer.moveDampingCoefficient = value;
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="value"></param>
        /// <param name="moveMode"></param>
        public void Move(Vector3 value, int moveMode)
        {
            try
            {
                cameraTransformer.Move(value, moveMode);
            }
            catch { }
        }

        #endregion

        #region 旋转

        /// <summary>
        /// 旋转速度系数
        /// </summary>
        public Vector3 rotateSpeedCoefficient
        {
            get => cameraTransformer.rotateSpeedCoefficient;
            set=> cameraTransformer.rotateSpeedCoefficient=value;
        }

        /// <summary>
        /// 旋转阻尼系数
        /// </summary>
        public float rotateDampingCoefficient
        {
            get => cameraTransformer.rotateDampingCoefficient;
            set=> cameraTransformer.rotateDampingCoefficient=value;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="rotateMode"></param>
        public void Rotate(Vector3 value, int rotateMode)
        {
            try
            {
                cameraTransformer.Rotate(value, rotateMode);
            }
            catch { }
        }

        #endregion

        #region 目标

        /// <summary>
        /// 相机目标控制器
        /// </summary>
        [Group("目标", defaultIsExpanded = true)]
        [Name("相机目标控制器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public BaseCameraTargetController _cameraTargetController;

        /// <summary>
        /// 相机目标控制器
        /// </summary>
        public BaseCameraTargetController cameraTargetController
        {
            get => this.XGetComponentInChildren(ref _cameraTargetController);
            set => this.XModifyProperty(ref _cameraTargetController, value);
        }

        ICameraTargetController ICameraMainControllerMembers.cameraTargetController => cameraTargetController;

        /// <summary>
        /// 主目标
        /// </summary>
        public Transform mainTarget { get => cameraTargetController.mainTarget; set => cameraTargetController.mainTarget = value; }

        object ITarget.target
        {
            get => mainTarget;
            set
            {
                if (value == null)
                {
                    mainTarget = null;
                }
                else if (value is Component component)
                {
                    mainTarget = component.transform;
                }
                else if (value is GameObject gameObject)
                {
                    mainTarget = gameObject.transform;
                }
            }
        }

        /// <summary>
        /// 尝试设置目标并同步变换
        /// </summary>
        /// <param name="target"></param>
        /// <param name="syncTransform"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TrySyncTransfrom(Transform target, Transform syncTransform, float time = 1f)
        {
            if (target)
            {
                mainTarget = target;
            }
            return TrySyncTransfrom(syncTransform, time);
        }

        /// <summary>
        /// 尝试同步变换
        /// </summary>
        /// <param name="syncTransform"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TrySyncTransfrom(Transform syncTransform, float time = 1f)
        {
            if (!syncTransform) return false;

            cameraTransformer.TransformTo(syncTransform.position, syncTransform.rotation, time);

            return true;
        }

        /// <summary>
        /// 尝试设置目标并变换到
        /// </summary>
        /// <param name="target"></param>
        /// <param name="lookAtPosition"></param>
        /// <param name="cameraRotation"></param>
        /// <param name="lookAtDistance"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TryTransformToByLookAt(Transform target, Vector3 lookAtPosition, Vector3 cameraRotation, float lookAtDistance, float time = 1f)
        {
            if (target)
            {
                mainTarget = target;
            }
            return TryTransformToByLookAt(lookAtPosition, cameraRotation, lookAtDistance, time);
        }

        /// <summary>
        /// 尝试变换到
        /// </summary>
        /// <param name="lookAtPosition"></param>
        /// <param name="cameraRotation"></param>
        /// <param name="lookAtDistance"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TryTransformToByLookAt(Vector3 lookAtPosition, Vector3 cameraRotation, float lookAtDistance, float time = 1f)
        {
            var dstRotation = Quaternion.Euler(cameraRotation);
            var dir = Quaternion.Euler(cameraRotation) * Vector3.forward;
            var dstPosition = lookAtPosition - dir.normalized * lookAtDistance;

            cameraTransformer.TransformTo(dstPosition, dstRotation, time);
            return true;
        }

        /// <summary>
        /// 尝试聚焦目标
        /// </summary>
        /// <param name="cameraFocusTargetMode"></param>
        /// <param name="cameraFocusPosition"></param>
        /// <param name="distanceScale"></param>
        /// <returns></returns>
        public virtual bool TryFocusTarget(ECameraFocusTargetMode cameraFocusTargetMode = ECameraFocusTargetMode.Linear, EBoundsAnchor cameraFocusPosition = EBoundsAnchor.RightTopFront, float distanceScale = 1.732f, float time = 1f)
        {
            var targetTransform = mainTarget;
            if (!targetTransform) return false;

            var targetPosition = cameraTargetController.UpdateTargetPosition();
            Vector3 dir;
            Vector3 srcPosition;

            var cameraTransformer = this.cameraTransformer;
            switch (cameraFocusTargetMode)
            {
                case ECameraFocusTargetMode.Linear:
                    {
                        srcPosition = cameraTransformer.position;
                        dir = srcPosition - targetPosition;
                        if (MathX.ApproximatelyZero(dir.sqrMagnitude)) dir = -cameraTransformer.forward;
                        break;
                    }
                case ECameraFocusTargetMode.TryKeepRotation:
                    {
                        srcPosition = cameraTransformer.position;
                        dir = -cameraTransformer.forward;
                        break;
                    }
                case ECameraFocusTargetMode.BoundsAnchor:
                    {
                        if (cameraFocusPosition == EBoundsAnchor.None) return false;
                        srcPosition = cameraTransformer.position;
                        dir = cameraFocusPosition.GetAnchorPoition(Vector3.zero, Vector3.one);
                        break;
                    }
                default: return false;
            }

            var distance = distanceScale;
            if (CommonFun.GetBounds(out var bounds, targetTransform)) distance *= bounds.size.magnitude;

            var srcRotation = cameraTransformer.rotation;
            var dstPosition = targetPosition + dir.normalized * distance;

            //临时修改相机的位置
            cameraTransformer.position = dstPosition;

            //临时修改相机的朝向
            cameraTransformer.LookAt(targetPosition);
            var dstRotation = cameraTransformer.rotation;

            cameraTransformer.position = srcPosition;
            cameraTransformer.rotation = srcRotation;

            cameraTransformer.TransformTo(dstPosition, dstRotation, time);

            return true;
        }

        /// <summary>
        /// 尝试聚焦目标：会修改主目标
        /// </summary>
        /// <param name="target">如果目标无效则聚焦原目标</param>
        /// <param name="cameraFocusTargetMode"></param>
        /// <param name="cameraFocusPosition"></param>
        /// <param name="distanceScale"></param>
        /// <returns></returns>
        public virtual bool TryFocusTarget(Transform target, ECameraFocusTargetMode cameraFocusTargetMode = ECameraFocusTargetMode.Linear, EBoundsAnchor cameraFocusPosition = EBoundsAnchor.RightTopFront, float distanceScale = 1.732f, float time = 1f)
        {
            if (target)
            {
                mainTarget = target;
            }
            return TryFocusTarget(cameraFocusTargetMode, cameraFocusPosition, distanceScale, time);
        }

        #endregion

        #region MB方法

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!enabled) return;
            CameraControllerEvent.CallOnEnabled(this);
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            CameraControllerEvent.CallOnDisabled(this);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (cameraEntityController) { }
            if (cameraTransformer) { }
            if (cameraTargetController) { }
        }

        #endregion
    }

    /// <summary>
    /// 相机聚焦目标模式属性值
    /// </summary>
    [Name("相机聚焦目标模式属性值")]
    [Serializable]
    [PropertyType(typeof(ECameraFocusTargetMode))]
    public class ECameraFocusTargetModePropertyValue : EnumPropertyValue<ECameraFocusTargetMode>
    {
    }

    /// <summary>
    /// 相机聚焦目标模式
    /// </summary>
    [Name("相机聚焦目标模式")]
    public enum ECameraFocusTargetMode
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        None,

        /// <summary>
        /// 线性
        /// </summary>
        [Name("线性")]
        Linear,

        /// <summary>
        /// 尝试保持旋转
        /// </summary>
        [Name("尝试保持旋转")]
        TryKeepRotation,

        /// <summary>
        /// 包围盒锚点
        /// </summary>
        [Name("包围盒锚点")]
        BoundsAnchor,
    }
}
