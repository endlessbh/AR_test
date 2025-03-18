using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Dataflows.Base;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 点和方向数据
    /// </summary>
    [Serializable]
    public class PointDirectionData
    {
        [Name("点")]
        public Vector3Data _pointData = new Vector3Data();

        [Name("方向")]
        public Vector3Data _directionData = new Vector3Data();

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="transform"></param>
        public void Reset(Transform transform)
        {
            _pointData.SetTransform(transform);
            _pointData._dataType = EVector3DataType.TransformPosition;

            _directionData.SetTransform(transform);
            _directionData._dataType = EVector3DataType.TransformForward;
        }

        /// <summary>
        /// 点关联变换
        /// </summary>
        public Transform pointTransform { get => _pointData.GetTransform(); set=> _pointData.SetTransform(value); }

        /// <summary>
        /// 方向关联变换
        /// </summary>
        public Transform directionTransform { get => _directionData.GetTransform(); set => _directionData.SetTransform(value); }

        /// <summary>
        /// 点
        /// </summary>
        public virtual Vector3 point => _pointData.data;

        /// <summary>
        /// 方向
        /// </summary>
        public virtual Vector3 direction => _directionData.data;

        /// <summary>
        /// 创建平面
        /// </summary>
        /// <returns></returns>
        public Plane CreatePlane() => new Plane(direction, point);

        /// <summary>
        /// 创建射线
        /// </summary>
        /// <returns></returns>
        public Ray CreateRay() => new Ray(point, direction);
    }

    /// <summary>
    /// 数据类型
    /// </summary>
    public enum EVector3DataType
    {
        [Name("三维向量")]
        Vector3,

        [Name("变换位置")]
        TransformPosition,

        [Name("变换旋转")]
        TransformRotation,

        [Name("变换本地位置")]
        TransformLocalPosition,

        [Name("变换本地旋转")]
        TransformLocalRotation,

        [Name("变换本地缩放")]
        TransformLocalScale,

        [Name("变换上")]
        TransformUp,

        [Name("变换下")]
        TransformDown,

        [Name("变换前")]
        TransformForward,

        [Name("变换后")]
        TransformBack,

        [Name("变换左")]
        TransformLeft,

        [Name("变换右")]
        TransformRight,

        [Name("变换点")]
        TransformPoint,

        [Name("变换方向")]
        TransformDirection,
    }

    [Serializable]
    public class Vector3Data
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        [Name("数据类型")]
        [EnumPopup]
        public EVector3DataType _dataType = EVector3DataType.TransformPosition;

        /// <summary>
        /// 变换对象
        /// </summary>
        [Name("变换对象")]
        [HideInSuperInspector(nameof(_dataType), EValidityCheckType.Equal, EVector3DataType.Vector3)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public TransformPropertyValue _transform = new TransformPropertyValue();

        /// <summary>
        /// 三维向量
        /// </summary>
        [Name("三维向量")]
        public Vector3PropertyValue _vector3 = new Vector3PropertyValue(Vector3.up);

        /// <summary>
        /// 获取变换对象
        /// </summary>
        /// <returns></returns>
        public Transform GetTransform() => _transform.TryGetValue(out var value) ? value : null;

        /// <summary>
        /// 设置变换对象
        /// </summary>
        /// <param name="transform"></param>
        public void SetTransform(Transform transform) => _transform._transfrom = transform;

        public Vector3 data => TryGetData(out var v3) ? v3 : default;

        public bool TryGetData(out Vector3 data)
        {
            switch (_dataType)
            {
                case EVector3DataType.Vector3:
                    {
                        return _vector3.TryGetValue(out data);
                    }
                case EVector3DataType.TransformPosition:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.position;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformRotation:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.eulerAngles;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformLocalPosition:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.localPosition;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformLocalRotation:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.localEulerAngles;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformLocalScale:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.localScale;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformUp:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.up;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformDown:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = -t.up;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformForward:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.forward;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformBack:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = -t.forward;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformLeft:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = -t.right;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformRight:
                    {
                        if (_transform.TryGetValue(out var t) && t)
                        {
                            data = t.right;
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformPoint:
                    {
                        if (_transform.TryGetValue(out var t) && t && _vector3.TryGetValue(out var v3))
                        {
                            data = t.TransformPoint(v3);
                            return true;
                        }
                        break;
                    }
                case EVector3DataType.TransformDirection:
                    {
                        if (_transform.TryGetValue(out var t) && t && _vector3.TryGetValue(out var v3))
                        {
                            data = t.TransformDirection(v3);
                            return true;
                        }
                        break;
                    }
            }
            data = default;
            return false;
        }
    }
}
