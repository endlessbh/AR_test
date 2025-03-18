using UnityEngine;
using XCSJ.Attributes;
using XCSJ.Extension.Interactions.Tools;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginsCameras;
using XCSJ.PluginTools.Effects;

namespace XCSJ.PluginTools.LineNotes
{
    /// <summary>
    /// 批注基类
    /// </summary>
    [Name("批注基类")]
    [Tip("目标对象中连线，在终点显示文字信息", "Display in target line information")]
    [RequireManager(typeof(ToolsManager))]
    public abstract class LineNote : InteractProvider
    {
        /// <summary>
        /// 渲染模式
        /// </summary>
        [Name("渲染模式")]
        [EnumPopup]
        public ERenderMode _RenderMode = ERenderMode.GL;

        /// <summary>
        /// 线样式
        /// </summary>
        [Name("线样式")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public LineStyle lineStyle;

        /// <summary>
        /// 可见距离
        /// </summary>
        [Name("可见距离")]
        [Tip("标注的目标对象离当前相机的可见距离超过该值，则批注不可见", "If the target object of the annotation is visible from the current camera beyond this value, the annotation is not visible")]
        [Range(0, 50000)]
        [Readonly(EEditorMode.Runtime)]
        public float visualDistance = 1000;

        /// <summary>
        /// 批注对象
        /// </summary>
        [Name("批注对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public GameObject target;

        public virtual Vector3 beginPoint => target ? target.transform.position : Vector3.zero;

        public virtual Vector3 endPoint { get; protected set; } = Vector3.zero;

        protected virtual Vector2 beginPointScreen { get; set; } = Vector2.zero;

        protected virtual Vector2 endPointScreen { get; set; } = Vector2.zero;

        protected virtual Vector3 drawEndPoint { get; set; } = Vector3.zero;

        protected bool isBeginPointInScreen = true;

        protected bool isEndPointInScreen = true;

        protected virtual bool valid => target && cam && lineStyle && !outVisualDistance;

        protected virtual bool display => targetVisable && isBeginPointInScreen;

        public bool targetVisable { get; set; } = true;

        protected Camera cam = null;

        private RendererVisibleListener visibleListener = null;

        /// <summary>
        /// 在可视距离之外
        /// </summary>
        private bool outVisualDistance = false;

        /// <summary>
        /// 重置函数
        /// </summary>
        protected virtual void Reset()
        {
            if (!lineStyle)
            {
                lineStyle = FindObjectOfType<LineStyle>() ?? gameObject.AddComponent<LineStyle>();
            }
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected virtual void Start()
        {
            if (target && target.GetComponent<Renderer>())
            {
                visibleListener = target.AddComponent<RendererVisibleListener>();
                if (visibleListener) visibleListener.Set(this);
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        protected void OnDestroy()
        {
            if (visibleListener) Destroy(visibleListener);
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected virtual void Update()
        {
            cam = CameraHelperExtension.currentCamera;

            if (target && cam)
            {
                outVisualDistance = (target.transform.position - cam.transform.position).sqrMagnitude > (visualDistance * visualDistance);
            }

            if (valid)
            {
                drawEndPoint = endPoint;

                beginPointScreen = cam.WorldToScreenPoint(beginPoint);
                endPointScreen = cam.WorldToScreenPoint(endPoint);

                isBeginPointInScreen = IsOutofScreen(beginPointScreen);
                isEndPointInScreen = IsOutofScreen(endPointScreen);
            }

            switch (_RenderMode)
            {
                case ERenderMode.LineRenderer:
                    {
                        var draw = canDrawLine;
                        if (draw)
                        {
                            lineRenderer.positionCount = linePointArray.Length;
                            lineRenderer.SetPositions(linePointArray);
                            lineRenderer.SetLineRendererStyle(lineStyle);
                        }
                        lineRenderer.XSetEnable(draw);
                        break;
                    }
            }
        }

        private Vector3[] linePointArray
        {
            get
            {
                _linePointArray[0] = beginPoint;
                _linePointArray[1] = drawEndPoint;
                return _linePointArray;
            }
        }

        private Vector3[] _linePointArray = new Vector3[2];

        protected bool canDrawLine => valid && display;

        protected LineRenderer lineRenderer
        {
            get
            {
                if (!_lineRenderer)
                {
                    _lineRenderer = this.XGetOrAddComponent<LineRenderer>();
                }
                return _lineRenderer;
            }
        }
        private LineRenderer _lineRenderer = null;

        /// <summary>
        /// 画线
        /// </summary>
        protected virtual void OnRenderObject()
        {
            // 数据有效，并且开始点在屏幕内
            if (canDrawLine)
            {
                switch (_RenderMode)
                {
                    case ERenderMode.GL:
                        {
                            CommonGL.LineStrip(linePointArray, lineStyle.color, cam, lineStyle.occlusion, lineStyle.width, false, lineStyle.mat);
                            break;
                        }
                }
            }
        }

        protected bool IsOutofScreen(Vector2 point)
        {
            return (point.x >= 0 && point.x < Screen.width && point.y >= 0 && point.y < Screen.height) ? true : false;
        }
    }

}
