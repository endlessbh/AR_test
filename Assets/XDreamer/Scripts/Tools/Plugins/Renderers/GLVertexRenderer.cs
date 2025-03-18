using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Tweens;
using XCSJ.PluginCommonUtils;
using XCSJ.Tools;
using ZXing.QrCode.Internal;
using XCSJ.PluginCommonUtils.Tools;

namespace XCSJ.PluginTools.Renderers
{
    [Tool("渲染器", rootType = typeof(ToolsManager))]
    [DisallowMultipleComponent]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    [Name("GL顶点渲染器")]
    public class GLVertexRenderer : ToolRenderer
    {
        [Name("材质")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material material;       

        [Name("模式")]
        [EnumPopup]
        public EGLMode mode = EGLMode.LINES;

        [Name("线条类型")]
        public enum ELineType
        {
            [Name("线段")]
            LineSegments,

            [Name("多边形线")]
            PolyLine,

            [Name("贝塞尔")]
            Bezier,

            [Name("贝塞尔多边形")]
            BezierPolygon,
        }

        [Name("线条类型")]
        [HideInSuperInspector(nameof(mode), EValidityCheckType.NotEqual, EGLMode.LINES)]
        [EnumPopup]
        public ELineType lineType = ELineType.PolyLine;

        [Name("顶点类型")]
        public enum EVertexType
        {
            [Name("变换")]
            [Tip("使用变换的世界位置坐标", "Use transformed world position coordinates")]
            Transform,

            [Name("点")]
            [Tip("基于世界坐标的点")]
            Point,
        }

        [Name("顶点类型")]
        [EnumPopup]
        public EVertexType vertexType = EVertexType.Transform;

        [Name("变换列表")]
        [HideInSuperInspector(nameof(vertexType), EValidityCheckType.NotEqual, EVertexType.Transform)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<Transform> transforms = new List<Transform>();

        [Name("点列表")]
        [HideInSuperInspector(nameof(vertexType), EValidityCheckType.NotEqual, EVertexType.Point)]
        [ValidityCheck(EValidityCheckType.NotNullOrEmpty)]
        public List<Vector3> points = new List<Vector3>();

        [Name("点类型")]
        [EnumPopup]
        public EPointType pointType = EPointType.World;

        [Name("点类型")]
        public enum EPointType
        {
            [Name("世界")]
            World,

            [Name("本地")]
            Local,

            [Name("本地旋转")]
            LocalRotation,
        }

        [Name("颜色列表")]
        public Color[] colors = new Color[] { };

        [Name("缺省颜色")]
        public Color defaultColor = Color.white;

        public Color GetColor(int i) => (i < 0 || i >= colors.Length) ? defaultColor : colors[i];

        public List<Vector3> vertexes
        {
            get
            {
                switch (vertexType)
                {
                    case EVertexType.Transform: return transforms.Where(t => t).ToList(t => t.position);
                    case EVertexType.Point:
                        {
                            switch (pointType)
                            {
                                case EPointType.Local:
                                    {
                                        var t = this.transform;
                                        return points.ToList(p => p + t.position);
                                    }
                                case EPointType.LocalRotation:
                                    {
                                        var t = this.transform;
                                        return points.ToList(p => t.rotation * p + t.position);
                                    }
                                case EPointType.World:
                                default:
                                    {
                                        return points;
                                    }
                            }
                        }
                    default: throw new ArgumentException("无效顶点类型:" + vertexType.ToString(), nameof(vertexType));
                }
            }
        }

        private void DrawVertexes()
        {
            var vertexes = this.vertexes;
            for (int i = 0; i < vertexes.Count; i++)
            {
                GL.Color(GetColor(i));
                GL.Vertex(vertexes[i]);
            }
        }

        public void OnRenderObject()
        {
            if (GLHelper.Begin(material, transform, mode))
            {
                switch (mode)
                {
                    case EGLMode.LINES:
                        {
                            switch (lineType)
                            {
                                case ELineType.LineSegments:
                                    {
                                        DrawVertexes();
                                        break;
                                    }
                                case ELineType.PolyLine:
                                    {
                                        var vertexes = this.vertexes;
                                        for (int i = 1; i < vertexes.Count; i++)
                                        {
                                            GL.Color(GetColor(i - 1));
                                            GL.Vertex(vertexes[i - 1]);

                                            GL.Color(GetColor(i));
                                            GL.Vertex(vertexes[i]);
                                        }
                                        break;
                                    }
                                case ELineType.Bezier:
                                    {
                                        GLHelper.DrawBezierLine(vertexes.ToArray(), colors, defaultColor);
                                        break;
                                    }
                                case ELineType.BezierPolygon:
                                    {
                                        var path01 = MathU.BezierPolygonPathControlPointGenerator(vertexes.ToArray());
                                        GLHelper.DrawLine(path01, colors, defaultColor);
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            DrawVertexes();
                            break;
                        }
                }

                GLHelper.End();
            }          
        }

#if UNITY_EDITOR

        public void OnDrawGizmosSelected()
        {
            if (Application.isPlaying) return;

            switch (mode)
            {
                case EGLMode.LINES:
                    {
                        var vertexes = this.vertexes;
                        switch (lineType)
                        {
                            case ELineType.LineSegments:
                                {
                                    var count = vertexes.Count;
                                    vertexes = vertexes.GetRange(0, count % 2 == 0 ? count : count - 1);
                                    UnityEditor.Handles.DrawLines(vertexes.ToArray());
                                    break;
                                }
                            case ELineType.PolyLine:
                                {
                                    UnityEditor.Handles.DrawPolyLine(vertexes.ToArray());
                                    break;
                                }
                            case ELineType.Bezier:
                                {
                                    XGizmos.DrawBezierLine(vertexes.ToArray(), defaultColor);
                                    break;
                                }
                            case ELineType.BezierPolygon:
                                {
                                    var path01 = MathU.BezierPolygonPathControlPointGenerator(vertexes.ToArray());
                                    XGizmos.DrawLine(path01, defaultColor);
                                    break;
                                }
                        }

                        break;
                    }
            }
        }

#endif
    }
}
