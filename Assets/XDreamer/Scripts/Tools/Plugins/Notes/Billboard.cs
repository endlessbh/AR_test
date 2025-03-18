using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Extensions;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Inputs;

namespace XCSJ.PluginTools.Notes
{
    /// <summary>
    /// 广告牌类型
    /// </summary>
    [Name("广告牌类型")]
    public enum EBillboardLookatRule
    {
        /// <summary>
        /// 无
        /// </summary>
        [Name("无")]
        [Tip("不受相机朝向影响", "Not affected by camera orientation")]
        None,

        /// <summary>
        /// 朝向相机
        /// </summary>
        [Name("朝向相机")]
        [Tip("当前游戏对象的自身坐标系Z轴始终朝向相机；类似粒子或者精灵朝向相机的效果", "The Z axis of the current game object's own coordinate system always faces the camera; Similar to the effect of particles or sprites facing the camera")]
        LookAtCamera,

        /// <summary>
        /// BB树
        /// </summary>
        [Name("BB树")]
        [Tip("当前游戏对象绕世界坐标系Y轴（与自身坐标系Y轴方向重合）旋转，且其自身坐标系X0Y面（Z轴正方向）始终朝向相机", "The current GameObject rotates around the Y-axis of the world coordinate system (coincident with the y-axis direction of its own coordinate system), and its own coordinate system x0y plane (positive direction of Z-axis) always faces the camera")]
        BillboardTree,

        /// <summary>
        /// 朝向游戏对象
        /// </summary>
        [Name("朝向游戏对象")]
        [Tip("当前游戏对象的自身坐标系Z轴始终朝向游戏对象", "The Z axis of the current GameObject's own coordinate system always faces the GameObject")]
        LookAtGameObject,

        /// <summary>
        /// 背向相机
        /// </summary>
        [Name("背向相机")]
        [Tip("当前游戏对象的自身坐标系Z轴始终背向相机；与朝向相机的方向相反", "The Z axis of the current game object's own coordinate system always faces away from the camera; Opposite the direction toward the camera")]
        BackToCamera,

        /// <summary>
        /// 背向游戏对象
        /// </summary>
        [Name("背向游戏对象")]
        [Tip("当前游戏对象的自身坐标系Z轴始终背向游戏对象；与朝向游戏对象的方向相反", "The Z axis of the current game object's own coordinate system always faces away from the game object; Opposite the direction toward the GameObject")]
        BackToGameObject,

        /// <summary>
        /// 与相机朝向对齐
        /// </summary>
        [Name("与相机朝向对齐")]
        [Tip("将当前游戏对象的自身坐标系对齐轴与相机Z轴朝向对齐", "Align the current GameObject's own coordinate system with the camera's Z axis")]
        AlginCameraDirection,

        /// <summary>
        /// 与游戏对象朝向对齐
        /// </summary>
        [Name("与游戏对象朝向对齐")]
        [Tip("将当前游戏对象的自身坐标系对齐轴与游戏对象Z轴朝向对齐", "Align the current GameObject's own coordinate system with the Z axis of the GameObject")]
        AlginGameObjectDirection,

        /// <summary>
        /// 使用射线碰撞检测器
        /// </summary>
        [Name("使用射线碰撞检测器")]
        [Tip("使用射线碰撞检测器提供的焦点作为广告牌的朝向参考点", "Use the focus provided by the ray collision detector as the orientation reference point for the billboard")]
        UseRayHitDetector,
    }

    /// <summary>
    /// 广告牌
    /// </summary>
    [Tool("标注")]
    [Name("广告牌")]
    [RequireManager(typeof(ToolsManager))]
    [Owner(typeof(ToolsManager))]
    public class Billboard : InteractProvider
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransform => this.XGetComponent(ref _targetTransform);

        /// <summary>
        /// 广告牌朝向规则
        /// </summary>
        [Name("广告牌朝向规则")]
        [EnumPopup]
        public EBillboardLookatRule _billboardLookatRule = EBillboardLookatRule.LookAtCamera;

        /// <summary>
        /// 对齐轴
        /// </summary>
        [Name("对齐轴")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.Equal, EBillboardLookatRule.None)]
        public EAlginAxis _alginAxis = EAlginAxis.Z_P;

        /// <summary>
        /// 对齐轴
        /// </summary>
        public enum EAlginAxis
        {
            [Name("X正轴")]
            X_P,

            [Name("X负轴")]
            X_N,

            [Name("Y正轴")]
            Y_P,

            [Name("Y负轴")]
            Y_N,

            [Name("Z正轴")]
            Z_P,

            [Name("Z负轴")]
            Z_N
        }

        /// <summary>
        /// 朝向对象
        /// </summary>
        [Name("朝向对象")]
        [Tip("根据广告牌类型不同，本参数有不同的解释", "This parameter has different explanations according to different types of billboards")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.NotEqual | EValidityCheckType.Or, EBillboardLookatRule.LookAtGameObject, nameof(_billboardLookatRule), EValidityCheckType.NotEqual, EBillboardLookatRule.BackToGameObject)]
        public GameObject lookAtObject;

        /// <summary>
        /// 射线碰撞检测器
        /// </summary>
        [Name("射线碰撞检测器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_billboardLookatRule), EValidityCheckType.NotEqual, EBillboardLookatRule.UseRayHitDetector)]
        public RayHitDetector _rayHitDetector;

        /// <summary>
        /// 启用自动缩放
        /// </summary>
        [Name("启用自动缩放")]
        public bool isScaleEnable = false;

        /// <summary>
        /// 缩放系数
        /// </summary>
        [Name("缩放系数")]
        [Range(0.001f, 100)]
        [HideInSuperInspector(nameof(isScaleEnable), EValidityCheckType.Equal, false)]
        public float scaleRatio = 0.1f;

        /// <summary>
        /// 初始化缩放值
        /// </summary>
        private Vector3 initScaleValue;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            initScaleValue = transform.localScale;
        }

        /// <summary>
        /// 延时更新
        /// </summary>
        protected void LateUpdate()
        {
            Vector3 pos = Vector3.zero;
            Vector3 dir = Vector3.forward;
            switch (_billboardLookatRule)
            {
                case EBillboardLookatRule.None: break;
                case EBillboardLookatRule.LookAtCamera:
                case EBillboardLookatRule.BackToCamera:
                case EBillboardLookatRule.BillboardTree:
                case EBillboardLookatRule.AlginCameraDirection:
                    {
                        var go = CameraHelperExtension.currentCamera?.gameObject;
                        if (go)
                        {
                            pos = go.transform.position;
                            dir = go.transform.forward;
                        }
                        break;
                    }
                case EBillboardLookatRule.LookAtGameObject:
                case EBillboardLookatRule.BackToGameObject:
                case EBillboardLookatRule.AlginGameObjectDirection:
                    {
                        if (lookAtObject)
                        {
                            pos = lookAtObject.transform.position;
                            dir = lookAtObject.transform.forward;
                        }
                        break;
                    }
                case EBillboardLookatRule.UseRayHitDetector:
                    {
                        pos = _rayHitDetector.focusPoint;
                        break;
                    }
                default: return;
            }

            var transform = targetTransform;
            if (isScaleEnable)
            {
                float distance = (transform.position - pos).magnitude;
                var scaleValue = Vector3.one * distance * scaleRatio;
                transform.localScale = Vector3.Scale(initScaleValue, scaleValue);
            }

            switch (_billboardLookatRule)
            {
                case EBillboardLookatRule.LookAtCamera:
                case EBillboardLookatRule.LookAtGameObject:
                case EBillboardLookatRule.UseRayHitDetector:
                    {
                        transform.LookAt(pos); 
                        break;
                    }
                case EBillboardLookatRule.BillboardTree:
                    {
                        var tmpPos = Vector3.ProjectOnPlane(pos, Vector3.up);
                        tmpPos.y += transform.position.y;
                        transform.LookAt(tmpPos);
                        break;
                    }
                case EBillboardLookatRule.BackToCamera:
                case EBillboardLookatRule.BackToGameObject:
                    {
                        transform.LookAt(transform.position * 2 - pos);
                        break;
                    }
                case EBillboardLookatRule.AlginCameraDirection:
                case EBillboardLookatRule.AlginGameObjectDirection:
                    {
                        transform.forward = dir;
                        break;
                    }
                default: break;
            }

            switch (_alginAxis)
            {
                case EAlginAxis.X_P:
                    {
                        transform.Rotate(new Vector3(0, -90, 0));
                        break;
                    }
                case EAlginAxis.X_N:
                    {
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;
                    }
                case EAlginAxis.Y_P:
                    {
                        transform.Rotate(new Vector3(90, 0, 0));
                        break;
                    }
                case EAlginAxis.Y_N:
                    {
                        transform.Rotate(new Vector3(-90, 0, 0));
                        break;
                    }
                case EAlginAxis.Z_P:
                    {
                        break;
                    }
                case EAlginAxis.Z_N:
                    {
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;
                    }
            }
        }
    }
}
