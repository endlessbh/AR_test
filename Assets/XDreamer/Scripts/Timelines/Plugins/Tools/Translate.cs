using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;
using static XCSJ.PluginSMS.States.Motions.Move;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 平移动画
    /// </summary>
    [Name("平移", nameof(Translate))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class Translate : PlayableContentWithTransform
    {
        /// <summary>
        /// 路径补间类型
        /// </summary>
        [Group("平移设置", textEN = "Translate Settings")]
        [Name("路径补间类型")]
        [FormerlySerializedAs("pathType")]
        [EnumPopup]
        public ELineType _lineType = ELineType.Liner;

        /// <summary>
        /// 空间规则
        /// </summary>
        [Name("空间规则")]
        [EnumPopup]
        public ESpaceRule _spaceRule = ESpaceRule.Local;

        /// <summary>
        /// 路径点列表
        /// </summary>
        [Name("路径点列表")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        public List<Vector3Value> _positionValues = new List<Vector3Value>();

        /// <summary>
        /// 标识是否时有效路径
        /// </summary>
        public bool validMovePath => _positionValues.Count >= 1;

        private Vector3[] path01 = null;

        private Vector3 localPosition = Vector3.zero;

        private Vector3 worldPosition = Vector3.zero;

        private Quaternion localRotation = Quaternion.identity;

        private Quaternion worldRotation = Quaternion.identity;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            localPosition = targetTransform.localPosition;
            worldPosition = targetTransform.position;

            localRotation = targetTransform.localRotation;
            worldRotation = targetTransform.rotation;

            path01 = XGizmos.PathControlPointGenerator(_lineType, GetOffsets());
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected override void Update()
        {
            base.Update();

            var needUpdatePath = false;
            foreach (var item in _positionValues)
            {
                if (item.CheckDataChanged())
                {
                    needUpdatePath = true;
                    break;
                }
            }
            if (needUpdatePath)
            {
                path01 = XGizmos.PathControlPointGenerator(_lineType, GetOffsets());
            }
        }

        private Vector3[] GetOffsets()
        {
            if (_positionValues.Count==1)
            {
                if (_positionValues[0].TryGetPosition(out var position))
                {
                    return new Vector3[] { position, position };
                }
            }
            else if (_positionValues.Count > 1)
            {
                var list = new List<Vector3>();
                foreach (var item in _positionValues)
                {
                    if (item.TryGetPosition(out var position))
                    {
                        list.Add(position);
                    }
                }
                if (list.Count > 1)
                {
                    return list.ToArray();
                }
            }
            return new Vector3[] { new Vector3(), new Vector3() };
        }

        private Vector3 GetVector3(double percent) => XGizmos.Interp(_lineType, path01, percent);

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            switch (_spaceRule)
            {
                case ESpaceRule.World:
                    {
                        targetTransform.position = worldPosition + GetVector3(percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.Local:
                    {
                        targetTransform.localPosition = localPosition + GetVector3(percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.WorldRotation:
                    {
                        List<Vector3> tmpPath01 = new List<Vector3>();
                        foreach (var v in path01)
                        {
                            tmpPath01.Add(worldRotation * v);
                        }
                        targetTransform.position = worldPosition + XGizmos.Interp(_lineType, tmpPath01.ToArray(), percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.LocalRotation:
                    {
                        List<Vector3> tmpPath01 = new List<Vector3>();
                        foreach (var v in path01)
                        {
                            tmpPath01.Add(localRotation * v);
                        }
                        targetTransform.localPosition = localPosition + XGizmos.Interp(_lineType, tmpPath01.ToArray(), percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.WorldAbsolutely:
                    {
                        targetTransform.position = GetVector3(percent.percent01OfWorkCurve);
                        break;
                    }
                case ESpaceRule.WorldAbsolutelyRotation:
                    {
                        List<Vector3> tmpPath01 = new List<Vector3>();
                        foreach (var v in path01)
                        {
                            tmpPath01.Add(worldRotation * v);
                        }
                        targetTransform.position = XGizmos.Interp(_lineType, tmpPath01.ToArray(), percent.percent01OfWorkCurve);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// 有变换组件的可播放内容
    /// </summary>
    public abstract class PlayableContentWithTransform : PlayableContent
    {
        /// <summary>
        /// 目标变换
        /// </summary>
        [Name("目标变换")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Transform _targetTransform;

        /// <summary>
        /// 目标变换
        /// </summary>
        protected Transform targetTransform => _targetTransform ? _targetTransform : transform;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_targetTransform) _targetTransform = transform;
        }
    }
}
