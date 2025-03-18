using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginSamsungWMR.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginSamsungWMR.Tools
{
    /// <summary>
    /// 变换通过三星玄龙手柄
    /// </summary>
    [Name("变换通过三星玄龙手柄")]
    [Tip("默认通过三星玄龙手柄的轴或按钮控制变换的移动、旋转、缩放", "By default, the movement, rotation and scaling of the transformation are controlled by the axis or button of the Samsung Xuanlong handle")]
    [Tool(SamsungWMRManager.Title)]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [RequireManager(typeof(SamsungWMRManager))]
    [Owner(typeof(SamsungWMRManager))]
    public class TransformBySamsungWMR : InteractProvider
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public Transform targetTransform
        {
            get
            {
                if (!_targetTransform)
                {
                    _targetTransform = this.transform;
                }
                return _targetTransform;
            }
        }

        /// <summary>
        /// 速度
        /// </summary>
        [Name("速度")]
        public Vector3 _speed = Vector3.one;

        /// <summary>
        /// 变换类型
        /// </summary>
        [Name("变换类型")]
        public enum ETransformType
        {
            /// <summary>
            /// 位置
            /// </summary>
            [Name("位置")]
            Position,

            /// <summary>
            /// 旋转
            /// </summary>
            [Name("旋转")]
            Rotation,

            /// <summary>
            /// 缩放
            /// </summary>
            [Name("缩放")]
            Scale,
        }

        /// <summary>
        /// 变换类型
        /// </summary>
        [Name("变换类型")]
        [EnumPopup]
        public ETransformType _transformType = ETransformType.Position;

        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.Local;

        /// <summary>
        /// 移动角度偏移量
        /// </summary>
        [Name("移动角度偏移量")]
        [Tip("当设备未被校准时，移动前进方向可能相机方向有偏差，是用当前量可进行软件层面得纠正", "When the equipment is not calibrated, the moving direction may deviate from the camera direction, which can be corrected at the software level with the current amount")]
        [EnumPopup]
        public Vector3 _offsetAngle = Vector3.zero;

        /// <summary>
        /// 手柄类型
        /// </summary>
        [Name("手柄类型")]
        [EnumPopup]
        public EHandRule _handleType = EHandRule.Left;

        /// <summary>
        /// 轴与按钮数据
        /// </summary>
        [Name("轴与按钮数据")]
        public SamsungWMRAxisAndButtonData _axisAndButtonData = new SamsungWMRAxisAndButtonData();

        /// <summary>
        /// 三星WMR轴与按钮数据
        /// </summary>
        [Serializable]
        [Name("零件按钮界面")]
        public class SamsungWMRAxisAndButtonData
        {
            /// <summary>
            /// X减:对应X值减小
            /// </summary>
            [Name("X减")]
            [Tip("对应X值减小", "The corresponding x value decreases")]
            [EnumPopup]
            public ESamsungWMRAxisAndButton _nx = ESamsungWMRAxisAndButton.JoyStickLeft;

            /// <summary>
            /// X增:对应X值增加
            /// </summary>
            [Name("X增")]
            [Tip("对应X值增加", "Corresponding x value increase")]
            [EnumPopup]
            public ESamsungWMRAxisAndButton _px = ESamsungWMRAxisAndButton.JoyStickRight;

            /// <summary>
            /// Y减:对应Y值减小
            /// </summary>
            [Name("Y减")]
            [Tip("对应Y值减小", "The corresponding y value decreases")]
            [EnumPopup]
            public ESamsungWMRAxisAndButton _ny = ESamsungWMRAxisAndButton.None;

            /// <summary>
            /// Y增:对应Y值增加
            /// </summary>
            [Name("Y增")]
            [Tip("对应Y值增加", "The corresponding y value increases")]
            [EnumPopup]
            public ESamsungWMRAxisAndButton _py = ESamsungWMRAxisAndButton.None;

            /// <summary>
            /// Z减:对应Z值减小
            /// </summary>
            [Name("Z减")]
            [Tip("对应Z值减小", "The corresponding Z value decreases")]
            [EnumPopup]
            public ESamsungWMRAxisAndButton _nz = ESamsungWMRAxisAndButton.JoyStickDown;

            /// <summary>
            /// Z增:对应Z值增加
            /// </summary>
            [Name("Z增")]
            [Tip("对应Z值增加", "Corresponding Z value increase")]
            [EnumPopup]
            public ESamsungWMRAxisAndButton _pz = ESamsungWMRAxisAndButton.JoyStickUp;

            /// <summary>
            /// 设置默认移动
            /// </summary>
            public void SetDefaultMove()
            {
                _nx = ESamsungWMRAxisAndButton.JoyStickLeft;
                _px = ESamsungWMRAxisAndButton.JoyStickRight;
                _ny = ESamsungWMRAxisAndButton.None;
                _py = ESamsungWMRAxisAndButton.None;
                _nz = ESamsungWMRAxisAndButton.JoyStickDown;
                _pz = ESamsungWMRAxisAndButton.JoyStickUp;
            }

            /// <summary>
            /// 设置默认旋转X
            /// </summary>
            public void SetDefaultRotateX()
            {
                _nx = ESamsungWMRAxisAndButton.JoyStickUp;
                _px = ESamsungWMRAxisAndButton.JoyStickDown;
            }

            /// <summary>
            /// 设置默认旋转Y
            /// </summary>
            public void SetDefaultRotateY()
            {
                _ny = ESamsungWMRAxisAndButton.JoyStickLeft;
                _py = ESamsungWMRAxisAndButton.JoyStickRight;
            }

            /// <summary>
            /// 设置全部无
            /// </summary>
            public void SetAllNone()
            {
                _nx = ESamsungWMRAxisAndButton.None;
                _px = ESamsungWMRAxisAndButton.None;
                _ny = ESamsungWMRAxisAndButton.None;
                _py = ESamsungWMRAxisAndButton.None;
                _nz = ESamsungWMRAxisAndButton.None;
                _pz = ESamsungWMRAxisAndButton.None;
            }

            private float GetValue(ESamsungWMRAxisAndButton axisAndButton, EHandRule handleType)
            {
                if (axisAndButton.TryGetAbsValue(handleType, out var value))
                {
                    return value < _deadZone ? 0 : value;
                }
                return default;
            }

            /// <summary>
            /// 死区
            /// </summary>
            [Name("死区")]
            [Tip("死区值，低于本值时不认为对应事件触发;", "Dead band value. When it is lower than this value, the corresponding event is not considered to be triggered;")]
            [Range(0, 1)]
            public float _deadZone = 0.5f;

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <returns></returns>
            public Vector3 GetOffset(EHandRule handleType) => GetOffset(Vector3.zero, handleType);

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public Vector3 GetOffset(Vector3 offset, EHandRule handleType) => GetOffset(ref offset, handleType);

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="offset"></param>
            /// <returns></returns>
            public Vector3 GetOffset(ref Vector3 offset, EHandRule handleType)
            {
                offset.x += -GetValue(_nx, handleType) + GetValue(_px, handleType);
                offset.y += -GetValue(_ny, handleType) + GetValue(_py, handleType);
                offset.z += -GetValue(_nz, handleType) + GetValue(_pz, handleType);
                return offset;
            }
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            if (targetTransform) { }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_targetTransform) return;
            var offset = Vector3.Scale(_axisAndButtonData.GetOffset(_handleType), _speed) * Time.deltaTime;
            switch (_transformType)
            {
                case ETransformType.Position:
                    {
                        var q = Quaternion.Euler(_offsetAngle);
                        _targetTransform.Translate(q * offset, (Space)_spaceType);
                        break;
                    }
                case ETransformType.Rotation:
                    {
                        _targetTransform.Rotate(offset, (Space)_spaceType);
                        break;
                    }
                case ETransformType.Scale:
                    {
                        if (_spaceType == ESpaceType.Local)
                        {
                            _targetTransform.localScale += offset;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// 设置默认移动
        /// </summary>
        public void SetDefaultMove()
        {
            this.XModifyProperty(() =>
            {
                _transformType = ETransformType.Position;
                _spaceType = ESpaceType.Local;
                _axisAndButtonData.SetDefaultMove();
                _speed = Vector3.one;
            });
        }

        /// <summary>
        /// 设置默认旋转世界Y
        /// </summary>
        public void SetDefaultRotateWorldY()
        {
            this.XModifyProperty(() =>
            {
                _transformType = ETransformType.Rotation;
                _spaceType = ESpaceType.World;
                _axisAndButtonData.SetAllNone();
                _axisAndButtonData.SetDefaultRotateY();
                _speed = new Vector3(1, 20, 1);
                _handleType = EHandRule.Right;
            });
        }

        /// <summary>
        /// 设置默认旋转本地X
        /// </summary>
        public void SetDefaultRotateLocalX()
        {
            this.XModifyProperty(() =>
            {
                _transformType = ETransformType.Rotation;
                _spaceType = ESpaceType.Local;
                _axisAndButtonData.SetAllNone();
                _axisAndButtonData.SetDefaultRotateX();
                _speed = new Vector3(20, 1, 1);
                _handleType = EHandRule.Right;
            });
        }
    }
}
