using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Extensions;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginTools.Base;

namespace XCSJ.PluginTools.Cameras
{
    /// <summary>
    /// 相机背景幕布
    /// </summary>
    [Name("相机背景幕布")]
    [XCSJ.Attributes.Icon(EIcon.Image)]
    public class CameraBackdrop : ToolMBForUnityEditorSelection
    {
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "相机背景幕布";

        /// <summary>
        /// 相机
        /// </summary>
        [Name("相机")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [ComponentPopup]
        public Camera _camera;

        /// <summary>
        /// 距离
        /// </summary>
        [Name("距离")]
        [Tip("相机距离视窗四角定点的距离", "The distance from the camera to the four corners of the window")]
        public float _distance = 500;

        private MeshFilter _meshFilter;
        private Mesh _mesh;

        /// <summary>
        /// 验证数据
        /// </summary>
        public void OnValidate()
        {
            if (_camera)
            {
                _distance = Mathf.Clamp(_distance, _camera.nearClipPlane, _camera.farClipPlane);
            }
            FindCamera();
            ResetMesh();
        }

        /// <summary>
        /// 启用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            FindCamera();
            ResetMesh();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            if (!_camera || !_mesh) return;

            _mesh.vertices = CalcVertices();
            _mesh.RecalculateNormals();
        }

        private void FindCamera()
        {
            if (!_camera) _camera = GetComponentInParent<Camera>();
        }

        private void ResetMesh()
        {
            if (!_meshFilter) _meshFilter = gameObject.XGetOrAddComponent<MeshFilter>();
            if (_meshFilter)
            {
                _mesh = new Mesh();
                _meshFilter.XModifyProperty(() =>
                {
                    _meshFilter.mesh = _mesh;
                });
                _mesh.XModifyProperty(() =>
                {
                    _mesh.name = this.name;
                    _mesh.vertices = CalcVertices();
                    _mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
                    _mesh.colors = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };
                    _mesh.triangles = new int[] { 0, 3, 1, 0, 2, 3 };
                });
            }

            //获取或添加网格渲染器
            gameObject.XGetOrAddComponent<MeshRenderer>();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            FindCamera();

            ResetMesh();
        }

        /// <summary>
        /// 计算定点；数组索引与视窗点关系依次为：0 左下，1 右下，2 左上，3 右上；
        /// </summary>
        /// <returns></returns>
        private Vector3[] CalcVertices()
        {
            if (!_camera) return null;
            var transform = this.transform;
            return new Vector3[] {
                transform.InverseTransformPoint(_camera.ViewportToWorldPoint(new Vector3(0, 0, _distance))),
                transform.InverseTransformPoint(_camera.ViewportToWorldPoint(new Vector3(1, 0, _distance))),
                transform.InverseTransformPoint(_camera.ViewportToWorldPoint(new Vector3(0, 1, _distance))),
                transform.InverseTransformPoint(_camera.ViewportToWorldPoint(new Vector3(1, 1, _distance))) };
        }

        /// <summary>
        /// 获取幕布矩形，基于世界坐标的值；数组索引与视窗点关系依次为：0 左下，1 右下，2 左上，3 右上；
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetBackdropRect(Camera camera, float distance)
        {
            if (!camera) return null;
            return new Vector3[] {
                camera.ViewportToWorldPoint(new Vector3(0, 0, distance)),
                camera.ViewportToWorldPoint(new Vector3(1, 0, distance)),
                camera.ViewportToWorldPoint(new Vector3(0, 1, distance)),
                camera.ViewportToWorldPoint(new Vector3(1, 1, distance)) };
        }
    }
}
