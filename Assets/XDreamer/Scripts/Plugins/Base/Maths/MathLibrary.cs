using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XCSJ.Attributes;

namespace XCSJ.Extension.Base.Maths
{
    /// <summary>
    /// 数学库
    /// </summary>
    public static class MathLibrary
    {
        /// <summary>
        /// Returns the direction adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>
        /// <param name="direction">The direction to be adjusted.</param>
        /// <param name="normal">The surface normal.</param>
        /// <param name="up">The given up-axis.</param>        
        public static Vector3 GetTangent(Vector3 direction, Vector3 normal, Vector3 up)
        {
            var right = Vector3.Cross(direction, up);
            var tangent = Vector3.Cross(normal, right);

            return tangent.normalized;
        }

        /// <summary>
        /// Returns the direction adjusted to be tangent to a specified surface normal relatively to the world's up axis.
        /// </summary>
        /// <param name="direction">The direction to be adjusted.</param>
        /// <param name="normal">The surface normal.</param>
        public static Vector3 GetTangent(Vector3 direction, Vector3 normal)
        {
            return GetTangent(direction, normal, Vector3.up);
        }

        /// <summary>
        /// 获取小数位格式;如0.000
        /// </summary>
        /// <param name="decimalPlaces">小数位的位数</param>
        /// <returns></returns>
        public static string ToDecimalFormat(this int decimalPlaces)
        {
            string formatString = "0.";
            for (int i = 0; i < decimalPlaces; i++)
            {
                formatString += "0";
            }
            return formatString;
        }

        /// <summary>
        /// 使用海伦公式求解三个点构成的三角形面积
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        public static float GetArea(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            var a = Vector3.Distance(point1, point2);
            var b = Vector3.Distance(point2, point3);
            var c = Vector3.Distance(point3, point1);
            var p = (a + b + c) / 2;
            return Mathf.Sqrt((p * (p - a) * (p - b) * (p - c)));
        }

        #region HSV

        /// <summary>
        /// 将颜色转为HSV，三维向量中XYZ值分别对应HSV值；
        /// 色调、饱和度、明度 0~1
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>HSV值</returns>
        public static Vector3 ToNormalizedHSV(this Color color)
        {
            Vector3 hsv = new Vector3();
            Color.RGBToHSV(color, out hsv.x, out hsv.y, out hsv.z);
            return hsv;
        }

        /// <summary>
        /// 将颜色转为HSV，三维向量中XYZ值分别对应HSV值；
        /// 色调 X:H 0~360
        /// 饱和度 Y: S 0~100
        /// 明度 Z: V 0~100
        /// </summary>
        /// <param name="color">颜色</param>
        /// <returns>HSV值</returns>
        public static Vector3Int ToHSV(this Color color)
        {
            Vector3 hsv = ToNormalizedHSV(color);
            return new Vector3Int((int)(hsv.x * 360), (int)(hsv.y * 100), (int)(hsv.z * 100));
        }

        /// <summary>
        /// 将HSV转为颜色，三维向量中XYZ值分别对应HSV值；
        /// </summary>
        /// <param name="hsv">HSV值 0~1</param>
        /// <returns>颜色</returns>
        public static Color HSVToColor(this Vector3 hsv)
        {
            return Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
        }

        /// <summary>
        /// 将HSV转为颜色，三维向量中XYZ值分别对应HSV值；
        /// 色调 X:H 0~360
        /// 饱和度 Y: S 0~100
        /// 明度 Z: V 0~100
        /// </summary>
        /// <param name="hsv">HSV值</param>
        /// <returns>颜色</returns>
        public static Color HSVToColor(this Vector3Int hsv)
        {
            return Color.HSVToRGB(hsv.x / 360f, hsv.y / 100f, hsv.z / 100f);
        }

        #endregion

        #region 点与线

        /// <summary>
        /// 获取点在直线上最近的点
        /// </summary>
        /// <param name="lineBegin"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnLine(Vector3 lineBegin, Vector3 lineEnd, Vector3 point)
        {
            return new Ray(lineBegin, lineEnd - lineBegin).GetClosestPointOnRay(point);
        }

        /// <summary>
        /// 获取点在线段上最近的点;可能是已知点在线段上的投影点，也可能是线段的起点或终点
        /// </summary>
        /// <param name="lineBegin"></param>
        /// <param name="lineEnd"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnLineSegment(Vector3 lineBegin, Vector3 lineEnd, Vector3 point)
        {
            var pointOnLine = GetClosestPointOnLine(lineBegin, lineEnd, point);

            if (Vector3.Dot(pointOnLine - lineBegin, pointOnLine - lineEnd) <= 0) return pointOnLine;

            var disBegin = Vector3.SqrMagnitude(pointOnLine - lineBegin);
            var disEnd = Vector3.SqrMagnitude(pointOnLine - lineEnd);

            return disBegin < disEnd ? lineBegin : lineEnd;
        }

        /// <summary>
        /// 获取点到线的距离
        /// http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="v">线段v点</param>
        /// <param name="w">线段w点</param>
        /// <returns></returns>
        public static float DistancePointLineSegment(Vector2 p, Vector2 v, Vector2 w)
        {
            // Return minimum distance between line segment vw and point p
            float l2 = ((v.x - w.x) * (v.x - w.x)) + ((v.y - w.y) * (v.y - w.y));  // i.e. |w-v|^2 -  avoid a sqrt

            if (l2 == 0.0f) return Vector2.Distance(p, v);   // v == w case

            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line.
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            float t = Vector2.Dot(p - v, w - v) / l2;

            if (t < 0.0) return Vector2.Distance(p, v);             // Beyond the 'v' end of the segment
            else if (t > 1.0) return Vector2.Distance(p, w);            // Beyond the 'w' end of the segment

            Vector2 projection = v + t * (w - v);   // Projection falls on the segment

            return Vector2.Distance(p, projection);
        }

        #endregion

        #region 点与多边形

        /// <summary>
        /// 获取点在多边形上最近的点
        /// </summary>
        /// <param name="lines">多边形的点列表</param>
        /// <param name="point"></param>
        /// <param name="defalutPoint">获取最近点失败时返回本值</param>
        /// <returns></returns>
        public static Vector3 GetClosestPointOnPolygon(this Vector3[] lines, Vector3 point, Vector3 defalutPoint)
        {
            return TryGetClosestPointOnPolygon(lines, point, out Vector3 closestPoint) ? closestPoint : defalutPoint;
        }

        /// <summary>
        /// 尝试获取点在多边形上最近的点
        /// </summary>
        /// <param name="lines">多边形的点列表</param>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns></returns>
        public static bool TryGetClosestPointOnPolygon(this Vector3[] lines, Vector3 point, out Vector3 closestPoint)
        {
            if (lines == null)
            {
                closestPoint = Vector3.zero;
                return false;
            }

            var length = lines.Length - 1;
            switch (length)
            {
                case 0:
                    {
                        closestPoint = lines[0];
                        return true;
                    }
                case 1:
                    {
                        closestPoint = GetClosestPointOnLineSegment(lines[0], lines[1], point);
                        return true;
                    }
                default:
                    {
                        var outPoint = lines[0];
                        var dis = float.MaxValue;
                        for (int i = 0; i < length; i++)
                        {
                            var p = GetClosestPointOnLineSegment(lines[i], lines[i + 1], point);
                            var tmpDis = Vector3.SqrMagnitude(point - p);
                            if (tmpDis < dis)
                            {
                                outPoint = p;
                                dis = tmpDis;
                            }
                        }

                        closestPoint = outPoint;
                        return true;
                    }
            }
        }

        /// <summary>
        /// 尝试获取点在多边形上最近的点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lines"></param>
        /// <param name="toPositionFunc"></param>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <param name="closedLines"></param>
        /// <returns></returns>
        public static bool TryGetClosestPointOnPolygon<T>(this IList<T> lines, Func<T, Vector3> toPositionFunc, Vector3 point, out Vector3 closestPoint, bool closedLines = false)
        {
            if (lines == null || lines.Count==0 || toPositionFunc == null)
            {
                closestPoint = default;
                return false;
            }

            var length = lines.Count - 1;
            switch (length)
            {
                case 0:
                    {
                        closestPoint = toPositionFunc(lines[0]);
                        return true;
                    }
                case 1:
                    {
                        closestPoint = GetClosestPointOnLineSegment(toPositionFunc(lines[0]), toPositionFunc(lines[1]), point);
                        return true;
                    }
                default:
                    {
                        var outPoint = default(Vector3);
                        var dis = float.MaxValue;
                        for (int i = 0; i < length; i++)
                        {
                            var p = GetClosestPointOnLineSegment(toPositionFunc(lines[i]), toPositionFunc(lines[i + 1]), point);
                            var tmpDis = Vector3.SqrMagnitude(point - p);
                            if (tmpDis < dis)
                            {
                                outPoint = p;
                                dis = tmpDis;
                            }
                        }
                        if (closedLines)
                        {
                            var p = GetClosestPointOnLineSegment(toPositionFunc(lines[0]), toPositionFunc(lines[length]), point);
                            var tmpDis = Vector3.SqrMagnitude(point - p);
                            if (tmpDis < dis)
                            {
                                outPoint = p;
                            }
                        }
                        closestPoint = outPoint;
                        return true;
                    }
            }
        }

        /// <summary>
        /// 尝试获取点在多边形上最近的点
        /// </summary>
        /// <param name="lines">多边形的点列表</param>
        /// <param name="point"></param>
        /// <param name="closestPoint">最近的点</param>
        /// <param name="closedLines">闭合多边形线，将队列中结尾与开头点再做一次最近点计算</param>
        /// <returns></returns>
        public static bool TryGetClosestPointOnPolygon(this IList<Vector3> lines, Vector3 point, out Vector3 closestPoint, bool closedLines = false)
        {
            if (lines == null)
            {
                closestPoint = Vector3.zero;
                return false;
            }

            var length = lines.Count - 1;
            switch (length)
            {
                case 0:
                    {
                        closestPoint = lines[0];
                        return true;
                    }
                case 1:
                    {
                        closestPoint = GetClosestPointOnLineSegment(lines[0], lines[1], point);
                        return true;
                    }
                default:
                    {
                        var outPoint = lines[0];
                        var dis = float.MaxValue;
                        for (int i = 0; i < length; i++)
                        {
                            var p = GetClosestPointOnLineSegment(lines[i], lines[i + 1], point);
                            var tmpDis = Vector3.SqrMagnitude(point - p);
                            if (tmpDis < dis)
                            {
                                outPoint = p;
                                dis = tmpDis;
                            }
                        }
                        if(closedLines)
                        {
                            var p = GetClosestPointOnLineSegment(lines[0], lines[length], point);
                            var tmpDis = Vector3.SqrMagnitude(point - p);
                            if (tmpDis < dis)
                            {
                                outPoint = p;
                            }
                        }
                        closestPoint = outPoint;
                        return true;
                    }
            }
        }

        /// <summary>
        /// 点是否在多边形内
        /// 从边界外将光线投射到多边形并检查有多少条边被击中
        /// 当交叉的边数不被2整除时，在多边形内部
        /// </summary>
        /// <param name="polygon">平面多边形</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public static bool PointInPolygon(Vector2[] polygon, Vector2 point)
        {
            float xmin = Mathf.Infinity, xmax = -Mathf.Infinity, ymin = Mathf.Infinity, ymax = -Mathf.Infinity;

            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].x < xmin) xmin = polygon[i].x;
                else if (polygon[i].x > xmax) xmax = polygon[i].x;

                if (polygon[i].y < ymin) ymin = polygon[i].y;
                else if (polygon[i].y > ymax) ymax = polygon[i].y;
            }

            if (point.x < xmin || point.x > xmax || point.y < ymin || point.y > ymax) return false;

            Vector2 rayStart = new Vector2(xmin - 1f, ymax + 1f);

            int collisions = 0;

            for (int i = 0; i < polygon.Length; i += 2)
            {
                if (IsLineSegmentIntersect(rayStart, point, polygon[i], polygon[i + 1]))
                    collisions++;
            }

            return collisions % 2 != 0;
        }

        /// <summary>
        /// 线段相交
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static bool IsLineSegmentIntersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 s1, s2;
            s1.x = p1.x - p0.x; 
            s1.y = p1.y - p0.y;
            s2.x = p3.x - p2.x; 
            s2.y = p3.y - p2.y;

            float s, t;
            s = (-s1.y * (p0.x - p2.x) + s1.x * (p0.y - p2.y)) / (-s2.x * s1.y + s1.x * s2.y);
            t = (s2.x * (p0.y - p2.y) - s2.y * (p0.x - p2.x)) / (-s2.x * s1.y + s1.x * s2.y);

            return (s >= 0 && s <= 1 && t >= 0 && t <= 1);
        }

        #region 求解凸轮廓

        /// <summary>
        /// 使用“Graham Scan算法”, 根据输入点集合获取凸多边形，凸多边形点顺序是逆时针方向
        /// </summary>
        /// <param name="inPoints">输入点集合：要求至少有一个点</param>
        /// <param name="outPoints">输出点集合：第一个点为最右下角</param>
        /// <returns>是否成功生成凸多边形</returns>
        public static bool TryGetConvexHull<T>(List<(Vector2, T)> inPoints, out List<(Vector2, T)> outPoints)
        {
            outPoints = new List<(Vector2, T)>();
            if (inPoints == null) return false;

            var count = inPoints.Count;
            if (count == 0) return false;

            if (count == 1)
            {
                outPoints.AddRange(inPoints);
                return true;
            }

            // 寻找到最右点
            var maxRihgtDownPoint = inPoints[0];
            var maxRightIndex = 0;
            for (int i = 1; i < count; i++)
            {
                var p = inPoints[i];

                if (p.Item1.x > maxRihgtDownPoint.Item1.x)
                {
                    maxRihgtDownPoint = p;
                    maxRightIndex = i;
                }
                else if (p.Item1.x == maxRihgtDownPoint.Item1.x)
                {
                    if (p.Item1.y < maxRihgtDownPoint.Item1.y)
                    {
                        maxRihgtDownPoint = p;
                        maxRightIndex = i;
                    }
                }
            }

            // 遍历所有点，对从原点p0到当前点的向量与上向量的构成的夹角大小进行升序排列
            var newPoints = new List<(Vector2, T)>();
            for (int i = 0; i < inPoints.Count; i++)
            {
                if (maxRightIndex == i) continue;

                var point = inPoints[i];
                var handle = false;
                for (int j = 0; j < newPoints.Count; j++)
                {
                    var rayPoint = newPoints[j];
                    var leftValue = GetLeftValue(maxRihgtDownPoint.Item1, rayPoint.Item1, point.Item1);
                    if (Mathf.Approximately(leftValue, 0))
                    {
                        var p2ToOrgPoint = (point.Item1 - maxRihgtDownPoint.Item1).sqrMagnitude;
                        var p1ToOrgPoint = (rayPoint.Item1 - maxRihgtDownPoint.Item1).sqrMagnitude;
                        if (p2ToOrgPoint > p1ToOrgPoint)
                        {
                            newPoints[j] = point;
                        }
                        handle = true;
                        break;
                    }
                    else if (leftValue < 0)// 此时p2点比p1点更靠近上向量，因此排列在前面
                    {
                        newPoints.Insert(j, point);
                        handle = true;
                        break;
                    }
                }
                if (!handle)
                {
                    newPoints.Add(point);
                }
            }

            if (newPoints.Count > 3)
            {
                // 求解凸多边形的点集合
                var stack = new Stack<(Vector2, T)>();
                stack.Push(maxRihgtDownPoint);
                stack.Push(newPoints[0]);
                int index = 1;
                while (index < newPoints.Count && stack.Count >= 2)
                {
                    var point = newPoints[index];
                    var rayPoint = stack.Pop();
                    var rayOrgin = stack.Peek();

                    var leftValue = GetLeftValue(rayOrgin.Item1, rayPoint.Item1, point.Item1);
                    if (Mathf.Approximately(leftValue, 0)) // 相等情况下，舍弃p1点，加入新点
                    {
                        stack.Push(point);
                        ++index;
                    }
                    else if (leftValue > 0) // p2此时在p0到p1的左侧，因此把之前弹出的p1和新点p2重新压入栈中
                    {
                        stack.Push(rayPoint);
                        stack.Push(point);
                        ++index;
                    }
                }

                // 逆时针获取输出点对象
                outPoints.AddRange(stack);
            }
            else
            {
                outPoints.Add(maxRihgtDownPoint);
                outPoints.AddRange(newPoints);
            }
            return true;
        }

        /// <summary>
        /// 获取凸轮廓
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<LineSegment<T>> GetConvexHullLines<T>(List<(Vector2, T)> inPoints)
        {
            var exitLines = new List<LineSegment<T>>();
            if (MathLibrary.TryGetConvexHull(inPoints, out var outPoints))
            {
                var count = outPoints.Count;
                for (int i = 0; i < count - 1; i++)
                {
                    var p1 = outPoints[i];
                    var p2 = outPoints[i+1];
                    exitLines.Add(new LineSegment<T>(p1.Item1, p2.Item1, p1.Item2, p2.Item2));
                }
                var lp1 = outPoints[count - 1];
                var lp2 = outPoints[0];
                exitLines.Add(new LineSegment<T>(lp1.Item1, lp2.Item1, lp1.Item2, lp2.Item2));
            }
            return exitLines;
        }

        /// <summary>
        /// 使用“Graham Scan算法”, 根据输入点集合获取凸多边形，凸多边形点顺序是逆时针方向
        /// </summary>
        /// <param name="inPoints">输入点集合：要求至少有一个点</param>
        /// <param name="outPoints">输出点集合：第一个点为最右下角</param>
        /// <returns>是否成功生成凸多边形</returns>
        public static bool TryGetConvexOutline(List<Vector2> inPoints, out List<Vector2> outPoints)
        {
            outPoints = new List<Vector2>();
            if (inPoints == null) return false;

            var count = inPoints.Count;
            if (count == 0) return false;

            if (count == 1)
            {
                outPoints.AddRange(inPoints);
                return true;
            }

            // 寻找到最右点
            var maxRihgtDownPoint = inPoints[0];
            var maxRightIndex = 0;
            for (int i = 1; i < count; i++)
            {
                var p = inPoints[i];

                if (p.x > maxRihgtDownPoint.x)
                {
                    maxRihgtDownPoint = p;
                    maxRightIndex = i;
                }
                else if (p.x == maxRihgtDownPoint.x)
                {
                    if (p.y < maxRihgtDownPoint.y)
                    {
                        maxRihgtDownPoint = p;
                        maxRightIndex = i;
                    }
                }
            }

            // 遍历所有点，对从原点p0到当前点的向量与上向量的构成的夹角大小进行升序排列
            var newPoints = new List<Vector2>();
            for (int i = 0; i < inPoints.Count; i++)
            {
                if (maxRightIndex == i) continue;

                var point = inPoints[i];
                var handle = false;
                for (int j = 0; j < newPoints.Count; j++)
                {
                    var rayPoint = newPoints[j];
                    var leftValue = GetLeftValue(maxRihgtDownPoint, rayPoint, point);
                    if (Mathf.Approximately(leftValue, 0))
                    {
                        var p2ToOrgPoint = (point - maxRihgtDownPoint).sqrMagnitude;
                        var p1ToOrgPoint = (rayPoint - maxRihgtDownPoint).sqrMagnitude;
                        if (p2ToOrgPoint > p1ToOrgPoint)
                        {
                            newPoints[j] = point;
                        }
                        handle = true;
                        break;
                    }
                    else if (leftValue < 0)// 此时p2点比p1点更靠近上向量，因此排列在前面
                    {
                        newPoints.Insert(j, point);
                        handle = true;
                        break;
                    }
                }
                if (!handle)
                {
                    newPoints.Add(point);
                }
            }

            if (newPoints.Count > 3)
            {
                // 求解凸多边形的点集合
                var stack = new Stack<Vector2>();
                stack.Push(maxRihgtDownPoint);
                stack.Push(newPoints[0]);
                int index = 1;
                while (index < newPoints.Count && stack.Count >= 2)
                {
                    var point = newPoints[index];
                    var rayPoint = stack.Pop();
                    var rayOrgin = stack.Peek();

                    var leftValue = GetLeftValue(rayOrgin, rayPoint, point);
                    if (Mathf.Approximately(leftValue, 0)) // 相等情况下，舍弃p1点，加入新点
                    {
                        stack.Push(point);
                        ++index;
                    }
                    else if (leftValue > 0) // p2此时在p0到p1的左侧，因此把之前弹出的p1和新点p2重新压入栈中
                    {
                        stack.Push(rayPoint);
                        stack.Push(point);
                        ++index;
                    }
                }

                // 逆时针获取输出点对象
                outPoints.AddRange(stack);
            }
            else
            {
                outPoints.AddRange(newPoints);
            }
            return true;
        }

        /// <summary>
        /// 在2D平面上，构建以rayOrgin为原点、以rayPoint为延长线上点的射线，求点point该射线的左边、线上或右边
        /// 计算结果:>0 点在射线左侧（逆时针方向）
        ///         =0 点在射线及反向延长线上
        ///         <0 点在射线右侧（顺时针方向）
        /// </summary>
        /// <param name="rayOrgin"></param>
        /// <param name="rayPoint"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static float GetLeftValue(Vector2 rayOrgin, Vector2 rayPoint, Vector2 point)
        {
            return CrossProduct(rayPoint - rayOrgin, point - rayOrgin);
        }

        /// <summary>
        /// 计算二维向量的叉乘
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static float CrossProduct(this Vector2 from, Vector2 to)
        {
            return from.x * to.y - to.x * from.y;
        }

        #endregion

        #region 求解凹轮廓

        /// <summary>
        /// 求解凹轮廓
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inPoints">输入点列表：元组中的第二个参数是线段两点携带的额外数据</param>
        /// <param name="concavity">插入点在线段两点之间的夹角：[0,180]</param>
        /// <param name="scaleFactor">线段搜索附近点的缩放因子</param>
        /// <param name="outPoints">输出点列表：元组中的第二个参数是线段两点携带的额外数据</param>
        /// <returns></returns>
        public static bool TryGetConcaveHull<T>(List<(Vector2, T)> inPoints, float angle, int scaleFactor, out List<(Vector2, T)> outPoints)
        {
            outPoints = new List<(Vector2, T)>();
            if (inPoints == null) return false;

            var count = inPoints.Count;
            if (count == 0) return false;

            if (count == 1)
            {
                outPoints.AddRange(inPoints);
                return true;
            }

            // 先求解凸多边形
            var convexHullEdges = GetConvexHullLines(inPoints);
            if (convexHullEdges.Count == 0) return false;

            // 使用凸多边形求解凹多边形
            var concavelines = GetConcaveHullLines(inPoints, convexHullEdges, angle, scaleFactor);
            if (concavelines.Count == 0) return false;

            // 通过凸多边形第一个线段的起始点，遍历查找后续
            var targetObj = convexHullEdges[0].n1;
            outPoints = new List<(Vector2, T)>();
            do
            {
                var index = concavelines.FindIndex(l => l.n1.Equals(targetObj));
                if (index >= 0)
                {
                    var targetLine = concavelines[index];
                    concavelines.RemoveAt(index);
                    outPoints.Add((targetLine.p1, targetLine.n1));
                    targetObj = targetLine.n2;
                }
                else
                {
                    break;
                }
            }
            while (concavelines.Count > 0);

            return outPoints.Count > 0;
        }

        /// <summary>
        /// 求解凹轮廓
        /// </summary>
        /// <param name="inPoints">输入点集</param>
        /// <param name="angle">插入点在线段两点之间的夹角:[0,180]</param>
        /// <param name="scaleFactor">线段搜索附近点的缩放因子</param>
        /// <returns>线段列表：元组中的第二和第三个参数是线段两点携带的额外数据</returns>
        public static List<LineSegment<T>> GetConcaveHullLines<T>(List<(Vector2, T)> inPoints, float angle, int scaleFactor)
        {
            return GetConcaveHullLines(inPoints, GetConvexHullLines(inPoints), angle, scaleFactor);
        }

        /// <summary>
        /// 求解凹轮廓
        /// </summary>
        /// <param name="inPoints">输入点集</param>
        /// <param name="convexHullEdges">凸多边形</param>
        /// <param name="angle">插入点在线段两点之间的夹角:[0,180]</param>
        /// <param name="scaleFactor">线段搜索附近点的缩放因子</param>
        /// <returns>线段列表：元组中的第二和第三个参数是线段两点携带的额外数据</returns>
        private static List<LineSegment<T>> GetConcaveHullLines<T>(List<(Vector2, T)> inPoints, List<LineSegment<T>> convexHullEdges, float angle, int scaleFactor)
        {
            var unusedPoints = new List<(Vector2, T)>();
            unusedPoints.AddRange(inPoints);
            foreach (var line in convexHullEdges)
            {
                unusedPoints.RemoveAll(p => line.n1.Equals(p.Item2) || line.n2.Equals(p.Item2));
            }
            bool aLineWasDividedInTheIteration;

            List<LineSegment<T>> concaveHullEdges = convexHullEdges.OrderByDescending(a => a.sqrMagnitude).ToList();
            do
            {
                aLineWasDividedInTheIteration = false;
                for (int i = 0; i < concaveHullEdges.Count; i++)
                {
                    var line = concaveHullEdges[i];
                    List<(Vector2, T, int)> nearbyPoints = GetNearbyPoints(line, unusedPoints, scaleFactor);
                    if (nearbyPoints.Count == 0) continue;

                    var middleIndex = FindDividedPointIndex(line, nearbyPoints, concaveHullEdges, angle);
                    if (middleIndex >= 0)
                    {
                        var middlePoint = nearbyPoints[middleIndex];

                        // 移除中间点和对应边
                        unusedPoints.RemoveAt(middlePoint.Item3);
                        concaveHullEdges.RemoveAt(i);

                        // 插入新的边
                        InsertOrderEdges(new LineSegment<T>(line.p1, middlePoint.Item1, line.n1, middlePoint.Item2), concaveHullEdges);
                        InsertOrderEdges(new LineSegment<T>(middlePoint.Item1, line.p2, middlePoint.Item2, line.n2), concaveHullEdges);
                        
                        // 继续迭代
                        aLineWasDividedInTheIteration = true;
                        break;
                    }
                }
            } while (aLineWasDividedInTheIteration);// 有线发生拆分

            return concaveHullEdges;
        }

        /// <summary>
        /// 二分查找
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortedList"></param>
        /// <param name="compare">等于0，返回对象；大于0，向右搜索；小于0，向左搜索</param>
        public static (int, T) BinarySearch<T>(this List<T> sortedList, Func<int, T, int> compare)
        {
            var r = sortedList.Count - 1;
            if (r < 0)
            {
                return (-1, default);
            }
            var l = 0;

            while (true)
            {
                if (l == r)
                {
                    var item = sortedList[l];
                    if (compare(l, item) == 0)
                    {
                        return (l, item);
                    }
                    return (-1, default);
                }

                var c = (l + r) / 2;
                var centerElement = sortedList[c];
                var result = compare(c, centerElement);
                if (result == 0)
                {
                    return (c, centerElement);
                }
                else if (result > 0)
                {
                    if (l != c)
                    {
                        l = c;
                    }
                    else
                    {
                        ++l;
                    }
                }
                else
                {
                    if (r != c)
                    {
                        r = c;
                    }
                    else
                    {
                        --r;
                    }
                }
            }
        }

        /// <summary>
        /// 升序添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sortedList"></param>
        /// <param name="element"></param>
        /// <param name="compare"></param>
        public static void AddWithSortAsc<T>(this List<T> sortedList, T element, Func<T, T, int> compare)
        {
            var r = sortedList.Count - 1;
            if (r < 0)
            {
                sortedList.Add(element);
                return;
            }
            var l = 0;

            while (true)
            {
                if (l == r)
                {
                    var item = sortedList[l];
                    if (compare(element, item) >= 0)
                    {
                        ++l;
                    }
                    sortedList.Insert(l, element);
                    return;
                }

                var c = (l + r) / 2;
                var centerElement = sortedList[c];
                if (compare(element, centerElement) >= 0)
                {
                    if (l != c)
                    {
                        l = c;
                    }
                    else
                    {
                        ++l;
                    }
                }
                else
                {
                    if (r!=c)
                    {
                        r = c;
                    }
                    else
                    {
                        --r;
                    }
                }
            }
        }

        private static void InsertOrderEdges<T>(LineSegment<T> line, List<LineSegment<T>> concaveHullEdges)
        {
            var rightIndex = concaveHullEdges.Count - 1;
            if (rightIndex < 0)
            {
                concaveHullEdges.Add(line);
                return;
            }
            var leftIndex = 0;

            while (true)
            {
                if (leftIndex == rightIndex)
                {
                    var item = concaveHullEdges[leftIndex];
                    if (line.sqrMagnitude <= item.sqrMagnitude)
                    {
                        ++leftIndex;
                    }
                    concaveHullEdges.Insert(leftIndex, line);
                    break;
                }

                var currentIndex = (leftIndex + rightIndex) / 2;
                var current = concaveHullEdges[currentIndex];
                if (line.sqrMagnitude > current.sqrMagnitude)
                {
                    if (rightIndex != currentIndex)
                    {
                        rightIndex = currentIndex;
                    }
                    else
                    {
                        --rightIndex;
                    }
                }
                else
                {
                    if (leftIndex != currentIndex)
                    {
                        leftIndex = currentIndex;
                    }
                    else
                    {
                        ++leftIndex;
                    }
                }
            }
        }

        private static List<(Vector2, T, int)> GetNearbyPoints<T>(LineSegment<T> line, List<(Vector2, T)> pointList, int scaleFactor)
        {
            /* The bigger the scaleFactor the more points it will return
             * Inspired by this precious algorithm:
             * http://www.it.uu.se/edu/course/homepage/projektTDB/ht13/project10/Project-10-report.pdf
             * Be carefull: if it's too small it will return very little points (or non!), 
             * if it's too big it will add points that will not be used and will consume time
             * */
            List<(Vector2, T, int)> nearbyPoints = new List<(Vector2, T, int)>();
            int tries = 0;

            var n1 = line.n1;
            var n2 = line.n2;
            var p1 = line.p1;
            var p2 = line.p2;
            var count = pointList.Count;
            while (tries < 2 && nearbyPoints.Count == 0)
            {
                float min_x_position = Mathf.Floor(Mathf.Min(p1.x, p2.x) / scaleFactor);
                float max_x_position = Mathf.Floor(Mathf.Max(p1.x, p2.x) / scaleFactor);

                float min_y_position = Mathf.Floor(Mathf.Min(p1.y, p2.y) / scaleFactor);
                float max_y_position = Mathf.Floor(Mathf.Max(p1.y, p2.y) / scaleFactor);

                for (int i = 0; i < count; i++)
                {
                    var node = pointList[i];
                    var point = node.Item1;

                    //Not part of the line
                    if (!n1.Equals(node.Item2) && !n2.Equals(node.Item2))
                    {
                        var node_x_rel_pos = Math.Floor(point.x / scaleFactor);
                        var node_y_rel_pos = Math.Floor(point.y / scaleFactor);

                        //Inside the boundary
                        if (node_x_rel_pos >= min_x_position && node_x_rel_pos <= max_x_position &&
                            node_y_rel_pos >= min_y_position && node_y_rel_pos <= max_y_position)
                        {
                            nearbyPoints.Add((node.Item1, node.Item2, i));
                        }
                    }
                }

                //if no points are found we increase the area
                scaleFactor = scaleFactor * 4 / 3;
                tries++;
            }
            return nearbyPoints;
        }

        /// <summary>
        /// 查找中间点，并返回临近点索引
        /// 如果查找失败，则线无法再拆分
        /// </summary>
        /// <param name="line"></param>
        /// <param name="nearbyPoints"></param>
        /// <param name="concaveHull"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static int FindDividedPointIndex<T>(LineSegment<T> line, List<(Vector2, T, int)> nearbyPoints, List<LineSegment<T>> concaveHull, float angle)
        {
            int middleIndex = -1;
            double middleAngle = 0;// 初始一个小角度

            for (int i = 0; i < nearbyPoints.Count; i++)
            {
                var middleNode = nearbyPoints[i];
                var middlePoint = middleNode.Item1;
                float a = line.GetAngle(middlePoint);

                // 夹角小于等于于限定角或与线段重合，则跳过
                if (a <= angle || LineCollidesWithHull(line.p1, middlePoint, line.p2, line.n1, middleNode.Item2, line.n2, concaveHull))
                {
                    continue;
                }

                // 使用中间点中夹角最大的点 (cos值最小的)
                if (middleIndex < 0 || a > middleAngle)
                {
                    middleIndex = i;
                    middleAngle = a;
                }
            }

            return middleIndex;
        }

        private static bool LineCollidesWithHull<T>(Vector2 p1, Vector2 middle, Vector2 p2, T d1, T md, T d2, List<LineSegment<T>> concave_hull)
        {
            foreach (var item in concave_hull)
            {
                var n1 = item.n1;
                var n2 = item.n2;
                if (!(d1.Equals(n1) || d1.Equals(n2) || md.Equals(n1) || md.Equals(n2))
                    && item.Intersect(p1, middle))
                {
                    return true;
                }

                if (!(md.Equals(n1) || md.Equals(n2) || d2.Equals(n1) || d2.Equals(n2))
                    && item.Intersect(middle, p2))
                {
                    return true;
                }

            }
            return false;
        }

        public struct LineSegment<T> 
        {
            public Vector2 p1;

            public T n1;

            public Vector2 p2;

            public T n2;

            public LineSegment(Vector2 p1, Vector2 p2, T n1, T n2)
            {
                this.p1 = p1;
                this.p2 = p2;
                this.n1 = n1;
                this.n2 = n2;

                sqrMagnitude = (p1 - p2).sqrMagnitude;
            }

            public double sqrMagnitude { get; private set; }

            public bool Intersect(Vector2 point1, Vector2 point2)
            {
                return MathLibrary.IsLineSegmentIntersect(p1, p2, point1, point2);
            }

            /// <summary>
            /// 线段两点与中间点的夹角
            /// </summary>
            /// <param name="middlePoint"></param>
            /// <returns></returns>
            public float GetAngle(Vector2 middlePoint)
            {
                return Vector2.Angle(p1 - middlePoint, p2 - middlePoint);
            }
        }

        #endregion

        #endregion

        #region 点与面

        /// <summary>
        /// Projects a given point onto the plane defined by plane origin and plane normal.
        /// </summary>
        /// <param name="point">The point to be projected.</param>
        /// <param name="planeOrigin">A point on the plane.</param>
        /// <param name="planeNormal">The plane normal.</param>
        public static Vector3 ProjectPointOnPlane(Vector3 point, Vector3 planeOrigin, Vector3 planeNormal)
        {
            var toPoint = point - planeOrigin;
            var toPointProjected = Vector3.Project(toPoint, planeNormal.normalized);

            return point - toPointProjected;
        }

        #endregion

        #region 点与球

        /// <summary>
        /// 获取点在球体上最近的点；最近点为球体内部或表面上的点
        /// </summary>
        /// <param name="center">圆心</param>
        /// <param name="radius">圆半径；如果半径小于等于0时，则最近点即为圆心</param>
        /// <param name="point">点；如果点在球体内部或球体表面时，则最近点即本参数点；如果点在球体外部，则最近点为本参数点在球体表面的投影点；</param>
        /// <returns>返回球体内部或是球体表面的点</returns>
        public static Vector3 GetClosestPointOnSphere(Vector3 center, float radius, Vector3 point)
        {
            if (radius <= 0) return center;

            var dir = point - center;
            if (dir.magnitude <= radius) return point;

            return center + dir.normalized * radius;
        }

        /// <summary>
        /// 获取点在球体表面上最近的点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns>当点与中心重合时，返回false</returns>
        public static bool TryGetClosestPointOnSphereSurface(Vector3 center, float radius, Vector3 point, out Vector3 closestPoint)
        {
            if (radius <= 0)
            {
                closestPoint = center;
                return true;
            }

            var dir = point - center;
            if (dir == Vector3.zero)
            {
                closestPoint = point;
                return false;
            }

            closestPoint = center + dir.normalized * radius;
            return true;
        }

        /// <summary>
        /// 获取点在球体表面上最近的点；即表面或外部的点；
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="point"></param>
        /// <param name="closestPoint"></param>
        /// <returns>当点与中心重合时，返回false</returns>
        public static bool TryGetClosestPointOnSphereNotInside(Vector3 center, float radius, Vector3 point, out Vector3 closestPoint)
        {
            if (radius <= 0)
            {
                closestPoint = center;
                return true;
            }

            var dir = point - center;
            if (dir.magnitude >= radius)
            {
                closestPoint = point;
                return true;
            }

            if (dir == Vector3.zero)
            {
                closestPoint = point;
                return false;
            }

            closestPoint = center + dir.normalized * radius;
            return true;
        }

        /// <summary>
        /// 射线与球体相交
        /// </summary>
        /// <param name="rayOrigin">射线起点</param>
        /// <param name="rayDirection">射线方向法向量</param>
        /// <param name="sphereCenter">球体中心；圆心</param>
        /// <param name="sphereRadius">球体半径</param>
        /// <param name="distance">射线起点到相交点的距离</param>
        /// <returns>相交返回True；不相交返回False</returns>
        public static bool RaySphere(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, float sphereRadius, out float distance)
        {
            Vector3 m = rayOrigin - sphereCenter;            // 圆心到射线起点的向量
            float b = Vector3.Dot(m, rayDirection);    // b大于0，说明射线方向背向圆心
            float c = Vector3.Dot(m, m) - sphereRadius * sphereRadius;
            if (c > 0f && b > 0f)           // 如果射线起点在圆外，并且方向与到圆方向相反，则不相交
            {
                distance = 0;
                return false;
            }
            float discr = b * b - c;        // 判别式小于0
            if (discr < 0f)
            {
                distance = 0;
                return false;
            }
            distance = -b - Mathf.Sqrt(discr);     // -b-sqrt(b*b-c)表示从射线起点比较近的相交点
            if (distance < 0f)                     // 射线的长度t值应该为正方向，最小为0
            {
                distance = 0f;
            }
            return true;
        }

        #endregion

        #region 射线（非物理）

        /// <summary>
        /// 返回两条线之间交点或最近两个点
        /// http://wiki.unity3d.com/index.php?title=3d_Math_functions
        /// 两条非平行的直线可能相交：有一个交点，不相交：每条直线上各有一个点离对方直线最近
        /// </summary>
        /// <param name="linePoint1"></param>
        /// <param name="lineVec1"></param>
        /// <param name="linePoint2"></param>
        /// <param name="lineVec2"></param>
        /// <param name="closestPointLine1"></param>
        /// <param name="closestPointLine2"></param>
        /// <returns></returns>
        public static bool ClosestPointsOnTwoLines(Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2, out Vector3 closestPointLine1, out Vector3 closestPointLine2)
        {
            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            // 线不平行
            if (d != 0.0f)
            {
                Vector3 r = linePoint1 - linePoint2;
                float c = Vector3.Dot(lineVec1, r);
                float f = Vector3.Dot(lineVec2, r);

                float s = (b * f - c * e) / d;
                float t = (a * f - c * b) / d;

                closestPointLine1 = linePoint1 + lineVec1 * s;
                closestPointLine2 = linePoint2 + lineVec2 * t;

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 两条射线最近点
        /// </summary>
        /// <param name="InLineA"></param>
        /// <param name="InLineB"></param>
        /// <param name="OutPointA"></param>
        /// <param name="OutPointB"></param>
        /// <returns></returns>
        public static bool ClosestPointsOnTwoRay(Ray InLineA, Ray InLineB, out Vector3 OutPointA, out Vector3 OutPointB)
        {
            return ClosestPointsOnTwoLines(InLineA.origin, InLineA.direction, InLineB.origin, InLineB.direction, out OutPointA, out OutPointB);
        }

        /// <summary>
        /// 返回射线与平面的交点
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="planePosition"></param>
        /// <param name="planeNormal"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool GetPointRayOnPlane(Ray ray, Vector3 planePosition, Vector3 planeNormal, out Vector3 hit)
        {
            return GetPointRayOnPlane(ray, new Plane(planeNormal, planePosition), out hit);
        }

        /// <summary>
        /// 返回射线与平面的交点
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool GetPointRayOnPlane(Ray ray, Plane plane, out Vector3 hit)
        {
            if (plane.Raycast(ray, out float distance))
            {
                hit = ray.GetPoint(distance);
                return true;
            }
            else
            {
                hit = Vector3.zero;
                return false;
            }
        }

        /// <summary>
        /// 射线与网格
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="ray"></param>
        /// <param name="hit"></param>
        /// <returns></returns>
        public static bool MeshRaycast(Mesh mesh, Ray ray, out RaycastHitInfo hit)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            float dist = Mathf.Infinity;
            Vector3 point = Vector3.zero;
            Vector3 a, b, c;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                a = vertices[triangles[i + 0]];
                b = vertices[triangles[i + 1]];
                c = vertices[triangles[i + 2]];

                if (RayIntersectsTriangle(ray, a, b, c, ECulling.Front, out dist, out point))
                {
                    hit = new RaycastHitInfo();
                    hit.point = point;
                    hit.distance = Vector3.Distance(hit.point, ray.origin);
                    hit.normal = Vector3.Cross(b - a, c - a);
                    hit.triangle = new int[] { triangles[i], triangles[i + 1], triangles[i + 2] };
                    return true;
                }
            }

            hit = null;
            return false;
        }


        /// <summary>
        /// Returns true if a raycast intersects a triangle.
        /// http://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
        /// http://www.cs.virginia.edu/~gfx/Courses/2003/ImageSynthesis/papers/Acceleration/Fast%20MinimumStorage%20RayTriangle%20Intersection.pdf
        /// </summary>
        /// <param name="InRay"></param>
        /// <param name="InTriangleA"></param>
        /// <param name="InTriangleB"></param>
        /// <param name="InTriangleC"></param>
        /// <param name="cull"></param>
        /// <param name="OutDistance"></param>
        /// <param name="OutPoint"></param>
        /// <returns></returns>
        public static bool RayIntersectsTriangle(
            Ray InRay,
            Vector3 InTriangleA,
            Vector3 InTriangleB,
            Vector3 InTriangleC,
            ECulling cull,
            out float OutDistance,
            out Vector3 OutPoint)
        {
            OutDistance = 0f;
            OutPoint = Vector3.zero;

            Vector3 e1, e2;  //Edge1, Edge2
            Vector3 P, Q, T;
            float det, inv_det, u, v;
            float t;

            //Find vectors for two edges sharing V1
            e1 = InTriangleB - InTriangleA;
            e2 = InTriangleC - InTriangleA;

            //Begin calculating determinant - also used to calculate `u` parameter
            P = Vector3.Cross(InRay.direction, e2);

            //if determinant is near zero, ray lies in plane of triangle
            det = Vector3.Dot(e1, P);

            // NON-CULLING
            if ((cull == ECulling.Front && det < Mathf.Epsilon) || (det > -Mathf.Epsilon && det < Mathf.Epsilon)) return false;

            inv_det = 1f / det;

            //calculate distance from V1 to ray origin
            T = InRay.origin - InTriangleA;

            // Calculate u parameter and test bound
            u = Vector3.Dot(T, P) * inv_det;

            //The intersection lies outside of the triangle
            if (u < 0f || u > 1f) return false;

            //Prepare to test v parameter
            Q = Vector3.Cross(T, e1);

            //Calculate V parameter and test bound
            v = Vector3.Dot(InRay.direction, Q) * inv_det;

            //The intersection lies outside of the triangle
            if (v < 0f || u + v > 1f) return false;

            t = Vector3.Dot(e2, Q) * inv_det;

            if (t > Mathf.Epsilon)
            {
                //ray intersection
                OutDistance = t;

                OutPoint.x = (u * InTriangleB.x + v * InTriangleC.x + (1 - (u + v)) * InTriangleA.x);
                OutPoint.y = (u * InTriangleB.y + v * InTriangleC.y + (1 - (u + v)) * InTriangleA.y);
                OutPoint.z = (u * InTriangleB.z + v * InTriangleC.z + (1 - (u + v)) * InTriangleA.z);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 返回射线与游戏对象集中最近碰撞对象（无需碰撞体）
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static GameObject ObjectRaycast(Ray ray, IEnumerable<GameObject> objects)
        {
            float best = Mathf.Infinity;
            GameObject obj = null;

            foreach (GameObject go in objects)
            {
                var localRay = TransformRay(ray, go.transform);
                var renderer = go.GetComponent<Renderer>();

                if (renderer)
                {
                    if (renderer.bounds.IntersectRay(ray, out float distance))
                    {
                        var mf = go.GetComponent<MeshFilter>();

                        if (mf != null && mf.sharedMesh != null
                            && MeshRaycast(mf.sharedMesh, localRay, out RaycastHitInfo hit)
                            && hit.distance < best)
                        {
                            best = hit.distance;
                            obj = go;
                        }
                    }
                }
                else
                {
                    var bounds = new Bounds(Vector3.zero, Vector3.one);
                    bounds.center = go.transform.position;

                    if (bounds.IntersectRay(ray, out float distance) && distance < best)
                    {
                        best = distance;
                        obj = go;
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// 将射线变换为Transform空间内的射线
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Ray TransformRay(Ray ray, Transform transform)
        {
            var mat = transform.worldToLocalMatrix;
            Ray local = new Ray(mat.MultiplyPoint(ray.origin), mat.MultiplyVector(ray.direction));

            return local;
        }


        /// <summary>
        /// 射线与网格交叉碰撞数据结构
        /// </summary>
        [Name("射线检测信息")]
        public class RaycastHitInfo
        {
            public Vector3 point;
            public float distance;
            public Vector3 normal;
            public int[] triangle;
        }

        /// <summary>
        /// 裁剪面配置
        /// </summary>
        [Name("裁剪")]
        public enum ECulling
        {
            Back = 0x1,
            Front = 0x2,
            FrontBack = 0x4
        }
        #endregion

        /// <summary>
        /// 获取数字掩码（蒙版）索引列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns>返回[0,32)的值</returns>
        public static IEnumerable<int> ToMaskIndexes(this int value)
        {
            for (int i = 0; i < 32; i++)
            {
                if (((1 << i) & value) != 0)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// 转为具有连续期望比特位数的数字掩码（蒙版）；
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int ToMask(this int count)
        {
            if (count >= 32) return -1;
            if (count <= 0) return 0;

            var mask = 0;
            for (int i = 0; i < count; i++)
            {
                mask |= (1 << i);
            }
            return mask;
        }

        /// <summary>
        /// 获取数字掩码（蒙版）
        /// </summary>
        /// <param name="indexes">比特位索引值列表</param>
        /// <returns></returns>
        public static int GetMask(params int[] indexes)
        {
            if (indexes == null || indexes.Length == 0) return 0;
            var mask = 0;
            foreach (var i in indexes)
            {
                if (i >= 0 && i < 32)
                {
                    mask |= (1 << i);
                }
            }
            return mask;
        }

        /// <summary>
        /// 正弦缓入缓出
        /// </summary>
        /// <param name="time">时间点</param>
        /// <param name="duration">持续时间，总时长</param>
        /// <returns></returns>
        public static float EaseInOut(float time, float duration)
        {
            return -0.5f * (Mathf.Cos(Mathf.PI * time / duration) - 1.0f);
        }

        /// <summary>
        /// 正弦缓入缓出
        /// </summary>
        /// <param name="percent">百分比：期望值范围[0,1]</param>
        /// <returns></returns>
        public static float EaseInOut(float percent) => EaseInOut(percent, 1);
    }
}