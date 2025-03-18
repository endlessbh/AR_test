using System;
using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Collections;
using XCSJ.Extension.Base.Maths;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.LineNotes;
using XCSJ.PluginTools.Effects;

namespace XCSJ.PluginTools.Points
{
    /// <summary>
    /// 线段可视化组件
    /// </summary>
    [Name("线段可视化")]
    [RequireManager(typeof(ToolsManager))]
    public class SegmentVisual : InteractProvider
    {
        /// <summary>
        /// 线段
        /// </summary>
        [Name("线段")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Segment _segment;

        /// <summary>
        /// 数据可视化类型
        /// </summary>
        [Flags]
        public enum EDataVisualType
        {
            [Name("长度")]
            Length = 1 << 0,

            [Name("角度")]
            Angle = 1 << 1,

            [Name("面积")]
            Area = 1 << 2,
        }

        /// <summary>
        /// 数据可视化类型
        /// </summary>
        [Name("数据可视化类型")]
        [EnumPopup]
        public EDataVisualType _dataVisualType = EDataVisualType.Length;

        /// <summary>
        /// 线类型
        /// </summary>
        [Group("线设置", textEN ="line Settings")]
        [Name("线类型")]
        [EnumPopup]
        public ERenderMode _lineRenderMode = ERenderMode.LineRenderer;

        /// <summary>
        /// 线样式
        /// </summary>
        [Name("线样式")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public LineStyle _lineStyle;

        /// <summary>
        /// 线渲染器模版
        /// </summary>
        [Name("线渲染器模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_lineRenderMode), EValidityCheckType.NotEqual, ERenderMode.LineRenderer)]
        public LineRenderer _lineRendererTemplate = null;

        /// <summary>
        /// 动态创建出来的线渲染器
        /// </summary>
        private LineRenderer lineRenderer
        {
            get
            {
                if (!_lineRenderer && _lineRendererTemplate)
                {
                    _lineRenderer = Clone<LineRenderer>(_lineRendererTemplate);
                }
                return _lineRenderer;
            }
        }

        private LineRenderer _lineRenderer = null;

        /// <summary>
        /// 文本类型
        /// </summary>
        public enum ETextType
        {
            [Name("文本网格")]
            TextMesh,
        }

        [Group("数据文本设置", textEN = "Data Text Settings")]
        /// <summary>
        /// 文本类型
        /// </summary>
        [Name("文本类型")]
        [EnumPopup]
        public ETextType _textType = ETextType.TextMesh;

        /// <summary>
        /// 文本网格模版
        /// </summary>
        [Name("文本网格模版")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        [HideInSuperInspector(nameof(_textType), EValidityCheckType.NotEqual, ETextType.TextMesh)]
        public TextMesh _textMeshTemplate;

        #region 长度设定

        /// <summary>
        /// 长度小数点位数
        /// </summary>
        [Group("长度设置", textEN = "Length Settings")]
        /// <summary>
        /// 长度文本位置偏移量
        /// </summary>
        [Name("长度文本位置偏移量")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Length)]
        public Vector3 _lengthTextPositionOffset = new Vector3(0, 0.05f, 0);

        [Name("长度小数点位数")]
        [Min(0)]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Length)]
        public int _lengthDecimals = 2;

        public enum ELengthUnit
        {
            [Name("米")]
            m,

            [Name("厘米")]
            cm,

            [Name("毫米")]
            mm,
        }

        /// <summary>
        /// 长度单位
        /// </summary>
        [Name("长度单位")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Length)]
        public ELengthUnit _lengthUnit = ELengthUnit.m;

        /// <summary>
        /// 长度文本颜色
        /// </summary>
        [Name("长度文本颜色")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Length)]
        public Color _lengthTextColor = Color.white;

        /// <summary>
        /// 显示总长度
        /// </summary>
        [Name("显示总长度")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Length)]
        public bool _displayTotalLength = true;

        /// <summary>
        /// 总长度文本颜色
        /// </summary>
        [Name("总长度文本颜色")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag | EValidityCheckType.Or, EDataVisualType.Length, nameof(_displayTotalLength), EValidityCheckType.False)]
        public Color _totalLengthTextColor = Color.yellow;

        private void SetTextMeshLength(Point point)
        {
            if (!point._nextPoint || point._nextPoint == point) return;

            var textMesh = textMeshPool.Alloc();

            textMesh.transform.position = point.nextCenter + _lengthTextPositionOffset;

            AlignWithDirection(textMesh.transform, point.nextDirection);

            var distance = ConvertToUnitLenght(point.nextDistance);
            textMesh.text = distance.ToString("F" + _lengthDecimals) + CommonFun.Name(_lengthUnit);
            textMesh.color = _lengthTextColor;
        }

        private void SetTextMeshTotalLength(Segment segment)
        {
            if (!segment) return;

            var textMesh = textMeshPool.Alloc();

            textMesh.transform.position = segment.center;

            var distance = ConvertToUnitLenght(segment.length);
            textMesh.text = distance.ToString("F" + _lengthDecimals) + CommonFun.Name(_lengthUnit);
            textMesh.color = _totalLengthTextColor;
        }

        private float ConvertToUnitLenght(float length)
        {
            switch (_lengthUnit)
            {
                case ELengthUnit.cm: length *= 100; break;
                case ELengthUnit.mm: length *= 1000; break;
            }
            return length;
        }

        #endregion

        #region 角度设定

        /// <summary>
        /// 角度小数点位数
        /// </summary>
        [Group("角度设置", textEN = "Angle Settings")]
        /// <summary>
        /// 角度文本位置偏移量
        /// </summary>
        [Name("角度文本位置偏移量")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Angle)]
        public Vector3 _angleTextPositionOffset = new Vector3(0, 0.05f, 0);

        [Name("角度小数点位数")]
        [Min(0)]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Angle)]
        public int _angleDecimals = 2;

        /// <summary>
        /// 角度位置偏移量
        /// </summary>
        [Name("角度位置偏移量")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Angle)]
        public float _angleOffset = 0.75f;

        /// <summary>
        /// 角度单位
        /// </summary>
        [Name("角度单位")]
        [Min(0)]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Angle)]
        public string _angleUnit = "度";

        /// <summary>
        /// 角度文本颜色
        /// </summary>
        [Name("角度文本颜色")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Angle)]
        public Color _angleTextColor = Color.yellow;

        /// <summary>
        /// 启用角度网格
        /// </summary>
        [Name("启用角度网格")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Angle)]
        public bool _enableAngleMesh = true;

        /// <summary>
        /// 角度网格颜色
        /// </summary>
        [Name("角度网格颜色")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag | EValidityCheckType.Or, EDataVisualType.Angle, nameof(_enableAngleMesh), EValidityCheckType.False)]
        public Color _angleMeshColor = Color.green;

        /// <summary>
        /// 角度网格材质
        /// </summary>
        [Name("角度网格材质")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag | EValidityCheckType.Or, EDataVisualType.Area, nameof(_enableAngleMesh), EValidityCheckType.False)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _angleMeshMaterial = null;

        private WorkObjectPool<MeshFilter> angleMeshFilterPool
        {
            get
            {
                if (_angleMeshFilterPool == null)
                {
                    _angleMeshFilterPool = new WorkObjectPool<MeshFilter>();

                    _angleMeshFilterPool.Init(() =>
                    {
                        var angleGO = UnityObjectHelper.CreateGameObject(name + "_angle");
                        angleGO.XSetParent(_segment.gameObject);
                        angleGO.AddComponent<MeshRenderer>().material = _angleMeshMaterial;
                        var meshFilter = angleGO.AddComponent<MeshFilter>();
                        var mesh = new Mesh();
                        mesh.vertices = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };
                        mesh.triangles = new int[3] { 0, 1, 2 };
                        meshFilter.mesh = mesh;
                        
                        return meshFilter;
                    },
                        item =>
                        {
                            item.gameObject.SetActive(true);
                        },
                        item =>
                        {
                            item.gameObject.SetActive(false);
                        },
                        item => item);
                }
                return _angleMeshFilterPool;
            }
        }
        private WorkObjectPool<MeshFilter> _angleMeshFilterPool;

        private void UpdateAngleMesh(Point point)
        {
            if (!_enableAngleMesh) return;

            var angleMeshFilter = angleMeshFilterPool.Alloc();

            // 顶点
            var vertices = new Vector3[3];
            var distance = Mathf.Min(point.previousDistance, point.nextDistance)/4; // 较短边的1/4
            vertices[0] = point.position + point.previousDirection.normalized * distance;
            vertices[1] = point.position;
            vertices[2] = point.position + point.nextDirection.normalized * distance;
            angleMeshFilter.mesh.vertices = vertices;

            // 修改颜色
            angleMeshFilter.mesh.colors = new Color[3] { _angleMeshColor, _angleMeshColor, _angleMeshColor };
        }

        private void SetTextMeshAngle(Point point)
        {
            var textMesh = textMeshPool.Alloc();

            // 将角度文本设置为第三边中心与角中心点的偏移量
            var dir = (point.previousNextCenter - point.position) * _angleOffset;
            textMesh.transform.position = point.position + dir + _angleTextPositionOffset;

            AlignWithDirection(textMesh.transform, point.previousNextDirection);

            textMesh.text = point.angle.ToString("F" + _angleDecimals) + _angleUnit;
            textMesh.color = _angleTextColor;
        }

        #endregion

        #region 面积设定

        /// <summary>
        /// 面积小数点位数
        /// </summary>
        [Group("面积设置", textEN = "Area Settings")]
        /// <summary>
        /// 面积文本位置偏移量
        /// </summary>
        [Name("面积文本位置偏移量")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Area)]
        public Vector3 _areaTextPositionOffset = new Vector3(0, 0.05f, 0);
        
        [Name("面积小数点位数")]
        [Min(0)]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Area)]
        public int _areaDecimals = 3;

        /// <summary>
        /// 面积单位
        /// </summary>
        [Name("面积单位")]
        [Min(0)]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Area)]
        public string _areaUnit = "平方米";

        /// <summary>
        /// 面积文本颜色
        /// </summary>
        [Name("面积文本颜色")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Area)]
        public Color _areaTextColor = Color.red;

        /// <summary>
        /// 启用面积网格
        /// </summary>
        [Name("启用面积网格")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag, EDataVisualType.Area)]
        public bool _enableAreaMesh = true;

        /// <summary>
        /// 面积网格颜色
        /// </summary>
        [Name("面积网格颜色")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag | EValidityCheckType.Or, EDataVisualType.Area, nameof(_enableAreaMesh), EValidityCheckType.False)]
        public Color _areaMeshColor = Color.grey;

        /// <summary>
        /// 面积网格材质
        /// </summary>
        [Name("面积网格材质")]
        [HideInSuperInspector(nameof(_dataVisualType), EValidityCheckType.NotHasFlag | EValidityCheckType.Or, EDataVisualType.Area, nameof(_enableAreaMesh), EValidityCheckType.False)]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public Material _areaMeshMaterial = null;

        private MeshFilter areaMeshFilter;
        private Mesh areaMesh;
        private int lastPointCount = -1;

        /// <summary>
        /// 更新面积网格模型
        /// </summary>
        private void UpdateAreaMesh()
        {
            if (!areaMesh)
            {
                var areaGameObject = UnityObjectHelper.CreateGameObject(name + "_area");
                areaGameObject.XSetParent(_segment.gameObject);

                areaMeshFilter = areaGameObject.AddComponent<MeshFilter>();
                areaGameObject.AddComponent<MeshRenderer>().material = _areaMeshMaterial;
            }

            var pointCount = _segment.pointCount;
            if (lastPointCount!= pointCount)
            {
                areaMesh = new Mesh();
                areaMeshFilter.mesh = areaMesh;
            }


            // 顶点
            var vertices = new Vector3[pointCount];
            Array.Copy(_segment.positions, vertices, pointCount);
            areaMesh.vertices = vertices;

            // 三角面
            var triangleCount = pointCount - 2;
            var triangles = new int[triangleCount * 3];
            for (int i = 0; i < triangleCount; i++)
            {
                triangles[i * 3] = i;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = pointCount - 1;
            }
            areaMesh.triangles = triangles;

            // 颜色
            var colors = new Color[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                colors[i] = _areaMeshColor;
            }
            areaMesh.colors = colors;
        }

        private void SetTextMeshArea(float area, Vector3 position)
        {
            var textMesh = textMeshPool.Alloc();
            textMesh.transform.position = position + _areaTextPositionOffset;

            textMesh.text = area.ToString("F" + _areaDecimals) + _areaUnit;
            textMesh.color = _areaTextColor;
        }

        #endregion

        /// <summary>
        /// 文字网格缓存
        /// </summary>
        public WorkObjectPool<TextMesh> textMeshPool
        {
            get
            {
                if (_textMeshPool == null)
                {
                    _textMeshPool = new WorkObjectPool<TextMesh>();
                    _textMeshTemplate.gameObject.SetActive(false);

                    _textMeshPool.Init(() =>
                    {
                        return Clone<TextMesh>(_textMeshTemplate);
                    },
                        item =>
                        {
                            item.gameObject.SetActive(true);
                        },
                        item =>
                        {
                            item.gameObject.SetActive(false);
                        },
                        item => item);
                }
                return _textMeshPool;
            }
        }

        /// <summary>
        /// 文字网格缓存
        /// </summary>
        private WorkObjectPool<TextMesh> _textMeshPool = null;

        private T Clone<T>(T template) where T : Component
        {
            var newGo = template.gameObject.XCloneObject<GameObject>();
            newGo.gameObject.XSetActive(true);
            newGo.transform.SetParent(_segment.transform);
            newGo.XSetUniqueName(template.name);
            return newGo.GetComponent<T>();
        }

        private void Update()
        {
            var pointCount = _segment.pointCount;
            if (pointCount == 0 || pointCount == 1) return;

            switch (_lineRenderMode)
            {
                case ERenderMode.GL:
                    {
                        if(lineRenderer) lineRenderer.gameObject.SetActive(false);
                        break;
                    }
                case ERenderMode.LineRenderer:
                    {
                        lineRenderer.gameObject.SetActive(true);
                        // 设置线渲染器
                        lineRenderer.loop = _segment.loop;
                        lineRenderer.positionCount = pointCount;
                        lineRenderer.SetPositions(_segment.positions);
                        lineRenderer.SetLineRendererStyle(_lineStyle);
                        break;
                    }
            }

            textMeshPool.Clear();
            angleMeshFilterPool.Clear();

            float area = 0; // 用于计算面积

            var firstPoint = _segment.firstPoint;
            var currentPoint = firstPoint;
            var lastPoint = _segment.lastPoint;
            var validAngleOrArea = pointCount >= 3;
            do
            {
                // 长度文本计算和设置
                if ((_dataVisualType & EDataVisualType.Length) == EDataVisualType.Length)
                {
                    SetTextMeshLength(currentPoint);
                }

                if (validAngleOrArea)
                {
                    // 角度文本计算和设置
                    if ((_dataVisualType & EDataVisualType.Angle) == EDataVisualType.Angle)
                    {
                        if (currentPoint.angleValid)
                        {
                            SetTextMeshAngle(currentPoint);
                            UpdateAngleMesh(currentPoint);
                        }
                    }

                    // 面积计算
                    if ((_dataVisualType & EDataVisualType.Area) == EDataVisualType.Area)
                    {
                        if (currentPoint != firstPoint && currentPoint != lastPoint)
                        {
                            area += MathLibrary.GetArea(firstPoint.position, currentPoint.position, lastPoint.position);
                        }
                    }
                }

                currentPoint = currentPoint._nextPoint;
            }
            while (currentPoint && currentPoint != firstPoint);

            // 设置总长度
            if (_displayTotalLength && pointCount>2 && (_dataVisualType & EDataVisualType.Length) == EDataVisualType.Length)
            {
                SetTextMeshTotalLength(_segment);
            }

            // 设置面积并更新面积网格
            var validArea = validAngleOrArea && (_dataVisualType & EDataVisualType.Area) == EDataVisualType.Area;
            if (validArea)
            {
                SetTextMeshArea(area, _segment.center);
                if(_enableAreaMesh) UpdateAreaMesh();
            }
            if (areaMeshFilter)
            {
                areaMeshFilter.gameObject.SetActive(validArea && _enableAreaMesh);
            }
        }

        /// <summary>
        /// GL画线需在当前函数内使用才有效
        /// </summary>
        protected virtual void OnRenderObject()
        {
            switch (_lineRenderMode)
            {
                case ERenderMode.GL:
                    {
                        CommonGL.LineStrip(_segment.positions, _lineStyle.color, CameraHelperExtension.currentCamera, _lineStyle.occlusion, _lineStyle.width, _segment.loop, _lineStyle.mat);
                        break;
                    }
            }
        }

        public void SetMeasureType(EMeasureType measureType)
        {
            switch (measureType)
            {
                case EMeasureType.Length:
                    {
                        _dataVisualType = SegmentVisual.EDataVisualType.Length;
                        _displayTotalLength = true;
                        break;
                    }
                case EMeasureType.Height:
                    {
                        _dataVisualType = SegmentVisual.EDataVisualType.Length;
                        break;
                    }
                case EMeasureType.Angle:
                    {
                        _dataVisualType = SegmentVisual.EDataVisualType.Angle;
                        _displayTotalLength = false;
                        break;
                    }
                case EMeasureType.Area:
                    {
                        _dataVisualType = SegmentVisual.EDataVisualType.Length | SegmentVisual.EDataVisualType.Area;
                        _displayTotalLength = false;
                        break;
                    }
                case EMeasureType.LengthAngle:
                    {
                        _dataVisualType = SegmentVisual.EDataVisualType.Length | SegmentVisual.EDataVisualType.Angle;
                        _displayTotalLength = true;
                        break;
                    }
                case EMeasureType.LengthAngleArea:
                    {
                        _dataVisualType = SegmentVisual.EDataVisualType.Length | SegmentVisual.EDataVisualType.Angle | SegmentVisual.EDataVisualType.Area;
                        _displayTotalLength = false;
                        break;
                    }
            }
        }

        public static void AlignWithDirection(Transform transform, Vector3 direction)
        {
            // 依据起点终点构建旋转量，让变换水平角度与线段对齐
            var forward = Vector3.Cross(direction, Vector3.up);
            if (forward == Vector3.zero)
            {
                forward = Vector3.forward;
            }
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

            // 旋转90度将变换放平，负向Z轴朝上
            transform.Rotate(new Vector3(90, 0, 0));

            // 绕自身Y轴旋转，将变换平贴在线段上
            var zAngle = Vector3.SignedAngle(transform.right, direction, transform.up);
            transform.Rotate(new Vector3(0, zAngle, 0));
        }
    }
}
