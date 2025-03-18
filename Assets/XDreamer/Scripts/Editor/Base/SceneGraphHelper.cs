using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using XCSJ.PluginCommonUtils;
using static Codice.CM.Common.CmCallContext;

namespace XCSJ.EditorExtension.Base
{
    public static class SceneGraphHelper
    {
        public static XGUIStyle labelStyle = new XGUIStyle(nameof(GUI.Label), style =>
        {
            style.fontSize = SceneGraphOption.weakInstance.labelSize;
            style.normal.textColor = SceneGraphOption.weakInstance.labelColor;
        });

        #region 使用 SceneGraphOption 首选项类

        public static void DrawArrow(Vector3 start, Vector3 end, string text, bool autoScaleOnView = true)
        {
            DrawArrow(start, end, text, SceneGraphOption.weakInstance.arrowColor, autoScaleOnView);
        }

        public static void DrawArrow(Vector3 position, Vector3 dir, float length, string text, bool autoScaleOnView = true)
        {
            DrawArrow(position, dir, length, text, SceneGraphOption.weakInstance.arrowColor, autoScaleOnView);
        }

        /// <summary>
        /// 绘制球体
        /// </summary>
        /// <param name="position">球位置</param>
        /// <param name="text">文字</param>
        /// <param name="color">颜色</param>
        /// <param name="autoScaleOnView">自动缩放大小</param>
        public static void DrawSphere(Vector3 position, string text, Color color, bool autoScaleOnView = true)
        {
            DrawSphere(position, Quaternion.identity, SceneGraphOption.weakInstance.sphereRadius, text, color, autoScaleOnView);
        }
        
        #endregion

        #region 箭头

        public static void DrawArrow(Vector3 start, Vector3 end, string text, Color color, bool autoScaleOnView = true)
        {
            var dir = end - start;
            DrawArrow(start, dir, dir.magnitude, text, color, autoScaleOnView);
        }

        public static void DrawArrow(Vector3 position, Vector3 dir, float length, string text, Color color, bool autoScaleOnView = true)
        {
            var l = length * GetScaleSize(autoScaleOnView, position);
            DrawColor(color, () => Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(dir, Vector3.up), l, EventType.Repaint));

            if (!string.IsNullOrEmpty(text))
            {
                Handles.Label(position + dir * l, text, labelStyle);
            }
        }

        #endregion

        #region 圆形

        /// <summary>
        /// 绘制实心并且带轮廓线的圆形
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="normal">法线</param>
        /// <param name="radius">半径</param>
        /// <param name="circleColor">圆形颜色</param>
        /// <param name="outlineColor">轮廓线颜色</param>
        public static void DrawSolidCircleWithOutline(Vector3 position, Vector3 normal, float radius, Color circleColor, Color outlineColor, bool autoScaleOnView = true)
        {
            DrawSolidCircle(position, normal, radius, circleColor, autoScaleOnView);
            DrawCircle(position, normal, radius, outlineColor, autoScaleOnView);
        }

        /// <summary>
        /// 绘制空心圆形
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="normal">圆形朝向</param>
        /// <param name="radius">半径</param>
        /// <param name="color">颜色</param>
        public static void DrawCircle(Vector3 position, Vector3 normal, float radius, Color color, bool autoScaleOnView = true)
        {
            DrawColor(color, () => Handles.DrawWireDisc(position, normal, radius * GetScaleSize(autoScaleOnView, position)));
        }

        /// <summary>
        /// 绘制实心圆形
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="normal">圆形朝向</param>
        /// <param name="radius">半径</param>
        /// <param name="color">颜色</param>
        public static void DrawSolidCircle(Vector3 position, Vector3 normal, float radius, Color color, bool autoScaleOnView = true)
        {
            DrawColor(color, ()=> Handles.DrawSolidDisc(position, normal, radius * GetScaleSize(autoScaleOnView, position)));
        }

        /// <summary>
        /// 绘制实心扇形
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="normal">扇形朝向</param>
        /// <param name="from">起始</param>
        /// <param name="angle">角度</param>
        /// <param name="radius">半径</param>
        /// <param name="color">颜色</param>
        /// <param name="autoScaleOnView">自动缩放</param>
        public static void DrawSolidArc(Vector3 position, Vector3 normal, Vector3 from, float angle, float radius, Color color, bool autoScaleOnView = true)
        {
            DrawColor(color, ()=> Handles.DrawSolidArc(position, normal, from, angle, radius * GetScaleSize(autoScaleOnView, position)));
        }


        /// <summary>
        /// 绘制线框扇形
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="normal">扇形朝向</param>
        /// <param name="from">起始</param>
        /// <param name="angle">角度</param>
        /// <param name="radius">半径</param>
        /// <param name="color">颜色</param>
        /// <param name="autoScaleOnView">自动缩放</param>
        public static void DrawWireArc(Vector3 position, Vector3 normal, Vector3 from, float angle, float radius, Color color, bool autoScaleOnView = true)
        {
            DrawColor(color, () => Handles.DrawWireArc(position, normal, from, angle, radius * GetScaleSize(autoScaleOnView, position)));
        }

        #endregion

        #region 球体

        /// <summary>
        /// 绘制球体
        /// </summary>
        /// <param name="position">球位置</param>
        /// <param name="rotation">球朝向</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="text">文字</param>
        public static void DrawSphere(Vector3 position, float radius, string text, Color color, bool autoScaleOnView = true)
        {
            DrawSphere(position, Quaternion.identity, radius, text, color, autoScaleOnView);
        }

        /// <summary>
        /// 绘制球体
        /// </summary>
        /// <param name="position">球位置</param>
        /// <param name="rotation">球朝向</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="text">文字</param>
        public static void DrawSphere(Vector3 position, Quaternion rotation, float radius, string text, Color color, bool autoScaleOnView = true)
        {
            if (Event.current.type == EventType.Repaint)
            {
                SetHandleColor(color, () =>
                {
                    Handles.SphereHandleCap(0, position, rotation, radius * GetScaleSize(autoScaleOnView, position), EventType.Repaint);
                });

                if (!string.IsNullOrEmpty(text))
                {
                    Handles.Label(position, text, labelStyle);
                }
            }
        }

        /// <summary>
        /// 箭头为球体的箭头
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="direction">方向</param>
        /// <param name="length">长度</param>
        /// <param name="radius">大小</param>
        /// <param name="sphereColor">球颜色</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="text">文本</param>
        public static void DrawSphereArrow(Vector3 start, Vector3 direction, float length, float radius, string text, Color sphereColor, Color lineColor, bool autoScaleOnView = true)
        {
            var end = start + direction.normalized * length;
            if (autoScaleOnView)
            {
                end = start + direction.normalized * GetScaleSize(autoScaleOnView, end);
            }

            DrawSphereArrow(start, start + direction.normalized * length, radius, text, sphereColor, lineColor, autoScaleOnView);
        }

        /// <summary>
        /// 箭头为球体的箭头
        /// </summary>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <param name="radius">大小</param>
        /// <param name="sphereColor">球颜色</param>
        /// <param name="lineColor">线颜色</param>
        /// <param name="text">文本</param>
        public static void DrawSphereArrow(Vector3 start, Vector3 end, float radius, string text, Color sphereColor, Color lineColor, bool autoScaleOnView = true)
        {
            DrawColor(lineColor, () => Handles.DrawLine(start, end));

            DrawSphere(end, Quaternion.identity, radius, text, sphereColor, autoScaleOnView);
        }

        #endregion

        #region 私有辅助函数

        public static float GetScaleSize(bool autoScaleOnView, Vector3 position) => autoScaleOnView ? HandleUtility.GetHandleSize(position) : 1;

        private static float ViewSize(Vector3 position) => HandleUtility.GetHandleSize(position);

        public static void DrawColor(Color color, Action drawAction)
        {
            if (Event.current.type == EventType.Repaint)
            {
                SetHandleColor(color, () =>
                {
                    drawAction();
                });
            }
        }

        private static void SetHandleColor(Color color, Action drawAction)
        {
            var orgColor = Handles.color;
            Handles.color = color;
            drawAction();
            Handles.color = orgColor;
        }

        #endregion

        #region 绘制限定距离和角度
        
        /// <summary>
        /// 绘制限定平移直线
        /// </summary>
        /// <param name="initPoint"></param>
        /// <param name="direction"></param>
        /// <param name="currentValue"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        public static void DrawLimitLine(Vector3 initPoint, Vector3 direction, float currentValue, float minValue, float maxValue)
        {
            // 初始点
            DrawSphere(initPoint, "0", Color.yellow);

            // 当前移动点
            if (!Mathf.Approximately(currentValue, 0))
            {
                DrawSphere(initPoint + direction * currentValue, currentValue.ToString(), Color.red);
            }

            var lineColor = SceneGraphOption.weakInstance.lineColor;
            var smallSphereRadius = SceneGraphOption.weakInstance.smallSphereRadius;

            // 最小值
            if (!Mathf.Approximately(minValue, 0))
            {
                DrawSphereArrow(initPoint, direction, minValue, smallSphereRadius, minValue.ToString(), lineColor, lineColor);
            }

            // 最大值
            if (!Mathf.Approximately(maxValue, 0))
            {
                DrawSphereArrow(initPoint, direction, maxValue, smallSphereRadius, maxValue.ToString(), lineColor, lineColor);
            }
        }

        #endregion
    }
}
