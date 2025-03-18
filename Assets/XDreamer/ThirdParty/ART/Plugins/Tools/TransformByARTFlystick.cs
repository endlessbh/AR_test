using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginART.Base;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginART.Tools
{
    /// <summary>
    /// 变换通过ART Flystick:通过与ART进行数据流通信，控制目标变换的姿态（位置与旋转）
    /// </summary> 
    [Name("变换通过ART Flystick")]
    [Tip("通过与ART进行数据流通信，控制目标变换的姿态（位置与旋转）", "Through data flow communication with art, the attitude (position and rotation) of the target is controlled")]
    [Tool(ARTHelper.Title, rootType = typeof(ARTManager))]
    [DisallowMultipleComponent]
    public class TransformByARTFlystick : BaseTransformByART
    {
        /// <summary>
        /// 目标变换:用于控制的目标变换
        /// </summary>
        [Name("目标变换")]
        [Tip("用于控制的目标变换", "Target transformation for control")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        public override Transform targetTransform
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
        /// 数据类型
        /// </summary>
        public override EDataType dataType { get => EDataType.FlyStick; set { } }

        /// <summary>
        /// 刚体ID:用于与ART软件进行数据流通信的刚体ID
        /// </summary>
        [Name("刚体ID")]
        [Tip("用于与ART软件进行数据流通信的刚体ID", "Rigid body ID for data flow communication with ART software")]
        [Range(0, 10)]
        public int _rigidBodyID;

        /// <summary>
        /// 刚体ID
        /// </summary>
        public override int rigidBodyID
        {
            get => _rigidBodyID;
            set => this.XModifyProperty(ref _rigidBodyID, value);
        }

        /// <summary>
        /// 空间类型
        /// </summary>
        [Name("空间类型")]
        [EnumPopup]
        public ESpaceType _spaceType = ESpaceType.Local;

        /// <summary>
        /// 空间类型
        /// </summary>
        public override ESpaceType spaceType => _spaceType;

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
        /// Flystick按钮数据
        /// </summary>
        [Name("Flystick按钮数据")]
        public FlystickButtonData _flystickButtonData = new FlystickButtonData();

        /// <summary>
        /// Flystick按钮数据
        /// </summary>
        [Serializable]
        [Name("Flystick按钮数据")]
        public class FlystickButtonData
        {
            /// <summary>
            /// X减:对应X值减小
            /// </summary>
            [Name("X减")]
            [Tip("对应X值减小", "The corresponding x value decreases")]
            [EnumPopup]
            public EFlystick2Switchs _nx = EFlystick2Switchs.JoystickLeft;

            /// <summary>
            /// X增:对应X值增加
            /// </summary>
            [Name("X增")]
            [Tip("对应X值增加", "Corresponding x value increase")]
            [EnumPopup]
            public EFlystick2Switchs _px = EFlystick2Switchs.JoystickRight;

            /// <summary>
            /// Y减:对应Y值减小
            /// </summary>
            [Name("Y减")]
            [Tip("对应Y值减小", "The corresponding y value decreases")]
            [EnumPopup]
            public EFlystick2Switchs _ny = EFlystick2Switchs.None;

            /// <summary>
            /// Y增:对应Y值增加
            /// </summary>
            [Name("Y增")]
            [Tip("对应Y值增加", "The corresponding y value increases")]
            [EnumPopup]
            public EFlystick2Switchs _py = EFlystick2Switchs.None;

            /// <summary>
            /// Z减:对应Z值减小
            /// </summary>
            [Name("Z减")]
            [Tip("对应Z值减小", "The corresponding Z value decreases")]
            [EnumPopup]
            public EFlystick2Switchs _nz = EFlystick2Switchs.JoystickDown;

            /// <summary>
            /// Z增:对应Z值增加
            /// </summary>
            [Name("Z增")]
            [Tip("对应Z值增加", "Corresponding Z value increase")]
            [EnumPopup]
            public EFlystick2Switchs _pz = EFlystick2Switchs.JoystickUp;

            /// <summary>
            /// 设置默认移动
            /// </summary>
            public void SetDefaultMove()
            {
                _nx = EFlystick2Switchs.JoystickLeft;
                _px = EFlystick2Switchs.JoystickRight;
                _ny = EFlystick2Switchs.None;
                _py = EFlystick2Switchs.None;
                _nz = EFlystick2Switchs.JoystickDown;
                _pz = EFlystick2Switchs.JoystickUp;
            }

            /// <summary>
            /// 设置默认旋转X
            /// </summary>
            public void SetDefaultRotateX()
            {
                _nx = EFlystick2Switchs.JoystickUp;
                _px = EFlystick2Switchs.JoystickDown;
            }

            /// <summary>
            /// 设置默认旋转Y
            /// </summary>
            public void SetDefaultRotateY()
            {
                _ny = EFlystick2Switchs.JoystickLeft;
                _py = EFlystick2Switchs.JoystickRight;
            }

            /// <summary>
            /// 设置全部无
            /// </summary>
            public void SetAllNone()
            {
                _nx = EFlystick2Switchs.None;
                _px = EFlystick2Switchs.None;
                _ny = EFlystick2Switchs.None;
                _py = EFlystick2Switchs.None;
                _nz = EFlystick2Switchs.None;
                _pz = EFlystick2Switchs.None;
            }

            private float GetValue(ARTFlyStickState state, EFlystick2Switchs flystickButton) => (float)state.GetAbsValue(flystickButton, _deadZone);

            /// <summary>
            /// 死区
            /// </summary>
            [Name("死区")]
            [Tip("死区值，低于本值时不认为对应事件触发;", "Dead band value. When it is lower than this value, the corresponding event is not considered to be triggered;")]
            [Range(0, 1)]
            public float _deadZone = 0.125f;

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="state"></param>
            /// <returns></returns>
            public Vector3 GetOffset(ARTFlyStickState state) => GetOffset(state, Vector3.zero);

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="state"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public Vector3 GetOffset(ARTFlyStickState state, Vector3 offset) => GetOffset(state, ref offset);

            /// <summary>
            /// 获取偏移值
            /// </summary>
            /// <param name="state"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public Vector3 GetOffset(ARTFlyStickState state, ref Vector3 offset)
            {
                offset.x += -GetValue(state, _nx) + GetValue(state, _px);
                offset.y += -GetValue(state, _ny) + GetValue(state, _py);
                offset.z += -GetValue(state, _nz) + GetValue(state, _pz);
                return offset;
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
                _flystickButtonData.SetDefaultMove();
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
                _flystickButtonData.SetAllNone();
                _flystickButtonData.SetDefaultRotateY();
                _speed = new Vector3(1, 20, 1);
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
                _flystickButtonData.SetAllNone();
                _flystickButtonData.SetDefaultRotateX();
                _speed = new Vector3(20, 1, 1);
            });
        }

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (targetTransform) { }
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
        /// 更新姿态
        /// </summary>
        protected override void UpdatePose()
        {
            //base.UpdatePose();
            if (!_streamClient || !_targetTransform) return;
            var state = _streamClient.GetLatestFlyStickState(rigidBodyID);
            if (state == null) return;
            var offset = _flystickButtonData.GetOffset(state);
            switch (_transformType)
            {
                case ETransformType.Position:
                    {
                        _targetTransform.Translate(offset, (Space)_spaceType);
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
    }
}
