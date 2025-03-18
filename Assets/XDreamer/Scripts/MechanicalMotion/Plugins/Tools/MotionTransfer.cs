using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Tools;

namespace XCSJ.PluginMechanicalMotion.Tools
{
    /// <summary>
    /// 运动转换器：用于传递或转换速度、角速度或位移偏移量等运动量
    /// </summary>
    [Name("运动转换器")]
    [Tip("用于传递或转换速度、角速度或位移偏移量等运动量", "It is used to transmit motion such as speed")]
    [XCSJ.Attributes.Icon()]
    [Tool(MechanicalMotionManager.Title, rootType = typeof(MechanicalMotionManager))]
    [RequireManager(typeof(MechanicalMotionManager))]
    [Owner(typeof(MechanicalMotionManager))]
    public class MotionTransfer : InteractProvider
    {
        /// <summary>
        /// 转换据列表
        /// </summary>
        [Name("转换数据列表")]
        public List<TransferData> _transferDatas = new List<TransferData>();

        [Serializable]
        public class TransferData
        {
            public enum ETransferDataType
            {
                [Name("无")]
                None,

                [Name("速度")]
                Velocity,

                [Name("连续位移")]
                Displacement,
            }

            /// <summary>
            /// 转换类型
            /// </summary>
            [Name("转换类型")]
            [EnumPopup]
            public ETransferDataType _transferDataType = ETransferDataType.Velocity;

            /// <summary>
            /// 输入机构
            /// </summary>
            [Name("输入机构")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public PlaneMechanism _inMechanism;

            /// <summary>
            /// 输入机构为旋转机构时有效
            /// </summary>
            [Name("自定义输入旋转机构半径")]
            public bool _inCustomRotationMechanismRadius = false;

            /// <summary>
            /// 输入旋转机构半径
            /// </summary>
            [Name("输入旋转机构半径")]
            [Min(0)]
            public double _inRotationMechanismRadius = 1;

            /// <summary>
            /// 输出机构
            /// </summary>
            [Name("输出机构")]
            [ValidityCheck(EValidityCheckType.NotNull)]
            public PlaneMechanism _outMechanism;

            /// <summary>
            /// 输出机构为旋转机构时有效
            /// </summary>
            [Name("自定义输出旋转机构半径")]
            public bool _outCustomRotationMechanismRadius = false;

            /// <summary>
            /// 输出旋转机构半径
            /// </summary>
            [Name("输出旋转机构半径")]
            [Min(0)]
            public double _outRotationMechanismRadius = 1;

            /// <summary>
            /// 速度计算类型
            /// </summary>
            public enum EComputeType
            {
                [Name("无")]
                None,

                [Name("乘")]
                Multiply,

                [Name("加")]
                Add,

                [Name("先乘后加")]
                MultiplyAndAdd,

                [Name("先加后乘")]
                AddAndMultiply,
            }

            /// <summary>
            /// 计算方式
            /// </summary>
            [Name("计算方式")]
            [EnumPopup]
            public EComputeType _computeType = EComputeType.Multiply;

            /// <summary>
            /// 乘值
            /// </summary>
            [Name("乘值")]
            [HideInSuperInspector(nameof(_computeType), EValidityCheckType.NotEqual, EComputeType.Multiply)]
            public FloatPropertyValue _multiplyValue = new FloatPropertyValue(1);

            /// <summary>
            /// 加值
            /// </summary>
            [Name("加值")]
            [HideInSuperInspector(nameof(_computeType), EValidityCheckType.NotEqual, EComputeType.Add)]
            public FloatPropertyValue _addValue = new FloatPropertyValue(0);

            /// <summary>
            /// 转换数据
            /// </summary>
            public void Transfer()
            {
                switch (_transferDataType)
                {
                    case ETransferDataType.Velocity: TransferVelocity(); break;
                    case ETransferDataType.Displacement: TransferDisplacement(); break;
                }
            }

            /// <summary>
            /// 传输速度
            /// </summary>
            private void TransferVelocity()
            {
                double velocity = 0;

                if (_inMechanism is RotationMechanism inRM && inRM)
                {
                    velocity = inRM.GetLinearVelocity(_inCustomRotationMechanismRadius ? _inRotationMechanismRadius : inRM._radius);
                }
                else
                {
                    velocity = _inMechanism.velocity;
                }

                velocity = Compute(velocity);

                if (_outMechanism is RotationMechanism outRM && outRM)
                {
                    outRM.SetLinearVelocity(velocity, _outCustomRotationMechanismRadius ? _outRotationMechanismRadius : outRM._radius);
                }
                else
                {
                    _outMechanism.velocity = velocity;
                }
            }

            private void TransferDisplacement()
            {
                double displacement = 0;

                if (_inMechanism is RotationMechanism inRM && inRM)
                {
                    displacement = inRM.GetDisplacementOffset(_inCustomRotationMechanismRadius ? _inRotationMechanismRadius : inRM._radius);
                }
                else
                {
                    displacement = _inMechanism.displacementOffset;
                }

                displacement = Compute(displacement);

                if (_outMechanism is RotationMechanism outRM && outRM)
                {
                    outRM.SetDisplacementOffset(displacement, _outCustomRotationMechanismRadius ? _outRotationMechanismRadius : outRM._radius);
                }
                else
                {
                    _outMechanism.displacementOffset = displacement;
                }
            }

            private double Compute(double inValue)
            {
                switch (_computeType)
                {
                    case EComputeType.None:break;
                    case EComputeType.Multiply:
                        {
                            inValue *= _multiplyValue.GetValue();
                            break;
                        }
                    case EComputeType.Add:
                        {
                            inValue += _addValue.GetValue();
                            break;
                        }
                    case EComputeType.MultiplyAndAdd:
                        {
                            inValue = inValue * _multiplyValue.GetValue() + _addValue.GetValue();
                            break;
                        }
                    case EComputeType.AddAndMultiply:
                        {
                            inValue = (inValue + _addValue.GetValue()) * _multiplyValue.GetValue();
                            break;
                        }
                }
                return inValue;
            }
        }

        private void Update()
        {
            foreach (var item in _transferDatas)
            {
                item.Transfer();
            }
        }
    }
}
