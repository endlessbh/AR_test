
using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginHTCVive.Base;
using XCSJ.PluginXBox.Base;
using XCSJ.PluginXXR.Interaction.Toolkit.Base;

namespace XCSJ.PluginHTCVive.Tools
{
    /// <summary>
    /// 变换通过Vive手柄
    /// </summary>
    [Name("变换通过Vive手柄")]
    [Tip("默认通过Vive手柄的轴或按钮控制变换的移动、旋转、缩放", "By default, the axis or button of vive handle controls the movement, rotation and scaling of transformation")]
    [Tool(HTCViveManager.Title)]
    [XCSJ.Attributes.Icon(EIcon.Model)]
    [RequireManager(typeof(HTCViveManager))]
    [Owner(typeof(HTCViveManager))]
    public class TransformByVive : InteractProvider
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
        /// 速度
        /// </summary>
        [Name("速度")]
        public Vector3 _speed = Vector3.one;

        /// <summary>
        /// 移动角度偏移量
        /// </summary>
        [Name("移动角度偏移量")]
        [Tip("当设备未被校准时，移动前进方向可能相机方向有偏差，是用当前量可进行软件层面得纠正", "When the equipment is not calibrated, the moving direction may deviate from the camera direction, which can be corrected at the software level with the current amount")]
        [EnumPopup]
        public Vector3 _offsetAngle = Vector3.zero;

        /// <summary>
        /// Vive变换手柄轴与按钮数据
        /// </summary>
        [Name("Vive变换手柄轴与按钮数据")]
        public ViveControllerTransformAxisAndButton _axisAndButtonData = new ViveControllerTransformAxisAndButton();

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
            var offset = Vector3.Scale(_axisAndButtonData.GetOffset(), _speed) * Time.deltaTime;
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
                _speed = Vector3.one;
                _axisAndButtonData.SetDefaultMove();
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
                _speed = new Vector3(1, 20, 1);
                _axisAndButtonData.SetDefaultRotateY();
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
                _speed = new Vector3(20, 1, 1);
                _axisAndButtonData.SetDefaultRotateX();
            });
        }
    }

    /// <summary>
    /// Vive变换手柄轴与按钮
    /// </summary>
    [Serializable]
    [Name("Vive变换手柄轴与按钮")]
    public class ViveControllerTransformAxisAndButton : BaseViveController
    {
        #region Vive Focus 轴与按键

        /// <summary>
        /// Vive Focus X减:对应X值减小
        /// </summary>
        [Name("X减")]
        [Tip("Vive Focus 对应X值减小", "Vive Focus corresponding X value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _nxOfViveFocus = EViveFocusAxisAndButton.ThumbStickLeft;

        /// <summary>
        /// Vive Focus X增:对应X值增加
        /// </summary>
        [Name("X增")]
        [Tip("Vive Focus 对应X值增加", "Vive focus corresponding X value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _pxOfViveFocus = EViveFocusAxisAndButton.ThumbStickRight;

        /// <summary>
        /// Vive Focus Y减:对应Y值减小
        /// </summary>
        [Name("Y减")]
        [Tip("Vive Focus 对应Y值减小", "Vive Focus corresponding Y value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _nyOfViveFocus = EViveFocusAxisAndButton.None;

        /// <summary>
        /// Vive Focus Y增:对应Y值增加
        /// </summary>
        [Name("Y增")]
        [Tip("Vive Focus 对应Y值增加", "Vive focus corresponding Y value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _pyOfViveFocus = EViveFocusAxisAndButton.None;

        /// <summary>
        /// Vive Focus Z减:对应Z值减小
        /// </summary>
        [Name("Z减")]
        [Tip("Vive Focus 对应Z值减小", "Vive Focus corresponding Z value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _nzOfViveFocus = EViveFocusAxisAndButton.ThumbStickDown;

        /// <summary>
        /// Vive Focus Z增:对应Z值增加
        /// </summary>
        [Name("Z增")]
        [Tip("Vive Focus 对应Z值增加", "Vive focus corresponding Z value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveFocus)]
        public EViveFocusAxisAndButton _pzOfViveFocus = EViveFocusAxisAndButton.ThumbStickUp;

        private float GetValue(EViveFocusAxisAndButton axisAndButton)
        {
            if (axisAndButton.GetAbsValue(_handleType, out var value))
            {
                return value < _deadZone ? 0 : value;
            }
            return default;
        }

        #endregion

        #region Vive Pro 轴与按键

        /// <summary>
        /// Vive Pro X减:对应X值减小
        /// </summary>
        [Name("X减")]
        [Tip("Vive Pro 对应X值减小", "Vive Pro corresponding X value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _nxOfVivePro = EViveProAxisAndButton.ThumbStickLeft;

        /// <summary>
        /// Vive Pro X增:对应X值增加
        /// </summary>
        [Name("X增")]
        [Tip("Vive Pro 对应X值增加", "Vive Pro corresponding X value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _pxOfVivePro = EViveProAxisAndButton.ThumbStickRight;

        /// <summary>
        /// Vive Pro Y减:对应Y值减小
        /// </summary>
        [Name("Y减")]
        [Tip("Vive Pro 对应Y值减小", "Vive Pro corresponding Y value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _nyOfVivePro = EViveProAxisAndButton.None;

        /// <summary>
        /// Vive Pro Y增:对应Y值增加
        /// </summary>
        [Name("Y增")]
        [Tip("Vive Pro 对应Y值增加", "Vive Pro corresponding Y value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _pyOfVivePro = EViveProAxisAndButton.None;

        /// <summary>
        /// Vive Pro Z减:对应Z值减小
        /// </summary>
        [Name("Z减")]
        [Tip("Vive Pro 对应Z值减小", "Vive Pro corresponding Z value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _nzOfVivePro = EViveProAxisAndButton.ThumbStickDown;

        /// <summary>
        /// Vive Pro Z增:对应Z值增加
        /// </summary>
        [Name("Z增")]
        [Tip("Vive Pro 对应Z值增加", "Vive Pro corresponding Z value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.VivePro)]
        public EViveProAxisAndButton _pzOfVivePro = EViveProAxisAndButton.ThumbStickUp;

        private float GetValue(EViveProAxisAndButton axisAndButton)
        {
            if (axisAndButton.GetAbsValue(_handleType, out var value))
            {
                return value < _deadZone ? 0 : value;
            }
            return default;
        }

        #endregion

        #region Vive Cosmos 轴与按键

        /// <summary>
        /// Vive Cosmos X减:对应X值减小
        /// </summary>
        [Name("X减")]
        [Tip("Vive Cosmos 对应X值减小", "Vive Cosmos corresponding X value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _nxOfViveCosmos = EViveCosmosAxisAndButton.JoyStickLeft;

        /// <summary>
        /// Vive Cosmos X增:对应X值增加
        /// </summary>
        [Name("X增")]
        [Tip("Vive Cosmos 对应X值增加", "Vive Cosmos corresponding X value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _pxOfViveCosmos = EViveCosmosAxisAndButton.JoyStickRight;

        /// <summary>
        /// Vive Cosmos Y减:对应Y值减小
        /// </summary>
        [Name("Y减")]
        [Tip("Vive Cosmos 对应Y值减小", "Vive Cosmos corresponding Y value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _nyOfViveCosmos = EViveCosmosAxisAndButton.None;

        /// <summary>
        /// Vive Cosmos Y增:对应Y值增加
        /// </summary>
        [Name("Y增")]
        [Tip("Vive Cosmos 对应Y值增加", "Vive Cosmos corresponding Y value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _pyOfViveCosmos = EViveCosmosAxisAndButton.None;

        /// <summary>
        /// Vive Cosmos Z减:对应Z值减小
        /// </summary>
        [Name("Z减")]
        [Tip("Vive Cosmos 对应Z值减小", "Vive Cosmos corresponding Z value decreases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _nzOfViveCosmos = EViveCosmosAxisAndButton.JoyStickDown;

        /// <summary>
        /// Vive Cosmos Z增:对应Z值增加
        /// </summary>
        [Name("Z增")]
        [Tip("Vive Cosmos 对应Z值增加", "Vive Cosmos corresponding Z value increases")]
        [EnumPopup]
        [HideInSuperInspector(nameof(_ViveDeviceType), EValidityCheckType.NotEqual, EViveDeviceType.ViveCosmos)]
        public EViveCosmosAxisAndButton _pzOfViveCosmos = EViveCosmosAxisAndButton.JoyStickUp;

        private float GetValue(EViveCosmosAxisAndButton axisAndButton)
        {
            if (axisAndButton.GetAbsValue(_handleType, out var value))
            {
                return value < _deadZone ? 0 : value;
            }
            return default;
        }

        #endregion

        /// <summary>
        /// 死区
        /// </summary>
        [Name("死区")]
        [Tip("死区值，低于本值时不认为对应事件触发;", "Dead band value. When it is lower than this value, the corresponding event is not considered to be triggered;")]
        [Range(0, 1)]
        public float _deadZone = 0.5f;

        /// <summary>
        /// 设置默认移动
        /// </summary>
        public void SetDefaultMove()
        {
            _nxOfViveFocus = EViveFocusAxisAndButton.ThumbStickLeft;
            _pxOfViveFocus = EViveFocusAxisAndButton.ThumbStickRight;
            _nyOfViveFocus = EViveFocusAxisAndButton.None;
            _pyOfViveFocus = EViveFocusAxisAndButton.None;
            _nzOfViveFocus = EViveFocusAxisAndButton.ThumbStickDown;
            _pzOfViveFocus = EViveFocusAxisAndButton.ThumbStickUp;


            _nxOfVivePro = EViveProAxisAndButton.ThumbStickLeft;
            _pxOfVivePro = EViveProAxisAndButton.ThumbStickRight;
            _nyOfVivePro = EViveProAxisAndButton.None;
            _pyOfVivePro = EViveProAxisAndButton.None;
            _nzOfVivePro = EViveProAxisAndButton.ThumbStickDown;
            _pzOfVivePro = EViveProAxisAndButton.ThumbStickUp;


            _nxOfViveCosmos = EViveCosmosAxisAndButton.JoyStickLeft;
            _pxOfViveCosmos = EViveCosmosAxisAndButton.JoyStickRight;
            _nyOfViveCosmos = EViveCosmosAxisAndButton.None;
            _pyOfViveCosmos = EViveCosmosAxisAndButton.None;
            _nzOfViveCosmos = EViveCosmosAxisAndButton.JoyStickDown;
            _pzOfViveCosmos = EViveCosmosAxisAndButton.JoyStickUp;
        }

        /// <summary>
        /// 设置默认旋转X
        /// </summary>
        public void SetDefaultRotateX()
        {
            SetAllNone();

            _nxOfViveFocus = EViveFocusAxisAndButton.ThumbStickUp;
            _pxOfViveFocus = EViveFocusAxisAndButton.ThumbStickDown;

            _nxOfVivePro = EViveProAxisAndButton.ThumbStickUp;
            _pxOfVivePro = EViveProAxisAndButton.ThumbStickDown;

            _nxOfViveCosmos = EViveCosmosAxisAndButton.JoyStickUp;
            _pxOfViveCosmos = EViveCosmosAxisAndButton.JoyStickDown;

            _handleType = EHandRule.Right;
        }

        /// <summary>
        /// 设置默认旋转Y
        /// </summary>
        public void SetDefaultRotateY()
        {
            SetAllNone();

            _nyOfViveFocus = EViveFocusAxisAndButton.ThumbStickLeft;
            _pyOfViveFocus = EViveFocusAxisAndButton.ThumbStickRight;


            _nyOfVivePro = EViveProAxisAndButton.ThumbStickLeft;
            _pyOfVivePro = EViveProAxisAndButton.ThumbStickRight;

            _nyOfViveCosmos = EViveCosmosAxisAndButton.JoyStickLeft;
            _pyOfViveCosmos = EViveCosmosAxisAndButton.JoyStickRight;

            _handleType = EHandRule.Right;
        }

        /// <summary>
        /// 设置全部无
        /// </summary>
        public void SetAllNone()
        {
            _nxOfViveFocus = EViveFocusAxisAndButton.None;
            _pxOfViveFocus = EViveFocusAxisAndButton.None;
            _nyOfViveFocus = EViveFocusAxisAndButton.None;
            _pyOfViveFocus = EViveFocusAxisAndButton.None;
            _nzOfViveFocus = EViveFocusAxisAndButton.None;
            _pzOfViveFocus = EViveFocusAxisAndButton.None;


            _nxOfVivePro = EViveProAxisAndButton.None;
            _pxOfVivePro = EViveProAxisAndButton.None;
            _nyOfVivePro = EViveProAxisAndButton.None;
            _pyOfVivePro = EViveProAxisAndButton.None;
            _nzOfVivePro = EViveProAxisAndButton.None;
            _pzOfVivePro = EViveProAxisAndButton.None;


            _nxOfViveCosmos = EViveCosmosAxisAndButton.None;
            _pxOfViveCosmos = EViveCosmosAxisAndButton.None;
            _nyOfViveCosmos = EViveCosmosAxisAndButton.None;
            _pyOfViveCosmos = EViveCosmosAxisAndButton.None;
            _nzOfViveCosmos = EViveCosmosAxisAndButton.None;
            _pzOfViveCosmos = EViveCosmosAxisAndButton.None;
        }

        /// <summary>
        /// 获取偏移值
        /// </summary>
        /// <returns></returns>
        public Vector3 GetOffset() => GetOffset(Vector3.zero);

        /// <summary>
        /// 获取偏移值
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 GetOffset(Vector3 offset) => GetOffset(ref offset);

        /// <summary>
        /// 获取偏移值
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Vector3 GetOffset(ref Vector3 offset)
        {
            offset.x += -nx + px;
            offset.y += -ny + py;
            offset.z += -nz + pz;
            return offset;
        }

        private float nx
        {
            get
            {
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            return GetValue(_nxOfViveFocus);
                        }
                    case EViveDeviceType.VivePro:
                        {
                            return GetValue(_nxOfVivePro);
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            return GetValue(_nxOfViveCosmos);
                        }
                }
                return 0;
            }
        }

        private float px
        {
            get
            {
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            return GetValue(_pxOfViveFocus);
                        }
                    case EViveDeviceType.VivePro:
                        {
                            return GetValue(_pxOfVivePro);
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            return GetValue(_pxOfViveCosmos);
                        }
                }
                return 0;
            }
        }

        private float ny
        {
            get
            {
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            return GetValue(_nyOfViveFocus);
                        }
                    case EViveDeviceType.VivePro:
                        {
                            return GetValue(_nyOfVivePro);
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            return GetValue(_nyOfViveCosmos);
                        }
                }
                return 0;
            }
        }

        private float py
        {
            get
            {
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            return GetValue(_pyOfViveFocus);
                        }
                    case EViveDeviceType.VivePro:
                        {
                            return GetValue(_pyOfVivePro);
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            return GetValue(_pyOfViveCosmos);
                        }
                }
                return 0;
            }
        }


        private float nz
        {
            get
            {
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            return GetValue(_nzOfViveFocus);
                        }
                    case EViveDeviceType.VivePro:
                        {
                            return GetValue(_nzOfVivePro);
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            return GetValue(_nzOfViveCosmos);
                        }
                }
                return 0;
            }
        }

        private float pz
        {
            get
            {
                switch (_ViveDeviceType)
                {
                    case EViveDeviceType.ViveFocus:
                        {
                            return GetValue(_pzOfViveFocus);
                        }
                    case EViveDeviceType.VivePro:
                        {
                            return GetValue(_pzOfVivePro);
                        }
                    case EViveDeviceType.ViveCosmos:
                        {
                            return GetValue(_pzOfViveCosmos);
                        }
                }
                return 0;
            }
        }
    }
}
