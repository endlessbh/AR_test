using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.Points
{
    /// <summary>
    /// 线段
    /// </summary>
    [Name("线段")]
    [RequireManager(typeof(ToolsManager))]
    public class Segment : InteractableVirtual
    {
        /// <summary>
        /// 循环
        /// </summary>
        [Name("循环")]
        [Tip("为True时，线段首尾相连形成多边形；为False时，线段首尾不相连为折线段", "When true, line segments are connected end to end to form polygons; When it is false, the beginning and end of the line segment are not connected, and it is a broken line segment")]
        [SerializeField]
        private bool _loop = false;

        /// <summary>
        /// 循环
        /// </summary>
        public bool loop
        {
            get { return _loop; }
            set
            {
                _loop = value;

                // 重构前后链接
                var last = lastPoint;
                if (last)
                {
                    if (_loop)
                    {
                        last.ConnectNext(firstPoint);
                    }
                    else
                    {
                        last.DisconnectNext();
                    }
                }
            }
        }

        /// <summary>
        /// 启用后顺序创建点连接
        /// </summary>
        [Name("启用后顺序创建点连接")]
        [Tip("当线段点集已经预设了点对象，则自动创建点与点的连接关系", "When the point object has been preset in the point set of the line segment, the connection relationship between points is automatically created")]
        public bool _createPointConectionOnEnable = true;

        /// <summary>
        /// 线段点集
        /// </summary>
        [Name("线段点集")]
        public List<Point> _points = new List<Point>();

        /// <summary>
        /// 第一个点
        /// </summary>
        public Point firstPoint => _points.FirstOrDefault();

        /// <summary>
        /// 最后一个点
        /// </summary>
        public Point lastPoint => _points.LastOrDefault();

        /// <summary>
        /// 点数量
        /// </summary>
        public int pointCount => _points.Count;

        public Vector3[] positions
        {
            get
            {
                var count = pointCount;
                if (_positions.Length != count)
                {
                    _positions = new Vector3[count];
                }
                for (int i = 0; i < count; i++)
                {
                    _positions[i] = _points[i].position;
                }
                return _positions;
            }
        }
        private Vector3[] _positions = new Vector3[0];

        /// <summary>
        /// 线段总长度
        /// </summary>
        public float length => _points.Sum(p => p.nextDistance);

        /// <summary>
        /// 点集中心
        /// </summary>
        public Vector3 center
        {
            get
            {
                var tp = Vector3.zero;

                _points.ForEach(point => tp += point.position);

                var count = pointCount;
                if (count > 0)
                {
                    tp /= count;
                }
                return tp;
            }
        }

        public override List<string> cmds => throw new NotImplementedException();

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            if (_createPointConectionOnEnable)
            {
                ConnectPoints();
            }
        }

        /// <summary>
        /// 讲点集合重新串联起来
        /// </summary>
        private void ConnectPoints()
        {
            var tmp = new List<Point>(_points);
            _points.Clear();
            foreach (var p in tmp)
            {
                if (p && p.enabled && p.gameObject.activeInHierarchy) // 只加入激活节点
                {
                    Add(p);
                }
            }
        }

        /// <summary>
        /// 点变化回调函数：参数1=当前线段，参数2=点，参数3=增加或减少
        /// </summary>
        public static event Action<Segment, Point, bool> onPointChanged;

        /// <summary>
        /// 增加点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Add(Point point)
        {
            if (!point || _points.Contains(point)) return false;

            // 关联前后点
            var last = lastPoint;
            if (last)
            {
                last.ConnectNext(point);
            }

            if (_loop)
            {
                point.ConnectNext(firstPoint);
            }

            // 将点加入当前线段管理中
            point._segment = this;
            _points.Add(point);

            // 点修改事件回调
            onPointChanged?.Invoke(this, point, true);

            return true;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Insert(int index, Point point)
        {
            if (!point || _points.Contains(point) || index < 0 || index > pointCount) return false;

            point.ConnectPrevious(index > 0 ? _points[index - 1] : null);
            point.ConnectNext(index < pointCount ? _points[index] : null);

            // 将点加入当前线段管理中
            point._segment = this;
            _points.Insert(index, point);

            // 点修改事件回调
            onPointChanged?.Invoke(this, point, true);
            return true;
        }

        /// <summary>
        /// 移除点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Remove(Point point)
        {
            if (!point || !_points.Contains(point)) return false;

            // 将前一个点和后一个点进行关联
            var previous = point._previousPoint;
            var next = point._nextPoint;

            // 解除前后点关联
            point.DisconnectPrevious();
            point.DisconnectNext();

            if (previous)
            {
                previous.ConnectNext(next);
            }

            if (next)
            {
                next.ConnectPrevious(previous);
            }

            // 将点从当前线段管理中移除
            point._segment = null;
            _points.Remove(point);

            // 点修改事件回调
            onPointChanged?.Invoke(this, point, false);

            return true;
        }
    }
}
