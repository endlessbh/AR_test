using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.ViewControllers;

namespace XCSJ.PluginTools.Points
{
    /// <summary>
    /// 线段创建器视图控制器
    /// </summary>
    [Name("线段创建器视图控制器")]
    public class SegmentCreaterViewController : BaseViewController
    {
        /// <summary>
        /// 线段创建器
        /// </summary>
        [Name("线段创建器")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public SegmentCreater _segmentCreater;

        /// <summary>
        /// 线段模版上的可视化组件
        /// </summary>
        private SegmentVisual segmentVisual;

        /// <summary>
        /// 唤醒
        /// </summary>
        protected virtual void Awake()
        {
            segmentVisual = _segmentCreater._segmentTemplate.GetComponent<SegmentVisual>();
            if (!segmentVisual)
            {
                Debug.LogError("线段模版无可视化组件");
            }
        }

        /// <summary>
        /// 测量中
        /// </summary>
        public bool measureWorking
        {
            get
            {
                return _segmentCreater.recording;
            }
            set
            {
                if (_segmentCreater.recording)
                {
                    _segmentCreater.EndPick();
                }
                if (value)
                {
                    _segmentCreater.BeginPick();
                }
            }
        }

        /// <summary>
        /// 记录测量点
        /// </summary>
        public void Record()
        {
            _segmentCreater.RecordPoint();
        }

        [Name("测量类型")]
        public EMeasureType measureType
        {
            get
            {
                return _measureType;
            }
            set 
            {
                if (_measureType != value)
                {
                    _measureType = value;

                    segmentVisual.SetMeasureType(_measureType);

                    _segmentCreater.SetMeasureType(_measureType);
                }
            }
        }
        public EMeasureType _measureType = EMeasureType.Length;

        [Name("长度单位")]
        public SegmentVisual.ELengthUnit lengthUnit
        {
            get
            {
                return segmentVisual._lengthUnit;
            }
            set
            {
                segmentVisual._lengthUnit = value;
            }
        }

        /// <summary>
        /// 删除所有线段
        /// </summary>
        public void DeleteAllSegment()
        {
            _segmentCreater.DeleteAllSegment();
        }

        /// <summary>
        /// 删除选择点
        /// </summary>
        public void DeleteSelectedPoint()
        {

            _segmentCreater.DeleteSelectedPoint();
        }

        /// <summary>
        /// 删除选择点所在线段
        /// </summary>
        public void DeleteSelectedSegment()
        {
            _segmentCreater.DeleteSelectedSegment();
        }
    }
}
