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
    /// 相机角度限定器
    /// </summary>
    [Name("相机角度限定器")]
    [Tool(CameraHelperExtension.ControllersCategoryName_Rotate,/* nameof(CameraController), */nameof(CameraTransformer))]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    public class CameraAngleLimiter : BaseCameraLimiter
    {
        /// <summary>
        /// 360度
        /// </summary>
        public const float _360 = 359.9999f;

        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.World;

        /// <summary>
        /// X限定区间:绕X轴旋转角度区间范围（-360, 360）的左开右开区间
        /// </summary>
        [Name("X限定区间")]
        [Tip("绕X轴旋转角度区间范围（-360, 360）的左开右开区间", "Left open right open interval of rotation angle range (- 360, 360) around X axis")]
        [LimitRange(-_360, _360)]
        public Vector2 _xLimitRange = new Vector2(-_360, _360);

        /// <summary>
        /// Y限定区间:绕Y轴旋转角度区间范围（-360, 360）的左开右开区间
        /// </summary>
        [Name("Y限定区间")]
        [Tip("绕Y轴旋转角度区间范围（-360, 360）的左开右开区间", "Left open right open interval of rotation angle range (- 360, 360) around Y axis")]
        [LimitRange(-_360, _360)]
        public Vector2 _yLimitRange = new Vector2(-_360, _360);

        /// <summary>
        /// Z限定区间:绕Z轴旋转角度区间范围（-360, 360）的左开右开区间
        /// </summary>
        [Name("Z限定区间")]
        [Tip("绕Z轴旋转角度区间范围（-360, 360）的左开右开区间", "Left open right open interval of rotation angle range (- 360, 360) around Z axis")]
        [LimitRange(-_360, _360)]
        public Vector2 _zLimitRange = new Vector2(-_360, _360);

        /// <summary>
        /// 验证
        /// </summary>
        public void OnValidate()
        {
            ValidateLimitRange();
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start()
        {
            ValidateLimitRange();
        }

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            if (!enabled) return;
            base.OnEnable();

            BaseCameraTransformer.onAfterRotate += OnAfterRotate;

            BaseCameraTransformer.onBeforeRotateAround += OnBeforeRotateAround;
            BaseCameraTransformer.onAfterRotateAround += OnAfterRotateAround;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            BaseCameraTransformer.onAfterRotate -= OnAfterRotate;

            BaseCameraTransformer.onBeforeRotateAround -= OnBeforeRotateAround;
            BaseCameraTransformer.onAfterRotateAround -= OnAfterRotateAround;
        }

        #region 旋转Rotate纠正机制

        private void OnAfterRotate(BaseCameraTransformer cameraTransformer, Vector3 eulers, Space relativeTo)
        {
            if (this.cameraTransformer != cameraTransformer) return;

            cameraTransformer.SetRotation(_spaceType, Quaternion.Euler(LimitAngle(cameraTransformer.GetRotation(_spaceType).eulerAngles)));
        }

        #endregion

        #region 绕物旋转RotateAround纠正机制

        ///// <summary>
        ///// 绕物旋转纠正规则
        ///// </summary>
        //[Name("绕物旋转纠正规则")]
        //[EnumPopup]
        //public ERotateAroundCorrectRule _rotateAroundCorrectRule = ERotateAroundCorrectRule.InLimitRange;

        /// <summary>
        /// 最小检测角度
        /// </summary>
        [Name("最小检测角度")]
        [Range(1f / 7200, 1f)]
        public float _minDetectionAngle = 0.1f;

        /// <summary>
        /// 绕物旋转纠正规则
        /// </summary>
        //[Name("绕物旋转纠正规则")]
        //public enum ERotateAroundCorrectRule
        //{
        //    /// <summary>
        //    /// 无
        //    /// </summary>
        //    [Name("无")]
        //    None,

        //    /// <summary>
        //    /// 在限定区间
        //    /// </summary>
        //    [Name("在限定区间")]
        //    InLimitRange,
        //}

        private bool validRotateAround = false;
        private Vector3 positionRotateAround;
        private Quaternion rotationRotateAround;

        private void OnBeforeRotateAround(BaseCameraTransformer cameraTransformer, Vector3 point, Vector3 axis, float angle)
        {
            if (this.cameraTransformer != cameraTransformer)
            {
                validRotateAround = false;
                return;
            }

            rotationRotateAround = cameraTransformer.GetRotation(_spaceType);
            positionRotateAround = cameraTransformer.position;

            validRotateAround = InLimitRange(rotationRotateAround.eulerAngles);
            // && _rotateAroundCorrectRule == ERotateAroundCorrectRule.InLimitRange
            if (!validRotateAround)//以下的纠正算法，对世界坐标系有效、本地坐标系会有问题！
            {
                var currentEulerAngles = rotationRotateAround.eulerAngles;
                var dstEulerAngles = LimitAngle(currentEulerAngles);
                var offset = dstEulerAngles - currentEulerAngles;

                cameraTransformer.RotateAround(point, Vector3.right, offset.x, false);
                cameraTransformer.RotateAround(point, Vector3.up, offset.y, false);
                cameraTransformer.RotateAround(point, Vector3.forward, offset.z, false);

                rotationRotateAround = cameraTransformer.GetRotation(_spaceType);
                positionRotateAround = cameraTransformer.position;
                validRotateAround = InLimitRange(rotationRotateAround.eulerAngles);
                //Debug.Log("Before RotateAround:" + offset + "==>" + validRotateAround);
            }
        }

        private void OnAfterRotateAround(BaseCameraTransformer cameraTransformer, Vector3 point, Vector3 axis, float angle)
        {
            if (!validRotateAround) return;
            if (this.cameraTransformer != cameraTransformer) return;

            if (!InLimitRange(cameraTransformer.GetRotation(_spaceType).eulerAngles))
            {
                angle = LimitAngleTo180(angle);//需要对旋转角度做纠正
                var half = angle / 2;
                cameraTransformer.RotateAround(point, axis, -half, false);
                if (RotateAround(cameraTransformer, point, axis, half, _minDetectionAngle))
                {
                    //允许
                }
                else
                {
                    //还原
                    cameraTransformer.position = positionRotateAround;
                    cameraTransformer.SetRotation(_spaceType, rotationRotateAround);
                }
            }
        }

        private bool RotateAround(BaseCameraTransformer cameraTransformer, Vector3 point, Vector3 axis, float angle, float min)
        {
            var half = angle / 2;
            if (InLimitRange(cameraTransformer.GetRotation(_spaceType).eulerAngles))
            {
                if (Mathf.Abs(angle) <= min) return true;
                cameraTransformer.RotateAround(point, axis, half, false);
                return RotateAround(cameraTransformer, point, axis, half, min);
            }
            else
            {
                if (Mathf.Abs(angle) <= min) return false;
                cameraTransformer.RotateAround(point, axis, -half, false);
                return RotateAround(cameraTransformer, point, axis, half, min);
            }
        }

        #endregion

        /// <summary>
        /// 限定角度
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        protected Vector3 LimitAngle(Vector3 eulerAngles)
        {
            eulerAngles.x = LimitAngle(_xLimitRange, eulerAngles.x);
            eulerAngles.y = LimitAngle(_yLimitRange, eulerAngles.y);
            eulerAngles.z = LimitAngle(_zLimitRange, eulerAngles.z);
            return eulerAngles;
        }

        /// <summary>
        /// 限定角度
        /// </summary>
        /// <param name="limitRange"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        protected float LimitAngle(Vector2 limitRange, float angle)
        {
            if (limitRange.y - limitRange.x > _360) return angle;
            if (limitRange.x >= 0)
            {
                while (angle > 360) angle -= 360;
                while (angle < 0) angle += 360;
            }
            else if (limitRange.y <= 0)
            {
                while (angle > 0) angle -= 360;
                while (angle < -360) angle += 360;
            }
            else
            {
                while (angle > 180) angle -= 360;
                while (angle < -180) angle += 360;
            }
            return Mathf.Clamp(angle, limitRange.x - _minDetectionAngle, limitRange.y + _minDetectionAngle);
        }

        /// <summary>
        /// 限定角度到[-180,180]范围内
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        protected float LimitAngleTo180(float angle)
        {
            while (angle > 180) angle -= 360;
            while (angle < -180) angle += 360;
            return angle;
        }

        /// <summary>
        /// 在限定区间内
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public bool InLimitRange(Vector3 eulerAngles)
        {
            return InLimitRange(_xLimitRange, eulerAngles.x)
                && InLimitRange(_yLimitRange, eulerAngles.y)
                && InLimitRange(_zLimitRange, eulerAngles.z);
        }

        /// <summary>
        /// 在限定区间内
        /// </summary>
        /// <param name="limitRange"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool InLimitRange(Vector2 limitRange, float angle)
        {
            if (limitRange.y - limitRange.x > _360) return true;
            if (limitRange.x >= 0)
            {
                while (angle > 360) angle -= 360;
                while (angle < 0) angle += 360;
            }
            else if (limitRange.y <= 0)
            {
                while (angle > 0) angle -= 360;
                while (angle < -360) angle += 360;
            }
            else
            {
                while (angle > 180) angle -= 360;
                while (angle < -180) angle += 360;
            }
            return angle >= limitRange.x - _minDetectionAngle && angle <= limitRange.y + _minDetectionAngle;
        }

        /// <summary>
        /// 校验限定区间
        /// </summary>
        public void ValidateLimitRange()
        {
            ValidateLimitRange(ref _xLimitRange);
            ValidateLimitRange(ref _yLimitRange);
            ValidateLimitRange(ref _zLimitRange);
        }

        private void ValidateLimitRange(ref Vector2 limitRange)
        {
            if (limitRange.x >= 360)
            {
                limitRange.x -= 360;
                limitRange.y -= 360;
            }
            if (limitRange.y <= -360)
            {
                limitRange.x += 360;
                limitRange.y += 360;
            }
        }
    }
}
