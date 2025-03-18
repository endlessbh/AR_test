using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;

namespace XCSJ.PluginTools.LineNotes
{
    [Name("批注-3D")]
    public class UGUILineNote3D : LineNote3D, INoteText
    {
        [Name("UI对象")]
        [ValidityCheck(EValidityCheckType.NotNull)]
        public RectTransform rectTransform;

        ///// <summary>
        ///// UGUI不可见时仍绘制线
        ///// </summary>
        //[Name("UGUI不可见时仍绘制线")]
        //public bool _drawLineWhenUGUIDisable = true;

        [Name("保持UGUI对象在屏幕内")]
        [HideInInspector]
        public bool _keepUGUIInScreen = false;

        [Name("屏幕空白边界")]
        [HideInSuperInspector(nameof(_keepUGUIInScreen), EValidityCheckType.Equal, false)]
        [Min(0)]
        public float _spaceToScreenBorder = 0;

        /// <summary>
        /// 内屏幕矩形
        /// </summary>        
        private Rect smallScreenRect;

        /// <summary>
        /// 屏幕对角线长
        /// </summary>
        private float maxScreenLengthOfRect;

        protected override bool valid => rectTransform && base.valid;

        /// <summary>
        /// 显示
        /// </summary>
        protected override bool display
        {
            get
            {
                //if (_drawLineWhenUGUIDisable)
                //{
                //    return base.display;
                //}
                //else
                {
                    return uguiInScreen && base.display;
                }
            }
        }

        /// <summary>
        /// 标注文本
        /// </summary>
        public string noteText 
        {
            get
            {
                if (rectTransform && rectTransform.GetComponentInChildren<Text>() is Text text)
                {
                    return text.text;
                }
                return default;
            }
            set
            {
                if (rectTransform && rectTransform.GetComponentInChildren<Text>() is Text text)
                {
                    text.text = value;
                }
            }
        }

        private bool uguiInScreen = false;

        /// <summary>
        /// 共享UGUI的缓存字典
        /// </summary>
        protected static Dictionary<RectTransform, List<UGUILineNote3D>> node3DDict = new Dictionary<RectTransform, List<UGUILineNote3D>>();

        private List<UGUILineNote3D> note3DList = new List<UGUILineNote3D>();

        protected override void Start()
        {
            base.Start();

            // 不能为负值
            if (_spaceToScreenBorder < 0) _spaceToScreenBorder = 0;

            if (rectTransform)
            {
                if (!node3DDict.TryGetValue(rectTransform, out List<UGUILineNote3D> note3Ds))
                {
                    node3DDict[rectTransform] = note3Ds = new List<UGUILineNote3D>();
                }
                note3DList = note3Ds;
                note3DList.Add(this);
            }
            else
            {
                Log.Error(CommonFun.Name(typeof(UGUILineNote3D), nameof(rectTransform)) + "不能为空!");
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            if (rectTransform)
            {
                rectTransform.gameObject.SetActive(false);
            }
            uguiInScreen = false;
        }

        /// <summary>
        /// 跟新数据
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            if (valid && display)
            {
                SetUGUIPosition();
            }
        }

        private void SetUGUIPosition()
        {
            // 设置UGUI位置
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            smallScreenRect = new Rect(rectTransform.sizeDelta / 2, screenSize - rectTransform.sizeDelta);
            maxScreenLengthOfRect = screenSize.magnitude;
            if (smallScreenRect.Contains(endPointScreen))
            {
                uguiInScreen = true;

                // 终点变换为视区坐标
                rectTransform.anchorMax = rectTransform.anchorMin = cam.WorldToViewportPoint(endPoint);
            }
            else
            {
                if (_keepUGUIInScreen)
                {
                    // 采用单位向量来计算屏幕点，防止终点投影到屏幕上变成负数的错误计算
                    Vector3 shortEndPoint = beginPoint + (endPoint - beginPoint).normalized;
                    Vector2 shortEndPointScreen = cam.WorldToScreenPoint(shortEndPoint);
                    Vector2 longEndPointScreen = beginPointScreen + (shortEndPointScreen - beginPointScreen).normalized * maxScreenLengthOfRect;

                    // 线段与屏幕内边矩形相交
                    Vector2 crossPoint = Vector2.zero;
                    uguiInScreen = LineRectIntersection(beginPointScreen, longEndPointScreen, smallScreenRect, ref crossPoint);
                    if (uguiInScreen)
                    {
                        rectTransform.anchorMax = rectTransform.anchorMin = cam.ScreenToViewportPoint(new Vector3(crossPoint.x, crossPoint.y));
                    }
                }
                else
                {
                    uguiInScreen = false;
                }
            }
        }

        /// <summary>
        /// 因为Update中跟新了UGUI,所以不能将计算UGUI位置放置在Update，将两帧隔开
        /// </summary>
        private void CalculateUGUIEndPoint()
        {
            // 终点超出屏幕
            Vector3 screenPoint = cam.WorldToViewportPoint(endPoint);
            uguiInScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0;
            if (uguiInScreen)
            {
                // 0=左上，1=右上
                Vector3[] rectPoints = new Vector3[4];
                rectTransform.GetWorldCorners(rectPoints);

                // 起始点在UGUI矩形内
                Rect uguiScreenRect = new Rect(rectPoints[0], rectPoints[2] - rectPoints[0]);
                Vector2 crossPoint = Vector2.zero;
                if (LineRectIntersection(beginPointScreen, uguiScreenRect.center, uguiScreenRect, ref crossPoint))
                {
                    float ratio = (crossPoint - beginPointScreen).magnitude / (uguiScreenRect.center - beginPointScreen).magnitude;

                    drawEndPoint = (endPoint - beginPoint) * ratio + beginPoint;
                }
            }

        }

        protected override void OnRenderObject()
        {
            if (valid)
            {
                CalculateUGUIEndPoint();
            }

            // 所有共用UGUI的注释都无效的时候隐藏UGUI 
            if (rectTransform && note3DList != null)
            {
                rectTransform.gameObject.SetActive(note3DList.Exists(n => n.valid && n.display));
            }

            base.OnRenderObject();
        }

        private static bool LineRectIntersection(Vector2 lineStartPoint, Vector2 lineEndPoint, Rect rectangle, ref Vector2 result)
        {
            Vector2 minXLinePoint = lineStartPoint.x <= lineEndPoint.x ? lineStartPoint : lineEndPoint;
            Vector2 maxXLinePoint = lineStartPoint.x <= lineEndPoint.x ? lineEndPoint : lineStartPoint;
            Vector2 minYLinePoint = lineStartPoint.y <= lineEndPoint.y ? lineStartPoint : lineEndPoint;
            Vector2 maxYLinePoint = lineStartPoint.y <= lineEndPoint.y ? lineEndPoint : lineStartPoint;

            double rectMaxX = rectangle.xMax;
            double rectMinX = rectangle.xMin;
            double rectMaxY = rectangle.yMax;
            double rectMinY = rectangle.yMin;

            if (rectMaxX >= minXLinePoint.x && rectMaxX <= maxXLinePoint.x)
            {
                double m = (maxXLinePoint.y - minXLinePoint.y) / (maxXLinePoint.x - minXLinePoint.x);

                double intersectionY = ((rectMaxX - minXLinePoint.x) * m) + minXLinePoint.y;

                if (minYLinePoint.y <= intersectionY && intersectionY <= maxYLinePoint.y
                    && rectMinY <= intersectionY && intersectionY <= rectMaxY)
                {
                    result = new Vector2((float)rectMaxX, (float)intersectionY);

                    return true;
                }
            }

            if (rectMinX >= minXLinePoint.x && rectMinX <= maxXLinePoint.x)
            {
                double m = (maxXLinePoint.y - minXLinePoint.y) / (maxXLinePoint.x - minXLinePoint.x);

                double intersectionY = ((rectMinX - minXLinePoint.x) * m) + minXLinePoint.y;

                if (minYLinePoint.y <= intersectionY && intersectionY <= maxYLinePoint.y
                    && rectMinY <= intersectionY && intersectionY <= rectMaxY)
                {
                    result = new Vector2((float)rectMinX, (float)intersectionY);

                    return true;
                }
            }

            if (rectMaxY >= minYLinePoint.y && rectMaxY <= maxYLinePoint.y)
            {
                double rm = (maxYLinePoint.x - minYLinePoint.x) / (maxYLinePoint.y - minYLinePoint.y);

                double intersectionX = ((rectMaxY - minYLinePoint.y) * rm) + minYLinePoint.x;

                if (intersectionX >= minXLinePoint.x && intersectionX <= maxXLinePoint.x
                    && rectMinX <= intersectionX && intersectionX <= rectMaxX)
                {
                    result = new Vector2((float)intersectionX, (float)rectMaxY);

                    return true;
                }
            }

            if (rectMinY >= minYLinePoint.y && rectMinY <= maxYLinePoint.y)
            {
                double rm = (maxYLinePoint.x - minYLinePoint.x) / (maxYLinePoint.y - minYLinePoint.y);

                double intersectionX = ((rectMinY - minYLinePoint.y) * rm) + minYLinePoint.x;

                if (minXLinePoint.x <= intersectionX && intersectionX <= maxXLinePoint.x
                    && rectMinX <= intersectionX && intersectionX <= rectMaxX)
                {
                    result = new Vector2((float)intersectionX, (float)rectMinY);

                    return true;
                }
            }

            return false;
        }
    }

    public interface INoteText
    {
        string noteText { get; set; }
    }

    [Name("UGUI锚点")]
    public enum EUGUIAnchor
    {
        [Name("无")]
        None,

        [Name("左上")]
        LeftTop,

        [Name("左中")]
        LeftMiddle,

        [Name("左下")]
        LeftBottom,

        [Name("中上")]
        MiddleTop,

        [Name("中心")]
        Center,

        [Name("中下")]
        MiddleBottom,

        [Name("右上")]
        RightTop,

        [Name("右中")]
        RightMiddle,

        [Name("右下")]
        RightBottom,
    }
}
