using System;
using UnityEngine;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Tweens
{
    /// <summary>
    /// Gizmos绘制类
    /// </summary>
    public static class XGizmos
    {
        /// <summary>
        /// 绘制路径；以线性方式绘制，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;
        /// </summary>
        /// <param name="path">路径关键点</param>
        public static void DrawPath(params Vector3[] path) => DrawLine(path, GUI.color);

        /// <summary>
        /// 绘制路径；以线性方式绘制，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;
        /// </summary>
        /// <param name="color">线条颜色</param>
        /// <param name="path">路径关键点</param>
        public static void DrawPath(Color color, params Vector3[] path) => DrawLine(path, color);

        /// <summary>
        /// 绘制路径；
        /// </summary>
        /// <param name="lineType">线类型</param>
        /// <param name="path">路径关键点</param>
        public static void DrawPath(ELineType lineType, params Vector3[] path) => DrawPath(lineType, path, GUI.color);

        /// <summary>
        /// 绘制路径；
        /// </summary>
        /// <param name="lineType">线类型</param>
        /// <param name="path">路径关键点</param>
        /// <param name="color">线颜色</param>
        public static void DrawPath(ELineType lineType, Vector3[] path, Color color)
        {
            switch (lineType)
            {
                case ELineType.Bezier:
                    {
                        DrawBezierLine(path, color);
                        break;
                    }
                case ELineType.Liner:
                    {
                        DrawLine(path, color);
                        break;
                    }
                case ELineType.BezierPolygon:
                    {
                        if (path == null || path.Length == 0) return;
                        var path01 = MathU.BezierPolygonPathControlPointGenerator(path);
                        DrawLine(path01, color);
                        break;
                    }
            }
        }

        /// <summary>
        /// 绘制线条；以线性方式绘制，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;
        /// </summary>
        /// <param name="line">线条关键点</param>
        /// <param name="color">线条颜色</param>
        public static void DrawLine(Vector3[] line, Color color)
        {
            if (line == null || line.Length == 0) return;

            Gizmos.color = color;
            for (int i = 0; i < line.Length - 1; i++)
            {
                Gizmos.DrawLine(line[i], line[i + 1]); ;
            }
        }

        /// <summary>
        /// 绘制Bezier线条;根据关键点,补间生成贝塞尔曲线;即两个关键点之间,曲线连接;补间插值时,拐弯位置的百分比非常密集；
        /// </summary>
        /// <param name="line">线条关键点</param>
        /// <param name="color">线条颜色</param>
        public static void DrawBezierLine(Vector3[] line, Color color)
        {
            if (line == null || line.Length == 0) return;

            Vector3[] vector3s = MathU.BezierPathControlPointGenerator(line);

            //线条绘制
            Vector3 prevPt = MathU.BezierInterp(vector3s, 0);
            Gizmos.color = color;
            int SmoothAmount = line.Length * 20;
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                Vector3 currPt = MathU.BezierInterp(vector3s, pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }

        /// <summary>
        /// 路径控制点生成器
        /// </summary>
        /// <param name="lineType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Vector3[] PathControlPointGenerator(ELineType lineType, Vector3[] path)
        {
            switch (lineType)
            {
                case ELineType.Bezier: return MathU.BezierPathControlPointGenerator(path);
                case ELineType.Liner: return path;
                case ELineType.BezierPolygon: return MathU.BezierPolygonPathControlPointGenerator(path);
                default:
                    {
                        throw new ArgumentException("无效的线条类型: " + lineType, nameof(lineType));
                    }
            }
        }

        /// <summary>
        /// 补间
        /// </summary>
        /// <param name="lineType"></param>
        /// <param name="pathControlPoints">路径控制点</param>
        /// <param name="percent">百分比</param>
        /// <returns></returns>
        public static Vector3 Interp(ELineType lineType, Vector3[] pathControlPoints, double percent)
        {
            switch (lineType)
            {
                case ELineType.Bezier:
                    {
                        return MathU.BezierInterp(pathControlPoints, percent);
                    }
                case ELineType.Liner:
                case ELineType.BezierPolygon:
                    {
                        return MathU.LinerInterp(pathControlPoints, percent);
                    }
                default:
                    {
                        throw new ArgumentException("无效的线条类型:" + lineType, nameof(lineType));
                    }
            }
        }
    }
}
