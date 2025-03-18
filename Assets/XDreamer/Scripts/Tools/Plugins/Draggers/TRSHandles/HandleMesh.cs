using UnityEngine;
using System;
using System.Collections.Generic;

namespace XCSJ.PluginTools.Draggers.TRSHandles
{
    /// <summary>
    ///  三轴模型
    /// </summary>
    public static class HandleMesh
    {
        /// <summary>
        /// x轴颜色
        /// </summary>
        public static Color xAxis = Color.red;

        /// <summary>
        /// y轴颜色
        /// </summary>
        public static Color yAxis = Color.green;

        /// <summary>
        /// z轴颜色
        /// </summary>
        public static Color zAxis = Color.blue;

        /// <summary>
        /// 平面透明度
        /// </summary>
        public const float AphlaPlane = 0.25f;

        /// <summary>
        /// x平面颜色
        /// </summary>
        public static Color xPlane = new Color(1, 0, 0, AphlaPlane);

        /// <summary>
        /// y平面颜色
        /// </summary>
        public static Color yPlane = new Color(0, 1, 0, AphlaPlane);

        /// <summary>
        /// z平面颜色
        /// </summary>
        public static Color zPlane = new Color(0, 0, 1, AphlaPlane);

        /// <summary>
        /// 设置轴与平面颜色
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        /// <param name="zAxis"></param>
        /// <param name="xPlane"></param>
        /// <param name="yPlane"></param>
        /// <param name="zPlane"></param>
        public static void SetColor(Color xAxis, Color yAxis, Color zAxis, Color xPlane, Color yPlane, Color zPlane)
        {
            HandleMesh.xAxis = xAxis;
            HandleMesh.yAxis = yAxis;
            HandleMesh.zAxis = zAxis;

            HandleMesh.xPlane = xPlane;
            HandleMesh.yPlane = yPlane;
            HandleMesh.zPlane = zPlane;
        }

        /// <summary>
        /// 恢复三轴原始颜色
        /// </summary>
        public static void RecoverColor()
        {
            xAxis = Color.red;
            yAxis = Color.green;
            zAxis = Color.blue;

            xPlane = new Color(1, 0, 0, AphlaPlane);
            yPlane = new Color(0, 1, 0, AphlaPlane);
            zPlane = new Color(0, 0, 1, AphlaPlane);
        }

        /// <summary>
        /// 创建平移或缩放三轴线
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="transform"></param>
        /// <param name="scale"></param>
        /// <param name="cam"></param>
        /// <param name="handleSize"></param>
        /// <returns></returns>
        public static Mesh CreateAxisLineMesh(ref Mesh mesh, Transform transform, Vector3 scale, Camera cam, EAxis axis)
        {
            var viewDir = (transform && cam) ? HandleHelper.DirectionMask(transform, cam.transform.forward) : Vector3.zero;

            return CreateAxisLineMesh(ref mesh, viewDir, scale, axis);
        }

        /// <summary>
        /// 创建平移或缩放三轴线
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="viewDir"></param>
        /// <param name="scale"></param>
        /// <param name="handleSize"></param>
        /// <returns></returns>
        public static Mesh CreateAxisLineMesh(ref Mesh mesh, Vector3 viewDir, Vector3 scale, EAxis axis)
        {
            if (mesh==null)
            {
                mesh = new Mesh();
            }
            var v = new List<Vector3>();
            var n = new List<Vector3>();
            var u = new List<Vector2>();
            var c = new List<Color>();
            var t = new List<int>();

            if ((axis & EAxis.X) == EAxis.X)
            {
                v.AddRange(new Vector3[] { Vector3.zero, Vector3.right * scale.x });
                c.AddRange(new Color[] { xAxis, xAxis });
            }

            if ((axis & EAxis.Y) == EAxis.Y)
            {
                v.AddRange(new Vector3[] { Vector3.zero, Vector3.up * scale.y });
                c.AddRange(new Color[] { yAxis, yAxis });
            }

            if ((axis & EAxis.Z) == EAxis.Z)
            {
                v.AddRange(new Vector3[] { Vector3.zero, Vector3.forward * scale.z });
                c.AddRange(new Color[] { zAxis, zAxis });
            }

            for (int i = 0; i < v.Count; i++)
            {
                n.Add(viewDir);//Vector3.up
                u.Add(Vector2.zero);
                t.Add(i);
            }

            mesh.Clear();
            mesh.name = "Handle_TRANSLATE";
            mesh.vertices = v.ToArray();
            mesh.uv = u.ToArray();
            mesh.subMeshCount = 1;
            mesh.SetIndices(t.ToArray(), MeshTopology.Lines, 0);
            mesh.colors = c.ToArray();
            mesh.normals = n.ToArray();

            return mesh;
        }

        /// <summary>
        /// 返回旋转网格顶点
        /// </summary>
        /// <param name="segments"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vector3[][] GetRotationVertices(int segments, float radius)
        {
            var v_x = new Vector3[segments];
            var v_y = new Vector3[segments];
            var v_z = new Vector3[segments];

            for (int i = 0; i < segments; i++)
            {
                float rad = (i / (float)(segments - 1)) * 360f * Mathf.Deg2Rad;

                var cosValue = Mathf.Cos(rad) * radius;
                var sinValue = Mathf.Sin(rad) * radius;

                v_x[i].x = cosValue;
                v_x[i].z = sinValue;
                v_y[i].x = cosValue;
                v_y[i].y = sinValue;
                v_z[i].z = cosValue;
                v_z[i].y = sinValue;
            }

            return new Vector3[3][] { v_x, v_y, v_z };
        }

        /// <summary>
        /// 创建旋转线网格
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="segments"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Mesh CreateRotateMesh(ref Mesh mesh, int segments, float radius, EAxis axis)
        {
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            var verts = GetRotationVertices(segments, radius);

            var V = new Vector3[segments * 3];
            var n = new Vector3[segments * 3];
            var col = new Color[segments * 3];
            int[] triA = new int[segments];
            int[] triB = new int[segments];
            int[] triC = new int[segments];

            int a = 0, b = segments, c = segments + segments;

            for (int i = 0; i < segments; i++)
            {
                a = i;
                b = i + segments;
                c = i + segments + segments;

                V[a] = verts[0][i];
                col[a] = yAxis;
                n[a].x = V[a].x;
                n[a].z = V[a].z;

                V[b] = verts[1][i];
                col[b] = zAxis;
                n[b].x = V[b].x;
                n[b].y = V[b].y;

                V[c] = verts[2][i];
                col[c] = xAxis;
                n[c].z = V[c].z;
                n[c].y = V[c].y;

                triA[i] = a;
                triB[i] = b;
                triC[i] = c;
            }

            mesh.Clear();
            mesh.name = "Handle_ROTATE";
            mesh.vertices = V;
            mesh.uv = new Vector2[segments * 3];
            mesh.subMeshCount = 3;

            if ((axis & EAxis.Y) == EAxis.Y)
            {
                mesh.SetIndices(triA, MeshTopology.LineStrip, 0);
            }
            if ((axis & EAxis.Z) == EAxis.Z)
            {
                mesh.SetIndices(triB, MeshTopology.LineStrip, 1);
            }
            if ((axis & EAxis.X) == EAxis.X)
            {
                mesh.SetIndices(triC, MeshTopology.LineStrip, 2);
            }
            mesh.colors = col;
            mesh.normals = n;

            return mesh;
        }

        /// <summary>
        /// 创建圆盘网格
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="segments"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Mesh CreateDiscMesh(ref Mesh mesh, int segments, float radius)
        {
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            var v = new Vector3[segments];
            var n = new Vector3[segments];
            var u = new Vector2[segments];
            int[] t = new int[segments];

            for (int i = 0; i < segments; i++)
            {
                float rad = (i / (float)(segments - 1)) * 360f * Mathf.Deg2Rad;
                v[i] = new Vector3(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius, 0f);
                n[i] = v[i];
                u[i] = Vector2.zero;
                t[i] = i < segments - 1 ? i : 0;
            }

            mesh.Clear();
            mesh.name = "Disc";
            mesh.vertices = v;
            mesh.uv = u;
            mesh.normals = n;
            mesh.subMeshCount = 1;
            mesh.SetIndices(t, MeshTopology.LineStrip, 0);

            return mesh;
        }

        /// <summary>
        /// 创建三轴的顶部
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="transform"></param>
        /// <param name="scale"></param>
        /// <param name="cam"></param>
        /// <param name="cap"></param>
        /// <param name="handleSize"></param>
        /// <param name="capSize"></param>
        /// <returns></returns>
        public static Mesh CreateAxisTopMesh(ref Mesh mesh, Transform transform, Vector3 scale, Camera cam, Mesh cap, float handleSize, float capSize, EAxis axis)
        {
            if (mesh == null)
            {
                mesh = new Mesh();
            }
            var forward = cam ? cam.transform.forward : Vector3.forward;
            var viewDir = HandleHelper.DirectionMask(transform, forward);

            var v = new List<Vector3>();
            var nrm = new List<Vector3>();
            var c = new List<Color>();

            // 垂直X轴平面
            if (((axis & EAxis.Y) == EAxis.Y) && ((axis & EAxis.Z) == EAxis.Z))
            {
                v.AddRange(new Vector3[] {  new Vector3(0f, 0, 0),  new Vector3(0f, 0, viewDir.z), 
                    new Vector3(0f, viewDir.y, viewDir.z),  new Vector3(0f, viewDir.y, 0) });

                nrm.AddRange(new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right,});
                c.AddRange(new Color[] { xPlane, xPlane, xPlane, xPlane });
            }

            // 垂直Y轴平面
            if (((axis & EAxis.X) == EAxis.X) && ((axis & EAxis.Z) == EAxis.Z))
            {
                v.AddRange(new Vector3[] {  new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, viewDir.z),
                    new Vector3(viewDir.x, 0f, viewDir.z), new Vector3(viewDir.x, 0f, 0f) });

                nrm.AddRange(new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up });
                c.AddRange(new Color[] { yPlane, yPlane, yPlane, yPlane });
            }

            // 垂直Z轴平面
            if (((axis & EAxis.X) == EAxis.X) && ((axis & EAxis.Y) == EAxis.Y))
            {
                v.AddRange(new Vector3[] {  new Vector3(0f, 0f, 0f), new Vector3(viewDir.x, 0f, 0f),
                    new Vector3(viewDir.x, viewDir.y, 0f), new Vector3(0f, viewDir.y, 0f) });

                nrm.AddRange(new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,});
                c.AddRange(new Color[] { zPlane, zPlane, zPlane, zPlane });
            }

            for (int i = 0; i < v.Count; i++) v[i] *= handleSize;                      

            var u = new List<Vector2>();
            var t = new List<int>();

            for (int n = 0; n < v.Count; n += 4)
            {
                u.Add(Vector2.zero);
                u.Add(Vector2.zero);
                u.Add(Vector2.zero);
                u.Add(Vector2.zero);

                t.Add(n + 0);
                t.Add(n + 1);
                t.Add(n + 3);
                t.Add(n + 1);
                t.Add(n + 2);
                t.Add(n + 3);
            }

            // 将子模型设置为第二层
            var t2 = new List<int>();
            if (cap)
            {
                if ((axis & EAxis.X) == EAxis.X)
                {
                    var _coneRightMatrix = Matrix4x4.TRS(Vector3.right * scale.x, Quaternion.Euler(90f, 90f, 0f), Vector3.one * capSize);
                    MergeMesh(cap, _coneRightMatrix, v, nrm, u, c, t2, v.Count, xAxis);
                }

                if ((axis & EAxis.Y) == EAxis.Y)
                {
                    var _coneUpMatrix = Matrix4x4.TRS(Vector3.up * scale.y, Quaternion.identity, Vector3.one * capSize);
                    MergeMesh(cap, _coneUpMatrix, v, nrm, u, c, t2, v.Count, yAxis);
                }

                if ((axis & EAxis.Z) == EAxis.Z)
                {
                    var _coneForwardMatrix = Matrix4x4.TRS(Vector3.forward * scale.z, Quaternion.Euler(90f, 0f, 0f), Vector3.one * capSize);
                    MergeMesh(cap, _coneForwardMatrix, v, nrm, u, c, t2, v.Count, zAxis);
                }
            }

            mesh.Clear();
            mesh.subMeshCount = 2;
            mesh.vertices = v.ToArray();
            mesh.normals = nrm.ToArray();
            mesh.uv = u.ToArray();
            mesh.SetTriangles(t.ToArray(), 0);
            mesh.SetTriangles(t2.ToArray(), 1);
            mesh.colors = c.ToArray();
            return mesh;
        }

        /// <summary>
        /// 使用矩阵变换网格顶点、法线等信息并合并到传入链表中
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="matrix"></param>
        /// <param name="v"></param>
        /// <param name="n"></param>
        /// <param name="u"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <param name="indexOffset">顶点偏移量</param>
        /// <param name="color">颜色值</param>
        private static void MergeMesh(Mesh mesh, Matrix4x4 matrix, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<Color> c, List<int> t, int indexOffset, Color color)
        {
            var mv = mesh.vertices;
            var mn = mesh.normals;
            var mu = mesh.uv;
            var mc = mesh.colors;
            var mt = mesh.triangles;

            color.a = 1f;

            for (int i = 0; i < mv.Length; i++)
            {
                // 增加平移变换，因为顶点坐标在原点0上
                mv[i] = matrix * mv[i] + matrix.GetColumn(3);
                mn[i] = matrix * mn[i];
                mc[i] = color;
            }

            for (int i = 0; i < mt.Length; i++) mt[i] += indexOffset;

            v.AddRange(mv);
            c.AddRange(mc);
            n.AddRange(mn);
            u.AddRange(mu);
            t.AddRange(mt);
        }
    }

    [Flags]
    public enum EAxis
    {
        X = 0x1,
        Y = 0x2,
        Z = 0x4
    }

}
