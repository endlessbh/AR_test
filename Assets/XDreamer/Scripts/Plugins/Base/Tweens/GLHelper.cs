using System;
using System.Collections.Generic;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.Extension.Base.Tweens
{
    public static class GLHelper
    {
        public static Color GetColor(List<Color> colors, int i, Color defaultColor) => (colors == null || i < 0 || i >= colors.Count) ? defaultColor : colors[i];

        public static Color GetColor(Color[] colors, int i, Color defaultColor) => (colors == null || i < 0 || i >= colors.Length) ? defaultColor : colors[i];

        public static void DrawLine(Vector3 from, Vector3 to, Color fromColor, Color toColor)
        {
            GL.Color(fromColor);
            GL.Vertex(from);
            GL.Color(toColor);
            GL.Vertex(to);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            GL.Color(color);
            GL.Vertex(from);
            GL.Vertex(to);
        }

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            GL.Vertex(from);
            GL.Vertex(to);
        }

        /// <summary>
        /// 绘制路径；以线性方式绘制，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;
        /// </summary>
        /// <param name="path">路径关键点</param>
        public static void DrawPath(params Vector3[] path) => DrawLine(path, GUI.color);

        /// <summary>
        /// 绘制线条；以线性方式绘制，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;
        /// </summary>
        /// <param name="line">线条关键点</param>
        /// <param name="color">线条颜色</param>
        public static void DrawLine(Vector3[] line, Color color)
        {
            if (line == null) return;
            var count = line.Length - 1;
            if (count < 1) return;

            GL.Color(color);
            for (int i = 0; i < count; i++)
            {
                GL.Vertex(line[i]);
                GL.Vertex(line[i + 1]);
            }
        }

        /// <summary>
        /// 绘制线条；以线性方式绘制，即根据关键点,补间生成连续折线;即两个关键点之间,直线连接;
        /// </summary>
        /// <param name="line">线条关键点</param>
        /// <param name="colors">线条关键点颜色</param>
        /// <param name="defaultColor">线条颜色</param>
        public static void DrawLine(Vector3[] line, Color[] colors, Color defaultColor)
        {
            if (line == null) return;
            var count = line.Length - 1;
            if (count < 1) return;

            for (int i = 0; i < count; i++)
            {
                GL.Color(GetColor(colors, i, defaultColor));
                GL.Vertex(line[i]);

                GL.Color(GetColor(colors, i + 1, defaultColor));
                GL.Vertex(line[i + 1]);
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
            int SmoothAmount = line.Length * 20;
            GL.Color(color);
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                Vector3 currPt = MathU.BezierInterp(vector3s, pm);
                DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }

        /// <summary>
        /// 绘制Bezier线条;根据关键点,补间生成贝塞尔曲线;即两个关键点之间,曲线连接;补间插值时,拐弯位置的百分比非常密集；
        /// </summary>
        /// <param name="line">线条关键点</param>
        /// <param name="color">线条颜色</param>
        public static void DrawBezierLine(Vector3[] line, Color[] colors, Color defaultColor)
        {
            if (line == null || line.Length == 0) return;

            Vector3[] vector3s = MathU.BezierPathControlPointGenerator(line);

            //线条绘制
            Vector3 prevPt = MathU.BezierInterp(vector3s, 0);
            int SmoothAmount = line.Length * 20;
            GL.Color(GetColor(colors, 0, defaultColor));
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                Vector3 currPt = MathU.BezierInterp(vector3s, pm);
                if (i % 20 == 0)
                {
                    DrawLine(currPt, prevPt, GetColor(colors, i / 20, defaultColor));
                }
                else
                {
                    DrawLine(currPt, prevPt);
                }
                prevPt = currPt;
            }
        }

        /// <summary>
        /// 开始GL绘制
        /// </summary>
        /// <param name="material"></param>
        /// <param name="transform"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool Begin(Material material, Transform transform, int mode, int pass = 0)
        {
            //检查材质有效性
            if (!material || !transform) return false;

            //设置着色器
            material.SetPass(pass);

            //渲染入栈  在Push——Pop之间写GL代码
            GL.PushMatrix();

            //矩阵相乘，将物体坐标转化为世界坐标
            GL.MultMatrix(transform.localToWorldMatrix);

            //绘制开始
            GL.Begin(mode);

            return true;
        }

        /// <summary>
        /// 开始GL绘制
        /// </summary>
        /// <param name="material"></param>
        /// <param name="transform"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static bool Begin(Material material, Transform transform, EGLMode mode, int pass = 0) => Begin(material, transform, (int)mode, pass);

        /// <summary>
        /// 结束GL绘制
        /// </summary>
        public static void End()
        {
            //绘制结束
            GL.End();

            //渲染出栈
            GL.PopMatrix();
        }
    }

    [Name("GL模式")]
    public enum EGLMode
    {
        [Name("线条")]
        LINES = 1,

        [Name("线条带")]
        LINE_STRIP = 2,

        [Name("三角形")]
        TRIANGLES = 4,

        [Name("三角形带")]
        TRIANGLE_STRIP = 5,

        [Name("四边形")]
        QUADS = 7
    }
}
