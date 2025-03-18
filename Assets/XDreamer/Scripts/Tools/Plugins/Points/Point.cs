using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Points
{
    /// <summary>
    /// 交互点：属于线段并指向前一个点和后一个点
    /// </summary>
    [Name("交互点")]
    [Tip("属于线段并指向前一个点和后一个点", "Belongs to a line segment and points to the previous point and the next point")]
    [RequireManager(typeof(ToolsManager))]
    public class Point : InteractableVirtual
    {
        /// <summary>
        /// 所属线段
        /// </summary>
        [Name("所属线段")]
        public Segment _segment;

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position { get => transform.position; set => transform.position = value; }

        #region 当前点与前一个点

        [Name("上一个点")]
        public Point _previousPoint;

        /// <summary>
        /// 当前点到前一个点方向
        /// </summary>
        public Vector3 previousDirection
        {
            get
            {
                if (_previousPoint)
                {
                    return _previousPoint.position - position;
                }
                return Vector3.zero;
            }
        }

        /// <summary>
        /// 当前点到前一个点距离
        /// </summary>
        public float previousDistance
        {
            get
            {
                if (_previousPoint)
                {
                    return Vector3.Distance(position, _previousPoint.position);
                }
                return 0;
            }
        }

        /// <summary>
        /// 当前点到前一个点中心
        /// </summary>
        public Vector3 previousCenter
        {
            get
            {
                if (_previousPoint)
                {
                    return (position + _previousPoint.position) / 2;
                }
                return Vector3.zero;
            }
        }

        #endregion

        #region 当前点与后一个点

        [Name("后一个点")]
        public Point _nextPoint;

        /// <summary>
        /// 当前点到后一个点方向
        /// </summary>
        public Vector3 nextDirection
        {
            get
            {
                if (_nextPoint)
                {
                    return _nextPoint.position - position;
                }
                return Vector3.zero;
            }
        }

        /// <summary>
        /// 当前点到后一个点距离
        /// </summary>
        public float nextDistance
        {
            get
            {
                if (_nextPoint)
                {
                    return Vector3.Distance(position, _nextPoint.position);
                }
                return 0;
            }
        }

        /// <summary>
        /// 当前点与后一个点中心点
        /// </summary>
        public Vector3 nextCenter
        {
            get
            {
                if (_nextPoint)
                {
                    return (position + _nextPoint.position) / 2;
                }
                return Vector3.zero;
            }
        }

        #endregion

        #region 前一个点与后一个点

        /// <summary>
        /// 前一个点到后一个点方向
        /// </summary>
        public Vector3 previousNextDirection
        {
            get
            {
                if (_previousPoint && _nextPoint)
                {
                    return _nextPoint.position - _previousPoint.position;
                }
                return Vector3.zero;
            }
        }

        /// <summary>
        /// 前一个点与后一个点距离
        /// </summary>
        public float previousNextDistance
        {
            get
            {
                if (_previousPoint && _nextPoint)
                {
                    return Vector3.Distance(_previousPoint.position, _nextPoint.position);
                }
                return 0;
            }
        }

        /// <summary>
        /// 前一个点与后一个点中心点
        /// </summary>
        public Vector3 previousNextCenter
        {
            get
            {
                if (_previousPoint && _nextPoint)
                {
                    return (_previousPoint.position + _nextPoint.position) / 2;
                }
                return Vector3.zero;
            }
        }

        #endregion

        public bool angleValid => _previousPoint && _previousPoint != this && _nextPoint && _nextPoint != this;

        /// <summary>
        /// 前一个点方向到后一个点方向的夹角
        /// </summary>
        public float angle
        {
            get
            {
                if (angleValid)
                {
                    return Vector3.Angle(previousDirection, nextDirection);
                }
                return 0;
            }
        }

        public override List<string> cmds => throw new System.NotImplementedException();

        /// <summary>
        /// 从前一个点方向到后一个点方向的有向角度
        /// </summary>
        public float SignedAngle(Vector3 axis)
        {
            if (angleValid)
            {
                return Vector3.SignedAngle(previousDirection, nextDirection, axis);
            }
            return 0;
        }

        /// <summary>
        /// 启用：加入所属线段
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_segment)
            {
                _segment.Add(this);
            }
        }

        /// <summary>
        /// 禁用：从所属线段中移除
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            RemoveFromSegement();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public void ResetData()
        {
            _segment = null;
            _previousPoint = null;
            _nextPoint = null;
        }

        public void RemoveFromSegement()
        {
            if (_segment)
            {
                _segment.Remove(this);
            }
        }

        /// <summary>
        /// 连接前一个点
        /// </summary>
        /// <param name="previous"></param>
        public void ConnectPrevious(Point previous)
        {
            _previousPoint = previous;

            if (previous)
            {
                previous._nextPoint = this;
            }
        }

        /// <summary>
        /// 断开前一个点
        /// </summary>
        public void DisconnectPrevious()
        {
            if (_previousPoint)
            {
                if (_previousPoint._nextPoint == this)
                {
                    _previousPoint._nextPoint = null;
                }

                _previousPoint = null;
            }
        }

        /// <summary>
        /// 连接下一个点
        /// </summary>
        /// <param name="next"></param>
        public void ConnectNext(Point next)
        {
            _nextPoint = next;

            if (next)
            {
                next._previousPoint = this;
            }
        }

        /// <summary>
        /// 断开下一个点
        /// </summary>
        public void DisconnectNext()
        {
            if (_nextPoint)
            {
                if (_nextPoint._previousPoint == this)
                {
                    _nextPoint._previousPoint = null;
                }

                _nextPoint = null;
            }
        }
    }
}
