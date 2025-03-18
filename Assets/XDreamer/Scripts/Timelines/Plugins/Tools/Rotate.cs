using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Maths;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using static XCSJ.PluginSMS.States.Motions.Rotate;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 旋转动画
    /// </summary>
    [Name("旋转", nameof(Rotate))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Rotate)]
    [RequireManager(typeof(TimelineManager))]
    public class Rotate : PlayableContentWithTransform
    {
        /// <summary>
        /// 旋转规则
        /// </summary>
        [Group("旋转设置", textEN = "Rotate Settings")]
        [Name("旋转规则")]
        [EnumPopup]
        public ERotateRule _rotateRule = ERotateRule.Local;

        /// <summary>
        /// 旋转值
        /// </summary>
        [Name("旋转值")]
        public Vector3Value _rotationValue = new Vector3Value();

        /// <summary>
        /// 注视上方向
        /// </summary>
        [Name("注视上方向")]
        [Tip("注视时的上方向(世界坐标系)；", "The upward direction of gaze (world coordinate system);")]
        [HideInSuperInspector(nameof(_rotateRule), EValidityCheckType.NotEqual, ERotateRule.LookAt)]
        public Vector3 _upwards = Vector3.up;

        [Name("轴角度")]
        [Tip("绕旋转轴发生旋转的角度；左手法则；", "The angle of rotation about the axis of rotation; Left hand rule;")]
        [HideInSuperInspector(nameof(_rotateRule), EValidityCheckType.Less | EValidityCheckType.Or, ERotateRule.LocalAxis, nameof(_rotateRule), EValidityCheckType.Greater, ERotateRule.WorldPointAxisThenLocal)]
        public float _axisAngle = 0;

        [Name("轴点")]
        [Tip("点轴旋转方式的轴点坐标(世界坐标系)；", "Axis point coordinates of point axis rotation mode (world coordinate system);")]
        [HideInSuperInspector(nameof(_rotateRule), EValidityCheckType.Less | EValidityCheckType.Or, ERotateRule.WorldPointAxis, nameof(_rotateRule), EValidityCheckType.Greater, ERotateRule.WorldPointAxisThenLocal)]
        public Vector3 _axisPoint = new Vector3();

        [Name("朝向轴点")]
        [HideInSuperInspector(nameof(_rotateRule), EValidityCheckType.NotEqual, ERotateRule.WorldPointAxis)]
        public bool _lookatAxisPoint = false;

        private double lastAxisAngle = 0;

        private Quaternion localRotation;
        private Quaternion worldRotation;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            lastAxisAngle = 0;

            localRotation = targetTransform.localRotation;
            worldRotation = targetTransform.rotation;
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            if (!_rotationValue.TryGetRotation(out var rotationValue)) return;

            switch (_rotateRule)
            {
                case ERotateRule.Local:
                    {
                        targetTransform.localRotation = localRotation;
                        targetTransform.Rotate(Vector3.Lerp(Vector3.zero, rotationValue, (float)percent.percent01OfWorkCurve), Space.Self);
                        break;
                    }
                case ERotateRule.World:
                    {
                        targetTransform.rotation = worldRotation;
                        targetTransform.Rotate(Vector3.Lerp(Vector3.zero, rotationValue, (float)percent.percent01OfWorkCurve), Space.World);
                        break;
                    }
                case ERotateRule.LookAt:
                    {
                        targetTransform.localRotation = Quaternion.Lerp(localRotation, Quaternion.LookRotation(rotationValue, _upwards), (float)percent.percent01OfWorkCurve);
                        break;
                    }
                case ERotateRule.LocalAxis:
                    {
                        targetTransform.localRotation = localRotation;
                        targetTransform.Rotate(rotationValue, (float)MathX.Lerp(0, _axisAngle, percent.percent01OfWorkCurve), Space.Self);
                        break;
                    }
                case ERotateRule.WorldAxis:
                    {
                        targetTransform.rotation = worldRotation;
                        targetTransform.Rotate(rotationValue, (float)MathX.Lerp(0, _axisAngle, percent.percent01OfWorkCurve), Space.World);
                        break;
                    }
                case ERotateRule.WorldPointAxis:
                    {
                        var currentAngle = MathX.Lerp(0, _axisAngle, percent.percent01OfWorkCurve);
                        targetTransform.RotateAround(_axisPoint, rotationValue, (float)(currentAngle - lastAxisAngle));
                        lastAxisAngle = currentAngle;
                        if (_lookatAxisPoint)
                        {
                            targetTransform.LookAt(_axisPoint);
                        }
                        break;
                    }
                case ERotateRule.WorldPointAxisThenLocal:
                    {
                        targetTransform.rotation = worldRotation;
                        targetTransform.RotateAround(_axisPoint, rotationValue, _axisAngle);
                        targetTransform.localRotation = localRotation;
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 三维向量值
    /// </summary>
    [Serializable]
    public class Vector3Value
    {
        public enum EValueType
        {
            [Name("三维向量")]
            Vector3,

            [Name("变换")]
            Transform,
        }

        /// <summary>
        /// 值类型
        /// </summary>
        [Name("值类型")]
        [EnumPopup]
        public EValueType _valueType = EValueType.Vector3;

        /// <summary>
        /// 三维向量
        /// </summary>
        [Name("三维向量")]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.NotEqual, EValueType.Vector3)]
        public Vector3PropertyValue _vector3Value = new Vector3PropertyValue();

        /// <summary>
        /// 变换
        /// </summary>
        [Name("变换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.NotEqual, EValueType.Transform)]
        public Transform _transform = null;

        /// <summary>
        /// 使用变换本地量
        /// </summary>
        [Name("变换")]
        [HideInSuperInspector(nameof(_valueType), EValidityCheckType.NotEqual, EValueType.Transform)]
        public bool _useLocalOfTransform = false;

        /// <summary>
        /// 获取位置值
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool TryGetPosition(out Vector3 position)
        {
            position = Vector3.zero;
            switch (_valueType)
            {
                case EValueType.Vector3:
                    {
                        return _vector3Value.TryGetValue(out position);
                    }
                case EValueType.Transform:
                    {
                        if (_transform)
                        {
                            position = _useLocalOfTransform ? _transform.localPosition : _transform.position;
                            return true;
                        }
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// 获取旋转值
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        public bool TryGetRotation(out Vector3 eulerAngles)
        {
            eulerAngles = Vector3.zero;
            switch (_valueType)
            {
                case EValueType.Vector3:
                    {
                        return _vector3Value.TryGetValue(out eulerAngles);
                    }
                case EValueType.Transform:
                    {
                        if (_transform)
                        {
                            eulerAngles = _useLocalOfTransform ? _transform.localEulerAngles : _transform.eulerAngles;
                            return true;
                        }
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// 获取缩放值
        /// </summary>
        /// <param name="scaleValue"></param>
        /// <returns></returns>
        public bool TryGetScale(out Vector3 scaleValue)
        {
            scaleValue = Vector3.zero;
            switch (_valueType)
            {
                case EValueType.Vector3:
                    {
                        return _vector3Value.TryGetValue(out scaleValue);
                    }
                case EValueType.Transform:
                    {
                        if (_transform)
                        {
                            scaleValue = _useLocalOfTransform ? _transform.localScale : _transform.lossyScale;
                            return true;
                        }
                        return false;
                    }
                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// 数据有效
        /// </summary>
        public bool validData => _valueType != EValueType.Transform || _transform;

        /// <summary>
        /// 检查数据是否发生变化：暂时只检测以变换为数据的情况
        /// </summary>
        /// <returns></returns>
        public bool CheckDataChanged()
        {
            switch (_valueType)
            {
                case EValueType.Transform:
                    {
                        if (_transform && _transform.hasChanged)
                        {
                            _transform.hasChanged = false;
                            return true;
                        }
                        break;
                    }
            }
            return false;
        }
    }

}
