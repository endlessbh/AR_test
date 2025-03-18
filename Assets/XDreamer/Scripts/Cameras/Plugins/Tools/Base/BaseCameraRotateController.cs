using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础相机旋转控制器
    /// </summary>
    [Name("基础相机旋转控制器")]
    public abstract class BaseCameraRotateController : BaseSpeedDampingController, ICameraRotateDamping
    {
        /// <summary>
        /// 速度系数
        /// </summary>
        public override Vector3 speedCoefficient => mainController.rotateSpeedCoefficient;

        /// <summary>
        /// 阻尼系数
        /// </summary>
        public override float dampingCoefficient => mainController.rotateDampingCoefficient;

        /// <summary>
        /// 旋转模式
        /// </summary>
        [Group("旋转设置", defaultIsExpanded = false)]
        [Name("旋转模式")]
        [EnumPopup]
        public ERotateMode _rotateMode = ERotateMode.Self_LocalXZ__Self_WorldY;

        /// <summary>
        /// 旋转模式:支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        public ERotateMode rotateMode
        {
            get => _rotateMode;
            set => this.XModifyProperty(ref _rotateMode, value, nameof(_rotateMode));
        }

        /// <summary>
        /// 角度偏移量
        /// </summary>
        [Name("角度偏移量")]
        [Readonly]
        public Vector3 _offset = Vector3.zero;

        /// <summary>
        /// 临时角度偏移量:用于阻尼处理时使用的临时量
        /// </summary>
        [Name("临时角度偏移量")]
        [Tip("用于阻尼处理时使用的临时量", "Temporary quantity used for damping treatment")]
        [Readonly]
        [EndGroup(true)]
        public Vector3 _tmpRotationAngle = Vector3.zero;

        private Quaternion _tmpRotation = Quaternion.identity;

        /// <summary>
        /// 旋转：使用<see cref="_offset"/>与<see cref="_rotateMode"/>执行旋转逻辑，之后将<see cref="_offset"/>恢复为默认值；
        /// </summary>
        public virtual void Rotate()
        {
            if (_offset == Vector3.zero) return;
            if (_useDamping)
            {
                _inDamping = true;
                _tmpRotation = Quaternion.Euler(_offset);
                _tmpRotationAngle = _offset;
            }
            else
            {
                cameraController.Rotate(_offset, (int)_rotateMode);
            }
            _offset = Vector3.zero;
        }

        /// <summary>
        /// 处理阻尼
        /// </summary>
        protected override void HandleDamping()
        {
            if (_inDamping)
            {
                //更新阻尼信息
                _dampingInfo.Update(Time.deltaTime, dampingCoefficient);

                {
                    //球面补间旋转量
                    _tmpRotation = Quaternion.Slerp(_tmpRotation, Quaternion.identity, _dampingInfo.valueRealtime);

                    //执行旋转
                    cameraController.Rotate(_tmpRotationAngle = _tmpRotation.eulerAngles, (int)_rotateMode);

                    //检测是否需要继续阻尼
                    if (_tmpRotation == Quaternion.identity || _tmpRotationAngle == Vector3.zero)
                    {
                        _inDamping = false;
                    }
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedInfo.Reset(60f);
            _speedInfo.Reset(Application.platform);
        }
    }
}
