using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Base.Tweens;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 刚体平移
    /// </summary>
    [Name("刚体平移", nameof(MoveRigidbody))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Move)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class MoveRigidbody : PlayableContent
    {
        [Group("平移设置", textEN = "Translate Settings")]
        [Name("3D刚体")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [Tip("用于控制自动移动的对象；如本参数无效，则使用当前组件所在游戏对象上当前参数类型的组件对象", "Used to control automatically moving objects; If this parameter is invalid, the component object of the current parameter type on the game object where the current component is located will be used")]
        [ComponentPopup]
        public Rigidbody _rigidbody3D;

        /// <summary>
        /// 3D刚体
        /// </summary>
        public Rigidbody rigidbody3D
        {
            get
            {
                if (!_rigidbody3D)
                {
                    _rigidbody3D = GetComponent<Rigidbody>();
                }
                return _rigidbody3D;
            }
        }

        /// <summary>
        /// 路径补间类型
        /// </summary>
        [Name("路径补间类型")]
        [FormerlySerializedAs("pathType")]
        [EnumPopup]
        public ELineType _lineType = ELineType.Liner;

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

        private Vector3 startPosition = Vector3.zero;

        private bool isKinematic { get; set; } = false;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!rigidbody3D)
            {
                enabled = false;
                return;
            }

            path01 = XGizmos.PathControlPointGenerator(_lineType, GetOffsets());

            startPosition = rigidbody3D.position;

            isKinematic = rigidbody3D.isKinematic;
            rigidbody3D.isKinematic = true;
            startPosition = rigidbody3D.position;
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            if (!rigidbody3D) return;

            rigidbody3D.isKinematic = isKinematic;
            rigidbody3D.position = startPosition;
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

        /// <summary>
        /// 设置百分比
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            rigidbody3D.MovePosition(startPosition + GetVector3(percent.percent01OfWorkCurve));
        }

        private Vector3[] GetOffsets()
        {
            if (_positionValues.Count == 1)
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
    }
}
