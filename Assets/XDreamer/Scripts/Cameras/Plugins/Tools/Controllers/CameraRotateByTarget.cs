using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.Extension.Base.Maths;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Controllers;
using XCSJ.PluginsCameras.Tools.Base;
using XCSJ.Tools;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginsCameras.Tools.Controllers
{
    /// <summary>
    /// 相机旋转通过目标：默认通过目标的旋转控制相机的旋转，即随着目标的旋转而旋转；
    /// </summary>
    [Name("相机旋转通过目标")]
    [Tip("默认通过目标的旋转控制相机的旋转，即随着目标的旋转而旋转；", "By default, the rotation of the camera is controlled by the rotation of the target, that is, it rotates with the rotation of the target;")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Rotate, /*nameof(CameraController),*/ nameof(CameraTransformer)/*, nameof(CameraTargetController)*/)]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    public class CameraRotateByTarget : BaseCameraRotateController
    {
        /// <summary>
        /// 上次目标旋转角度
        /// </summary>
        [Group("跟随设置", defaultIsExpanded = false)]
        [Name("上次目标旋转角度")]
        [Readonly]
        public Vector3 _lastTargetRotationAngle = new Vector3();

        /// <summary>
        /// 使用速度
        /// </summary>
        [Name("使用速度")]
        public bool _useSpeed = true;

        /// <summary>
        /// 近似零角:因浮点数误差，当相机角度与目标角度值小于本值时，则认为已经旋转到位；默认值为1/3600度，即1角秒；
        /// </summary>
        [Name("近似零角")]
        [Tip("因浮点数误差，当相机角度与目标角度值小于本值时，则认为已经旋转到位；默认值为1/3600度，即1角秒；", "Due to floating-point number error, when the camera angle and target angle value are less than this value, it is considered to have been rotated in place; The default value is 1 / 3600 degrees, i.e. 1 arcsecond;")]
        public Vector3 _approximatelyZeroAngle = new Vector3(1 / 3600f, 1 / 3600f, 1 / 3600f);

        /// <summary>
        /// 0角度
        /// </summary>
        readonly Vector3 ZeroAngle = Vector3.zero;

        /// <summary>
        /// 目标角度类型
        /// </summary>
        [Name("目标角度类型")]
        [EnumPopup]
        public ETargetAngleType _targetAngleType = ETargetAngleType.TargetForwardY;

        /// <summary>
        /// 鼠标按钮处理器
        /// </summary>
        [EndGroup(false)]
        [Name("鼠标按钮处理器")]
        public MouseButtonHandler _mouseButtonHandler = new MouseButtonHandler();

        /// <summary>
        /// 更新缓存
        /// </summary>
        private void UpdateCache()
        {
            _lastTargetRotationAngle = cameraController.cameraTargetController.targetRotationAngle;
        }

        /// <summary>
        /// 获取目标旋转角度
        /// </summary>
        /// <param name="targetRotationAngle"></param>
        /// <returns></returns>
        private bool TryGetTargetRotationAngle(out Vector3 targetRotationAngle)
        {
            switch (_targetAngleType)
            {
                case ETargetAngleType.Cache:
                    {
                        targetRotationAngle = cameraController.cameraTargetController.targetRotationAngle;
                        _offset = targetRotationAngle - _lastTargetRotationAngle;
                        return true;
                    }
                case ETargetAngleType.TargetForward:
                    {
                        targetRotationAngle = cameraController.cameraTargetController.targetRotationAngle;
                        var currrentRotationAngle = cameraController.cameraTransformer.eulerAngles;
                        _offset = (targetRotationAngle - currrentRotationAngle).WrapDegrees180();

                        if (_offset.Approximately(ZeroAngle, _approximatelyZeroAngle))
                        {
                            _lastTargetRotationAngle = targetRotationAngle;
                            break;
                        }
                        return true;
                    }
                case ETargetAngleType.TargetForwardY:
                    {
                        targetRotationAngle = cameraController.cameraTargetController.targetRotationAngle;
                        var currrentRotationAngle = cameraController.cameraTransformer.eulerAngles;
                        _offset = targetRotationAngle - currrentRotationAngle;

                        _offset.y = _offset.y.WrapDegrees180();
                        if (MathX.Approximately(_offset.y, 0, _approximatelyZeroAngle.y))
                        {
                            _lastTargetRotationAngle = targetRotationAngle;
                            break;
                        }

                        _offset.x = 0;
                        _offset.z = 0;
                        return true;
                    }
            }
            targetRotationAngle = default;
            return false;
        }

        /// <summary>
        /// 验证数据
        /// </summary>
        public void OnValidate()
        {
            _approximatelyZeroAngle = Vector3.Max(_approximatelyZeroAngle, new Vector3(0.000001f, 0.000001f, 0.000001f));
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!enabled) return;

            _lastTargetRotationAngle = cameraController.cameraTargetController.targetRotationAngle;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            if (!_mouseButtonHandler.CanContinue(UpdateCache))
            {
                return;
            }
            if (!TryGetTargetRotationAngle(out Vector3 targetRotationAngle))
            {
                return;
            }
            if (_useSpeed)
            {
                base.Update();
                //var speedRealtime = this.speedRealtime;

                _offset = Vector3.Scale(_offset, speedRealtime);
                _lastTargetRotationAngle += _offset;
            }
            else
            {
                _lastTargetRotationAngle = targetRotationAngle;
            }

            Rotate();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _rotateMode = ERotateMode.Target_World;

            _speedInfo.Reset(10f);

            _mouseButtonHandler._mouseButtons.Add(EMouseButton.Any);
        }

        /// <summary>
        /// 目标角度类型
        /// </summary>
        [Name("目标角度类型")]
        public enum ETargetAngleType
        {
            /// <summary>
            /// 无
            /// </summary>
            [Name("无")]
            None,

            /// <summary>
            /// 缓存量
            /// </summary>
            [Name("缓存量")]
            Cache,

            /// <summary>
            /// 目标前方:实现与目标的旋转量完全一致，即前方方向完全一致；相当于处于目标的正后方，并朝向目标；
            /// </summary>
            [Name("目标前方")]
            [Tip("实现与目标的旋转量完全一致，即前方方向完全一致；相当于处于目标的正后方，并朝向目标；", "The rotation amount is completely consistent with the target, that is, the front direction is completely consistent; It is equivalent to being directly behind the target and facing the target;")]
            TargetForward,

            /// <summary>
            /// 目标前方Y:实现与目标的世界Y旋转量完全一致,即前方方向在XZ面上的投影完全一致；相当于处于目标的正后方，并朝向目标；主要用于角色控制情况；
            /// </summary>
            [Name("目标前方Y")]
            [Tip("实现与目标的世界Y旋转量完全一致,即前方方向在XZ面上的投影完全一致；相当于处于目标的正后方，并朝向目标；主要用于角色控制情况；", "It is completely consistent with the world y rotation of the target, that is, the projection of the front direction on the XZ plane is completely consistent; It is equivalent to being directly behind the target and facing the target; Mainly used for role control;")]
            TargetForwardY,
        }
    }
}
