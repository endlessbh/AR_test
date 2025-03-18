using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras.Base;
using XCSJ.PluginsCameras.Controllers;

namespace XCSJ.PluginsCameras.Tools.Base
{
    /// <summary>
    /// 基础相机移动控制器
    /// </summary>
    [Name("基础相机移动控制器")]
    public abstract class BaseCameraMoveController : BaseSpeedDampingController, ICameraMoveDamping
    {
        /// <summary>
        /// 速度系数
        /// </summary>
        public override Vector3 speedCoefficient => mainController.moveSpeedCoefficient;

        /// <summary>
        /// 阻尼系数
        /// </summary>
        public override float dampingCoefficient => mainController.moveDampingCoefficient;

        /// <summary>
        /// 移动模式
        /// </summary>
        [Group("移动设置", defaultIsExpanded = false)]
        [Name("移动模式")]
        [EnumPopup]
        public EMoveMode _moveMode = EMoveMode.Self_Local;

        /// <summary>
        /// 移动模式:支持在Unity编辑器中执行撤销与重做；
        /// </summary>
        public EMoveMode moveMode
        {
            get => _moveMode;
            set => this.XModifyProperty(ref _moveMode, value, nameof(_moveMode));
        }

        /// <summary>
        /// 位置偏移量
        /// </summary>
        [Name("位置偏移量")]
        [Readonly]
        public Vector3 _offset = Vector3.zero;

        /// <summary>
        /// 临时位置偏移量:用于阻尼处理时使用的临时量
        /// </summary>
        [Name("临时位置偏移量")]
        [Tip("用于阻尼处理时使用的临时量", "Temporary quantity used for damping treatment")]
        [Readonly]
        [EndGroup(true)]
        public Vector3 _tmpOffset = Vector3.zero;

        /// <summary>
        /// 移动：使用<see cref="_offset"/> 与<see cref= "_moveMode" /> 执行移动逻辑，之后将<see cref="_offset"/> 恢复为默认值；
        /// </summary>
        public virtual void Move()
        {
            if (_offset == Vector3.zero) return;
            if (_useDamping)
            {
                _inDamping = true;
                _tmpOffset = _offset;
            }
            else
            {
                cameraController.Move(_offset, (int)_moveMode);
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

                //球面补间移动量
                _tmpOffset = Vector3.Slerp(_tmpOffset, Vector3.zero, _dampingInfo.valueRealtime);

                //执行移动
                cameraController.Move(_tmpOffset, (int)_moveMode);

                //检测是否需要继续阻尼
                if (_tmpOffset == Vector3.zero)
                {
                    _inDamping = false;
                }
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            _speedInfo.Reset(10f);
            _speedInfo.Reset(Application.platform);
        }
    }

    /// <summary>
    /// 基础相机变换控制器
    /// </summary>
    [Name("基础相机变换控制器")]
    public abstract class BaseCameraTransformController : BaseCameraToolController { }
}
