using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Algorithms;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTimelines.Tools
{
    /// <summary>
    /// 延伸线
    /// </summary>
    [Name("延伸线", nameof(SpreadLine))]
    [Tool(TimelineManager.Title, rootType = typeof(TimelineManager), purposes = new string[] { TimelineManager.PlayableContent })]
    [XCSJ.Attributes.Icon(EIcon.Path)]
    [RequireManager(typeof(TimelineManager))]
    [Owner(typeof(TimelineManager))]
    public class SpreadLine : PlayableContent
    {
        /// <summary>
        /// 线渲染器
        /// </summary>
        [Name("线渲染器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public LineRenderer _line;

        /// <summary>
        /// 自定义线路径
        /// </summary>
        [Name("自定义线路径")]
        public bool _customLinePath = false;

        /// <summary>
        /// 线路径
        /// </summary>
        [Name("线路径")]
        [ValidityCheck(EValidityCheckType.ElementCountGreater, 0)]
        [HideInSuperInspector(nameof(_customLinePath), EValidityCheckType.False)]
        public List<Transform> _linePath = new List<Transform>();

        private List<Vector3> linePositions = new List<Vector3>();
        private List<float> lineLengths = new List<float>();
        private float totalLength = 0;

        /// <summary>
        /// 重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            if (!_line) _line = GetComponent<LineRenderer>(); 
        }

        /// <summary>
        /// 需重新记录线渲染器数据
        /// </summary>
        public bool needRecordLinePosition { get; set; } = true;

        /// <summary>
        /// 启用时
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_line)
            {
                if (needRecordLinePosition)
                {
                    needRecordLinePosition = false;

                    // 记录初始线段信息
                    linePositions.Clear();
                    lineLengths.Clear();
                    totalLength = 0;

                    if (_customLinePath)
                    {
                        var path = _linePath.Where(item => item).ToList();
                        var count = path.Count;
                        for (int i = 0; i < count; i++)
                        {
                            var currentPoint = path[i];
                            if (currentPoint && i < count - 1)
                            {
                                var length = Vector3.Distance(currentPoint.position, path[i + 1].position);
                                lineLengths.Add(length);
                                totalLength += length;
                            }
                            linePositions.Add(currentPoint.position);
                        }
                    }
                    else
                    {
                        var count = _line.positionCount;
                        for (int i = 0; i < count; i++)
                        {
                            var currentPoint = _line.GetPosition(i);
                            if (i < count - 1)
                            {
                                var length = Vector3.Distance(currentPoint, _line.GetPosition(i + 1));
                                lineLengths.Add(length);
                                totalLength += length;
                            }
                            linePositions.Add(currentPoint);
                        }
                    }
                }

                // 启用时设置为0
                _line.positionCount = 0;
            }
        }

        /// <summary>
        /// 设置百分比回调
        /// </summary>
        /// <param name="percent"></param>
        public override void OnSetPercent(Percent percent, PlayableData playableData)
        {
            var currentLength = totalLength * percent.percent01OfWorkCurve;

            var count = 0;
            while (currentLength > 0 && count < lineLengths.Count)
            {
                currentLength -= lineLengths[count++];
            }

            if (count > 0)
            {
                _line.positionCount = count + 1;
                for (int i = 0; i < count; i++)
                {
                    _line.SetPosition(i, linePositions[i]);
                }

                var startIndex = count - 1;
                var startPoint = linePositions[startIndex];
                var dir = (linePositions[count] - startPoint).normalized;
                _line.SetPosition(count, startPoint + dir * (float)(lineLengths[startIndex] + currentLength));
            }
        }
    }
}
