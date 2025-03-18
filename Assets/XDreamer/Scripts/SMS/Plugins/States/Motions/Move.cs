using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Recorders;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.ComponentModel;
using XCSJ.PluginSMS.Base;
using XCSJ.PluginSMS.Kernel;

namespace XCSJ.PluginSMS.States.Motions
{
    /// <summary>
    /// 移动:移动组件是游戏对象的相对路径动画。在给定的时间内，游戏对象沿着相对于自身的路径移动，移动完成后，组件切换为完成态。
    /// </summary>
    [Serializable]
    [ComponentMenu("动作/"+ Title, typeof(SMSManager))]
    [Name(Title, nameof(Move))]
    [Tip("移动组件是游戏对象的相对路径动画。在给定的时间内，游戏对象沿着相对于自身的路径移动，移动完成后，组件切换为完成态。", "The moving component is the relative path animation of the game object. Within a given time, the game object moves along the path relative to itself. After the movement is completed, the component switches to the completed state.")]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [DisallowMultipleComponent]
    public class Move : TransformMotion<Move>, IPath
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "移动";

        /// <summary>
        /// 创建携带移动状态组件的状态
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [StateLib("动作", typeof(SMSManager))]
        [StateComponentMenu("动作/" + Title, typeof(SMSManager))]
        [Name(Title, nameof(Move))]
        [Tip("移动组件是游戏对象的相对路径动画。在给定的时间内，游戏对象沿着相对于自身的路径移动，移动完成后，组件切换为完成态。", "The moving component is the relative path animation of the game object. Within a given time, the game object moves along the path relative to itself. After the movement is completed, the component switches to the completed state.")]
        [XCSJ.Attributes.Icon(EIcon.Move)]
        public static State CreateMove(IGetStateCollection obj) => CreateNormalState(obj);

        /// <summary>
        /// 路径补间类型
        /// </summary>
        [Name("路径补间类型")]
        [FormerlySerializedAs("pathType")]
        [EnumPopup]
        public ELineType pathPatchType = ELineType.Liner;

        /// <summary>
        /// 空间规则枚举
        /// </summary>
        [Name("空间规则")]
        public enum ESpaceRule
        {
            /// <summary>
            /// 本地
            /// </summary>
            [Name("本地")]
            Local = 0,

            /// <summary>
            /// 本地旋转
            /// </summary>
            [Name("本地旋转")]
            LocalRotation,

            /// <summary>
            /// 世界(相对
            /// </summary>
            [Name("世界(相对)")]
            World,

            /// <summary>
            /// 世界旋转(相对)
            /// </summary>
            [Name("世界旋转(相对)")]
            WorldRotation,

            /// <summary>
            /// 世界
            /// </summary>
            [Name("世界")]
            WorldAbsolutely,

            /// <summary>
            /// 世界旋转
            /// </summary>
            [Name("世界旋转")]
            WorldAbsolutelyRotation,
        }

        /// <summary>
        /// 空间规则
        /// </summary>
        [Name("空间规则")]
        [EnumPopup]
        public ESpaceRule spaceRule = ESpaceRule.Local;

        /// <summary>
        /// 偏移值列表
        /// </summary>
        [Name("偏移值")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<Vector3> offsetValues = new List<Vector3>();

        /// <summary>
        /// 标识是否时有效路径
        /// </summary>
        public bool validMovePath => offsetValues.Count >= 1;

        private Vector3[] path01 = null;

        #region IPath
        private Transform pathTransform
        {
            get
            {
                var go = gameObjectSet.objects.FirstOrDefault();
                return go ? go.transform : null;
            }
        }

        /// <summary>
        /// 将空间类型的点列表转化为基于世界坐标系的点列表输出
        /// </summary>
        /// <param name="points">点列表</param>
        /// <param name="fromSpaceRule">点列表的空间类型</param>
        /// <returns></returns>
        public List<Vector3> ToWorldPoints(List<Vector3> points, ESpaceRule fromSpaceRule)
        {
            switch (fromSpaceRule)
            {
                case ESpaceRule.Local:
                case ESpaceRule.LocalRotation:
                    {
                        var t = pathTransform;
                        if (t)
                        {
                            return points.ToList(p => t.TransformPoint(p));
                        }
                        break;
                    }
                case ESpaceRule.World:
                case ESpaceRule.WorldRotation:
                    {
                        var t = pathTransform;
                        if (t)
                        {
                            return points.ToList(p => p + t.position);
                        }
                        break;
                    }
                case ESpaceRule.WorldAbsolutely:
                case ESpaceRule.WorldAbsolutelyRotation:
                    {
                        return points;
                    }
                default:
                    break;
            }
            throw new InvalidProgramException("无法将空间类型的点列表转化为基于世界坐标系的点列表!");
        }

        /// <summary>
        /// 将基于世界坐标系的点列表转化为期望空间类型的点列表输出
        /// </summary>
        /// <param name="points">基于世界坐标系的点列表</param>
        /// <param name="toSpaceRule">期望转换后的空间类型</param>
        /// <returns></returns>
        public List<Vector3> FromWorldPoints(List<Vector3> points, ESpaceRule toSpaceRule)
        {
            switch (toSpaceRule)
            {
                case ESpaceRule.Local:
                case ESpaceRule.LocalRotation:
                    {
                        var t = pathTransform;
                        if (t)
                        {
                            return points.ToList(p => t.InverseTransformPoint(p));
                        }
                        break;
                    }
                case ESpaceRule.World:
                case ESpaceRule.WorldRotation:
                    {
                        var t = pathTransform;
                        if (t)
                        {
                            return points.ToList(p => p - t.position);
                        }
                        break;
                    }
                case ESpaceRule.WorldAbsolutely:
                case ESpaceRule.WorldAbsolutelyRotation:
                    {
                        return points;
                    }
                default:
                    break;
            }
            throw new InvalidProgramException("无法将基于世界坐标系的点列表转化为期望空间类型的点列表输出!");
        }

        /// <summary>
        /// 路径的关键控制点
        /// </summary>
        public List<Vector3> keyPoints
        {
            get => ToWorldPoints(offsetValues, spaceRule);
            set
            {
                this.XModifyProperty(() =>
                {
                    offsetValues = FromWorldPoints(value, spaceRule);
                });
            }
        }

        /// <summary>
        /// 路径中被控制对象的变换
        /// </summary>
        public List<Transform> transforms => gameObjectSet.objects.ToList(go => go.transform);

        /// <summary>
        /// 向路径中被控制对象的变换列表中增加变换
        /// </summary>
        /// <param name="transform"></param>
        public void AddTransform(Transform transform)
        {
            if (transform)
            {
                gameObjectSet.Add(transform.gameObject);
            }
        }

        EViewRule IPath.viewRule { get; set; }

        #endregion

        /// <summary>
        /// 获取完整移动路径
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetFullMovePath()
        {
            return path01 = XGizmos.PathControlPointGenerator(pathPatchType, GetOffsets());
        }

        internal Vector3[] GetOffsets()
        {
            switch (offsetValues.Count)
            {
                case 0:
                    {
                        return new Vector3[] { new Vector3(), new Vector3() };
                    }
                case 1:
                    {
                        return new Vector3[] { offsetValues[0], offsetValues[0] };
                    }
                default:
                    {
                        return offsetValues.ToArray();
                    }
            }
        }

        /// <summary>
        /// 插值补间
        /// </summary>
        /// <param name="pathPatchType"></param>
        /// <param name="pathControlPoints"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Vector3 Interp(ELineType pathPatchType, Vector3[] pathControlPoints, double percent)
        {
            return XGizmos.Interp(pathPatchType, pathControlPoints, percent);
        }

        internal Vector3 GetVector3(double percent) => Interp(pathPatchType, path01, percent);

        /// <summary>
        /// 当设置百分比时
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="stateData"></param>
        protected override void OnSetPercent(Percent percent, StateData stateData)
        {
            if (validMovePath)
            {
                GetFullMovePath();
                base.OnSetPercent(percent, stateData);
            }
        }

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="info"></param>
        /// <param name="percent"></param>
        protected override void SetPercent(TransformRecorder.Info info, Percent percent)
        {
            switch (spaceRule)
            {
                case ESpaceRule.World:
                    {
                        info.transform.position = info.worldPosition + GetVector3(percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.Local:
                    {
                        info.transform.localPosition = info.localPosition + GetVector3(percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.WorldRotation:
                    {
                        List<Vector3> tmpPath01 = new List<Vector3>();
                        foreach (var v in path01)
                        {
                            tmpPath01.Add(info.worldRotation * v);
                        }
                        info.transform.position = info.worldPosition + Interp(pathPatchType, tmpPath01.ToArray(), percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.LocalRotation:
                    {
                        List<Vector3> tmpPath01 = new List<Vector3>();
                        foreach (var v in path01)
                        {
                            tmpPath01.Add(info.localRotation * v);
                        }
                        info.transform.localPosition = info.localPosition + Interp(pathPatchType, tmpPath01.ToArray(), percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.WorldAbsolutely:
                    {
                        info.transform.position = GetVector3(percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.WorldAbsolutelyRotation:
                    {
                        List<Vector3> tmpPath01 = new List<Vector3>();
                        foreach (var v in path01)
                        {
                            tmpPath01.Add(info.worldRotation * v);
                        }
                        info.transform.position = Interp(pathPatchType, tmpPath01.ToArray(), percent.percent01OfWorkCurve);
                        break;
                    }
            }
        }

        /// <summary>
        /// 数据有效性；对当前对象的数据进行有效性判断；仅判断，不做其它处理；
        /// </summary>
        /// <returns></returns>
        public override bool DataValidity()
        {
            return validMovePath && base.DataValidity();
        }
     
    }
}
