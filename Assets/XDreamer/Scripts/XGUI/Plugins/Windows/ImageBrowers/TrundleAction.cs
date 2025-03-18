using UnityEngine;
using XCSJ.Attributes;
using XCSJ.PluginCommonUtils;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 滚动动作
    /// </summary>
    [Name("滚动动作")]
    [RequireComponent(typeof(RectTransform))]
    public class TrundleAction : View
    {
        /// <summary>
        /// 基础缩放速度
        /// </summary>
        [Name("基础缩放速度")]
        public float baseScaleSpeed = 0.05f;

        /// <summary>
        /// PC平台图片缩放速度
        /// </summary>
        [Name("PC平台图片缩放速度")]
        public float pcSacleSpeed = 1.0f;

        /// <summary>
        /// 安卓平台图片缩放速度
        /// </summary>
        [Name("安卓平台图片缩放速度")]
        public float androidScaleSpeed = 1.0f;

        /// <summary>
        /// IOS平台图片缩放速度
        /// </summary>
        [Name("IOS平台图片缩放速度")]
        public float iosScaleSpeed = 1.0f;

        /// <summary>
        /// 缩放限定范围
        /// </summary>
        [Name("缩放限定范围")]
        [LimitRange(0.01f, 20f)]
        public Vector2 zoomLimitRange = new Vector2(1f, 8f);

        /// <summary>
        /// 手机灵敏度控制
        /// </summary>
        [Name("手机灵敏度控制")]
        public float sensitive = 0.1f;

        private Vector3 initZoom;          
        private Vector3 initPos;

        /// <summary>
        /// 开始
        /// </summary>
        protected void Start()
        {
            initZoom = rectTransform.localScale;
            initPos = rectTransform.localPosition;
        }

        /// <summary>
        /// 更新
        /// </summary>
        protected void Update()
        {
            if ( Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount != 2) return;

                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    Vector3 oldTouchPos1 = touch1.position - touch1.deltaPosition;
                    Vector3 oldTouchPos2 = touch2.position - touch2.deltaPosition;
                    Vector3 touchCenter = (oldTouchPos1 + oldTouchPos2)/ 2;

                    float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);
                    float lastTouchDistance = Vector2.Distance(oldTouchPos1, oldTouchPos2);
                    float distanceOffset = currentTouchDistance - lastTouchDistance;

                    // 缩放范围太小，判定为无效手势
                    if (Mathf.Abs(distanceOffset) < sensitive) return;

                    // 常量缩放
                    float scaleValue = distanceOffset > 0 ? baseScaleSpeed : -baseScaleSpeed;
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        ScalePanel(scaleValue * androidScaleSpeed);
                    }
                    else
                    {
                        ScalePanel(scaleValue * iosScaleSpeed);
                    }
                }
            }
            else
            {
                float ratiopc = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(ratiopc) > 0) ScalePanel(ratiopc * baseScaleSpeed * pcSacleSpeed);
            }
        }

        /// <summary>
        /// 重置物体的位置, 将物体的position重置为初始位置
        /// </summary>
        public void ResetScale()
        {
            rectTransform.localScale = this.initZoom;
        }

        /// <summary>
        /// 缩放面板
        /// </summary>
        /// <param name="scaleValue"></param>
        void ScalePanel(float scaleValue)
        {            
            Vector3 scaleOffset = new Vector3(scaleValue, scaleValue, 0);

            //调整缩小的情况下，调整位置
            if (scaleValue <= 0)
            {
                Vector3 newPos = initPos;
                float offset = rectTransform.localScale.x - 1;
                if (offset > 0)
                {
                    newPos = Vector3.Scale(rectTransform.localPosition / offset, scaleOffset) + rectTransform.localPosition;
                }
                rectTransform.localPosition = newPos;
            }

            // 设置缩放量
            Vector3 newScale = rectTransform.localScale + scaleOffset;
            newScale.x = Mathf.Clamp(newScale.x, zoomLimitRange.x, zoomLimitRange.y);
            newScale.y = Mathf.Clamp(newScale.y, zoomLimitRange.x, zoomLimitRange.y);
            rectTransform.localScale = newScale;
        }

        /// <summary>
        /// 缩放面板
        /// </summary>
        /// <param name="scaleValue"></param>
        /// <param name="touchCenter"></param>
        void ScalePanel(float scaleValue, Vector3 touchCenter)
        {
            Vector3 scaleOffset = new Vector3(scaleValue, scaleValue, 0);
            Vector3 oldlocalScale = rectTransform.localScale;

            // 设置缩放量
            //rectTransform.localScale.Scale(scaleOffset);
            Vector3 newScale = rectTransform.localScale + scaleOffset;
            newScale.x = Mathf.Clamp(newScale.x, zoomLimitRange.x, zoomLimitRange.y);
            newScale.y = Mathf.Clamp(newScale.y, zoomLimitRange.x, zoomLimitRange.y);
            rectTransform.localScale = newScale;

            // 设置位置
            Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 dir = rectTransform.position + center - touchCenter; 
            Vector3 moveOffset = Vector3.Scale(dir, (newScale- oldlocalScale));
            rectTransform.position += moveOffset;
        }
    }
}
