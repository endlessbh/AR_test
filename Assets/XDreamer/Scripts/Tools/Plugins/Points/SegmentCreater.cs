using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Base;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginCommonUtils.Interactions;
using XCSJ.PluginCommonUtils.Runtime;
using XCSJ.PluginTools.Inputs;

namespace XCSJ.PluginTools.Points
{
    /// <summary>
    /// 线段创建命令
    /// </summary>
    public enum ESegmentCreateCmd
    {
        [Name("开始测量")]
        BeginMeasure,

        [Name("记录点")]
        RecordPoint,

        [Name("结束测量")]
        EndMeasure,

        [Name("射线碰撞点")]
        RayHitPoint,
    }

    [Serializable]
    public class SegmentCreateCmd : Cmd<ESegmentCreateCmd> { }

    [Serializable]
    public class SegmentCreateCmds : Cmds<ESegmentCreateCmd, SegmentCreateCmd> { }

    /// <summary>
    /// 线段创建器
    /// </summary>
    [Name("线段创建器")]
    [DisallowMultipleComponent]
    [RequireManager(typeof(ToolsManager))]
    public class SegmentCreater : Interactor
    {
        /// <summary>
        /// 线段创建命令
        /// </summary>
        [Name("线段创建命令")]
        [OnlyMemberElements]
        public SegmentCreateCmds _segmentCreateterCmd = new SegmentCreateCmds();

        /// <summary>
        /// 全部命令
        /// </summary>
        public override List<string> cmds => _segmentCreateterCmd.cmdNames;

        /// <summary>
        /// 获取工作命令
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override List<string> GetWorkCmds(InteractData interactData)
        {
            if (recording)
            {
                return _segmentCreateterCmd.GetCmdNames(ESegmentCreateCmd.RecordPoint, ESegmentCreateCmd.EndMeasure, ESegmentCreateCmd.RayHitPoint);
            }
            else
            {
                return _segmentCreateterCmd.GetCmdNames(ESegmentCreateCmd.BeginMeasure);
            }
        }

        /// <summary>
        /// 能否交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        public override bool CanInteract(InteractData interactData) => GetWorkCmds(interactData).Contains(interactData.cmdName);

        /// <summary>
        /// 尝试交互
        /// </summary>
        /// <param name="interactData"></param>
        /// <returns></returns>
        protected override EInteractResult OnInteract(InteractData interactData)
        {
            if (_segmentCreateterCmd.TryGetECmd(interactData.cmdName, out var eCmd))
            {
                switch (eCmd)
                {
                    case ESegmentCreateCmd.BeginMeasure: BeginPick(); return EInteractResult.Finished;
                    case ESegmentCreateCmd.RecordPoint: RecordPoint(); return EInteractResult.Finished;
                    case ESegmentCreateCmd.EndMeasure: EndPick(); return EInteractResult.Finished;
                    case ESegmentCreateCmd.RayHitPoint: RayHitPoint(interactData); return EInteractResult.Finished;
                }
            }
            return EInteractResult.Aborted;
        }

        /// <summary>
        /// 点最小数量
        /// </summary>
        [Group("属性设置", textEN = "Property Settings")] 
        [Name("点最小数量")]
        [Min(2)]
        public int _minCount = 2;

        /// <summary>
        /// 点最大数量
        /// </summary>
        [Name("点最大数量")]
        [Min(2)]
        public int _maxCount = 100;

        /// <summary>
        /// 点模版
        /// </summary>
        [Name("点模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Point _pointTemplate = null;

        /// <summary>
        /// 当前拾取点
        /// </summary>
        [Name("当前拾取点")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Point _currentPoint = null;

        /// <summary>
        /// 拾取点偏移量
        /// </summary>
        [Name("拾取点偏移量")]
        public Vector3 _positionOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// 线段模版
        /// </summary>
        [Name("线段模版")]
        public Segment _segmentTemplate = null;

        /// <summary>
        /// 高度测量模式
        /// </summary>
        [Name("高度测量模式")]
        public bool _heightMeasureMode = false;

        /// <summary>
        /// 初始高度
        /// </summary>
        [Name("初始高度")]
        [HideInSuperInspector(nameof(_heightMeasureMode), EValidityCheckType.False)]
        public float _height = 0.5f;

        public bool recording => currentSegment;

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            EndPick();
        }

        /// <summary>
        /// 开始拾取
        /// </summary>
        public void BeginPick()
        {
            if (recording) return;

            CreateSegment();
            _currentPoint.gameObject.SetActive(true);
        }

        /// <summary>
        /// 记录当前点：记录点超过最大值，重新创建一个新线段
        /// </summary>
        public void RecordPoint()
        {
            if (!recording) return;

            currentSegment.Remove(_currentPoint);
            if (_heightMeasureMode)
            {
                CreatePoint(_currentPoint.position);
                CreatePoint(_currentPoint.position + new Vector3(0, _height, 0));
            }
            else
            {
                CreatePoint(_currentPoint.position);
            }
            currentSegment.Add(_currentPoint);

            // 记录点超过最大值，重新创建一个新线段
            if (currentSegment.pointCount-1 >= _maxCount)
            {
                currentSegment.Remove(_currentPoint);
                CreateSegment();
            }
        }

        private void RayHitPoint(InteractData interactData)
        {
            var rayInteractData = interactData as RayInteractData;
            if (rayInteractData != null && rayInteractData.raycastHit != null && rayInteractData.raycastHit.HasValue)
            {
                _currentPoint.position = rayInteractData.raycastHit.Value.point + _positionOffset;
            }
        }

        /// <summary>
        /// 结束拾取
        /// </summary>
        public void EndPick()
        {
            if (!recording) return;

            if (currentSegment)
            {
                currentSegment.Remove(_currentPoint);

                if (currentSegment.pointCount < _minCount)
                {
                    currentSegment.gameObject.XDestoryObject();
                }
            }
            if (_currentPoint)
            {
                _currentPoint.ResetData();
                _currentPoint.gameObject.SetActive(false);
            }

            currentSegment = null;
        }

        [Readonly]
        [EndGroup(false)]
        public List<GameObject> createSegments = new List<GameObject>();

        private void CreateSegment()
        {
            GameObject segmentGO = null;
            if (_segmentTemplate)
            {
                segmentGO = _segmentTemplate.gameObject.XCloneObject<GameObject>();
                currentSegment = segmentGO.GetComponent<Segment>();
            }
            else
            {
                segmentGO = UnityObjectHelper.CreateGameObject();
                currentSegment = segmentGO.XAddComponent<Segment>();
            }
            segmentGO.XSetParent(transform);
            segmentGO.XSetUniqueName(CommonFun.Name(typeof(Segment)));
            segmentGO.XSetActive(true);
            createSegments.Add(segmentGO);  
        }

        private void CreatePoint(Vector3 position)
        {
            var go = _pointTemplate.gameObject.XCloneObject();
            go.name = _pointTemplate.name;
            go.XSetActive(true);
            var point = go.GetComponent<Point>();
            point.transform.position = position;
            if(currentSegment.Add(point))
            {
                point.transform.SetParent(currentSegment.transform);
                point.gameObject.XSetUniqueName(point.name);
            }
        }

        private Segment currentSegment;

        /// <summary>
        /// 删除所有创建线段
        /// </summary>
        public void DeleteAllSegment()
        {
            EndPick();

            foreach (var s in createSegments)
            {
                if(s) UnityObjectHelper.XDestoryObject(s);
            }
        }

        /// <summary>
        /// 删除选择点
        /// </summary>
        public void DeleteSelectedPoint()
        {
            EndPick();

            var go = Selection.selection;
            if (go)
            {
                var point = go.GetComponentInChildren<Point>();
                if (point)
                {
                    point.RemoveFromSegement();
                    UnityObjectHelper.XDestoryObject(point.gameObject);
                }
            }
        }

        /// <summary>
        /// 删除选择线段
        /// </summary>
        public void DeleteSelectedSegment()
        {
            EndPick();

            var go = Selection.selection;
            if (go)
            {
                var point = go.GetComponentInChildren<Point>();
                if (point && point._segment)
                {
                    var goOfSegment = point._segment.gameObject;
                    createSegments.Remove(goOfSegment);
                    UnityObjectHelper.XDestoryObject(goOfSegment);
                }
            }
        }

        /// <summary>
        /// 设置测量类型
        /// </summary>
        /// <param name="measureType"></param>
        public void SetMeasureType(EMeasureType measureType)
        {
            switch (measureType)
            {
                case EMeasureType.Length:
                    {
                        _minCount = 2;
                        _maxCount = 100;
                        break;
                    }
                case EMeasureType.Height:
                    {
                        _maxCount = _minCount = 2;
                        break;
                    }
                case EMeasureType.Angle:
                    {
                        _maxCount = _minCount = 3;
                        break;
                    }
                case EMeasureType.Area:
                    {
                        _minCount = 3;
                        _maxCount = 100;
                        break;
                    }
                case EMeasureType.LengthAngle:
                    {
                        _minCount = 2;
                        _maxCount = 100;
                        break;
                    }
                case EMeasureType.LengthAngleArea:
                    {
                        _minCount = 2;
                        _maxCount = 100;
                        break;
                    }
            }

            _heightMeasureMode = measureType == EMeasureType.Height;
            _segmentTemplate.loop = (measureType == EMeasureType.Area || measureType == EMeasureType.LengthAngleArea);

            if (recording)
            {
                EndPick();
                BeginPick();
            }
        }
    }

    public enum EMeasureType
    {
        [Name("长度")]
        Length,

        [Name("高度")]
        Height,

        [Name("角度")]
        Angle,

        [Name("面积")]
        Area,

        [Name("长度角度")]
        LengthAngle,

        [Name("长度角度面积")]
        LengthAngleArea,
    }
}
