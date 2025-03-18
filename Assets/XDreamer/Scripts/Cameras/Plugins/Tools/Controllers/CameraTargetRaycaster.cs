using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机目标射线检测器:以相机目标位置为射线原点，朝向相机变换位置的方向为射线方向构建的射线检测器
    /// </summary>
    [Name("相机目标射线检测器")]
    [Tip("以相机目标位置为射线原点，朝向相机变换位置的方向为射线方向构建的射线检测器", "The ray detector is constructed with the camera target position as the ray origin and the direction towards the camera transformation position as the ray direction")]
    [Tool(CameraHelperExtension.ControllersCategoryName, /*nameof(CameraController), nameof(CameraTransformer), */nameof(CameraTargetController))]
    [XCSJ.Attributes.Icon(EIcon.AR)]
    [DisallowMultipleComponent]
    public class CameraTargetRaycaster : BaseCameraLimiter
    {
        /// <summary>
        /// 距离：射线检测器的检测距离
        /// </summary>
        [Name("距离")]
        [Tip("射线检测器的检测距离", "Detection distance of X-ray detector")]
        [Readonly]
        public float _distance = 0;

        /// <summary>
        /// 距离
        /// </summary>
        public float distance
        {
            get => _distance;
            set
            {
                _distance = Mathf.Clamp(value, _distanceRange.x, _distanceRange.y);
            }
        }

        /// <summary>
        /// 有命中:标识射线检测是否命中
        /// </summary>
        [Name("有命中")]
        [Tip("标识射线检测是否命中", "Identify whether the radiographic test is hit")]
        [Readonly]
        public bool _hasHit = false;

        /// <summary>
        /// 图层蒙版:射线检测的图层蒙版
        /// </summary>
        [Name("图层蒙版")]
        [Tip("射线检测的图层蒙版", "Layer mask for radiographic testing")]
        public LayerMask _layerMask = 1;

        /// <summary>
        /// 距离区间:射线检测器的检测距离区间
        /// </summary>
        [Name("距离区间")]
        [Tip("射线检测器的检测距离区间", "Detection distance range of X-ray detector")]
        [LimitRange(0.00001f, 100)]
        public Vector2 _distanceRange = new Vector2(0.01f, 100f);

        /// <summary>
        /// 当未命中时恢复距离
        /// </summary>
        [Name("当未命中时恢复距离")]
        public bool _recoverDistanceWhenNotHit = true;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            if (!enabled) return;
            base.OnEnable();

            UpdateDistance();

            BaseCameraTransformer.onBeforeTranslate += OnBeforeTranslate;
            BaseCameraTransformer.onAfterTranslate += OnAfterTranslate;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            BaseCameraTransformer.onBeforeTranslate -= OnBeforeTranslate;
            BaseCameraTransformer.onAfterTranslate -= OnAfterTranslate;
        }

        float lastDistance = 0;

        private void OnBeforeTranslate(BaseCameraTransformer cameraTransformer, Vector3 translation, Space relativeTo)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_hasHit)
            {
                lastDistance = GetDistance();
            }
        }

        private void OnAfterTranslate(BaseCameraTransformer cameraTransformer, Vector3 translation, Space relativeTo)
        {
            if (cameraTransformer != this.cameraTransformer) return;
            if (_hasHit)
            {
                distance += (GetDistance() - lastDistance);
            }
            else
            {
                UpdateDistance();
            }
        }

        private float GetDistance()
        {
            var position = cameraTransformer.position;
            var targetPosition = cameraController.cameraTargetController.targetPosition;
            return (position - targetPosition).magnitude;
        }

        private void UpdateDistance()
        {
            distance = GetDistance();
        }

        /// <summary>
        /// 延后更新
        /// </summary>
        public void LateUpdate()
        {
            var position = cameraTransformer.position;
            var targetPosition = cameraController.cameraTargetController.targetPosition;
            var dir = position - targetPosition;

            float distance;
            if (_recoverDistanceWhenNotHit)
            {
                distance = this.distance;
            }
            else
            {
                distance = dir.magnitude;
            }

            //射线检测
            if (Physics.Raycast(targetPosition, dir, out RaycastHit hitInfo, distance, _layerMask))
            {
                cameraTransformer.position = hitInfo.point;
                _hasHit = true;
            }
            else
            {
                _hasHit = false;
                if (_recoverDistanceWhenNotHit)//还原位置
                {
                    cameraTransformer.position = targetPosition + dir.normalized * distance;
                }
            }
        }
    }
}
