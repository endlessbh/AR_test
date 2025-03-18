using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Rendering;

namespace XCSJ.EditorExtension.Base.XUnityEditor
{
    public static class HandlesHelper
    {
        /// <summary>
        /// 绘制箭头
        /// </summary>
        /// <param name="color"></param>
        /// <param name="direction"></param>
        /// <param name="center"></param>
        /// <param name="arrowSize"></param>
        /// <param name="outlineWidth"></param>
        public static void DrawArrow(Color color, Vector3 direction, Vector3 center, float arrowSize, float outlineWidth = 4)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var cross = direction.Rotate(90);
                Vector3[] array = new Vector3[4]
                {
                center + direction * arrowSize,
                center - direction * arrowSize + cross * arrowSize,
                center - direction * arrowSize - cross * arrowSize,
                default(Vector3)
                };
                array[3] = array[0];
                //Shader.SetGlobalColor("_HandleColor", color);
                HandleUtility_LinkType.ApplyWireMaterial();
                //GL.LoadOrtho();
                GL.Begin(GL.QUADS);
                GL.Color(color);
                GL.Vertex(array[0]);
                GL.Vertex(array[1]);
                GL.Vertex(array[2]);
                GL.Vertex(array[3]);
                GL.End();
                Handles.color = color;
                Handles.DrawAAPolyLine((Texture2D)Styles.connectionTexture.image, outlineWidth, array);
            }
        }

        /// <summary>
        /// 扩展向量旋转
        /// </summary>
        /// <param name="v"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector3 Rotate(this Vector3 v, float degrees)
        {
            float num = Mathf.Sin(degrees * 0.0174532924f);
            float num2 = Mathf.Cos(degrees * 0.0174532924f);
            float x = v.x;
            float y = v.y;
            v.x = num2 * x - num * y;
            v.y = num * x + num2 * y;
            return v;
        }
    }
}
