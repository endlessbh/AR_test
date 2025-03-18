using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机移动至目标前方
    /// </summary>
    [Name("相机移动至目标前方")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Move, nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    public class CameraMoveToTargetForward : BaseCameraToolController
    {
        /// <summary>
        /// 观察距离
        /// </summary>
        [Name("观察距离")]
        [Tip("相机与目标的距离", "Distance between camera and target")]
        [Min(0.1f)]
        public float _moveDistance = 24f;

        /// <summary>
        /// 最大观察距离
        /// </summary>
        [Name("最大观察距离")]
        [Tip("相机与目标的距离大于该值时，重新移动相机到目标前方", "When the distance between the camera and the target is greater than this value, move the camera to the front of the target again")]
        [Min(0.1f)]
        public float _maxDistance = 30f;

        /// <summary>
        /// 水平随机目标朝向偏转角范围
        /// </summary>
        [Name("目标朝向水平随机偏转角范围")]
        [Tip("该偏转角为目标朝向的水平随机偏转角范围", "The deflection angle is the range of horizontal random deflection angle towards the target")]
        public Vector2 _targetForwadHorizontalRandomAngleRange = new Vector2(-25f, 25f);

        /// <summary>
        /// 目标朝向水平偏转最小角范围
        /// </summary>
        [Name("目标朝向水平偏转最小角范围")]
        [Tip("目标朝向水平偏转角必须小于等于当前的X值或大于等于当前的Y值", "The horizontal deflection angle towards the target must be less than or equal to the current x value or greater than or equal to the current y value")]
        public Vector2 _targetForwadHorizontalAngleMinRange = new Vector2(-5f, 5f);

        /// <summary>
        /// 垂直随机偏移量范围
        /// </summary>
        [Name("目标朝向垂直随机偏转角范围")]
        [Tip("该偏转角为目标朝向的垂直随机偏转角范围", "The deflection angle is the range of vertical random deflection angle towards the target")]
        public Vector2 _targetForwadVerticalRandomAngleRange = new Vector2(0, 10);

        /// <summary>
        /// 启用碰撞检测
        /// </summary>
        [Name("启用碰撞检测")]
        public bool _occlusionDetectionEnable = true;

        /// <summary>
        /// 碰撞排除对象列表
        /// </summary>
        [Name("碰撞排除对象列表")]
        public List<GameObject> _occlusionExcludeGameObject = new List<GameObject>();

        /// <summary>
        /// 相机控制器位置
        /// </summary>
        protected Vector3 cameraControllerPosition => cameraController.transform.position;

        /// <summary>
        /// 相机目标位置
        /// </summary>
        protected Vector3 cameraTargetPosition => cameraTargetController.targetPosition;

        /// <summary>
        /// 相机目标旋转
        /// </summary>
        protected Quaternion cameraTargetRotation => cameraTargetController.targetRotation;

        /// <summary>
        /// 相机目标前朝向
        /// </summary>
        protected Vector3 cameraTargetForward => cameraTargetRotation * Vector3.forward;

        /// <summary>
        /// 相机目标上朝向
        /// </summary>
        protected Vector3 cameraTargetUp => cameraTargetRotation * Vector3.up;

        /// <summary>
        /// 相机目标右朝向
        /// </summary>
        protected Vector3 cameraTargetRight => cameraTargetRotation * Vector3.right;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            ChangePosition();
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            if (InValidDistance(cameraControllerPosition))
            {
                ChangePosition();
            }
        }

        private bool InValidDistance(Vector3 position)
        {
            return Occluding(position) || OutOfMaxDistance(position);
        }

        /// <summary>
        /// 目标与相机之间有障碍物
        /// </summary>
        /// <returns></returns>
        private bool Occluding(Vector3 position)
        {
            if (_occlusionDetectionEnable
                && Physics.Linecast(cameraTargetPosition, position, out RaycastHit hit)
                //&& !hit.transform.IsChildOf(target.transform)
                && !hit.collider.isTrigger)
            {
                return true;
            }
            return false;
        }

        private bool OutOfMaxDistance(Vector3 position)
        {
            var distance = Vector3.SqrMagnitude(cameraTargetPosition - position);
            return distance > _maxDistance * _maxDistance;
        }

        /// <summary>
        /// 重新获取一个位置
        /// </summary>
        protected void ChangePosition()
        {
            var randomHorizontalAngle = UnityEngine.Random.Range(_targetForwadHorizontalRandomAngleRange.x, _targetForwadHorizontalRandomAngleRange.y);
            if (randomHorizontalAngle > 0 && randomHorizontalAngle < _targetForwadHorizontalAngleMinRange.y) randomHorizontalAngle = _targetForwadHorizontalAngleMinRange.y;
            if (randomHorizontalAngle < 0 && randomHorizontalAngle > _targetForwadHorizontalAngleMinRange.x) randomHorizontalAngle = _targetForwadHorizontalAngleMinRange.x;

            var randomVerticalAngle = UnityEngine.Random.Range(_targetForwadVerticalRandomAngleRange.x, _targetForwadVerticalRandomAngleRange.y);


            var dir = Quaternion.AngleAxis(randomVerticalAngle, -cameraTargetRight) * Quaternion.AngleAxis(randomHorizontalAngle, cameraTargetUp) * cameraTargetForward;

            var position = Vector3.zero;

            // 射线检测，排除对象子物体和触发器对象
            if (Physics.Raycast(cameraTargetPosition, dir, out RaycastHit hit, _maxDistance)
                //&& !hit.transform.IsChildOf(target) 
                && !hit.collider.isTrigger)
            {
                position = hit.point - dir * 2f;
            }
            else
            {
                position = cameraTargetPosition + dir * _moveDistance;
            }

            if (!InValidDistance(position))
            {
                cameraTransformer.SetPosition(position, Space.World);
            }
        }

        private bool OcclusionExclude()
        {
            return true;
        }
    }
}
