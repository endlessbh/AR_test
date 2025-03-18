using UnityEngine;
using UnityEngine.EventSystems;
using XCSJ.Attributes;
using XCSJ.Extension.Base.Inputs;
using XCSJ.PluginXGUI.Base;

namespace XCSJ.PluginXGUI.Windows.ImageBrowers
{
    /// <summary>
    /// 拖拽动作
    /// </summary>
    [Name("拖拽动作")]
    [RequireComponent(typeof(RectTransform))]
    public class DragAction : View, IDragHandler//IPointerUpHandler, IPointerDownHandler, 
    {
        /// <summary>
        /// PC平台图片移动速度
        /// </summary>
        [Name("PC平台图片移动速度")]
        public float pcMoveSpeed = 1.0f;

        /// <summary>
        /// 安卓平台图片移动速度
        /// </summary>
        [Name("安卓平台图片移动速度")]
        public float androidMoveSpeed = 1.0f;

        /// <summary>
        /// IOS平台图片移动速度
        /// </summary>
        [Name("IOS平台图片移动速度")]
        public float iosMoveSpeed = 1.0f;

        private Vector3 initPos;           

        private float moveSpeed ;

        /// <summary>
        /// Start函数
        /// </summary>
        void Start()
        {
            this.initPos = rectTransform.localPosition;
            
            switch (Application.platform)
            {
                case RuntimePlatform.Android: moveSpeed = androidMoveSpeed; break;
                case RuntimePlatform.IPhonePlayer: moveSpeed = iosMoveSpeed; break;
                default: moveSpeed = pcMoveSpeed; break;
            }
        }

        /// <summary>
        /// 重置物体的位置, 将物体的position重置为初始位置
        /// </summary>
        public void ResetPosition()
        {
            rectTransform.localPosition = this.initPos;
        }

        /// <summary>
        /// 当拖拽时
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            if (IsDragEnable()==false) return;

            // 新位置
            var offset = eventData.delta * moveSpeed;
            var newPosition = rectTransform.localPosition + new Vector3(offset.x, offset.y);

            // 左右上下移动的限定
            newPosition = LimitRectTransformPosition(rectTransform, initPos, newPosition);

            rectTransform.localPosition = newPosition;
        }

        private bool IsDragEnable()
        {
            switch (XInput.touchCount)
            {
                case 0:
                case 1: return true;
                default: return false;
            }
        }

        /// <summary>
        /// // 左右上下移动的限定Rectransform的坐标
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="initPos"></param>
        /// <param name="newPosition"></param>
        /// <returns></returns>
        public static Vector3 LimitRectTransformPosition(RectTransform rectTransform, Vector3 initPos, Vector3 newPosition)
        {
            Vector2 scaleValue = (rectTransform.localScale - Vector3.one) / 2;
            Vector2 scaleRange = Vector2.Scale(rectTransform.sizeDelta, scaleValue);

            newPosition.x = Mathf.Clamp(newPosition.x, initPos.x - scaleRange.x, initPos.x + scaleRange.x);
            newPosition.y = Mathf.Clamp(newPosition.y, initPos.y - scaleRange.y, initPos.y + scaleRange.y);

            return newPosition;
        }
    }
}
